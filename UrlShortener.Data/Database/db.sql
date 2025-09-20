IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'URLShortener')
BEGIN
    CREATE DATABASE URLShortener;
END;
GO

USE URLShortener;
GO

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'ROLES' AND xtype = 'U')
BEGIN
	CREATE Table ROLES (
		id   VARCHAR(30) PRIMARY KEY,
		name VARCHAR(30) NOT NULL
	);

	INSERT INTO ROLES(id, name)
	VALUES('admin', 'Adminstrator'),
		  ('mod', 'Moderator'),
		  ('user', 'User');

END;
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'PLANS' AND xtype = 'U')
BEGIN
	CREATE Table PLANS (
	  id               INT         PRIMARY KEY IDENTITY(1,1),
	  name             VARCHAR(20) UNIQUE NOT NULL,
	  price            DECIMAL     NOT NULL,
	  max_daily_url    INT         NOT NULL,
	  url_expires_after INT        NOT NULL DEFAULT(30) ,
	  has_custom_slugs BIT         NOT NULL,
	  support_level    VARCHAR(20) NOT NULL
	)

	INSERT INTO PLANS(name, price, max_daily_url, url_expires_after, has_custom_slugs, support_level)
	VALUES('Basic', 100, 5, 30, 0, 'Low');
END;
GO



IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'USERS' AND xtype = 'U')
BEGIN
	CREATE TABLE USERS (
	  id              INT          PRIMARY KEY IDENTITY(1,1),
	  email           VARCHAR(60)  UNIQUE NOT NULL,
	  password        VARCHAR(120) NOT NULL,
	  plan_id         INT          NOT NULL,
	  plan_expires_at DATETIME2,
	  created_at      DATETIME2    DEFAULT(GETDATE()) NOT NULL,
	  FOREIGN KEY     (plan_id)    REFERENCES PLANS(id),
	);

	INSERT INTO USERS(email, password, plan_id, plan_expires_at)
	VALUES('admin@gmail.com', 'AQAAAAIAAYagAAAAENf7iaWmVWTESE19cQAeKtvX2BPvmR9FOm2cwrYai0VJnFZAM1c+WjD1vhgOvS2XHA==', 1, '2025-12-12');
END;
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'REFRESH_TOKEN' AND xtype = 'U')
BEGIN
	CREATE TABLE REFRESH_TOKEN (
	  id              UNIQUEIDENTIFIER PRIMARY KEY,
	  token			  VARCHAR(200)     NOT NULL,
	  user_id         INT              NOT NULL,
	  expire_at       DATETIME2		   NOT NULL,
	  FOREIGN KEY     (user_id)        REFERENCES USERS(id) ON DELETE CASCADE,
	);
END;
GO

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'URLS' AND xtype = 'U')
BEGIN
	CREATE TABLE URLS (
	  id            INT           PRIMARY KEY IDENTITY(1,1),
	  short_code    VARCHAR(12)   UNIQUE			   NOT NULL,
	  source        VARCHAR(500)  NOT NULL,
	  title         VARCHAR(120),
	  visit_count   INT           DEFAULT(0)		   NOT NULL,
	  created_at    DATETIME2     DEFAULT(GETDATE())   NOT NULL,
	  created_by    INT           NOT NULL,
	  last_modified DATETIME2     DEFAULT(GETDATE())   NOT NULL,
	  expires_at    DATETIME2	  NOT NULL,
	  is_active     BIT           DEFAULT(1)		   NOT NULL,
	  FOREIGN KEY  (created_by)   REFERENCES USERS(id) ON DELETE CASCADE
	);
END;
GO


IF NOT EXISTS (SELECT * FROM sysobjects WHERE name = 'URL_VISIT' AND xtype = 'U')
BEGIN
	CREATE Table URL_VISIT (
	  url_id       INT			  NOT NULL,             
	  visitor_ip   VARCHAR(32)	  NOT NULL,
	  user_agent   VARCHAR(300),
	  visited_at   DATETIME2	  DEFAULT(GETDATE())  NOT NULL,
	  FOREIGN KEY  (url_id)		  REFERENCES URLS(id) ON DELETE CASCADE,
	  PRIMARY KEY  (url_id,
					visitor_ip,
					visited_at)
	);
END;
GO

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'ACTIONS' AND xtype = 'U')
BEGIN
	CREATE Table ACTIONS (
		id   VARCHAR(30)       PRIMARY KEY
	);

	INSERT INTO ACTIONS(id)
	VALUES('Create'), ('Read'),
		  ('Update'), ('Delete');
END;
GO




IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'RESOURCES' AND xtype = 'U')
BEGIN
	CREATE Table RESOURCES (
		id   INT         PRIMARY KEY IDENTITY(1,1),
		name VARCHAR(30) NOT NULL
	);

	INSERT INTO RESOURCES(name)
	VALUES('User'), ('Plan'), ('Url');
END;
GO




IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'PERMISSIONS' AND xtype = 'U')
BEGIN
	CREATE Table PERMISSIONS (
		role_id      VARCHAR(30),
		resource_id  INT        ,
		action_id    VARCHAR(30),
		PRIMARY KEY  (role_id, 
					 resource_id,
					 action_id),
		FOREIGN KEY  (role_id)	   REFERENCES ROLES(id) ON DELETE CASCADE,
		FOREIGN KEY  (resource_id) REFERENCES RESOURCES(id) ON DELETE CASCADE,
		FOREIGN KEY  (action_id)   REFERENCES ACTIONS(id),
	);
END;
GO

INSERT INTO PERMISSIONS(role_id, resource_id, action_id)
VALUES ('admin', 1, 'Create')
     , ('admin', 1, 'Read'  ), ('mod', 1, 'Read'  )
     , ('admin', 1, 'Update')
     , ('admin', 1, 'Delete')
     , ('admin', 2, 'Create')
     , ('admin', 2, 'Read'  ), ('mod', 2, 'Read'  )
     , ('admin', 2, 'Update')
     , ('admin', 2, 'Delete')
     , ('admin', 3, 'Create'), ('mod', 3, 'Create')
     , ('admin', 3, 'Read'  ), ('mod', 3, 'Read'  )
     , ('admin', 3, 'Update'), ('mod', 3, 'Update')
     , ('admin', 3, 'Delete'), ('mod', 3, 'Delete');

IF NOT EXISTS(SELECT * FROM sysobjects WHERE name = 'USER_ROLES' AND xtype = 'U')
BEGIN
	CREATE Table USER_ROLES (
		user_id     INT,
		role_id     VARCHAR(30),
		PRIMARY KEY (user_id, 
					 role_id),
		FOREIGN KEY (user_id)   REFERENCES USERS(id) ON DELETE CASCADE,
		FOREIGN KEY (role_id)   REFERENCES ROLES(id) ON DELETE CASCADE
	);

	INSERT INTO USER_ROLES(user_id, role_id)
	VALUES(1, 'admin'), (1, 'mod');
END;
GO

