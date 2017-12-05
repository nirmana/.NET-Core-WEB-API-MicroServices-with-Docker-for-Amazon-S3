using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using AppDto;
using Microsoft.AspNetCore.Cors;
using S3Helper;
using System.Net.Http;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace AwsFileDownload.Controllers
{
    [Route("api/[controller]")]
    public class FileDownloadController : Controller
    {
        private readonly IOptions<Appsettings> _appSettings;

        public FileDownloadController(IOptions<Appsettings> appsettings)
        {
            _appSettings = appsettings;


        }



        [HttpPost]
        [Route("GetAllDocumentsExist")]
        [EnableCors("s3Policy")]
        public async Task<ActionResult> GetAllDocumentsExist()
        {
            object message = null;
            try
            {
                S3Wrapper s3Wrapper = new S3Wrapper(_appSettings.Value.AWSUniqueDbKey, _appSettings.Value.AWSAccessKey, _appSettings.Value.AWSSecrteKey);
                var files = await s3Wrapper.GetAllFilesByBucketName("s3");
                return Json(files.ToArray());
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message);
        }


        [HttpPost]
        [Route("DownloadFile")]
        [EnableCors("s3Policy")]
        public async Task<HttpResponseMessage> DownloadFile(string fileKey)
        {
            HttpResponseMessage result = null;
            try
            {
                S3Wrapper s3Wrapper = new S3Wrapper(_appSettings.Value.AWSUniqueDbKey, _appSettings.Value.AWSAccessKey, _appSettings.Value.AWSSecrteKey);
                var response = await s3Wrapper.DownloadObject(fileKey, "s3");

                result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Content = new StreamContent(response.ResponseStream);
                result.Content.Headers.ContentType =
                    new MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.Add("x-filename", fileKey.Split('-')[1]);
                result.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")

                {
                    FileName = fileKey.Split('-')[1]
                };
            }
            catch (Exception)
            {
            }
            return result;
        }


        [HttpPost]
        [Route("DeleteFile")]
        [EnableCors("s3Policy")]
        public async Task<ActionResult> DeleteFile(string fileKey)
        {
            string message = null;
            try
            {
                S3Wrapper s3Wrapper = new S3Wrapper(_appSettings.Value.AWSUniqueDbKey, _appSettings.Value.AWSAccessKey, _appSettings.Value.AWSSecrteKey);
                var status = await s3Wrapper.DeleteObject(fileKey, "s3");
                if (status.HttpStatusCode == System.Net.HttpStatusCode.NoContent)
                {
                    message = "ok";
                }
                else
                {
                    message = status.HttpStatusCode.ToString();
                }

                return Json(message);
            }
            catch (Exception ex)
            {
                message = ex.Message;
            }
            return Json(message);
        }


    }
}
