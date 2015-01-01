using System;
using System.IO;
using WixSharp;
using System;
using System.IO;
using System.Diagnostics;
using File = WixSharp.File;

namespace Setup
{
    internal class Script
    {
        public static void Main(string[] args)
        {

            Feature appFeature = new Feature("Application files");
            Feature obsPlugin = new Feature("Obs Plugin");

            var appDir = new Dir(@"digiCamControl_new",
                new DirFiles(appFeature, @"*.exe"),
                new DirFiles(appFeature, @"*.dll"),
                new File(appFeature, "regwia.bat"),
                new File(appFeature, "logo_big.jpg"),
                new File(appFeature, "baseMtpDevice.xml"),
                new Dir(appFeature, "Data",
                    new DirFiles(appFeature, @"Data\*.*")),
                new Dir(appFeature, "Plugins",
                    new Files(appFeature, @"Plugins\*.*", "MahApps.Metro.*", "System.Windows.Interactivity.*", "WriteableBitmapEx.Wpf.*", "GalaSoft.MvvmLight.*", "*.config")),
                new Dir(appFeature, "Languages",
                    new DirFiles(appFeature, @"Languages\*.xml")),
                new Dir(appFeature, "Licenses",
                    new DirFiles(appFeature, @"Licenses\*.*")),
                new Dir(appFeature, "Tools",
                    new DirFiles(appFeature, @"Tools\*.*")),
                new Dir(appFeature, "WebServer",
                    new Files(appFeature, @"WebServer\*.*"))
                );

            var obsDir = new Dir(@"OBS\plugins",
                new File(obsPlugin, @"ObsPlugin\CLRHostPlugin.dll"),
                new Dir(obsPlugin, "CLRHostPlugin",
                    new DirFiles(obsPlugin, @"ObsPlugin\CLRHostPlugin\*.*")
                    ));

            var baseDir = new Dir(@"%ProgramFiles%",
                appDir,
                obsDir
                );


            Project project = new Project("digiCamControl", baseDir);

            project.UI = WUI.WixUI_FeatureTree;
            project.GUID = new Guid("19d12628-7654-4354-a305-9ab0932af676");

            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Debug\"));

            project.ResolveWildCards();

            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(Path.Combine(project.SourceBaseDir, "CameraControl.exe"));
            
            project.LicenceFile = @"Licenses\DigiCamControlLicence.txt";
            
            project.Version = new Version(ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            project.OutFileName = string.Format("digiCamControlsetup_{0}", ver.FileVersion);
            Compiler.PreserveTempFiles = true;
            Compiler.AllowNonRtfLicense = true;
            Compiler.BuildMsi(project);

        }
    }
}
