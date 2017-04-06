using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using CameraControl.Devices;
using Capture.Workflow.Core.Classes;
using Capture.Workflow.Core.Classes.Attributes;
using Capture.Workflow.Core.Interface;

namespace Capture.Workflow.Core
{
    public class WorkflowManager
    {
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

        public WorkFlow CurrentWorkflow { get; set; }

        public List<PluginInfo> Plugins { get; set; }

        public WorkflowManager()
        {
            Plugins = new List<PluginInfo>();
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


    }
}
