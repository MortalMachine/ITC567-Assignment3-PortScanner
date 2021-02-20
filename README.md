# ITC567-Assignment3-PortScanner
A Windows Form application that can scan given ports for a given IPv4 address or IPv4 subnet.

## How to compile and run

The application has been tested in Ubuntu 18.04. Some asynchronous bugs may occur.
The application has a bug with not displaying results when run in Windows.
Mono is required to compile the code.

Ubuntu 18.xx and higher:
```
sudo apt-get install mono-complete
sudo apt-get update
mono-csc program.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll
mono program.exe
```

Windows:
Compile in a Ubuntu machine first, then copy the program.exe file to Windows.
