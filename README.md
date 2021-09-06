# VolumeShadowCopyCollector

VolumeShadowCopyCollector was designed for all DFIR analysts who want to automate artifacts collection from VSS. It is a command line tool written in C# .Net Framework and requires admin rights. 

Tools was designed for .NET Framework version 4.7.2. It may work on earlier versions as well, but I did not test it. I used one specific package called System.IO.Compression.ZipFile that is supported by version 4.3.0 and newer, therefore it will not work properly on versions prior to 4.3.0. 

Based on the article https://docs.microsoft.com/en-us/archive/blogs/astebner/mailbag-what-version-of-the-net-framework-is-included-in-what-version-of-the-os Windows 10 (all editions) includes the .NET Framework 4.6 as an OS component, and it is installed by default. It means that the tool supposed to work on all Windows 10 systems without any issues.


- Windows 10 1507 (all editions) includes the .NET Framework 4.6.0
- Windows 10 1511 November 2015 Update (all editions) includes the .NET Framework 4.6.1
- Windows 10 1607 Anniversary Update (all editions) includes the .NET Framework 4.6.2
- Windows 10 1703 Creators Update (all editions) includes the .NET Framework 4.7
- Windows 10 1709 Fall 2017 Creators Update (all editions) includes the .NET Framework 4.7.1
- Windows 10 1803 April 2018 Update (all editions) includes the .NET Framework 4.7.2
- Windows 10 1903 May 2019 Update (all editions) includes the .NET Framework 4.8

The tool uses symbolic links to get access to Volume Shadows Copies, which means that artifacts like $MFT and $USNJrnl are not accessible this way. First It creates a symbolic link, collects artifacts and saves them to a new directory, which then is packed to a ZIP format. Once collection is packed, symbolic links and folders created to store collections are removed. 

If administrator rights are missing the tool will throw an error:
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Error.png?raw=true)

If the tool is launched with the admin rights, it will start collecting files.
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Admin_rights_1.png?raw=true)

When all files are collected it packs the collection and removes all created files and folders.
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Admin_rights_2.png?raw=true)

For each VSS detected on the system, the tool creatas a seperate ZIP file with collection.
![alt text](https://github.com/gajos112/VolumeShadowCopyCollector/blob/main/Images/Collections.PNG?raw=true)

In adiditon to the collected files, you will find a log.txt file in each ZIP file. That file as the name suggestes contains all logs indicaiting what files were collected and which were not.
