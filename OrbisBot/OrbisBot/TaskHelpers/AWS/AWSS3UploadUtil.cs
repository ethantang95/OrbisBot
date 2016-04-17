using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.TaskHelpers.AWS
{
    static class AWSS3UploadUtil
    {

        public static string UploadImage(Image image)
        {
            return UploadImage(image, Guid.NewGuid().ToString());
        }
        public static string UploadImage(Image image, string fileName)
        {
            TransferUtility fileTransferUtility = new
                TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast1));

            using (var fileToUpload = new MemoryStream())
            {
                image.Save(fileToUpload, ImageFormat.Png);
                fileTransferUtility.Upload(fileToUpload, "orbis-bot-s3", fileName + ".png");
            }

            GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest()
            {
                BucketName = "orbis-bot-s3",
                Key = fileName + ".png",
                Expires = DateTime.Now.AddDays(10)
            };

            string url = new AmazonS3Client(Amazon.RegionEndpoint.USEast1).GetPreSignedURL(request1);

            return url;
        }
    }
}
