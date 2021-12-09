select *
from openrowset(
        bulk 'https://powerplatformdl.blob.core.windows.net/powerplatform/powerapps/Applications',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  
				resourceId varchar(100),
				tenantId varchar(100),
				environmentId varchar(100),
				subType varchar(100),
				DocumentVersion varchar(100),
				[name] varchar(100),
				[description] nvarchar(MAX),
				uri varchar(100),
				lifecycleState varchar(100),
				[owner] varchar(100),
				createdTime datetime2,
				createdPrincipalId varchar(100),
				lastModifiedTime datetime2,
				lastModifiedPrincipalId varchar(100),
				lastEnabledPrincipalId varchar(100),
				sharedUsers int,
				sharedGroups int,
				actionsresourceworkflowActions varchar(100),
				settings varchar(100),
				solution varchar(100),
				clientDeviceType varchar(100),
				embeddingHost varchar(100),
				creationType varchar(100),
				iconUri varchar(100),
				documentUri varchar(100)
			)