# VolumeShadowCopyCollector

VolumeShadowCopyCollector was designed for all DFIR analysts who want to automate artifact collections from VSS. It is a command line tool written in C# .Net Framework and requires admin rights to run.

The tool was designed for .NET Framework version 4.6; however, it may work on earlier versions as well, but this has not been tested. A key dependency is a package named "System.IO.Compression.ZipFile" that is supported by version 4.3.0 and newer, therefore it will not work properly on versions prior to 4.3.0.

Based on the following article Windows 10 (all editions) includes the .NET Framework 4.6 as an OS component, and it is installed by default. Therefore, based upon this, the tool should work on all current Windows 10 systems.

- Windows 10 1507 (all editions) includes the .NET Framework 4.6.0
- Windows 10 1511 November 2015 Update (all editions) includes the .NET Framework 4.6.1
- Windows 10 1607 Anniversary Update (all editions) includes the .NET Framework 4.6.2
- Windows 10 1703 Creators Update (all editions) includes the .NET Framework 4.7
- Windows 10 1709 Fall 2017 Creators Update (all editions) includes the .NET Framework 4.7.1
- Windows 10 1803 April 2018 Update (all editions) includes the .NET Framework 4.7.2
- Windows 10 1903 May 2019 Update (all editions) includes the .NET Framework 4.8

The tool uses symbolic links to gain access to Volume Shadows Copies, this means that artifacts such as $MFT and $USNJrnl are not accessible with this tool. The tool will create a symbolic link, collect the necessary artifacts, and then saves them to a new directory. This is then packed to a ZIP format for ease of transfer. Once the collection is packed, the previously created symbolic links and folders are removed.

If administrator rights are missing the tool will throw an error:
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Error.png?raw=true)

If the tool is launched with the admin rights, it will start collecting the necessary files:
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Admin_rights_1.png?raw=true)
  
When all files are collected it packs the collection, and removes all created files and folders.
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Admin_rights_2.png?raw=true)

For each VSS detected on the system, the tool creates a separate ZIP file for each collection.
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Collections.PNG?raw=true)

In addition to the collected files, inside the ZIP file you can find a log.txt file. As the name suggests, this file contains all logs demonstrating which files were collected, and which were not.

#Updates
The current version of the tool conly ollects files from the ReccleBin that are smaller than 100MB. I discovered that otherwise the collection can be extremaly big. 
