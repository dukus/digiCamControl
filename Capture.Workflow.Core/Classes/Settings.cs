using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using CameraControl.Devices;

namespace Capture.Workflow.Core.Classes
{
    public class Settings
    {
        public static string ConfigFile = "workflow.settings.xml";
        private static Settings _instance;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                    _instance = Load();
                    return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public string ClientId { get; set; }
        public bool SendStatistics { get; set; }


        public static string BasePath => Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        public static string DataPath => Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Workflow.Capture");

        public string TempFolder => Path.Combine(DataPath, "Temp");
        public string WorkflowFolder => Path.Combine(DataPath, "Workflows");
        public string DefaultWorkflowFolder => Path.Combine(BasePath, "Workflows");
        public string CacheFolder => Path.Combine(DataPath, "Cache");
        public string LogFolder => Path.Combine(DataPath, "Log");
        public string QueueFolder => Path.Combine(DataPath, "Queue");


        public Settings()
        {
            ClientId = Guid.NewGuid().ToString();
            SendStatistics = true;
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                Utils.CreateFolder(Path.Combine(DataPath, ConfigFile));
                // Create a FileStream to write with.
                Stream writer = new FileStream(Path.Combine(DataPath,ConfigFile), FileMode.Create);
                // Serialize the object, and close the TextWriter
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch (Exception exception)
            {
                Log.Error("Unable to save settings ", exception);
            }
        }

        public static Settings Load()
        {
            Settings settings = new Settings();
            try
            {
                if (File.Exists(Path.Combine(DataPath, ConfigFile)))
                {
                    settings = LoadSettings(Path.Combine(DataPath, ConfigFile), settings);
                }
                else
                {
                    settings.Save();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error loading config file ", exception);
            }
            return settings;
        }

        public static Settings LoadSettings(string fileName, Settings defaultSettings)
        {
            try
            {
                if (File.Exists(Path.Combine(DataPath, ConfigFile)))
                {
                    XmlSerializer mySerializer =
                        new XmlSerializer(typeof(Settings));
                    FileStream myFileStream = new FileStream(Path.Combine(DataPath, ConfigFile), FileMode.Open);
                    defaultSettings = (Settings)mySerializer.Deserialize(myFileStream);
                    myFileStream.Close();
                }
            }
            catch (Exception exception)
            {
                Log.Error("Error loading config file ", exception);
            }
            return defaultSettings;
        }
    }
}
