using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using KingOfSortedFiles.UiElements;
using KingOfSortedFiles.Views;
using Microsoft.Extensions.Configuration;

namespace KingOfSortedFiles;

public class ProgramStartRoutine
{
    private AppSettings Settings { get; set; } = null!;

    public ProgramStartRoutine(MainWindow mainWindow)
    {
        UiElementsBinding.BindUiElements(mainWindow);
        ReadJsonSettings();
        LoadFileExtensionsInListBox();
        ReadStartPathFilesAndDirectory();
        
        CustomLogSystem
            .BindListBox(mainWindow.Find<ListBox>("LogListBox"))
            .BindLogFile(Path.Combine(Directory.GetCurrentDirectory(),"Log.txt"));
    }

    private void ReadJsonSettings()
    {
        IConfigurationRoot builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
            .Build();

        Settings = builder.Get<AppSettings>()!;

    }

    private void LoadFileExtensionsInListBox()
    {
       
        var  fileExtensionListBoxItems =  UiElementsBinding.FileExtensionListBox!.Items;
        
       fileExtensionListBoxItems.Add(new FileExtensionTab(Settings.FileExtensions.TextFiles,
            "Text Files"));
        
        fileExtensionListBoxItems.Add(
            new FileExtensionTab(Settings.FileExtensions.AudioImageAndVideoFiles, "Audio Image And Video Files"));
        
        fileExtensionListBoxItems.Add(new FileExtensionTab(Settings.FileExtensions.CompressedFiles,
            "Compressed Files"));

        fileExtensionListBoxItems.Add(new FileExtensionTab(Settings.FileExtensions.SystemFiles,
            "System Files"));

        fileExtensionListBoxItems.Add(
            new FileExtensionTab(Settings.FileExtensions.OtherFileExtensions, "Other File Extensions"));
        
        
    }

    private async Task ReadStartPathFilesAndDirectory()
    {
        if (string.IsNullOrEmpty(Settings.StartPath))
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
                UiElementsBinding.SourceListBox!.Items.Add(new DriverTab(item));
            }
        }
    }
    
}