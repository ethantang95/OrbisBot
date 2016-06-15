using System;
using Discord;
using Amazon.S3.Transfer;
using Amazon.S3;
using System.IO;
using System.Drawing.Imaging;
using Amazon.S3.Model;
using OrbisBot.Permission;
using OrbisBot.TaskHelpers.WolframAlpha;
using OrbisBot.TaskAbstracts;
using OrbisBot.TaskHelpers.AWS;
using OrbisBot.TaskPermissions;

namespace OrbisBot.Tasks
{
    class WolframAlphaTask : TaskAbstract
    {
        public WolframAlphaTask(FileBasedTaskPermission permission) : base(permission)
        {

        }

        public override string CommandText()
        {
            return "wolfram";
        }

        public override string AboutText()
        {
            return "Asks WolframAlpha for results to a question";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            if (args.Length != 2)
            {
                return "This command's syntax is: \"<search query>\"";
            }

            var wolframTask = new WARequest();

            var image = wolframTask.WolframRequest(args[1]);

            if (image == null)
            {
                return "Wolfram did not find any search result for this query";
            }

            var url = AWSS3UploadUtil.UploadImage(image, args[1]);

            return url;
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length <= 2;
        }

        public override string UsageText()
        {
            return "(\"search query\")";
        }
    }
}
