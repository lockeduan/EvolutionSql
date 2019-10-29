﻿using Evolution.Sql.MySqlTest.Modal;
using Evolution.Sql.TestCommon.Interface;
using MySql.Data.MySqlClient;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Evolution.Sql.MySqlTest
{
    public class QueryTest : IQueryTest
    {
        private string connectionStr = @"server=localhost;user=root;database=blog;port=3306;password=root";
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public async Task QueryOne_With_Inline_Sql()
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                var userId1 = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId1,
                    FirstName = "Bruce",
                    LastName = "Lee",
                    CreatedOn = DateTime.Now
                };
                var result = connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .Execute(user);
                Assert.Greater(result, 0);

                var bruce = connection.Sql("select * from `user` where userid = @UserId")
                    .Query<User>(new { UserId = userId1 })?.FirstOrDefault();
                Assert.IsNotNull(bruce);
                Assert.AreEqual(userId1, bruce.UserId);

                var userId2 = Guid.NewGuid();
                user = new User
                {
                    UserId = userId2,
                    FirstName = "Tom",
                    LastName = "Ren",
                    CreatedOn = DateTime.Now
                };

                result = await connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .ExecuteAsync(user);
                Assert.Greater(result, 0);

                var tom = (await connection.Sql("select * from `user` where userid = @UserId").QueryAsync<User>(new { UserId = userId2 }))?.FirstOrDefault();
                Assert.IsNotNull(tom);
                Assert.AreEqual(userId2, tom.UserId);

                Assert.AreNotEqual(bruce.FirstName, tom.FirstName);
            }
        }

        [Test]
        public void QueryOne_With_StoredProcedure()
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                var result = connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .Execute(user);
                Assert.Greater(result, 0);

                var outPuts = new Dictionary<string, dynamic>();
                var userFromDb = connection.Procedure("usp_user_get").Query<User>(new { pUserId = userId }, outPuts)?.FirstOrDefault();
                Assert.IsNotNull(userFromDb);
                Assert.AreEqual(userId, userFromDb.UserId);
                Assert.True(outPuts.ContainsKey("totalCount"));
                Assert.Greater(outPuts["totalCount"], 0);
            }
        }

        [Test]
        public void Query_With_Inline_Sql()
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .Execute(user);

                var blog = new Blog
                {
                    Title = "this is a test post title",
                    Content = "this is a test post content",
                    CreatedBy = userId,
                    CreatedOn = DateTime.Now,
                    UpdatedOn = DateTime.Now
                };

                var outPuts = new Dictionary<string, dynamic>();
                connection.Procedure("usp_blog_ins").Execute(blog, outPuts);
                var postId = outPuts["BlogId"];
                Assert.NotNull(postId);
                Assert.Greater(int.Parse(postId.ToString()), 0);
                outPuts = new Dictionary<string, dynamic>();
                connection.Procedure("usp_blog_ins").Execute(blog, outPuts);
                postId = outPuts["BlogId"];
                Assert.NotNull(postId);
                Assert.Greater(int.Parse(postId.ToString()), 0);

                // query
                var blogs = connection.Sql("select * from `blog`").Query<Blog>(null);
                Assert.NotNull(blogs);
                Assert.Greater(blogs.ToList().Count, 1);
            }
        }

        [Test]
        public void Query_With_StoredProcedure()
        {
            //throw new NotImplementedException();
        }

        [Test]
        public async Task QueryAsyn_With_StoredProcedure()
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                var result = connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .Execute(user);
                Assert.Greater(result, 0);

                var outPuts = new Dictionary<string, dynamic>();
                var users = await connection.Procedure("usp_user_get").QueryAsync<User>(new { pUserId = userId }, outPuts);
                Assert.IsNotNull(users);
                Assert.True(outPuts.ContainsKey("totalCount"));
                Assert.Greater(outPuts["totalCount"], 0);
            }
        }

        [Test]
        public void Get_Null_Value_From_DB_Property_Should_Set_Default_Value()
        {
            using (var connection = new MySqlConnection(connectionStr))
            {
                var userId = Guid.NewGuid();
                var user = new User
                {
                    UserId = userId,
                    FirstName = "Bruce",
                    LastName = "Lee"
                };
                var result = connection.Sql("insert into `user`(UserId, FirstName, LastName) values(@UserId, @FirstName, @LastName);")
                    .Execute(user);
                Assert.Greater(result, 0);

                var userFromDb = connection.Sql("select UserId, FirstName, CreatedOn from `user` where userid = @userId").Query<User>(new { UserId = userId })?.FirstOrDefault();
                Assert.IsNotNull(userFromDb);
                Assert.AreEqual(userId, userFromDb.UserId);
                Assert.AreEqual(default(DateTime), userFromDb.CreatedOn);
            }
        }

    }
}
