# ITC567-Assignment3-PortScanner
A Windows Form application that can scan given ports for a given IPv4 address or IPv4 subnet.

## How to compile and run

The application has been tested in Ubuntu 18.04. Some asynchronous bugs may occur.
The application has a bug with not displaying results when run in Windows.
Mono is required to compile the code.

Ubuntu 18.xx and higher:
1. Install .NET 5.0 at https://dotnet.microsoft.com/download.
(For installation on Ubuntu 18.04, see https://docs.microsoft.com/en-us/dotnet/core/install/linux-ubuntu#1804-)
2. Install Mono (Mono makes it possible to compile and run Windows Form applications on Linux):
```
sudo apt-get install mono-complete
sudo apt-get update
```
3. Compile the program with Mono's C# compiler and run the executable with Mono:
```
mono-csc program.cs -r:System.Windows.Forms.dll -r:System.Drawing.dll
mono program.exe
```

Windows:
Compile in a Ubuntu machine first, then copy the program.exe file to Windows.

## How to use the application

<img src="./images/image1.png">

The application's GUI has 5 parts: the IP Addresses box, the Ports box, the Scan button, the Progress Bar, and the Results box.

Select one of the options in the IP Addresses box (Address or Subnet).
If you've selected Address, type an IPv4 address like 192.168.1.209 in the adjacent text box.
If you've selected Subnet, type an IPv4 subnet like 192.168.1.0/24 in the adjacent text box.

Next, select one of the options in the Ports box (tcp, udp, or both).
In the adjacent text box, you can enter a single port, a comma-separated list of ports (no whitespaces), or a range of ports like 1-100.

Finally, click the Scan button. The Progress Bar is between the Scan button and the Results box. It will fill by increments to show the progress (tcp ports take 2 seconds each to scan at the longest). After the Progress Bar has filled completely, the list of open and closed ports will populate the Results box.
