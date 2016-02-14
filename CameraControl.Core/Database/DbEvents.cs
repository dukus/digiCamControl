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
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string CameraSerial { get; set; }
        public string Camera { get; set; }

        [Ignore]
        public TimeSpan Duration
        {
            get
            {
                if (StartDate.HasValue && EndDate.HasValue)
                {
                    return EndDate.Value - StartDate.Value;
                }
                return new TimeSpan();
            }
        }

        public DbEvents()
        {
            
        }

        public DbEvents(EventType eventType)
        {
            EventType = eventType;
            StartDate = DateTime.Now;
        }

    }
}
