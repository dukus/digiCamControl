echo Open two Tcl script windows and load this script in both.
echo Stop all running scripts if necessary
echo Start script #1 and monitor its output

echo At least 10 seconds between events for 'test'
dcc syncevent test interval 10
echo [ dcc list syncevents ]

echo This script will wait for event 'test'
dcc syncevent test use
echo [ dcc list syncevents ]

echo [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo [ dcc list syncevents ]
echo Wait #1 for event 'test'.
echo If this is script #1: there is no wait as this is the only script running and the event has not yet fired
echo If this is script #2: the script waits as script #1 is not yet waiting.
echo [ dcc syncevent test wait ]
echo [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo [ dcc list syncevents ]
echo Wait #1 completed

echo If this is script #1: start script #2 in the next few seconds
after 5000

echo [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo [ dcc list syncevents ]
echo Wait #2 for event 'test', but cancel the wait if it takes more than 11 seconds
echo If this is script #1: the wait is ended 10 seconds after wait #1 has ended
echo If this is script #2: script #1 is no longer running, the wait would end at the 'After' date/time as set by script #1
echo [dcc syncevent test wait 11000]
echo [ clock format [clock seconds] -format "%Y-%m-%dT%H:%M:%S" ]
echo Wait #2 completed
echo If this is script #1: return value is true because the wait was completed
echo If this is script #2: return value is false because the wait was cancelled

echo [ dcc list syncevents ]

echo Stop using the event and postpone the next event until now + 2 hours
echo This no longer has an effect for this script, as it is almost finished.

dcc syncevent test discard
dcc syncevent test after 120
echo [ dcc list syncevents ]
