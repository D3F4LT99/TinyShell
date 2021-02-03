# TinyShell
TinyShell is a tiny reverse shell written in C#

# Options
You can have a built in host/port to connect back to or pass them through the commandline:

`TinyShell.exe <host> <port>`

You can also use it as the server itself for handling connections:

`TinyShell.exe --server <port>`
  
There is basic functionality for custom commands added, simply adding "tinyshellcustom:" will trigger it.
Currently there is only one custom command, "help", which doesn't give any useful output.
