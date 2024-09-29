![Banner](https://eonfn.dev/CrashReportClient%20Helper.png)

### How to Use:
1. Build the solution, then place the executable (`exe`) in the `\Engine\Binaries\Win64` directory.
2. Within the `Win64` folder, rename the file to `CrashReportClient.exe`.

### How to Implement:
1. (Optional) Upload the new `CrashReportClient.exe` to a domain. In your launcher, implement code to remove the existing `CrashReportClient.exe` and replace it by downloading the new version from your domain.

### Changing the Project Name or Console Colors:
1. **Project Name**:  
   To change the project name, update the following line:
   ```csharp
   const string ProjectName = "Eon";
   ```
   Replace `"Eon"` with your desired project name.

2. **Console Colors**:  
   To modify the console colors, adjust the following:
   ```csharp
   static readonly ConsoleColor Color = ConsoleColor.DarkBlue;
   static readonly ConsoleColor Important = ConsoleColor.DarkYellow;
   ```
   You can replace the colors with other available `ConsoleColor` options in C#. For a complete list, refer to the C# documentation.

3. **URL for Crash Report**
   Modify the server URL to show your server, and add a route.
```csharp
   var serverUrl = "http:///upload";
   ```

