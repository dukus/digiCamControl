REM %1 = Solution Directory
REM %2 = $(ConfigurationName) Debug/Release

echo.
echo PostBuild.bat running...
echo     Solution directory..... %1
echo     Configuration name..... %2
echo.

echo Copying updated additional Files from %1CameraControl.Application...
xcopy %1CameraControl.Application %1CameraControl\Bin\%2\ /Y /E /R /I /D
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying application from %1CameraControl.Application to %1CameraControl\Bin\%2\
	echo.
	GOTO :end
)

echo.
echo Copying additional Files from %1CameraControl.Application\*.*...
xcopy %1CameraControl.Application\*.* %1CameraControl\Bin\%2\ /Y
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying application from %1CameraControl.Application\*.* %1CameraControl\Bin\%2\
	echo.
	GOTO :end
)

IF NOT EXIST %1CameraControl\Bin\%2\Plugins (
	echo.
	echo  Creating directory %1CameraControl\Bin\%2\Plugins
	MD %1CameraControl\Bin\%2\Plugins
)
rem copy %1CameraControl.Plugins\Bin\%2\CameraControl.Plugins.* %1CameraControl\Bin\%2\Plugins\ /Y

rem copy these if they exist...
echo.
echo checking %1MtpTester\Bin\%2\MtpTester.exe
IF EXIST %1MtpTester\Bin\%2\MtpTester.exe (
	echo ...copying
	copy %1MtpTester\Bin\%2\MtpTester.exe %1CameraControl\Bin\%2\ /Y 
) else (
	echo ...does not exist - skipping
)

echo.
echo checking %1DccObsPlugin\Bin\%2\DccObsPlugin.dll
IF EXIST %1DccObsPlugin\Bin\%2\DccObsPlugin.dll (
    md %1CameraControl\Bin\%2\ObsPlugin\CLRHostPlugin\   
	copy %1DccObsPlugin\Bin\%2\DccObsPlugin.dll %1CameraControl\Bin\%2\ObsPlugin\CLRHostPlugin\ /Y 
	echo ...copying
) else (
	echo ...does not exist - skipping
)

:end
echo.
echo ... PostBuild done