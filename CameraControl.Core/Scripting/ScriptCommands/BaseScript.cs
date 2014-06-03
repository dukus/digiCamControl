#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Xml;
using CameraControl.Core.Classes;
using CameraControl.Devices.Classes;

#endregion

namespace CameraControl.Core.Scripting.ScriptCommands
{
    public class BaseScript : BaseFieldClass, IScriptCommand
    {
        public ValuePairEnumerator LoadedParams = new ValuePairEnumerator();

        public BaseScript()
        {
            HaveEditControl = false;
        }

        #region Implementation of IScriptCommand

        public virtual bool Execute(ScriptObject scriptObject)
        {
            return true;
        }

        public virtual IScriptCommand Create()
        {
            return new BaseScript();
        }

        public virtual XmlNode Save(XmlDocument doc)
        {
            XmlNode nameNode = doc.CreateElement(Name);
            foreach (var valuePair in LoadedParams.Items)
            {
                nameNode.Attributes.Append(ScriptManager.CreateAttribute(doc, valuePair.Name, valuePair.Value));
            }
            return nameNode;
        }

        public virtual IScriptCommand Load(XmlNode node)
        {
            if (node.Attributes != null)
            {
                foreach (XmlAttribute attribute in node.Attributes)
                {
                    LoadedParams[attribute.Name] = attribute.Value;
                }
            }
            return this;
        }

        private bool _isExecuted;

        public virtual bool IsExecuted
        {
            get { return _isExecuted; }
            set
            {
                _isExecuted = value;
                NotifyPropertyChanged("IsExecuted");
            }
        }

        private bool _executing;

        public virtual bool Executing
        {
            get { return _executing; }
            set
            {
                _executing = value;
                NotifyPropertyChanged("Executing");
            }
        }

        public virtual string Name { get; set; }

        public virtual string DisplayName
        {
            get
            {
                StringBuilder s = new StringBuilder();
                s.Append("[");
                s.Append(Name);
                s.Append("]");
                foreach (ValuePair item in LoadedParams.Items)
                {
                    s.Append(string.Format("{0}={1}", item.Name, item.Value));
                }
                return s.ToString();
            }
            set { }
        }

        public string Description { get; set; }

        public string DefaultValue { get; set; }

        public virtual UserControl GetConfig()
        {
            return new UserControl();
        }

        public bool HaveEditControl { get; set; }

        #endregion
    }
}