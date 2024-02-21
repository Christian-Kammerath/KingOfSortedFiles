using System.IO;
using KingOfSortedFiles.UiElements;
using Microsoft.Extensions.Configuration;

namespace KingOfSortedFiles;

public class ProgramStartRoutine
{
    private AppSettings Settings { get; set; } = null!;

    public ProgramStartRoutine()
    {
        
        CustomLogSystem.Debug("Read appSettings.json",false);
        ReadJsonSettings();
        CustomLogSystem.Debug("Read appSettings.json finish",false);
        
        CustomLogSystem.Debug("Load File Extensions In ListBox",false);
        LoadFileExtensionsInListBox();
        CustomLogSystem.Debug("Load File Extensions In ListBox finish",false);

        
        new LoadElementsIntoList(Settings.SourceStartPath,UiElementsBinding.SourceListBox!,true);
        new LoadElementsIntoList(Settings.TargetStartPath,UiElementsBinding.TargetListBox!,false);
        
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

    
}