echo This script should be loaded by importing the project 'External - Webservice.dccproject.zip'
echo Ensure that the button, external services and script window are visible
echo "Ensure that the webservice is running: start Webservice\ScriptExternalObjectPluginRestService.exe"
echo "Open a browser and visit http://Serial.digiCamControl.local/ to view and change the state of the service"
dcc do UIButtonWnd_Show
dcc do ExternalServicesWnd_Show
dcc do ScriptWnd_Show

echo Ensure that the webservice is activated as external service (via script)
dcc external my_mcu_via_wifi activate

echo Details about the web service:
echo [ dcc list external my_mcu_via_wifi ]

echo The button window shows a button for the PressButton trigger - try it!

#  Note that the names of properties are case sensitive
# Use longer interval to check the service - a MCU is not that fast
dcc external my_mcu_via_wifi use EventRaised 2000

set count 0
set idle 0
while true {
	set value [ dcc external my_mcu_via_wifi  get BooleanValue ]
	echo BooleanValue is: $value
	set value [expr { !$value }]
	dcc external my_mcu_via_wifi set BooleanValue $value

	set value [ dcc external my_mcu_via_wifi  get LongValue ]
	echo LongValue is: $value
	incr value
	dcc external my_mcu_via_wifi set LongValue $value

	set svalue [ dcc external my_mcu_via_wifi  get StringValue ]
	echo StringValue is: $svalue
	set svalue "The LongValue is $value"
	dcc external my_mcu_via_wifi set StringValue $svalue

	set value [ dcc external my_mcu_via_wifi  get EnumValue ]
	echo EnumValue is: $value
	if { $value == "One" } {
		set value "Two"
	} elseif { $value == "Two" } {
		set value "Three"
	} else  {
		set value "One"
	}
	dcc external my_mcu_via_wifi set EnumValue $value

	set value [ dcc external my_mcu_via_wifi  get ReadOnlyValue ]
	echo ReadOnlyValue is: $value

	set value "The event EventRaised has not been raised in the last $idle seconds"
	dcc external my_mcu_via_wifi  set WriteOnlyValue $value

	set raised [ dcc external my_mcu_via_wifi wait EventRaised 10000 ]
 	if {$raised} {
 		incr count
		echo  The event EventRaised has been raised $count times
		set idle 0
	} else {
		incr idle 10
		echo  The event EventRaised has not been raised in the last $idle seconds
	}
}
