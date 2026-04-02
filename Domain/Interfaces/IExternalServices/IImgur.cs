using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;


namespace onlineshopowner_api.Domain.Interfaces.IExternalServices
{
    public  interface IImgur
    {
        Task<(string url, string deleteHash)> UploadImageAsync(Stream imageStream);
        Task<string> DeleteImageAsync(string deleteHash);
    }
}
