using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLROBS;

namespace DccObsPlugin
{
    public class DccObsPlugin : AbstractPlugin
    {
        public DccObsPlugin()
        {
            Name = "digiCamControl Plugin";
            Description = "Set Nikon or Canon DSLR camera as stream source";
        }

        public override bool LoadPlugin()
        {
            API.Instance.AddImageSourceFactory(new DccObsImageSourceFactory());
            API.Instance.AddSettingsPane(new DccObsSettingsPane());
            return true;
        }
    }
}
