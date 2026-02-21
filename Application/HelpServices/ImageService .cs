using onlineshopowner_api.Application.Interfaces.Ivalidator;
using onlineshopowner_api.Domain.Interfaces.IExternalServices;
using onlineshopowner_api.Infrastructure.ExternalServices;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Validatorandclean
{
   
        public class ImageService : IImageService
        {
            private readonly IImgur _imgur;

            public ImageService(IImgur imgur)
            {
                _imgur = imgur;
            }

            public async Task<(string logourl, string hashdelete)> ProcessImageAsync(
                int maxFileSizeMB,
                int maxWidth,
                int maxHeight,
                HttpPostedFile file = null,
                string imageUrl = null)
            {
                if (file == null && string.IsNullOrWhiteSpace(imageUrl))
                    throw new Exception("No image provided.");

                int maxBytes = maxFileSizeMB * 1024 * 1024;

                MemoryStream stream = file != null
                    ? GetStreamFromFile(file, maxBytes)
                    : await DownloadImageSafelyAsync(imageUrl, maxBytes);

                ValidateImage(stream, maxWidth, maxHeight);

                var cleanStream = ReEncodeToJpeg(stream);

                string safeFileName = Guid.NewGuid() + ".jpg";

                var (url, deleteHash) = await _imgur.UploadImageAsync(cleanStream, safeFileName);

                if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(deleteHash))
                    throw new Exception("Cloud upload failed.");

                return (url, deleteHash);
            }

            // ---------------- helpers ----------------

            private MemoryStream GetStreamFromFile(HttpPostedFile file, int maxBytes)
            {
                if (file == null || file.ContentLength == 0)
                    throw new Exception("No file provided.");

                if (file.ContentLength > maxBytes)
                    throw new Exception("File too large.");

                var ms = new MemoryStream();
                file.InputStream.CopyTo(ms);
                ms.Position = 0;
                return ms;
            }

            private void ValidateImage(Stream stream, int maxWidth, int maxHeight)
            {
                try
                {
                    using (var image = System.Drawing.Image.FromStream(stream, true, true))
                    {
                        if (image.Width > maxWidth || image.Height > maxHeight)
                            throw new Exception("Image dimensions too large.");

                        long pixels = (long)image.Width * image.Height;
                        if (pixels > 40_000_000)
                            throw new Exception("Image too complex.");
                    }

                    stream.Position = 0;
                }
                catch
                {
                    throw new Exception("Invalid or corrupted image.");
                }
            }

            private MemoryStream ReEncodeToJpeg(Stream original)
            {
                var cleanStream = new MemoryStream();

                using (var image = System.Drawing.Image.FromStream(original))
                {
                    image.Save(cleanStream, System.Drawing.Imaging.ImageFormat.Jpeg);
                }

                cleanStream.Position = 0;
                return cleanStream;
            }
        



        public async Task<MemoryStream> DownloadImageSafelyAsync(
        string imageUrl,
        int maxBytes,
        int timeoutSeconds = 5)
        {
            if (!Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri))
                throw new Exception("Invalid URL");

            // allow only http/https
            if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                throw new Exception("Only HTTP/HTTPS allowed");

            // 🚨 SSRF Protection – block localhost & private IPs
            var addresses = await Dns.GetHostAddressesAsync(uri.Host);
            foreach (var ip in addresses)
            {
                if (IsPrivateIp(ip))
                    throw new Exception("Access to internal network is forbidden");
            }

            using (var http = new HttpClient())
            {
                http.Timeout = TimeSpan.FromSeconds(timeoutSeconds);

                // try HEAD first → check size before download
                var headRequest = new HttpRequestMessage(HttpMethod.Head, uri);
                var headResponse = await http.SendAsync(headRequest);

                if (headResponse.Content.Headers.ContentLength.HasValue &&
                    headResponse.Content.Headers.ContentLength.Value > maxBytes)
                {
                    throw new Exception("Image too large");
                }

                // download
                var response = await http.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead);
                response.EnsureSuccessStatusCode();

                // content type validation
                var contentType = response.Content.Headers.ContentType?.MediaType;
                if (contentType == null || !contentType.StartsWith("image/"))
                    throw new Exception("URL is not an image");

                // read with limit
                var stream = await response.Content.ReadAsStreamAsync();
                var ms = new MemoryStream();

                var buffer = new byte[8192];
                int total = 0;
                int read;

                while ((read = await stream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    total += read;
                    if (total > maxBytes)
                        throw new Exception("Image too large");

                    ms.Write(buffer, 0, read);
                }

                ms.Position = 0;
                return ms;
            }
        }
        private bool IsPrivateIp(IPAddress ip)
        {
            if (IPAddress.IsLoopback(ip))
                return true;

            var bytes = ip.GetAddressBytes();

            // 10.0.0.0 – 10.255.255.255
            if (bytes[0] == 10)
                return true;

            // 172.16.0.0 – 172.31.255.255
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31)
                return true;

            // 192.168.0.0 – 192.168.255.255
            if (bytes[0] == 192 && bytes[1] == 168)
                return true;

            return false;
        }

    }
    }