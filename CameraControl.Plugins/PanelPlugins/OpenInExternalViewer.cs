using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.PanelPlugins
{
    public class OpenInExternalViewer : IPanelPlugin
    {
        private UserControl _control;

        public UserControl Control
        {
            get
            {
                if (_control == null)
                    _control = new OpenInExternalViewerControl();
                return _control;
            }
            private set { _control = value; }
        }

        public bool Visible { get; set; }

        public string Id
        {
            get { return "{68BEC560-FF4B-4C76-8562-790206C521DC}"; } 
        }

        public OpenInExternalViewer()
        {
            Visible = true;
        }

        public void Init()
        {
            
        }
    }
}
