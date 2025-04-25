using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Interface
{
    public interface IImageHelperService
    {
        string ConvertToBase64(byte[]? imageData, string? fileExtension);
        byte[]? ConvertToByte(string? base64String);
    }

}
