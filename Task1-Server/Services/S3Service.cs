using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace SafeEscape.Services
{
    public class S3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;

        public S3Service(IAmazonS3 s3Client, string bucketName)
        {
            _s3Client = s3Client;
            _bucketName = bucketName;
        }

        // Uploads a file to S3 with the specified key and returns the URL of the uploaded file.
        public async Task<string> UploadFileAsync(IFormFile file, string key)
        {
            using (var newMemoryStream = new MemoryStream())
            {
                // Copy the file content to a memory stream.
                file.CopyTo(newMemoryStream);

                var putRequest = new PutObjectRequest
                {
                    BucketName = _bucketName,
                    Key = key,
                    InputStream = newMemoryStream,
                    ContentType = file.ContentType
                };

                var response = await _s3Client.PutObjectAsync(putRequest);

                if (response.HttpStatusCode != System.Net.HttpStatusCode.OK)
                {
                    throw new Exception("Error uploading file to S3");
                }
            }

            // Return the URL of the uploaded file.
            return $"https://{_bucketName}.s3.amazonaws.com/{key}";
        }

        // Deletes a file from S3 with the specified key.
        public async Task DeleteFileAsync(string key)
        {
            var deleteObjectRequest = new DeleteObjectRequest
            {
                BucketName = _bucketName,
                Key = key
            };

            await _s3Client.DeleteObjectAsync(deleteObjectRequest);
        }
    }
}
