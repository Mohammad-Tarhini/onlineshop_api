using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public interface IImgur
    {
        Task<(bool Isuccess, string url, string deleteHash)> UploadImageAsync(Stream imageStream, string fileName);
        Task<(bool IsSuccess, string Message)> DeleteImageAsync(string deleteHash);
        }
}
