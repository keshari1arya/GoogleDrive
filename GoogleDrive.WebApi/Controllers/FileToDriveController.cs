using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using GoogleDrive.WebApi.GoogleDrive;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Http;

namespace GoogleDrive.WebApi.Controllers
{
    public class FileToDriveController : ApiController
    {
        [HttpPost]
        public IHttpActionResult SaveFile([FromBody] byte[] byteArray, [FromUri] string fileName)
        {

            //Gets credentials 
            GoogleUtility googleUtility = new GoogleUtility();


            UserCredential credential = googleUtility.GetCredential();

            //byte array to HttpPostedFileBase

            HttpPostedFileBase file = new ByteArrayToHttpPostedFileBase(byteArray, fileName);

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveApiFileUpload"
            });


            var folderId = "0B2pC99R_P4taZHdQeU9ScDJoSWs";

            bool folderExists = googleUtility.SearchFolder(service, folderId);

            //check wheather the folder exists or not
            if (!folderExists)
            {
                //folder Creation
                var folder = new Google.Apis.Drive.v2.Data.File { Title = "Photos", MimeType = "application/vnd.google-apps.folder" };
                var result = service.Files.Insert(folder).Execute();
                folderId = result.Id;
            }

            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File()
            {
                Title = Path.GetFileNameWithoutExtension(file.FileName),
                Description = "Uploaded from WebApi",
                MimeType = MimeMapping.GetMimeMapping(fileName), //Maps the file MIME type
                Parents = new List<ParentReference>() { new ParentReference() { Id = folderId } }
            };

            //byte[] byteArray = new byte[file.ContentLength];
            //file.InputStream.Read(byteArray, 0, file.ContentLength);

            MemoryStream stream = new MemoryStream(byteArray);

            FilesResource.InsertMediaUpload requestToUpload = service.Files.Insert(body, stream, body.MimeType);
            requestToUpload.Upload();
            var response = requestToUpload.ResponseBody;
            return Ok($"file ID: {response.Id} and Folder ID: {folderId}");

        }

        public IHttpActionResult GetAllFiles()
        {
            //Gets credentials 
            GoogleUtility googleUtility = new GoogleUtility();

            UserCredential credential = googleUtility.GetCredential();


            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveApiFileUpload"
            });

            var listRequest = service.Files.List();
            listRequest.MaxResults = 10;

            IList<Google.Apis.Drive.v2.Data.File> files = listRequest.Execute().Items;

            if(files!=null)
            {
                return Ok(files);
            }

            return InternalServerError();
        }
    }
}
