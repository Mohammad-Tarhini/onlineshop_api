using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace onlineshopowner_api.Application.Interfaces.Ivalidator
{
    public interface IImageService
    {
        Task<(bool issucces, string logourl, string hashdelete)> ProcessImageAsync(int maxFileSizeMB, int maxWidth, int maxHeight, HttpPostedFile file = null, string imageUrl = null);

    }
}
