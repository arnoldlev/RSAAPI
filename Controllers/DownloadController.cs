using Amazon.S3.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Mvc;

namespace RSAAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        private readonly IAmazonS3 _s3Client;
        private const string BucketName = "rsabotsbeta";

        public DownloadController(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        [HttpGet("generate-url/{platform}")]
        public IActionResult GeneratePresignedUrl(string platform)
        {
            string keyName;

            if (platform == "macarm")
            {
                keyName = "RSABotMACARM.zip";
            }
            else if (platform == "macintel")
            {
                keyName = "RSABotMACIntel.zip";
            }
            else
            {
                keyName = "RSABotWindows.zip";
            }

            var request = new GetPreSignedUrlRequest
            {
                BucketName = BucketName,
                Key = keyName,
                Expires = DateTime.UtcNow.AddMinutes(5)
            };

            string url = _s3Client.GetPreSignedURL(request);

            return Ok(new { url });
        }
    }

}
