using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CameraControl.Core.Interfaces;

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
            Templates.Add("[Exposure Compensation]", templatePharser);
            Templates.Add("[Date yyyy-MM-dd]", templatePharser);
            Templates.Add("[Barcode]", templatePharser);
            Templates.Add("[File format]", templatePharser);
            Templates.Add("[Camera Name]", templatePharser);
            Templates.Add("[Selected Tag1]", templatePharser);
            Templates.Add("[Selected Tag2]", templatePharser);
            Templates.Add("[Selected Tag3]", templatePharser);
            Templates.Add("[Selected Tag4]", templatePharser);
            Templates.Add("[Unix Time]", templatePharser);

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
        }
    }
}
