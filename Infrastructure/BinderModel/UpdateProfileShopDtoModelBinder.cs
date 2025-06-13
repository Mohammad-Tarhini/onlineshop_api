using Newtonsoft.Json;
using onlineshopowner_api.Application.Dtos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;


namespace onlineshopowner_api.Infrastructure.BinderModel
{
    public class UpdateProfileShopDtoModelBinder : IModelBinder
    {

        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var request = HttpContext.Current.Request;
            var contentType = request.ContentType?.ToLower() ?? "";

            if (contentType.Contains("multipart/form-data"))
            {
                if (!int.TryParse(request.Form["shopid"], out var sid))
                {
                    bindingContext.ModelState.AddModelError("shopid", "Invalid or missing shop ID.");
                    return false;
                }
                // Bind from form and files

                var dto = new UpdatProfileShopeDto
                {
                    
                    shopid =sid,
                    logo_url = request.Form["logo_url"],
                    File = request.Files.Count > 0 ? request.Files[0] : null
                }; 

                // Validation: only file or url, not both
                bool hasFile = dto.File != null && dto.File.ContentLength > 0;
                bool hasUrl = !string.IsNullOrWhiteSpace(dto.logo_url);

                if (hasFile && hasUrl)
                {
                    bindingContext.ModelState.AddModelError("Logo", "Provide either a logo URL or upload a file, not both.");
                    return false;
                }
                if (!hasFile && !hasUrl)
                {
                    bindingContext.ModelState.AddModelError("Logo", "You must provide a logo URL or upload a file.");
                    return false;
                }

                bindingContext.Model = dto;
                return true;
            }
            else if (contentType.Contains("application/json") || contentType.Contains("text/json"))
            {
                // Bind normally from JSON body
                // Use the default JSON formatter to deserialize

                var stream = actionContext.Request.Content.ReadAsStreamAsync().Result;
                stream.Position = 0;

                using (var reader = new StreamReader(stream))
                {
                    var json = reader.ReadToEnd();
                    var dto = JsonConvert.DeserializeObject<UpdatProfileShopeDto>(json);

                    if (dto == null)
                    {
                        bindingContext.ModelState.AddModelError("Body", "Invalid JSON body");
                        return false;
                    }

                    // Validate that file is null and logo_url exists (since no file upload in JSON)
                    if (!string.IsNullOrWhiteSpace(dto.logo_url))
                    {
                        bindingContext.Model = dto;
                        return true;
                    }
                    else
                    {
                        bindingContext.ModelState.AddModelError("Logo", "LogoUrl must be provided if no file is uploaded.");
                        return false;
                    }
                }
            }
            else
            {
                bindingContext.ModelState.AddModelError("ContentType", "Unsupported content type");
                return false;
            }
        }

    }
}