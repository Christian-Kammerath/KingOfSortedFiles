using Avalonia.Controls;
using KingOfSortedFiles.Views;

namespace KingOfSortedFiles;

public static class UiElementsBinding
{
    public static ListBox? SourceListBox { get; set; }
    public static ListBox? FileExtensionListBox { get; set; }
    
    public static ListBox? TargetListBox { get; set; }

    public static void BindUiElements(MainWindow mainWindow)
    {
        SourceListBox = mainWindow.Find<ListBox>("SourceListBox");
        FileExtensionListBox = mainWindow.Find<ListBox>("FileExtensionListBox");
        TargetListBox = mainWindow.Find<ListBox>("TargetListBox");
    }
}