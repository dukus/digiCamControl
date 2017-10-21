using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Xml.Serialization;
using Capture.Workflow.Core.Annotations;

namespace Capture.Workflow.Core.Classes
{
    public class BaseItem: INotifyPropertyChanged
    {
        public PluginInfo PluginInfo { get; set; }

        [XmlAttribute]
        public string Name
        {
            get { return Properties["(Name)"]?.Value; }
            set { Properties["(Name)"].Value = value; }
        }

        public string SettingData { get; set; }
        public CustomPropertyCollection Properties { get; set; }


        public void SetSize(FrameworkElement element, Context context)
        {
            if (Properties["Width"].ToInt(context) > 0)
                element.Width = Properties["Width"].ToInt(context);
            if (Properties["Height"].ToInt(context) > 0)
                element.Height = Properties["Height"].ToInt(context);
            element.Margin = new Thickness(Properties["Margins"].ToInt(context));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
