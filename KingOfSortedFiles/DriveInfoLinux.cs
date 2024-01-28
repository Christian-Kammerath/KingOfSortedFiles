using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;


namespace KingOfSortedFiles;


public class Drive
{
    public string Name { get;  set; } = null!;
    public string Size { get;  set; } = null!;

    public string FsType { get; set; } = null!;

    public string MountPoint { get; set;} = null!;
}

public static class DriveInfoLinux
{
    
    private static readonly string[] Patterns = { "\u251C\u2500", "\u2514\u2500","\u2502" };

    
    public static  List<Drive> CreateDriveList(this string[] array)
    {
        
        var driverLinuxList = new List<Drive>();
        
        foreach (var item in array.Skip(1))
        {
            var splitArrayItem = item.Split(' ',StringSplitOptions.RemoveEmptyEntries);
            
            if (splitArrayItem.Length == 4)
            {
                var newDriverInfo = new Drive
                {
                    Name = splitArrayItem[0],
                    Size = splitArrayItem[1],
                    FsType = splitArrayItem[2],
                    MountPoint = splitArrayItem[3].Length >= 1 ? splitArrayItem[3] : "Empty"
                };
                
                if(newDriverInfo.MountPoint != "[SWAP]")
                    driverLinuxList.Add(newDriverInfo);
            }    
        }    
        
        return driverLinuxList;
    }
    public static string[] StringManipulator(this string lsblkOutputString)
    {
       
        var lines = lsblkOutputString.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

        return lines.Select(a => Regex
            .Replace(a, String.Join("|", Patterns), " ")
            .Trim()).ToArray();
        
    }

    
    public static string GetDrivesString()
    {
        var process = new Process();
        process.StartInfo.FileName = "lsblk";
        process.StartInfo.Arguments = "-a -o NAME,SIZE,FSTYPE,MOUNTPOINT"; 
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.CreateNoWindow = true;

        process.Start();

        return process.StandardOutput.ReadToEnd();
    }
    
}