using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using GoogleDrive.Model;
using GoogleDrive.WebApi.GoogleDrive;
using Newtonsoft.Json;
using System;
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
            return Ok(response.Id);

        }

        [HttpGet]
        public IHttpActionResult GetFile(string fileId)
        {
            GoogleUtility googleUtility = new GoogleUtility();
            UserCredential credential = googleUtility.GetCredential();

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveApiFileUpload",
            });
            try
            {
                Google.Apis.Drive.v2.Data.File  file = service.Files.Get(fileId).Execute();

                if(file == null)
                {
                    return NotFound();
                }

                return Ok(file);
            }
            catch (GoogleApiException ex)
            {
                return InternalServerError(ex);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

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
            listRequest.MaxResults = 120;
            listRequest.Q = "mimeType!='application/vnd.google-apps.folder' and trashed=false";
            IList<Google.Apis.Drive.v2.Data.File> files = listRequest.Execute().Items;

            if(files!=null)
            {
                
                return Ok(files);
            }

            return InternalServerError();
        }
    }
}
