using System;
using System.Collections.Generic;

namespace PhotoBooth
{
    public class WindowsMessage
    {
        private static Lazy<List<WindowsMessage>> oneButtonMessage = new Lazy<List<WindowsMessage>>(GetOneButtonMessages);

        public static List<WindowsMessage> OneButtonMessages
        {
            get { return oneButtonMessage.Value; }
        }

        public int Message { get; set; }

        public IntPtr wParam { get; set; }

        public IntPtr lParam { get; set; }

        public string Description { get; set; }

        public string Name { get; set; }

        public bool IgnoreLParam { get; set; }

        public bool IgnoreWParam { get; set; }

        public bool Matches(int message, IntPtr wparam, IntPtr lparam)
        {
            return this.Message == message && (this.IgnoreWParam || this.wParam == wparam) && (this.IgnoreLParam || this.lParam == lparam);
        }

        public WindowsMessage()
        {
            this.IgnoreLParam = true;
            this.IgnoreWParam = true;
        }

        private static List<WindowsMessage> GetOneButtonMessages()
        {
            List<WindowsMessage> messages = new List<WindowsMessage>();
            messages.Add(new WindowsMessage()
            {
                Message = Constants.WM_LBUTTONDOWN,
                Name = "WM_LBUTTONDOWN",
                Description = "Left Mouse Button Down"
            });
            messages.Add(new WindowsMessage()
            {
                Message = Constants.WM_MBUTTONDOWN,
                Name = "WM_MBUTTONDOWN",
                Description = "Middle Mouse Button Down"
            });
            messages.Add(new WindowsMessage()
            {
                Message = Constants.WM_RBUTTONDOWN,
                Name = "WM_RBUTTONDOWN",
                Description = "Right Mouse Button Down"
            });

            return messages;
        }
    }
}
