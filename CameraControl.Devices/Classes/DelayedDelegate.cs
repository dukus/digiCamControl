using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;

namespace CameraControl.Devices.Classes
{
    public class DelayedDelegate
    {
        static Timer runDelegates;
        static Dictionary<Action, DateTime> delayedDelegates = new Dictionary<Action, DateTime>();

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

        static void RunDelegates(object sender, EventArgs e)
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
