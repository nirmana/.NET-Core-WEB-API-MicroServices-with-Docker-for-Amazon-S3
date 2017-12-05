using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using AppDto;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http.Headers;
using S3Helper;
using Microsoft.AspNetCore.Cors;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AwsFileUpload.Controllers
{
    [Route("api/[controller]")]
    public class FileUploadController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;

        public FileUploadController(IOptions<Appsettings> appsettings)
        {
            _appSettings = appsettings;
        }

        [HttpPost]
        [Route("UploadFilesAjax")]
        [EnableCors("s3Policy")]
        public async Task<ActionResult> UploadFilesAjax()
        {
            string message = string.Empty;
            try
            {
                long size = 0;
                var files = Request.Form.Files;
                foreach (var file in files)
                {
                    
                    var filename = ContentDispositionHeaderValue
                                    .Parse(file.ContentDisposition)
                                    .FileName
                                    .Trim('"');
                    size += file.Length;
                    using (Stream fs = file.OpenReadStream())
                    {
                        S3Wrapper s3Wrapper = new S3Wrapper(_appSettings.Value.AWSUniqueDbKey, _appSettings.Value.AWSAccessKey, _appSettings.Value.AWSSecrteKey);
                        bool isBucketAvailable = await s3Wrapper.IsExistBucket("s3");
                        if (!isBucketAvailable) { await s3Wrapper.CreateBucket("s3"); }
                        var response = await s3Wrapper.CreateObject(
                            new Random().Next(50000, 100000)+"-"+ filename,
                            "s3",
                            Path.GetExtension(file.FileName),
                            fs as Stream);

                        if (response.HttpStatusCode == System.Net.HttpStatusCode.OK)
                        {
                            message = "ok";
                        }
                        else
                        {
                            message = response.HttpStatusCode.ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                message = ex.Message;
                //throw;
            }
          
            return Content(message);
        }
    }
}
