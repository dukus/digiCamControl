using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CameraControl.Core.Interfaces;
using CameraControl.Devices;

namespace CameraControl.Core.Classes
{
    public class FilenameTemplateManager
    {
        public Dictionary<string, IFilenameTemplate> Templates { get; set; }


        public List<string> Values
        {
            get { return Templates.Keys.ToList(); }
        }

        public FilenameTemplateManager()
        {
            Templates = new Dictionary<string, IFilenameTemplate>();
            var templatePharser = new FilenameTemplate();

            Templates.Add("[Counter 4 digit]", templatePharser);            
            Templates.Add("[Camera Counter 4 digit]", templatePharser);
            Templates.Add("[Session Name]", templatePharser);
            Templates.Add("[Capture Name]", templatePharser);
            Templates.Add("[Series 4 digit]", templatePharser);
            Templates.Add("[Exposure Compensation]", templatePharser);
            Templates.Add("[FNumber]", templatePharser);
            Templates.Add("[Date yyyy-MM-dd]", templatePharser);
            Templates.Add("[Barcode]", templatePharser);
            Templates.Add("[File format]", templatePharser);
            Templates.Add("[Camera Name]", templatePharser);
            Templates.Add("[Selected Tag1]", templatePharser);
            Templates.Add("[Selected Tag2]", templatePharser);
            Templates.Add("[Selected Tag3]", templatePharser);
            Templates.Add("[Selected Tag4]", templatePharser);
            Templates.Add("[Unix Time]", templatePharser);
            Templates.Add("[Original Filename]", templatePharser);

            Templates.Add("[Counter 3 digit]", templatePharser);
            Templates.Add("[Counter 5 digit]", templatePharser);
            Templates.Add("[Counter 6 digit]", templatePharser);
            Templates.Add("[Counter 7 digit]", templatePharser);
            Templates.Add("[Counter 8 digit]", templatePharser);
            Templates.Add("[Counter 9 digit]", templatePharser);

            Templates.Add("[Camera Counter 3 digit]", templatePharser);
            Templates.Add("[Camera Counter 5 digit]", templatePharser);
            Templates.Add("[Camera Counter 6 digit]", templatePharser);
            Templates.Add("[Camera Counter 7 digit]", templatePharser);
            Templates.Add("[Camera Counter 8 digit]", templatePharser);
            Templates.Add("[Camera Counter 9 digit]", templatePharser);

            Templates.Add("[Date yyyy]", templatePharser);
            Templates.Add("[Date yyyy-MM]", templatePharser);
            Templates.Add("[Date yyyy-MM-dd-hh-mm-ss]", templatePharser);
            Templates.Add("[Date MMM]", templatePharser);
            Templates.Add("[Time hh-mm-ss]", templatePharser);
            Templates.Add("[Time hh-mm]", templatePharser);
            Templates.Add("[Time hh]", templatePharser);
            Templates.Add("[DB Row 1]", templatePharser);
            Templates.Add("[DB Row 2]", templatePharser);
            Templates.Add("[DB Row 3]", templatePharser);
            Templates.Add("[DB Row 4]", templatePharser);
            Templates.Add("[DB Row 5]", templatePharser);
            Templates.Add("[DB Row 6]", templatePharser);
            Templates.Add("[DB Row 7]", templatePharser);
            Templates.Add("[DB Row 8]", templatePharser);
            Templates.Add("[DB Row 9]", templatePharser);
        }

        public string GetExample(string res, PhotoSession session, ICameraDevice device, string fileName)
        {
            Regex regPattern = new Regex(@"\[(.*?)\]", RegexOptions.Singleline);
            MatchCollection matchX = regPattern.Matches(res);
            foreach (Match match in matchX)
            {
                if (ServiceProvider.FilenameTemplateManager.Templates.ContainsKey(match.Value))
                {
                    res = res.Replace(match.Value,
                        ServiceProvider.FilenameTemplateManager.Templates[match.Value].Pharse(match.Value, session, device,
                            fileName));
                }
            }

            //prevent multiple \ if a tag is empty 
            while (res.Contains(@"\\"))
            {
                res = res.Replace(@"\\", @"\");
            }
            // if the file name start with \ the Path.Combine isn't work right 
            if (res.StartsWith("\\"))
                res = res.Substring(1);
            return res;
        }
    }
}
