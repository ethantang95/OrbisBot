using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using WolframAlphaNET;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.IO;
using System.Drawing.Imaging;
using Amazon.S3.Model;
using OrbisBot.Permission;

namespace OrbisBot.Tasks
{
    class WolframAlphaTask : TaskAbstract
    {
        public override CommandPermission DefaultCommands()
        {
            return new CommandPermission(false, PermissionLevel.User, false);
        }

        public override string PermissionFileSource()
        {
            return Constants.WOLFRAMALPHA_SETTINGS_FILE;
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length != 2)
            {
                return "This command's syntax is: !Wolfram \"<search query>\"";
            }

            var wolframTask = new WARequest();

            var image = wolframTask.WolframRequest(args[1]);

            if (image == null)
            {
                return "Wolfram did not find any search result for this query";
            }

            try
            {

                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client(Amazon.RegionEndpoint.USEast1));

                using (var fileToUpload = new MemoryStream())
                {
                    image.Save(fileToUpload, ImageFormat.Png);
                    fileTransferUtility.Upload(fileToUpload,
                                               "orbis-bot-s3", args[1] + "-WARequest.png");
                }

                GetPreSignedUrlRequest request1 = new GetPreSignedUrlRequest()
                {
                    BucketName = "orbis-bot-s3",
                    Key = args[1] + "-WARequest.png",
                    Expires = DateTime.Now.AddDays(10)
                };

                string url = new AmazonS3Client(Amazon.RegionEndpoint.USEast1).GetPreSignedURL(request1);

                return url;
            }
            catch (AmazonS3Exception s3Exception)
            {
                return "Error: " + s3Exception.Message;
            }
        }
    }
}
