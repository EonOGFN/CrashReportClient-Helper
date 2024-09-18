using System.IO.Compression;

class Program
{
    const string ProjectName = "Eon";
    static readonly ConsoleColor Color = ConsoleColor.DarkBlue;
    static readonly ConsoleColor Important = ConsoleColor.DarkYellow;
    static async Task Main()
    {
        Console.Title = $"{ProjectName} Crash Report Client";
        Print("We're sorry that your game has crashed and an error has occurred.\nBy submitting this crash report, you assist our developers in identifying and resolving issues more efficiently.\n");
        Console.Write(new string('-', 35) + "( "); Console.ForegroundColor = Color; Console.Write(ProjectName); Console.ResetColor(); Console.WriteLine(" )" + new string('-', 35) + "\n");

        Console.Write("Would you like to submit the crash report to help improve the game? (Yes to continue): ");
        string Answer = Console.ReadLine();  
        Console.WriteLine();

        if (string.Equals(Answer, "Yes", StringComparison.OrdinalIgnoreCase))
        {
            Print("Processing your request...");
            await Task.Delay(1000);

            try
            {
                string AppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string CrashFolder = Path.Combine(AppData, "FortniteGame\\Saved\\Crashes");
                Print($"Searching for crash reports in: {CrashFolder}");

                if (Directory.Exists(CrashFolder))
                {
                    var Dir = Directory.GetDirectories(CrashFolder);
                    if (Dir.Length == 0)
                    {
                        Print("No crash report directories found.");
                    }
                    else
                    {
                        var Latest = Dir.Select(Dir => new DirectoryInfo(Dir))
                            .OrderByDescending(Info => Info.LastWriteTime)
                            .FirstOrDefault();

                        if (Latest != null)
                        {
                            Print($"Latest Crash Report Located in: {Latest.Name}");
                            Print("Reviewing recent files in the latest crash report directory:");

                            var Recent = Latest.GetFiles()
                                .OrderByDescending(File => File.LastWriteTime)
                                .Take(3)
                                .ToList();

                            if (Recent.Count > 0)
                            {
                                string Download = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
                                string Folder = Path.Combine(Download, "Downloads", $"{ProjectName} Crash Report.zip");

                                if (File.Exists(Folder)){
                                    File.Delete(Folder);
                                }

                                using (var Zip = ZipFile.Open(Folder, ZipArchiveMode.Create))
                                {
                                    foreach (var File in Recent)
                                    {
                                        Print($"- {File.Name} (Updated: {File.LastWriteTime})");
                                        Zip.CreateEntryFromFile(File.FullName, File.Name);
                                    }
                                }

                                Console.Write("\n" + new string('-', 35) + "( "); Console.ForegroundColor = Important; Console.Write("IMPORTANT"); Console.ResetColor(); Console.WriteLine(" )" + new string('-', 35) + "\n");
                                PrintImportant($"Crash report files have been compressed and saved to: {Folder}");
                                PrintImportant("Please send the zip file in the Discord server so we can review it.");
                            }
                            else
                            {
                                Print("No files detected in the latest crash report directory.");
                            }
                        }
                        else
                        {
                            Print("Failed to locate the latest crash report.");
                        }
                    }
                }
                else
                {
                    Print("Crash report directory does not exist.");
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                Print($"Access denied when attempting to retrieve the crash report directory: {ex.Message}");
            }
            catch (Exception ex)
            {
                Print($"Error Occurred during the crash report directory retrieval: {ex.Message}");
            }
        }
        else
        {
            Print("No actions will be taken.");
        }

        Console.WriteLine("\nThank you for submitting your crash report. We will review it as soon as possible! Closing in 5 seconds...");
        await Task.Delay(5000);
    }

    static void Print(string message)
    {
        Console.Write("[");
        Console.Write(DateTime.Now.ToString());
        Console.Write("] [");
        Console.ForegroundColor = Color;
        Console.Write(ProjectName);
        Console.ResetColor();
        Console.WriteLine($"] {message}");
    }

    static void PrintImportant(string message)
    {
        Console.Write("[");
        Console.Write(DateTime.Now.ToString());
        Console.Write("] [");
        Console.ForegroundColor = Important;
        Console.Write(ProjectName);
        Console.ResetColor();
        Console.WriteLine($"] {message}");
    }
}
