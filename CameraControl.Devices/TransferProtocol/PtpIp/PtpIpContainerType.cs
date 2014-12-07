namespace CameraControl.Devices.TransferProtocol.PtpIp
{
    public enum PtpIpContainerType
    {
        Init_Command_Request = 1,
        Init_Command_Ack = 2,
        Init_Event_Request = 3,
        Init_Event_Ack = 4,
        Init_Fail = 5,
        Cmd_Request = 6,
        Cmd_Response = 7,
        Event = 8,
        Start_Data_Packet = 9,
        Data_Packet = 10,
        Cancel_Transaction = 11,
        End_Data_Packet = 12,
        Ping = 13,
        Pong = 14
    }
}