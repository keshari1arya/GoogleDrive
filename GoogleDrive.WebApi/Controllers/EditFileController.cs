using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using GoogleDrive.Model;
using GoogleDrive.WebApi.GoogleDrive;
using System;
using System.Net;
using System.Web.Http;

namespace GoogleDrive.WebApi.Controllers
{
    public class EditFileController : ApiController
    {
        [HttpPost]
        [Route("api/EditFile/SaveEdited/{fileId}/{Title}/{OriginalFileName}/{Description}")]
        public IHttpActionResult SaveEdited([FromUri]string fileId, [FromUri] string Title, [FromUri]string OriginalFileName, [FromUri]string Description)
        {
            try
            {
                GoogleUtility utility = new GoogleUtility();
                var credential = utility.GetCredential();
                var service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "DriveApiFileUpload"
                });

                Google.Apis.Drive.v2.Data.File fileMetaData = service.Files.Get(fileId).Execute();

                //getting file content from web as downloaded 
                WebClient client = new WebClient();
                Uri uri = new Uri(fileMetaData.WebContentLink);
                byte[] ar = client.DownloadData(uri);
                //// File's new content.
                System.IO.MemoryStream stream = new System.IO.MemoryStream(ar);

                Google.Apis.Drive.v2.Data.File GoogleFile = new Google.Apis.Drive.v2.Data.File
                {
                    Id = fileId,
                    Title = Title,
                    Description = Description,
                };
                FilesResource.UpdateMediaUpload request = service.Files.Update(GoogleFile, fileId, stream, GoogleFile.MimeType);
                request.Upload();
                Google.Apis.Drive.v2.Data.File updatedFile = request.ResponseBody;
                return Ok(updatedFile);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }

    }
}