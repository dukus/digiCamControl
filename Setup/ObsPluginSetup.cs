using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Win32;
using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File;

namespace Setup
{
    public class ObsPluginSetup
    {
        public static void Execute()
        {
            Feature obsPlugin = new Feature("Obs Plugin");
            var obsDir = new Dir(@"%ProgramFiles%\OBS\plugins",
                new File(obsPlugin, @"ObsPlugin\CLRHostPlugin.dll"),
                new Dir(obsPlugin, "CLRHostPlugin",
                    new DirFiles(obsPlugin, @"ObsPlugin\CLRHostPlugin\*.*")
                    ));

            Project project = new Project("OBS plugin for digiCamControl",obsDir);

            project.SetNetFxPrerequisite("NETFRAMEWORK45 >= '#378389'", "Please install .Net 4.5 First");
            project.UI = WUI.WixUI_Minimal;
            project.GUID = new Guid("357E0D80-5093-478E-8C11-28B1A72096E7");

#if DEBUG
            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Debug\"));
#else
            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Release\"));
#endif

            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(Path.Combine(project.SourceBaseDir, "ObsPlugin\\CLRHostPlugin", "DccObsPlugin.dll"));

            project.LicenceFile = @"Licenses\DigiCamControlLicence.rtf";

            project.Version = new Version(ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            //project.MajorUpgradeStrategy.NewerProductInstalledErrorMessage = "A version of the digiCamControl already installed. Unistall it first from Control Panel !";
            project.MajorUpgradeStrategy.RemoveExistingProductAfter = Step.InstallInitialize;
            ////project.MajorUpgradeStrategy.UpgradeVersions = VersionRange.ThisAndOlder;
            ////project.MajorUpgradeStrategy.PreventDowngradingVersions = VersionRange.ThisAndOlder;

            project.ControlPanelInfo.Manufacturer = "Duka Istvan";
            project.OutFileName = string.Format("ObsPluginSetup_{0}", ver.FileVersion);
            project.ControlPanelInfo.ProductIcon = "logo.ico";


            project.ResolveWildCards();
            Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);
        }
    }
}
