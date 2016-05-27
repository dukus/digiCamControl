using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Interfaces;

namespace CameraControl.Plugins.ToolPlugins
{
    public class StatisticsPlugin : IToolPlugin
    {
        public bool Execute()
        {
            var _window = new StatisticsWindow();
            _window.DataContext = new StatisticsViewModel();
            _window.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
            _window.ShowDialog();
            return true;
        }

        public string Title { get; set; }
        
        public string Id
        {
            get { return "{3EC27FCC-9BE3-4252-BA54-9A2284B998DE}"; }
        }

        public void Init()
        {
            
        }

        public StatisticsPlugin()
        {
            Title = "Statistics";
        }
    }
}
