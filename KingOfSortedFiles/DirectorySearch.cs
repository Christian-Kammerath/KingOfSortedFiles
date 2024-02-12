using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace KingOfSortedFiles;

public static class DirectorySearch
{

    public static List<DirectoryInfo> GetDirectory(string path, string searchString = "null")
    {
        var scriptPath = Path.Combine(Directory.GetCurrentDirectory(), "GetFolder", "main.py");
        return RunScript(scriptPath, path, searchString);
    }
    
    public static List<DirectoryInfo> RunScript(string scriptName,string path,string searchString)
    {
        var interpreter = Path.Combine(Directory.GetCurrentDirectory(),"GetFolder",".venv","bin","python3.11");
        
        Process pythonProcess = new Process();
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = interpreter,
            Arguments = scriptName + " " + path + " " + searchString,  
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        pythonProcess.StartInfo = startInfo;
        pythonProcess.Start();


        var output = pythonProcess.StandardOutput.ReadToEnd();
        pythonProcess.WaitForExit();

        var result = output.Split('\n');

        var directoryList = new List<DirectoryInfo>();

        foreach (var item in result)
        {
            try
            {
                var dir = new DirectoryInfo(item);
                directoryList.Add(dir);
            }
            catch (Exception)
            {
                continue;
            }
        }

        return directoryList;
    }
   

}

