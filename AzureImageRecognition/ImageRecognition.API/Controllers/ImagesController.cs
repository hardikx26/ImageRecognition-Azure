using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ImageRecognition.API.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ImageRecognition.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImagesController : ControllerBase
    {
        private readonly AzureConfig azureConfig;

        public ImagesController(IOptions<AzureConfig> config)
        {
            this.azureConfig = config.Value;
        }

        [HttpPost("tags")]
        public async Task<IActionResult> RetrieveTags([FromForm] IFormFile file1 )
        {

            try
            {
                var tags = new List<string>();
                var client = ComputerVisionClientFactory.Authenticate(this.azureConfig.EndPoint, this.azureConfig.SubscriptionKey);
                var stream = new MemoryStream();
                file1.CopyTo(stream);

                var results = await client.TagImageInStreamWithHttpMessagesAsync(stream);
                foreach (var tag in results.Body.Tags)
                {
                    tags.Add(tag.Name);
                }

                return this.Ok(tags);
            }
            catch (System.Exception ex)
            {
                return this.StatusCode(500, ex);
            }
        }

        [HttpPost("descriptions")]
        public async Task<IActionResult> RetrieveDescription([FromForm] IFormFile file1)
        {

            try
            {
                var client = ComputerVisionClientFactory.Authenticate(this.azureConfig.EndPoint, this.azureConfig.SubscriptionKey);
                var stream = new MemoryStream();
                file1.CopyTo(stream);

                var descriptions = new List<string>();

                var results = await client.DescribeImageInStreamWithHttpMessagesAsync(stream);
                foreach (var caption in results.Body.Captions)
                {
                    descriptions.Add($"description:{caption.Text}, confidence level:{caption.Confidence}");
                }

                return this.Ok(descriptions);
            }
            catch (Exception ex)
            {

                return this.StatusCode(500, ex);
            }

        }
    }
}
