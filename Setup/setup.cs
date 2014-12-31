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

            Project project = new Project("digiCamControl",
                new Dir(@"%ProgramFiles%\digiCamControl_new",
                    new DirFiles(appFeature, @"*.exe"),
                    new DirFiles(appFeature, @"*.dll")
                    ));

            project.UI = WUI.WixUI_FeatureTree;
            project.GUID = new Guid("19d12628-7654-4354-a305-9ab0932af676");

            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Debug\"));

            project.ResolveWildCards();
            
            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(Path.Combine(project.SourceBaseDir, "CameraControl.exe"));
            project.Version = new Version(ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            project.OutFileName = string.Format("digiCamControlsetup_{0}", ver.FileVersion);
            Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);

        }
    }
}
