using System.Runtime.Serialization;

namespace CameraControl.Service.Exceptions
{
    [DataContract(Namespace = "urn:CameraControl.Service.Exceptions")]
    public class CustomFaultException
    {
        [DataMember]
        public string Message { get; set; }
    }
}
