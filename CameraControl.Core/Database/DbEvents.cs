using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace CameraControl.Core.Database
{
    public class DbEvents
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public EventType EventType { get; set; }
        public DateTime DateTime { get; set; }
        public string File { get; set; }
        public string CameraSerial { get; set; }
        public string Camera { get; set; }

        public DbEvents(EventType eventType)
        {
            EventType = eventType;
            DateTime=DateTime.Now;
        }
    }
}
