using Google;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using GoogleDrive.Model;
using GoogleDrive.WebApi.GoogleDrive;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
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

            //Folder id can be retrived from database which is user specific
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

            MemoryStream stream = new MemoryStream(byteArray);

            FilesResource.InsertMediaUpload requestToUpload = service.Files.Insert(body, stream, body.MimeType);
            requestToUpload.Upload();
            var response = requestToUpload.ResponseBody;
            GoogleDriveFile outputFile = new GoogleDriveFile(response);
            return Ok(outputFile);

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
                Google.Apis.Drive.v2.Data.File file = service.Files.Get(fileId).Execute();

                if (file == null)
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
        [HttpGet]
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

            if (files != null)
            {

                return Ok(files);
            }

            return InternalServerError();
        }
        [HttpDelete]
        [Route("api/FileToDrive/Delete/{id}")]
        public IHttpActionResult DeleteFile(string id)
        {
            try
            {
                GoogleUtility utility = new GoogleUtility();
                var credential = utility.GetCredential();
                DriveService service = new DriveService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "DriveApiFileUpload"
                });
                var value= service.Files.Delete(id).Execute();
                
                return Ok(value);
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }
        }
        [HttpPut]
        public IHttpActionResult Put(string fileId, string Title, string OriginalFileName, string Description)
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
                HttpWebRequest fileRequest = (HttpWebRequest)WebRequest.Create(fileMetaData.WebContentLink);
                HttpWebResponse response = (HttpWebResponse)fileRequest.GetResponse();
                var responseStream = response.GetResponseStream();

                // File's new content.
                //byte[] byteArray = System.IO.File.ReadAllBytes(responseStream);
                //System.IO.MemoryStream stream = new System.IO.MemoryStream(byteArray);

                Google.Apis.Drive.v2.Data.File GoogleFile = new Google.Apis.Drive.v2.Data.File
                {
                    Id = fileId,
                    Title = Title,
                    Description = Description,
                };
                FilesResource.UpdateMediaUpload request = service.Files.Update(GoogleFile, fileId, responseStream, "image/jpeg");
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