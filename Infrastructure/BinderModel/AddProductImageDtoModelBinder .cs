using System;
using System.IO;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;
using onlineshopowner_api.Application.Dtos;

namespace onlineshopowner_api.Application.ModelBinders
{
    public class AddProductImageDtoModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var request = HttpContext.Current.Request;
            var contentType = request.ContentType?.ToLower() ?? "";

            if (contentType.Contains("multipart/form-data"))
            {
                try
                {
                    var dto = new AddProductImageDto();

                    // Extract form fields
                    dto.logo_url = request.Form["logo_url"];

                    if (int.TryParse(request.Form["shopid"], out int shopId))
                        dto.shopid = shopId;

                    if (int.TryParse(request.Form["productid"], out int productId))
                        dto.productid = productId;

                    // Safely assign uploaded file
                    if (request.Files.Count > 0)
                    {
                        // Try to get by name, fallback to first file
                        dto.File = request.Files["File"] ?? request.Files[0];
                    }

                    bindingContext.Model = dto;
                    return true;
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError("File", $"Failed to bind AddProductImageDto (multipart): {ex.Message}");
                    return false;
                }
            }
            else if (contentType.Contains("application/json") || contentType.Contains("text/json"))
            {
                try
                {
                    var stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
                    stream.Position = 0;

                    using (var reader = new StreamReader(stream))
                    {
                        var json = reader.ReadToEnd();
                        var dto = JsonConvert.DeserializeObject<AddProductImageDto>(json);
                        bindingContext.Model = dto;
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    bindingContext.ModelState.AddModelError("Body", $"Invalid JSON: {ex.Message}");
                    return false;
                }
            }

            bindingContext.ModelState.AddModelError("ContentType", "Unsupported content type. Use multipart/form-data or application/json.");
            return false;
        }
    }
}
