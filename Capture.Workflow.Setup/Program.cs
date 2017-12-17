using System;
using System.Diagnostics;
using System.IO;
using WixSharp;
using File = WixSharp.File;

namespace Capture.Workflow.Setup
{
    class Program
    {
        static void Main()
        {

            var project = new Project("Capture.Workflow",
                new Dir(@"%ProgramFiles%\Capture.Workflow",
                    new DirFiles(@"*.dll"),
                    new DirFiles(@"*.config"),
                    new DirFiles(@"*.pdb"),
                    new Dir("Workflows",
                        new DirFiles(@"Workflows\*.*")),
                    new File("Capture.Workflow.exe",
                        new FileShortcut("Capture Workflow", @"%ProgramMenu%\Capture.Workflow") {WorkingDirectory = @"INSTALLDIR"},
                        new FileShortcut("Capture Workflow", @"%Desktop%") {WorkingDirectory = @"INSTALLDIR"})
                    ));

            project.GUID = new Guid("B83E588B-BD7B-40C9-A78E-4E18E6916EA7");
            //project.SourceBaseDir = "<input dir path>";
            //project.OutDir = "<output dir path>";

#if DEBUG
            project.SourceBaseDir =
                Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\Capture.Workflow\bin\Debug\"));
#else
            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\Capture.Workflow\bin\Release\"));
#endif
            FileVersionInfo ver = null;
            ver =
                    FileVersionInfo.GetVersionInfo(Path.Combine(project.SourceBaseDir, "Capture.Workflow.exe"));

            project.LicenceFile = @"Licenses\DigiCamControlLicence.rtf";
            project.Version = new Version(ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            //project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            ////project.MajorUpgradeStrategy.NewerProductInstalledErrorMessage = "A version of the digiCamControl already installed. Unistall it first from Control Panel !";
            //project.MajorUpgradeStrategy.RemoveExistingProductAfter = Step.InstallInitialize;
            project.ControlPanelInfo.Manufacturer = "Duka Istvan";
            project.OutFileName = string.Format("Capture.Workflow.Setup_{0}", ver.FileVersion);

            project.InstallScope = InstallScope.perMachine;
            project.ResolveWildCards();
            Compiler.PreserveTempFiles = false;

            project.BuildMsi();
        }
    }
}