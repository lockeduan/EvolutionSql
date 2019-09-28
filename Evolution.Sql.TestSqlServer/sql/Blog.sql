﻿
create database Blog;
go;
use Blog;


create table [User](
	UserId varchar(256) not null,
	FirstName nvarchar(256) not null,
	LastName nvarchar(256) not null,
	constraint pk_user primary key(UserId)
)

create table tag(
	Id int not null identity,
	[Name] nvarchar(256) not null,
	[Description] nvarchar(2000),
	constraint pk_tag primary key(Id)
)

create table Blog(
	Id int not null identity,
	Title nvarchar(1000) not null,
	Content nvarchar(max) not null,
	CreatedBy varchar(256) not null,
	CreatedOn datetime2 not null,
	constraint pk_blog primary key(Id),
	constraint fk_blog_user foreign key(CreatedBy) references [User].UserId
)

create table Post(
	Id int not null identity,
	Content nvarchar(max) not null,
	CreatedBy int not null,
	CreatedOn datetime not null,
	PostId int not null,
	constraint pk_post primary key(Id),
	constraint fk_post_user foreign key(CreatedBy) references [User].UserId,
	constraint fk_post_blog foreign key(PostId) references Blog.Id
)