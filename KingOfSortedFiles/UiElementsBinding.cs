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

    public static ListBox? SearchTagListBox { get; set; } = new();
    
    public static TextBox? SearchTagTextBox { get; set; }
    public static ListBox? LogListBox { get; set; }
    
    public static TextBox SourceSearchBox { get; set; } = null!;
    
    public static TextBox TargetSearchBox { get; set; } = null!;

    
    public static SortingSettings SortingSettings { get; set; }

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
        SourceSearchBox = mainWindow.Find<TextBox>("SourceSearchBox")!;
        TargetSearchBox = mainWindow.Find<TextBox>("TargetSearchBox")!;
        
        
        LoadCheckBoxesToSettings(mainWindow);
        LoadCopyAndOrMoveCheckBoxes(mainWindow);
        LoadSearchTags(SearchTagListBox!);
    }

    private static void LoadSearchTags(ListBox listBox)
    {
        SortingSettings.SearchTagList = new SearchTags(listBox);
    }
    private static void LoadCopyAndOrMoveCheckBoxes(MainWindow mainWindow)
    {
        
        SortingSettings.MoveAndOrCopy = new MoveAndOrCopy(
            mainWindow.Find<CheckBox>("MoveOnly")!,
            mainWindow.Find<CheckBox>("CopyAndMove")!
            );
    }

    private static void LoadCheckBoxesToSettings(MainWindow mainWindow)
    {

        SortingSettings = new SortingSettings();
        
        SortingSettings.SortCheckBoxes = new SortCheckBoxes(
            mainWindow.Find<CheckBox>("CreatedOne")!,
            mainWindow.Find<CheckBox>("ChangedOne")!,
            mainWindow.Find<CheckBox>("LastAccessTimeOne")!,
            mainWindow.Find<CheckBox>("FileExtensionOne")!,
            mainWindow.Find<CheckBox>("SearchTagOne")!,

            mainWindow.Find<CheckBox>("CreatedTwo")!,
            mainWindow.Find<CheckBox>("ChangedTwo")!,
            mainWindow.Find<CheckBox>("LastAccessTimeTwo")!,
            mainWindow.Find<CheckBox>("FileExtensionTwo")!,
            mainWindow.Find<CheckBox>("SearchTagTwo")!
        );

        SortingSettings.SearchByCheckBoxes!.FileExtensions =
            mainWindow.Find<CheckBox>("SearchFileExtension")!;
        
        SortingSettings.SearchByCheckBoxes!.SearchTags =
            mainWindow.Find<CheckBox>("SearchSearchTags")!;

    }
    
}