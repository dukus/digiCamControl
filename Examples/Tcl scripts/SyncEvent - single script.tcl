echo Open a Tcl script window and load this file
echo Stop all running scripts and run this script
echo No sync events are defined:
echo [ dcc list syncevents ]

echo Indicate that this script is going to wait for event 'test'
dcc syncevent test use
echo [ dcc list syncevents  ]

echo Wait for the event - there is no wait as this is the only script using the event
echo [ dcc syncevent test wait  ]
echo Return value is true as the wait was completed
echo [ dcc list syncevents ]

echo Make sure the next event must be 5 seconds after the previous.
dcc syncevent test interval 5
echo [ dcc list syncevents ]
echo Current time (not UTC):  [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]

echo Wait for the event - the wait is caused by the interval, not by waiting for another script.
echo [ dcc syncevent test wait  ]
echo Return value is true as the wait was completed
echo Current time (not UTC):  [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo [ dcc list syncevents ]

echo Make sure the next event must also be 2 minutes from now.
dcc syncevent test after 120
echo [ dcc list syncevents ]
echo Current time (not UTC):  [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]

echo Wait at most 3 seconds for the event
echo [ dcc syncevent test wait 3000  ]
echo Return value is false as the wait was completed
echo Current time (not UTC):  [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo [ dcc list syncevents ]

echo Stop using the event - no events are mentioned when the events are listed:
dcc syncevent test discard
echo [ dcc list syncevents ]
echo Script completed
