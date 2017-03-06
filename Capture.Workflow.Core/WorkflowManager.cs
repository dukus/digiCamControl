using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Capture.Workflow.Core.Classes;

namespace Capture.Workflow.Core
{
    public class WorkflowManager
    {
        public List<Type> Plugins { get; set; }

        public WorkflowManager()
        {
            Plugins = new List<Type>();
        }

        public List<Type> Get<T>()
        {
            List<Type> res=new List<Type>();
            foreach (Type type in Plugins)
            {
                if(type is T)
                    res.Add(type);
            }
            return res;
        }


        public void Save(WorkFlow workflow, string file)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(WorkFlow));
            // Create a FileStream to write with.

            Stream writer = new FileStream(file, FileMode.Create);
            // Serialize the object, and close the TextWriter
            serializer.Serialize(writer, this);
            writer.Close();
        }

        public WorkFlow Load(string fileName)
        {
            if (File.Exists(fileName))
            {
                XmlSerializer mySerializer =
                    new XmlSerializer(typeof(WorkFlow));
                FileStream myFileStream = new FileStream(fileName, FileMode.Open);
                WorkFlow flow = (WorkFlow)mySerializer.Deserialize(myFileStream);
                myFileStream.Close();
                return flow;
            }
            return null;
        }

    }
}
