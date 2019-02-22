using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AWS.Bucket.Images
{

    public class ImageUpload
    {
        private const string bucketName = "trueagency-minotaur-production";
        private static readonly RegionEndpoint bucketRegion = RegionEndpoint.APSoutheast2;

        private string FileName = string.Empty;
        private string FilePath = string.Empty;
        private string ContentType = string.Empty;

        private static IAmazonS3 client;

        public string UploadImage(string filePath)
        {
            var fileInfo = new FileInfo(filePath);
            FileName = fileInfo.Name;
            FilePath = filePath;
            ContentType = Utility.GetMIMEType(FileName);
            client = new AmazonS3Client(ApplicationSettings.AccessKey, ApplicationSettings.SecreteKey, bucketRegion);
            string result = WritingAnObjectAsync().Result;
            return result;
        }

        async Task<string> FindBucketLocationAsync(IAmazonS3 client)
        {
            string bucketLocation=string.Empty;
            try
            {
                
                var request = new GetBucketLocationRequest()
                {
                    BucketName = bucketName
                };
                GetBucketLocationResponse response = await client.GetBucketLocationAsync(request);
                bucketLocation = response.Location.ToString();
                
            }
            catch(Exception ex)
            {
                ;
            }
            return bucketLocation;
        }

        public async Task<string> WritingAnObjectAsync()
        {
            string url = string.Empty;
            string bucketLocation = string.Empty;
            try
            {
                try
                {
                    if (!(await AmazonS3Util.DoesS3BucketExistAsync(client, bucketName)))
                    {
                        var putBucketRequest = new PutBucketRequest
                        {
                            BucketName = bucketName,
                            UseClientRegion = true
                        };

                        PutBucketResponse putBucketResponse = await client.PutBucketAsync(putBucketRequest);
                    }
                    bucketLocation = await FindBucketLocationAsync(client);
                }
                catch (AmazonS3Exception e)
                {
                    Console.WriteLine("Error encountered on server. Message:'{0}' when writing an object", e.Message);
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unknown encountered on server. Message:'{0}' when writing an object", e.Message);
                }


                // 1. Put object-specify only key name for the new object.
                var putRequest1 = new PutObjectRequest
                {
                    BucketName = bucketName,
                    Key = FileName,
                    FilePath = FilePath,
                    ContentType = ContentType,/// "image/jpg",
                    CannedACL = S3CannedACL.PublicRead
                };

                PutObjectResponse response1 = await client.PutObjectAsync(putRequest1);

                GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
                request.BucketName = bucketName;
                request.Key = FileName;
                request.Expires = DateTime.Now.AddYears(5);
                request.Protocol = Protocol.HTTPS;
                url = client.GetPreSignedURL(request);

            }
            catch (AmazonS3Exception e)
            {
                Console.WriteLine(
                        "Error encountered ***. Message:'{0}' when writing an object"
                        , e.Message);
            }
            catch (Exception e)
            {
                Console.WriteLine(
                    "Unknown encountered on server. Message:'{0}' when writing an object"
                    , e.Message);
            }
            return url;
        }
        
    }
}
