using System;
using System.Runtime.InteropServices;

namespace Canon.Eos.Framework.Internal.SDK
{

    public partial class Edsdk
    {


        #region Callback Functions

        public delegate uint EdsProgressCallback( uint inPercent, IntPtr inContext, ref bool outCancel);
        public delegate uint EdsCameraAddedHandler(IntPtr inContext);
        public delegate uint EdsPropertyEventHandler(uint inEvent, uint inPropertyID, uint inParam, IntPtr inContext); 
        public delegate uint EdsObjectEventHandler( uint inEvent, IntPtr inRef, IntPtr inContext); 
        public delegate uint EdsStateEventHandler( uint inEvent, uint inParameter, IntPtr inContext);

        #endregion

        #region Data Types

        public enum EdsDataType : uint
        {       
            Unknown     = 0,      
            Bool        = 1,     
            String      = 2,     
            Int8        = 3,     
            UInt8       = 6,    
            Int16       = 4,      
            UInt16      = 7,      
            Int32       = 8,      
            UInt32      = 9,      
            Int64       = 10,     
            UInt64      = 11,    
            Float       = 12,
            Double      = 13,
            ByteBlock   = 14,    
            Rational    = 20,    
            Point       = 21,   
            Rect        = 22,
            Time        = 23,

            Bool_Array   = 30,     
            Int8_Array   = 31,     
            Int16_Array  = 32,     
            Int32_Array  = 33,     
            UInt8_Array  = 34,     
            UInt16_Array     = 35,     
            UInt32_Array     = 36,     
            Rational_Array  = 37,      

            FocusInfo        = 101,    
            PictureStyleDesc,
        } 

        #endregion


        #region Property IDs

        /*----------------------------------
         Camera Setting Properties
        ----------------------------------*/
        public const uint   PropID_Unknown              = 0x0000ffff;     
    
        public const uint   PropID_ProductName          = 0x00000002;   
        public const uint   PropID_BodyIDEx             = 0x00000015;   
        public const uint   PropID_OwnerName            = 0x00000004;       
        public const uint   PropID_MakerName            = 0x00000005;       
        public const uint   PropID_DateTime             = 0x00000006;   
        public const uint   PropID_FirmwareVersion      = 0x00000007;   
        public const uint   PropID_BatteryLevel         = 0x00000008;   
        public const uint   PropID_CFn                  = 0x00000009;       
        public const uint   PropID_SaveTo               = 0x0000000b;
        public const uint   kEdsPropID_CurrentStorage   = 0x0000000c;
        public const uint   kEdsPropID_CurrentFolder    = 0x0000000d;
        public const uint   kEdsPropID_MyMenu           = 0x0000000e;

		public const uint	PropID_BatteryQuality       = 0x00000010;
        public const uint   PropID_UTCTime              = 16777238;

        public const uint	PropID_HDDirectoryStructure   = 0x00000020;

        /*----------------------------------
         Image Properties
        ----------------------------------*/
        public const uint   PropID_ImageQuality         = 0x00000100;   
        public const uint   PropID_JpegQuality          = 0x00000101;    
        public const uint   PropID_Orientation          = 0x00000102;   
        public const uint   PropID_ICCProfile           = 0x00000103;   
        public const uint   PropID_FocusInfo            = 0x00000104;   
        public const uint   PropID_DigitalExposure      = 0x00000105;   
        public const uint   PropID_WhiteBalance         = 0x00000106;   
        public const uint   PropID_ColorTemperature     = 0x00000107;   
        public const uint   PropID_WhiteBalanceShift    = 0x00000108;
        public const uint   PropID_Contrast             = 0x00000109;
        public const uint   PropID_ColorSaturation      = 0x0000010a;
        public const uint   PropID_ColorTone            = 0x0000010b;
        public const uint   PropID_Sharpness            = 0x0000010c;
        public const uint   PropID_ColorSpace           = 0x0000010d;
        public const uint   PropID_ToneCurve            = 0x0000010e;
        public const uint   PropID_PhotoEffect          = 0x0000010f;
        public const uint   PropID_FilterEffect         = 0x00000110;
        public const uint   PropID_ToningEffect         = 0x00000111;
        public const uint   PropID_ParameterSet         = 0x00000112;   
        public const uint   PropID_ColorMatrix          = 0x00000113;
        public const uint   PropID_PictureStyle         = 0x00000114;
        public const uint   PropID_PictureStyleDesc     = 0x00000115;
        public const uint   PropID_PictureStyleCaption  = 0x00000200;

        /*----------------------------------
         Image Processing Properties
        ----------------------------------*/
        public const uint   PropID_Linear               = 0x00000300;
        public const uint   PropID_ClickWBPoint         = 0x00000301;
        public const uint   PropID_WBCoeffs             = 0x00000302;

        /*----------------------------------
         Property Mask
        ----------------------------------*/
        public const uint   PropID_AtCapture_Flag       = 0x80000000;   

        /*----------------------------------
         Capture Properties
        ----------------------------------*/
        public const uint   PropID_AEMode               = 0x00000400;
		public const uint   PropID_AEModeSelect         = 0x00000436;
        public const uint   PropID_DriveMode            = 0x00000401;
        public const uint   PropID_ISOSpeed             = 0x00000402;
        public const uint   PropID_MeteringMode         = 0x00000403;
        public const uint   PropID_AFMode               = 0x00000404;
        public const uint   PropID_Av                   = 0x00000405;
        public const uint   PropID_Tv                   = 0x00000406;
        public const uint   PropID_ExposureCompensation = 0x00000407;
        public const uint   PropID_FlashCompensation    = 0x00000408;
        public const uint   PropID_FocalLength          = 0x00000409;
        public const uint   PropID_AvailableShots       = 0x0000040a;
        public const uint   PropID_Bracket              = 0x0000040b;
        public const uint   PropID_WhiteBalanceBracket  = 0x0000040c;
        public const uint   PropID_LensName             = 0x0000040d;
        public const uint   PropID_AEBracket            = 0x0000040e;
        public const uint   PropID_AEBracketType        = 16778300;
        public const uint   PropID_FEBracket            = 0x0000040f;
        public const uint   PropID_ISOBracket           = 0x00000410;
        public const uint   PropID_NoiseReduction       = 0x00000411;
        public const uint   PropID_FlashOn              = 0x00000412;
        public const uint   PropID_RedEye               = 0x00000413;
        public const uint   PropID_FlashMode            = 0x00000414;
        public const uint   PropID_LensStatus           = 0x00000416;
        
        public const uint   PropID_Artist	            = 0x00000418;
        public const uint   PropID_Copyright	        = 0x00000419;
        public const uint   PropID_DepthOfField	        = 0x0000041b;
        public const uint   PropID_EFCompensation       = 0x0000041e;

		/*----------------------------------
		 EVF Properties
		----------------------------------*/
        public const uint   PropID_Evf_OutputDevice        = 0x00000500;
        public const uint   PropID_Evf_Mode                = 0x00000501;
        public const uint   PropID_Evf_WhiteBalance        = 0x00000502;
        public const uint   PropID_Evf_ColorTemperature    = 0x00000503;
        public const uint   PropID_Evf_DepthOfFieldPreview = 0x00000504;

		// EVF IMAGE DATA Properties
        public const uint   PropID_Evf_Zoom                = 0x00000507;
        public const uint   PropID_Evf_ZoomPosition        = 0x00000508;
        public const uint   PropID_Evf_FocusAid            = 0x00000509;
        public const uint   PropID_Evf_Histogram           = 0x0000050A;
        public const uint   PropID_Evf_ImagePosition       = 0x0000050B;
        public const uint PropID_Evf_ZoomRect = 0x00000541;
		public const uint   PropID_Evf_HistogramStatus     = 0x0000050C;
        public const uint   PropID_Evf_AFMode              = 0x0000050E;
        public const uint PropID_Evf_CoordinateSystem = 0x00000540;
		public const uint   PropID_Record                  = 0x00000510;
             
        /*----------------------------------
         Image GPS Properties
        ----------------------------------*/
        public const uint   PropID_GPSVersionID			 =  0x00000800; 	
        public const uint   PropID_GPSLatitudeRef		 =  0x00000801; 		
        public const uint   PropID_GPSLatitude			 =  0x00000802; 	
        public const uint   PropID_GPSLongitudeRef		 =  0x00000803; 	
        public const uint   PropID_GPSLongitude			 =  0x00000804; 		
		public const uint   PropID_GPSAltitudeRef		 =  0x00000805; 		
        public const uint   PropID_GPSAltitude			 =  0x00000806; 		
        public const uint   PropID_GPSTimeStamp			 =  0x00000807; 		
        public const uint   PropID_GPSSatellites		 =  0x00000808; 		
        public const uint   PropID_GPSStatus			 =  0x00000809;
        public const uint   PropID_GPSMapDatum			 =  0x00000812; 		
		public const uint   PropID_GPSDateStamp			 =  0x0000081D; 		
		#endregion

        #region Camera commands

        /*-----------------------------------------------------------------------------
         Send Commands
        -----------------------------------------------------------------------------*/
        public const uint CameraCommand_TakePicture         = 0x00000000;       
        public const uint CameraCommand_ExtendShutDownTimer = 0x00000001;
        public const uint CameraCommand_BulbStart			= 0x00000002; 
        public const uint CameraCommand_BulbEnd				= 0x00000003; 
        public const uint CameraCommand_DoEvfAf             = 0x00000102;
        public const uint CameraCommand_DriveLensEvf        = 0x00000103;
        public const uint CameraCommand_DoClickWBEvf        = 0x00000104;

        public const uint CameraCommand_PressShutterButton  = 0x00000004;
        public const uint CameraCommand_MovieSelectSwON     = 0x00000107;
        public const uint CameraCommand_MovieSelectSwOFF    = 0x00000108;
        
        public enum EdsEvfAf : uint
        {
	        CameraCommand_EvfAf_OFF		= 0,
	        CameraCommand_EvfAf_ON		= 1,
        }

        public enum EdsShutterButton : uint
        {
	        CameraCommand_ShutterButton_OFF					= 0x00000000,
	        CameraCommand_ShutterButton_Halfway				= 0x00000001,
	        CameraCommand_ShutterButton_Completely			= 0x00000003,
	        CameraCommand_ShutterButton_Halfway_NonAF		= 0x00010001,
	        CameraCommand_ShutterButton_Completely_NonAF	= 0x00010003,
        } 
        #endregion

        #region Camera status command

        /*----------------------------------
         Camera Status Commands
        ----------------------------------*/
        public const uint CameraState_UILock                = 0x00000000;           
        public const uint CameraState_UIUnLock              = 0x00000001;       
        public const uint CameraState_EnterDirectTransfer   = 0x00000002;       
        public const uint CameraState_ExitDirectTransfer    = 0x00000003;   

         #endregion


        #region  Enumeration of property value  

        /*-----------------------------------------------------------------------------
         Stream Seek Origins
        -----------------------------------------------------------------------------*/
        public enum EdsSeekOrigin : uint
        {
            Cur = 0 ,    
            Begin    ,  
            End  ,   
        }

        /*-----------------------------------------------------------------------------
         File and Propaties Access
        -----------------------------------------------------------------------------*/
        public enum EdsAccess : uint
        {
            Read = 0  ,  
            Write    ,   
            ReadWrite ,  
            Error = 0xFFFFFFFF,
        }

        /*-----------------------------------------------------------------------------
         File Create Disposition
        -----------------------------------------------------------------------------*/
        public enum EdsFileCreateDisposition : uint
        {
            CreateNew = 0    ,  
            CreateAlways     ,   
            OpenExisting     ,   
            OpenAlways   ,  
            TruncateExsisting, 
        } 


        /*-----------------------------------------------------------------------------
         Target Image Types
        -----------------------------------------------------------------------------*/
        public enum EdsTargetImageType : uint
        {
            Unknown = 0x00000000,
            Jpeg    = 0x00000001,
            TIFF    = 0x00000007,
            TIFF16  = 0x00000008,
            RGB     = 0x00000009,
            RGB16   = 0x0000000A,
        }


        /*-----------------------------------------------------------------------------
         Image Source 
        -----------------------------------------------------------------------------*/
        public enum EdsImageSource : uint
        {
            FullView = 0   ,
            Thumbnail,
            Preview,
        }

        /*-----------------------------------------------------------------------------
         Progress Option
        -----------------------------------------------------------------------------*/
        public enum EdsProgressOption : uint
        {
            NoReport = 0   ,    
            Done          ,     
            Periodically   ,    
        } 


        /*-----------------------------------------------------------------------------
         file attribute
        -----------------------------------------------------------------------------*/
        public enum EdsFileAttribute : uint
        {
            Normal     = 0x00000000,
            ReadOnly   = 0x00000001,
            Hidden     = 0x00000002,
            System     = 0x00000004,
            Archive    = 0x00000020,
        }

        /*-----------------------------------------------------------------------------
         Save To
        -----------------------------------------------------------------------------*/
        public enum EdsSaveTo : uint 
        {
            Camera  =  1,
            Host    =  2,
            Both    =  3,
        }

        /*-----------------------------------------------------------------------------
         StorageTypes
        -----------------------------------------------------------------------------*/
        public enum EdsStorageType : uint 
        {
            Non = 0,
            CF  = 1,
            SD  = 2,
        } 

        /*-----------------------------------------------------------------------------
         Transfer Option
        -----------------------------------------------------------------------------*/
        public enum EdsTransferOption : uint
        {
            ByDirectTransfer    = 1,
            ByRelease           = 2,
            ToDesktop           = 0x00000100,
        }

		/*-----------------------------------------------------------------------------
		 Drive Lens
		-----------------------------------------------------------------------------*/
        public const uint EvfDriveLens_Near1	= 0x00000001;
        public const uint EvfDriveLens_Near2	= 0x00000002;
        public const uint EvfDriveLens_Near3	= 0x00000003;
        public const uint EvfDriveLens_Far1		= 0x00008001;
        public const uint EvfDriveLens_Far2		= 0x00008002;
        public const uint EvfDriveLens_Far3		= 0x00008003;

		/*-----------------------------------------------------------------------------
		 Depth of Field Preview
		-----------------------------------------------------------------------------*/
        public const uint EvfDepthOfFieldPreview_OFF	= 0x00000000;
		public const uint EvfDepthOfFieldPreview_ON		= 0x00000001;


        /*-----------------------------------------------------------------------------
         Image Format 
        -----------------------------------------------------------------------------*/

        public const int ImageFormat_Unknown = 0x00000000;
        public const int ImageFormat_Jpeg = 0x00000001;
        public const int ImageFormat_CRW = 0x00000002;
        public const int ImageFormat_RAW = 0x00000004;

        public const int ImageFormat_CR2 = 0x00000006;


        public const int ImageSize_Large = 0;
        public const int ImageSize_Middle = 1;
        public const int ImageSize_Small = 2;
        public const int ImageSize_Middle1 = 5;
        public const int ImageSize_Middle2 = 6;
        public const int ImageSize_Unknown = -1;



        public const int CompressQuality_Normal = 2;
		public const int CompressQuality_Fine = 3;
		public const int CompressQuality_Lossless = 4;
		public const int CompressQuality_SuperFine = 5;
		public const int CompressQuality_Unknown = -1;


        /*-----------------------------------------------------------------------------
         Battery level
        -----------------------------------------------------------------------------*/
        public const uint   BatteryLevel_Empty    = 1;   
        public const uint   BatteryLevel_Low      = 30;      
        public const uint   BatteryLevel_Half     = 50;      
        public const uint   BatteryLevel_Normal   = 80;
        public const uint   BatteryLevel_AC       = 0xFFFFFFFF;     

        /*-----------------------------------------------------------------------------
         White Balance
        -----------------------------------------------------------------------------*/
        public const int WhiteBalance_Click         = -1;
        public const int WhiteBalance_Auto          = 0;
        public const int WhiteBalance_Daylight      = 1;
        public const int WhiteBalance_Cloudy        = 2;
        public const int WhiteBalance_Tangsten      = 3;
        public const int WhiteBalance_Fluorescent   = 4;
        public const int WhiteBalance_Strobe        = 5;
        public const int WhiteBalance_WhitePaper    = 6;
        public const int WhiteBalance_Shade         = 8;
        public const int WhiteBalance_ColorTemp     = 9;
        public const int WhiteBalance_PCSet1        = 10;
        public const int WhiteBalance_PCSet2        = 11;
        public const int WhiteBalance_PCSet3        = 12;   

        /*-----------------------------------------------------------------------------
         Photo Effects
        -----------------------------------------------------------------------------*/
        public const int PhotoEffect_Off         = 0;
        public const int PhotoEffect_Monochrome  = 5;

        /*-----------------------------------------------------------------------------
         Color Matrix
        -----------------------------------------------------------------------------*/
        public const int ColorMatrix_Custom      = 0;
        public const int ColorMatrix_1           = 1;
        public const int ColorMatrix_2           = 2;
        public const int ColorMatrix_3           = 3;
        public const int ColorMatrix_4           = 4;
        public const int ColorMatrix_5           = 5;
        public const int ColorMatrix_6           = 6;
        public const int ColorMatrix_7           = 7;

        /*-----------------------------------------------------------------------------
         Filter Effects
        -----------------------------------------------------------------------------*/
        public const int FilterEffect_None       = 0;
        public const int FilterEffect_Yellow     = 1;
        public const int FilterEffect_Orange     = 2;
        public const int FilterEffect_Red        = 3;
        public const int FilterEffect_Green      = 4;

        /*-----------------------------------------------------------------------------
         Toning Effects
        -----------------------------------------------------------------------------*/
        public const int TonigEffect_None       = 0;
        public const int TonigEffect_Sepia      = 1;
        public const int TonigEffect_Blue       = 2;
        public const int TonigEffect_Purple     = 3;
        public const int TonigEffect_Green      = 4;

        /*-----------------------------------------------------------------------------
         Color Space
        -----------------------------------------------------------------------------*/
        public const uint   ColorSpace_sRGB       = 1;
        public const uint   ColorSpace_AdobeRGB   = 2;
        public const uint   ColorSpace_Unknown    = 0xffffffff;

        /*-----------------------------------------------------------------------------
         PictureStyle
        -----------------------------------------------------------------------------*/
        public const uint PictureStyle_Standard     = 0x0081;
        public const uint PictureStyle_Portrait     = 0x0082;
        public const uint PictureStyle_Landscape    = 0x0083;
        public const uint PictureStyle_Neutral      = 0x0084;
        public const uint PictureStyle_Faithful     = 0x0085;
        public const uint PictureStyle_Monochrome   = 0x0086;
        public const uint PictureStyle_Auto         = 0x0087;
        public const uint PictureStyle_FineDetail   = 0x0088;
        public const uint PictureStyle_User1        = 0x0021;
        public const uint PictureStyle_User2        = 0x0022;
        public const uint PictureStyle_User3        = 0x0023;
        public const uint PictureStyle_PC1          = 0x0041;
        public const uint PictureStyle_PC2          = 0x0042;
        public const uint PictureStyle_PC3          = 0x0043;


        /*-----------------------------------------------------------------------------
         AE Mode
        -----------------------------------------------------------------------------*/
        public const uint   AEMode_Program          = 0;
        public const uint   AEMode_Tv               = 1;
        public const uint   AEMode_Av               = 2;
        public const uint   AEMode_Mamual           = 3;
        public const uint   AEMode_Bulb             = 4;
        public const uint   AEMode_A_DEP            = 5;
        public const uint   AEMode_DEP              = 6;
        public const uint   AEMode_Custom           = 7;
        public const uint   AEMode_Lock             = 8;
        public const uint   AEMode_Green            = 9;
        public const uint   AEMode_NigntPortrait    = 10;
        public const uint   AEMode_Sports           = 11;
        public const uint   AEMode_Portrait         = 12;
        public const uint   AEMode_Landscape        = 13;
        public const uint   AEMode_Closeup          = 14;
        public const uint   AEMode_FlashOff         = 15;
        public const uint   AEMode_CreativeAuto     = 19;
        public const uint   AEMode_Movie			= 20;
        public const uint   AEMode_PhotoInMovie		= 21;
		public const uint   AEMode_SceneIntelligentAuto = 22;
		public const uint   AEMode_SCN              = 25;
        public const uint   AEMode_Unknown          = 0xffffffff;

        /*-----------------------------------------------------------------------------
         Bracket
        -----------------------------------------------------------------------------*/
        public const uint   Bracket_AEB             = 0x01;
        public const uint   Bracket_ISOB            = 0x02;
        public const uint   Bracket_WBB             = 0x04;
        public const uint   Bracket_FEB             = 0x08;
        public const uint   Bracket_Unknown         = 0xffffffff;


		/*-----------------------------------------------------------------------------
		 EVF Output Device [Flag]
		-----------------------------------------------------------------------------*/
        public const uint   EvfOutputDevice_TFT			= 1;
        public const uint   EvfOutputDevice_PC			= 2;


		/*-----------------------------------------------------------------------------
		 EVF Zoom
		-----------------------------------------------------------------------------*/
        public const int   EvfZoom_Fit			= 1;
        public const int   EvfZoom_x5			= 5;
		public const int	EvfZoom_x10			= 10;

        public enum EdsEvfAFMode : uint
        {
            Evf_AFMode_Quick    = 0,
            Evf_AFMode_Live     = 1,
            Evf_AFMode_LiveFace = 2,
			Evf_AFMode_LiveMulti = 3,
        }

        /*-----------------------------------------------------------------------------
         Strobo Mode
        -----------------------------------------------------------------------------*/
        public enum EdsStroboMode
        {
            kEdsStroboModeInternal = 0,
            kEdsStroboModeExternalETTL = 1,
            kEdsStroboModeExternalATTL = 2,
            kEdsStroboModeExternalTTL = 3,
            kEdsStroboModeExternalAuto = 4,
            kEdsStroboModeExternalManual = 5,
            kEdsStroboModeManual = 6,
        }

        public enum EdsETTL2Mode
        {
	        kEdsETTL2ModeEvaluative		= 0,
	        kEdsETTL2ModeAverage		= 1,
        }

        public enum ImageQuality : uint
        {
			/* Jpeg Only */
			EdsImageQuality_LJ = 0x0010ff0f,	/* Jpeg Large */
			EdsImageQuality_M1J = 0x0510ff0f,	/* Jpeg Middle1 */
			EdsImageQuality_M2J = 0x0610ff0f,	/* Jpeg Middle2 */
			EdsImageQuality_SJ = 0x0210ff0f,	/* Jpeg Small */
			EdsImageQuality_LJF = 0x0013ff0f,	/* Jpeg Large Fine */
			EdsImageQuality_LJN = 0x0012ff0f,	/* Jpeg Large Normal */
			EdsImageQuality_MJF = 0x0113ff0f,	/* Jpeg Middle Fine */
			EdsImageQuality_MJN = 0x0112ff0f,	/* Jpeg Middle Normal */
			EdsImageQuality_SJF = 0x0213ff0f,	/* Jpeg Small Fine */
			EdsImageQuality_SJN = 0x0212ff0f,	/* Jpeg Small Normal */
			EdsImageQuality_S1JF = 0x0E13ff0f,	/* Jpeg Small1 Fine */
			EdsImageQuality_S1JN = 0x0E12ff0f,	/* Jpeg Small1 Normal */
			EdsImageQuality_S2JF = 0x0F13ff0f,	/* Jpeg Small2 */
			EdsImageQuality_S3JF = 0x1013ff0f,	/* Jpeg Small3 */

			/* RAW + Jpeg */
			EdsImageQuality_LR = 0x0064ff0f,	/* RAW */
			EdsImageQuality_LRLJF = 0x00640013,	/* RAW + Jpeg Large Fine */
			EdsImageQuality_LRLJN = 0x00640012,	/* RAW + Jpeg Large Normal */
			EdsImageQuality_LRMJF = 0x00640113,	/* RAW + Jpeg Middle Fine */
			EdsImageQuality_LRMJN = 0x00640112,	/* RAW + Jpeg Middle Normal */
			EdsImageQuality_LRSJF = 0x00640213,	/* RAW + Jpeg Small Fine */
			EdsImageQuality_LRSJN = 0x00640212,	/* RAW + Jpeg Small Normal */
			EdsImageQuality_LRS1JF = 0x00640E13,	/* RAW + Jpeg Small1 Fine */
			EdsImageQuality_LRS1JN = 0x00640E12,	/* RAW + Jpeg Small1 Normal */
			EdsImageQuality_LRS2JF = 0x00640F13,	/* RAW + Jpeg Small2 */
			EdsImageQuality_LRS3JF = 0x00641013,	/* RAW + Jpeg Small3 */

			EdsImageQuality_LRLJ = 0x00640010,	/* RAW + Jpeg Large */
			EdsImageQuality_LRM1J = 0x00640510,	/* RAW + Jpeg Middle1 */
			EdsImageQuality_LRM2J = 0x00640610,	/* RAW + Jpeg Middle2 */
			EdsImageQuality_LRSJ = 0x00640210,	/* RAW + Jpeg Small */

			/* MRAW(SRAW1) + Jpeg */
			EdsImageQuality_MR = 0x0164ff0f,	/* MRAW(SRAW1) */
			EdsImageQuality_MRLJF = 0x01640013,	/* MRAW(SRAW1) + Jpeg Large Fine */
			EdsImageQuality_MRLJN = 0x01640012,	/* MRAW(SRAW1) + Jpeg Large Normal */
			EdsImageQuality_MRMJF = 0x01640113,	/* MRAW(SRAW1) + Jpeg Middle Fine */
			EdsImageQuality_MRMJN = 0x01640112,	/* MRAW(SRAW1) + Jpeg Middle Normal */
			EdsImageQuality_MRSJF = 0x01640213,	/* MRAW(SRAW1) + Jpeg Small Fine */
			EdsImageQuality_MRSJN = 0x01640212,	/* MRAW(SRAW1) + Jpeg Small Normal */
			EdsImageQuality_MRS1JF = 0x01640E13,	/* MRAW(SRAW1) + Jpeg Small1 Fine */
			EdsImageQuality_MRS1JN = 0x01640E12,	/* MRAW(SRAW1) + Jpeg Small1 Normal */
			EdsImageQuality_MRS2JF = 0x01640F13,	/* MRAW(SRAW1) + Jpeg Small2 */
			EdsImageQuality_MRS3JF = 0x01641013,	/* MRAW(SRAW1) + Jpeg Small3 */

			EdsImageQuality_MRLJ = 0x01640010,	/* MRAW(SRAW1) + Jpeg Large */
			EdsImageQuality_MRM1J = 0x01640510,	/* MRAW(SRAW1) + Jpeg Middle1 */
			EdsImageQuality_MRM2J = 0x01640610,	/* MRAW(SRAW1) + Jpeg Middle2 */
			EdsImageQuality_MRSJ = 0x01640210,	/* MRAW(SRAW1) + Jpeg Small */

			/* SRAW(SRAW2) + Jpeg */
			EdsImageQuality_SR = 0x0264ff0f,	/* SRAW(SRAW2) */
			EdsImageQuality_SRLJF = 0x02640013,	/* SRAW(SRAW2) + Jpeg Large Fine */
			EdsImageQuality_SRLJN = 0x02640012,	/* SRAW(SRAW2) + Jpeg Large Normal */
			EdsImageQuality_SRMJF = 0x02640113,	/* SRAW(SRAW2) + Jpeg Middle Fine */
			EdsImageQuality_SRMJN = 0x02640112,	/* SRAW(SRAW2) + Jpeg Middle Normal */
			EdsImageQuality_SRSJF = 0x02640213,	/* SRAW(SRAW2) + Jpeg Small Fine */
			EdsImageQuality_SRSJN = 0x02640212,	/* SRAW(SRAW2) + Jpeg Small Normal */
			EdsImageQuality_SRS1JF = 0x02640E13,	/* SRAW(SRAW2) + Jpeg Small1 Fine */
			EdsImageQuality_SRS1JN = 0x02640E12,	/* SRAW(SRAW2) + Jpeg Small1 Normal */
			EdsImageQuality_SRS2JF = 0x02640F13,	/* SRAW(SRAW2) + Jpeg Small2 */
			EdsImageQuality_SRS3JF = 0x02641013,	/* SRAW(SRAW2) + Jpeg Small3 */

			EdsImageQuality_SRLJ = 0x02640010,	/* SRAW(SRAW2) + Jpeg Large */
			EdsImageQuality_SRM1J = 0x02640510,	/* SRAW(SRAW2) + Jpeg Middle1 */
			EdsImageQuality_SRM2J = 0x02640610,	/* SRAW(SRAW2) + Jpeg Middle2 */
			EdsImageQuality_SRSJ = 0x02640210,	/* SRAW(SRAW2) + Jpeg Small */

			EdsImageQuality_Unknown = 0xffffffff,
        }

        public enum ImageQualityForLegacy : uint
        {
			kEdsImageQualityForLegacy_LJ = 0x001f000f,	/* Jpeg Large */
			kEdsImageQualityForLegacy_M1J = 0x051f000f,	/* Jpeg Middle1 */
			kEdsImageQualityForLegacy_M2J = 0x061f000f,	/* Jpeg Middle2 */
			kEdsImageQualityForLegacy_SJ = 0x021f000f,	/* Jpeg Small */
			kEdsImageQualityForLegacy_LJF = 0x00130000,	/* Jpeg Large Fine */
			kEdsImageQualityForLegacy_LJN = 0x00120000,	/* Jpeg Large Normal */
			kEdsImageQualityForLegacy_MJF = 0x01130000,	/* Jpeg Middle Fine */
			kEdsImageQualityForLegacy_MJN = 0x01120000,	/* Jpeg Middle Normal */
			kEdsImageQualityForLegacy_SJF = 0x02130000,	/* Jpeg Small Fine */
			kEdsImageQualityForLegacy_SJN = 0x02120000,	/* Jpeg Small Normal */

			kEdsImageQualityForLegacy_LR = 0x00240000,	/* RAW */
			kEdsImageQualityForLegacy_LRLJF = 0x00240013,	/* RAW + Jpeg Large Fine */
			kEdsImageQualityForLegacy_LRLJN = 0x00240012,	/* RAW + Jpeg Large Normal */
			kEdsImageQualityForLegacy_LRMJF = 0x00240113,	/* RAW + Jpeg Middle Fine */
			kEdsImageQualityForLegacy_LRMJN = 0x00240112,	/* RAW + Jpeg Middle Normal */
			kEdsImageQualityForLegacy_LRSJF = 0x00240213,	/* RAW + Jpeg Small Fine */
			kEdsImageQualityForLegacy_LRSJN = 0x00240212,	/* RAW + Jpeg Small Normal */

			kEdsImageQualityForLegacy_LR2 = 0x002f000f,	/* RAW */
			kEdsImageQualityForLegacy_LR2LJ = 0x002f001f,	/* RAW + Jpeg Large */
			kEdsImageQualityForLegacy_LR2M1J = 0x002f051f,	/* RAW + Jpeg Middle1 */
			kEdsImageQualityForLegacy_LR2M2J = 0x002f061f,	/* RAW + Jpeg Middle2 */
			kEdsImageQualityForLegacy_LR2SJ = 0x002f021f,	/* RAW + Jpeg Small */

			kEdsImageQualityForLegacy_Unknown = 0xffffffff,
        }

        #endregion

        #region Event IDs

        /*-----------------------------------------------------------------------------
         Camera Events
        -----------------------------------------------------------------------------*/
        /*----------------------------------
         Property Event
        ----------------------------------*/
        /* Notifies all property events. */
        public const uint   PropertyEvent_All                   = 0x00000100;

        /* Notifies that a camera property value has been changed. 
         The changed property can be retrieved from event data. 
         The changed value can be retrieved by means of EdsGetPropertyData. 
         In the case of type 1 protocol standard cameras, 
         notification of changed properties can only be issued for custom functions (CFn). 
         If the property type is 0x0000FFFF, the changed property cannot be identified. 
         Thus, retrieve all required properties repeatedly. */
        public const uint   PropertyEvent_PropertyChanged       = 0x00000101;

        /* Notifies of changes in the list of camera properties with configurable values. 
         The list of configurable values for property IDs indicated in event data 
          can be retrieved by means of EdsGetPropertyDesc. 
         For type 1 protocol standard cameras, the property ID is identified as "Unknown"
          during notification. 
          Thus, you must retrieve a list of configurable values for all properties and
          retrieve the property values repeatedly. 
         (For details on properties for which you can retrieve a list of configurable
          properties, 
          see the description of EdsGetPropertyDesc). */
        public const uint   PropertyEvent_PropertyDescChanged    = 0x00000102;
                
        /*----------------------------------
         Object Event
        ----------------------------------*/
        /* Notifies all object events. */
        public const uint   ObjectEvent_All                     = 0x00000200;

        /* Notifies that the volume object (memory card) state (VolumeInfo)
          has been changed. 
         Changed objects are indicated by event data. 
         The changed value can be retrieved by means of EdsGetVolumeInfo. 
         Notification of this event is not issued for type 1 protocol standard cameras. */
        public const uint   ObjectEvent_VolumeInfoChanged       = 0x00000201;

        /* Notifies if the designated volume on a camera has been formatted.
         If notification of this event is received, get sub-items of the designated
          volume again as needed. 
         Changed volume objects can be retrieved from event data. 
         Objects cannot be identified on cameras earlier than the D30
          if files are added or deleted. 
         Thus, these events are subject to notification. */
        public const uint   ObjectEvent_VolumeUpdateItems       = 0x00000202;

        /* Notifies if many images are deleted in a designated folder on a camera.
         If notification of this event is received, get sub-items of the designated
          folder again as needed. 
         Changed folders (specifically, directory item objects) can be retrieved
          from event data. */
        public const uint   ObjectEvent_FolderUpdateItems       = 0x00000203;

        /* Notifies of the creation of objects such as new folders or files
          on a camera compact flash card or the like. 
         This event is generated if the camera has been set to store captured
          images simultaneously on the camera and a computer,
          for example, but not if the camera is set to store images
          on the computer alone. 
         Newly created objects are indicated by event data. 
         Because objects are not indicated for type 1 protocol standard cameras,
          (that is, objects are indicated as NULL),
         you must again retrieve child objects under the camera object to 
         identify the new objects. */
        public const uint   ObjectEvent_DirItemCreated          = 0x00000204;

        /* Notifies of the deletion of objects such as folders or files on a camera
          compact flash card or the like. 
         Deleted objects are indicated in event data. 
         Because objects are not indicated for type 1 protocol standard cameras, 
         you must again retrieve child objects under the camera object to
          identify deleted objects. */
        public const uint   ObjectEvent_DirItemRemoved          = 0x00000205;
            
        /* Notifies that information of DirItem objects has been changed. 
         Changed objects are indicated by event data. 
         The changed value can be retrieved by means of EdsGetDirectoryItemInfo. 
         Notification of this event is not issued for type 1 protocol standard cameras. */
        public const uint   ObjectEvent_DirItemInfoChanged      = 0x00000206;

        /* Notifies that header information has been updated, as for rotation information
          of image files on the camera. 
         If this event is received, get the file header information again, as needed. 
         This function is for type 2 protocol standard cameras only. */
        public const uint   ObjectEvent_DirItemContentChanged   = 0x00000207;

        /* Notifies that there are objects on a camera to be transferred to a computer. 
         This event is generated after remote release from a computer or local release
          from a camera. 
         If this event is received, objects indicated in the event data must be downloaded.
          Furthermore, if the application does not require the objects, instead
          of downloading them,
           execute EdsDownloadCancel and release resources held by the camera. 
         The order of downloading from type 1 protocol standard cameras must be the order
          in which the events are received. */
        public const uint   ObjectEvent_DirItemRequestTransfer  = 0x00000208;

        /* Notifies if the camera's direct transfer button is pressed. 
         If this event is received, objects indicated in the event data must be downloaded. 
         Furthermore, if the application does not require the objects, instead of
          downloading them, 
          execute EdsDownloadCancel and release resources held by the camera. 
         Notification of this event is not issued for type 1 protocol standard cameras. */
        public const uint   ObjectEvent_DirItemRequestTransferDT    = 0x00000209;

        /* Notifies of requests from a camera to cancel object transfer 
          if the button to cancel direct transfer is pressed on the camera. 
         If the parameter is 0, it means that cancellation of transfer is requested for
          objects still not downloaded,
          with these objects indicated by kEdsObjectEvent_DirItemRequestTransferDT. 
         Notification of this event is not issued for type 1 protocol standard cameras. */          
        public const uint   ObjectEvent_DirItemCancelTransferDT     = 0x0000020a;
            
        public const uint   ObjectEvent_VolumeAdded                 = 0x0000020c;
		public const uint	ObjectEvent_VolumeRemoved				= 0x0000020d;

        /*----------------------------------
         State Event
        ----------------------------------*/
        /* Notifies all state events. */
        public const uint   StateEvent_All                      = 0x00000300;

        /* Indicates that a camera is no longer connected to a computer, 
         whether it was disconnected by unplugging a cord, opening
          the compact flash compartment, 
          turning the camera off, auto shut-off, or by other means. */
        public const uint   StateEvent_Shutdown                 = 0x00000301;

        /* Notifies of whether or not there are objects waiting to
          be transferred to a host computer. 
         This is useful when ensuring all shot images have been transferred 
         when the application is closed. 
         Notification of this event is not issued for type 1 protocol 
         standard cameras. */
        public const uint   StateEvent_JobStatusChanged         = 0x00000302;

        /* Notifies that the camera will shut down after a specific period. 
         Generated only if auto shut-off is set. 
         Exactly when notification is issued (that is, the number of
          seconds until shutdown) varies depending on the camera model. 
         To continue operation without having the camera shut down,
         use EdsSendCommand to extend the auto shut-off timer.
         The time in seconds until the camera shuts down is returned
          as the initial value. */
        public const uint   StateEvent_WillSoonShutDown         = 0x00000303;
        
        /* As the counterpart event to kEdsStateEvent_WillSoonShutDown,
         this event notifies of updates to the number of seconds until
          a camera shuts down. 
         After the update, the period until shutdown is model-dependent. */
        public const uint   StateEvent_ShutDownTimerUpdate      = 0x00000304; 

        /* Notifies that a requested release has failed, due to focus
          failure or similar factors. */
        public const uint   StateEvent_CaptureError             = 0x00000305;

        /* Notifies of internal SDK errors. 
         If this error event is received, the issuing device will probably
          not be able to continue working properly,
          so cancel the remote connection. */
        public const uint   StateEvent_InternalError            = 0x00000306;

        public const uint   StateEvent_AfResult                  = 0x00000309;


		public const uint StateEvent_BulbExposureTime			= 0x00000310;	

        #endregion


        #region Proto type defenition of EDSDK API

        /*----------------------------------
         Basic functions
        ----------------------------------*/
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsInitializeSDK
        //
        //  Description:
        //      Initializes the libraries. 
        //      When using the EDSDK libraries, you must call this API once  
        //          before using EDSDK APIs.
        //
        //  Parameters:
        //       In:    None
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsInitializeSDK();

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsTerminateSDK
        //
        //  Description:
        //      Terminates use of the libraries. 
        //      This function muse be called when ending the SDK.
        //      Calling this function releases all resources allocated by the libraries.
        //
        //  Parameters:
        //       In:    None
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsTerminateSDK();


        /*-------------------------------------------
         Reference-counter operating functions
        --------------------------------------------*/
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsRetain
        //
        //  Description:
        //      Increments the reference counter of existing objects.
        //
        //  Parameters:
        //       In:    inRef - The reference for the item.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsRetain(  IntPtr inRef );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsRelease
        //
        //  Description:
        //      Decrements the reference counter to an object. 
        //      When the reference counter reaches 0, the object is released.
        //
        //  Parameters:
        //       In:    inRef - The reference of the item.
        //      Out:    None
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsRelease( IntPtr inRef );


        /*----------------------------------
         Item-tree operating functions
        ----------------------------------*/
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetChildCount
        //
        //  Description:
        //      Gets the number of child objects of the designated object.
        //      Example: Number of files in a directory
        //
        //  Parameters:
        //       In:    inRef - The reference of the list.
        //      Out:    outCount - Number of elements in this list.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetChildCount( IntPtr inRef, out int outCount );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetChildAtIndex
        //
        //  Description:
        //       Gets an indexed child object of the designated object. 
        //
        //  Parameters:
        //       In:    inRef - The reference of the item.
        //              inIndex -  The index that is passed in, is zero based.
        //      Out:    outRef - The pointer which receives reference of the 
        //                           specified index .
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/ 
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetChildAtIndex( IntPtr inRef, int inIndex, out IntPtr outRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetParent
        //
        //  Description:
        //      Gets the parent object of the designated object.
        //
        //  Parameters:
        //       In:    inRef        - The reference of the item.
        //      Out:    outParentRef - The pointer which receives reference.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetParent( IntPtr inRef, out IntPtr outParentRef);
    
    
        /*----------------------------------
          Property operating functions
        ----------------------------------*/    
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetPropertySize
        //
        //  Description:
        //      Gets the byte size and data type of a designated property 
        //          from a camera object or image object.
        //
        //  Parameters:
        //       In:    inRef - The reference of the item.
        //              inPropertyID - The ProprtyID
        //              inParam - Additional information of property.
        //                   We use this parameter in order to specify an index
        //                   in case there are two or more values over the same ID.
        //      Out:    outDataType - Pointer to the buffer that is to receive the property
        //                        type data.
        //              outSize - Pointer to the buffer that is to receive the property
        //                        size.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/ 
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetPropertySize(IntPtr inRef, uint inPropertyID, int inParam,
             out EdsDataType outDataType, out int outSize);
        
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetPropertyData
        //
        //  Description:
        //      Gets property information from the object designated in inRef.
        //
        //  Parameters:
        //       In:    inRef - The reference of the item.
        //              inPropertyID - The ProprtyID
        //              inParam - Additional information of property.
        //                   We use this parameter in order to specify an index
        //                   in case there are two or more values over the same ID.
        //              inPropertySize - The number of bytes of the prepared buffer
        //                  for receive property-value.
        //       Out:   outPropertyData - The buffer pointer to receive property-value.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/      
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam,
             int inPropertySize, IntPtr outPropertyData);

        #region GetPorpertyData Wrapper

        public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out uint outPropertyData)
        {
            int size = Marshal.SizeOf(typeof(uint));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);

            outPropertyData = (uint)Marshal.PtrToStructure(ptr, typeof(uint));
            Marshal.FreeHGlobal(ptr);
            return err;
        }

        public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam,
             out Edsdk.EdsTime outPropertyData)
        {
            int size = Marshal.SizeOf(typeof(Edsdk.EdsTime));
            IntPtr ptr = Marshal.AllocHGlobal(size);
            uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, size, ptr);

            outPropertyData = (Edsdk.EdsTime)Marshal.PtrToStructure(ptr, typeof(Edsdk.EdsTime));
            Marshal.FreeHGlobal(ptr);
            return err;
        }

        public static uint EdsGetPropertyData(IntPtr inRef, uint inPropertyID, int inParam, out string outPropertyData)
        {
            IntPtr ptr = Marshal.AllocHGlobal(256);
            uint err = EdsGetPropertyData(inRef, inPropertyID, inParam, 256, ptr);

            outPropertyData = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return err;
        }
        #endregion

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetPropertyData
        //
        //  Description:
        //      Sets property data for the object designated in inRef. 
        //
        //  Parameters:
        //       In:    inRef - The reference of the item.
        //              inPropertyID - The ProprtyID
        //              inParam - Additional information of property.
        //              inPropertySize - The number of bytes of the prepared buffer
        //                  for set property-value.
        //              inPropertyData - The buffer pointer to set property-value.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetPropertyData( IntPtr inRef, uint inPropertyID,
             int inParam, int inPropertySize, [MarshalAs(UnmanagedType.AsAny), In] object inPropertyData);
    
        /*-----------------------------------------------------------------------------
        //  
        //  Function:   EdsGetPropertyDesc
        //
        //  Description:
        //      Gets a list of property data that can be set for the object 
        //          designated in inRef, as well as maximum and minimum values. 
        //      This API is intended for only some shooting-related properties.
        //
        //  Parameters:
        //       In:    inRef - The reference of the camera.
        //              inPropertyID - The Property ID.
        //       Out:   outPropertyDesc - Array of the value which can be set up.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/ 
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetPropertyDesc( IntPtr inRef, uint inPropertyID,
             out EdsPropertyDesc outPropertyDesc);

        /*--------------------------------------------
          Device-list and device operating functions
        ---------------------------------------------*/     
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetCameraList
        //
        //  Description:
        //      Gets camera list objects.
        //
        //  Parameters:
        //       In:    None
        //      Out:    outCameraListRef - Pointer to the camera-list.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetCameraList( out IntPtr  outCameraListRef);

        /*--------------------------------------------
          Camera operating functions
        ---------------------------------------------*/ 
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetDeviceInfo
        //
        //  Description:
        //      Gets device information, such as the device name.  
        //      Because device information of remote cameras is stored 
        //          on the host computer, you can use this API 
        //          before the camera object initiates communication
        //          (that is, before a session is opened). 
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera.
        //      Out:    outDeviceInfo - Information as device of camera.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetDeviceInfo( IntPtr  inCameraRef, out EdsDeviceInfo  outDeviceInfo);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsOpenSession
        //
        //  Description:
        //      Establishes a logical connection with a remote camera. 
        //      Use this API after getting the camera's EdsCamera object.
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsOpenSession( IntPtr inCameraRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCloseSession
        //
        //  Description:
        //       Closes a logical connection with a remote camera.
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCloseSession( IntPtr inCameraRef);
    
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSendCommand
        //
        //  Description:
        //       Sends a command such as "Shoot" to a remote camera. 
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera which will receive the 
        //                      command.
        //              inCommand - Specifies the command to be sent.
        //              inParam -     Specifies additional command-specific information.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/ 
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSendCommand( IntPtr inCameraRef, uint inCommand, int inParam);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSendStatusCommand
        //
        //  Description:
        //       Sets the remote camera state or mode.
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera which will receive the 
        //                      command.
        //              inStatusCommand - Specifies the command to be sent.
        //              inParam -     Specifies additional command-specific information.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSendStatusCommand(IntPtr inCameraRef, uint inCameraState, int inParam);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetCapacity
        //
        //  Description:
        //      Sets the remaining HDD capacity on the host computer
        //          (excluding the portion from image transfer),
        //          as calculated by subtracting the portion from the previous time. 
        //      Set a reset flag initially and designate the cluster length 
        //          and number of free clusters.
        //      Some type 2 protocol standard cameras can display the number of shots 
        //          left on the camera based on the available disk capacity 
        //          of the host computer. 
        //      For these cameras, after the storage destination is set to the computer, 
        //          use this API to notify the camera of the available disk capacity 
        //          of the host computer.
        //
        //  Parameters:
        //       In:    inCameraRef - The reference of the camera which will receive the 
        //                      command.
        //              inCapacity -  The remaining capacity of a transmission place.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetCapacity( IntPtr inCameraRef, EdsCapacity inCapacity);


        /*--------------------------------------------
          Volume operating functions
        ---------------------------------------------*/ 
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetVolumeInfo
        //
        //  Description:
        //      Gets volume information for a memory card in the camera.
        //
        //  Parameters:
        //       In:    inVolumeRef - The reference of the volume.
        //      Out:    outVolumeInfo - information of  the volume.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetVolumeInfo( IntPtr inCameraRef, out EdsVolumeInfo outVolumeInfo);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsFormatVolume
        //
        //  Description:
        //       .
        //
        //  Parameters:
        //       In:    inVolumeRef - The reference of volume .
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsFormatVolume(IntPtr inVolumeRef);


        /*--------------------------------------------
          Directory-item operating functions
        ---------------------------------------------*/ 
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetDirectoryItemInfo
        //
        //  Description:
        //      Gets information about the directory or file objects 
        //          on the memory card (volume) in a remote camera.
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //      Out:    outDirItemInfo - information of the directory item.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetDirectoryItemInfo( IntPtr inDirItemRef,
             out EdsDirectoryItemInfo outDirItemInfo);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsDeleteDirectoryItem
        //
        //  Description:
        //      Deletes a camera folder or file.
        //      If folders with subdirectories are designated, all files are deleted 
        //          except protected files. 
        //      EdsDirectoryItem objects deleted by means of this API are implicitly 
        //          released by the EDSDK. Thus, there is no need to release them 
        //          by means of EdsRelease.
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsDeleteDirectoryItem( IntPtr inDirItemRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsDownload
        //
        //  Description:
        //       Downloads a file on a remote camera 
        //          (in the camera memory or on a memory card) to the host computer. 
        //      The downloaded file is sent directly to a file stream created in advance. 
        //      When dividing the file being retrieved, call this API repeatedly. 
        //      Also in this case, make the data block size a multiple of 512 (bytes), 
        //          excluding the final block.
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //              inReadSize   - 
        //
        //      Out:    outStream    - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsDownload(IntPtr inDirItemRef, UInt64 inReadSize, IntPtr outStream);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsDownloadCancel
        //
        //  Description:
        //       Must be executed when downloading of a directory item is canceled. 
        //      Calling this API makes the camera cancel file transmission.
        //      It also releases resources. 
        //      This operation need not be executed when using EdsDownloadThumbnail. 
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsDownloadCancel ( IntPtr inDirItemRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsDownloadComplete
        //
        //  Description:
        //       Must be called when downloading of directory items is complete. 
        //          Executing this API makes the camera 
        //              recognize that file transmission is complete. 
        //          This operation need not be executed when using EdsDownloadThumbnail.
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //
        //      Out:    outStream    - None.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsDownloadComplete ( IntPtr inDirItemRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsDownloadThumbnail
        //
        //  Description:
        //      Extracts and downloads thumbnail information from image files in a camera. 
        //      Thumbnail information in the camera's image files is downloaded 
        //          to the host computer. 
        //      Downloaded thumbnails are sent directly to a file stream created in advance.
        //
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //
        //      Out:    outStream - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsDownloadThumbnail( IntPtr inDirItemRef, IntPtr outStream);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetAttribute
        //
        //  Description:
        //      Gets attributes of files on a camera.
        //  
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //      Out:    outFileAttribute  - Indicates the file attributes. 
        //                  As for the file attributes, OR values of the value defined
        //                  by enum EdsFileAttributes can be retrieved. Thus, when 
        //                  determining the file attributes, you must check 
        //                  if an attribute flag is set for target attributes. 
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetAttribute( IntPtr inDirItemRef, out EdsFileAttribute outFileAttribute);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetAttribute
        //
        //  Description:
        //      Changes attributes of files on a camera.
        //  
        //  Parameters:
        //       In:    inDirItemRef - The reference of the directory item.
        //              inFileAttribute  - Indicates the file attributes. 
        //                      As for the file attributes, OR values of the value 
        //                      defined by enum EdsFileAttributes can be retrieved. 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetAttribute( IntPtr inDirItemRef, EdsFileAttribute inFileAttribute);

        /*--------------------------------------------
          Stream operating functions
        ---------------------------------------------*/
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCreateFileStream
        //
        //  Description:
        //      Creates a new file on a host computer (or opens an existing file) 
        //          and creates a file stream for access to the file. 
        //      If a new file is designated before executing this API, 
        //          the file is actually created following the timing of writing 
        //          by means of EdsWrite or the like with respect to an open stream.
        //
        //  Parameters:
        //       In:    inFileName - Pointer to a null-terminated string that specifies
        //                           the file name.
        //              inCreateDisposition - Action to take on files that exist, 
        //                                and which action to take when files do not exist.  
        //              inDesiredAccess - Access to the stream (reading, writing, or both).
        //      Out:    outStream - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/     
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCreateFileStream( string inFileName, EdsFileCreateDisposition inCreateDisposition,
             EdsAccess inDesiredAccess, out IntPtr outStream);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCreateMemoryStream
        //
        //  Description:
        //      Creates a stream in the memory of a host computer. 
        //      In the case of writing in excess of the allocated buffer size, 
        //          the memory is automatically extended.
        //
        //  Parameters:
        //       In:    inBufferSize - The number of bytes of the memory to allocate.
        //      Out:    outStream - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCreateMemoryStream(UInt64 inBufferSize, out IntPtr outStream);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCreateStreamEx
        //
        //  Description:
        //      An extended version of EdsCreateStreamFromFile. 
        //      Use this function when working with Unicode file names.
        //
        //  Parameters:
        //       In:    inFileName - Designate the file name. 
        //              inCreateDisposition - Action to take on files that exist, 
        //                                and which action to take when files do not exist.  
        //              inDesiredAccess - Access to the stream (reading, writing, or both).
        //
        //      Out:    outStream - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCreateStreamEx( 
           string                       inFileName,
           EdsFileCreateDisposition     inCreateDisposition,
           EdsAccess                    inDesiredAccess,
           out IntPtr                   outStream 
           );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCreateMemoryStreamFromPointer        
        //
        //  Description:
        //      Creates a stream from the memory buffer you prepare. 
        //      Unlike the buffer size of streams created by means of EdsCreateMemoryStream, 
        //      the buffer size you prepare for streams created this way does not expand.
        //
        //  Parameters:
        //       In:    inBufferSize - The number of bytes of the memory to allocate.
        //      Out:    outStream - The reference of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCreateMemoryStreamFromPointer(byte[] inUserBuffer,
             UInt64 inBufferSize, out IntPtr outStream);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetPointer
        //
        //  Description:
        //      Gets the pointer to the start address of memory managed by the memory stream. 
        //      As the EDSDK automatically resizes the buffer, the memory stream provides 
        //          you with the same access methods as for the file stream. 
        //      If access is attempted that is excessive with regard to the buffer size
        //          for the stream, data before the required buffer size is allocated 
        //          is copied internally, and new writing occurs. 
        //      Thus, the buffer pointer might be switched on an unknown timing. 
        //      Caution in use is therefore advised. 
        //
        //  Parameters:
        //       In:    inStream - Designate the memory stream for the pointer to retrieve. 
        //      Out:    outPointer - If successful, returns the pointer to the buffer 
        //                  written in the memory stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetPointer(IntPtr inStreamRef, out IntPtr outPointer);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsRead
        //
        //  Description:
        //      Reads data the size of inReadSize into the outBuffer buffer, 
        //          starting at the current read or write position of the stream. 
        //      The size of data actually read can be designated in outReadSize.
        //
        //  Parameters:
        //       In:    inStreamRef - The reference of the stream or image.
        //              inReadSize -  The number of bytes to read.
        //      Out:    outBuffer - Pointer to the user-supplied buffer that is to receive
        //                          the data read from the stream. 
        //              outReadSize - The actually read number of bytes.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsRead(IntPtr inStreamRef, UInt64 inReadSize, IntPtr outBuffer,
             out UInt64 outReadSize);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsWrite
        //
        //  Description:
        //      Writes data of a designated buffer 
        //          to the current read or write position of the stream. 
        //
        //  Parameters:
        //       In:    inStreamRef  - The reference of the stream or image.
        //              inWriteSize - The number of bytes to write.
        //              inBuffer - A pointer to the user-supplied buffer that contains 
        //                         the data to be written to the stream.
        //      Out:    outWrittenSize - The actually written-in number of bytes.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsWrite(IntPtr inStreamRef, UInt64 inWriteSize, IntPtr inBuffer,
             out uint outWrittenSize);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSeek
        //
        //  Description:
        //      Moves the read or write position of the stream
                    (that is, the file position indicator).
        //
        //  Parameters:
        //       In:    inStreamRef  - The reference of the stream or image. 
        //              inSeekOffset - Number of bytes to move the pointer. 
        //              inSeekOrigin - Pointer movement mode. Must be one of the following 
        //                             values.
        //                  kEdsSeek_Cur     Move the stream pointer inSeekOffset bytes 
        //                                   from the current position in the stream. 
        //                  kEdsSeek_Begin   Move the stream pointer inSeekOffset bytes
        //                                   forward from the beginning of the stream. 
        //                  kEdsSeek_End     Move the stream pointer inSeekOffset bytes
        //                                   from the end of the stream. 
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSeek(IntPtr inStreamRef, Int64 inSeekOffset, EdsSeekOrigin inSeekOrigin);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetPosition
        //
        //  Description:
        //       Gets the current read or write position of the stream
        //          (that is, the file position indicator).
        //
        //  Parameters:
        //       In:    inStreamRef - The reference of the stream or image.
        //      Out:    outPosition - The current stream pointer.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetPosition(IntPtr inStreamRef, out UInt64 outPosition);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetLength
        //
        //  Description:
        //      Gets the stream size.
        //
        //  Parameters:
        //       In:    inStreamRef - The reference of the stream or image.
        //      Out:    outLength - The length of the stream.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetLength(IntPtr inStreamRef, out UInt64 outLength);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCopyData
        //
        //  Description:
        //      Copies data from the copy source stream to the copy destination stream. 
        //      The read or write position of the data to copy is determined from 
        //          the current file read or write position of the respective stream. 
        //      After this API is executed, the read or write positions of the copy source 
        //          and copy destination streams are moved an amount corresponding to 
        //          inWriteSize in the positive direction. 
        //
        //  Parameters:
        //       In:    inStreamRef - The reference of the stream or image.
        //              inWriteSize - The number of bytes to copy.
        //      Out:    outStreamRef - The reference of the stream or image.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCopyData(IntPtr inStreamRef, UInt64 inWriteSize, IntPtr outStreamRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetProgressCallback
        //
        //  Description:
        //      Register a progress callback function. 
        //      An event is received as notification of progress during processing that 
        //          takes a relatively long time, such as downloading files from a
        //          remote camera. 
        //      If you register the callback function, the EDSDK calls the callback
        //          function during execution or on completion of the following APIs. 
        //      This timing can be used in updating on-screen progress bars, for example.
        //
        //  Parameters:
        //       In:    inRef - The reference of the stream or image.
        //              inProgressCallback - Pointer to a progress callback function.
        //              inProgressOption - The option about progress is specified.
        //                              Must be one of the following values.
        //                         kEdsProgressOption_Done 
        //                             When processing is completed,a callback function
        //                             is called only at once.
        //                         kEdsProgressOption_Periodically
        //                             A callback function is performed periodically.
        //              inContext - Application information, passed in the argument 
        //                      when the callback function is called. Any information 
        //                      required for your program may be added. 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetProgressCallback( IntPtr inRef, EdsProgressCallback inProgressFunc,
             EdsProgressOption inProgressOption, IntPtr inContext);
    

        /*--------------------------------------------
          Image operating functions
        ---------------------------------------------*/ 
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCreateImageRef
        //
        //  Description:
        //      Creates an image object from an image file. 
        //      Without modification, stream objects cannot be worked with as images. 
        //      Thus, when extracting images from image files, 
        //          you must use this API to create image objects. 
        //      The image object created this way can be used to get image information 
        //          (such as the height and width, number of color components, and
        //           resolution), thumbnail image data, and the image data itself.
        //
        //  Parameters:
        //       In:    inStreamRef - The reference of the stream.
        //
        //       Out:    outImageRef - The reference of the image.
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCreateImageRef( IntPtr inStreamRef,  out IntPtr outImageRef);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetImageInfo
        //
        //  Description:
        //      Gets image information from a designated image object. 
        //      Here, image information means the image width and height, 
        //          number of color components, resolution, and effective image area.
        //
        //  Parameters:
        //       In:    inStreamRef - Designate the object for which to get image information. 
        //              inImageSource - Of the various image data items in the image file,
        //                  designate the type of image data representing the 
        //                  information you want to get. Designate the image as
        //                  defined in Enum EdsImageSource. 
        //
        //                      kEdsImageSrc_FullView
        //                                  The image itself (a full-sized image) 
        //                      kEdsImageSrc_Thumbnail
        //                                  A thumbnail image 
        //                      kEdsImageSrc_Preview
        //                                  A preview image
        //                      kEdsImageSrc_RAWThumbnail
        //                                  A RAW thumbnail image 
        //                      kEdsImageSrc_RAWFullView
        //                                  A RAW full-sized image 
        //       Out:    outImageInfo - Stores the image data information designated 
        //                      in inImageSource. 
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetImageInfo( IntPtr inImageRef, EdsImageSource inImageSource,
              out EdsImageInfo outImageInfo );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsGetImage                         
        //
        //  Description:
        //      Gets designated image data from an image file, in the form of a
        //          designated rectangle. 
        //      Returns uncompressed results for JPEGs and processed results 
        //          in the designated pixel order (RGB, Top-down BGR, and so on) for
        //           RAW images. 
        //      Additionally, by designating the input/output rectangle, 
        //          it is possible to get reduced, enlarged, or partial images. 
        //      However, because images corresponding to the designated output rectangle 
        //          are always returned by the SDK, the SDK does not take the aspect 
        //          ratio into account. 
        //      To maintain the aspect ratio, you must keep the aspect ratio in mind 
        //          when designating the rectangle. 
        //
        //  Parameters:
        //      In:     
        //              inImageRef - Designate the image object for which to get 
        //                      the image data.
        //              inImageSource - Designate the type of image data to get from
        //                      the image file (thumbnail, preview, and so on). 
        //                      Designate values as defined in Enum EdsImageSource. 
        //              inImageType - Designate the output image type. Because
        //                      the output format of EdGetImage may only be RGB, only
        //                      kEdsTargetImageType_RGB or kEdsTargetImageType_RGB16
        //                      can be designated. 
        //                      However, image types exceeding the resolution of 
        //                      inImageSource cannot be designated. 
        //              inSrcRect - Designate the coordinates and size of the rectangle
        //                      to be retrieved (processed) from the source image. 
        //              inDstSize - Designate the rectangle size for output. 
        //
        //      Out:    
        //              outStreamRef - Designate the memory or file stream for output of
        //                      the image.
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsGetImage( IntPtr inImageRef, EdsImageSource inImageSource,
             EdsTargetImageType inImageType, EdsRect inSrcRect, EdsSize inDstSize, IntPtr outStreamRef );
    
        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSaveImage            
        //
        //  Description:
        //      Saves as a designated image type after RAW processing. 
        //      When saving with JPEG compression, 
        //          the JPEG quality setting applies with respect to EdsOptionRef.
        //
        //  Parameters:
        //      In:    
        //          inImageRef - Designate the image object for which to produce the file. 
        //          inImageType - Designate the image type to produce. Designate the 
        //                  following image types.
        //
        //                  kEdsTargetImageType - Jpeg  JPEG
        //                  kEdsTargetImageType - TIFF  8-bit TIFF
        //                  kEdsTargetImageType - TIFF16    16-bit TIFF
        //          inSaveSetting - Designate saving options, such as JPEG image quality.
        //      Out:    
        //          outStreamRef - Specifies the output file stream. The memory stream 
        //                  cannot be specified here.
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/ 
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSaveImage( IntPtr inImageRef, EdsTargetImageType inImageType,
             EdsSaveImageSetting inSaveSetting, IntPtr outStreamRef );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsCacheImage                       
        //
        //  Description:
        //      Switches a setting on and off for creation of an image cache in the SDK 
        //          for a designated image object during extraction (processing) of
        //          the image data. 
        //          Creating the cache increases the processing speed, starting from
        //          the second time.
        //
        //  Parameters:
        //      In:     inImageRef - The reference of the image.
        //              inUseCache - If cache image data or not
        //                          If set to FALSE, the cached image data will released.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsCacheImage( IntPtr inImageRef, bool inUseCache );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsReflectImageProperty         
        //  Description:
        //      Incorporates image object property changes 
        //          (effected by means of EdsSetPropertyData) in the stream. 
        //
        //  Parameters:
        //      In:     inImageRef - The reference of the image.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsReflectImageProperty( IntPtr inImageRef );

        //----------------------------------------------
        //   Event handler registering functions
        //----------------------------------------------            
         /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetCameraAddedHandler
        //
        //  Description:
        //      Registers a callback function for when a camera is detected.
        //
        //  Parameters:
        //       In:    inCameraAddedHandler - Pointer to a callback function
        //                          called when a camera is connected physically
        //              inContext - Specifies an application-defined value to be sent to
        //                          the callback function pointed to by CallBack parameter.
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetCameraAddedHandler(EdsCameraAddedHandler inCameraAddedHandler,
              IntPtr inContext);

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetPropertyEventHandler
        //              
        //  Description:
        //       Registers a callback function for receiving status 
        //          change notification events for property states on a camera.
        //
        //  Parameters:
        //       In:    inCameraRef - Designate the camera object. 
        //              inEvent - Designate one or all events to be supplemented.
        //              inPropertyEventHandler - Designate the pointer to the callback
        //                      function for receiving property-related camera events.
        //              inContext - Designate application information to be passed by 
        //                      means of the callback function. Any data needed for
        //                      your application can be passed. 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetPropertyEventHandler( IntPtr inCameraRef,  uint inEvnet,
             EdsPropertyEventHandler  inPropertyEventHandler, IntPtr inContext );

        /*-----------------------------------------------------------------------------
        //
        //  Function:   EdsSetObjectEventHandler
        //              
        //  Description:
        //       Registers a callback function for receiving status 
        //          change notification events for objects on a remote camera. 
        //      Here, object means volumes representing memory cards, files and directories, 
        //          and shot images stored in memory, in particular. 
        //
        //  Parameters:
        //       In:    inCameraRef - Designate the camera object. 
        //              inEvent - Designate one or all events to be supplemented.
        //                  To designate all events, use kEdsObjectEvent_All. 
        //              inObjectEventHandler - Designate the pointer to the callback function
        //                  for receiving object-related camera events.
        //              inContext - Passes inContext without modification,
        //                  as designated as an EdsSetObjectEventHandler argument. 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetObjectEventHandler( IntPtr  inCameraRef, uint inEvnet,
             EdsObjectEventHandler  inObjectEventHandler, IntPtr inContext );

        /*-----------------------------------------------------------------------------
        //
        //  Function:  EdsSetCameraStateEventHandler
        //              
        //  Description:
        //      Registers a callback function for receiving status 
        //          change notification events for property states on a camera.
        //
        //  Parameters:
        //       In:    inCameraRef - Designate the camera object. 
        //              inEvent - Designate one or all events to be supplemented.
        //                  To designate all events, use kEdsStateEvent_All. 
        //              inStateEventHandler - Designate the pointer to the callback function
        //                  for receiving events related to camera object states.
        //              inContext - Designate application information to be passed
        //                  by means of the callback function. Any data needed for
        //                  your application can be passed. 
        //      Out:    None
        //
        //  Returns:    Any of the sdk errors.
        -----------------------------------------------------------------------------*/
        [DllImport("EDSDK.dll")]
        public extern static uint EdsSetCameraStateEventHandler( IntPtr inCameraRef, uint inEvnet,
             EdsStateEventHandler  inStateEventHandler, IntPtr inContext );

		/*-----------------------------------------------------------------------------
		//
		//  Function:   EdsCreateEvfImageRef         
		//  Description:
		//       Creates an object used to get the live view image data set. 
		//
		//  Parameters:
		//      In:     inStreamRef - The stream reference which opened to get EVF JPEG image.
		//      Out:    outEvfImageRef - The EVFData reference.
		//
		//  Returns:    Any of the sdk errors.
		-----------------------------------------------------------------------------*/
		[DllImport("EDSDK.dll")]
		public extern static uint EdsCreateEvfImageRef(IntPtr inStreamRef, out IntPtr outEvfImageRef);


		/*-----------------------------------------------------------------------------
		//
		//  Function:   EdsDownloadEvfImage         
		//  Description:
		//		Downloads the live view image data set for a camera currently in live view mode.
		//		Live view can be started by using the property ID:kEdsPropertyID_Evf_OutputDevice and
		//		data:EdsOutputDevice_PC to call EdsSetPropertyData.
		//		In addition to image data, information such as zoom, focus position, and histogram data
		//		is included in the image data set. Image data is saved in a stream maintained by EdsEvfImageRef.
		//		EdsGetPropertyData can be used to get information such as the zoom, focus position, etc.
		//		Although the information of the zoom and focus position can be obtained from EdsEvfImageRef,
		//		settings are applied to EdsCameraRef.
		//
		//  Parameters:
		//      In:     inCameraRef - The Camera reference.
		//      In:     inEvfImageRef - The EVFData reference.
		//
		//  Returns:    Any of the sdk errors.
		-----------------------------------------------------------------------------*/
		[DllImport("EDSDK.dll")]
		public extern static uint EdsDownloadEvfImage(IntPtr inCameraRef, IntPtr outEvfImageRef);   

        #endregion


        #region Definition of base Structures

        public const int EDS_MAX_NAME = 256;
        public const int EDS_TRANSFER_BLOCK_SIZE =  512;

        /*-----------------------------------------------------------------------------
         Point
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public  struct EdsPoint 
        {
            public int  x;
            public int  y;
        }

        /*-----------------------------------------------------------------------------
         Rectangle
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsRect 
        {
            public int  x;
            public int  y;
            public int  width;
            public int  height;
        }

        /*-----------------------------------------------------------------------------
         Size
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsSize
        {
            public int width;
            public int height;
        }

        /*-----------------------------------------------------------------------------
         Rational
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsRational 
        {
            public int  Numerator;
            public uint Denominator;
        }

        /*-----------------------------------------------------------------------------
         Time
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsTime
        {
            public int   Year;                      
            public int   Month;                     
            public int   Day;                  
            public int   Hour;                      
            public int   Minute;                  
            public int   Second;                 
            public int   Milliseconds;    
        }

        
        /*-----------------------------------------------------------------------------
         Device Info
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsDeviceInfo 
        {
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = EDS_MAX_NAME)] 
            public string   szPortName;
        
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = EDS_MAX_NAME)] 
            public string   szDeviceDescription;

            public uint DeviceSubType;

            public uint reserved;
        }

        /*-----------------------------------------------------------------------------
         Volume Info
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsVolumeInfo 
        {
            public uint StorageType;
            public uint Access;
            public ulong    MaxCapacity;
            public ulong    FreeSpaceInBytes;

            [MarshalAs( UnmanagedType.ByValTStr, SizeConst = EDS_MAX_NAME)]
            public string   szVolumeLabel;
        }


        /*-----------------------------------------------------------------------------
         DirectoryItem Info
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsDirectoryItemInfo 
        {
            public UInt64 Size;
            public int  isFolder;               
            public uint GroupID;
            public uint Option;
            
            [MarshalAs( UnmanagedType.ByValTStr, SizeConst=EDS_MAX_NAME)]
            public string szFileName;

			public uint format;
			public uint dateTime;
		} 


        /*-----------------------------------------------------------------------------
         Image Info
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsImageInfo
        {
            public uint Width;                  // image width 
            public uint Height;                 // image height
    
            public uint NumOfComponents;        // number of color components in image.
            public uint ComponentDepth;         // bits per sample.  8 or 16.

            public EdsRect  EffectiveRect;          // Effective rectangles except 
                                            // a black line of the image. 
                                            // A black line might be in the top and bottom
                                            // of the thumbnail image. 

            public uint reserved1;
            public uint reserved2;
                                        
        }

        /*-----------------------------------------------------------------------------
         SaveImage Setting
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsSaveImageSetting 
        {
            public uint     JPEGQuality;    
            IntPtr          iccProfileStream;   
            public uint     reserved;   
        }

        /*-----------------------------------------------------------------------------
         Property Desc
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsPropertyDesc 
        {
            public int      Form;   
            public uint     Access; 
            public int      NumElements;
        
            [MarshalAs( UnmanagedType.ByValArray, SizeConst=128)]
            public int[ ]   PropDesc;       
        }

        /*-----------------------------------------------------------------------------
         Picture Style Desc
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsPictureStyleDesc 
        {
            public int      contrast;   
            public uint     sharpness;  
            public int      saturation;
            public int      colorTone;  
            public uint     filterEffect;   
            public uint     toningEffect;
            public uint     sharpFineness;
            public uint     sharpThreshold;
        }

        /*-----------------------------------------------------------------------------
         Focus Info
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential)]
        public struct EdsFocusPoint 
        {
            public uint     valid;
            public uint     selected;
            public uint     justFocus;  
            public EdsRect  rect;
            public uint     reserved;       
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct EdsFocusInfo 
        {
            public EdsRect          imageRect;  
            public uint             pointNumber;

            [MarshalAs( UnmanagedType.ByValArray, SizeConst=128)]
            public EdsFocusPoint[]  focusPoint;
            public uint             executeMode;
        }


        /*-----------------------------------------------------------------------------
         Capacity
        -----------------------------------------------------------------------------*/
        [StructLayout(LayoutKind.Sequential, Pack = 2)]
        public struct EdsCapacity 
        {
            public int      NumberOfFreeClusters;
            public int      BytesPerSector;     
            public int      Reset;      
        }

        #endregion


        #region  Definition of error Codes

        /*-----------------------------------------------------------------------
           ED-SDK Error Code Masks
        ------------------------------------------------------------------------*/
        public const uint EDS_ISSPECIFIC_MASK =                                 0x80000000;
        public const uint EDS_COMPONENTID_MASK =                                0x7F000000;
        public const uint EDS_RESERVED_MASK =                                   0x00FF0000;
        public const uint EDS_ERRORID_MASK =                                    0x0000FFFF;

        /*-----------------------------------------------------------------------
           ED-SDK Base Component IDs
        ------------------------------------------------------------------------*/
        public const uint EDS_CMP_ID_CLIENT_COMPONENTID =                       0x01000000;
        public const uint EDS_CMP_ID_LLSDK_COMPONENTID =                        0x02000000;
        public const uint EDS_CMP_ID_HLSDK_COMPONENTID =                        0x03000000;

        /*-----------------------------------------------------------------------
           ED-SDK Functin Success Code
        ------------------------------------------------------------------------*/
        public const uint EDS_ERR_OK =                                          0x00000000;

        /*-----------------------------------------------------------------------
           ED-SDK Generic Error IDs
        ------------------------------------------------------------------------*/
        /* Miscellaneous errors */
        public const uint EDS_ERR_UNIMPLEMENTED =                               0x00000001;  
        public const uint EDS_ERR_INTERNAL_ERROR =                              0x00000002;
        public const uint EDS_ERR_MEM_ALLOC_FAILED =                            0x00000003;
        public const uint EDS_ERR_MEM_FREE_FAILED =                             0x00000004;
        public const uint EDS_ERR_OPERATION_CANCELLED =                         0x00000005;
        public const uint EDS_ERR_INCOMPATIBLE_VERSION =                        0x00000006;
        public const uint EDS_ERR_NOT_SUPPORTED =                               0x00000007;
        public const uint EDS_ERR_UNEXPECTED_EXCEPTION =                        0x00000008;
        public const uint EDS_ERR_PROTECTION_VIOLATION =                        0x00000009;
        public const uint EDS_ERR_MISSING_SUBCOMPONENT =                        0x0000000A;
        public const uint EDS_ERR_SELECTION_UNAVAILABLE =                       0x0000000B;

        /* File errors */
        public const uint EDS_ERR_FILE_IO_ERROR =                               0x00000020;
        public const uint EDS_ERR_FILE_TOO_MANY_OPEN =                          0x00000021;
        public const uint EDS_ERR_FILE_NOT_FOUND =                              0x00000022;
        public const uint EDS_ERR_FILE_OPEN_ERROR =                             0x00000023;
        public const uint EDS_ERR_FILE_CLOSE_ERROR =                            0x00000024;
        public const uint EDS_ERR_FILE_SEEK_ERROR =                             0x00000025;
        public const uint EDS_ERR_FILE_TELL_ERROR =                             0x00000026;
        public const uint EDS_ERR_FILE_READ_ERROR =                             0x00000027;
        public const uint EDS_ERR_FILE_WRITE_ERROR =                            0x00000028;
        public const uint EDS_ERR_FILE_PERMISSION_ERROR =                       0x00000029;
        public const uint EDS_ERR_FILE_DISK_FULL_ERROR =                        0x0000002A;
        public const uint EDS_ERR_FILE_ALREADY_EXISTS =                         0x0000002B;
        public const uint EDS_ERR_FILE_FORMAT_UNRECOGNIZED =                    0x0000002C;
        public const uint EDS_ERR_FILE_DATA_CORRUPT =                           0x0000002D;
        public const uint EDS_ERR_FILE_NAMING_NA =                              0x0000002E;

        /* Directory errors */          
        public const uint EDS_ERR_DIR_NOT_FOUND =                               0x00000040;
        public const uint EDS_ERR_DIR_IO_ERROR =                                0x00000041;
        public const uint EDS_ERR_DIR_ENTRY_NOT_FOUND =                         0x00000042;
        public const uint EDS_ERR_DIR_ENTRY_EXISTS =                            0x00000043;
        public const uint EDS_ERR_DIR_NOT_EMPTY =                               0x00000044;

        /* Property errors */
        public const uint EDS_ERR_PROPERTIES_UNAVAILABLE =                      0x00000050;
        public const uint EDS_ERR_PROPERTIES_MISMATCH =                         0x00000051;
        public const uint EDS_ERR_PROPERTIES_NOT_LOADED =                       0x00000053;

        /* Function Parameter errors */     
        public const uint EDS_ERR_INVALID_PARAMETER =                           0x00000060;
        public const uint EDS_ERR_INVALID_HANDLE =                              0x00000061;
        public const uint EDS_ERR_INVALID_POINTER =                             0x00000062;
        public const uint EDS_ERR_INVALID_INDEX =                               0x00000063;
        public const uint EDS_ERR_INVALID_LENGTH =                              0x00000064;
        public const uint EDS_ERR_INVALID_FN_POINTER =                          0x00000065;
        public const uint EDS_ERR_INVALID_SORT_FN =                             0x00000066;

        /* Device errors */
        public const uint EDS_ERR_DEVICE_NOT_FOUND =                            0x00000080;
        public const uint EDS_ERR_DEVICE_BUSY =                                 0x00000081;
        public const uint EDS_ERR_DEVICE_INVALID =                              0x00000082;
        public const uint EDS_ERR_DEVICE_EMERGENCY =                            0x00000083;
        public const uint EDS_ERR_DEVICE_MEMORY_FULL =                          0x00000084;
        public const uint EDS_ERR_DEVICE_INTERNAL_ERROR =                       0x00000085;
        public const uint EDS_ERR_DEVICE_INVALID_PARAMETER =                    0x00000086;
        public const uint EDS_ERR_DEVICE_NO_DISK =                              0x00000087;
        public const uint EDS_ERR_DEVICE_DISK_ERROR =                           0x00000088;
        public const uint EDS_ERR_DEVICE_CF_GATE_CHANGED =                      0x00000089;
        public const uint EDS_ERR_DEVICE_DIAL_CHANGED =                         0x0000008A;
        public const uint EDS_ERR_DEVICE_NOT_INSTALLED =                        0x0000008B;
        public const uint EDS_ERR_DEVICE_STAY_AWAKE =                           0x0000008C;
        public const uint EDS_ERR_DEVICE_NOT_RELEASED =                         0x0000008D;

        /* Stream errors */
        public const uint EDS_ERR_STREAM_IO_ERROR =                             0x000000A0;
        public const uint EDS_ERR_STREAM_NOT_OPEN =                             0x000000A1;
        public const uint EDS_ERR_STREAM_ALREADY_OPEN =                         0x000000A2;
        public const uint EDS_ERR_STREAM_OPEN_ERROR =                           0x000000A3;
        public const uint EDS_ERR_STREAM_CLOSE_ERROR =                          0x000000A4;
        public const uint EDS_ERR_STREAM_SEEK_ERROR =                           0x000000A5;
        public const uint EDS_ERR_STREAM_TELL_ERROR =                           0x000000A6;
        public const uint EDS_ERR_STREAM_READ_ERROR =                           0x000000A7;
        public const uint EDS_ERR_STREAM_WRITE_ERROR =                          0x000000A8;
        public const uint EDS_ERR_STREAM_PERMISSION_ERROR =                     0x000000A9;
        public const uint EDS_ERR_STREAM_COULDNT_BEGIN_THREAD =                 0x000000AA;
        public const uint EDS_ERR_STREAM_BAD_OPTIONS =                          0x000000AB;
        public const uint EDS_ERR_STREAM_END_OF_STREAM =                        0x000000AC;

        /* Communications errors */
        public const uint EDS_ERR_COMM_PORT_IS_IN_USE =                         0x000000C0;
        public const uint EDS_ERR_COMM_DISCONNECTED =                           0x000000C1;
        public const uint EDS_ERR_COMM_DEVICE_INCOMPATIBLE =                    0x000000C2;
        public const uint EDS_ERR_COMM_BUFFER_FULL =                            0x000000C3;
        public const uint EDS_ERR_COMM_USB_BUS_ERR =                            0x000000C4;

        /* Lock/Unlock */
        public const uint EDS_ERR_USB_DEVICE_LOCK_ERROR =                       0x000000D0;
        public const uint EDS_ERR_USB_DEVICE_UNLOCK_ERROR =                     0x000000D1;

        /* STI/WIA */
        public const uint EDS_ERR_STI_UNKNOWN_ERROR =                           0x000000E0;
        public const uint EDS_ERR_STI_INTERNAL_ERROR =                          0x000000E1;
        public const uint EDS_ERR_STI_DEVICE_CREATE_ERROR =                     0x000000E2;
        public const uint EDS_ERR_STI_DEVICE_RELEASE_ERROR =                    0x000000E3;
        public const uint EDS_ERR_DEVICE_NOT_LAUNCHED =                         0x000000E4;
    
        public const uint EDS_ERR_ENUM_NA =                                     0x000000F0;
        public const uint EDS_ERR_INVALID_FN_CALL =                             0x000000F1;
        public const uint EDS_ERR_HANDLE_NOT_FOUND =                            0x000000F2;
        public const uint EDS_ERR_INVALID_ID =                                  0x000000F3;
        public const uint EDS_ERR_WAIT_TIMEOUT_ERROR =                          0x000000F4;

        /* PTP */
        public const uint EDS_ERR_SESSION_NOT_OPEN =                            0x00002003;
        public const uint EDS_ERR_INVALID_TRANSACTIONID =                       0x00002004;
        public const uint EDS_ERR_INCOMPLETE_TRANSFER =                         0x00002007;
        public const uint EDS_ERR_INVALID_STRAGEID =                            0x00002008;
        public const uint EDS_ERR_DEVICEPROP_NOT_SUPPORTED =                    0x0000200A;
        public const uint EDS_ERR_INVALID_OBJECTFORMATCODE =                    0x0000200B;
        public const uint EDS_ERR_SELF_TEST_FAILED =                            0x00002011;
        public const uint EDS_ERR_PARTIAL_DELETION =                            0x00002012;
        public const uint EDS_ERR_SPECIFICATION_BY_FORMAT_UNSUPPORTED =         0x00002014;
        public const uint EDS_ERR_NO_VALID_OBJECTINFO =                         0x00002015;
        public const uint EDS_ERR_INVALID_CODE_FORMAT =                         0x00002016;
        public const uint EDS_ERR_UNKNOWN_VENDER_CODE =                         0x00002017;
        public const uint EDS_ERR_CAPTURE_ALREADY_TERMINATED =                  0x00002018;
        public const uint EDS_ERR_INVALID_PARENTOBJECT =                        0x0000201A;
        public const uint EDS_ERR_INVALID_DEVICEPROP_FORMAT =                   0x0000201B;
        public const uint EDS_ERR_INVALID_DEVICEPROP_VALUE =                    0x0000201C;
        public const uint EDS_ERR_SESSION_ALREADY_OPEN =                        0x0000201E;
        public const uint EDS_ERR_TRANSACTION_CANCELLED =                       0x0000201F;
        public const uint EDS_ERR_SPECIFICATION_OF_DESTINATION_UNSUPPORTED =    0x00002020;
        public const uint EDS_ERR_UNKNOWN_COMMAND =                             0x0000A001;
        public const uint EDS_ERR_OPERATION_REFUSED =                           0x0000A005;
        public const uint EDS_ERR_LENS_COVER_CLOSE =                            0x0000A006;
		public const uint EDS_ERR_LOW_BATTERY =									0x0000A101;
		public const uint EDS_ERR_OBJECT_NOTREADY =								0x0000A102;

		/* Capture Error */
		public const uint EDS_ERR_TAKE_PICTURE_AF_NG =							0x00008D01;
		public const uint EDS_ERR_TAKE_PICTURE_RESERVED =						0x00008D02;
		public const uint EDS_ERR_TAKE_PICTURE_MIRROR_UP_NG =					0x00008D03;
		public const uint EDS_ERR_TAKE_PICTURE_SENSOR_CLEANING_NG =				0x00008D04;
		public const uint EDS_ERR_TAKE_PICTURE_SILENCE_NG =						0x00008D05;
		public const uint EDS_ERR_TAKE_PICTURE_NO_CARD_NG =						0x00008D06;
		public const uint EDS_ERR_TAKE_PICTURE_CARD_NG =						0x00008D07;
		public const uint EDS_ERR_TAKE_PICTURE_CARD_PROTECT_NG =				0x00008D08;


        public const uint EDS_ERR_LAST_GENERIC_ERROR_PLUS_ONE =                 0x000000F5;

        #endregion


    }
}
