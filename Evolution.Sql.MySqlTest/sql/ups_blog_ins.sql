﻿DROP PROCEDURE IF EXISTS usp_blog_ins;
DELIMITER //
CREATE PROCEDURE usp_blog_ins(Title VARCHAR(1000),
	content TEXT,
	CreatedBy CHAR(36),
	CreatedOn DATETIME,
	UpdatedOn DATETIME,
	OUT Id	int 
)
BEGIN
	INSERT INTO Blog VALUES(NULL, Title, content, CreatedBy, CreatedOn, UpdatedOn);
	SELECT LAST_INSERT_ID() INTO Id;
END//
DELIMITER ;