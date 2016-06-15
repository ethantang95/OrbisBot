using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrbisBot.Permission;

namespace OrbisBot.TaskPermissions.PermissionBuilders
{
    class FileBasedTaskPermissionBuilder : TaskPermissionBuilderAbstract<FileBasedTaskPermission, FileBasedTaskPermissionBuilder>
    {
        private string _fileSource;

        public FileBasedTaskPermissionBuilder()
        {

        }

        public FileBasedTaskPermissionBuilder SetFileSource(string value) 
        {
            _fileSource = value;
            return this;
        }

        public override FileBasedTaskPermission ConstructTaskTypePermissions(CommandPermission permission)
        {
            if (string.IsNullOrEmpty(_fileSource))
            {
                throw new ArgumentNullException("file source has not been set");
            }
            try
            {
                var readPermission = FileHelper.GetObjectFromFile<CommandPermission>(_fileSource);

                if (readPermission == null)
                {
                    FileHelper.WriteObjectToFile(_fileSource, permission);
                }
                else
                {
                    //we want only the channel permissions so it can be flexible
                    permission.ChannelPermission = readPermission.ChannelPermission;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Problem parsing the settings file, creating default");
                FileHelper.WriteObjectToFile(_fileSource, permission);
            }

            var taskPermmission = new FileBasedTaskPermission(permission, _fileSource);

            return taskPermmission;
        }
    }
}
