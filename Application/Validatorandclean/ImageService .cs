using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices;
using onlineshopowner_api.Application.Interfaces.Ivalidator;

namespace onlineshopowner_api.Application.Validatorandclean
{
    public class ImageService : IImageService
    {
        private  IImgur _imgur;

        public ImageService(IImgur imgur) 
        {
            _imgur = imgur;
        }
 

        public async Task<(bool issucces, string logourl,string hashdelete)> ProcessImageAsync( int maxFileSizeMB, int maxWidth, int maxHeight,HttpPostedFile file= null, string imageUrl=null)
        {
            Stream imageStream;
            string fileName;
            int maxFileSizeInBytes = maxFileSizeMB * 1024 * 1024;
            if (file != null )
            {
                imageStream = file.InputStream;
                // Check file size
                if (file.ContentLength > maxFileSizeInBytes)
                    return (false, "File size exceeds the maximum allowed size.",null);

                // Check image dimensions
                using (var image = System.Drawing.Image.FromStream(imageStream))
                {
                    if (image.Width > maxWidth || image.Height > maxHeight)
                        return (false, "Image dimensions exceed the allowed limit.",null);
                }

                // Reset stream position before upload
                imageStream.Position = 0;
                fileName = file.FileName;

                // Upload file to cloud and get URL
               (bool issuccess, string Cloudurlorerror ,string deletehash)= await _imgur.UploadImageAsync(imageStream, fileName);
                if (issuccess)
                { 
                    return (true, Cloudurlorerror, deletehash);
                }
                else return (false,Cloudurlorerror,null);
                
            }
            else if (imageUrl !=null)
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.Timeout = TimeSpan.FromSeconds(5);
                    var imageData = await httpClient.GetByteArrayAsync(imageUrl);
                    if (imageData.Length > maxFileSizeInBytes)
                        return (false, "Image too large ",null);

                    using (var ms = new MemoryStream(imageData)) 
                    {
                        try
                        {
                            using (var img = System.Drawing.Image.FromStream(ms))
                            {
                                if (img.Width > maxWidth || img.Height > maxHeight)
                                    return (false, "Remote image dimensions too large",null);



                            }
                        }
                        catch (Exception ex)
                        {
                            return (false, "Invalid or malicious image"+ex.Message,null);
                        }
                        ms.Position = 0;
                        var (issuccess,cloudurlorerror,deletehash)= await _imgur.UploadImageAsync(ms, "remote-image.jpg");
                        if (issuccess) 
                        {
                            return (true, cloudurlorerror, deletehash);
                        }
                        else return (false, cloudurlorerror,null);

                    }
                }
            }

            throw new Exception("No image provided");
        }
       
    }

       
}