/**************************************************************************
Name: PowerAppsConnections
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Apps Connections from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerapps/Connections',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  
				resourceId varchar(100),
				tenantId varchar(100),
				environmentId varchar(100),
				connectionName varchar(100),
				apiId varchar(100),
				connectionId varchar(100),
				displayName varchar(100),
				createdTime datetime2,
				createdPrincipalId varchar(100),
				isCustomApI varchar(100),
				lastModifiedTime datetime2,
				swaggerUrl varchar(100)
			)