using Google.Apis.Drive.v2.Data;
using GoogleDrive.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GoogleDrive.MVC.UtilityMethods
{
    public class WebApiCall
    {
        public async Task<GoogleDriveFile> PostToGoogleDrive(HttpPostedFileBase file)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:16184/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                //converting byte to array
                byte[] byteArray = new byte[file.ContentLength];
                file.InputStream.Read(byteArray, 0, file.ContentLength);

                //converting HttpPostedfileBase to json
                string jsonConvertedFile = JsonConvert.SerializeObject(byteArray);

                StringContent content = new StringContent(jsonConvertedFile, Encoding.UTF8, "application/json");
                //sends file name along with querystring
                HttpResponseMessage message = await client.PostAsync("api/FileToDrive/SaveFile?fileName=" + file.FileName, content);

                if (message.StatusCode == HttpStatusCode.OK)
                {
                    //Problem Arise Here as Some extra chareters added to id
                    string responseContent = await message.Content.ReadAsStringAsync();
                    //Removing the slashes from both the end e.g. "\"0B2pC99R_P4taZlFPRnJEbENVbTA\""

                    GoogleDriveFile uploadedFile = JsonConvert.DeserializeObject<GoogleDriveFile>(responseContent);

                    return uploadedFile;
                }
                return null;
            }

        }

        public async Task<GoogleDriveFile> GetFile(string fileId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:16184/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                string url = string.Format($"api/FileToDrive/GetFile?fileId={fileId}");

                HttpResponseMessage message = await client.GetAsync(url);

                if (message.StatusCode == HttpStatusCode.OK)
                {
                    var file = JsonConvert.DeserializeObject<GoogleDriveFile>(await message.Content.ReadAsStringAsync());
                    return file;
                }
                return null;
            }
        }
        public async Task<HttpResponseMessage> GetAllFiles()
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:16184/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage message = await client.GetAsync("api/FileToDrive/GetAllFiles").ConfigureAwait(false);

                if (message.StatusCode == HttpStatusCode.OK)
                {
                    return message;
                }
                return null;
            }
        }

        public async Task<string> DeleteFile(string fileId)
        {
            using (var Client = new HttpClient())
            {
                Client.BaseAddress = new Uri("http://localhost:16184/");
                Client.DefaultRequestHeaders.Accept.Clear();
                Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                var result = Client.DeleteAsync($"api/FileToDrive/Delete/{fileId}").Result;
                if (result.StatusCode == HttpStatusCode.OK)
                    return "deleted";
                else if (result.StatusCode == HttpStatusCode.InternalServerError)
                    return "internal server error";

                return "error";
            }
        }
        public async Task<string> EditFile(string fileId, string Title, string OriginalFileName, string Description)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://localhost:16184/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                HttpResponseMessage message =
                    await client.PostAsync($"api/EditFile/SaveEdited/{fileId}/{Description}/{OriginalFileName}/{Title}", null);

                if (message.StatusCode == HttpStatusCode.OK)
                {
                    return "success";
                }
                return null;
            }
        }
    }
}

//String fileId, String newTitle,
//      String newDescription, String newMimeType, String newFilename