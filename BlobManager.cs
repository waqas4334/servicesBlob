/*using System;
using System.Net.Http;
using System.Configuration;
using FieldForceUtil;
using StorageService;
using FieldForce.Model.DTO;
using System.IO;
using FieldForce.Model.Interfaces;

namespace ExportService.Helpers
{
    public static class BlobManager
    {
        private const uint _minFileContentLength = 1;
        private const uint _minURLLength = 5;
        private const uint _maxURLLength = 1024;
        static string _downloadURL = ConfigurationSettings.AppSettings["storageService"] + "images/get?path=";
       
        //private bool shouldValidate = Boolean.Parse(ConfigurationSettings.AppSettings["shouldAuthenticate"]);
       
        /// <summary>
        /// ReportsFolder variable can be used to access the folder which is shared for specific type of reporting
        /// </summary>
        private static string ReportsFolder = ConfigurationSettings.AppSettings["ReportsFolder"];

        public static string UploadZipFile(ExportsFileJSON zipFile,string sharedPathValue,string containerName)
        {
            string zipFolder = ReportsFolder, 
                directoryPath = sharedPathValue + zipFolder,
                filePath = directoryPath + "\\" + zipFile.FileName,
                OriginalImageUrl = string.Empty;

            OriginalImageUrl = StorageAgent.StoreLargeFileFromDisk(containerName, zipFile.fileExtension, directoryPath, zipFile.FileName, zipFile.blobName);

            //deleting the file on disk, no point in keeping the file if it is successfully uploaded to BLOB
            if (!string.IsNullOrEmpty(OriginalImageUrl) && File.Exists(Path.Combine(directoryPath, zipFile.FileName)))
            {
                File.Delete(Path.Combine(directoryPath, zipFile.FileName));
            }
            return OriginalImageUrl + ".zip";
        }

        // FFMVPS-6903, not moving this call to a reference storage service
        public static byte[] GetFileContent(string filePath, string containerName)
        {
            IStorageHelper storageHelper = new StorageAgent();
            string BlobFilePath = filePath.Split(new string[] { "_s" }, StringSplitOptions.None)[0];

            byte[] bytes = storageHelper.GetFile(containerName, BlobFilePath, 0);
            if (bytes != null && bytes.Length > 0)
            {
                return bytes;
            }
            return new byte[0];
        }
        [Obsolete]
        public static string UploadFile(byte[] fileBytes, string fileExtension,string webURL)
        {
            var fileName = Path.GetRandomFileName().Replace(".","");
             string _webURL = webURL + "Image/load?path=";
            // upload Path
            string _uploadURL = ConfigurationSettings.AppSettings["storageService"] + "images/store/" + fileExtension + "/" + fileName;           
            string filePath = String.Empty;

            // prepare and send file to storage service
            using (var httpClient = new HttpClient())
            {
                // pass authentication token for authentication
                httpClient.DefaultRequestHeaders.Add(WebConstants.Token, GenerateToken());

                var response = httpClient.PutAsync(_uploadURL, new ByteArrayContent(fileBytes)).Result;
                //if success from storage service
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    filePath = response.Content.ReadAsStringAsync().Result;
                }
            }

            return _webURL + filePath + "." + fileExtension;
        }

        public static string UploadOriginalFile(byte[] fileBytes, string fileExtension, string containerName)
        {
            var fileName = Path.GetRandomFileName().Replace(".", "");
            using (Stream stream = new MemoryStream(fileBytes))
            {
                var OriginalImageUrl = StorageAgent.StoreLargeFile(stream, containerName, fileExtension, fileName);
                return OriginalImageUrl;
            }
        }

        /// <summary>
        /// Gets Auth Token
        /// </summary>
        /// <param name="filePath">file name</param>
        /// <returns>Auth Token</returns>
        private static string GenerateToken()
        {
            string token = "--";
            token += WebConstants.KeysSeparator + "--" + WebConstants.ValuesSeparator + "--";
            token += WebConstants.KeysSeparator + "--" + WebConstants.ValuesSeparator + "--";
            token += WebConstants.KeysSeparator + "generated" + WebConstants.ValuesSeparator + DateTime.Now.ToString("s");
            token += WebConstants.KeysSeparator + "expiry" + WebConstants.ValuesSeparator + DateTime.Now.AddDays(WebConstants.TokenExpiry).ToString("s");
            token += WebConstants.KeysSeparator + "--" + WebConstants.ValuesSeparator + "--";
            token += WebConstants.KeysSeparator + "--" + WebConstants.ValuesSeparator + "--";
            token = Encryption.Encrypt(token);

            return token;
        }
    }
}
*/