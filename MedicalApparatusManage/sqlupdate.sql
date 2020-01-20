
UPDATE SysResource
SET ResourceName='上游企业'
WHERE ResourceId=35;
GO

UPDATE SysResource
SET ResourceName='下游企业'
WHERE ResourceId=36;
GO

UPDATE SysResource
SET ResourceName='企业信息'
WHERE ResourceId=18;
GO

UPDATE SysResource
SET ResourceName='经营许可证目录'
WHERE ResourceId=76;
GO

UPDATE SysResource
SET ResourceName='员工信息'
WHERE ResourceId=11;
GO

if not exists (select 1
            from  sysobjects
           where  id = object_id('T_CKCollect')
            and   type = 'U')
		BEGIN
		CREATE TABLE [dbo].[T_CKCollect] (
		[CollectID] int NOT NULL IDENTITY(1,1) ,
		[CKID] int NOT NULL ,
		[CollectDate] datetime NULL ,
		[CurWD] float(53) NULL ,
		[CurSD] float(53) NULL ,
		[CollectPerson] nvarchar(20) NULL ,
		[Remark] nvarchar(200) NULL 
		)
		END

GO
ALTER TABLE [dbo].[T_CKCollect] ADD PRIMARY KEY NONCLUSTERED ([CollectID])

GO
ALTER TABLE [dbo].[T_CKCollect] ADD FOREIGN KEY ([CKID]) REFERENCES [dbo].[T_CK] ([CKID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO

if not exists (select 1
            from  SysResource
           where  ResourceId = 2002)
	BEGIN
		INSERT INTO [dbo].[SysResource]
							 ([ResourceId]
							 ,[ResourceName]
							 ,[ResourceCode]
							 ,[ParentCode]
							 ,[ResourceRemark]
							 ,[ResourceOrder]
							 ,[ResourceNodeTage]
							 ,[ResourceIsHavChild])
				 VALUES
							 (2002
							 ,'库房监测'
							 ,'2002'
							 ,'401'
							 ,'/T_CKCollect/Index'
							 ,2
							 ,1
							 ,0)
	END
GO

if not exists (select 1
            from  SysRoleResource
           where  RoleId = 1 AND ResourceId=2002)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(1,2002);
		END
GO
if not exists (select 1
            from  SysRoleResource
           where  RoleId = 2 AND ResourceId=2002)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(2,2002);
		END
GO
if not exists (select 1
            from  SysRoleResource
           where  RoleId = 3 AND ResourceId=2002)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(3,2002);
		END
GO