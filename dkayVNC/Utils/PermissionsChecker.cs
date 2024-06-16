using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dkayVNC.Utils
{
    internal class PermissionsChecker
    {
        public static bool BoundChannelOnly()
        {
            if (Program.Config.EnforceDefaultChannel)
            {
                if (Program.Config.DefaultChannelId == Program.CurrentBoundChannel.Id)
                    return true;
            }
            return false;
        }
    }
}
