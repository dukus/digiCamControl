using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Xml;
using CameraControl.Devices;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class BulbCapture : BaseScript
    {
        #region Implementation of IScriptCommand

        public override bool Execute(ScriptObject scriptObject)
        {
            ServiceProvider.ScriptManager.OutPut("Bulb capture started");
            Executing = true;
            if (scriptObject.CameraDevice != null)
            {
                if (scriptObject.CameraDevice.IsoNumber != null)
                    scriptObject.CameraDevice.IsoNumber.SetValue(Iso);
                Thread.Sleep(200);
                if (scriptObject.CameraDevice.FNumber != null)
                    scriptObject.CameraDevice.FNumber.SetValue(Aperture);
                Thread.Sleep(200);
            }
            scriptObject.StartCapture();
            Thread.Sleep(200);
            for (int i = 0; i < CaptureTime; i++)
            {
                if (ServiceProvider.ScriptManager.ShouldStop)
                    break;
                Thread.Sleep(1000);
                ServiceProvider.ScriptManager.OutPut(string.Format("Bulb capture in progress .... {0}/{1}", i + 1, CaptureTime));
            }
            scriptObject.StopCapture();
            Thread.Sleep(200);
            Executing = false;
            IsExecuted = true;
            ServiceProvider.ScriptManager.OutPut("Bulb capture done");
            return true;
        }

        public override IScriptCommand Create()
        {
            return new BulbCapture();
        }

        public override XmlNode Save(XmlDocument doc)
        {
            XmlNode nameNode = doc.CreateElement("bulbcapture");
            nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, "capturetime", CaptureTime.ToString()));
            nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, "iso", Iso));
            nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, "aperture", Aperture));
            return nameNode;
        }

        public override IScriptCommand Load(XmlNode node)
        {
            BulbCapture res = new BulbCapture
                                  {
                                      CaptureTime = Convert.ToInt32(ScriptManager.GetValue(node, "capturetime")),
                                      Iso = ScriptManager.GetValue(node, "iso"),
                                      Aperture = ScriptManager.GetValue(node, "aperture")
                                  };
            return res;
        }

        public override string DisplayName
        {
            get { return string.Format("[{0}][CaptureTime={1}, Iso={2}, Aperture={3}]", Name, CaptureTime, Iso,Aperture); }
            set { }
        }

        public override UserControl GetConfig()
        {
            return new BulbCaptureControl(this);
        }

        #endregion

        private int _captureTime;
        public int CaptureTime
        {
            get { return _captureTime; }
            set
            {
                _captureTime = value;
                NotifyPropertyChanged("CaptureTime");
                NotifyPropertyChanged("DisplayName");
            }
        }

        private string _iso;
        public string Iso
        {
            get { return _iso; }
            set
            {
                _iso = value;
                NotifyPropertyChanged("Iso");
                NotifyPropertyChanged("DisplayName");
            }
        }

        private string _aperture;
        public string Aperture
        {
            get { return _aperture; }
            set
            {
                _aperture = value;
                NotifyPropertyChanged("Aperture");
                NotifyPropertyChanged("DisplayName");
            }
        }

        public BulbCapture()
        {
            Name = "bulbcapture";
            Description = "Start bulb capture camera should be set in manual mode with bulb.\nParameters: capturetime\niso\naperture";
            DefaultValue = "bulbcapture capturetime=\"10\" iso=\"100\"";
            IsExecuted = false;
            Executing = false;
            CaptureTime = 30;
            HaveEditControl = true;
        }

    }
}
