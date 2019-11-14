﻿DROP PROCEDURE IF EXISTS usp_data_type_ins;
DELIMITER //
CREATE PROCEDURE usp_data_type_ins(
	ColTinyInt 		TINYINT,
	ColUTinyInt		tinyint unsigned,
    ColSmallInt		SMALLINT,
    ColMediumInt		mediumint,
    ColInt			int,
    ColBigInt		bigint,
    ColDecimal		decimal(10,4),
    ColNumeric		numeric(10,4),
    ColFloat		float,
    ColDouble		double,
    ColReal			real,
    ColBit			bit,
	ColDate			date,
    ColDateTime		datetime,
    ColTime			time,
    ColTimeStamp	timestamp,
    ColYear			year,
    ColChar			char(255),
    ColVarChar		varchar(1000),
    ColTinyText		tinytext,
    ColText			text,
	ColMediumText	mediumText,
    ColLongText		longtext,
    ColBinary		binary(255),
	ColVarBinary	varbinary(8000),
    ColTinyBlob		tinyblob,
    ColBlob			blob,
    ColMediumBlob	mediumBlob,
    ColLongBlob		longblob,
    ColJson			json,
    ColBool			bool
)
BEGIN
	INSERT INTO data_type VALUES(			
	ColTinyInt,
    ColSmallInt,	
    ColMediumInt,	
    ColInt,			
    ColBigInt,		
    ColDecimal,		
    ColNumeric,	
    ColFloat,		
    ColDouble,		
    ColReal,			
    ColBit,			
	ColDate,
    ColDateTime,		
    ColTime,			
    ColTimeStamp,
    #current_timestamp(),
    ColYear,			
    ColChar,		
    ColVarChar,		
    ColTinyText,		
    ColText,			
	ColMediumText,	
    ColLongText,		
    ColBinary,		
	ColVarBinary,	
    ColTinyBlob,		
    ColBlob,			
    ColMediumBlob,
    ColLongBlob,
    ColJson,
    ColBool
    );
END//
DELIMITER ;