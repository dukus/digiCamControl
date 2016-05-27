using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CameraControl.Core;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Devices.Classes;
using Newtonsoft.Json;

namespace CameraControl.Plugins.ToolPlugins
{
    public class CombineZpPlugin : IToolPlugin, IExecutePlugin
    {
        private CombineZpWindow _window;
        private CombineZpViewModel _viewModel;
        public bool Execute()
        {
            if (_window == null || !_window.IsVisible)
            {
                _window = new CombineZpWindow();
                _viewModel = new CombineZpViewModel(_window);
                _window.DataContext = _viewModel;
                _window.Owner = ServiceProvider.PluginManager.SelectedWindow as Window;
                _window.Show();
            }
            else
            {
                _window.Activate();
            }
            return true;
        }

        public string Title { get; set; }

        public string Id
        {
            get { return "{F3155291-D688-49B8-B22D-E74A2D5E020E}"; }
        }

        public void Init()
        {
            
        }

        public CombineZpPlugin()
        {
            Title = "Combine Zp";
        }

        /// <summary>
        /// Usage
        ///dynamic data = new System.Dynamic.ExpandoObject();
        ///data.Files = Files.Select(x => x.FileName).ToList();
        ///data.ResultFile = newFile;
        ///data.ThumbOnly = true;
        ///var s = JsonConvert.SerializeObject(data);
        ///ServiceProvider.PluginManager.GetExecutePlugin("{F3155291-D688-49B8-B22D-E74A2D5E020E}").Execute(s);
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public string Execute(string command)
        {
            dynamic data = JsonConvert.DeserializeObject(command);
            CombineZpViewModel model = _viewModel ??
                                       new CombineZpViewModel((Window)ServiceProvider.PluginManager.SelectedWindow);
            model.Init();
            model.Files = new AsyncObservableCollection<FileItem>();
            foreach (string file in data.Files)
            {
                model.Files.Add(new FileItem(file));
            }
            model.CopyFiles((bool)data.ThumbOnly);
            model.Combine();
            if (File.Exists(model._resulfile))
                File.Copy(model._resulfile, (string)data.ResultFile);
            return "";
        }
    }
}
