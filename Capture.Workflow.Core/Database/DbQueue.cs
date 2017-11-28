using SQLite;

namespace Capture.Workflow.Core.Database
{
    public class DbQueue
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string SourceFile { get; set; }

        public string Action { get; set; }

        public string ActionParam { get; set; }

        public string Error { get; set; }

        public bool? Done { get; set; }

        public DbQueue(string sourceFile,string action, string param)
        {
            SourceFile = sourceFile;
            Action = action;
            ActionParam = param;
            Done = false;
            Error = "";
        }

        public DbQueue()
        {
            
        }
    }
}
