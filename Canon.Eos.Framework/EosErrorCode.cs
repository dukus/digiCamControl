
namespace Canon.Eos.Framework
{
    public enum EosErrorCode : long
    {
        Ok = 0x00000000,
        Unimplemented = 0x00000001,
        InternalError = 0x00000002,
        MemAllocFailed = 0x00000003,
        MemFreeFailed = 0x00000004,
        OperationCancelled = 0x00000005,
        IncompatibleVersion = 0x00000006,
        NotSupported = 0x00000007,
        UnexpectedException = 0x00000008,
        ProtectionViolation = 0x00000009,
        MissingSubcomponent = 0x0000000A,
        SelectionUnavailable = 0x0000000B,

        /* File errors */
        FileIoError = 0x00000020,
        FileTooManyOpen = 0x00000021,
        FileNotFound = 0x00000022,
        FileOpenError = 0x00000023,
        FileCloseError = 0x00000024,
        FileSeekError = 0x00000025,
        FileTellError = 0x00000026,
        FileReadError = 0x00000027,
        FileWriteError = 0x00000028,
        FilePermissionError = 0x00000029,
        FileDiskFullError = 0x0000002A,
        FileAlreadyExists = 0x0000002B,
        FileFormatUnrecognized = 0x0000002C,
        FileDataCorrupt = 0x0000002D,
        FileNamingNa = 0x0000002E,

        /* Directory errors */
        DirNotFound = 0x00000040,
        DirIoError = 0x00000041,
        DirEntryNotFound = 0x00000042,
        DirEntryExists = 0x00000043,
        DirNotEmpty = 0x00000044,

        /* Property errors */
        PropertiesUnavailable = 0x00000050,
        PropertiesMismatch = 0x00000051,
        PropertiesNotLoaded = 0x00000053,

        /* Function Parameter errors */
        InvalidParameter = 0x00000060,
        InvalidHandle = 0x00000061,
        InvalidPointer = 0x00000062,
        InvalidIndex = 0x00000063,
        InvalidLength = 0x00000064,
        InvalidFunctionPointer = 0x00000065,
        InvalidSortFunction = 0x00000066,

        /* Device errors */
        DeviceNotFound = 0x00000080,
        DeviceBusy = 0x00000081,
        DeviceInvalid = 0x00000082,
        DeviceEmergency = 0x00000083,
        DeviceMemoryFull = 0x00000084,
        DeviceInternalError = 0x00000085,
        DeviceInvalidParameter = 0x00000086,
        DeviceNoDisk = 0x00000087,
        DeviceDiskError = 0x00000088,
        DeviceCfGateChanged = 0x00000089,
        DeviceDialChanged = 0x0000008A,
        DeviceNotInstalled = 0x0000008B,
        DeviceStayAwake = 0x0000008C,
        DeviceNotReleased = 0x0000008D,

        /* Stream errors */
        StreamIoError = 0x000000A0,
        StreamNotOpen = 0x000000A1,
        StreamAlreadyOpen = 0x000000A2,
        StreamOpenError = 0x000000A3,
        StreamCloseError = 0x000000A4,
        StreamSeekError = 0x000000A5,
        StreamTellError = 0x000000A6,
        StreamReadError = 0x000000A7,
        StreamWriteError = 0x000000A8,
        StreamPermissionError = 0x000000A9,
        StreamCouldntBeginThread = 0x000000AA,
        StreamBadOptions = 0x000000AB,
        StreamEndOfStream = 0x000000AC,

        /* Communications errors */
        CommPortIsInUse = 0x000000C0,
        CommDisconnected = 0x000000C1,
        CommDeviceIncompatible = 0x000000C2,
        CommBufferFull = 0x000000C3,
        CommUsbBusError = 0x000000C4,

        /* Lock/Unlock */
        UsbDeviceLockError = 0x000000D0,
        UsbDeviceUnlockError = 0x000000D1,

        /* STI/WIA */
        StiUnknownError = 0x000000E0,
        StiInternalError = 0x000000E1,
        StiDeviceCreateError = 0x000000E2,
        StiDeviceReleaseError = 0x000000E3,
        DeviceNotLaunched = 0x000000E4,

        EnumNa = 0x000000F0,
        InvalidFunctionCall = 0x000000F1,
        HandleNotFound = 0x000000F2,
        InvalidId = 0x000000F3,
        WaitTimeoutError = 0x000000F4,

        /* PTP */
        SessionNotOpen = 0x00002003,
        InvalidTransactionid = 0x00002004,
        IncompleteTransfer = 0x00002007,
        InvalidStrageid = 0x00002008,
        DevicepropNotSupported = 0x0000200A,
        InvalidObjectformatcode = 0x0000200B,
        SelfTestFailed = 0x00002011,
        PartialDeletion = 0x00002012,
        SpecificationByFormatUnsupported = 0x00002014,
        NoValidObjectinfo = 0x00002015,
        InvalidCodeFormat = 0x00002016,
        UnknownVenderCode = 0x00002017,
        CaptureAlreadyTerminated = 0x00002018,
        InvalidParentobject = 0x0000201A,
        InvalidDevicepropFormat = 0x0000201B,
        InvalidDevicepropValue = 0x0000201C,
        SessionAlreadyOpen = 0x0000201E,
        TransactionCancelled = 0x0000201F,
        SpecificationOfDestinationUnsupported = 0x00002020,
        UnknownCommand = 0x0000A001,
        OperationRefused = 0x0000A005,
        LensCoverClose = 0x0000A006,
        LowBattery = 0x0000A101,
        ObjectNotReady = 0x0000A102,

        /* Capture Error */
        TakePictureAutoFocusFailed = 0x00008D01,
        TakePictureReserved = 0x00008D02,
        TakePictureMirrorUp = 0x00008D03,
        TakePictureSensorCleaning = 0x00008D04,
        TakePictureSilence = 0x00008D05,
        TakePictureNoCard = 0x00008D06,
        TakePictureCard = 0x00008D07,
        TakePictureCardProtect = 0x00008D08,


        LastGenericErrorPlusOne = 0x000000F5,        
    }
}
