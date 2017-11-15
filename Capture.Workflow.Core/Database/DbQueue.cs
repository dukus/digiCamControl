using SQLite;

namespace Capture.Workflow.Core.Database
{
    public class DbQueue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Action { get; set; }

        public string ActionParam { get; set; }

        public bool? Done { get; set; }

        public DbQueue(string action, string param)
        {
            Action = action;
            ActionParam = param;
            Done = false;
        }

        public DbQueue()
        {
            
        }
    }
}
