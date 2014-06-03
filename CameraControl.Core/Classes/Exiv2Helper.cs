using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows;
using CameraControl.Devices;

namespace CameraControl.Core.Classes
{
    public enum FocusPointType
    {
        Square,
        HRectangle,
        VRectangle
    }

    public class FocusPointDefinition
    {
        public double XRat { get; set; }
        public double YRat { get; set; }
        public FocusPointType FocusPointType { get; set; }
    }

    public class Exiv2Data
    {
        public string Tag { get; set; }
        public string Type { get; set; }
        public string Length { get; set; }
        public string Value { get; set; }


        public override string ToString()
        {
            return Tag + "|" + Type + "|" + Length + "|" + Value;
        }
    }

    public class Exiv2Helper
    {
        public Dictionary<string, Exiv2Data> Tags { get; set; }
        public List<Rect> Focuspoints { get; set; }
        public Dictionary<string, FocusPointDefinition> FocusPoints7 { get; set; }
        public Dictionary<string, FocusPointDefinition> FocusPoints11 { get; set; }
        public Dictionary<string, FocusPointDefinition> FocusPoints51 { get; set; }
        public int Width;
        public int Height;

        public Exiv2Helper()
        {
            Tags = new Dictionary<string, Exiv2Data>();
            Focuspoints = new List<Rect>();
            FocusPoints7 = new Dictionary<string, FocusPointDefinition>
                        {
                          {"Center", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.50}},
                          {"Mid-right", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.75, YRat = 0.50}},
                          {"Bottom", new FocusPointDefinition(){FocusPointType = FocusPointType.VRectangle,XRat =  0.50, YRat =  0.75}},
                          {"Mid-left", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.25, YRat =  0.50}},
                          {"Top", new FocusPointDefinition(){FocusPointType = FocusPointType.VRectangle,XRat =  0.50, YRat =  0.25}},
                          {"6", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.31, YRat =  0.62}},
                          {"7", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.21, YRat =  0.50}},
                          {"8", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.67, YRat =  0.50}},
                          {"9", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.67, YRat =  0.36}},
                          {"10", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.67, YRat = 0.62}},
                          {"11", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.80, YRat = 0.50}},
                        };
            FocusPoints11 = new Dictionary<string, FocusPointDefinition>
                        {
                          {"1", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.50}},
                          {"2", new FocusPointDefinition(){FocusPointType = FocusPointType.VRectangle,XRat =  0.50, YRat = 0.28}},
                          {"3", new FocusPointDefinition(){FocusPointType = FocusPointType.VRectangle,XRat =  0.50, YRat =  0.72}},
                          {"4", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.31, YRat =  0.50}},
                          {"5", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.31, YRat =  0.36}},
                          {"6", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.31, YRat =  0.62}},
                          {"7", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.21, YRat =  0.50}},
                          {"8", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat = 0.67, YRat =  0.50}},
                          {"9", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.67, YRat =  0.36}},
                          {"10", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.67, YRat = 0.62}},
                          {"11", new FocusPointDefinition(){FocusPointType = FocusPointType.HRectangle,XRat =  0.80, YRat = 0.50}},
                        };
            FocusPoints51 = new Dictionary<string, FocusPointDefinition>()
                      {
                        {"1", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.50}},
                        {"2", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.42}},
                        {"3", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.35}},
                        {"4", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.59}},
                        {"5", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.50, YRat =  0.65}},
                        {"6", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.56, YRat =  0.50}},
                        {"7", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.56, YRat =  0.42}},
                        {"8", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.56, YRat =  0.35}},
                        {"9", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.56, YRat =  0.59}},
                        {"10", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.56, YRat =  0.65}},
                        {"11", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.45, YRat =  0.50}},
                        {"12", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.45, YRat =  0.42}},
                        {"13", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.45, YRat =  0.35}},
                        {"14", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.45, YRat =  0.59}},
                        {"15", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.45, YRat =  0.65}},
                        {"16", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.61, YRat =  0.50}},
                        {"17", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.61, YRat =  0.44}},
                        {"18", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.61, YRat =  0.38}},
                        {"19", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.61, YRat =  0.57}},
                        {"20", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.61, YRat =  0.44}},
                        {"21", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.65, YRat =  0.50}},
                        {"22", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.65, YRat =  0.44}},
                        {"23", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.65, YRat =  0.38}},
                        {"24", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.65, YRat =  0.57}},
                        {"25", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.65, YRat =  0.44}},
                        {"26", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.70, YRat =  0.50}},
                        {"27", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.70, YRat =  0.44}},
                        {"28", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.70, YRat =  0.38}},
                        {"29", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.70, YRat =  0.57}},
                        {"30", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.70, YRat =  0.44}},
                        {"31", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.75, YRat =  0.50}},
                        {"32", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.75, YRat =  0.44}},
                        {"33", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.75, YRat =  0.57}},
                        {"34", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.39, YRat =  0.50}},
                        {"35", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.39, YRat =  0.44}},
                        {"36", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.39, YRat =  0.38}},
                        {"37", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.39, YRat =  0.57}},
                        {"38", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.39, YRat =  0.44}},
                        {"39", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.35, YRat =  0.50}},
                        {"40", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.35, YRat =  0.44}},
                        {"41", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.35, YRat =  0.38}},
                        {"42", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.35, YRat =  0.57}},
                        {"43", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.35, YRat =  0.44}},
                        {"44", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.3, YRat =  0.50}},
                        {"45", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.3, YRat =  0.44}},
                        {"46", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.3, YRat =  0.38}},
                        {"47", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.3, YRat =  0.57}},
                        {"48", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.3, YRat =  0.44}},
                        {"49", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.25, YRat =  0.50}},
                        {"50", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.25, YRat =  0.44}},
                        {"51", new FocusPointDefinition(){FocusPointType = FocusPointType.Square,XRat =  0.25, YRat =  0.57}},
                      };
        }

        /// <summary>
        /// Saves a comment string in a image file as Iptc.Application2.Caption
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="comment">The comment.</param>
        public static void SaveComment(string filename, string comment)
        {
            try
            {
                var startInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools", "exiv2.exe"))
                {
                    Arguments = "-M\"set Iptc.Application2.Caption " + comment + "\" " + "\"" + filename + "\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                var process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                Log.Error("Error set comment to file", exception);
            }
        }


        /// <summary>
        /// Adds a keyword to image file
        /// </summary>
        /// <param name="filename">The filename.</param>
        /// <param name="keyword">The keyword.</param>
        public static void AddKeyword(string filename, string keyword)
        {
            try
            {
                var startInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools", "exiv2.exe"))
                {
                    Arguments = "-M\"add Iptc.Application2.Keywords " + keyword + "\" " + "\"" + filename + "\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                var process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                Log.Error("Error set keyword to file", exception);
            }
        }

        public static void DelKeyword(string filename)
        {
            try
            {
                var startInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools", "exiv2.exe"))
                {
                    Arguments = "-M\"del Iptc.Application2.Keywords\" " + "\"" + filename + "\"",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Minimized
                };

                var process = Process.Start(startInfo);
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                Log.Error("Error set keyword to file", exception);
            }
        }

        public void Load(string filename, int relWidth, int relHeight)
        {
            var startInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools", "exiv2.exe"))
            {
                Arguments = "\"" + filename + "\"" + " -p a",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            var process = Process.Start(startInfo);
            process.OutputDataReceived += new DataReceivedEventHandler(process_OutputDataReceived);
            process.BeginOutputReadLine();
            //string outstr = process.StandardOutput.ReadToEnd();

            process.WaitForExit();
            if (Tags.ContainsKey("Exif.Photo.PixelXDimension"))
            {
                int.TryParse(Tags["Exif.Photo.PixelXDimension"].Value, out Width);
                int.TryParse(Tags["Exif.Photo.PixelYDimension"].Value, out Height);
            }
            else
            {
                if (Tags.ContainsKey("Exif.SubImage2.ImageWidth"))
                    int.TryParse(Tags["Exif.SubImage2.ImageWidth"].Value, out Width);
                if (Tags.ContainsKey("Exif.SubImage2.ImageLength"))
                    int.TryParse(Tags["Exif.SubImage2.ImageLength"].Value, out Height);
            }
            double dw = (double)relWidth / Width;
            double dh = (double)relHeight / Height;
            Focuspoints = new List<Rect>();
            if (Tags.ContainsKey("Exif.NikonAf2.ContrastDetectAF"))
            {
                if (Tags["Exif.NikonAf2.ContrastDetectAF"].Value == "On")
                {
                    int x = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaXPosition"].Value) * dw);
                    int y = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaYPosition"].Value) * dh);
                    int w = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaWidth"].Value) * dw);
                    int h = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaHeight"].Value) * dh);
                    if (x - (w / 2) > 0 && y - (h / 2) > 0 && x > 0 && y > 0 && w > 0 && h > 0)
                        Focuspoints.Add(new Rect(x - (w / 2), y - (h / 2), w, h));
                }
            }
            if (Tags.ContainsKey("Exif.NikonAf2.PhaseDetectAF") && Tags["Exif.NikonAf2.PhaseDetectAF"].Value == "On (11-point)")
            {
                if (Tags["Exif.NikonAf2.AFPointsUsed"].Value.Contains(" "))
                {
                    string[] strbytes = Tags["Exif.NikonAf2.AFPointsUsed"].Value.Split(' ');
                    byte[] bytes = new byte[8];
                    for (int i = 0; i < strbytes.Length; i++)
                    {
                        byte.TryParse(strbytes[i], out bytes[i]);
                    }
                    Int64 focuspoints = BitConverter.ToInt64(bytes, 0);
                    for (var i = 1; i < 12; i++)
                    {
                        if (StaticHelper.GetBit(focuspoints, i - 1))
                            Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints11[i.ToString()], dw, dh));
                    }
                }
            }
            if (Tags.ContainsKey("Exif.NikonAf2.PhaseDetectAF") && Tags["Exif.NikonAf2.PhaseDetectAF"].Value == "On (51-point)")
            {
                if (Tags["Exif.NikonAf2.AFPointsUsed"].Value.Contains(" "))
                {
                    string[] strbytes = Tags["Exif.NikonAf2.AFPointsUsed"].Value.Split(' ');
                    byte[] bytes = new byte[8];
                    for (int i = 0; i < strbytes.Length; i++)
                    {
                        byte.TryParse(strbytes[i], out bytes[i]);
                    }
                    BitArray bitArray = new BitArray(bytes);
                    for (var i = 1; i < 52; i++)
                    {
                        if (bitArray.Get(i - 1))
                            Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints51[i.ToString()], dw, dh));
                    }
                }
            }

            if (Tags.ContainsKey("Exif.NikonAf.AFPointsInFocus"))
            {
                if (FocusPoints7.ContainsKey(Tags["Exif.NikonAf.AFPointsInFocus"].Value))
                {
                    Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints7[Tags["Exif.NikonAf.AFPointsInFocus"].Value], dw, dh));
                }
            }
            return;
        }


        public void Load(FileItem fileItem)
        {
            var startInfo = new ProcessStartInfo(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Tools", "exiv2.exe"))
            {
                Arguments = "\"" + fileItem.FileName + "\"" + " -p a",
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            var process = Process.Start(startInfo);
            process.OutputDataReceived += process_OutputDataReceived;
            process.BeginOutputReadLine();

            process.WaitForExit();
            if (Tags.ContainsKey("Exif.Photo.PixelXDimension"))
            {
                int.TryParse(Tags["Exif.Photo.PixelXDimension"].Value, out Width);
                int.TryParse(Tags["Exif.Photo.PixelYDimension"].Value, out Height);
            }
            else
            {
                if (Tags.ContainsKey("Exif.SubImage2.ImageWidth"))
                    int.TryParse(Tags["Exif.SubImage2.ImageWidth"].Value, out Width);
                if (Tags.ContainsKey("Exif.SubImage2.ImageLength"))
                    int.TryParse(Tags["Exif.SubImage2.ImageLength"].Value, out Height);
            }
            int relWidth = Width;
            int relHeight = Height;

            Focuspoints = new List<Rect>();
            if (Tags.ContainsKey("Exif.NikonAf2.ContrastDetectAF"))
            {
                if (Tags["Exif.NikonAf2.ContrastDetectAF"].Value == "On")
                {
                    int x = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaXPosition"].Value) );
                    int y = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaYPosition"].Value) );
                    int w = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaWidth"].Value)  );
                    int h = (int)(ToSize(Tags["Exif.NikonAf2.AFAreaHeight"].Value) );
                    if (x - (w / 2) > 0 && y - (h / 2) > 0 && x > 0 && y > 0 && w > 0 && h > 0)
                        Focuspoints.Add(new Rect(x - (w / 2), y - (h / 2), w, h));
                }
            }
            if (Tags.ContainsKey("Exif.NikonAf2.PhaseDetectAF") && Tags["Exif.NikonAf2.PhaseDetectAF"].Value == "On (11-point)")
            {
                if (Tags["Exif.NikonAf2.AFPointsUsed"].Value.Contains(" "))
                {
                    string[] strbytes = Tags["Exif.NikonAf2.AFPointsUsed"].Value.Split(' ');
                    byte[] bytes = new byte[8];
                    for (int i = 0; i < strbytes.Length; i++)
                    {
                        byte.TryParse(strbytes[i], out bytes[i]);
                    }
                    Int64 focuspoints = BitConverter.ToInt64(bytes, 0);
                    for (var i = 1; i < 12; i++)
                    {
                        if (StaticHelper.GetBit(focuspoints, i - 1))
                            Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints11[i.ToString()], 1, 1));
                    }
                }
            }
            if (Tags.ContainsKey("Exif.NikonAf2.PhaseDetectAF") && Tags["Exif.NikonAf2.PhaseDetectAF"].Value == "On (51-point)")
            {
                if (Tags["Exif.NikonAf2.AFPointsUsed"].Value.Contains(" "))
                {
                    string[] strbytes = Tags["Exif.NikonAf2.AFPointsUsed"].Value.Split(' ');
                    byte[] bytes = new byte[8];
                    for (int i = 0; i < strbytes.Length; i++)
                    {
                        byte.TryParse(strbytes[i], out bytes[i]);
                    }
                    BitArray bitArray = new BitArray(bytes);
                    for (var i = 1; i < 52; i++)
                    {
                        if (bitArray.Get(i - 1))
                            Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints51[i.ToString()], 1, 1));
                    }
                }
            }

            if (Tags.ContainsKey("Exif.NikonAf.AFPointsInFocus"))
            {
                if (FocusPoints7.ContainsKey(Tags["Exif.NikonAf.AFPointsInFocus"].Value))
                {
                    Focuspoints.Add(ToRect(relWidth, relHeight, FocusPoints7[Tags["Exif.NikonAf.AFPointsInFocus"].Value], 1, 1));
                }
            }

            fileItem.FileInfo = new FileInfo();
            fileItem.FileInfo.ExifTags.Items.Clear();

            foreach (KeyValuePair<string, Exiv2Data> data in Tags)
            {
                fileItem.FileInfo.ExifTags.Items.Add(new ValuePair() {Name = data.Key, Value = data.Value.Value});
            }
            fileItem.FileInfo.FocusPoints.Clear();
            foreach (Rect focuspoint in Focuspoints)
            {
                fileItem.FileInfo.FocusPoints.Add(focuspoint);
            }
            return;
        }



        void process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (e.Data != null && !Tags.ContainsKey(e.Data.Substring(0, 45).Trim()))
                Tags.Add(e.Data.Substring(0, 45).Trim(), new Exiv2Data() { Tag = e.Data.Substring(0, 45).Trim(), Type = e.Data.Substring(45, 11), Length = e.Data.Substring(45 + 11, 4), Value = e.Data.Substring(45 + 11 + 4).Trim() });
        }

        private int ToSize(string s)
        {
            int val = 0;
            if (int.TryParse(s, out val))
            {
                byte[] bytval = BitConverter.GetBytes(val);
                return BitConverter.ToInt16(new[] { bytval[1], bytval[0] }, 0);
            }
            return 0;
        }

        private Rect ToRect(int w, int h, FocusPointDefinition definition, double dw, double dh)
        {
            switch (definition.FocusPointType)
            {
                case FocusPointType.Square:
                    return new Rect(w * definition.XRat - (150 * dw), h * definition.YRat - (100 * dh), 300 * dw, 200 * dh);
                case FocusPointType.VRectangle:
                    return new Rect(w * definition.XRat - (150 * dw), h * definition.YRat - (50 * dh), 300 * dw, 100 * dh);
                case FocusPointType.HRectangle:
                    return new Rect(w * definition.XRat - (50 * dw), h * definition.YRat - (150 * dh), 100 * dw, 300 * dh);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        // live view in focus 
        //Exif.NikonAf2.Version                        Undefined   4  1.00
        //Exif.NikonAf2.ContrastDetectAF               Byte        1  On
        //Exif.NikonAf2.AFAreaMode                     Byte        1  2
        //Exif.NikonAf2.PhaseDetectAF                  Byte        1  Off
        //Exif.NikonAf2.PrimaryAFPoint                 Byte        1  0
        //Exif.NikonAf2.AFPointsUsed                   Byte        7  0 0 0 0 0 0 0
        //Exif.NikonAf2.AFImageWidth                   Short       1  16403
        //Exif.NikonAf2.AFImageHeight                  Short       1  49164
        //Exif.NikonAf2.AFAreaXPosition                Short       1  13835
        //Exif.NikonAf2.AFAreaYPosition                Short       1  61953
        //Exif.NikonAf2.AFAreaWidth                    Short       1  12291
        //Exif.NikonAf2.AFAreaHeight                   Short       1  40962
        //Exif.NikonAf2.ContrastDetectAFInFocus        Short       1  1

        // no liveview 3 point focus
        //Exif.NikonAf2.ContrastDetectAF               Byte        1  Off
        //Exif.NikonAf2.AFAreaMode                     Byte        1  8
        //Exif.NikonAf2.PhaseDetectAF                  Byte        1  On (11-point)
        //Exif.NikonAf2.PrimaryAFPoint                 Byte        1  1
        //Exif.NikonAf2.AFPointsUsed                   Byte        7  35 0 0 0 0 0 0
        //Exif.NikonAf2.AFImageWidth                   Short       1  0
        //Exif.NikonAf2.AFImageHeight                  Short       1  0
        //Exif.NikonAf2.AFAreaXPosition                Short       1  0
        //Exif.NikonAf2.AFAreaYPosition                Short       1  0
        //Exif.NikonAf2.AFAreaWidth                    Short       1  0
        //Exif.NikonAf2.AFAreaHeight                   Short       1  0
        //Exif.NikonAf2.ContrastDetectAFInFocus        Short       1  0
    }
}
