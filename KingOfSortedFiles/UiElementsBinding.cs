using Avalonia.Controls;
using KingOfSortedFiles.Views;

namespace KingOfSortedFiles;

public static class UiElementsBinding
{
    public static ListBox? SourceListBox { get; set; }
    public static ListBox? FileExtensionListBox { get; set; }
    
    public static ListBox? TargetListBox { get; set; }
    
    public static TextBox? SourcePathBox { get; set; } 
    
    public static TextBox? TargetPathBox { get; set; }
    
    public static ListBox? SearchTagListBox { get; set; }
    
    public static TextBox? SearchTagTextBox { get; set; }
    public static ListBox? LogListBox { get; set; }
    
    public static void BindUiElements(MainWindow mainWindow)
    {
        SourceListBox = mainWindow.Find<ListBox>("SourceListBox");
        FileExtensionListBox = mainWindow.Find<ListBox>("FileExtensionListBox");
        TargetListBox = mainWindow.Find<ListBox>("TargetListBox");
        SourcePathBox = mainWindow.Find<TextBox>("SourcePathBox");
        TargetPathBox = mainWindow.Find<TextBox>("TargetPathBox");
        SearchTagListBox = mainWindow.Find<ListBox>("SearchTagListBox");
        SearchTagTextBox = mainWindow.Find<TextBox>("SearchTagTextBox");
        LogListBox = mainWindow.Find<ListBox>("LogListBox");
        
        LoadCheckBoxesToSettings(mainWindow);
    }

    private static void LoadCheckBoxesToSettings(MainWindow mainWindow)
    {

        SortingSettings.SortCheckBoxes = new SortCheckBoxes(
            mainWindow.Find<CheckBox>("CreatedOne")!,
            mainWindow.Find<CheckBox>("ChangedOne")!,
            mainWindow.Find<CheckBox>("LastUpdateOne")!,
            mainWindow.Find<CheckBox>("FileExtensionOne")!,
            mainWindow.Find<CheckBox>("SearchTagOne")!,

            mainWindow.Find<CheckBox>("CreatedTwo")!,
            mainWindow.Find<CheckBox>("ChangedTwo")!,
            mainWindow.Find<CheckBox>("LastUpdatedTwo")!,
            mainWindow.Find<CheckBox>("FileExtensionTwo")!,
            mainWindow.Find<CheckBox>("SearchTagTwo")!
        );

        SortingSettings.SearchByCheckBoxes!.FileExtensions =
            mainWindow.Find<CheckBox>("SearchFileExtension")!;
        
        SortingSettings.SearchByCheckBoxes!.SearchTags =
            mainWindow.Find<CheckBox>("SearchSearchTags")!;

    }
    
}