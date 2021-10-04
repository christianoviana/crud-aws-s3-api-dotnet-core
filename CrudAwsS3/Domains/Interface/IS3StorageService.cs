using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace CrudAwsS3.Domains.Interface
{
    public interface IS3StorageService
    {
        Task<GetObjectResponse> GetObject(string bucket, string fileName, string prefix = "");
        Task<PutObjectResponse> AddObject(string bucket, IFormFile file, string prefix = "");
        Task<DeleteObjectResponse> DeleteObject(string bucket, string fileName, string prefix = "");
    }
}
