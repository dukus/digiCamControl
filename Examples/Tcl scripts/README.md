# Examples of tcl scripts

For individual *.tcl scripts in this directory: open the *Script execution* window, select *New Tcl script* from the *Script* menu, open the file via *File | Open...* and run the script.

For a *.dccproject.zip project in this directory: import the project via *File | Import project*, open the *Script execution* window, find the script from the *Script* menu and run it. 

## SyncEvent - single script

Demonstrates how SyncEvent can be used to synchronise a single script with moments in time.

## SyncEvent - parallel running scripts

Demonstrates how SyncEvent can be used to synchronise scripts running in parallel (e.g., for different cameras).

## External - UI button

Demonstrates how a single button can be added to the *Buttons* window, and how a script can wait for the button to be clicked.

## External - WebService

Demonstrates how a script can communicate with a REST web server. The web service must satisfy certain technical requirements.

The example uses the web service from the *Examples\ScriptExternalObjectRestService* project. If that project is published, it creates an executable in the *Examples\Tcl scripts\Webserver* directory (not part of the git repository). The *ScriptExternalObjectRestService.exe* should be run for this example; it requires .NET 10. Also open a browser for [http://localhost:5000/](http://localhost:5000/) to view the state of the web service. The web service implementation can be found in the code files in *Examples\ScriptExternalObjectRestService\Components\ScriptExternalObjectDemo*.

## External - Microcontroller

Demonstrates how a script can communicate via a serial connection with a microcontroller like an Arduino, ESP32, Raspberry Pico or similar.

To run the example, upload the Arduino sketch in the *Microcontroller* directory to the microcontroller via the Arduino IDE or similar. Keep the device connected to the PC that is running digiCamControl. After a restart the microcontroller should be visible as a COM port in the Windows device manager. The sketch assumes that the device has a built-in LED. The service exposed by the device to digiCamControl includes an event. That event is raised every time pin/GPIO 1 transitions from low to high by connecting the pin to +3.3V or 5V (depending on the device).

After importing the *External - Microcontroller.dccproject.zip* in digiCamControl, open the *External services and devices* window and verify that the COM port matches the one in the device manager. If not, correct the port and re-create the registration of the device.