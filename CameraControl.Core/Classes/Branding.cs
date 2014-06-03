using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace CameraControl.Core.Classes
{
  public class Branding
  {
    public string ApplicationTitle { get; set; }

    public string StartupScreenImage { get; set; }

    public string LogoImage { get; set; }

    public string DefaultThumbImage { get; set; }

    public string DefaultMissingThumbImage { get; set; }

    public bool ShowAboutWindow { get; set; }

    public bool OnlineReference { get; set; }

    public Branding()
    {
      ShowAboutWindow = true;
      OnlineReference = true;
    }
  }
}
