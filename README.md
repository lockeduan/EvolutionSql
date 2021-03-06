# EvolutionSql
Simple dot net database access library, if you don't like Full-ORM framework such as EntityFramework, and you want to write your own sql and/or stored procedure, EvolutionSql is what you want.

by using EvolutionSql, it's very simple to execute either inline sql or stored procedure; EvolutionSql extend DbConnection with ONLY two methods ```Sql()``` and ```Procedure()```, for execute inline sql and stored procedure respectively.

## Database Tested
- [x] MySQL
- [x] SqlServer
- [x] PostgreSQL
- [x] SQLite

## Sample


###### insert sample with inline sql
```c#
  // anonymouse type as parameters
  var parameters = new
  {
      UserId = Guid.NewGuid(),
      FirstName = "Bruce",
      LastName = "Lee",
      CreatedOn = DateTime.Now
  };
  //
  var sql =@"INSERT INTO [user](UserId, FirstName, LastName, CreatedOn) 
              VALUES(@UserId, @FirstName, @LastName, @CreatedOn)";
  connection.Sql(sql).Execute(parameters);
```



###### get sample with stored procedure
  ```sql
  //Stored Procedure
  create procedure uspUserGet(
    @userId uniqueidentifier,
    @totalCount int out
  )
  as
  begin
    select * from [user] where userid = @userId
    select @totalCount = count(1) from [user]
  end
  ```
  
  ```c#
  var userFromDb = connection.Procedure("uspUserGet")
                             .QueryOne<User>(new { UserId = userId });
```
## Result mapping
When query from database, column name are auto mapped to property of modal, the following two pattern are legal, and both case-insensitive
1. column name and property name are same: FirstName -> FirstName
2. column name with under score: first_name -> FirstName

## Stored Procedure parameter mapping mysql sample
###### use anonymous type as parameter
```C#
    var userFromDb = connection.Procedure("usp_user_get")                    
                               .QueryOne<User>(new { pUserId = userId });

```
```SQL
  #Stored Procedure
  DROP PROCEDURE IF EXISTS usp_user_get;
  DELIMITER //
  CREATE PROCEDURE usp_user_get(
    IN pUserId CHAR(36)
  )
  BEGIN
    SELECT * FROM `user` 
      WHERE user_id = pUserId;
  END//
  DELIMITER ;
```

###### use explict parameter
```C#
    var parameters = new MySqlParameter[]
    {
        new MySqlParameter("p_user_id", userId),
        new MySqlParameter("p_first_name", "Bob"),
        new MySqlParameter("p_last_name", "Lee"),
        new MySqlParameter("p_updated_by", "system"),
        new MySqlParameter("p_updated_on", DateTime.Now)
    };
    connection.Procedure("usp_user_upd")
        .Execute(parameters);

```
```SQL
  #Stored Procedure
  DROP PROCEDURE IF EXISTS usp_user_upd;
  DELIMITER //
  CREATE PROCEDURE usp_user_upd(
    p_user_id CHAR(36),
    p_first_name VARCHAR(256),
    p_last_name VARCHAR(256),
    p_updated_by VARCHAR(256),
    p_updated_on DATETIME
  )
  BEGIN
    UPDATE `user`
      SET first_name = p_first_name,
        last_name = p_last_name,
              updated_by = p_updated_by,
              updated_on = p_updated_on
    WHERE user_id = p_user_id;
  END//
  DELIMITER ;
```
