using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.IO;
using System.Threading;

namespace GoogleDrive.WebApi.GoogleDrive
{
    public class GoogleUtility
    {
        //Getting google credential with OAuth2
        public UserCredential GetCredential()
        {
            UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                                new ClientSecrets
                                {
                                    ClientId = "1095883481859-uqkb7r66vth2402p0dn8dpel8g2ptidm.apps.googleusercontent.com",
                                    ClientSecret = "3ZCqmmF5xfcM1_YE9NGjeUt6"
                                },
                                new[] { DriveService.Scope.Drive },
                                "user",
                                CancellationToken.None
                        ).Result;
            return credential;
        }

        public bool SearchFolder(DriveService service, string folderId)
        {
            ChildrenResource.ListRequest request = service.Children.List("root");
            //to search only folder and nontrashed area
            request.Q = "mimeType='application/vnd.google-apps.folder' and trashed=false";
            do
            {
                try
                {
                    ChildList childern = request.Execute();

                    foreach (ChildReference item in childern.Items)
                    {
                        if (item.Id == folderId)
                            return true;
                    }
                    //reffers to next page of data
                    request.PageToken = childern.NextPageToken; ;
                }
                catch (Exception ex)
                {
                    request.PageToken = null;
                }
            } while (!string.IsNullOrEmpty(request.PageToken));

            return false;
        }
    }
}