#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using CameraControl.Core.Classes;
using CameraControl.Core.Interfaces;
using CameraControl.Core.Plugin;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using FileInfo = System.IO.FileInfo;

#endregion

namespace CameraControl.Core
{
    public class PluginManager : BaseFieldClass
    {
        public AsyncObservableCollection<PluginInfo> AvaiablePlugins { get; set; }

        private AsyncObservableCollection<IPlugin> _plugins;

        public AsyncObservableCollection<IPlugin> Plugins
        {
            get { return _plugins; }
            set
            {
                _plugins = value;
                NotifyPropertyChanged("Plugins");
            }
        }

        private AsyncObservableCollection<IExportPlugin> _exportPlugins;

        public AsyncObservableCollection<IExportPlugin> ExportPlugins
        {
            get { return _exportPlugins; }
            set
            {
                _exportPlugins = value;
                NotifyPropertyChanged("ExportPlugins");
            }
        }

        private AsyncObservableCollection<IMainWindowPlugin> _mainWindowPlugins;

        public AsyncObservableCollection<IMainWindowPlugin> MainWindowPlugins
        {
            get { return _mainWindowPlugins; }
            set
            {
                _mainWindowPlugins = value;
                NotifyPropertyChanged("MainWindowPlugins");
            }
        }

        public IMainWindowPlugin SelectedWindow { get; set; }
        
        private AsyncObservableCollection<IToolPlugin> _toolPlugins;
        private AsyncObservableCollection<IAutoExportPlugin> _autoExportPlugins;
        private AsyncObservableCollection<IImageTransformPlugin> _imageTransformPlugins;
        private AsyncObservableCollection<IPanelPlugin> _panelPlugins;
        private AsyncObservableCollection<IPanelPlugin> _toolBarPlugins;

        public AsyncObservableCollection<IToolPlugin> ToolPlugins
        {
            get { return _toolPlugins; }
            set
            {
                _toolPlugins = value;
                NotifyPropertyChanged("ToolPlugins");
            }
        }

        public AsyncObservableCollection<IAutoExportPlugin> AutoExportPlugins
        {
            get { return _autoExportPlugins; }
            set
            {
                _autoExportPlugins = value;
                NotifyPropertyChanged("AutoExportPlugins");
            }
        }

        public AsyncObservableCollection<IImageTransformPlugin> ImageTransformPlugins
        {
            get { return _imageTransformPlugins; }
            set
            {
                _imageTransformPlugins = value;
                NotifyPropertyChanged("ImageTransformPlugins");
            }
        }

        public AsyncObservableCollection<IPanelPlugin> PanelPlugins
        {
            get { return _panelPlugins; }
            set
            {
                _panelPlugins = value;
                NotifyPropertyChanged("PanelPlugins");
            }
        }

        public AsyncObservableCollection<IPanelPlugin> ToolBarPlugins
        {
            get { return _toolBarPlugins; }
            set
            {
                _toolBarPlugins = value;
                NotifyPropertyChanged("ToolBarPlugins");
            }
        }

        public string PluginsFolder
        {
            get { return Path.Combine(Settings.DataFolder, "Plugins"); }
        }

        public string PluginsFolderInInstallFolder
        {
            get { return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Plugins"); }
        }


        public PluginManager()
        {
            Plugins = new AsyncObservableCollection<IPlugin>();
            ExportPlugins = new AsyncObservableCollection<IExportPlugin>();
            ToolPlugins = new AsyncObservableCollection<IToolPlugin>();
            MainWindowPlugins = new AsyncObservableCollection<IMainWindowPlugin>();
            AvaiablePlugins = new AsyncObservableCollection<PluginInfo>();
            AutoExportPlugins = new AsyncObservableCollection<IAutoExportPlugin>();
            ImageTransformPlugins = new AsyncObservableCollection<IImageTransformPlugin>();
            PanelPlugins = new AsyncObservableCollection<IPanelPlugin>();
            ToolBarPlugins = new AsyncObservableCollection<IPanelPlugin>();
        }

        public IAutoExportPlugin GetAutoExportPlugin(string type)
        {
            return AutoExportPlugins.FirstOrDefault(x => x.Name == type);
        }

        public IImageTransformPlugin GetImageTransformPlugin(string type)
        {
            return ImageTransformPlugins.FirstOrDefault(x => x.Name == type);
        }

        /// <summary>
        /// Copies the plugins folders from program files to programdata folder.
        /// 
        /// </summary>
        public void CopyPlugins()
        {
            if (!Directory.Exists(PluginsFolderInInstallFolder))
                return;
            if (!Directory.Exists(PluginsFolder))
                Directory.CreateDirectory(PluginsFolder);
            string[] folders = Directory.GetDirectories(PluginsFolderInInstallFolder);
            foreach (string folder in folders)
            {
                CopyFilesRecursively(new DirectoryInfo(folder),
                                     new DirectoryInfo(Path.Combine(PluginsFolder, Path.GetFileName(folder))));
            }
        }

        public static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            if (!target.Exists)
                target.Create();
            foreach (DirectoryInfo dir in source.GetDirectories())
            {
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            }
            foreach (FileInfo file in source.GetFiles())
            {
                string newfile = Path.Combine(target.FullName, file.Name);
                try
                {
                    if (!File.Exists(newfile))
                        file.CopyTo(newfile, true);
                    else
                    {
                        if (File.GetLastWriteTimeUtc(newfile) < file.LastWriteTimeUtc)
                            file.CopyTo(newfile, true);
                    }
                }
                catch (Exception exception)
                {
                    Log.Debug("Unable to copy file:" + newfile, exception);
                }
            }
        }

        public void LoadPlugins(string pluginFolder)
        {
            if (!Directory.Exists(pluginFolder))
                return;
            string[] folders = Directory.GetDirectories(pluginFolder);
            foreach (string folder in folders)
            {
                string configFile = Path.Combine(folder, "dcc.plugin");
                if (File.Exists(configFile))
                {
                    try
                    {
                        PluginInfo pluginInfo = PluginInfo.Load(configFile);
                        if (File.Exists(Path.Combine(folder, "disabled")))
                            return;
                        string assemblyFile = Path.Combine(folder, pluginInfo.AssemblyFileName);
                        AvaiablePlugins.Add(pluginInfo);
                        Log.Debug("Loading plugin dll: " + assemblyFile);
                        if (!File.Exists(assemblyFile))
                        {
                            Log.Error("Assembly file not exist " + assemblyFile);
                            continue;
                        }
                        Assembly pluginAssembly = null;
                        pluginAssembly = Assembly.LoadFrom(assemblyFile);
                        if (pluginAssembly == null)
                        {
                            Log.Error("Error loading assembly");
                            continue;
                        }
                        Type[] exportedTypes = pluginAssembly.GetExportedTypes();
                        foreach (var exportedType in exportedTypes)
                        {
                            if (exportedType.IsAbstract)
                                continue;
                            object pluginObject = null;
                            try
                            {
                                pluginObject = Activator.CreateInstance(exportedType);
                            }
                            catch (Exception exception)
                            {
                                Log.Error("Error loading type " + exportedType.FullName, exception);
                            }
                            if (pluginObject != null)
                            {
                                var plugin = pluginObject as IPlugin;
                                try
                                {
                                    if (plugin != null)
                                    {
                                        plugin.Register();
                                        Plugins.Add(plugin);
                                    }
                                }
                                catch (Exception exception)
                                {
                                    Log.Error("Error registering plugiin.", exception);
                                }
                            }
                        }
                    }
                    catch (Exception exception)
                    {
                        Log.Error("Error loading plugin " + configFile, exception);
                    }
                }
            }
        }

        public void LoadPluginsOld(string pluginFolder)
        {
            if (!Directory.Exists(pluginFolder))
                return;
            string[] files = Directory.GetFiles(pluginFolder, "*.dll");
            foreach (var pluginFile in files)
            {
                Assembly pluginAssembly = null;
                try
                {
                    Log.Debug("LoadPlugins from:" + pluginFile);
                    pluginAssembly = Assembly.LoadFrom(pluginFile);
                }
                catch (BadImageFormatException)
                {
                    Log.Error(string.Format(" {0} has a bad image format", pluginFile));
                }
                catch (Exception exception)
                {
                    Log.Error("Error loading plugin :", exception);
                }
                if (pluginAssembly == null) continue;
                try
                {
                    Type[] exportedTypes = pluginAssembly.GetExportedTypes();
                    foreach (var exportedType in exportedTypes)
                    {
                        if (exportedType.IsAbstract)
                            continue;
                        object pluginObject = null;
                        try
                        {
                            pluginObject = Activator.CreateInstance(exportedType);
                        }
                        catch (Exception exception)
                        {
                            Log.Error("Error loading type " + exportedType.FullName, exception);
                        }
                        if (pluginObject != null)
                        {
                            var plugin = pluginObject as IPlugin;
                            try
                            {
                                if (plugin != null)
                                {
                                    plugin.Register();
                                    Plugins.Add(plugin);
                                }
                            }
                            catch (Exception exception)
                            {
                                Log.Error("Error registering plugiin.", exception);
                            }
                        }
                    }
                }
                catch (Exception exception)
                {
                    Log.Error("Error loading plugin  ", exception);
                }
            }
        }
    }
}