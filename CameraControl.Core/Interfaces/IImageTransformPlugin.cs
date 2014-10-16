
using System.Windows.Controls;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IImageTransformPlugin
    {
        string Name { get; }
        bool Execute(string source, string dest, ValuePairEnumerator configData);
        UserControl GetConfig(ValuePairEnumerator configData);
    }
}