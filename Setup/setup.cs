using System;
using System.Diagnostics;
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Xml;
using Microsoft.Deployment.WindowsInstaller;
using WixSharp;
using WixSharp.CommonTasks;
using File = WixSharp.File;

namespace Setup
{
    internal class Script
    {
        public static void Main(string[] args)
        {

            Feature appFeature = new Feature("Application files", "Main application files", true, false, @"INSTALLDIR");
            var shortcut = new FileShortcut(appFeature, "digiCamControl", @"%ProgramMenu%\digiCamControl") { WorkingDirectory = @"INSTALLDIR" };
            var shortcutD = new FileShortcut(appFeature, "digiCamControl", @"%Desktop%") { WorkingDirectory = @"INSTALLDIR" };
            var appDir = new Dir(@"digiCamControl",
                new File(appFeature, "CameraControl.exe", shortcut, shortcutD),
                new File(appFeature, "CameraControl.PluginManager.exe"),
                new File(appFeature, "CameraControlCmd.exe"),
                new File(appFeature, "CameraControlRemoteCmd.exe"),
                new File(appFeature, "dcraw.exe"),
                new File(appFeature, "ffmpeg.exe"),
                new File(appFeature, "ngrok.exe"),
                new File(appFeature, "MtpTester.exe"),
                //new File(appFeature, "PhotoBooth.exe",new FileShortcut(appFeature, "PhotoBooth", @"%ProgramMenu%\digiCamControl")),
                new DirFiles(appFeature, @"*.dll"),
#if DEBUG
                new DirFiles(appFeature, @"*.pdb"),
#endif
                new File(appFeature, "regwia.bat"),
                new File(appFeature, "logo.ico"),
                new File(appFeature, "logo_big.jpg"),
                new File(appFeature, "baseMtpDevice.xml"),
                new DirFiles(appFeature, @"*.png"),
                new File(appFeature, "DigiCamControl.xbs"),
                new Dir(appFeature, "Data",
                    new Files(appFeature, @"Data\*.*")),
                //new Dir(appFeature, "Plugins",
                //    new Files(appFeature, @"Plugins\*.*", "MahApps.Metro.*", "System.Windows.Interactivity.*",
                //        "WriteableBitmapEx.Wpf.*", "GalaSoft.MvvmLight.*", "*.config")),
                new Dir(appFeature, "Plugins",
                    new Dir(appFeature, "CameraControl.Plugins",
                        new File(appFeature, "Plugins\\CameraControl.Plugins\\CameraControl.Plugins.dll"),
                        new File(appFeature, "Plugins\\CameraControl.Plugins\\dcc.plugin")),
                    new Dir(appFeature, "Plugin.DeviceControlBox",
                        new File(appFeature, "Plugins\\Plugin.DeviceControlBox\\Plugin.DeviceControlBox.dll"),
                        new File(appFeature, "Plugins\\Plugin.DeviceControlBox\\dcc.plugin"))
                    ),
                new Dir(appFeature, "Languages",
                    new DirFiles(appFeature, @"Languages\*.xml")),
                new Dir(appFeature, "Licenses",
                    new DirFiles(appFeature, @"Licenses\*.*")),
                new Dir(appFeature, "x64",
                    new DirFiles(appFeature, @"x64\*.*")),
                new Dir(appFeature, "x86",
                    new DirFiles(appFeature, @"x86\*.*")),
                new Dir(appFeature, "Tools",
                    new DirFiles(appFeature, @"Tools\*.*")),
                new Dir(appFeature, "WebServer",
                    new Files(appFeature, @"WebServer\*.*"))
                );


            var baseDir = new Dir(@"%ProgramFiles%",
                appDir
                );


            Project project = new Project("digiCamControl",
                baseDir,
                new Binary(@"vcredist_x86.exe"),
                new ManagedAction(@"InstallCRTAction",
                    Return.check,
                    When.Before,
                    Step.LaunchConditions,
                    Condition.NOT_Installed,
                    Sequence.InstallUISequence),
                new ManagedAction(@"MyAction", Return.ignore, When.Before, Step.InstallExecute,
                    Condition.NOT_Installed, Sequence.InstallExecuteSequence),
                new ManagedAction(@"SetRightAction", Return.ignore, When.Before, Step.InstallFinalize,
                    Condition.Always, Sequence.InstallExecuteSequence)
                );

            project.UI = WUI.WixUI_InstallDir;
            project.GUID = new Guid("19d12628-7654-4354-a305-9ab0932af676");
            project.SetNetFxPrerequisite("NETFRAMEWORK40FULL='#1'");

#if DEBUG
            project.SourceBaseDir =
                Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Debug\"));
#else
            project.SourceBaseDir =
                Path.GetFullPath(System.IO.Path.Combine(Environment.CurrentDirectory, @"..\CameraControl\bin\Release\"));
#endif

            FileVersionInfo ver =
                FileVersionInfo.GetVersionInfo(Path.Combine(project.SourceBaseDir, "CameraControl.exe"));

            project.LicenceFile = @"Licenses\DigiCamControlLicence.rtf";

            project.Version = new Version(ver.FileMajorPart, ver.FileMinorPart, ver.FileBuildPart, ver.FilePrivatePart);
            project.MajorUpgradeStrategy = MajorUpgradeStrategy.Default;
            //project.MajorUpgradeStrategy.NewerProductInstalledErrorMessage = "A version of the digiCamControl already installed. Unistall it first from Control Panel !";
            project.MajorUpgradeStrategy.RemoveExistingProductAfter = Step.InstallInitialize;
            ////project.MajorUpgradeStrategy.UpgradeVersions = VersionRange.ThisAndOlder;
            ////project.MajorUpgradeStrategy.PreventDowngradingVersions = VersionRange.ThisAndOlder;

            project.ControlPanelInfo.Manufacturer = "Duka Istvan";
            project.OutFileName = string.Format("digiCamControlsetup_{0}", ver.FileVersion);
            project.ControlPanelInfo.ProductIcon = "logo.ico";

            string branding = Path.Combine(project.SourceBaseDir, "branding.xml");
            if (System.IO.File.Exists(branding))
            {
                var doc = new XmlDocument();
                doc.LoadXml(System.IO.File.ReadAllText(branding));
                string name = doc.DocumentElement.SelectSingleNode("/Branding/ApplicationTitle").InnerText;
                project.ControlPanelInfo.Manufacturer = name;
                project.OutFileName = string.Format(name.Replace(" ", "_") + "_{0}", ver.FileVersion);
                appDir.AddFile(new File(appFeature, "branding.xml"));
                appDir.AddDir(new Dir(appFeature, "Branding",
                    new Files(appFeature, @"Branding\*.*")));
                project.Name = name;
                if (System.IO.File.Exists(Path.Combine(project.SourceBaseDir, "Branding", "logo.ico")))
                {
                    project.ControlPanelInfo.ProductIcon = Path.Combine(project.SourceBaseDir, "Branding", "logo.ico");
                    shortcut.IconFile = Path.Combine(project.SourceBaseDir, "Branding", "logo.ico");
                    shortcutD.IconFile = Path.Combine(project.SourceBaseDir, "Branding", "logo.ico");
                    shortcut.Name = name;
                    shortcutD.Name = name;
                }
                if (System.IO.File.Exists(Path.Combine(project.SourceBaseDir, "Branding", "Licence.rtf")))
                    project.ControlPanelInfo.ProductIcon = "Branding\\Licence.rtf";

            }

            project.ResolveWildCards();
            Compiler.PreserveTempFiles = true;
            Compiler.BuildMsi(project);
            ObsPluginSetup.Execute();
        }

    }

    public class CustomActions
    {
        [CustomAction]
        public static ActionResult InstallCRTAction(Session session)
        {
            //This can be successfully executed only from UISequence
                //extract CRT msi into temp directory
                string CRTMsiFile = Path.ChangeExtension(Path.GetTempFileName(), ".exe");
                string CRTMsiId = "vcredist_x86.exe".Expand();//Expand() is needed to normalize file name into file ID

                session.SaveBinary(CRTMsiId, CRTMsiFile);

                //install CTR
                Process.Start(CRTMsiFile,"/passive /install").WaitForExit();

            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult MyAction(Session session)
        {
            var unInstallFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "digiCamControl", "uninstall.exe");
            if (System.IO.File.Exists(unInstallFile))
            {
                try
                {
                    ProcessStartInfo pro = new ProcessStartInfo();
                    pro.Verb = "runas";
                    pro.UseShellExecute = true;
                    pro.FileName = unInstallFile;
                    pro.Arguments = "/S";
                    pro.WindowStyle = ProcessWindowStyle.Normal;
                    Process process = new Process {StartInfo = pro};
                    process.Start();
                    process.WaitForExit();
                }
                catch (Exception ex)
                {
                    session.Log("Uninstall old " + ex.Message);
                }
            }
            return ActionResult.Success;
        }

        [CustomAction]
        public static ActionResult SetRightAction(Session session)
        {
            try
            {
                string folder = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "digiCamControl");
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                DirectoryInfo dInfo = new DirectoryInfo(folder);
                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                SecurityIdentifier everyone = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                dSecurity.AddAccessRule(new FileSystemAccessRule(everyone, FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.InheritOnly, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);
                string cachfolder = Path.Combine(
                                    Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "digiCamControl", "Cache");
                if (Directory.Exists(cachfolder))
                {
                    Directory.Delete(cachfolder, true);
                }

            }
            catch (Exception ex)
            {
                session.Log("Set right error " + ex.Message);
            }
            return ActionResult.Success;
        }
    }
}
