echo This script should be loaded by importing the project 'External - Webservice.dccproject.zip'
echo Ensure that the button, external services and script window are visible
echo "Ensure that the webservice is running: start Webservice\ScriptExternalObjectPluginRestService.exe"
echo "Open a browser and visit http://localhost:5000/ to view and change the state of the service"
dcc do UIButtonWnd_Show
dcc do ExternalServicesWnd_Show
dcc do ScriptWnd_Show

echo Ensure that the webservice is activated as external service (via script)
dcc external my_web_service activate

echo Details about the web service:
echo [ dcc list external my_web_service ]

echo The button window shows a button for the PressButton trigger - try it!

#  Note that the names of properties are case sensitive
dcc external my_web_service use EventRaised

set count 0
set idle 0
while true {
	set value [ dcc external my_web_service  get BooleanValue ]
	echo BooleanValue is: $value
	set value [expr { !$value }]
	dcc external my_web_service set BooleanValue $value

	set value [ dcc external my_web_service  get LongValue ]
	echo LongValue is: $value
	incr value
	dcc external my_web_service set LongValue $value

	set svalue [ dcc external my_web_service  get StringValue ]
	echo StringValue is: $svalue
	set svalue "The LongValue is $value"
	dcc external my_web_service set StringValue $svalue

	set value [ dcc external my_web_service  get EnumValue ]
	echo EnumValue is: $value
	if { $value == "One" } {
		set value "Two"
	} elseif { $value == "Two" } {
		set value "Three"
	} else  {
		set value "One"
	}
	dcc external my_web_service set EnumValue $value

	set value [ dcc external my_web_service  get ReadOnlyValue ]
	echo ReadOnlyValue is: $value

	set value "The event EventRaised has not been raised in the last $idle seconds"
	dcc external my_web_service  set WriteOnlyValue $value

	set raised [ dcc external my_web_service wait EventRaised 5000 ]
 	if {$raised} {
 		incr count
		echo  The event EventRaised has been raised $count times
		set idle 0
	} else {
		incr idle 5
		echo  The event EventRaised has not been raised in the last $idle seconds
	}
}
