using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLROBS;

namespace DccObsPlugin
{
    public class DccObsImageSourceFactory : AbstractImageSourceFactory
    {
        public DccObsImageSourceFactory()
        {
            ClassName = "DccObsImageSourceClass";
            DisplayName = "digiCamControl Image Source";
        }

        public override ImageSource Create(XElement data)
        {
            return new DccObsImageSource(data);
        }

        public override bool ShowConfiguration(XElement data)
        {
            DccObsConfigurationDialog dialog = new DccObsConfigurationDialog(data);
            return dialog.ShowDialog().GetValueOrDefault(false);
        }
    }
}
