using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v2;
using Google.Apis.Services;
using GoogleDrive.Model;
using GoogleDrive.MVC.UtilityMethods;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace GoogleDrive.MVC.Controllers
{
    public class FIleUploadController : Controller
    {
        // GET: FIleUpload
        public ActionResult Index()
        {
            return View();
        }

        public async Task<ActionResult> PostFile(HttpPostedFileBase file)
        {

            if (file == null)
            {
                ModelState.AddModelError("File", "Please Upload Your file");
            }
            else if (file.ContentLength > 0)
            {
                int MaxContentLength = 1024 * 1024 * 4; //4 MB
                string[] AllowedFileExtensions = new string[] { ".jpg", ".gif", ".png", ".pdf", ".jpeg" };

                if (!AllowedFileExtensions.Contains(file.FileName.Substring(file.FileName.LastIndexOf('.'))))
                {
                    ModelState.AddModelError("File", "Please upload file of type: " + string.Join(", ", AllowedFileExtensions));
                }

                else if (file.ContentLength > MaxContentLength)
                {
                    ModelState.AddModelError("File", $"Your file is too large, maximum allowed size is: { MaxContentLength } MB");
                }
                else
                {
                    ModelState.Clear();
                    #region Transferd to WebApi
                    //UserCredential credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    //            new ClientSecrets
                    //            {
                    //                ClientId = "1095883481859-uqkb7r66vth2402p0dn8dpel8g2ptidm.apps.googleusercontent.com",
                    //                ClientSecret = "YMkKtv6YrZIBTqdNLuVpKCl7"
                    //            },
                    //            new[] { DriveService.Scope.Drive },
                    //            "user",
                    //            CancellationToken.None
                    //    ).Result;
                    //var service = new DriveService(new BaseClientService.Initializer()
                    //{
                    //    HttpClientInitializer = credential,
                    //    ApplicationName = "DriveApiFileUpload"///Can be changed
                    //});

                    //Google.Apis.Drive.v2.Data.File body = new Google.Apis.Drive.v2.Data.File();
                    //body.Title = "Candidates Document-" + System.IO.Path.GetFileNameWithoutExtension(file.FileName);
                    //body.Description = "Carrer document";
                    //body.MimeType = "image/jpeg";


                    //byte[] byteArray = new byte[file.ContentLength];// System.IO.File.ReadAllBytes(path);
                    //file.InputStream.Read(byteArray, 0, file.ContentLength);
                    //MemoryStream stream = new MemoryStream(byteArray);


                    //FilesResource.InsertMediaUpload request = service.Files.Insert(body, stream, "image/jpeg");
                    //request.Upload();

                    //var driveFile = request.ResponseBody;
                    #endregion

                    //calling to api to upload file to dirve  http://localhost:16184/api/FileUpload/PostToGoogleDrive
                    WebApiCall call = new WebApiCall();
                    string response = await call.PostToGoogleDrive(file);


                    ViewBag.Message = "File uploaded successfully.File id: " + response;
                }
            }

            return View();


        }

        public async Task<ActionResult> GetAllFile()
        {
            WebApiCall call = new WebApiCall();

            var message = await call.getAllFiles();

            string result = await message.Content.ReadAsStringAsync();

            // GoogleDriveFile files = new JavaScriptSerializer().Deserialize<GoogleDriveFile>(result);

            object files = JsonConvert.DeserializeObject<IList<GoogleDriveFile>>(result);

            //return Json(files, JsonRequestBehavior.AllowGet);
            return View(files);
        }

        public async Task<string> FileName(HttpPostedFileBase file)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:16184/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var message = await client.GetAsync("api/FileUpload/Demo");

                return message.Content.ToString();

            }
        }
    }
}