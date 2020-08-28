using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Clients;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace AzureDevOpsReleaseNotes
{
    public static class ReleaseNotesWebHook
    {
        [FunctionName("ReleaseNotesWebHook")]
        public static async void RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            //Extract data from request body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            dynamic releaseContainer;
            string releaseId = String.Empty;
            if (data?.resource == null)
            {
                return;
            }
            if (data?.resource?.environment != null)
            {
                releaseId = data?.resource?.environment?.release?.id;
            }
            if (data?.resource?.release != null)
            {
                releaseId = data?.resource?.release?.id;
            }
            if (String.IsNullOrEmpty(releaseId)) return;

            //string releaseName = data?.resource?.release?.name;
            //string releaseBody = data?.resource?.release?.description;

            VssBasicCredential credentials = new VssBasicCredential(Environment.GetEnvironmentVariable("DevOps.Username"), Environment.GetEnvironmentVariable("DevOps.AccessToken"));
            VssConnection connection = new VssConnection(new Uri(Environment.GetEnvironmentVariable("DevOps.OrganizationURL")), credentials);


            int intReleaseId = Convert.ToInt32(releaseId);
            Release release = GetRelease(connection, intReleaseId, "dbf0edd0-8900-4211-b91d-3053ab6f7128");
            if (data?.resource?.project?.id != null)
            {
                string projectId = data?.resource?.project?.id;
            }
            
            

            

            //Time span of 14 days from today
            var dateSinceLastRelease = DateTime.Today.Subtract(new TimeSpan(14, 0, 0, 0));

            //GetReleaseDetails(connection, releaseId, projectId);
            

            
            //Accumulate closed work items from the past 14 days in text format
            var workItems = GetClosedItems(connection, dateSinceLastRelease);
            var pulls = GetMergedPRs(connection, dateSinceLastRelease);
            
            foreach(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact in release.Artifacts)
            {
                if (artifact.Type == "Build")
                {
                    //var commits = GetCommitsFromBuild(connection, artifact.DefinitionReference["version"]);
                    
                }
            }

            


            //Create a new blob markdown file
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("releases");
            var blob = container.GetBlockBlobReference(release.Name + ".md");

            //Format text content of blob
            var text = String.Format("# {0} \n {1} \n\n" + "# Work Items Resolved:" + workItems 
                + "\n\n# Changes Merged:" + pulls 
                //+ "\n\n" + GetReleaseTaskIssues(release)
                //+ "\n\n" + GetReleaseArtifacts(release)
                , release.Name, release);

            //Add text to blob
            await blob.UploadTextAsync(text);
        }

        private static void GetCommitsFromBuild(VssConnection connection, string buildArtifactVersion)
        {
            var buildCommitsClient = connection.GetClient<Microsoft.TeamFoundation.Build.WebApi.BuildHttpClientBase>();
            using (buildCommitsClient) { 
            
            
            }

        }


        [FunctionName("ReleaseDeploymentNotesWebHook")]
        public static async void RunReleaseDeploymentNotesWebHookAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = null)]HttpRequest req, ILogger log)
        {
            //Extract data from request body
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            string releaseName = data?.resource?.environment?.release?.name;
            string releaseBody = data?.resource?.environment?.release?.description;
            int releaseId = data?.resource?.environment?.release?.id;
            string projectId = data?.resource?.project?.id;

            VssBasicCredential credentials = new VssBasicCredential(Environment.GetEnvironmentVariable("DevOps.Username"), Environment.GetEnvironmentVariable("DevOps.AccessToken"));
            VssConnection connection = new VssConnection(new Uri(Environment.GetEnvironmentVariable("DevOps.OrganizationURL")), credentials);

            //Time span of 14 days from today
            var dateSinceLastRelease = DateTime.Today.Subtract(new TimeSpan(14, 0, 0, 0));

            //GetReleaseDetails(connection, releaseId, projectId);
            Release release = GetRelease(connection, releaseId, "dbf0edd0-8900-4211-b91d-3053ab6f7128");

            //TODO
            //Accumulate closed work items from the past 14 days in text format
            //var workItems = GetClosedItems(connection, dateSinceLastRelease);
            //var pulls = GetMergedPRs(connection, dateSinceLastRelease);




            //Create a new blob markdown file
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("StorageAccountConnectionString"));
            var blobClient = storageAccount.CreateCloudBlobClient();
            var container = blobClient.GetContainerReference("releases");
            var blob = container.GetBlockBlobReference(releaseName + ".md");

            //Format text content of blob
            var text = String.Format("# {0} \n {1} \n\n" 
                //+ "# Work Items Resolved:" + workItems
                //+ "\n\n# Changes Merged:" + pulls
                + "\n\n" + GetReleaseTaskIssues(release)
                + "\n\n" + GetReleaseArtifacts(release)
                , releaseName, releaseBody);

            //Add text to blob
            await blob.UploadTextAsync(text);
        }


        public static Release GetRelease(VssConnection connection, int releaseId, string projectId)
        {
            string projectName = Environment.GetEnvironmentVariable("DevOps.ProjectName");
            var releaseClient = connection.GetClient<ReleaseHttpClient2>();

            using (releaseClient)
            {
                Release release = releaseClient.GetReleaseAsync(projectId, releaseId).Result;
                return release;
            }

        }

        public static string GetReleaseArtifacts(Release release)
        {
            StringBuilder rtnObject = new StringBuilder();
            rtnObject.AppendLine("## Artifacts for Release " + release.Name);
            foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.Artifact artifact in release.Artifacts)
            {
                Debug.WriteLine("Artifact Alias: " + artifact.Alias);
                Debug.WriteLine("Artifact Type: " + artifact.Type);
                rtnObject.AppendLine("Artifact " + artifact.Alias + " of type " + artifact.Type + " included in this release.");
                //rtnObject.AppendLine("## Artifacts for Release " + release.Name);
            }
            return rtnObject.ToString();


            //List<ReleaseTaskAttachment> releaseTaskAttachment = releaseClient.GetReleaseTaskAttachmentsAsync(
            //    project: projectName, 
            //    releaseId: release.Id, 
            //    environmentId: environment.Id, 
            //    attemptId: deployStep.Attempt, 
            //    planId: planId, type: "myattachmenttype").Result;

        }

        public static string GetReleaseTaskIssues(Release release)
        {
            StringBuilder rtnObject = new StringBuilder();
            rtnObject.AppendLine("## Issues for Release " + release.Name);
            foreach (ReleaseEnvironment environment in release.Environments)
            {
                Debug.WriteLine("Environment Name: " + environment.Name);

                foreach (DeploymentAttempt deploySteps in environment.DeploySteps)
                {
                    //Debug.WriteLine("Environment Name: " + deploySteps.Name);
                    foreach (ReleaseDeployPhase deployPhase in deploySteps.ReleaseDeployPhases)
                    {
                        rtnObject.AppendLine("## Deployment Information for " + release.Name);
                        foreach (DeploymentJob deploymentJob in deployPhase.DeploymentJobs)
                        {
                            rtnObject.AppendLine("Deployment Job " + deploymentJob.Job.Name + "  included " + deploymentJob.Tasks.Count + " tasks.");
                            foreach (ReleaseTask releaseTask in deploymentJob.Tasks)
                            {
                                
                                Debug.WriteLine(releaseTask.Name);
                                Debug.WriteLine("Issue Count: " + releaseTask.Issues.Count);
                                if (releaseTask.Issues.Count > 0)
                                {
                                    foreach (Issue issue in releaseTask.Issues)
                                    {
                                        Debug.WriteLine("Issue Found in '" + releaseTask.Name + "' Task. ISSUE: " + issue.Message);
                                        rtnObject.AppendLine("Issue Found in '" + releaseTask.Name + "' Task. ISSUE: " + issue.Message);
                                    }
                                }
                            }
                        }
                    }
                }
            }
                return rtnObject.ToString();


                //List<ReleaseTaskAttachment> releaseTaskAttachment = releaseClient.GetReleaseTaskAttachmentsAsync(
                //    project: projectName, 
                //    releaseId: release.Id, 
                //    environmentId: environment.Id, 
                //    attemptId: deployStep.Attempt, 
                //    planId: planId, type: "myattachmenttype").Result;

        }

        public static string GetClosedItems(VssConnection connection, DateTime releaseSpan)
        {
            string project = Environment.GetEnvironmentVariable("DevOps.ProjectName");
            var workItemTrackingHttpClient = connection.GetClient<WorkItemTrackingHttpClient>();
            
            //Query that grabs all of the Work Items marked "Done" in the last 14 days
            Wiql wiql = new Wiql()
            {
                Query = "Select [State], [Title] " +
                        "From WorkItems Where " +
                        "[System.TeamProject] = '" + project + "' " +
                        "And [System.State] = 'Done' " +
                        "And [Closed Date] >= '" + releaseSpan.ToString() + "' " +
                        "Order By [State] Asc, [Changed Date] Desc"
            };

            using (workItemTrackingHttpClient)
            {
                WorkItemQueryResult workItemQueryResult = workItemTrackingHttpClient.QueryByWiqlAsync(wiql).Result;

                if (workItemQueryResult.WorkItems.Count() != 0)
                {
                    List<int> list = new List<int>();
                    foreach (var item in workItemQueryResult.WorkItems)
                    {
                        list.Add(item.Id);
                    }

                    //Extraxt desired work item fields
                    string[] fields = { "System.Id", "System.Title" };
                    var workItems = workItemTrackingHttpClient.GetWorkItemsAsync(list, fields, workItemQueryResult.AsOf).Result;

                    //Format Work Item info into text
                    string txtWorkItems = string.Empty;
                    foreach (var workItem in workItems)
                    {
                        txtWorkItems += String.Format("\n 1. #{0}-{1}", workItem.Id, workItem.Fields["System.Title"]);
                    }
                    return txtWorkItems;
                }
                return string.Empty;
            }
        }

        public static string GetMergedPRs(VssConnection connection, DateTime releaseSpan)
        {
            string projectName = Environment.GetEnvironmentVariable("DevOps.ProjectName");
            var gitClient = connection.GetClient<GitHttpClient>();

            using (gitClient)
            {
                //Get first repo in project
                var releaseRepo = gitClient.GetRepositoriesAsync().Result[0]; 

                //Grabs all completed PRs merged into master branch
                List<GitPullRequest> prs = gitClient.GetPullRequestsAsync(
                   releaseRepo.Id,
                   new GitPullRequestSearchCriteria()
                   {
                       TargetRefName = "refs/heads/master",
                       Status = PullRequestStatus.Completed                       

                   }).Result;

                if (prs.Count != 0)
                {
                    //Query that grabs PRs merged since the specified date
                    var pulls = from p in prs
                            where p.ClosedDate >= releaseSpan
                            select p;

                    //Format PR info into text
                    var txtPRs = string.Empty;
                    foreach (var pull in pulls)
                    {
                        txtPRs += String.Format("\n 1. #{0}-{1}", pull.PullRequestId, pull.Title);
                    }

                    return txtPRs;
                }
                return string.Empty;
            }
        }
    }
}
