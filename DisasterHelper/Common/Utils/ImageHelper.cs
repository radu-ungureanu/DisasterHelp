using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utils
{
    public class ImageHelper
    {
        // Used for saving image into database
        public static byte[] GetFileBytes(Stream stream)
        {
            byte[] fileBytes;
            using (BinaryReader br = new BinaryReader(stream))
            {
                fileBytes = br.ReadBytes((int)stream.Length);
            }
            return fileBytes;
        }

        // Used for retrieving image from database
        public static Stream GetFileStream(byte[] fileBytes)
        {
            return new MemoryStream(fileBytes);
        }
    }
}
