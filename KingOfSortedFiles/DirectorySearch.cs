using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace KingOfSortedFiles;

public static class DirectorySearch
{

    public static List<DirectoryInfo> DirectoryList { get; set; } = new();
    public static void ReadFolder(string path)
    {
        try
        {

            var dirResult = Directory.GetDirectories(path).Select(d => new DirectoryInfo(d));
            DirectoryList.AddRange(dirResult);

            
            foreach (var dir in dirResult)
            {
                
                ReadFolder(dir.FullName);
            }
            

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        
    }

    public static List<DirectoryInfo> SearchDirectory(string path,string searchPattern)
    {
        try
        {
            var directoryListCopy = new List<DirectoryInfo>(DirectoryList);

            var resultPath = directoryListCopy.Where(d => Regex.IsMatch(d.FullName, path));
            var resultDirectory = resultPath.Where(d => Regex.IsMatch(d.Name.ToUpper(), searchPattern.ToUpper())).ToList();
        
            return resultDirectory;
        }
        catch (Exception e)
        {
            return SearchDirectory(path, searchPattern);
        }
       
    }
}

