using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleDrive.WebApi.GoogleDrive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;

namespace GoogleDrive.WebApi.Controllers
{
    public class FileUploadController : ApiController
    {


        [HttpPost]
        [Route("Drive")]
        public IHttpActionResult PostToGoogleDrive([FromBody] byte[] byteArray, [FromUri] string fileName)
        {
            //Gets credentials 
            UserCredential credential = new GoogleUtility().GetCredential();

            var service = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "DriveApiFileUpload"
            });

            Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File()
            {
                Title = Path.GetFileNameWithoutExtension(fileName),
                Description = "Uploaded from WebApi",
                MimeType = "image/jpeg" //Maps the file MIME type
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
        public IHttpActionResult Demo()
        {
            return Ok("This is demo method");
        }

    }
}
