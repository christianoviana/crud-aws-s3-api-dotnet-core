using CrudAwsS3.Domains.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;


namespace CrudAwsS3.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IS3StorageService amazonS3 { get; set; }
        private const string BUCKET_NAME = "demo-christiano-bucket-aws-s3";

        public FileController(IS3StorageService s3StorageService)
        {
            this.amazonS3 = s3StorageService;
        }

        [HttpGet("{name}")]
        public async Task<IActionResult> Get([FromQuery] string prefix, string name)
        {
            var response = await amazonS3.GetObject(BUCKET_NAME, name, prefix);

            if (response == null)
            {
                return NotFound();
            }

            using Stream stream = response.ResponseStream;
            var ms = new MemoryStream();
            await stream.CopyToAsync(ms);
            ms.Position = 0;

            return new FileStreamResult(ms, response.Headers["Content-Type"])
            {
                FileDownloadName = name
            };     
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromQuery] string prefix, [FromForm] IFormFile file)
        {
            var response = await amazonS3.AddObject(BUCKET_NAME, file, prefix);

            return Ok(response);
        }

        [HttpDelete("{name}")]
        public async Task<IActionResult> Delete([FromQuery] string prefix, string name)
        {
            var response = await amazonS3.DeleteObject(BUCKET_NAME, name, prefix);

            return NoContent() ;
        }
    }
}
