using System;
using System.Diagnostics;
using System.IO;
using System.Security.Principal;
using System.IO.Compression;

namespace VSSCollector
{
    class Program
    {

        static void PrintYellow(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintGreen(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintRed(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintBlue(string message)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(message);
            Console.ForegroundColor = ConsoleColor.White;
        }

        static void PrintError(string pathToCollection, string message)
        {
            string log = pathToCollection + @"\log.txt";
            if (File.Exists(log))
            {
                File.AppendAllText(log, PrintDateUTC() + message + " not collected" + Environment.NewLine);
            }

            Console.Write(PrintDateUTC() + message);
            PrintRed(" not collected\r\n");
        }

        static void PrintLog(string pathToCollection, string message)
        {
            string log = pathToCollection + @"\log.txt";
            if (File.Exists(log))
            {
                File.AppendAllText(log, PrintDateUTC() + message + Environment.NewLine);
            }
            Console.WriteLine(PrintDateUTC() + message);
        }

        static void PrintOnlyLog(string pathToCollection, string message)
        {
            string log = pathToCollection + @"\log.txt";
            if (File.Exists(log))
            {
                File.AppendAllText(log, PrintDateUTC() + message + Environment.NewLine);
            }
        }

        static string PrintDateUTC()
        {
            DateTime utc = DateTime.UtcNow;
            string date = utc.ToUniversalTime().ToString("yyyy'-'MM'-'dd' 'HH':'mm':'ss'.'fff ");
            return (date);
        }

        static string CreateFolderForCollection(string currentDirectory, int numberOfVSS)
        {
            string pathToCollection = currentDirectory + @"\Collection_VolumeShadowCopy" + numberOfVSS;

            if (Directory.Exists(pathToCollection))
            {
                Console.WriteLine("Folder " + pathToCollection + " already exists.");
                pathToCollection = "";
            }
            else
            {
                DirectoryInfo di = Directory.CreateDirectory(pathToCollection);
                di.Attributes = FileAttributes.Directory | FileAttributes.Hidden;

                Console.Write("The directory ");
                PrintGreen(pathToCollection);
                Console.WriteLine(" was created successfully at {1}. \r\n", pathToCollection, Directory.GetCreationTime(pathToCollection));

            }
            return pathToCollection;
        }

        static void CollectRegistry(string pathToVSS, string pathToCollection)
        {
            string SAM = @"\Windows\System32\config\SAM";
            string SOFTWARE = @"\Windows\System32\config\SOFTWARE";
            string SECURITY = @"\Windows\System32\config\SECURITY";
            string SYSTEM = @"\Windows\System32\config\SYSTEM";
            string AMCACHE = @"\Windows\appcompat\Programs\Amcache.hve";

            string SAM_Source = pathToVSS + SAM;
            string SAM_Destination = pathToCollection + SAM;

            string SOFTWARE_Source = pathToVSS + SOFTWARE;
            string SOFTWARE_Destination = pathToCollection + SOFTWARE;

            string SECURITY_Source = pathToVSS + SECURITY;
            string SECURITY_Destination = pathToCollection + SECURITY;

            string SYSTEM_Source = pathToVSS + SYSTEM;
            string SYSTEM_Destination = pathToCollection + SYSTEM;

            string AMCACHE_Source = pathToVSS + AMCACHE;
            string AMCACHE_Destination = pathToCollection + AMCACHE;

            PrintGreen("Collecting: ");
            Console.WriteLine("registry hives...");

            PrintOnlyLog(pathToCollection, "Collecting: registry hives...");

            CollectKey(pathToCollection, "SAM", SAM_Source, SAM_Destination);
            CollectKey(pathToCollection, "SOFTWARE", SOFTWARE_Source, SOFTWARE_Destination);
            CollectKey(pathToCollection, "SECURITY", SECURITY_Source, SECURITY_Destination);
            CollectKey(pathToCollection, "SYSTEM", SYSTEM_Source, SYSTEM_Destination);
            CollectKey(pathToCollection, "AMCACHE", AMCACHE_Source, AMCACHE_Destination);
        }

        static void CollectRegistryForUsers(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("NTUSER.DAT...");

            PrintOnlyLog(pathToCollection, "Collecting: NTUSER.DAT...");

            string NTUSER = @"\NTUSER.DAT";
            var directories = Directory.GetDirectories(pathToVSS + @"\Users\");
            foreach (string dir in directories)
            {
                string NTUSER_Source = dir + NTUSER;
                string NTUSER_Destination = pathToCollection + @"\Windows\Users\" + Path.GetFileName(dir) + NTUSER;

                string KeyName = Path.GetFileName(dir) + NTUSER;

                CollectKey(pathToCollection, KeyName, NTUSER_Source, NTUSER_Destination);
            }
        }

        static void CollectKey(string pathToCollection, string KEY_Name, string KEY_Source, string KEY_Destination)
        {
            if (File.Exists(KEY_Source))
            {
                try
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(KEY_Destination));
                    File.Copy(KEY_Source, KEY_Destination);

                    if (File.Exists(KEY_Destination))
                    {
                        //Console.WriteLine(PrintDateUTC() + "registry hive " + KEY_Name + " collected");
                        PrintLog(pathToCollection, "registry hive " + KEY_Name + " collected");
                    }
                }
                catch(Exception)
                {
                    PrintError(pathToCollection, KEY_Name);
                }
            }
        }

        static void CollectPrefetch(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("prefetch files...");

            PrintOnlyLog(pathToCollection, "Collecting: prefetch files...");

            string PREFETCH = @"\Windows\Prefetch\";

            string PREFETCH_Source = pathToVSS + PREFETCH;
            string PREFETCH_Destination = pathToCollection + PREFETCH;

            DirectoryInfo dirPrefetch = new DirectoryInfo(PREFETCH_Source);
            Directory.CreateDirectory(Path.GetDirectoryName(PREFETCH_Destination));

            if (Directory.Exists(PREFETCH_Source))
            {
                foreach (var file in dirPrefetch.GetFiles("*.pf"))
                {
                    try
                    {
                        File.Copy(file.FullName, PREFETCH_Destination + file.Name);

                        if (File.Exists(PREFETCH_Destination + file.Name))
                        {
                            //Console.WriteLine(PrintDateUTC() + file.Name + " collected");
                            PrintLog(pathToCollection, file.Name + " collected");
                        }
                    }
                    catch(Exception)
                    {
                        PrintError(pathToCollection, file.Name);
                    }
                }
            }
        }

        static void CollectWinEvent(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("Windows Event Logs...");

            PrintOnlyLog(pathToCollection, "Collecting: Windows Event Logs...");

            string WinEvnt = @"\Windows\System32\winevt\Logs\";

            string WinEvnt_Source = pathToVSS + WinEvnt;
            string WinEvnt_Destination = pathToCollection + WinEvnt;

            DirectoryInfo dirWinEvt = new DirectoryInfo(WinEvnt_Source);
            Directory.CreateDirectory(Path.GetDirectoryName(WinEvnt_Destination));

            if (Directory.Exists(WinEvnt_Source))
            {
                foreach (var file in dirWinEvt.GetFiles("*.evtx"))
                {
                    try
                    {
                        File.Copy(file.FullName, WinEvnt_Destination + file.Name);
                        if (File.Exists(WinEvnt_Destination + file.Name))
                        {
                            //Console.WriteLine(PrintDateUTC() + file.Name + " collected");
                            PrintLog(pathToCollection, file.Name + " collected");
                        }
                    }
                    catch (Exception)
                    {
                        PrintError(pathToCollection, file.Name);
                    }
                }
            }
        }

        static void CollectSrum(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("SRUM database...");

            PrintOnlyLog(pathToCollection, "Collecting: SRUM database...");

            string SRUM = @"\windows\system32\sru\srudb.dat";

            string SRUM_Source = pathToVSS + SRUM;
            string SRUM_Destination = pathToCollection + SRUM;

            Directory.CreateDirectory(Path.GetDirectoryName(SRUM_Destination));

            if (File.Exists(SRUM_Source))
            {
                try
                {
                    File.Copy(SRUM_Source, SRUM_Destination);
                    if (File.Exists(SRUM_Destination))
                    {
                        //Console.WriteLine(PrintDateUTC() + Path.GetFileName(PrintDateUTC() + SRUM_Source) + " collected");
                        PrintLog(pathToCollection, Path.GetFileName(PrintDateUTC() + SRUM_Source) + " collected");
                    }
                }
                catch (Exception)
                {
                    PrintError(pathToCollection, "srumbdb.dat");
                }
            }
            else
            {
                PrintError(pathToCollection, "srumbdb.dat");
            }
        }

        static void CollectWindowsTasksXML(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("XML-based Windows Scheduled Tasks...");

            PrintOnlyLog(pathToCollection, "Collecting: XML-based Windows Scheduled Tasks...");

            string WinTask = @"\Windows\System32\Tasks\";

            string WinTask_Source = pathToVSS + WinTask;
            string WinTask_Destination = pathToCollection + WinTask;

            Directory.CreateDirectory(Path.GetDirectoryName(WinTask_Destination));

            DirectoryInfo dirPrefetch = new DirectoryInfo(WinTask_Source);
            if (Directory.Exists(WinTask_Source))
            {
                try
                {
                    foreach (string dirPath in Directory.GetDirectories(WinTask_Source, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(WinTask_Source, WinTask_Destination));
                    }
                    foreach (string newPath in Directory.GetFiles(WinTask_Source, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Copy(newPath, newPath.Replace(WinTask_Source, WinTask_Destination), true);
                            //Console.WriteLine(PrintDateUTC() + Path.GetFileName(Path.GetFileName(newPath)) + " collected");
                            PrintLog(pathToCollection, Path.GetFileName(Path.GetFileName(newPath)) + " collected");
                        }
                        catch (Exception)
                        {
                            PrintError(pathToCollection, WinTask_Destination);
                        }
                    }
                }
                catch (Exception)
                {
                    PrintError(pathToCollection, "Windows Scheduled Tasks (XML)");
                }
            }
            else
            {
                PrintError(pathToCollection, "Windows Scheduled Tasks (XML)");
            }
        }

        static void CollectWindowsTasksOldFormat(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("Windows Scheduled Tasks...");

            PrintOnlyLog(pathToCollection, "Collecting: Windows Scheduled Tasks...");

            string WinTask = @"\Windows\Tasks\";

            string WinTask_Source = pathToVSS + WinTask;
            string WinTask_Destination = pathToCollection + WinTask;

            Directory.CreateDirectory(Path.GetDirectoryName(WinTask_Destination));

            DirectoryInfo dirPrefetch = new DirectoryInfo(WinTask_Source);
            if (Directory.Exists(WinTask_Source))
            {
                try
                {
                    foreach (string dirPath in Directory.GetDirectories(WinTask_Source, "*", SearchOption.AllDirectories))
                    {
                        Directory.CreateDirectory(dirPath.Replace(WinTask_Source, WinTask_Destination));
                    }
                    foreach (string newPath in Directory.GetFiles(WinTask_Source, "*.*", SearchOption.AllDirectories))
                    {
                        File.Copy(newPath, newPath.Replace(WinTask_Source, WinTask_Destination), true);
                        //Console.WriteLine(PrintDateUTC() + Path.GetFileName(Path.GetFileName(newPath)) + " collected");
                        PrintLog(pathToCollection, Path.GetFileName(Path.GetFileName(newPath)) + " collected");
                    }
                }
                catch (Exception)
                {
                    PrintError(pathToCollection, "Windows Scheduled Tasks (old format)");
                }
            }
            else
            {
                PrintError(pathToCollection, "Windows Scheduled Tasks (old format)");
            }
        }

        static void CollectRecycleBin(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("files from the RecycleBin...");

            PrintOnlyLog(pathToCollection, "Collecting: files from the RecycleBin...");

            string RecycleBin = @"\$Recycle.Bin\";

            string RecycleBin_Source = pathToVSS + RecycleBin;
            string RecycleBin_Destination = pathToCollection + RecycleBin;

            Directory.CreateDirectory(Path.GetDirectoryName(RecycleBin_Destination));

            DirectoryInfo dirPrefetch = new DirectoryInfo(RecycleBin_Source);
            if (Directory.Exists(RecycleBin_Source))
            {
               
                    foreach (string file in Directory.EnumerateFiles(RecycleBin_Source, "*.*", SearchOption.AllDirectories))
                    {
                        try
                        {
                            File.Copy(file, RecycleBin_Destination + Path.GetFileName(file));
                            if (File.Exists(RecycleBin_Destination + Path.GetFileName(file)))
                            {
                                //Console.WriteLine(PrintDateUTC() + Path.GetFileName(file) + " collected");
                                PrintLog(pathToCollection, "RecycleBin file " + Path.GetFileName(file) + " collected");
                        }
                        }
                        catch(Exception)
                        {
                            PrintError(pathToCollection, Path.GetFileName(file));
                        }
                    }
            }
            else
            {
                PrintError(pathToCollection, "Recycle Bin");
            }
        }

        static void CollectLnkFiles(string pathToVSS, string pathToCollection)
        {
            PrintGreen("Collecting: ");
            Console.WriteLine("LNK files...");

            PrintOnlyLog(pathToCollection, "Collecting LNK files...");

            string LNK_Windows = @"\AppData\Roaming\Microsoft\Windows\Recent\";
            string LNK_Office = @"\AppData\Roaming\Microsoft\Office\Recent\";

            var directories = Directory.GetDirectories(pathToVSS + @"\Users\");
            foreach (string user_dir in directories)
            {
                string LNK_Windows_Source = user_dir + LNK_Windows;
                string LNK_Windows_Destination = pathToCollection + @"\Windows\Users\" + Path.GetFileName(user_dir) + LNK_Windows;
    
                if (Directory.Exists(LNK_Windows_Source))
                {

                    Directory.CreateDirectory(Path.GetDirectoryName(LNK_Windows_Destination));
                    var sourceDirectories = Directory.GetFiles(LNK_Windows_Source,"*.lnk");
                    foreach (string file in sourceDirectories)
                    {
                        try
                        {
                            File.Copy(file, LNK_Windows_Destination + Path.GetFileName(file));

                            if (File.Exists(LNK_Windows_Destination + Path.GetFileName(file)))
                            {
                                //Console.WriteLine(PrintDateUTC() + "LNK file " + Path.GetFileName(file) + " collected");
                                PrintLog(pathToCollection, "LNK file " + Path.GetFileName(file) + " collected");
                            }

                        }
                        catch (Exception)
                        {
                            PrintError(pathToCollection, Path.GetFileName(file));
                        }
                    }
                }
            }
        }

        static void RemoveVSS(string pathToCollection, string pathToShadowFolder)
        {
            if (Directory.Exists(pathToShadowFolder))
            {
                Directory.Delete(pathToShadowFolder);

                if (!Directory.Exists(pathToShadowFolder))
                {
                    PrintYellow("Removed the symbolic link to VSS: ");
                    Console.WriteLine(pathToShadowFolder);

                    PrintOnlyLog(pathToCollection, "Removed the symbolic link to VSS:  " + pathToShadowFolder );
                }
                else
                {
                    PrintRed("[Error] ");
                    Console.WriteLine("Symbolic link to VSS could not be removed.");
                }
            }
            else
            {
                PrintRed("[Error] ");
                Console.WriteLine("Symbolic link to VSS could not be removed.");
            }
        }

        static void CompressCollection(string pathToVSS, string CurrentPath, int VSSNumber)
        {
            var zipFile = CurrentPath + @"\Collection_VolumeShadowCopy" + VSSNumber + ".zip";

            if (File.Exists(zipFile))
            {
                File.Delete(zipFile);
            }

            PrintYellow("Compressing: ");
            Console.WriteLine(pathToVSS + " to " + zipFile);

            PrintOnlyLog(pathToVSS, "Compressing: " + pathToVSS + " to " + zipFile);

            ZipFile.CreateFromDirectory(pathToVSS, zipFile);
        }

        public static void DeleteDirectory(string target_dir)
        {
            string[] files = Directory.GetFiles(target_dir);
            string[] dirs = Directory.GetDirectories(target_dir);

            foreach (string file in files)
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string dir in dirs)
            {
                DeleteDirectory(dir);
            }

            Directory.Delete(target_dir, false);
        }

        static void DeleteCollection(string pathToCollection)
        {
            try
            {
                if (Directory.Exists(pathToCollection))
                {
                    PrintYellow("Deleting: ");
                    Console.WriteLine(pathToCollection);

                    DeleteDirectory(pathToCollection);

                    if (!Directory.Exists(pathToCollection))
                    {
                        Console.Write("Collection: ");
                        PrintGreen("DELETED");
                        Console.WriteLine("");

                        //(pathToCollection, "Collection: DELETED");
                    }
                    else
                    {
                        Console.Write("Collection: ");
                        PrintRed("NOT DELETED");
                        Console.WriteLine("");
                        //PrintOnlyLog(pathToCollection, "Collection: NOT DELETED");
                    }
                }

            }
            catch (Exception)
            {
                Console.Write("Collection: ");
                PrintRed("NOT DELETED");
                Console.WriteLine("");
            }
        }

        static void Main(string[] args)
        {

            WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

            if (isAdmin)
            {
                int i = 0;
                string CurrentPath = Directory.GetCurrentDirectory();
                string pathToCollection;

                var process_WMIC = new ProcessStartInfo("wmic");
                process_WMIC.Arguments = "shadowcopy get DeviceObject";
                process_WMIC.WindowStyle = ProcessWindowStyle.Hidden;
                process_WMIC.UseShellExecute = false;
                process_WMIC.RedirectStandardOutput = true;
                var p = Process.Start(process_WMIC);
                p.WaitForExit();
                String output = p.StandardOutput.ReadToEnd();

                using (StringReader reader = new StringReader(output))
                {
                    string line = string.Empty;
                    do
                    {
                        line = reader.ReadLine();
                        try
                        {
                            if (line != null)
                            {
                                if (line.Contains("HarddiskVolumeShadowCopy"))
                                {
                                    i++;
                                    line = line.Replace(" ", "");
                                    string strCmdText;
                                    string pathShadowFolder = CurrentPath + "\\ShadowCopy" + i;
                                    strCmdText = "/c MkLink /d " + pathShadowFolder + " " + line + "\\";

                                    Console.WriteLine("\r\n");
                                    PrintGreen("Volume Shadow Copy: ");
                                    Console.WriteLine(line);

                                    var process = System.Diagnostics.Process.Start("CMD.exe", strCmdText);
                                    process.WaitForExit();
                                    Console.WriteLine("");

                                    pathToCollection = CreateFolderForCollection(CurrentPath, i);

                                    string log = pathToCollection + @"\log.txt";
                                    using (FileStream fs = File.Create(log))

                                        if (!File.Exists(log))
                                        {
                                            // Create a file to write to.
                                            string createText = "Hello and Welcome" + Environment.NewLine;
                                            File.WriteAllText(log, createText);
                                        }

                                    PrintLog(pathToCollection, "Volume Shadow Copy " + line);

                                    if (pathToCollection.Length > 1)
                                    {
                                        CollectPrefetch(pathShadowFolder, pathToCollection);
                                        CollectRegistry(pathShadowFolder, pathToCollection);
                                        CollectWinEvent(pathShadowFolder, pathToCollection);
                                        CollectSrum(pathShadowFolder, pathToCollection);
                                        CollectRegistryForUsers(pathShadowFolder, pathToCollection);
                                        CollectWindowsTasksXML(pathShadowFolder, pathToCollection);
                                        CollectWindowsTasksOldFormat(pathShadowFolder, pathToCollection);
                                        CollectRecycleBin(pathShadowFolder, pathToCollection);
                                        CollectLnkFiles(pathShadowFolder, pathToCollection);
                                    }

                                    RemoveVSS(pathToCollection, pathShadowFolder);
                                    CompressCollection(pathToCollection, CurrentPath, i);
                                    DeleteCollection(pathToCollection);

                                    Console.WriteLine();
                                }
                            }
                        }
                        catch (Exception)
                        {

                        }

                    } while (line != null);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[Error] ");

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Please run the applicaiton with administrator rights!");
            }

        }
    }
}
