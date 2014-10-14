using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IAutoExportPlugin
    {
        bool Execute(string filename, ValuePairEnumerator configData);
        string Name { get;}
    }
}