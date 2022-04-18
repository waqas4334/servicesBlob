/*using StorageService.Util;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using FieldForce.Model.Interfaces;
using System.Threading.Tasks;

namespace StorageService
{
    public class StorageAgent : IStorageHelper
    {
        static string _blobId = "_" + ConfigurationManager.AppSettings["BlobStorge"];
        /// <summary>
        /// container naming rules:
        /// 1.  Blob container names must start with a letter or number, and can contain only letters, numbers, and the dash (-) character.
        /// 2. Every dash (-) character must be immediately preceded and followed by a letter or number.
        /// 3. All letters in a container name must be lowercase.
        /// 4. Blob container names must be from 3 through 63 characters long.
        /// social.msdn.microsoft.com/Forums/azure/en-US/4e40b925-dbc9-458e-9279-ad0cc0b6ee83/error-while-creating-a-blob-container?forum=windowsazuredata&prof=required
        /// </summary>
       // static string _container = ConfigurationManager.AppSettings["blobContainer"];


        public static HttpResponseMessage Store(Stream stream, Uri uri, String container, String fileExtension, String fileName)
        {
            var image = stream;
            if (image == null)
            {
                return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidContent);
            }
            
            try
            {
                if (IsLocalMode())
                {
                    return ResponseHelper.CreateStringResponse(LocalStorageAgent.Store(container, image, fileExtension, uri));
                }
                else if (IsBlobMode())
                {
                    return ResponseHelper.CreateStringResponse(BlobStorageAgent.Store(container, image, fileName, fileExtension));
                }
                else
                {
                    return ResponseHelper.CreateStringResponse(AzureStorageAgent.Store(container, image, fileExtension));
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Storing Image unable to store" + fileName, e);
                return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidPath);
            }
        }


        /// <summary>
        /// store file to container
        /// </summary>
        /// <param name="data">bytes data</param>
        /// <param name="container">container to save to</param>
        /// <param name="fileName">file name to save against</param>
        /// <returns></returns>
        public string StoreFile(byte[] data, String container, String fileName)
        {
            string filePath = string.Empty;
            // return if there is no data to save
            if(data == null || data.Length == 0)
            {
                Logger.LogTrace("storing file: " + fileName, "file is empty");
                return string.Empty;
            }
            try
            {
                filePath = BlobStorageAgent.Store(container, data, fileName);
            }
            catch(Exception e)
            {
                Logger.LogException("Storing Image unable to store" + fileName, e);
            }

            Logger.LogTrace("storing file: " + fileName, "path: " + filePath);
            return filePath;
        }

        public static string StoreLargeFile(Stream stream, String container, String fileExtension, String fileName) 
        {
            var image = stream;
            string filePath = "";
            if (image == null || image.Length == 0)
            {
                Logger.LogTrace("storing file: " + fileName, "file is empty");
                return string.Empty;
            }
            try
            {                
                if (IsLocalMode())
                {
                    filePath = LocalStorageAgent.Store(container, image, fileExtension, null);
                }
                else if (IsBlobMode())
                {
                    filePath = BlobStorageAgent.Store(container, image, fileName, fileExtension);
                }
                //else
                //{
                //    filePath = AzureStorageAgent.Store(container, image, fileExtension);
                //}                
            }
            catch (Exception e)
            {
                Logger.LogException("Storing Image unable to store" + fileName, e);
                filePath = string.Empty;
            }

            Logger.LogTrace("storing file: " + fileName, "path: " + filePath);

            return filePath;
        }

        public static string StoreLargeFileFromDisk(String container, String fileExtension, String directoryPath, String fileName, string blobName = "")
        {
            string filePath = "";
            string path = directoryPath + "\\" + fileName;

            // Checks if the path is valid or not
            if (!File.Exists(path))
            { 
                Logger.LogTrace("storing file: " + fileName, "file does not exist");
                return string.Empty;
            }
            try
            {
                filePath = BlobStorageAgent.StoreFromFile(container, path, fileName, fileExtension, blobName);
            }
            catch (Exception e)
            {
                Logger.LogException("Storing Image unable to store" + fileName, e);
                filePath = string.Empty;
            }

            Logger.LogTrace("storing file: " + fileName, "path: " + filePath);

            return filePath;
        }
        public string StoreFile(Stream stream, String container, String fileExtension, String fileName)
        {
            
            return StoreLargeFile(stream, container, fileExtension, fileName);
        }

        public byte[] GetFile(String container, String path, int isLargeFile)
        {
            if (string.IsNullOrEmpty(container))
            {
                return null;
            }

            if (String.IsNullOrWhiteSpace(path))
            {
                Logger.LogTrace("Retrieve for " + path, "path has length issues");
                return null;
            }
            try
            {
                if (path.IndexOf(_blobId) > -1) //retrirve from blob
                {
                    return GetImagesFromBlobStorage(container, path, isLargeFile);
                }
                else //local read
                {
                    return LocalStorageAgent.Retrieve(container, path);
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Retrieve exception for path: " + path, e);
            }
            return null;
        }

        public static HttpResponseMessage StoreThumbnail(Stream stream, Uri uri, String container, String fileExtension, String fileName)
        {

            using (var image = stream)
            {
                //if (image == null)
                //{
                //    return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidContent);
                //}

                //// arbitrarily chosen number to discount images that appear too small / invalid 
                //if (image.Length < minFileContentLength)
                //{
                //    return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidContent);
                //}

                try
                {
                    if (IsLocalMode())
                    {
                        return ResponseHelper.CreateStringResponse(LocalStorageAgent.StoreThumbnail(container, image, fileExtension));
                    }
                    else if (IsBlobMode())
                    {
                        return ResponseHelper.CreateStringResponse(BlobStorageAgent.StoreThumbnail(container, image, fileName));
                    }
                    else
                    {
                        return ResponseHelper.CreateStringResponse(AzureStorageAgent.StoreThumbnail(container, image, fileExtension));
                    }

                }
                catch (Exception e)
                {
                    Logger.LogException("Storing Image: Unable to store file " + fileName, e);
                    return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidPath);
                }
            }
        }

        public static HttpResponseMessage Retrieve(String container, String path, uint minURLLength, uint maxURLLength)
        {

            if (String.IsNullOrWhiteSpace(path) || path.Length < minURLLength || path.Length > maxURLLength)
            {
                Logger.LogTrace("Retrieve for " + path, "path has length issues");
                return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidPath);
            }


            try
            {
                if (path.IndexOf(_blobId) > -1) //retrieve from blob
                {
                    return ResponseHelper.CreateByteArrayResponse(BlobStorageAgent.Retrieve(container, path));
                }
                else //local read
                {
                    return ResponseHelper.CreateByteArrayResponse(LocalStorageAgent.Retrieve(container, path));
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Retrieve exception for path: " + path, e);
                return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidPath);
            }
        }

        public static HttpResponseMessage Delete(String path, uint minURLLength, uint maxURLLength, string container)
        {
            Logger.LogTrace("Delete", "asked for " + path);


            try
            {
                if (path.IndexOf(_blobId) > -1)
                {
                    BlobStorageAgent.Delete(container, path);
                    return ResponseHelper.CreateEmptyResponse();
                }

                else
                {
                    LocalStorageAgent.Delete(container, path);
                    return ResponseHelper.CreateEmptyResponse();
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Delete exception for path: " + path, e);
                return ResponseHelper.CreateErrorResponse(HttpStatusCode.BadRequest, Errors.InvalidPath);
            }
        }

        private static bool IsLocalMode()
        {
            return Boolean.Parse(ConfigurationManager.AppSettings["WriteLocally"]);
        }
        private static bool IsBlobMode()
        {
            return Boolean.Parse(ConfigurationManager.AppSettings["WritetoBlob"]);
        }

        /// <summary>
        /// gets image stream to upload its thumbnail on the blob
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="container"></param>
        /// <param name="fileExtension"></param>
        /// <param name="fileName"></param>
        /// <returns>file path of thumbnail</returns>
        public static string StoreThumbnails(Stream stream, String container, String fileExtension, String fileName)
        {
            using (var image = stream)
            {

                try
                {
                    if (IsLocalMode())      //checks storage configuration
                    {
                        return LocalStorageAgent.StoreThumbnail(container, image, fileExtension);
                    }
                    else if (IsBlobMode())
                    {
                        return BlobStorageAgent.StoreThumbnail(container, image, fileName) + "." +fileExtension;
                    }
                    else
                    {
                        return AzureStorageAgent.StoreThumbnail(container, image, fileExtension);
                    }

                }
                catch (Exception e)
                {
                    Logger.LogException("Storing Thumbnail Image: Unable to store thumbnail of file " + fileName, e);
                    return e.Message;
                }
            }

        }

        /// <summary>
        /// gets image stream to upload compressed image on the blob
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="container"></param>
        /// <param name="fileExtension"></param>
        /// <param name="fileName"></param>
        /// <returns>file path of thumbnail</returns>
        public static string StoreCompressedImage(Stream stream, String container, String fileExtension, String fileName, long compressedImagesQuality = 40L)
        {
            using (var image = stream)
            {

                try
                {
                    if (IsLocalMode())      //checks storage configuration
                    {
                        return LocalStorageAgent.StoreCompressedImage(container, image, fileExtension, compressedImagesQuality: compressedImagesQuality);
                    }
                    else if (IsBlobMode())
                    {
                        return BlobStorageAgent.StoreCompressedImage(container, image, fileName, compressedImagesQuality) + "." + fileExtension;
                    }
                    else
                    {
                        return AzureStorageAgent.StoreCompressedImage(container, image, fileExtension, compressedImagesQuality: compressedImagesQuality);
                    }

                }
                catch (Exception e)
                {
                    Logger.LogException("Storing Compressed Image: Unable to store compressed image of file " + fileName, e);
                    return null;
                }
            }

        }

        public static Stream RetrieveImages(String container, String path, uint minURLLength = Constants.MinURLLength, uint maxURLLength = Constants.MaxURLLength)
        {
            Stream res = null;
            //check if path is valid
            if (String.IsNullOrWhiteSpace(path) || path.Length < minURLLength || path.Length > maxURLLength)
            {
                Logger.LogTrace("Retrieve for " + path, "path has length issues");
                return null;
            }

            try
            {
                if (path.IndexOf(_blobId) > -1) //retrirve from blob
                {
                    res = new MemoryStream(BlobStorageAgent.Retrieve(container, path));
                    return res;
                }
                else //local read
                {
                    res = new MemoryStream(LocalStorageAgent.Retrieve(container, path));
                    return res;
                }
            }
            catch (Exception e)
            {
                Logger.LogException("Retrieve exception for path: " + path, e);
                return null;
            }
        }
        /// <summary>
        /// created this class to return the original image when its thumbnail is not present on blob storage
        /// </summary>
        /// <param name="container"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static byte[] GetImagesFromBlobStorage(String container, String path, int isLargeFile)
        {
            byte[] img = null;
            if(isLargeFile == 1)
            {
                img = BlobStorageAgent.RetrieveLargeFile(container, path);
            } else
            {
                img = BlobStorageAgent.Retrieve(container, path);
            }
            
            if (img != null)
            {
                return img;
            }
            else
            {
                if (path.Contains("_thumb"))
                {
                    path = path.Replace("_thumb", "");
                    return BlobStorageAgent.Retrieve(container, path);
                }

                return img;
            }
        }

        /// <summary>
        /// created this class to return image when its thumbnail is not present on local storage
        /// </summary>
        /// <param name="container"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        private static byte[] GetImagesFromLocalStorage(String container, String path)
        {
            byte[] img = null;
            img = LocalStorageAgent.Retrieve(container, path);
            if (img != null)
            {
                return img;
            }

            path = path.Replace("_thumb", "");
            return LocalStorageAgent.Retrieve(container, path);

        }
    }
}*/