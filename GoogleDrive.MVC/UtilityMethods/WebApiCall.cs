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
        public async Task<string> PostToGoogleDrive(HttpPostedFileBase file)
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
                    string id = await message.Content.ReadAsStringAsync();
                    return id;
                }
                return message.StatusCode.ToString();
            }

        }

        public async Task<HttpResponseMessage> getAllFiles()
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
    }
}