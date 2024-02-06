using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles;

public class Sorting
{
    private SortCheckBoxes? SortCheckBoxes { get; set; }
    private bool IsSearchTags { get; set; }
    private bool IsFileExtension { get; set; }
    private List<string> SourceDirectoryPathList { get; set; }
    private static string TargetDirectoryPath { get; set; } = null!;
    private List<string> SearchTagList { get; set; } = new()!;

    private List<string> FileExtensionFilterList { get; set; }
    
    public static MoveAndOrCopy MoveAndOrCopyBool { get; set; } = null!;



    public Sorting(SortingSettings sortingSettings)
    {
        SortCheckBoxes = sortingSettings.SortCheckBoxes;
        IsSearchTags = (bool)sortingSettings.SearchByCheckBoxes!.SearchTags.IsChecked!;
        IsFileExtension = (bool)sortingSettings.SearchByCheckBoxes!.FileExtensions.IsChecked!;
        SourceDirectoryPathList = sortingSettings.SourceDirectoryPathList;
        TargetDirectoryPath = sortingSettings.TargetDirectoryPath;
        FileExtensionFilterList = sortingSettings.FileExtensionList;
        MoveAndOrCopyBool = sortingSettings.MoveAndOrCopy;

        SearchTagList = sortingSettings.SearchTagList.SearchTagList;
    }

    public void StartSorting()
    {
        if (CheckWhetherTheSettingsAreValid())
        {

            var sortCheckBoxOne = SortCheckBoxes!.SortOneCheckBoxList.SingleOrDefault(c => (bool)c.IsChecked!);
            var sortCheckBoxTwo = SortCheckBoxes!.SortTwoCheckBoxList.SingleOrDefault(c => (bool)c.IsChecked!);

            
            for (int i = 0; i < SourceDirectoryPathList.Count;)
            {
                switch(SourceDirectoryPathList.Count)
                {
                    case >= 4:
                        var taskOne = new Task(()=> SortingTask(i,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskTwo = new Task(()=>SortingTask(i+1,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskThree = new Task(()=>SortingTask(i+2,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskFour = new Task(()=>SortingTask(i+3,sortCheckBoxOne!,sortCheckBoxTwo!));
                        
                        taskOne.Start();
                        taskTwo.Start();
                        taskThree.Start();
                        taskFour.Start();

                        taskOne.Wait();
                        taskTwo.Wait();
                        taskThree.Wait();
                        taskFour.Wait();

                        i += 4;
                        break;
                    case 3:
                        var taskOneA = new Task(()=> SortingTask(i,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskTwoA = new Task(()=>SortingTask(i+1,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskThreeA = new Task(()=>SortingTask(i+2,sortCheckBoxOne!,sortCheckBoxTwo!));
                        
                        taskOneA.Start();
                        taskTwoA.Start();
                        taskThreeA.Start();

                        taskOneA.Wait();
                        taskTwoA.Wait();
                        taskThreeA.Wait();

                        i += 3;
                        break;
                    case 2:
                        var taskOneB = new Task(()=> SortingTask(i,sortCheckBoxOne!,sortCheckBoxTwo!));
                        var taskTwoB = new Task(()=>SortingTask(i+1,sortCheckBoxOne!,sortCheckBoxTwo!));
                        
                        taskOneB.Start();
                        taskTwoB.Start();

                        taskOneB.Wait();
                        taskTwoB.Wait();

                        i += 2;
                        break;
                    case 1:
                        var taskOneC = new Task(()=> SortingTask(i,sortCheckBoxOne!,sortCheckBoxTwo!));
                        taskOneC.Start();
                        taskOneC.Wait();
                        i += 1;
                        break;
                    default:
                        break;
                }
            }
            
            
        }
    }

    private void SortingTask(int index, CheckBox? sortCheckBoxOne, CheckBox? sortCheckBoxTwo  )
    {
        
        var allFiles = GetAllFiles(IsFileExtension,IsSearchTags,SourceDirectoryPathList[index]);
        
        
        
        if ((bool)sortCheckBoxOne!.IsChecked! && 
            sortCheckBoxTwo == null)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath,sortCheckBoxOne);
                MoveFile(file,directoryPathOne,MoveAndOrCopyBool.MoveOnly);
                
            }
        }

        else if ((bool)sortCheckBoxTwo!.IsChecked! && sortCheckBoxOne == null)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath,sortCheckBoxTwo);
                MoveFile(file,directoryPathOne,MoveAndOrCopyBool.MoveOnly);
            }
        }
        
        else if ((bool)sortCheckBoxTwo!.IsChecked! && (bool)sortCheckBoxOne.IsChecked)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath,sortCheckBoxOne);
                var directoryPathTwo = CreateTargetDirectoryPath(file, directoryPathOne,sortCheckBoxTwo);
                MoveFile(file,directoryPathTwo,MoveAndOrCopyBool.MoveOnly);
            }
        }
        
    }

    private void MoveFile(FileInfo file, string targetDirectoryPath, bool moveOnly)
    {
        
        var newTargetDirectory = IsNotPresentCreateTargetDirectory(targetDirectoryPath);
                
        if(moveOnly)
            File.Move(file.FullName,newTargetDirectory.FullName);
        else
        {
            File.Copy(file.FullName,newTargetDirectory.FullName);
        }
    }

    public string CreateTargetDirectoryPath(FileInfo fileInfo, string targetDirectoryPath,CheckBox? sortingCheckBox)
    {

        var checkBox = sortingCheckBox;

        if ((string)checkBox.Tag! == "Created")
        {
            return Path.Combine(targetDirectoryPath, fileInfo.CreationTime.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture));
        }

        if ((string)checkBox.Tag! == "changed")
        {
            return Path.Combine(targetDirectoryPath, fileInfo.LastWriteTime.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture));
        }
        
        if ((string)checkBox.Tag! == "LastAccessTime")
        {
            return Path.Combine(targetDirectoryPath, fileInfo.LastAccessTime.ToString("yyyy-MM-dd",CultureInfo.CurrentCulture));
        }
        
        if ((string)checkBox.Tag! == "FileExtension")
        {
            return Path.Combine(targetDirectoryPath,fileInfo.Extension.TrimStart('.'));
        }
        
        
        var searchTag = Path.Combine(targetDirectoryPath,SearchTagList
            .SingleOrDefault(s => Regex.IsMatch(fileInfo.Name,s))?? "");

        return !string.IsNullOrEmpty(searchTag) ? 
            searchTag :
            "could-not-be-classified";
    }

    public DirectoryInfo IsNotPresentCreateTargetDirectory(string directoryPath)
    {
        return Directory.Exists(directoryPath)
            ? new DirectoryInfo(directoryPath)
            : Directory.CreateDirectory(directoryPath);

    }

    private List<FileInfo> GetAllFiles(bool isFileExtensions, bool isSearchTags,string sourcePath)
    {
        return isSearchTags switch
        {
            true when !isFileExtensions => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => Regex.IsMatch(f.Name, string.Join("||", SearchTagList)))
                .ToList(),
            true when isFileExtensions => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => Regex.IsMatch(f.Name, string.Join("||", SearchTagList)) &&
                            FileExtensionFilterList.Contains(f.Extension))
                .ToList(),
            false when isFileExtensions => (Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => FileExtensionFilterList.Contains(f.Extension))
                .ToList()),
            _ => null!
        };
    }

    private bool CheckWhetherTheSettingsAreValid()
    {

        var errorMsgCount = 0;

        if (SourceDirectoryPathList.Count < 0)
        {
            CustomLogSystem.Error("You must select at least one source folder",true);
            errorMsgCount++;
        }

        if (string.IsNullOrEmpty(TargetDirectoryPath))
        {
            CustomLogSystem.Error("You need to select a target folder",true);
            errorMsgCount++;
        }
        
        if (!IsSearchTags && !IsFileExtension)
        {
            CustomLogSystem.Error("Invalid settings, you must select either Search Tags and or File extensions. when setting search By",true);
            errorMsgCount++;
        }

        if (IsFileExtension && FileExtensionFilterList.Count < 1)
        {
            CustomLogSystem.Error("You must select at least one file extension",true);
            errorMsgCount++;

        }

        if (IsSearchTags && SearchTagList.Count < 1)
        {
            CustomLogSystem.Error("You must add at least one search tag",true);

        }
        
        if (SourceDirectoryPathList.Contains(TargetDirectoryPath))
        {
            CustomLogSystem.Error("at least one target folder and one source folder are identical",true);
            errorMsgCount++;

        }

        if (MoveAndOrCopyBool is { CopyAndMove: false, MoveOnly: false })
        {
            CustomLogSystem.Error("You have to choose whether the files should be moved or copied and then moved.",true);
            errorMsgCount++;
        }
        
        if (errorMsgCount > 0)
        {
            CustomLogSystem.Error($"Several configuration errors found [{errorMsgCount}]",true);
            return false;
        }

        return true;

    }

    
    
}
