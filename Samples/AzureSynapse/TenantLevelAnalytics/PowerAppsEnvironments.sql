/**************************************************************************
Name: PowerAppsEnvironments
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Apps Environments from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerapps/environments',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  
				environmentName varchar(100),
				purpose varchar(100),
				tenantGuid varchar(100),
				environmentState varchar(100),
				environmentType varchar(100),
				securityGroup varchar(100),
				environmentRegion varchar(100),
				environmentUrl varchar(100),
				isDefault varchar(100),
				cdsInstanceUrl varchar(100),
				cdsInstanceId varchar(100),
				createdPrincipalId varchar(100),
				createdTime datetime2,
				modifiedTime varchar(100),
				modifiedPrincipalId varchar(100),
				deletedTime varchar(100),
				deletedPrincipalId varchar(100)
			)