using CameraControl.Core.Classes;
using CameraControl.Devices;

namespace CameraControl.Core.Interfaces
{
    public interface IFilenameTemplate
    {
        string Pharse(string template, PhotoSession session, ICameraDevice device, string fileName);
    }
}