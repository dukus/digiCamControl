using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IImageTransformPlugin
    {
        string Name { get; set; }
        bool Execute(string source, string dest, ValuePairEnumerator configData);
    }
}