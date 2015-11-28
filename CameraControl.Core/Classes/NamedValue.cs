using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CameraControl.Core.Classes
{
    public class NamedValue<T> 
    {
        public string Name { get; set; }
        public T Value { get; set; }

        public NamedValue(string name,T value)
        {
            Name = name;
            Value = value;
        }
    }
}
