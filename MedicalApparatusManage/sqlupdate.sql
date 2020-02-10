
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

DROP TABLE [dbo].[T_THMX]
GO

DROP TABLE [dbo].[T_THD]
GO
CREATE TABLE [dbo].[T_THD] (
[THID] int NOT NULL IDENTITY(1,1) ,
[THMC] nvarchar(50) NULL ,
[SQR] nvarchar(50) NULL ,
[SQRQ] datetime NULL ,
[THYY] nvarchar(400) NULL ,
[SHR] nvarchar(50) NULL ,
[SHRQ] date NULL ,
[SHYJ] nvarchar(200) NULL ,
[FLAG] int NULL DEFAULT ((0)) ,
[BZ] nvarchar(200) NULL ,
[YSID] int NULL ,
[RKFlag] int NULL DEFAULT ((0)) ,
[THDH] nvarchar(50) NULL ,
[ISSH] int NULL ,
[THCJR] varchar(50) NULL ,
[THCJRQ] date NULL ,
[IsFinish] int NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[T_THD]', RESEED, 3)
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
NULL, NULL)) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货单'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货单'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'THID')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THID'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THID'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'SQR')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'申请人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SQR'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'申请人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SQR'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'THYY')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货原因'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THYY'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货原因'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THYY'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'SHR')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'审核人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SHR'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'审核人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SHR'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'SHYJ')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'审核意见'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SHYJ'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'审核意见'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'SHYJ'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'FLAG')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'0合格退货，1不合格退货'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'FLAG'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'0合格退货，1不合格退货'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'FLAG'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'YSID')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'验收ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'YSID'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'验收ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'YSID'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'RKFlag')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'0入库前退货1入库后退货'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'RKFlag'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'0入库前退货1入库后退货'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'RKFlag'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'THDH')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货单号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THDH'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货单号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THDH'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'ISSH')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'审核状态，0待审核，1审核通过，2审核不通过'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'ISSH'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'审核状态，0待审核，1审核通过，2审核不通过'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'ISSH'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'THCJR')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货采集人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THCJR'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货采集人'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THCJR'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'THCJRQ')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货采集日期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THCJRQ'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货采集日期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'THCJRQ'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THD', 
'COLUMN', N'IsFinish')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'是否完成，0未完成1完成'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'IsFinish'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'是否完成，0未完成1完成'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THD'
, @level2type = 'COLUMN', @level2name = N'IsFinish'
GO

-- ----------------------------
-- Indexes structure for table T_THD
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_THD
-- ----------------------------
ALTER TABLE [dbo].[T_THD] ADD PRIMARY KEY ([THID])
GO

-- ----------------------------
-- Foreign Key structure for table [dbo].[T_THD]
-- ----------------------------
ALTER TABLE [dbo].[T_THD] ADD FOREIGN KEY ([YSID]) REFERENCES [dbo].[T_YSD] ([YSID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO


CREATE TABLE [dbo].[T_THMX] (
[MXID] int NOT NULL IDENTITY(1,1) ,
[CPID] int NOT NULL ,
[THID] int NOT NULL ,
[CPNUM] float(53) NULL ,
[BZ] nvarchar(200) NULL ,
[CKID] int NOT NULL ,
[FLAG] int NULL ,
[CPPH] varchar(50) NULL ,
[CPSCRQ] date NULL ,
[CPYXQ] date NULL 
)


GO
DBCC CHECKIDENT(N'[dbo].[T_THMX]', RESEED, 3)
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
NULL, NULL)) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货明细'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货明细'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'CPID')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'产品ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPID'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'产品ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPID'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'THID')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'退货ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'THID'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'退货ID'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'THID'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'CPNUM')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'产品数量'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPNUM'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'产品数量'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPNUM'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'BZ')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'备注'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'BZ'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'备注'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'BZ'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'CPPH')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'产品批号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPPH'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'产品批号'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPPH'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'CPSCRQ')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'产品生产日期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPSCRQ'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'产品生产日期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPSCRQ'
GO
IF ((SELECT COUNT(*) from fn_listextendedproperty('MS_Description', 
'SCHEMA', N'dbo', 
'TABLE', N'T_THMX', 
'COLUMN', N'CPYXQ')) > 0) 
EXEC sp_updateextendedproperty @name = N'MS_Description', @value = N'产品有效期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPYXQ'
ELSE
EXEC sp_addextendedproperty @name = N'MS_Description', @value = N'产品有效期'
, @level0type = 'SCHEMA', @level0name = N'dbo'
, @level1type = 'TABLE', @level1name = N'T_THMX'
, @level2type = 'COLUMN', @level2name = N'CPYXQ'
GO

-- ----------------------------
-- Indexes structure for table T_THMX
-- ----------------------------

-- ----------------------------
-- Primary Key structure for table T_THMX
-- ----------------------------
ALTER TABLE [dbo].[T_THMX] ADD PRIMARY KEY ([MXID])
GO

-- ----------------------------
-- Foreign Key structure for table [dbo].[T_THMX]
-- ----------------------------
ALTER TABLE [dbo].[T_THMX] ADD FOREIGN KEY ([CKID]) REFERENCES [dbo].[T_CK] ([CKID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[T_THMX] ADD FOREIGN KEY ([THID]) REFERENCES [dbo].[T_THD] ([THID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO
ALTER TABLE [dbo].[T_THMX] ADD FOREIGN KEY ([CPID]) REFERENCES [dbo].[T_YLCP] ([CPID]) ON DELETE NO ACTION ON UPDATE NO ACTION
GO




UPDATE SysResource
SET ResourceOrder=2
WHERE ResourceId=71;

UPDATE SysResource
SET ResourceOrder=3
WHERE ResourceId=2003;

UPDATE SysResource
SET ResourceOrder=4
WHERE ResourceId=72;

UPDATE SysResource
SET ResourceOrder=5
WHERE ResourceId=73;

UPDATE SysResource
SET ResourceOrder=6
WHERE ResourceId=74;

UPDATE SysResource
SET ResourceOrder=7
WHERE ResourceId=75;

UPDATE SysResource
SET ResourceOrder=8
WHERE ResourceId=99;

UPDATE SysResource
SET ResourceOrder=9
WHERE ResourceId=23;