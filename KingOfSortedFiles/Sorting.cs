using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Avalonia.Controls;

namespace KingOfSortedFiles;

public class Sorting
{
    private SortCheckBoxes? SortCheckBoxes { get; set; }
    private bool IsSearchTags { get; set; }
    private bool IsFileExtension { get; set; }
    private List<string> SourceDirectoryPathList { get; set; }
    private static string TargetDirectoryPath { get; set; } = null!;
    private List<string> SearchTagList { get; set; }

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
        //determines the selected sorting parameters
        var sortCheckBoxOne = SortCheckBoxes!.SortOneCheckBoxList.SingleOrDefault(c => (bool)c.IsChecked!);
        var sortCheckBoxTwo = SortCheckBoxes!.SortTwoCheckBoxList.SingleOrDefault(c => (bool)c.IsChecked!);
        //Checks whether the settings made are valid
        if (CheckWhetherTheSettingsAreValid(sortCheckBoxOne, sortCheckBoxTwo))
        {
            

            //runs through the selected source directories
            var counter = 0;
            foreach (var dir in SourceDirectoryPathList)
            {
                SortingTask(dir, sortCheckBoxOne, sortCheckBoxTwo);
                
                //determines the progress and passes it to the progressBar
                counter++;
                UiElementsBinding.SortingProcessTab.StartAndProgress.ProgressBar.Value =
                    counter / SourceDirectoryPathList.Count * 100;
            }
        }
    }

    //Reads all files from the path and controls the sorting
    public void SortingTask(string path, CheckBox? sortCheckBoxOne, CheckBox? sortCheckBoxTwo)
    {
        
        var allFiles = GetAllFiles(IsFileExtension, IsSearchTags, path);

        if (sortCheckBoxOne != null && (bool)sortCheckBoxOne.IsChecked! && sortCheckBoxTwo == null)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath, sortCheckBoxOne);
                MoveFile(file, directoryPathOne, MoveAndOrCopyBool.MoveOnly);
            }
        }
        else if (sortCheckBoxTwo != null && (bool)sortCheckBoxTwo.IsChecked! && sortCheckBoxOne == null)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath, sortCheckBoxTwo);
                MoveFile(file, directoryPathOne, MoveAndOrCopyBool.MoveOnly);
            }
        }
        else if (sortCheckBoxOne != null && sortCheckBoxTwo != null &&
                 (bool)sortCheckBoxOne.IsChecked! && (bool)sortCheckBoxTwo.IsChecked!)
        {
            foreach (var file in allFiles)
            {
                var directoryPathOne = CreateTargetDirectoryPath(file, TargetDirectoryPath, sortCheckBoxOne);
                var directoryPathTwo = CreateTargetDirectoryPath(file, directoryPathOne, sortCheckBoxTwo);
                MoveFile(file, directoryPathTwo, MoveAndOrCopyBool.MoveOnly);
            }
        }
        else
        {
            CustomLogSystem.Error("Invalid sort parameter configuration", true);
        }
    }

    //moves or copies files - ensures unique filenames if file already exists
    private void MoveFile(FileInfo file, string targetDirectoryPath, bool moveOnly)
    {
        //checks if target directory exists, if not it will be created
        var newTargetDirectory = IsNotPresentCreateTargetDirectory(targetDirectoryPath);
        
        //build initial target file path
        var targetFilePath = Path.Combine(newTargetDirectory.FullName, file.Name);
        
        //if file already exists, generate unique filename using GUID
        if (File.Exists(targetFilePath))
        {
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            var extension = file.Extension;
            
            do
            {
                var uniqueId = Guid.NewGuid().ToString()[..8];
                var newFileName = $"{fileNameWithoutExtension}-CopyId({uniqueId}){extension}";
                targetFilePath = Path.Combine(newTargetDirectory.FullName, newFileName);
            } while (File.Exists(targetFilePath));
        }
        
        if (moveOnly)
            File.Move(file.FullName, targetFilePath);
        else
            File.Copy(file.FullName, targetFilePath);
    }

    //creates the destination paths
    public string CreateTargetDirectoryPath(FileInfo fileInfo, string targetDirectoryPath, CheckBox? sortingCheckBox)
    {
        switch ((string)sortingCheckBox!.Tag!)
        {
            case "Created":
                return Path.Combine(targetDirectoryPath, fileInfo.CreationTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture));
            case "changed":
                return Path.Combine(targetDirectoryPath, fileInfo.LastWriteTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture));
            case "LastAccessTime":
                return Path.Combine(targetDirectoryPath, fileInfo.LastAccessTime.ToString("yyyy-MM-dd", CultureInfo.CurrentCulture));
            case "FileExtension":
                return Path.Combine(targetDirectoryPath, fileInfo.Extension.TrimStart('.'));
            default:
            {
                var searchTag = SearchTagList.SingleOrDefault(s => Regex.IsMatch(fileInfo.Name, s));
                return string.IsNullOrEmpty(searchTag)
                    ? Path.Combine(targetDirectoryPath, "could-not-be-classified")
                    : Path.Combine(targetDirectoryPath, searchTag);
            }
        }
    }

    // checks if target directory exists, if not it will be created
    public DirectoryInfo IsNotPresentCreateTargetDirectory(string directoryPath)
    {
        return Directory.Exists(directoryPath)
            ? new DirectoryInfo(directoryPath)
            : Directory.CreateDirectory(directoryPath);
    }

    //searches for the appropriate files based on the settings and returns them
    public List<FileInfo> GetAllFiles(bool isFileExtensions, bool isSearchTags, string sourcePath)
    {
        return isSearchTags switch
        {
            true when !isFileExtensions => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => Regex.IsMatch(f.Name.ToLower(), string.Join("|", SearchTagList).ToLower()))
                .ToList(),
            true when isFileExtensions => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => Regex.IsMatch(f.Name.ToLower(), string.Join("|", SearchTagList).ToLower()) &&
                            FileExtensionFilterList.Contains(f.Extension))
                .ToList(),
            false when isFileExtensions => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .Where(f => FileExtensionFilterList.Contains(f.Extension))
                .ToList(),
            _ => Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories)
                .Select(f => new FileInfo(f))
                .ToList()
        };
    }

    //Check whether the settings made are valid, if not, corresponding notes are displayed in the log ListBox
    private bool CheckWhetherTheSettingsAreValid(CheckBox? sortCheckBoxOne, CheckBox? sortCheckBoxTwo)
    {
        var errorMsgCount = 0;

        if (SourceDirectoryPathList.Count < 1)
        {
            CustomLogSystem.Error("You must select at least one source folder", true);
            errorMsgCount++;
        }

        if (string.IsNullOrEmpty(TargetDirectoryPath))
        {
            CustomLogSystem.Error("You need to select a target folder", true);
            errorMsgCount++;
        }
        
        if (!IsSearchTags && !IsFileExtension)
        {
            CustomLogSystem.Error("Invalid settings, you must select either Search Tags and or File extensions. when setting search By", true);
            errorMsgCount++;
        }

        if (IsFileExtension && FileExtensionFilterList.Count < 1)
        {
            CustomLogSystem.Error("You must select at least one file extension", true);
            errorMsgCount++;
        }

        if (IsSearchTags && SearchTagList.Count < 1)
        {
            CustomLogSystem.Error("You must add at least one search tag", true);
        }
        
        if (SourceDirectoryPathList.Contains(TargetDirectoryPath))
        {
            CustomLogSystem.Error("at least one target folder and one source folder are identical", true);
            errorMsgCount++;
        }

        if (MoveAndOrCopyBool is { CopyAndMove: false, MoveOnly: false })
        {
            CustomLogSystem.Error("You have to choose whether the files should be moved or copied and then moved.", true);
            errorMsgCount++;
        }

        if (sortCheckBoxOne is null && sortCheckBoxTwo is null)
        {
            CustomLogSystem.Error("You must select at least one sorting parameter.", true);
            errorMsgCount++;
        }
        
        if (errorMsgCount > 0)
        {
            CustomLogSystem.Error($"Several configuration errors found [{errorMsgCount}]", true);
            return false;
        }

        return true;
    }
}
