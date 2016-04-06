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

namespace CameraControl.Devices.Classes
{
    public class LiveViewData
    {
        public int LiveViewImageWidth { get; set; }
        public int LiveViewImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the width of the captured image.
        /// </summary>
        /// <value>
        /// The width of the image.
        /// </value>
        public int ImageWidth { get; set; }
        /// <summary>
        /// Gets or sets the height of the captured image.
        /// </summary>
        /// <value>
        /// The height of the image.
        /// </value>
        public int ImageHeight { get; set; }

        /// <summary>
        /// Gets or sets the focus point x axis .
        /// </summary>
        /// <value>
        /// The focus x.
        /// </value>
        public int FocusX { get; set; }
        /// <summary>
        /// Gets or sets the focus point y axis.
        /// </summary>
        /// <value>
        /// The focus y.
        /// </value>
        public int FocusY { get; set; }

        public int FocusFrameXSize { get; set; }
        public int FocusFrameYSize { get; set; }


        /// <summary>
        /// Gets or sets a value indicating whether focussing info included.
        /// </summary>
        /// <value>
        ///   <c>true</c> if [have focus data]; otherwise, <c>false</c>.
        /// </value>
        public bool HaveFocusData { get; set; }

        /// <summary>
        /// Gets or sets the live view image data.
        /// May contain additional information, use ImageDataPosition to get live view image 
        /// </summary>
        /// <value>
        /// The image data.
        /// </value>
        public byte[] ImageData { get; set; }

        public bool Focused { get; set; }

        /// <summary>
        /// Gets or sets the image data starting position in ImageData.
        /// </summary>
        /// <value>
        /// The image data position.
        /// </value>
        public int ImageDataPosition { get; set; }

        public int Rotation { get; set; }

        public bool MovieIsRecording { get; set; }

        public bool IsLiveViewRunning { get; set; }

        public bool HaveLevelAngleData { get; set; }

        public decimal LevelAngleRolling { get; set; }

        public decimal LevelAnglePitching { get; set; }

        public decimal LevelAngleYawing { get; set; }

        public decimal MovieTimeRemain { get; set; }

        public int SoundL { get; set; }
        
        public int SoundR { get; set; }

        public int PeakSoundL { get; set; }

        public int PeakSoundR { get; set; }

        public bool HaveSoundData { get; set; }
        
        public LiveViewData()
        {
            IsLiveViewRunning = true;
            HaveLevelAngleData = false;
            SoundL = 0;
            SoundR = 0;
            HaveSoundData = false;
        }
    }
}