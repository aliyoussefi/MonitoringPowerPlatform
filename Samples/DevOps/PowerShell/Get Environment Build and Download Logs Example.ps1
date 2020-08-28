function GetUrl(){
    param(
        [string]$orgUrl,
        [hashtable]$header,
        [string]$AreaId
    )

    $orgResrouceAreasUrl = [string]::Format("{0}/_apis/resourceAreas/{1}?api-preview=5.0-preview.1", $orgUrl, $AreaId)

    $results = Invoke-RestMethod -Uri $orgResrouceAreasUrl -Headers $header

    if ("null" -eq $results){
        $areaUrl = $orgUrl
    }
    else{
        $areaUrl = $results.locationUrl
    }

    return $areaUrl
}

#https://docs.microsoft.com/en-us/azure/devops/extend/develop/work-with-urls?view=azure-devops&tabs=http
#https://www.imaginet.com/2019/how-use-azure-devops-rest-api-with-powershell/
Write-Host "Define Global Variables" -ForegroundColor Yellow
$orgUrl = "https://dev.azure.com/<org>/";
$personalToken = "<generated PAT";
$projectName = "<your project>";
$apiVersion = "api-version=6.0-preview.1";
$buildDefName = "<build definition>";
$environmentName = "<environment>";
$coreAreaId = "79134c72-4a58-4b42-976c-04e7115f32bf"

Write-Host "Init auth context" -ForegroundColor Yellow
$token = [System.Convert]::ToBase64String([System.Text.Encoding]::ASCII.GetBytes(":$($personalToken)"))
$header = @{authorization = "Basic $token"}

$devopsBaseUrl = GetUrl -orgUrl $orgUrl -header $header -AreaId $coreAreaId

<#
This is a demo to Get the Projects and eventually the Project ID of what we need.
#>
Write-Host "Demo 0 - Get Projects" -ForegroundColor Yellow
$projectsUrl = "$($devopsBaseUrl)_apis/projects?api-version=5.0"
$projects = Invoke-RestMethod -Uri $projectsUrl -Method Get -ContentType "application/json" -Headers $header

$projects.value | ForEach-Object{
    $project = $_.name
    
    if ($project -eq $projectName){
        Write-Host $project  $_.id
        $projectId = $_.id
    }
}
Write-Host "Demo 1 - Get Environment" -ForegroundColor Yellow
#https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/environments/get?view=azure-devops-rest-6.0#environmentexpands
#https://docs.microsoft.com/en-us/rest/api/azure/devops/distributedtask/environmentdeployment%20records/list?view=azure-devops-rest-6.0
$environmentNameEncoded = [System.Web.HttpUtility]::UrlEncode($environmentName);
$environmentsUrl = "$($devopsBaseUrl)$($projectId)/_apis/distributedtask/environments?name=$($environmentNameEncoded)&$($apiVersion)"
$environments = Invoke-RestMethod -Uri $environmentsUrl -Method Get -ContentType "application/json" -Headers $header

$envirommentId = $environments.value[0].id;

$environmentDeploymentRecordsUrl = "$($devopsBaseUrl)$($projectId)/_apis/distributedtask/environments/$($envirommentId)/environmentdeploymentrecords?$($apiVersion)"
$environmentDeploymentRecords = Invoke-RestMethod -Uri $environmentDeploymentRecordsUrl -Method Get -ContentType "application/json" -Headers $header

<#
https://dev.azure.com/youssefiworkspace/{project}/_apis/build/builds
?definitions={definitions}
&queues={queues}
&buildNumber={buildNumber}
&minTime={minTime}
&maxTime={maxTime}
&requestedFor={requestedFor}
&reasonFilter={reasonFilter}
&statusFilter={statusFilter}
&resultFilter={resultFilter}
&tagFilters={tagFilters}
&properties={properties}
&$top={$top}
&continuationToken={continuationToken}
&maxBuildsPerDefinition={maxBuildsPerDefinition}
&deletedFilter={deletedFilter}
&queryOrder={queryOrder}
&branchName={branchName}
&buildIds={buildIds}
&repositoryId={repositoryId}
&repositoryType={repositoryType}
&api-version=5.1
#MinTime = 2019-12-17T15:04:22.07Z
#>
Write-Host "Demo 2 - Get Build Definition Id from unencoded name" -ForegroundColor Yellow
$buildDefNameEncoded = [System.Web.HttpUtility]::UrlEncode($buildDefName);
$buildDefUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/definitions?name=$($buildDefNameEncoded)&api-version=5.0"
$buildDef = Invoke-RestMethod -Uri $buildDefUrl -Method Get -ContentType "application/json" -Headers $header


#Build Def Id
$buildDefId = $buildDef.value[0].id;


Write-Host "Demo 2 - Get Builds in Build Definition Id, query order start time desc" -ForegroundColor Yellow
$buildsUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/builds?definitions=$($buildDefId)&queryorder=startTimeDescending&api-version=5.0"
$builds = Invoke-RestMethod -Uri $buildsUrl -Method Get -ContentType "application/json" -Headers $header

Write-Host 'Found' $builds.count 'builds in' $buildDefName

$build = $builds.value[0];
$buildId = $builds.value[0].id;
Write-Host 'Working with Build' $builds.value[0].url;

#Get Changes between Builds
Write-Host "Demo 3 - Get Changes in a Build" -ForegroundColor Yellow
#$buildChangesUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/changes?fromBuildId=$($fromBuildId)&toBuildId=$($lastBuildId)&api-version=5.1-preview.2"
$buildChangesUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/builds/$($buildId)/changes?api-version=5.1-preview.2";
$buildChanges = Invoke-RestMethod -Uri $buildChangesUrl -Method Get -ContentType "application/json" -Headers $header

Write-Host 'Found' $buildChanges.count 'changes in' $buildDefName

#https://docs.microsoft.com/en-us/rest/api/azure/devops/build/builds/get%20build%20work%20items%20refs?view=azure-devops-rest-6.0
Write-Host "Demo 4 - Get Work Items Associated in a Build" -ForegroundColor Yellow
$buildWorkItemsUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/builds/$($buildId)/workitems?api-version=5.1-preview.2";
$buildWorkItems = Invoke-RestMethod -Uri $buildWorkItemsUrl -Method Get -ContentType "application/json" -Headers $header

Write-Host 'Found' $buildWorkItems.count 'work items in' $buildDefName

#https://docs.microsoft.com/en-us/rest/api/azure/devops/build/artifacts/get%20file?view=azure-devops-rest-6.0
Write-Host "Demo 5 - Get Artifacts Attached to a Build" -ForegroundColor Yellow
$buildArtifactsUrl = "$($devopsBaseUrl)$($projectId)/_apis/build/builds/$($buildId)/artifacts?api-version=5.1-preview.2";
$buildArtifacts = Invoke-RestMethod -Uri $buildArtifactsUrl -Method Get -ContentType "application/json" -Headers $header

Write-Host 'Found' $buildArtifacts.count 'artifacts in' $buildDefName

#https://dev.azure.com/%7Borganization%7D/%7Bproject%7D/_apis/pipelines/%7BpipelineId%7D/runs/%7BrunId%7D/logs/%7BlogId%7D?api-version=6.1-preview.1
Write-Host "Demo 6 - Get Logs Attached to a Pipeline run" -ForegroundColor Yellow


#get the url for the task logs of a build log
$buildTaskLogs = Invoke-RestMethod -Uri $build.logs.url -Method Get -ContentType "application/json" -Headers $header
$buildTaskLogs.value | ForEach-Object{
    #get the log payload of each task log in a build
    $log = Invoke-RestMethod -Uri $_.url -Method Get -ContentType "application/json" -Headers $header
    Write-Host $log
}


