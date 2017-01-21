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
using WIA;

#endregion

namespace CameraControl.Devices.Classes
{
    public delegate void PhotoCapturedEventHandler(object sender, PhotoCapturedEventArgs eventArgs);

    public class PhotoCapturedEventArgs
    {
        private Item _wiaImageItem;

        public Item WiaImageItem
        {
            get { return _wiaImageItem; }
            set
            {
                _wiaImageItem = value;
                //ImageFile = (ImageFile)WiaImageItem.Transfer("{B96B3CAE-0728-11D3-9D7B-0000F81EF32E}");
            }
        }

        public ImageFile ImageFile { get; set; }

        public bool Transfer(string fileName)
        {
            //imageFile.SaveFile(fileName);
            return true;
        }

        public object EventArgs { get; set; }

        public ICameraDevice CameraDevice { get; set; }
        public string FileName { get; set; }
        public object Handle { get; set; }


    }
}