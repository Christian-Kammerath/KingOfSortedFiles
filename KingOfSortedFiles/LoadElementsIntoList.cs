using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.Controls;

using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles;

public class LoadElementsIntoList
{
    private string Path { get; set; }
    private ListBox ListBoxTooLoaded { get; set; }

    public LoadElementsIntoList(string path,ListBox listBoxTooLoaded)
    {
        Path = path;
        ListBoxTooLoaded = listBoxTooLoaded;
        
        ListBoxTooLoaded.Items.Clear();
        ReadStartPathFilesAndDirectory();

        if (ListBoxTooLoaded.Name == "SourceListBox")
            UiElementsBinding.SourcePathBox!.Text = path;
        else
            UiElementsBinding.TargetPathBox!.Text = path;

    }

    private void LoadFolders()
    {
        var folderInPath = Directory.GetDirectories(Path)
            .Select(f => new DirectoryInfo(f));

        foreach (var folder in folderInPath)
        {
            ListBoxTooLoaded.Items.Add(ListBoxTooLoaded.Name == "SourceListBox"
                ? new SourceFolderTab(folder)
                : new TargetFolderTab(folder) );
        }
    }

    private void LoadFiles()
    {
        var filesInPath = Directory.GetFiles(Path)
            .Select(f => new FileInfo(f));

        foreach (var files in filesInPath)
        {
            ListBoxTooLoaded.Items.Add(new FileTab(files));
        }
        
    }
    
    private void ReadStartPathFilesAndDirectory()
    {   
        if (string.IsNullOrEmpty(Path))
        {

            var isLinux =  RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            var isWindows =  RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

            var driverList = new List<Drive>();

            if (isLinux)
                driverList =  DriveInfoLinux.GetDrivesString()
                    .StringManipulator()
                    .CreateDriveList();
            
            if (isWindows)
                driverList = DriveInfo.GetDrives().Select(d => new Drive()
                { 
                    Name = d.Name,
                    FsType = d.DriveFormat,
                    Size = d.TotalSize.ToString(),
                    MountPoint = d.RootDirectory.FullName
                        
                }).ToList();
            
            foreach (var item in driverList)
            {
                ListBoxTooLoaded.Items.Add(new DriverTab(item));
            }
        }
        else
        {
            LoadFolders();
            LoadFiles();
        }
        
    }
}