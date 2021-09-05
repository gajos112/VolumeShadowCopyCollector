# VolumeShadowCopyCollector

VolumeShadowCopyCollector was designed for all DFIR analysts who want to automate artifacts collection from VSS. It is a command line tool written in C# .Net and requires admin rights. 

Tools was designed for .NET Framework version 4.7.2. It may work on earlier versions as well, but I did not test it. I used one specific package called System.IO.Compression.ZipFile that is supported by version 4.3.0 and newer, therefore it will not work properly on versions lower than 4.3.0. 

Based on the article https://docs.microsoft.com/en-us/archive/blogs/astebner/mailbag-what-version-of-the-net-framework-is-included-in-what-version-of-the-os Windows 10 (all editions) includes the .NET Framework 4.6 as an OS component, and it is installed by default. It means that the tool supposed to work on all Windows 10 systems without any issues.
