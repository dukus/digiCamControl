using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AForge.Math.Geometry;

namespace CameraControl.Core.TclScripting
{
    public delegate void OutputEventHandler(string message, bool newline);

    public class ConsoleRedirect : TextWriter
    {
        private string _line = "";
        public event OutputEventHandler Output;

        public override void Write(char value)
        {
            if (value == '\n')
            {
                Write(_line);
                _line = "";
            }
            else
            {
                _line += value;
            }
        }

        public override void Write(string value)
        {
            if (Output != null)
                Output(value, false);
        }

        public override Encoding Encoding
        {
            get { return Encoding.ASCII; }
        }
    }
}
