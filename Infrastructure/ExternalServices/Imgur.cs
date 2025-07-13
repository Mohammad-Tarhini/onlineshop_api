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

namespace onlineshopowner_api.Infrastructure.ExternalServices
{
    public class Imgur: IImgur
    {
        private readonly string _clientId;

        public Imgur(ImgurSettings imgurSetings)
        {
            _clientId = imgurSetings.ClientId;
        }

        public async Task<(bool Isuccess, string url, string deleteHash)> UploadImageAsync(Stream imageStream, string fileName)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", _clientId);

                    using (var content = new MultipartFormDataContent())
                    {
                        var imageContent = new StreamContent(imageStream);
                        imageContent.Headers.ContentType = MediaTypeHeaderValue.Parse("image/jpeg"); // or "image/png"
                        content.Add(imageContent, "image", fileName);

                        var response = await client.PostAsync("https://api.imgur.com/3/image", content);
                        response.EnsureSuccessStatusCode();

                        var resultJson = await response.Content.ReadAsStringAsync();

                        dynamic result = Newtonsoft.Json.JsonConvert.DeserializeObject(resultJson);
                        return (true,result.data.link, result.data.deletehash.ToString());
                    }
                }
            }
            catch (System.Exception ex) 
            {
                return(false,ex.Message,null);
            }
            
        }
        public async Task<(bool IsSuccess, string Message)> DeleteImageAsync(string deleteHash)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Client-ID", _clientId);

                    var response = await client.DeleteAsync($"https://api.imgur.com/3/image/{deleteHash}");

                    if (response.IsSuccessStatusCode)
                    {
                        return (true, "Image deleted successfully");
                    }
                    else
                    {
                        string error = await response.Content.ReadAsStringAsync();
                        return (false, $"Failed to delete image: {error}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                return (false, $"Exception: {ex.Message}");
            }
        }

    }
}
