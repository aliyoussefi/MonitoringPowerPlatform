/**************************************************************************
Name: PowerAutomateFlows
Created Date: 12/8/21
Author: Ali Youssefi
Description: Used for Power Automate Flows from Tenant Level Analytics
References:
https://docs.microsoft.com/en-us/power-platform/admin/build-custom-reports
*************************************************************************/
select *
from openrowset(
        bulk 'https://<Data Lake Name>.blob.core.windows.net/powerplatform/powerautomate/Flows',
        format = 'csv',
        fieldterminator ='0x0b',
        fieldquote = '0x0b'
    ) with (doc nvarchar(max)) as rows
	    cross apply openjson (doc)
        with (  [type] varchar(100),
				subType varchar(100),
                resourceId varchar(100),
				[name] varchar(100),
				tenantId varchar(100),
				environmentId varchar(100),
				resourceVersion varchar(100),
				lifecycleState varchar(100),
				events_created_timestamp datetime2,
				events_created_principalId varchar(100),
				events_modified_timestamp datetime2,
				sharedUsers int,
				sharedGroups int,
				events_created_userType varchar(100) '$.customExtensions.events_created_userType',
				sharingType varchar(100) '$.customExtensions.sharingType',
				flowTriggerUri varchar(100) '$.customExtensions.flowTriggerUri',
				triggerKind varchar(100) '$.customExtensions.triggerKind',
				flowSuspensionTime varchar(100) '$.customExtensions.flowSuspensionTime',
				flowSuspensionReason varchar(100) '$.customExtensions.flowSuspensionReason')
--where resourceId = 'e9edb53d-6b3c-0cec-de2c-988680719ffc'
--order by timeAccessed desc;