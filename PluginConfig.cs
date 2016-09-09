using Rocket.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PrivateMessage 
{
    public class PluginConfig : IRocketPluginConfiguration
    {
        public string Color;
        public void LoadDefaults()
        {
            Color = "Magenta";
        }
    }
}
