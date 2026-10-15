echo This script should be loaded by importing the project 'External - UI button.dccproject.zip'
echo Ensure that the button, external services and script window are visible
dcc do UIButtonWnd_Show
dcc do ExternalServicesWnd_Show
dcc do ScriptWnd_Show

echo Details about the button:
echo [ dcc list external my_button ]

dcc external my_button use IsClicked

echo Click the 'My Button' in the UI button window - that is detected by the script
set count 0
set idle 0
while true {
#  Note that the names of properties are case sensitive
	set clicked [ dcc external my_button wait IsClicked 10000 ]
 	if {$clicked} {
 		incr count
		echo  The button has been clicked $count times
		set idle 0
	} else {
		incr idle 10
		echo  The button has not been clicked in the last $idle seconds
	}
	echo [ dcc list external my_button IsClicked  ]
}
