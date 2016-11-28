using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GoogleDrive.WebApi.GoogleDrive
{
    public class ByteArrayToHttpPostedFileBase: HttpPostedFileBase
    {
        private readonly byte[] fileBytes;
        private Stream inputStream;
        public ByteArrayToHttpPostedFileBase(byte[] byteArray, string fileName=null)
        {
            this.fileBytes = byteArray;
            this.FileName = fileName;
            this.InputStream = new MemoryStream(byteArray);
        }
        public override int ContentLength => fileBytes.Length;

        public override string FileName { get; }

        public override Stream InputStream { get; }
    }
}