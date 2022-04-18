using System;
using System.Threading.Tasks;

namespace C_sharp_init
{
    internal class CloudBlockBlob
    {
        private Uri uri;

        public CloudBlockBlob(Uri uri)
        {
            this.uri = uri;
        }

        internal Task UploadFromFileAsync(string v)
        {
            throw new NotImplementedException();
        }
    }
}