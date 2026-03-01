CREATE TABLE PulseCategory (
  Id   CHAR(36) NOT NULL,           -- store GUID as string
  Name VARCHAR(100) NOT NULL,
  CONSTRAINT PK_PulseCategory PRIMARY KEY (Id)
);

CREATE TABLE PulseEntry (
  Id         CHAR(36) NOT NULL,      -- store GUID as string
  CategoryId CHAR(36) NOT NULL,
  Score      TINYINT UNSIGNED NOT NULL,
  Comment    VARCHAR(500) NULL,
  CreatedAt  DATETIME(3) NOT NULL,   
  CONSTRAINT PK_PulseEntry PRIMARY KEY (Id),
  CONSTRAINT CK_PulseEntry_Score CHECK (Score BETWEEN 1 AND 5),
  CONSTRAINT FK_PulseEntry_Category FOREIGN KEY (CategoryId) 
  REFERENCES PulseCategory(Id)
);



