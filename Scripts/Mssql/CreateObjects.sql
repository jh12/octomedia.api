
--BEGIN TRAN
-- TODO: Proper schema authorization
--CREATE SCHEMA OctoMedia

DROP TABLE IF EXISTS OctoMedia.Media;
DROP TABLE IF EXISTS OctoMedia.SourceReddit;
DROP TABLE IF EXISTS OctoMedia.Source;

-- OctoMedia.Source
CREATE TABLE OctoMedia.Source (
	Id                INT IDENTITY(1,1) NOT NULL,
	Title             NVARCHAR(250)     NULL,
	Interface         NVARCHAR(25)      NULL,
	SiteDomain        NVARCHAR(100)     NOT NULL,
	SiteUri           NVARCHAR(1000)    NOT NULL,
	RefererUri        NVARCHAR(500)     NULL,
	Deleted           BIT               NOT NULL,
	SiteUriHash AS CAST( HASHBYTES('SHA2_256', SiteUri) AS BINARY(32) )
);

ALTER TABLE OctoMedia.Source ADD CONSTRAINT PK_Source_Id PRIMARY KEY CLUSTERED ( Id );
--ALTER TABLE OctoMedia.Source ADD CONSTRAINT UQ_Source_SiteUri UNIQUE ( SiteUri );
CREATE NONCLUSTERED INDEX IX_Source_SiteUriHash ON OctoMedia.Source ( SiteUriHash ) INCLUDE ( SiteUri );

-- OctoMedia.Media
CREATE TABLE OctoMedia.Media (
	Id                INT IDENTITY(1,1) NOT NULL,
	Title             NVARCHAR(250)     NULL,
	[Description]     NVARCHAR(1000)    NULL,
	AuthorUsername    NVARCHAR(100)     NULL,
	Height            SMALLINT          NULL,
	Width             SMALLINT          NULL,
	SourceId          INT               NOT NULL,
	ImageUri          NVARCHAR(1000)    NULL,
	FileTypeExtension NVARCHAR(10)      NULL,
	FileTypeClass     TINYINT           NULL,
	Mature            BIT               NULL,
	Approved          BIT               NULL,
	FileHash          BINARY(20)        NULL,
	CreatedAt         DATETIME2(0)      NOT NULL,
	Deleted           BIT               NOT NULL,
	ImageUriHash AS CAST( HASHBYTES('SHA2_256', ImageUri) AS BINARY(32) )
);

ALTER TABLE OctoMedia.Media ADD CONSTRAINT PK_Media_Id PRIMARY KEY CLUSTERED ( Id );
ALTER TABLE OctoMedia.Media ADD CONSTRAINT FK_Media_Source FOREIGN KEY ( SourceId ) REFERENCES OctoMedia.Source ( Id ) ON DELETE CASCADE
--ALTER TABLE OctoMedia.Media ADD CONSTRAINT UQ_Media_ImageUri UNIQUE ( ImageUri );
CREATE NONCLUSTERED INDEX IX_Media_ImageUriHash ON OctoMedia.Media ( ImageUriHash ) INCLUDE ( ImageUri );


-- Reddit Source Interface
CREATE TABLE OctoMedia.SourceReddit (
	Id        INT          NOT NULL,
	Subreddit NVARCHAR(50) NOT NULL,
    Post      NVARCHAR(10) NOT NULL,
	[User]    NVARCHAR(50) NULL,
	NSFW      BIT          NULL,
	PostedAt  DATETIME2(0) NOT NULL
)

ALTER TABLE OctoMedia.SourceReddit ADD CONSTRAINT PK_SourceReddit_Id PRIMARY KEY CLUSTERED (Id);
ALTER TABLE OctoMedia.SourceReddit ADD CONSTRAINT FK_SourceReddit_Source FOREIGN KEY (Id) REFERENCES OctoMedia.Source (Id);