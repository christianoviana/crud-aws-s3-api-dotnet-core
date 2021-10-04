using Amazon.S3;
using Amazon.S3.Model;
using CrudAwsS3.Domains.Interface;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace CrudAwsS3.Services
{
    public class S3StorageService : IS3StorageService
    {
        private IAmazonS3 amazonS3 { get; set; }

        public S3StorageService(IAmazonS3 amazonS3)
        {
            this.amazonS3 = amazonS3;
        }

        public async Task<GetObjectResponse> GetObject(string bucket, string fileName, string prefix = "")
        {
            try
            {
                string key = this.GenerateS3Key(fileName, prefix);

                var _object = new GetObjectRequest()
                {
                    BucketName = bucket,
                    Key = key
                };

                bool hasFile = await FileExists(bucket, key);

                if (!hasFile)
                    return null;

                var response = await amazonS3.GetObjectAsync(_object);
                return response;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<PutObjectResponse> AddObject(string bucket, IFormFile file, string prefix = "")
        {
            try
            {
                string key = this.GenerateS3Key(file.FileName, prefix);

                var _object = new PutObjectRequest()
                {
                    BucketName = bucket,
                    Key = key,
                    InputStream = file.OpenReadStream(),
                    ContentType = file.ContentType
                };

                var response = await this.amazonS3.PutObjectAsync(_object);
                return response;
            }
            catch (Exception)
            {
                throw;
            }           
        }      

        public async Task<DeleteObjectResponse> DeleteObject(string bucket, string fileName, string prefix = "")
        {
            try
            {
                string key = this.GenerateS3Key(fileName, prefix);

                var _object = new DeleteObjectRequest()
                {
                    BucketName = bucket,
                    Key = key
                };

                var response = await this.amazonS3.DeleteObjectAsync(_object);
                return response;
            }
            catch (Exception)
            {
                throw;
            }          
        }   
        
        private async Task<bool> FileExists(string bucket, string key)
        {
            var request = new ListObjectsRequest
            {
                BucketName = bucket,
                Prefix = key,
                MaxKeys = 1
            };

            var response = await amazonS3.ListObjectsAsync(request);

            return response.S3Objects.Any();
        }

        private string GenerateS3Key(string fileName, string prefix = "")
        {
            return String.IsNullOrEmpty(prefix) ? fileName : (prefix.EndsWith("/") ? $"{prefix}{fileName}" : $"{prefix}/{fileName}");
        }
    }
}
