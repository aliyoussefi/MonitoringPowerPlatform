/**************************************************************************
Name: PowerAutomateUsage
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Automate Usage from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerautomate/usage',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  timeAccessed datetime2,
				[status] varchar(100),
                tenantId varchar(100),
                runs int '$.runs',
                resourceId varchar(100) '$.resourceId')