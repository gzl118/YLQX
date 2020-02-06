
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
							 ,'库房管理'
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

UPDATE SysResource
SET ResourceName='库房信息',
ParentCode=201
WHERE ResourceId=4;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_CGD') AND name = 'IsFinish') 
		BEGIN
			ALTER TABLE T_CGD ADD IsFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_CGD', N'column', N'IsFinish';
		END
GO
UPDATE T_CGD SET IsFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_YSD') AND name = 'IsFinish') 
		BEGIN
			ALTER TABLE T_YSD ADD IsFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_YSD', N'column', N'IsFinish';
		END
GO

UPDATE T_YSD SET IsFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_XSD') AND name = 'IsFinish') 
		BEGIN
			ALTER TABLE T_XSD ADD IsFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_XSD', N'column', N'IsFinish';
		END
GO

UPDATE T_XSD SET IsFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_RKD') AND name = 'IsFinish') 
		BEGIN
			ALTER TABLE T_RKD ADD IsFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_RKD', N'column', N'IsFinish';
		END
GO

UPDATE T_RKD SET IsFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_CKD') AND name = 'IsFinish') 
		BEGIN
			ALTER TABLE T_CKD ADD IsFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_CKD', N'column', N'IsFinish';
		END
GO

UPDATE T_CKD SET IsFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_YSD') AND name = 'IsCGFinish') 
		BEGIN
			ALTER TABLE T_YSD ADD IsCGFinish int NULL DEFAULT 0;  -- 是否完成，0未完成1完成
			EXECUTE sp_addextendedproperty N'MS_Description', '是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_YSD', N'column', N'IsCGFinish';
		END
GO

UPDATE T_YSD SET IsCGFinish=0;
GO

if not exists (select 1
            from  SysResource
           where  ResourceId = 2003)
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
							 (2003
							 ,'非采购验收管理'
							 ,'2003'
							 ,'900'
							 ,'/T_YSD/NCGIndex'
							 ,11
							 ,1
							 ,2)
	END
GO

if not exists (select 1
            from  SysRoleResource
           where  RoleId = 1 AND ResourceId=2003)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(1,2003);
		END
GO
if not exists (select 1
            from  SysRoleResource
           where  RoleId = 2 AND ResourceId=2003)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(2,2003);
		END
GO
if not exists (select 1
            from  SysRoleResource
           where  RoleId = 3 AND ResourceId=2003)
		BEGIN
			INSERT INTO SysRoleResource(RoleId,ResourceId)
			VALUES(3,2003);
		END
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_YSD') AND name = 'IsCGYS') 
		BEGIN
			ALTER TABLE T_YSD ADD IsCGYS int NULL DEFAULT 0;  
			EXECUTE sp_addextendedproperty N'MS_Description', '是否采购验收，0是1不是', N'user', N'dbo', N'table', N'T_YSD', N'column', N'IsCGYS';
		END
GO

UPDATE T_YSD SET IsCGYS=0;
GO

UPDATE SysResource
SET ParentCode='900',
ResourceRemark='/T_THD/Index/',
ResourceIsHavChild=2
WHERE ResourceId=23;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_YSD') AND name = 'IsTHFinish') 
		BEGIN
			ALTER TABLE T_YSD ADD IsTHFinish int NULL DEFAULT 0;  -- 针对退货来判断是否该验收单已经完毕
			EXECUTE sp_addextendedproperty N'MS_Description', '退货是否完成，0未完成1完成', N'user', N'dbo', N'table', N'T_YSD', N'column', N'IsTHFinish';
		END
GO

UPDATE T_YSD SET IsTHFinish=0;
GO

IF NOT EXISTS(SELECT 1 FROM sys.syscolumns WHERE id = OBJECT_ID('T_THMX') AND name = 'CPPH') 
		BEGIN
			ALTER TABLE T_THMX ADD CPPH varchar(50) NULL;
			EXECUTE sp_addextendedproperty N'MS_Description', '产品批号', N'user', N'dbo', N'table', N'T_THMX', N'column', N'CPPH';
			ALTER TABLE T_THMX ADD CPSCRQ date NULL;
			EXECUTE sp_addextendedproperty N'MS_Description', '产品生产日期', N'user', N'dbo', N'table', N'T_THMX', N'column', N'CPSCRQ';
			ALTER TABLE T_THMX ADD CPYXQ date NULL;
			EXECUTE sp_addextendedproperty N'MS_Description', '产品有效期', N'user', N'dbo', N'table', N'T_THMX', N'column', N'CPYXQ';
		END
GO