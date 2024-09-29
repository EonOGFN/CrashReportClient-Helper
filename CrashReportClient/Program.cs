using System.Net.Http;
using System.IO.Compression;
using System.Text;

class Program
{
    const string ProjectName = "Eon"; // Project Name
    static readonly ConsoleColor Color = ConsoleColor.Magenta; // Project Name Color
    static readonly ConsoleColor Important = ConsoleColor.DarkYellow; // Important Color
    const int CloseHelper = 15; // Adjust this to your personal liking (15 Seconds)

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
                                string TempFolder = Path.GetTempPath();
                                string Folder = Path.Combine(TempFolder, $"{ProjectName} CrashReport.zip");

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
                                PrintImportant("Uploading the crash report to the web server...");

                                
                                await UploadCrashReportToServer(Folder);

                                PrintImportant("Thank you for submitting your crash report. We will review it as soon as possible! Closing in {CloseHelper} seconds...");
                                await Task.Delay(CloseHelper * 1000);
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
    }

    static async Task UploadCrashReportToServer(string filePath)
    {
        using (HttpClient client = new HttpClient())
        {
            try
            {
                var serverUrl = "http:///upload"; // Add your server URL or IP
                var form = new MultipartFormDataContent();

                
                var fileContent = new ByteArrayContent(File.ReadAllBytes(filePath));
                fileContent.Headers.Add("Content-Type", "application/zip");
                form.Add(fileContent, "file", Path.GetFileName(filePath));

                
                var response = await client.PostAsync(serverUrl, form);
                response.EnsureSuccessStatusCode();

                Console.WriteLine("Crash report successfully uploaded to the server!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading crash report to server: {ex.Message}");
            }
        }
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
