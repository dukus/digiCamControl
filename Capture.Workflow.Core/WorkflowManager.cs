using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;
using CameraControl.Devices;
using CameraControl.Devices.Classes;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Database;
using Capture.Workflow.Core.Interface;
using GalaSoft.MvvmLight;
using Ionic.Zip;


namespace Capture.Workflow.Core
{
    public class WorkflowManager: ViewModelBase
    {
        #region private declarations

        private DispatcherTimer _liveViewTimer = new DispatcherTimer();
        private long _frames = 0;
        private DateTime _startTime ;

        #endregion

        #region events

        public delegate void MessageEventHandler(object sender, MessageEventArgs e);
        public event MessageEventHandler Message;

        #endregion

        private bool _thumbRunning = false;
        private bool _thumbNext = false;
        private object _lock = new object();


        private static WorkflowManager _instance;
        private BitmapSource _bitmap;
        private FileItem _selectedItem;

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
        public SqliteDatabase Database { get; set; }

        public static void ExecuteAsync(CommandCollection collection, Context context)
        {
            Task.Factory.StartNew(()=>Execute(collection, context));
        }

        public static void Execute(CommandCollection collection, Context context)
        {
            var currCmd = "";
            try
            {
                foreach (var command in collection.Items)
                {
                    currCmd = command.Name;
                    Log.Debug("Executing command " + currCmd);
                    if (!command.Instance.Execute(command, context))
                        return;
                }
            }
            catch (Exception ex)
            {
                Log.Debug("Command execution error "+currCmd, ex);
            }
        }

        public int PreviewSize { get; set; }

        public FileItem FileItem { get; set; }

        public AsyncObservableCollection<FileItem> FileItems { get; set; }

        public List<PluginInfo> Plugins { get; set; }

        public FileItem SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                if (_selectedItem != null)
                {
                    foreach (var variable in _selectedItem.Variables.Items)
                    {
                        var item = Context.WorkFlow.Variables[variable.Name];
                        if (item != null)
                        {
                            item.AttachedVariable = variable;
                        }
                    }
                }
            }
        }

        public BitmapSource Bitmap
        {
            get { return _bitmap; }
            set
            {
                _bitmap = value;
                RaisePropertyChanged(() => Bitmap);
            }
        }

        public WorkflowManager()
        {
            Plugins = new List<PluginInfo>();
            Context = new Context();

            // live view stuff for nikon
            _liveViewTimer.Interval = TimeSpan.FromMilliseconds(20);
            _liveViewTimer.Tick += _liveViewTimer_Tick;
            ServiceProvider.Instance.DeviceManager.CameraConnected += DeviceManager_CameraConnected;
            ServiceProvider.Instance.DeviceManager.PhotoCaptured += DeviceManager_PhotoCaptured;
            FileItems = new AsyncObservableCollection<FileItem>();
            FileItems.CollectionChanged += FileItems_CollectionChanged;
            ConfigureDatabase();
            QueueManager.Instance.Start();
        }

        private void FileItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Context?.WorkFlow?.Variables.SetValue("CapturedFilesCount", FileItems.Count.ToString());
        }

        private void DeviceManager_CameraConnected(ICameraDevice cameraDevice)
        {
            if (Context != null)
                Context.CameraDevice = cameraDevice;
        }

        public void ConfigureDatabase()
        {
            try
            {
                //Database = new Database.Database(Path.Combine(Settings.DataFolder, "database.db"));
                Database = new SqliteDatabase(Path.Combine(Settings.DataPath, "database.db"));
            }
            catch (DllNotFoundException ex)
            {
                Log.Error(
                    $"Error(ignored): Database at {Path.Combine(Settings.DataPath, "database.db")}: {ex.Message}",
                    null);
            }
            catch (SQLite.SQLiteException exception)
            {
                Log.Error("Unable to open database ", exception);
                try
                {
                    File.Delete(Path.Combine(Settings.DataPath, "database.db"));
                    Database = new SqliteDatabase(Path.Combine(Settings.DataPath, "database.db"));
                }
                catch (Exception e)
                {
                    Log.Error("Unable to create database ", e);
                }
            }
            catch (Exception exception)
            {
                Log.Error("Unable to create database ", exception);
            }
        }


        public BitmapSource GetLargeThumbnail(FileItem item)
        {
            if (item != null)
            {
                lock (_lock)
                {
                    try
                    {
                        var bitmap = Utils.LoadImage(WorkflowManager.Instance.SelectedItem.TempFile, PreviewSize, 0);
                        using (MemoryStream stream = new MemoryStream())
                        {
                            Utils.Save2Jpg(bitmap, stream);
                            Context.FileItem = item;
                            Context.ImageStream = stream;
                            OnMessage(new MessageEventArgs(Messages.ThumbChanged, FileItem) {Context = Context});
                            stream.Seek(0, SeekOrigin.Begin);
                            item.Thumb = Utils.LoadImage(stream, 200);
                            stream.Seek(0, SeekOrigin.Begin);
                            return Utils.LoadImage(stream);
                        }
                    }
                    catch (Exception e)
                    {
                        Log.Debug("Unable to create thumbnail", e);
                    }
                    Context.ImageStream = null;
                }
            }
            return null;
        }

        private void DeviceManager_PhotoCaptured(object sender, PhotoCapturedEventArgs eventArgs)
        {
            try
            {
                // if no workflow loaded do nothing 
                if (Context?.WorkFlow == null)
                    return;

                string tempFile = Path.Combine(Settings.Instance.TempFolder,
                    Path.GetRandomFileName() + Path.GetExtension(eventArgs.FileName));

                // set in varieable the captured file original name
                Context?.WorkFlow?.Variables.SetValue("CapturedFileName",
                    Path.GetFileNameWithoutExtension(eventArgs.FileName));

                Utils.CreateFolder(tempFile);

                if (File.Exists(tempFile))
                    File.Delete(tempFile);

                eventArgs.CameraDevice.TransferFile(eventArgs.Handle, tempFile);
                eventArgs.CameraDevice.ReleaseResurce(eventArgs);

                if (!Context.CaptureEnabled)
                {
                    // files should be transferred anyway if capture is enabled or not
                    // to prevent camera buffer fill up 
                    Utils.DeleteFile(tempFile);
                    Log.Debug("File transfer disabled");
                    return;
                }

                FileItem item = new FileItem()
                {
                    TempFile = tempFile,
                    Thumb = Utils.LoadImage(tempFile, 200),
                    Variables = Context.WorkFlow.Variables.GetItemVariables()
                };
                Bitmap = Utils.LoadImage(tempFile, 1090);
                FileItems.Add(item);
                FileItem = item;
                Context.FileItem = FileItem;

                Utils.WaitForFile(tempFile);
                Context.FileItem = item;
                OnMessage(new MessageEventArgs(Messages.PhotoDownloaded, FileItem) {Context = Context});
                OnMessage(new MessageEventArgs(Messages.FileTransferred, Context) {Context = Context});
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
                if (liveViewData?.ImageData != null)
                {
                    using (MemoryStream stream = new MemoryStream())
                    {
                        // can be used in MemoryStream constructor because throw error with image magick 
                        stream.Write(liveViewData.ImageData, liveViewData.ImageDataPosition, liveViewData.ImageData.Length - liveViewData.ImageDataPosition);
                        Context.ImageStream = stream;
                        OnMessage(new MessageEventArgs(Messages.LiveViewChanged, new object[] { stream, liveViewData }) { Context = Context });
                        _frames++;
                        var fps = ((double)_frames / (DateTime.Now - _startTime).Seconds);
                        Context?.WorkFlow?.Variables.SetValue("Fps", fps.ToString("###.00"));
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

            return res.OrderBy(x => x.Name).ToList();
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
                    var attributeIcon = exportedType.GetCustomAttribute<IconAttribute>();

                    if (attribute != null)
                    {
                        var plugin=new PluginInfo()
                        {
                            Type = attribute.PluginType,
                            Class = exportedType.AssemblyQualifiedName,
                            Description = attributeDes?.Description,
                            Name = attributeName?.DisplayName,
                            Icon = attributeIcon?.Icon
                        };
                        Log.Debug("Loading plugin " + plugin.Type.ToString().PadRight(15) + "=>" +
                                  plugin.Name.PadRight(25) + "=>" + plugin.Class);

                        Plugins.Add(plugin);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Unable to load plugins", ex);
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

        public void SaveAsPsackage(WorkFlow workflow, string file)
        {
            var files = new Dictionary<string, string>();

            using (ZipFile zip = new ZipFile())
            {
                XmlSerializer serializer = new XmlSerializer(typeof(WorkFlow));
                // Create a FileStream to write with.
                using (MemoryStream writer=new MemoryStream())
                {
                    // Serialize the object, and close the TextWriter
                    serializer.Serialize(writer, workflow);

                    foreach (var view in workflow.Views)
                    {
                        foreach (var viewProperty in view.Properties.Items.Where(
                            x => x.PropertyType == CustomPropertyType.File))
                        {
                            if (File.Exists(viewProperty.Value) && !files.ContainsKey(viewProperty.Value))
                                files.Add(viewProperty.Value, "");

                        }
                        foreach (var element in view.Elements)
                        foreach (var property in element.Properties.Items.Where(
                            x => x.PropertyType == CustomPropertyType.File))
                        {
                            if (File.Exists(property.Value) && !files.ContainsKey(property.Value))
                                files.Add(property.Value, "");
                            }
                    }
                    foreach (var viewProperty in workflow.Properties.Items.Where(
                        x => x.PropertyType == CustomPropertyType.File))
                    {
                        if (File.Exists(viewProperty.Value) && !files.ContainsKey(viewProperty.Value))
                            files.Add(viewProperty.Value, "");

                    }
                    zip.AddEntry("package.xml", writer.ToArray());
                    foreach (var file1 in files)
                    {
                        zip.AddFile(file1.Key, "files");
                    }
                }
                zip.Save(file);
            }
        }


        public WorkFlow LoadFromPackage(string file)
        {
            using (ZipFile zip = new ZipFile(file))
            {
                using (MemoryStream reader = new MemoryStream())
                {
                    zip["package.xml"].Extract(reader);
                    reader.Seek(0, SeekOrigin.Begin);
                    var package = Load(reader);
                    package.Package = file;
                    return package;
                }
            }
        }

        public WorkFlow Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                using (FileStream myFileStream = new FileStream(fileName, FileMode.Open,FileAccess.Read,FileShare.Read))
                {
                    return Load(myFileStream);
                }

            }
            return null;
        }

        public WorkFlow Load(Stream myFileStream)
        {
            XmlSerializer mySerializer =
                new XmlSerializer(typeof(WorkFlow));
            WorkFlow flow = (WorkFlow) mySerializer.Deserialize(myFileStream);

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
                LoadPluginInfo(event_.PluginInfo);

                foreach (var flowCommand in flowEvent.CommandCollection.Items)
                {
                    IWorkflowCommand commandPlugin = Instance.GetCommandPlugin(flowCommand.PluginInfo.Class);
                    var wCommand = commandPlugin.CreateCommand();
                    wCommand.Instance = commandPlugin;
                    wCommand.PluginInfo = flowCommand.PluginInfo;
                    wCommand.Name = flowCommand.Name;
                    wCommand.Properties.CopyValuesFrom(flowCommand.Properties);
                    event_.CommandCollection.Items.Add(wCommand);
                    LoadPluginInfo(flowCommand.PluginInfo);
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
                LoadPluginInfo(view.PluginInfo);
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
                    LoadPluginInfo(element.PluginInfo);
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
                                IWorkflowCommand commandPlugin =
                                    Instance.GetCommandPlugin(flowCommand.PluginInfo.Class);
                                var wCommand = commandPlugin.CreateCommand();
                                wCommand.Instance = commandPlugin;
                                wCommand.PluginInfo = flowCommand.PluginInfo;
                                wCommand.Name = flowCommand.Name;
                                wCommand.Properties.CopyValuesFrom(flowCommand.Properties);
                                commandCollection.Items.Add(wCommand);
                                LoadPluginInfo(wCommand.PluginInfo);
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
                            LoadPluginInfo(wCommand.PluginInfo);
                        }
                    }
                }
                resflow.Views.Add(view);
            }
            LoadVariables(resflow);
            return resflow;
        }

        public void LoadPluginInfo(PluginInfo info)
        {
            var item= Plugins.FirstOrDefault(x => x.Name == info.Name);
            if (item != null)
            {
                info.Icon = item.Icon;
                info.Description = item.Description;
            }
        }

        public IViewPlugin GetViewPlugin(string className)
        {
            try
            {
                return (IViewPlugin)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
            }
            catch (Exception e)
            {
                Log.Error("Unable to load view plugin" + className, e);
                throw;
            }

        }

        public IViewElementPlugin GetElementPlugin(string className)
        {
            try
            {
                return (IViewElementPlugin)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
            }
            catch (Exception e)
            {
                Log.Error("Unable to load element plugin" + className, e);
                throw;
            }

        }

        public IEventPlugin GetEventPlugin(string className)
        {
            try
            {
                Log.Debug("Load event plugin " + className);
                var plugin = Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
                return (IEventPlugin)plugin;
            }
            catch (Exception e)
            {
                Log.Error("Unable to load event plugin" + className,e);
                throw;
            }
        }

        public IWorkflowCommand GetCommandPlugin(string className)
        {
            try
            {
                return (IWorkflowCommand)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
            }
            catch (Exception e)
            {
                Log.Error("Unable to load command plugin" + className, e);
                throw;
            }

        }

        public IWorkflowQueueCommand GetQueueCommandPlugin(string pluginName)
        {
            var className = Plugins.Where(x => x.Name == pluginName).Select(x => x.Class).FirstOrDefault();
            return (IWorkflowQueueCommand)Activator.CreateInstance(Type.GetType(className, AssemblyResolver, null));
        }

        public WorkFlow CreateWorkFlow()
        {
            WorkFlow resflow = new WorkFlow();
            resflow.Variables.Items.Add(new Variable() {Name = "SessionFolder", Editable = true});
            //resflow.Variables.Items.Add(new Variable() {Name = "FileNameTemplate", Editable = false});
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
                    _frames = 0;
                    _startTime = DateTime.Now;
                    _liveViewTimer.Start();
                    break;
                case Messages.StopLiveView:
                    _liveViewTimer.Stop();
                    break;
                case Messages.ShowMessage:
                    MessageBox.Show(e.Param.ToString());
                    break;
                case Messages.SaveVariables:
                case Messages.SessionFinished:
                    SaveVariables(e.Context.WorkFlow);
                    break;
                case Messages.ThumbCreate:
                    UpdateThumbAsync();
                    break;
                case Messages.NextPhoto:
                    if (SelectedItem != null)
                    {
                        var i = FileItems.IndexOf(SelectedItem);
                        i++;
                        if (i < FileItems.Count)
                        {
                            SelectedItem = FileItems[i];
                        }
                    }
                    UpdateThumbAsync();
                    break;
                case Messages.PrevPhoto:
                    if (SelectedItem != null)
                    {
                        var i = FileItems.IndexOf(SelectedItem);
                        i--;
                        if (i > -1)
                        {
                            SelectedItem = FileItems[i];
                        }
                    }
                    UpdateThumbAsync();
                    break;
                case Messages.DeletePhoto:
                {
                    if (SelectedItem == null || FileItems.Count == 0)
                        return;
                    var i = FileItems.IndexOf(SelectedItem);
                    SelectedItem.Clear();
                    FileItems.Remove(SelectedItem);
                    
                    if (i >= FileItems.Count)
                        i--;
                    if (i >= 0 && FileItems.Count > 0)
                        SelectedItem = FileItems[i];
                    UpdateThumbAsync();
                }
                    break;
                case Messages.ClearPhotos:
                {
                    foreach (var item in FileItems)
                    {
                        item.Clear();
                    }
                    FileItems.Clear();
                    SelectedItem = null;
                    UpdateThumbAsync();
                    break;
                }
            }
            Message?.Invoke(this, e);
        }

        private void UpdateThumbAsync()
        {
            if (_thumbRunning)
            {
                _thumbNext = true;
                return;
            }
            _thumbNext = false;
            Task.Factory.StartNew(UpdateThumb);
        }

        private void UpdateThumb()
        {
            lock (_lock)
            {
                _thumbRunning = true;
                Bitmap = GetLargeThumbnail(SelectedItem);
                OnMessage(new MessageEventArgs(Messages.ThumbUpdated, null));
                _thumbRunning = false;
                if (_thumbNext)
                {
                    _thumbNext = false;
                    UpdateThumbAsync();
                }
            }
        }

        public void SaveVariables(WorkFlow workflow)
        {
            string file = Path.Combine(Settings.Instance.CacheFolder, workflow.Id + ".xml");
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(VariableCollection));
                Utils.CreateFolder(file);
                // Create a FileStream to write with.
                Stream writer = new FileStream(file, FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, workflow.Variables);
                writer.Close();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save variable values ", exception);
            }
        }

        /// <summary>
        /// Reinit all variable values with default one and
        /// load the saved variables value from cache.
        /// </summary>
        /// <param name="workflow">The workflow.</param>
        public void LoadVariables(WorkFlow workflow)
        {
            try
            {
                // set for all variables the default value,
                // which will be overwrited or not later 
                foreach (var variable in workflow.Variables.Items)
                {
                    variable.Value = variable.DefaultValue;
                }

                string file = Path.Combine(Settings.Instance.CacheFolder, workflow.Id + ".xml");
                if (!File.Exists(file))
                    return;

                XmlSerializer mySerializer =new XmlSerializer(typeof(VariableCollection));
                FileStream myFileStream = new FileStream(file, FileMode.Open);
                var values = (VariableCollection) mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                foreach (var val in values.Items)
                {
                    var selval = workflow.Variables[val.Name];
                    if (selval != null && !selval.Reinit)
                        selval.Value = val.Value;
                }
                foreach (var variable in workflow.Variables.Items)
                {
                    if (variable.Reinit)
                        variable.Value = variable.DefaultValue;
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error loading variable values", exception);
            }
        }


    }
}
