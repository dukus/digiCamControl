using System;
using System.Collections.Generic;
using System.Linq;
using CameraControl.Devices;
using SQLite;

namespace Capture.Workflow.Core.Database
{
    public class SqliteDatabase: SQLiteConnection
    {
        private object _locker = new object();

        public SqliteDatabase(string path)
            : base(path)
        {

            CreateTable<DbQueue>();
        }

        public void Add(DbQueue fileQueue)
        {
            lock (_locker)
            {
                Insert(fileQueue);
            }
        }

        public void Save(DbQueue fileQueue)
        {
            lock (_locker)
            {
                try
                {
                    Update(fileQueue);
                }
                catch (Exception ex)
                {
                    Log.Error("Queue insert error", ex);
                }
            }
        }

        public void Clear()
        {
            lock (_locker)
            {
                DeleteAll<DbQueue>();
            }
        }

        public List<DbQueue> GetList()
        {
            lock (_locker)
            {
                try
                {
                    return Table<DbQueue>().Where((x) => x.Done == false).ToList();
                }
                catch (Exception ex)
                {
                    Log.Error("Queue insert error", ex);
                }
                return new List<DbQueue>();
            }
        }


    }
}
