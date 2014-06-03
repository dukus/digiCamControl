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
using System.Timers;

#endregion

namespace CameraControl.Devices.Classes
{
    public class DelayedDelegate
    {
        private static Timer runDelegates;
        private static Dictionary<Action, DateTime> delayedDelegates = new Dictionary<Action, DateTime>();

        static DelayedDelegate()
        {
            runDelegates = new Timer();
            runDelegates.Interval = 250;
            runDelegates.Elapsed += RunDelegates;
            runDelegates.Enabled = true;
            runDelegates.Start();
        }

        public static void Add(Action method, int delay)
        {
            delayedDelegates.Add(method, DateTime.Now + TimeSpan.FromMilliseconds(delay));
        }

        private static void RunDelegates(object sender, EventArgs e)
        {
            List<Action> removeDelegates = new List<Action>();

            foreach (Action method in delayedDelegates.Keys)
            {
                if (DateTime.Now >= delayedDelegates[method])
                {
                    method();
                    removeDelegates.Add(method);
                }
            }

            foreach (Action method in removeDelegates)
            {
                delayedDelegates.Remove(method);
            }
        }
    }
}