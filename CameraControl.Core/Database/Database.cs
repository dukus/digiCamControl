using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CameraControl.Core.Classes;
using CameraControl.Devices;
using SQLite;

namespace CameraControl.Core.Database
{
    public class Database : SQLiteConnection
    {
        public Database(string path)
            : base(path)
        {
            CreateTable<DbEvents>();
            CreateTable<DbFile>();
        }

        public void Add(DbEvents events)
        {
            try
            {
                this.Insert(events);
            }
            catch (Exception ex)
            {
                Log.Error("",ex);
            }
        }

        public void Add(DbFile file)
        {
            try
            {
                this.Insert(file);
            }
            catch (Exception ex)
            {
                Log.Error("", ex);
            }
        }

        public void StartEvent(EventType eventType)
        {
            try
            {
                Insert(new DbEvents(eventType));
            }
            catch (Exception ex)
            {
                Log.Error("Insert error", ex);
            }
        }

        public void EndEvent(EventType eventType)
        {
            try
            {
                var e = Table<DbEvents>().Last(x => x.EventType == EventType.App);
                if (!e.EndDate.HasValue)
                {
                    e.EndDate = DateTime.Now;
                    Update(e);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Insert error", ex);
            }
        }

        public IEnumerable<DbEvents> GetApp(DateTime from, DateTime to)
        {
            var res =
                Table<DbEvents>()
                    .Where((x) => x.EventType == EventType.App).ToList();
            return res.Where(
                        (x) =>
                            x.EndDate != null && (x.StartDate != null && (x.StartDate.Value >= @from &&
                                                                          x.EndDate.Value.Date <= to.Date)));
        }

        public IEnumerable<DbFile> GetFiles(DateTime from, DateTime to)
        {
            var res = Table<DbFile>().ToList();
            return res.Where(
                (x) => x.Date != null && (x.Date.Value.Date >= from.Date && x.Date.Value.Date <= to.Date));

        }

        public void SaveFile(FileItem fileItem)
        {
            var e = Table<DbFile>().LastOrDefault(x => x.FileId == fileItem.Id);
            if (e == null)
            {
                var dbfile = new DbFile(fileItem,"","",ServiceProvider.Settings.DefaultSession.Name);
                Insert(dbfile);
            }
            else
            {
                e.Copy(fileItem);
                Update(e);
            }
        }
    }
}
