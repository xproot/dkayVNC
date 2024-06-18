using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkayVNC.Utils
{
    internal class PermissionData
    {
        public static List list;
        private static string jsonFile = "list.json";

        public static void Init()
        {
            // Load/create data
            if (File.Exists(jsonFile))
                list = JsonConvert.DeserializeObject<List>(File.ReadAllText(jsonFile));
            else
            {
                list = new List();
                Log.Warning("No list.json found. Making a new one...");
                File.WriteAllText(jsonFile, JsonConvert.SerializeObject(list, Formatting.Indented));
            }
        }

        public static void Add(ulong id)
        {
            list.IDs.Add(id);
            Save();
        }

        public static void Remove(ulong id)
        {
            list.IDs.Remove(id);
            Save();
        }

        public static bool IsOnList(ulong id)
        {
            return list.IDs.Contains(id);
        }

        public static bool CheckIfAllowed(ulong id, ulong cid)
        {
            if (PermissionsChecker.IsOwner(id))
                return true;
            if (Program.Config.EnforceDefaultChannel == true && cid != Program.CurrentBoundChannel.Id)
                return false;
            if (!list.Enabled)
                return true;
            if (list.IDs.Contains(id) && list.Enabled && !list.Blacklist)
                return true;
            if (!list.IDs.Contains(id) && list.Enabled && list.Blacklist)
                return true;
            return false;
        }

        public static void ToggleOn()
        {
            list.Enabled = !list.Enabled;
            Save();
        }

        public static void ToggleMode()
        {
            list.Blacklist = !list.Blacklist;
            Save();
        }

        internal static void Save()
        {
            File.WriteAllText(jsonFile, JsonConvert.SerializeObject(list, Formatting.Indented));
        }
    }

    public class List
    {
        public bool Enabled { get; set; } = true;
        public bool Blacklist { get; set; } = false;
        public List<ulong> IDs { get; set; } = new List<ulong>();
    }
}
