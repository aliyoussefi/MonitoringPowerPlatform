/**************************************************************************
Name: PowerAppsUsage
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Apps Usage from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerapps/usage',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  
				appId varchar(100),
				tenantGuid varchar(100),
				environmentId varchar(100),
				sessionId varchar(100),
				objectId varchar(100),
				buildVersion varchar(100),
				countryCode varchar(10),
				[platform] varchar(100),
				browserName varchar(100),
				[version] varchar(100),
				playerVersion varchar(100)
			
			)