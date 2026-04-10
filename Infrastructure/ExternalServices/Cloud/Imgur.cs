using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Linq.Expressions;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.App_Start.Setting;
using System.Web.UI.WebControls;

namespace onlineshopowner_api.Infrastructure.ExternalServices
{
    public class Imgur:IImgur
    {
        private readonly string _clientId;

        public Imgur(ImgurSettings imgurSetings)
        {
            _clientId = imgurSetings.ClientId;
        }
        public async Task<(string url, string deleteHash)> UploadImageAsync(Stream imageStream)
        {
            using (var client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);

                client.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Client-ID", _clientId);

                imageStream.Position = 0;

                using (var content = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(imageStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                    content.Add(streamContent, "image", "upload.jpg");

                    var response = await client.PostAsync("https://api.imgur.com/3/image", content);

                    var resultJson = await response.Content.ReadAsStringAsync();

                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception($"Imgur upload failed: {response.StatusCode} - {resultJson}");
                    }

                    dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(resultJson);

                    return (result.data.link.ToString(), result.data.deletehash.ToString());
                }
            }
        }

        //public async Task<(string url, string deleteHash)> UploadImageAsync(Stream imageStream)
        //{
        //    using (var client = new HttpClient())
        //    {

        //        client.Timeout = TimeSpan.FromSeconds(15);

        //        client.DefaultRequestHeaders.Authorization =
        //            new AuthenticationHeaderValue("Client-ID", _clientId);

        //        imageStream.Position = 0;

        //        using (var ms = new MemoryStream())
        //        {
        //            await imageStream.CopyToAsync(ms);
        //            var base64Image = Convert.ToBase64String(ms.ToArray());

        //            var content = new FormUrlEncodedContent(new[]
        //            {
        //        new KeyValuePair<string, string>("image", base64Image)
        //    });

        //            var response = await client.PostAsync("https://api.imgur.com/3/image", content);

        //            var resultJson = await response.Content.ReadAsStringAsync();

        //            if (!response.IsSuccessStatusCode)
        //            {
        //                throw new Exception($"Imgur upload failed: {response.StatusCode} - {resultJson}");
        //            }

        //            dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(resultJson);

        //            return (result.data.link.ToString(), result.data.deletehash.ToString());
        //        }
        //    }
        //}


        public async Task< string > DeleteImageAsync(string deleteHash)
        {
           
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", _clientId);
                    if (string.IsNullOrWhiteSpace(deleteHash))
                    {
                       throw new Exception( "DeleteHash is empty or invalid");
                    }

                    var response = await client.DeleteAsync($"https://api.imgur.com/3/image/{deleteHash}");
                     response.EnsureSuccessStatusCode();

                    if (response.IsSuccessStatusCode)
                    {
                        return  "Image deleted successfully";
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                       throw new Exception ( $"Failed to delete image. StatusCode: {response.StatusCode}, Response: {error}");
                    }
                }
          
        }

    }
}
