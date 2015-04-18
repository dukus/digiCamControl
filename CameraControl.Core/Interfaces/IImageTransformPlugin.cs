
using System.Windows.Controls;
using CameraControl.Core.Classes;

namespace CameraControl.Core.Interfaces
{
    public interface IImageTransformPlugin
    {
        string Name { get; }
        string Execute(FileItem item,string infile, string dest, ValuePairEnumerator configData);
        UserControl GetConfig(ValuePairEnumerator configData);
    }
}