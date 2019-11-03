﻿using Evolution.Sql.PgSqlTest.Modal;
using Evolution.Sql.TestCommon.Interface;
using Npgsql;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolution.Sql.PgSqlTest
{
    public class QueryTest : IQueryTest
    {
        private string connectionStr = @"Server=127.0.0.1;Port=5432;Database=blog;User Id=postgres;Password=";

        private string userInsSql = "insert into \"user\"(user_id, first_name, last_name, created_by, created_on)" +
            "                        values(@UserId, @FirstName, @LastName, @CreatedBy, @CreatedOn)";
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task QueryOne_With_Inline_Sql()
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                var userId1 = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId1,
                    FirstName = "Bruce",
                    LastName = "Lee",
                    UpdatedBy = "Locke",
                    CreatedOn = DateTime.Now
                };

                var result = connection.Sql(userInsSql)
                   .Execute(user);
                Assert.Greater(result, 0);

                var bruce = connection.Sql("select * from \"user\" where user_id = @UserId")
                    .Query<User>(new { UserId = userId1 })?.FirstOrDefault();
                Assert.IsNotNull(bruce);
                Assert.AreEqual(userId1, bruce.UserId);

                var userId2 = Guid.NewGuid();
                user = new User
                {
                    UserId = userId2,
                    FirstName = "Tom",
                    LastName = "Ren",
                    CreatedBy = "Locke",
                    CreatedOn = DateTime.Now
                };

                result = await connection.Sql("insert into \"user\"(user_id, first_name, last_name) values(@UserId, @FirstName, @LastName);")
                    .ExecuteAsync(user);
                Assert.Greater(result, 0);

                var tom = await connection.Sql("select * from \"user\" where user_id = @UserId").QueryOneAsync<User>(new { UserId = userId2 });
                Assert.IsNotNull(tom);
                Assert.AreEqual(userId2, tom.UserId);

                Assert.AreNotEqual(bruce.FirstName, tom.FirstName);
            }
        }

        [Test]
        public void QueryOne_With_StoredProcedure()
        {
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee",
                    CreatedBy = "Locke",
                    CreatedOn = DateTime.UtcNow
                };
                var result = connection.Sql(userInsSql).Execute(user);
                Assert.AreEqual(result, 1);

                var outPuts = new Dictionary<string, dynamic>();
                var userFromDb = connection.Sql($"call usp_user_get(@p_user_id, @p_total_count)")
                    .QueryOne<User>(new { p_user_id = userId, p_total_count = 0 }, outPuts);
                Assert.IsNotNull(userFromDb);
                Assert.AreEqual(userId, userFromDb.UserId);
                Assert.True(outPuts.ContainsKey("p_total_count"));
                Assert.Greater(outPuts["p_total_count"], 0);
            }
        }

        [Test]
        public void Query_With_Inline_Sql()
        {
            /*
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                sqlSession.Execute<User>("insert", user);

                var blog = new Blog
                {
                    Title = "this is a test post title",
                    Content = "this is a test post content",
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                };

                var outPuts = new Dictionary<string, dynamic>();
                sqlSession.Execute<Blog>("insert", blog, outPuts);
                var postId = outPuts["Id"];
                Assert.NotNull(postId);
                Assert.Greater(int.Parse(postId.ToString()), 0);
                sqlSession.Execute<Blog>("insert", blog, outPuts);
                postId = outPuts["Id"];
                Assert.NotNull(postId);
                Assert.Greater(int.Parse(postId.ToString()), 0);

                // query
                var blogs = sqlSession.Query<Blog>("getall", null);
                Assert.NotNull(blogs);
                Assert.Greater(blogs.ToList().Count, 1);
            }*/
        }

        [Test]
        public void Query_With_StoredProcedure()
        {
            //throw new NotImplementedException();
        }

        [Test]
        public void Get_Null_Value_From_DB_Property_Should_Set_Default_Value()
        {
            /*
            using (var connection = new NpgsqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                var result = sqlSession.Execute<User>("insert", user);
                Assert.Greater(result, 0);

                var userFromDb = sqlSession.QueryOne<User>("getPartialCol", new { UserId = userId });
                Assert.IsNotNull(userFromDb);
                Assert.AreEqual(userId, userFromDb.UserId);
                Assert.AreEqual(default(DateTime), userFromDb.CreatedOn);
            }*/
        }

        public Task Query_Column_Name_Contain_UnderScore_Can_Map_To_Property()
        {
            throw new NotImplementedException();
        }
    }
}
