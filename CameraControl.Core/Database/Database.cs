using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace CameraControl.Core.Database
{
    public class Database : SQLiteConnection
    {
        public Database(string path)
            : base(path)
        {
            CreateTable<DbEvents>();
        }

        public void Add(DbEvents events)
        {
            this.Insert(events);
        }
    }
}
