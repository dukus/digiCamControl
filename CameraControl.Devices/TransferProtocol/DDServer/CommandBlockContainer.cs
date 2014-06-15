using CameraControl.Devices.TransferProtocol.DDServer;

namespace ddserverTest
{
    public class CommandBlockContainer : ParameterContainer
    {
        public CommandBlockContainer(int commandCode, params uint[] parameters)
            : base(parameters)
        {
            Header.Code = commandCode;
            Header.ContainerType = ContainerType.CommandBlock;
        }
    }
}