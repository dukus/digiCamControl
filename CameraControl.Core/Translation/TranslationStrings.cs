#region Licence

// Distributed under MIT License
// ===========================================================
// 
// digiCamControl - DSLR camera remote control open source software
// Copyright (C) 2014 Duka Istvan
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, 
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF 
// MERCHANTABILITY,FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. 
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY 
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH 
// THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

#endregion

namespace CameraControl.Core.Translation
{
    public class TranslationStrings
    {
        public static string Mode = "Mode";
        public static string Iso = "_Iso";
        public static string ShutterSpeed = "_Shutter speed";
        public static string Aperture = "_Aperture";
        public static string WhiteBalance = "_White Balance";
        public static string ExposureComp = "_Exposure Comp.";
        public static string Compression = "_Compression";
        public static string MeteringMode = "_Metering Mode";
        public static string FocusMode = "_Focus Mode";
        public static string Battery = "Battery";
        public static string SDRam = "SD_Ram";
        public static string SDRamToolTip = "Capture images direct to PC without using card";
        public static string CapturePhotoToolTip = "Capture photo";
        public static string CapturePhotoNoAfToolTip = "Capture photo no auto focus";
        public static string BraketingToolTip = "Bracketing";
        public static string SettingsToolTip = "Settings";
        public static string TimeLapseToolTip = "Time lapse";
        public static string FullscreenToolTip = "Fullscreen";
        public static string LiveViewToolTip = "Live view";
        public static string BrowseSessionsToolTip = "Browse sessions";
        public static string SelectTagsToolTip = "Select Tags";
        public static string AboutToolTip = "About";
        public static string Session = "Session";
        public static string SessionAdd = "Add";
        public static string SessionEdit = "Edit";
        public static string SessionDel = "Del";

        public static string MainWindowTitle = "NCC";
        public static string SettingsWindowTitle = "Settings";
        public static string SessionWindowTitle = "Session [Add/Edit]";
        public static string LiveViewWindowTitle = "Live view";
        public static string PresetEditWindowTitle = "Preset Edit";
        public static string BraketingWindowTitle = "Bracketing";
        public static string EditTagWindowTitle = "Tag [Add/Edit]";
        public static string MultipleCameraWindowTitle = "Multiple camera support";
        public static string SavePresetWindowTitle = "Save Preset";
        public static string TimeLapseWindowTitle = "Time Lapse";

        public static string ButtonOk = "Ok";
        public static string ButtonCancel = "Cancel";
        public static string ButtonAdd = "Add";
        public static string ButtonDelete = "Delete";
        public static string ButtonEdit = "Edit";
        public static string ButtonAutoFocus = "_Auto Focus";
        public static string ButtonCapture = "_Capture";
        public static string ButtonRecordMovie = "Record video";
        public static string ButtonFreezeImage = "Freeze Image";
        public static string ButtonStart = "Start";
        public static string ButtonPreview = "Preview";
        public static string ButtonStop = "Stop";
        public static string ButtonDeletePreset = "Delete preset";
        public static string ButtonDeleteProperty = "Delete property";
        public static string ButtonClose = "Close";
        public static string ButtonSave = "Save";
        public static string ButtonCapturePhotos = "Capture photos";
        public static string ButtonStartTimeLapse = "Start TimeLapse";
        public static string ButtonStopTimeLapse = "Stop TimeLapse";
        public static string ButtonCreateMovie = "Create video";

        public static string ButtonLocateLogFile = "Locate log file";

        public static string LabelInterfaceLanguage = "Interface language ";
        public static string LabelDisableDriver = "Disable native drivers (not recommended)";
        public static string LabelTheme = "Theme ";
        public static string LabelGeneral = "General";
        public static string LabelPreview = "Preview";
        public static string LabelPlaySound = "Play sound after capture";
        public static string LabelAutoPreview = "Auto preview";
        public static string LabelPreviewAfterCapture = "Preview image after capture in fullscreen";
        public static string LabelPreviewTime = "Preview time in sec  ";
        public static string LabelPreviewInLIveView = "Preview image after shot in live view";
        public static string LabelLIveViewImageFreeze = "Live view image freeze in sec  ";
        public static string LabelRotateImage = "Rotate image ";
        public static string LabelDontLoadThumbs = "Don't load thumbnails";
        public static string LabelFullscrenBack = "Full screen window background  ";
        public static string LabelTriggers = "Triggers";
        public static string LabelUseKeybordToTrigger = "Use keyboard to trigger take photo";
        public static string LabelWebserver = "Webserver";
        public static string LabelUseWebserver = "Use web server";
        public static string LabelFocusAndLiveview = "Live view";
        public static string LabelSmallFocusSstep = "Small focus step";
        public static string LabelMediumFocusStep = "Medium focus step";
        public static string LabelLargFocusStep = "Large focus step";
        public static string ReStartToolTip = "You need to restart the application for these changes to take affect";

        public static string LabelSessonName = "Session name ";
        public static string LabelFolder = "Folder ";
        public static string LabelFileNameTemplate = "File Name Template ";

        public static string LabelCounter = "Counter";
        public static string LabelUseOriginal = "Use original filename given by the camera";
        public static string LabelUseOriginalToolTip = "This option doesn’t work with WIA drivers";
        public static string LabelDontDownloadPhotos = "Don't download photos to PC";
        public static string LabelAlowFolderChange = "Allow folder change using image browser";
        public static string LabelTags = "Tags";

        public static string LabelManualFocus = "Manual Focus";
        public static string LabelGrid = "Grid";
        public static string LabelLvZoomRation = "Lv zoom ratio";
        public static string LabelFocusStacking = "Focus Stacking";
        public static string LabelNoOfPhots = "No. of photos";
        public static string LabelFocusStep = "Focus step";
        public static string LabelLuminosity = "Luminosity";

        public static string LabelExposureBracketing = "Exposure bracketing";

        public static string LabelExposureBracketingToolTip =
            "Set camera to Aperture mode and turn off Auto ISO and Auto White Balance for most reliable results.";

        public static string LabelManualExposureBracketing = "Manual Exposure bracketing";

        public static string LabelManualExposureBracketingToolTip =
            "Set camera to Manual (M) mode and turn off Auto ISO and Auto White Balance for most reliable results.";

        public static string LabelPresetBracketing = "Preset bracketing";

        public static string LabelTagDisplayValue = "Display Value";
        public static string LabelTagValue = "Value";
        public static string LabelTagIncludeInGroup1 = "Include in group 1";
        public static string LabelTagIncludeInGroup2 = "Include in group 2";
        public static string LabelTagIncludeInGroup3 = "Include in group 3";
        public static string LabelTagIncludeInGroup4 = "Include in group 4";

        public static string LabelDelay = "Delay (msec)";
        public static string LabelNumberOfCaptures = "Number of captures";
        public static string LabelWaitTime = "Wait time (sec)";
        public static string LabelDisableAutofocus = "Disable autofocus";
        public static string LabelConnectedCameras = "Connected cameras";

        public static string LabelPresetName = "Preset name";

        public static string LabelTimeLapse = "TimeLapse";
        public static string LabelSecondsBetweenShots = "Seconds between shots";
        public static string LabelNumberOfPhotos = "Number of photos";
        public static string LabelDontAutofocusOnEveryCapture = "Don't autofocus on every capture";
        public static string LabelCaptureInterval = "Capture interval (HHMMSS)";
        public static string LabelMovieLenght = "Video length (HHMMSS)";
        public static string LabelMovieSettings = "Video settings";
        public static string LabelVideoFormat = "Video format";
        public static string LabelOutputFile = "Output file";
        public static string LabelFps = "Fps";
        public static string LabelFillImage = "Fill image";
        public static string LabelCreateMovie = "Create video";
        public static string LabelAddVirtualMovingToVideo = "Add virtual moving to video";
        public static string LabelMovingSurface = "Moving surface (%)";
        public static string LabelMovingDirection = "Moving direction";
        public static string LabelMovingDirectionLeftToRight = "Left to Right";
        public static string LabelMovingDirectionRightToLeft = "Right to Left";
        public static string LabelMovingDirectionTopToBottom = "Top to Bottom";
        public static string LabelMovingDirectionBottomToTop = "Bottom to Top";
        public static string LabelMovingDirectionLeftTopToRightBottom = "Left/Top to Right/Bottom";
        public static string LabelMovingDirectionRightBottomToLeftTop = "Right/Bottom to Left/Top";
        public static string LabelImageAlignment = "Image alignment";
        public static string LabelImageAlignmentLeftTop = "Left/Top";
        public static string LabelImageAlignmentCenter = "Center";
        public static string LabelImageAlignmentRightBottom = "Right/Bottom";
        public static string LabelVideoCodecProblem = "Video codec problem";

        public static string MsgPhotoTransferBegin = "Transferring captured image...";
        public static string MsgPhotoTransferDone = "Photo transfer done";
        public static string MsgPhotoTransferError = "Transfer error !\nMessage {0}";
        public static string MsgBulbModeNotSupported = "Bulb mode not supported !";
        public static string MsgApplicationUpToDate = "Your application is up to date !";
        public static string MsgUseSessionEditorTags = "Use session editor to define tags !";
        public static string MsgLastSessionCantBeDeleted = "Last session can't be deleted";
        public static string MsgStopTimeLapse = "The time lapse has not finished! Do you want to stop the time lapse ?";

        public static string MsgInstallXvidCodec =
            "Xvid codec not installed !\nDo you want to download and install it ? ";

        public static string MsgDeleteSessionQuestion =
            "Do you want to continue to delete the session {0} ?\nNo files will be deleted !";

        public static string MsgBracketingDone = "Bracketing done";
        public static string MsgActionInProgress = "Action in progress {0}/{1}";

        public static string MenuFile = "_File";
        public static string MenuReScan = "Rescan connected cameras";
        public static string MenuConnectedCameras = "Connected cameras";
        public static string MenuExit = "Exit";
        public static string MenuView = "_View";
        public static string MenuMultipleCamera = "Multiple camera control";
        public static string MenuSelectedCameraProperty = "Selected camera property";
        public static string MenuPresets = "Presets";
        public static string MenuPresetsLoad = "Load preset in camera";
        public static string MenuPresetsSave = "Save";
        public static string MenuPresetsEdit = "Edit";
        public static string MenuHelp = "_Help";
        public static string MenuHomePage = "Home page";
        public static string MenuCheckForUpdate = "Check for update";
        public static string MenuDonate = "Donate";
        public static string MenuAbout = "About";
        public static string MenuLayout = "Layout";
        public static string MenuLayoutDefault = "Default";
        public static string MenuLayoutNormal = "Normal";
        public static string MenuLayoutGridRight = "Grid right";
        public static string MenuLayoutGrid = "Grid";

        public static string MenuProperties = "Properties";
        public static string MenuUseSelectedCameraPreset = "Use selected camera preset";

        // 15/10/2012
        public static string LabelExternalViewerPath = "Context menu external viewer";
        public static string LabelStayOnTop = "Always on top";
        // 22/10/2012
        public static string LabelDisplay = "Display";
        public static string LabelMotionDetection = "Motion detection";
        public static string LabelActivateMotionDetection = "Activate motion detection";
        public static string LabelCaptureWhenMove = "Capture when motion detected";
        public static string LabelThreshold = "Threshold (%)";
        public static string LabelWaitMoion = "Wait (sec)";
        // 25/10/2012
        public static string LabelAutofocusBeforeCapture = "Autofocus before capture";
        // add 26/10/2012
        public static string LabelOpacity = "Opacity";
        // 29/10/2012
        public static string LabelMotionDetectionType = "Motion detection type";
        public static string LabelTwoFramesDifferenceDetector = "Two Frames Difference Detector";
        public static string LabelSimpleBackgroundModelingDetector = "Simple Background Modeling Detector";
        public static string LabelSmallestBlockSize = "Smallest block size";
        public static string ButtonHelp = "Help";
        // 31/10/2012
        public static string LabelMenu = "Menu";
        // 03/11/2012
        public static string MenuForum = "Forum";
        // 07/11/2012
        public static string ButtonGetRawCodec = "Get raw codec";
        // 08/11/2012
        public static string LabelErrorRecordMovie = "Error when starting video recording";
        public static string LabelNoCardInserted = "No card inserted";
        public static string LabelCardError = "Card Error";
        public static string LabelCardNotFormatted = "Card not formatted";
        public static string LabelNoFreeAreaInCard = "No free storage space left on the card";

        public static string LabelCardBufferNotEmpty =
            "The buffer still contains data to be stored to the card.";

        public static string LabelPcBufferNotEmpty =
            "The buffer still contains data to be transferred to the PC.";

        public static string LabelBufferNotEmpty = "There is video data in the buffer. ";
        public static string LabelRecordInProgres = "During video file recording";
        public static string LabelCardProtected = "Card protected";
        public static string LabelDuringEnlargedDisplayLiveView = "During enlarged display of Live view";
        public static string LabelWrongLiveViewType = "The live view selector is set to live view photography.";
        public static string LabelNotInApplicationMode = "The camera is not in the application mode.";
        public static string ButtonResetSettings = "Reset settings";
        // 14/11/2012
        public static string LabelUseExternalViewer = "Use external viewer";
        public static string LabelExternalPreViewerPath = "External viewer path";
        public static string LabelExternalPreViewerArgs = "External viewer arguments";
        // 18/11/2012
        public static string LabelControl = "Control";
        // 20/11/2012
        public static string LabelExport = "Export";
        // 21/11/2012
        public static string LabelDownload = "Download";
        // 25/11/2012
        public static string DownloadWindowTitle = "Download photos";
        public static string LabelDeleteFilesAfterTransfer = "Delete files after transfer";
        public static string LabelAskForDelete = "Transferred files will be deleted.\nDo you want to continue ?";
        // 26/11/2012
        public static string LabelErrorSetFocusPos = "Error when setting focus position";
        public static string LabelErrorUnableFocus = "Unable to focus";
        // 28/11/2012
        public static string LabelErrorLoadingFileList = "Error loading file list";
        // 30/11/2012
        public static string LabelTransfer = "Transfer";
        public static string LabelTransferItem1 = "1.Save to PC only";
        public static string LabelTransferItem2 = "2.Save to camera only";
        public static string LabelTransferItem3 = "3.Save to PC and camera";
        // 05/12/2012
        public static string MsgDisabledDrivers = "Native drivers are disabled! Do you want to enable them?";
        // 14/12/2012
        public static string LabelShowFocusPoints = "Show focus points";
        public static string LabelMainWindow = "Main window";
        public static string LabelSelect = "Select";
        public static string TimeSelectWindowTitle = "Select window";
        // 16/12/2012
        public static string LabelRotateNone = "None";
        public static string LabelRotate90 = "Rotate 90";
        public static string LabelRotate180 = "Rotate 180";
        public static string LabelRotate270 = "Rotate 270";
        // 04/01/2013
        public static string LabelRawCodecNotInstalled = "Raw codec not installed or unknown file format";
        // 05/01/2013
        public static string LabelUseCameraCounter = "Use camera counter instead of generic counter";
        public static string LabelResetCounters = "Reset counters";
        // 07/01/2013
        public static string LabelUnabletoDeleteSession = "Unable to delete session";
        // 12/01/2013
        public static string LabelWaitingForDevice = "Waiting for device to be ready";
        public static string LabelAdvanced = "Advanced";
        public static string LabelUseParellelTransfer = "Use parallel file transfer";
        // 19/01/2013
        public static string LabelUnHandledError =
            "An application error occurred.\nPlease check whether your data is correct and repeat the action. If this message appears again, this indicates a serious malfunction in the application, so we advise you to close it.\n\nError:{0}\n\nDo you want to continue?\n(if you click Yes, you will continue with your work, if you click No, the application will close)";

        public static string LabelApplicationError = "Application Error";
        public static string LabelWiaNotInstalled = "WIA 2.0 not installed";
        public static string LabelRestartTheApplication = "Restart the application !";
        public static string LabelOutOfMemory = "The application ran out of memory and will shut down!";
        public static string LabelApplicationAlreadyRunning = "Application already running";
        public static string LabelDeleteSession = "Delete session";
        public static string LabelStartCapture = "Start Capture";
        public static string LabelStopCapture = "Stop Capture";
        public static string LabelCaptureTime = "Capture time (sec)";
        public static string LabelTimeBetweenShots = "Time between shots";
        public static string LabelDontAutofocus = "Don't autofocus";
        public static string LabelShowUnTranslatedStringIds = "Show untranslated string ids";
        // 20/01/2013
        public static string LabelUseAsMaster = "Use as master";
        // 29/01/2013
        public static string LabelCounterIncrementer = "Counter incrementer";
        public static string LabelSelected = "Selected";
        // 31/01/2013
        public static string LabelNone = "None";
        public static string LabelRuleOfThirds = "Rule of thirds";
        public static string LabelComboGrid = "Grid";
        public static string LabelDiagonal = "Diagonal";
        public static string LabelSplit = "Split";
        // 13/03/2013
        public static string LabelTools = "Tools";
        // 17/03/2012
        public static string LabelMFError = "Camera is in MF focus mode";
        public static string LabelNotAFSError = "Camera isn't in AF-S focus mode";
        // 28/03/2012
        public static string LabelSequenceError = "Sequence error";
        public static string LabelFullyPressedButtonError = "Fully pressed button error";
        public static string LabelApertureValueError = "The aperture value is being set by the lens aperture ring.";
        public static string LabelBulbError = "Bulb error";
        public static string LabelDuringCleaningMirror = "During cleaning mirror-up operation";
        public static string LabelDuringInsufficiencyBattery = "During insufficiency of battery";
        public static string LabelTTLError = "TTL error";
        public static string LabelNonCPULEnseError = "A non-CPU lens is mounted and the exposure mode is not M.";
        public static string LabelImageInRAM = "There is an image whose recording destination is SDRAM.";

        public static string LabelNoCardInsertedError =
            "The recording destination is the card or the card and SDRAM,\n and the card is not inserted with the release disabled without a card.";

        public static string LabelCommandProcesingError = "During processing by the shooting command";
        public static string LabelShoutingInProgress = "The shooting mode is EFFECTS.";
        public static string LabelOverHeatedError = "Live view cannot be started because the camera sensor temperature is too high.";
        public static string LabelCardProtectedError = "Card protected";

        public static string LabelMirrorUpError =
            "The release mode is set to Mirror-up and the mirror-up operation is being performed.";

        public static string LabelMirrorUpError2 = "The release mode is [Mirror-up].";
        public static string LabelDestinationCardError = "The recording destination is the card.";
        public static string LabelLiveViewError = "Error starting live view ";
        //07/05/2013
        public static string LabelDevices = "Devices";
        public static string LabelExternalShutterRelease = "External shutter release";
        public static string LabelConfigName = "Configuration name";
        public static string LabelDriver = "Driver";
        public static string LabelAstronomy = "Astronomy";
        public static string LabelEnable = "Enable";
        public static string LabelUseConfiguration = "Use configuration";
        public static string LabelNoExternalDeviceSelected = "No external device is selected";
        public static string LabelScript = "Script";
        public static string LabelStartScript = "Start Script";
        public static string LabelStopScript = "Stop Script";
        public static string LabelDeviceSettings = "Device settings";
        public static string LabelAvailableDevices = "Available devices";
        public static string LabelCommands = "Commands";
        public static string LabelStartLiveView = "Start Live View";
        public static string LabelStopLiveView = "Stop Live View";
        public static string LabelBrightness = "Brightness";
        public static string LabelUseExternalShutterRelease = "Use external shutter release";
        public static string LabelDownloadJpgOnly = "Download Jpg file only";
        public static string LabelDownloadJpgOnlyToolTip = "Only active if photos are saved on camera card too";
        public static string LabelSetCounters = "Set Counters";
        public static string LabelEdgeDetection = "Edge detection";
        public static string LabelLeadingZeros = "Counter leading zeros";
        //04/07/2013
        public static string LabelShowFocusRect = "Show focus rectangle";
        public static string LabelEasyLiveViewControl = "Easy live view control";
        public static string LabelExif = "Exif";
        public static string LabelImageProperties = "Image Properties";
        public static string LabelFileName = "File Name";
        public static string LabelSet = "Set";
        public static string LabelTagSelector = "Tag selector";
        public static string LabelComment = "Comment";
        public static string LabelWriteComments = "Write comments/tags to downloaded image file";
        public static string LabelStayOnTop2 = "Stay on top";
        public static string LabelWriteSelectedTags = "Write tags from tag selector";
        public static string LabelClearCache = "Clear cache";
        public static string LabelShowMagnifierInFullSccreen = "Show magnifier in full screen";
        public static string LabelDelayImageLoading = "Delay image loading";
        public static string LabelShowOverlayFolder = "Show overlay folder";
        public static string LabelPHDGuiding = "PHD Guiding";
        public static string LabelWaitPHDGuiding = "Wait for PHD (sec)";
        public static string LabelCountDown = "Countdown";
        public static string LabelCurrentEvent = "Current event";
        public static string LabelRemainingCaptures = "Captures left";
        public static string LabelLiveViewRotation = "Live view rotation";
        public static string LabelAutomaticallyGuiding = "Automatic PHD Guiding after photo capture";
        public static string LabelAutoRotate = "Auto rotate";
        public static string LabelReset = "Reset";
        public static string LabelFocus = "Focus";
        //17/11/2013
        public static string LabelAddFakeCamera = "Add a fake camera";
        public static string LabelBarcode = "Barcode";
        public static string LabelSendTheLogFile = "Send the log file";
        public static string LabelAskSendLogFile = "Do you want to send the log file to the developers?";
        public static string LabelSyncCameraDate = "Sync camera date/time on camera connect";
        public static string LabelShowThumbUpDown = "Show thumb up/down buttons";
        public static string LabelSend = "Send";
        public static string LabelLogMessage = "Message to the developers of digiCamControl ";

        public static string LabelLogText =
            "The log file will be sent to the digiCamcontrol developer team for improved application stability.";

        public static string LabelFullscreen = "Full screen";
        public static string LabelSelection = "Selection";
        public static string LabelSelectAll = "Select All";
        public static string LabelSelectNone = "Select None";
        public static string LabelSelectLiked = "Select Liked";
        public static string LabelSelectUnLiked = "Select UnLiked";
        //19/01/2014
        public static string LabelLockNearFar = "Please lock nerest and farest focus points";
        public static string LabelError = "Error";
        public static string LabelChangelog = "Change Log";
        //27/02/2014
        public static string LabelAllowOverWrite = "Allow overwrite";
        public static string LabelShowRuler = "Show Ruler";
        public static string LabelAutoPreviewJpgOnly = "Auto Preview Jpg Only";
        public static string LabelSettingsLiveView = "Live view";
        public static string LabelHostMode = "Host mode";
        //17.05.2014
        public static string LabelClear = "Clear";
        public static string LabelHistogram = "Histogram";
        public static string LabelMetadata = "Metadata";
        public static string LabelFullSize = "Full size image";
        public static string LabelWifi = "Connect with DSLRDASHBOARDSERVER";
        public static string ButtonRecordStopMovie = "Stop video recording";
        public static string LabelOverlay = "Overlay";
        public static string LabelGenerateThumbs = "Generate Thumbnails";
        public static string LabelPlay = "Play";
        public static string LabelPause = "Pause";
        public static string LabelImageSequencer = "Image Sequencer";
        public static string LabelLoop = "Loop";
        public static string LabelProgress = "Progress";
        public static string LabelStartImageFrame = "Start image frame";
        public static string LabelStopImageFrame = "Stop image frame";
        public static string LabelPlayVideo = "Play Video";
        public static string LabelLoadPresetAllCamera = "Load preset in all cameras";
        public static string LabelBackUpFolder = "Backup folder";
        public static string LabelBackUp = "Backup photos";
        public static string LabelVerifyPreset = "Verify preset";
        public static string LabelRefreshSession = "Refresh file list";
        public static string LabelErrorEmail = "Your email address (if you want to get feedback)";
        public static string LabelApertureBracketing = "Aperture bracketing ";
        //20/08/2014
        public static string LabelCaptureName = "Capture Name";
        public static string LabelSeries = "Series";
        public static string LabelAdvancedProperties = "Advanced Properties";
        public static string LabelDirection = "Direction";
        public static string LabelFarNear = "Far -> Near";
        public static string LabelNearFar = "Near -> Far";
        public static string LabelStepSmall = "Small step";
        public static string LabelStepMedium = "Medium step";
        public static string LabelStepLarge = "Large step";
        public static string LabelSessionOpenFolder = "Open session folder";
        public static string LabelActivate = "Activate";
        public static string LabelScaling = "Scaling";
        public static string LabelHorizontal = "Horizontal";
        public static string LabelVertical = "Vertical";
        //20/08/2014
        public static string LabelLockNear = "Lock nearest point";
        public static string LabelLockFar = "Lock farthest point";
        public static string LabelGoNear = "Go to nearest focusing point";
        public static string LabelGoFar = "Go to farthest focusing point";
        public static string LabelMoveSmallToCamera = "Move small step to camera";
        public static string LabelMoveMediumToCamera = "Move medium step to camera";
        public static string LabelMoveLargeToCamera = "Move large step to camera";
        public static string LabelMoveSmallToInfinity = "Move small step to infinity";
        public static string LabelMoveMediumToInfinity = "Move medium step to infinity";
        public static string LabelMoveLargeToInfinity = "Move large step to infinity";
        public static string LabelDistantFromPoints = "Distance between nearest and farthest locked focus points";
        public static string LabelFocusStackingSimple = "Focus Stacking Simple";
        public static string LabelFocusStackingAdvanced = "Focus Stacking Advanced";
        public static string LabelErrorFarPoit = "First set the farthest focus point";
        public static string LabelErrorAutoFocusLock = "The focus is locked, unlock first to autofocus";
        public static string LabelErrorSimpleStackingFocusLock = "The focus is locked. Simple focus stacking does not require focus lock, unlock it first to start.";
        public static string LabelLowerCaseExtension = "Lower case extension";
        public static string LabelOverlayTransparency = "Transparency";
        public static string LabelOverlayUseLastCaptured = "Use last captured";
        public static string LabelMinimizeToTryIcon = "Minimize to tray icon";
        public static string LabelStartMinimized = "Start minimized";
        public static string LabelStartupWithWindows = "Start when Windows starts";
        public static string LabelSelectSeries = "Select photos in same series";
        public static string LabelCaptureDelay = "Capture delay";
        public static string LabelAutoExportPlugins = "Auto Export Plugins";
        public static string LabelAddPlugin = "Add plugin";
        public static string LabelErrorSetFolder = "Error set folder";
        public static string LabelDetectArea = "Detect only in ruler area";
        public static string LabelMessagesLog = "Message log";
        public static string LabelRefreshCameraList = "Refresh camera list";
        public static string LabelSortOrder = "Sort order";
        public static string LabelSaveOrder = "Save sort order";
        public static string LabelLoadThumbDownload = "Load thumbs in download window";
        public static string LabelFormatAllCards = "Format all cards";
        public static string LabelPrint = "Print";
        public static string LabelPrintSetup = "Print setup";
        public static string LabelPageSetup = "Page setup";
        public static string LabelMarginBetweenImages = "Margin";
        public static string LabelRotate = "Rotate";
        public static string LabelRepeatImages = "Repeat images";
        public static string LabelWaitBetweenSteps = "Wait between steps (ms)";
        public static string LabelShowSecondary  = "Show on secondary monitor";
        // 20/12/2014
        public static string LabelKeyboardTrigger = "Keyboard trigger";
        public static string LabelNoProccesing = "No procesing";
        public static string LabelNoProccesingTooltip = "Most of the functionality is disabled";
        public static string LabelSendUsage = "Automatically send usage statistics";
        public static string LabelSheduleStart = "Start schedule";
        public static string LabelSheduleStop = "Stop schedule";
        public static string LabelTimeLapseSettings = "Timelapse settings";
        public static string LabelTimeLapseNow = "Start now";
        public static string LabelTimeLapseIn = "Start in";
        public static string LabelTimeLapseAt = "Start at";
        public static string LabelTimeLapseDaily = "Start Daily";
        public static string LabelDate = "Date";
        public static string LabelTime = "Time";
        public static string LabelSunday = "Sunday";
        public static string LabelMonday = "Monday";
        public static string LabelTuesday = "Tuesday";
        public static string LabelWednesday = "Wednesday";
        public static string LabelThursday = "Thursday";
        public static string LabelFriday = "Friday";
        public static string LabelSaturday = "Saturday";
        public static string LabelTimeLapseStop = "Stop ";
        public static string LabelTimeLapseStopIn = "Stop in";
        public static string LabelTimeLapseStopAt = "Stop at";
        public static string LabelTimeLapseStopPhotoCount = "After capture count";
        public static string LabelTimeLapseWaitBetweenCaptures = "Time to wait between captures (sec)";
        public static string LabelTimeLapseCapture = "Capture with selected camera";
        public static string LabelTimeLapseCaptureAll = "Capture with all cameras";
        public static string LabelTimeLapseCaptureScript = "Execute script";
        public static string LabelTimeLapseScriptFile = "Script file";
        public static string LabelBrowse = "Browse";
        public static string LabelFlip = "Flip horizontally";
        public static string LabelLiveViewPreviewTime = "Preview captured photo time";
        public static string LabelColor = "Color";
        public static string LabelImportData = "Import Data";
        public static string LabelExportData = "Export Data";
        public static string LabelData = "Data";
        public static string LabelScanBarcode = "Scan Barcode";
        public static string LabelCaptureAfterScan = "Capture after scan";
        public static string LabelKeepWindowActive = "Keep window active";
        public static string LabelDeleteFiles = "Delete selected files";
        public static string LabelShowInExplorer = "Show in explorer";
        public static string LabelOpenInExternalViewer = "Open in external viewer";
        public static string LabelRestoreFromBackup = "Restore from backup copy";
        // 11/03/2015
        public static string LabelAllowWebCommands = "Allow interaction via webserver";
        public static string LabelCurrentMotion = "Detected motion";
        public static string LabelStartupScript = "Execute script on startup";
        public static string LabelPublicWebserver = "Allow public access";
        public static string LabelLoadCanonTransferMode = "Load Canon transfer mode";
        public static string LabelFolderSelector = "Folder selector";
        public static string LabelReloadOnFolderChange = "Reload files on folder change";
        public static string MenuSession = "Session";
        public static string MenuCamera = "Camera";
        public static string MenuThumbnailSize = "Thumbnail Size";
        public static string MenuInver = "Invert selection";
        public static string MenuResetDevice = "Reset device";
        public static string MenuSessionAdd = "Add new session ...";
        public static string MenuSessionEdit = "Edit current session ...";
        public static string MenuSessionDel = "Remove current session";
        public static string MenuIncrementSeries = "Increment series";
        public static string MenuWindows = "Windows";
        public static string MenuOnlineManual = "Online manual";
        public static string MenuPlugins = "Plugins";
        public static string LabelAutorotate = "Auto rotate image";
        public static string MenuCameraProperty = "Camera property";
        public static string LabelShowThumbInfo = "Show info next to thumbnail";
        public static string MenuLayoutPortrait = "Portrait";
        public static string MenuLayoutLandscape = "Landscape";
        public static string MenuExportSession = "Export session ...";
        public static string MenuImportSession = "Import session ...";
        public static string LabelFullSpeed = "Full speed";
        // 10/08/2015
        public static string LabelPreviewWindows = "Preview Windows";
        public static string LabelTransformed = "Transformed";
        public static string LabelOriginal = "Original";
        public static string LabelPluginProperties = "Properties";
        public static string LabelPluginTransformPlugins = "Transform plugins";
        public static string LabelConfigurePlugin = "Configure Plugin";
        // 12/09/2015
        public static string LabelEmailPublicWebAddress = "Email public web address";
        public static string LabelBracketingMode = "Bracketing mode";
        public static string LabelDepthOfFieldBracketing = "Depth-of-field bracketing";
        public static string LabelWhiteBalanceBracketing = "White balance bracketing";
        public static string LabelIsoBracketing = "Iso bracketing";
        public static string LabelBracketingLow = "Low value";
        public static string LabelBracketingHigh = "High value";
        public static string LabelBracketingNumberOfShots = "Number Of shots";
        public static string LabelNoLowValueError = "No low value is set";
        public static string LabelNoHighValueError = "No high value is set";
        public static string LabelWrongValue = "Wrong low or high value";
        public static string LabelWrongFNumber = "Aperture cannot be modified";
        public static string LabelBracketingMMode = "Set camera to M mode";
        public static string LabelTimeLapseCaptureBracketing = "Capture with bracketing";
        public static string LabelZoomToFocus = "Always zoom to first focus point";
        public static string LabelPreviewFullSize = "Preview full size photo";
        public static string LabelCropMargins = "Crop margins (%)";
        public static string LabelShowActiveArea = "Show active area";
        public static string LabelSetArea = "Set";
        public static string LabelDoneArea = "Done";
        public static string LabelRotateCounterclockwise = "Rotate counterclockwise";
        public static string LabelRotateClockwise = "Rotate clockwise";
        public static string LabelSaveJpg = "Save as JPG";
        public static string LabelPageWidth = "Page width";
        public static string LabelPageHeigh = "Page height";
        public static string LabelFill = "Fill";
        // 19/10/2015
        public static string LabelApplyToImage = "Apply to selected image";
        public static string LabelCaptureCount = "Capture Count";
        public static string LabelAutoFocusBeforCapture = "Autofocus before capture";
        public static string LabelEnhancedThumbs = "Enhanced Thumbs";
        public static string LabelAskSavePath = "Ask for captured file path";
        public static string LabelSaveLiveviewWindow = "Save live view window position";
        public static string LabelShowFullscreenControls = "Show fullscreen window controls";
        public static string LabelCurrentLockNear = "Lock current position as nearest point";
        public static string LabelCurrentLockFar = "Lock current position as farthest point";
        public static string LabelStatistics = "Statistics";
        public static string LabelRefresh = "Refresh";
        public static string LabelOverview = "Overview";
        public static string LabelItems = "Items";
        //30/01/2016
        public static string LabelDownloadThumb = "Download photo thumb only";
        public static string LabelDeleteFileAfterTransfer = "Delete file after transfer";
        public static string LabelHideTrayNotifications = "Hide tray bar notifications";
        //26/03/2016
        public static string LabelConditions = "Conditions";
        public static string LabelVariable = "Variable";
        public static string LabelCondition = "Condition";
        public static string LabelOperator = "Operator";
        public static string LabelValue = "Value";
        public static string LabelDelete = "Delete";
        public static string LabelDisableHardwareAcceleratione = "Disable Hardware Acceleration";
        public static string LabelDisabled = "Disabled";
        public static string LabelCapturePhoto = "Capture photo";
        public static string LabelRecordVideo = "Record video";
        public static string LabelVideoLength = "Video length";
        public static string LabelRestore = "Restore";
        public static string LabelTrayMessage = "Application was minimized \n Double click to restore!";
        public static string LabelExecuteAfterTransfer = "Execute after file was transferred ";
        public static string LabelInvert = "Invert";
        // 05/07/2016
        public static string LabelClosePreview = "Close Preview";
        public static string LabelReload = "Reload";
        public static string LabelSeriesAsIndex = "Save index as series";
        public static string LabelStackingFinished = "Focus stacking finished";

        // 01/09/2016
        public static string LabelOpenLightroom = "Open files in Adobe Lightroom";
        public static string LabelOpenPhotoshop = "Open files in Adobe Photoshop";
        public static string LabelPasswordOnClose = "Ask password on close";
        public static string LabelList = "List";
        public static string LabelCameraname = "Camera name";
        public static string LabelSerialNumber = "Serial number";
        public static string LabelResetCounterOnSeriesChange = "Reset counter on series change";

        // 04/05/2014
        public static string LabelSnapshot = "Snapshot";
        public static string LabelCaptureSnapshot = "Capture Snapshot";
        public static string LabelWaitForCapture = "Wait for Capture (msec)";
        public static string LabelWaitForFocus = "Wait for Focus (msec)";
        public static string LabelGridColor = "Grid Color";
        public static string LabelWebserverPort = "Webserver Port";
        public static string LabelWebcameraSupport = "Webcamera Support";
        public static string LabelContrast = "Contrast";

        public static string LabelZoom = "Zoom";
        public static string LabelDefaultSession = "Default Session";
        public static string LabelDefaultPreset = "Default Preset";
        public static string LabelWiaDeviceSupport = "Wia device support";
        public static string LabelWiFiProvider = "WiFi Provider";
        public static string LabelIpAddreess = "Ip Address";

        // 04/11/2018
        public static string LabelShowFocusControlBar = "Show Focus Control Bar";
    }
}