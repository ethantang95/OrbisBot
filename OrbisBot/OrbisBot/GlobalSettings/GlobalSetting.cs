using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrbisBot.GlobalSettings
{
    class GlobalSetting
    {
        public HashSet<ulong> HideList { get; private set; }

        public GlobalSetting()
        {
            PopulateFields();
        }

        private void PopulateFields()
        {
            var hideListContent = FileHelper.GetObjectFromFile<HashSet<ulong>>(Path.Combine(Constants.GLOBAL_SETTINGS_FOLDER, Constants.HIDE_LIST_FILE));

            if (hideListContent != null)
            {
                HideList = hideListContent;
            }
            else
            {
                HideList = new HashSet<ulong>();
            }
        }

        public void AddToHide(ulong id)
        {
            HideList.Add(id);
            SaveSettings();
        }

        public void RemoveFromHide(ulong id)
        {
            HideList.Remove(id);
            SaveSettings();
        }

        private void SaveSettings()
        {
            FileHelper.WriteObjectToFile(Path.Combine(Constants.GLOBAL_SETTINGS_FOLDER, Constants.HIDE_LIST_FILE), HideList);
        }
    }
}
