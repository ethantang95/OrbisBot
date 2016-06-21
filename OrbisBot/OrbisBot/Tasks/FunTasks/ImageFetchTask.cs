using OrbisBot.TaskAbstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.TaskPermissions;
using Discord;
using OrbisBot.TaskHelpers.PhotoBucket;

namespace OrbisBot.Tasks.FunTasks
{
    class ImageFetchTask : TaskAbstract
    {
        public ImageFetchTask(FileBasedTaskPermission permission) : base(permission)
        {
        }

        public override string AboutText()
        {
            return "Returns a random image from a source, currently it supports PhotoBucket";
        }

        public override bool CheckArgs(string[] args)
        {
            return args.Length == 2;
        }

        public override string CommandText()
        {
            return "picture";
        }

        public override string TaskComponent(string[] args, MessageEventArgs messageSource)
        {
            return PhotoBucketRandomHelper.GetRandomPicture(args[1], 0, 2);
        }

        public override string UsageText()
        {
            return "(\"search query\")";
        }
    }
}
