using System.IO.Compression;

class Program
{
    const string ProjectName = "Eon"; // Project Name
    static readonly ConsoleColor Color = ConsoleColor.Magenta; // Project Name Color
    static readonly ConsoleColor Important = ConsoleColor.DarkYellow; // Important Color
    const int CloseHelper = 15000; // Adjust this to your personal liking (1000 = 1 Second)

    static async Task Main()
    {
        Console.Title = $"{ProjectName} Helper";
        Print("We're sorry that your game has crashed and an error has occurred.\nBy submitting this crash report, you assist our developers in identifying and resolving issues more efficiently.\n");
        PrintHeader(ProjectName, Color);

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
                                string Folder = Path.Combine(Download, "Downloads", $"{ProjectName} CrashReport.zip");

                                if (File.Exists(Folder)) {
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

                                Console.WriteLine();
                                PrintHeader("IMPORTANT", Important);
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

        PrintImportant($"Thank you for submitting your crash report. We will review it as soon as possible! Closing in {CloseHelper / 1000} seconds...");
        await Task.Delay(CloseHelper);
    }

    static void Print(string Message)
    {
        Console.Write("[");
        Console.Write(DateTime.Now.ToString());
        Console.Write("] [");
        Console.ForegroundColor = Color;
        Console.Write(ProjectName);
        Console.ResetColor();
        Console.WriteLine($"] {Message}");
    }

    static void PrintImportant(string Message)
    {
        Console.Write("[");
        Console.Write(DateTime.Now.ToString());
        Console.Write("] [");
        Console.ForegroundColor = Important;
        Console.Write(ProjectName);
        Console.ResetColor();
        Console.WriteLine($"] {Message}");
    }

    static void PrintHeader(string Header, ConsoleColor Color)
    {
        int X = 100; 
        int Y = Header.Length;
        int Z = (X - Y - 4) / 2; 

        string Dashes = new string('-', Z);
        Console.Write($"{Dashes} ( ");
        Console.ForegroundColor = Color;
        Console.Write(Header);
        Console.ResetColor();
        Console.WriteLine($" ) {Dashes}\n");
    }
}
