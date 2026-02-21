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
    public class Imgur: IImgur
    {
        private readonly string _clientId;

        public Imgur(ImgurSettings imgurSetings)
        {
            _clientId = imgurSetings.ClientId;
        }

        public async Task<( string url, string deleteHash)> UploadImageAsync(Stream imageStream)
        {
           {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", _clientId);

                    using (var content = new MultipartFormDataContent())
                    {
                        var imageContent = new StreamContent(imageStream);
                        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // or "image/png"
                        content.Add(imageContent, "image");

                        var response = await client.PostAsync("https://api.imgur.com/3/image", content);
                        response.EnsureSuccessStatusCode();

                        var resultJson = await response.Content.ReadAsStringAsync();

                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(resultJson);
                        return (result.data.link, result.data.deletehash.ToString());
                    }
                }
            }
           
            
        }
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
