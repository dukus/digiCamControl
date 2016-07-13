REM %1 = Solution Directory
REM %2 = $(ConfigurationName) Debug/Release

echo.
echo PostBuild64.bat running...
echo     Solution directory..... %1
echo     Configuration name..... %2
echo.


rem copy commandline utility
echo Copying commandline utility from %1CameraControlCmd\Bin\%2\CameraControlCmd.exe...
copy %1CameraControlCmd\Bin\%2\CameraControlCmd.exe %1CameraControl\Bin\x64\%2\ /Y
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying commandline utility from %1CameraControlCmd\Bin\%2\ to %1CameraControl\Bin\x64\%2\
	echo.
	GOTO :end
)

echo Copying updated additional Files from %1CameraControl.Application...
xcopy %1CameraControl.Application %1CameraControl\Bin\x64\%2\ /Y /E /R /I /D
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying application from %1CameraControl.Application to %1CameraControl\Bin\x64\%2\
	echo.
	GOTO :end
)

echo.
echo Copying additional Files from %1CameraControl.Application\*.*...
xcopy %1CameraControl.Application\*.* %1CameraControl\Bin\x64\%2\ /Y
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying application from %1CameraControl.Application\*.* %1CameraControl\Bin\x64\%2\
	echo.
	GOTO :end
)

IF NOT EXIST %1CameraControl\Bin\%2\Plugins (
	echo.
	echo  Creating directory %1CameraControl\Bin\%2\Plugins
	MD %1CameraControl\Bin\%2\Plugins
)

copy %1CameraControl.Plugins\Bin\x64\%2\CameraControl.Plugins.* %1CameraControl\Bin\x64\%2\Plugins\ /Y 
if NOT [%errorlevel%] == [0] (
	echo.
	echo Error copying plugins from %1CameraControl.Plugins\Bin\x64\%2\CameraControl.Plugins.* to %1CameraControl\Bin\x64\%2\Plugins\
	echo.
	GOTO :end
)

pause

:end
echo.
echo ... PostBuild64 done
