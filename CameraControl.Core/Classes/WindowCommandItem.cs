using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CameraControl.Devices.Classes;

namespace CameraControl.Core.Classes
{
    public class WindowCommandItem : BaseFieldClass
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public bool HaveKeyAssigned { get; set; }

        public string Key { get; set; }

        public Key KeyEnum
        {

            get
            {
                Key key = System.Windows.Input.Key.None;
                Enum.TryParse(Key, out key);
                return key;
            }
        }

        public bool Alt { get; set; }

        public bool Ctrl { get; set; }

        public WindowCommandItem()
        {
            Key = "None";
        }

        public void SetKey(Key key)
        {
            Key = Enum.GetName(typeof (Key), key);
        }
    }
}
