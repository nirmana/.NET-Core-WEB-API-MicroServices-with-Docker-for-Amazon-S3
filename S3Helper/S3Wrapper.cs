using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace S3Helper
{
    public class S3Wrapper
    {
        string aWSUniqueDbKey, AWSAccessKey, AWSSecrteKey;
        static AmazonS3Client s3Client;

        public S3Wrapper(String AWSUniqueDbKey, String AWSAccessKey, String AWSSecrteKey)
        {
            aWSUniqueDbKey = AWSUniqueDbKey;
            AmazonS3Config config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.USWest1;
            s3Client = new AmazonS3Client(
                       AWSAccessKey,
                       AWSSecrteKey,
                       config
                       );
        }

        #region S3Bucket
        public async Task<PutBucketResponse> CreateBucket(String bucketName)
        {
            PutBucketRequest request = new PutBucketRequest();
            request.BucketName = aWSUniqueDbKey + bucketName;
            return await s3Client.PutBucketAsync(request);
        }

        public async Task<ListBucketsResponse> GetBuckets()
        {
            ListBucketsResponse response = await s3Client.ListBucketsAsync();
            return response;
        }

        public async Task<bool> IsExistBucket(String BucketName)
        {
            bool isBucketExist = false;
            ListBucketsResponse response = await GetBuckets();
            foreach (S3Bucket bucket in response.Buckets)
            {
                if (bucket.BucketName == aWSUniqueDbKey + BucketName)
                {
                    isBucketExist = true;
                    break;
                }
            }
            return isBucketExist;
        }

        public async Task<List<S3Object>> GetAllFilesByBucketName(string BucketName)
        {
            ListObjectsRequest request = new ListObjectsRequest();
            request.BucketName = aWSUniqueDbKey + BucketName;
            ListObjectsResponse response = await s3Client.ListObjectsAsync(request);
            return response.S3Objects;
            //if (response.S3Objects.Count >= (page - 1) * size)
            //{
            //    return response.S3Objects.GetRange(((page - 1) * size), size);
            //}
            //else
            //{
            //    return response.S3Objects.GetRange(((page - 1) * size), response.S3Objects.Count);
            //}
        }

        #endregion S3Bucket

        #region S3Object
        public async Task<PutObjectResponse> CreateObject(String FileKey, String BucketName, String FileType, Stream inputStream)
        {
            PutObjectRequest request = new PutObjectRequest();
            request.BucketName = aWSUniqueDbKey + BucketName;
            request.Key = FileKey;
            request.InputStream = inputStream;
            request.CannedACL = S3CannedACL.Private;
            return await s3Client.PutObjectAsync(request);
        }

        public async Task<GetObjectResponse> DownloadObject(String FileKey, String BucketName)
        {
            GetObjectRequest request = new GetObjectRequest();
            request.BucketName = aWSUniqueDbKey + BucketName;
            request.Key = FileKey;
            return await s3Client.GetObjectAsync(request);
        }

        public async Task<DeleteObjectResponse> DeleteObject(String FileKey, String BucketName)
        {
            DeleteObjectRequest request = new DeleteObjectRequest();
            request.BucketName = aWSUniqueDbKey + BucketName;
            request.Key = FileKey;
            return await s3Client.DeleteObjectAsync(request);
        }

        #endregion S3Object
    }
}
