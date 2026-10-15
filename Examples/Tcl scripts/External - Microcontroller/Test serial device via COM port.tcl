echo This script should be loaded by importing the project 'External - Serial device.dccproject.zip'
echo Ensure that the button, external services and script window are visible
echo "Ensure that the device is running the Arduino sketch in 'Microcontroller' (or something equivalent)"
echo "Ensure that the device is connected to a USB/COM port"
echo "Verify that the same COM port is listed for the 'My serial device' in the external services window"
echo "If there is a mismatch, correct the COM port and re-create the registration."
dcc do UIButtonWnd_Show
dcc do ExternalServicesWnd_Show
dcc do ScriptWnd_Show

echo Ensure that the serial device is activated as external service (via script)
dcc external my_mcu_via_serial activate

echo Details about the serial device:
echo [ dcc list external my_mcu_via_serial ]

echo The button window shows a button for the PressButton trigger - try it!

#  Note that the names of properties are case sensitive
dcc external my_mcu_via_serial use EventRaised

set count 0
set idle 0
while true {
	set value [ dcc external my_mcu_via_serial  get BooleanValue ]
	echo BooleanValue is: $value
	set value [expr { !$value }]
	dcc external my_mcu_via_serial set BooleanValue $value

	set value [ dcc external my_mcu_via_serial  get LongValue ]
	echo LongValue is: $value
	incr value
	dcc external my_mcu_via_serial set LongValue $value

	set svalue [ dcc external my_mcu_via_serial  get StringValue ]
	echo StringValue is: $svalue
	set svalue "The LongValue is $value"
	dcc external my_mcu_via_serial set StringValue $svalue

	set value [ dcc external my_mcu_via_serial  get EnumValue ]
	echo EnumValue is: $value
	if { $value == "One" } {
		set value "Two"
	} elseif { $value == "Two" } {
		set value "Three"
	} else  {
		set value "One"
	}
	dcc external my_mcu_via_serial set EnumValue $value

	set value $idle
	dcc external my_mcu_via_serial  set WriteOnlyValue $value

	set value [ dcc external my_mcu_via_serial  get ReadOnlyValue ]
	echo ReadOnlyValue is: $value

	set raised [ dcc external my_mcu_via_serial wait EventRaised 5000 ]
 	if {$raised} {
 		incr count
		echo  The event EventRaised has been raised $count times
		set idle 0
	} else {
		incr idle 5
		echo  The event EventRaised has not been raised in the last $idle seconds
	}
}
