using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using CameraControl.Core;
using CameraControl.Core.Classes;
using GalaSoft.MvvmLight;

namespace CameraControl.Plugins.AutoExportPlugins
{
    public class BasePluginViewModel : ViewModelBase
    {

        protected AutoExportPluginConfig _config = new AutoExportPluginConfig();
        public const string EmptyTransformFilter = "NoTransform";

        public string TransformPlugin
        {
            get
            {
                var pl = _config.ConfigData["TransformPlugin"];
                return string.IsNullOrEmpty(pl) ? EmptyTransformFilter : pl;
            }
            set
            {
                _config.ConfigData["TransformPlugin"] = value;
                RaisePropertyChanged(() => ConfigControl);
            }
        }

        public string Name
        {
            get { return _config.Name; }
            set { _config.Name = value; }
        }

        public UserControl ConfigControl
        {
            get
            {
                var tp = ServiceProvider.PluginManager.GetImageTransformPlugin(TransformPlugin);
                if (tp != null)
                {
                    return tp.GetConfig(_config.ConfigData);
                }
                return null;
            }
        }

        public int ToInt(string name)
        {
            int i = 0;
            int.TryParse(_config.ConfigData[name], out i);
            return i;
        }

        public string TransformTemplate(FileItem item, string text)
        {
            Regex regPattern = new Regex(@"\[(.*?)\]", RegexOptions.Singleline);
            MatchCollection matchX = regPattern.Matches(text);
            return matchX.Cast<Match>().Aggregate(text, (current1, match) => item.FileNameTemplates.Where(template => System.String.Compare(template.Name, match.Value, System.StringComparison.InvariantCultureIgnoreCase) == 0).Aggregate(current1, (current, template) => current.Replace(match.Value, template.Value)));

        }
    }
}
