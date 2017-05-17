using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Threading;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;


namespace Capture.Workflow.Core
{
    public class WorkflowManager
    {
        #region private declarations

        private DispatcherTimer _liveViewTimer = new DispatcherTimer();

        #endregion

        #region events

        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public event MessageEventHandler Message;

        #endregion

        private static WorkflowManager _instance;

        public static WorkflowManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new WorkflowManager();
                return _instance;
            }
            set { _instance = value; }
        }

        public Context Context { get; set; }

        public static void Execute(CommandCollection collection)
        {
            try
            {
                foreach (var command in collection.Items)
                {
                    if (!command.Instance.Execute(command))
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Command execution error", ex);                
            }
        }

        public FileItem FileItem { get; set; }

        public AsyncObservableCollection<FileItem> FileItems { get; set; }

        public List<PluginInfo> Plugins { get; set; }

        public WorkflowManager()
        {
            Plugins = new List<PluginInfo>();
            Context = new Context();

            // live view stuff for nikon
            _liveViewTimer.Interval = TimeSpan.FromMilliseconds(40);
            _liveViewTimer.Tick += _liveViewTimer_Tick;
            ServiceProvider.Instance.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            FileItems = new AsyncObservableCollection<FileItem>();
        }

        private void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            try
            {
                string tempFile = Path.Combine(Settings.Instance.TempFolder, Path.GetRandomFileName() + Path.GetExtension(eventArgs.FileName));

                Utils.CreateFolder(tempFile);

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                eventArgs.CameraDevice.TransferFile(eventArgs.Handle, tempFile);
                FileItem item = new FileItem() { FileName = tempFile, Thumb = Utils.LoadImage(tempFile, 200, 0) };
                FileItems.Add(item);
                FileItem = item;
                item.ThumbFile = Path.GetTempFileName();
                Utils.Save2Jpg(Utils.LoadImage(tempFile, 800, 0), item.ThumbFile);
                OnMessage(new MessageEventArgs(Messages.PhotoDownloaded, FileItem));
            }
            catch (Exception ex)
            {
                Log.Error("Error transfer file", ex);
            }
        }

        private void _liveViewTimer_Tick(object sender, EventArgs e)
        {
            _liveViewTimer.Stop();
            Task.Factory.StartNew(GetLiveView);
        }

        private void GetLiveView()
        {
            try
            {
                var liveViewData = Context.CameraDevice.GetLiveViewImage();
                if (liveViewData != null && liveViewData.ImageData != null)
                {
                    using (
                        MemoryStream stream = new MemoryStream(liveViewData.ImageData, liveViewData.ImageDataPosition,
                            liveViewData.ImageData.Length - liveViewData.ImageDataPosition))
                    {
                       
                        OnMessage(new MessageEventArgs(Messages.LiveViewChanged, new object[] {stream, liveViewData}));
                    }
                }
            }
            catch (Exception)
            {

            }
            _liveViewTimer.Start();
        }

        public List<PluginInfo> GetPlugins(PluginType type)
        {
            List<PluginInfo> res = new List<PluginInfo>();
            foreach (PluginInfo plugin in Plugins)
            {
                if (plugin.Type==type)
                    res.Add(plugin);
            }
            return res;
        }


        public void LoadPlugins(string assemblyFile)
        {
            try
            {
                var pluginAssembly = Assembly.LoadFrom(assemblyFile);
                if (pluginAssembly == null)
                {
                    Log.Error("Error loading assembly");
                    return;
                }
                Type[] exportedTypes = pluginAssembly.GetExportedTypes();
                foreach (var exportedType in exportedTypes)
                {
                    var attribute = exportedType.GetCustomAttribute<PluginTypeAttribute>();
                    var attributeDes = exportedType.GetCustomAttribute<DescriptionAttribute>();
                    var attributeName = exportedType.GetCustomAttribute<DisplayNameAttribute>();
                    
                    if (attribute != null)
                    {
                        Plugins.Add(new PluginInfo()
                        {
                            Type = attribute.PluginType,
                            Class = exportedType.AssemblyQualifiedName,
                            Description = attributeDes?.Description,
                            Name = attributeName?.DisplayName
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to load plugins");
            }
        }

        public void Save(WorkFlow workflow, string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorkFlow));
            // Create a FileStream to write with.

            Stream writer = new FileStream(file, FileMode.Create);
            // Serialize the object, and close the TextWriter
            serializer.Serialize(writer, workflow);
            writer.Close();
        }

        public WorkFlow Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer mySerializer =
                    new XmlSerializer(typeof(WorkFlow));
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                WorkFlow flow = (WorkFlow)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                WorkFlow resflow = Instance.CreateWorkFlow();
                resflow.Id = flow.Id;
                resflow.Name = flow.Name;
                resflow.Description = flow.Description;
                resflow.Version = flow.Version;
                resflow.Properties.CopyValuesFrom(flow.Properties);

                foreach (Variable variable in flow.Variables.Items)
                {
                    if (resflow.Variables[variable.Name] != null)
                    {
                        resflow.Variables[variable.Name].Value = variable.Value;
                    }
                    else
                    {
                        resflow.Variables.Items.Add(variable);
                    }
                }

                foreach (var flowEvent in flow.Events)
                {
                    IEventPlugin plugin = Instance.GetEventPlugin(flowEvent.PluginInfo.Class);
                    WorkFlowEvent event_ = plugin.CreateEvent();
                    event_.Parent = resflow;
                    event_.Instance = plugin;
                    event_.PluginInfo = flowEvent.PluginInfo;
                    event_.Name = flowEvent.Name;
                    event_.Properties.CopyValuesFrom(flowEvent.Properties);
                    foreach (var flowCommand in flowEvent.CommandCollection.Items)
                    {
                        IWorkflowCommand commandPlugin = Instance.GetCommandPlugin(flowCommand.PluginInfo.Class);
                        var wCommand = commandPlugin.CreateCommand();
                        wCommand.Instance = commandPlugin;
                        wCommand.PluginInfo = flowCommand.PluginInfo;
                        wCommand.Name = flowCommand.Name;
                        wCommand.Properties.CopyValuesFrom(flowCommand.Properties);
                        event_.CommandCollection.Items.Add(wCommand);
                    }
                    resflow.Events.Add(event_);
                }

                foreach (var _view in flow.Views)
                {
                    IViewPlugin plugin = Instance.GetViewPlugin(_view.PluginInfo.Class);
                    WorkFlowView view = plugin.CreateView();
                    view.Parent = resflow;
                    view.Instance = plugin;
                    view.PluginInfo = _view.PluginInfo;
                    view.Name = _view.Name;
                    view.Properties.CopyValuesFrom(_view.Properties);
                    foreach (var viewElement in _view.Elements)
                    {
                        IViewElementPlugin elementplugin = Instance.GetElementPlugin(viewElement.PluginInfo.Class);
                        WorkFlowViewElement element = elementplugin.CreateElement(view);
                        element.Parent = view;
                        element.Instance = elementplugin;
                        element.PluginInfo = viewElement.PluginInfo;
                        element.Name = viewElement.Name;
                        element.Properties.CopyValuesFrom(viewElement.Properties);
                        view.Elements.Add(element);
                        foreach (var commandCollection in element.Events)
                        {
                            CommandCollection loadedcommand = null;
                            foreach (var collection in viewElement.Events)
                            {
                                if (collection.Name == commandCollection.Name)
                                    loadedcommand = collection;
                            }
                            if (loadedcommand != null)
                            {
                                foreach (var flowCommand in loadedcommand.Items)
                                {
                                    IWorkflowCommand commandPlugin= Instance.GetCommandPlugin(flowCommand.PluginInfo.Class);
                                    var wCommand = commandPlugin.CreateCommand();
                                    wCommand.Instance = commandPlugin;
                                    wCommand.PluginInfo = flowCommand.PluginInfo;
                                    wCommand.Name = flowCommand.Name;
                                    wCommand.Properties.CopyValuesFrom(flowCommand.Properties);
                                    commandCollection.Items.Add(wCommand);
                                }
                            }
                        }
                    }
                    foreach (var commandCollection in view.Events)
                    {
                        CommandCollection loadedcommand = null;
                        foreach (var collection in _view.Events)
                        {
                            if (collection.Name == commandCollection.Name)
                                loadedcommand = collection;
                        }
                        if (loadedcommand != null)
                        {
                            foreach (var flowCommand in loadedcommand.Items)
                            {
                                IWorkflowCommand commandPlugin = Instance.GetCommandPlugin(flowCommand.PluginInfo.Class);
                                var wCommand = commandPlugin.CreateCommand();
                                wCommand.Instance = commandPlugin;
                                wCommand.PluginInfo = flowCommand.PluginInfo;
                                wCommand.Name = flowCommand.Name;
                                wCommand.Properties.CopyValuesFrom(flowCommand.Properties);
                                commandCollection.Items.Add(wCommand);
                            }
                        }
                    }
                    resflow.Views.Add(view);
                }
                return resflow;
            }
            return null;
        }

        public IViewPlugin GetViewPlugin(string className)
        {
            return (IViewPlugin) Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
        }

        public IViewElementPlugin GetElementPlugin(string className)
        {
            return (IViewElementPlugin)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
        }

        public IEventPlugin GetEventPlugin(string className)
        {
            return (IEventPlugin)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
        }

        public IWorkflowCommand GetCommandPlugin(string className)
        {
            return (IWorkflowCommand)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
        }

        public WorkFlow CreateWorkFlow()
        {
            WorkFlow resflow = new WorkFlow();
            resflow.Variables.Items.Add(new Variable() {Name = "SessionFolder", Editable = false});
            resflow.Variables.Items.Add(new Variable() {Name = "FileNameTemplate", Editable = false});
            return resflow;
        }

        private static Assembly AssemblyResolver(AssemblyName assemblyName)
        {
            assemblyName.Version = null;
            return Assembly.Load(assemblyName);
        }


        public virtual void OnMessage(MessageEventArgs e)
        {
            switch (e.Name)
            {
                case Messages.StartLiveView:
                    _liveViewTimer.Start();
                    break;
                case Messages.StopLiveView:
                    _liveViewTimer.Stop();
                    break;
            }
            Message?.Invoke(this, e);
        }

    }
}
