/**************************************************************************
Name: PowerAutomateConnectionReferences
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Automate Connection References from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerautomate/FlowConnectionReference',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  
				resourceId varchar(100),
				tenantId varchar(100),
				environmentId varchar(100),
				connectionrefId varchar(100),
				connectorType varchar(100),
				connectionId varchar(100),
				displayName varchar(100),
				tier varchar(100)
			)