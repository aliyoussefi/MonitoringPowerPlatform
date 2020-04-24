#  This Sample Code is provided for the purpose of illustration only and is not intended to be used in a production environment.  
#  THIS SAMPLE CODE AND ANY RELATED INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED, 
#  INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.  
#  We grant You a nonexclusive, royalty-free right to use and modify the Sample Code and to reproduce and distribute the object 
#  code form of the Sample Code, provided that You agree: (i) to not use Our name, logo, or trademarks to market Your software 
#  product in which the Sample Code is embedded; (ii) to include a valid copyright notice on Your software product in which the 
#  Sample Code is embedded; and (iii) to indemnify, hold harmless, and defend Us and Our suppliers from and against any claims 
#  or lawsuits, including attorneys’ fees, that arise or result from the use or distribution of the Sample Code.

Set-ExecutionPolicy RemoteSigned -Scope CurrentUser

$myCredential = Get-AutomationPSCredential -Name 'CanvasAppCredentials-alyousse'
$userName = $myCredential.UserName
$securePassword = $myCredential.Password
$password = $myCredential.GetNetworkCredential().Password

$Credential = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $userName, $securePassword

Import-Module ExchangeOnlineManagement  

Connect-ExchangeOnline -Credential $Credential
$endDate = Get-Date
$startDate = $endDate.AddDays(-1)

$PowerAppsCreatePowerAppAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "CreatePowerApp"
$PowerAppsUpdatePowerAppAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "UpdatePowerApp"
$PowerAppsDeletePowerAppAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "DeletePowerApp"
$PowerAppsLaunchPowerAppAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "LaunchPowerApp"
$PowerAppsPublishPowerAppAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "PublishPowerApp"
$PowerAppsMarkPowerAppAsHeroAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "MarkPowerAppAsHero"
$PowerAppsMarkPowerAppAsFeaturedAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "MarkPowerAppAsFeatured"
$PowerAppsPowerAppPermissionEditedAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "PowerAppPermissionEdited"
$PowerAppsPowerAppPermissionDeletedAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "PowerAppPermissionDeleted"
$PowerAppsPromotePowerAppVersionAuditResult = Search-UnifiedAuditLog -StartDate $startDate -EndDate $endDate -RecordType "PowerAppsApp" -Operations "PromotePowerAppVersion"


$PowerAppsUpdatePowerAppAuditResult | ForEach-Object{
    $PowerAppsUpdatePowerAppAuditResultEvent = $_.Operations
    
    if ($PowerAppsUpdatePowerAppAuditResultEvent -eq 'UpdatePowerApp'){
        Write-Host $PowerAppsUpdatePowerAppAuditResultEvent  $_.Identity
        $AuditData = ConvertFrom-Json -InputObject $_.AuditData
        Write-Output $AuditData.AppName " had Operation " $PowerAppsUpdatePowerAppAuditResultEvent " performed at " $_.CreationDate " by " $_.UserIds
        $PowerAppsUpdatePowerAppAuditResultEvent = $_.Identity
    }
}

