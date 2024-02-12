using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        
        InitializeComponent();

        CustomLogSystem
            .BindListBox(this.Find<ListBox>("LogListBox"))
            .BindLogFile(Path.Combine(Directory.GetCurrentDirectory(),"Log.txt"));
        
        CustomLogSystem.Debug("Start ProgramStartRoutine",false);
        UiElementsBinding.BindUiElements(this);
        new ProgramStartRoutine();
        CustomLogSystem.Debug("ProgramStartRoutine finish",false);

        
        CustomLogSystem.Informational("Program is ready to start",true);
        
    }

    private void SourcePathBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var sourcePathBox = UiElementsBinding.SourcePathBox;
        var sourceListBox = UiElementsBinding.SourceListBox;

        if (Directory.Exists(sourcePathBox!.Text) || string.IsNullOrEmpty(sourcePathBox.Text))
            new LoadElementsIntoList(sourcePathBox.Text!, sourceListBox!);
        else
        {
            sourceListBox!.Items.Clear();
            sourceListBox.Items.Add(new TextBlock() { Text = "Path not found" });
        }

    }

    private void TargetPathBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var targetPathBox = UiElementsBinding.TargetPathBox;
        var targetListBox = UiElementsBinding.TargetListBox;
        
        if (Directory.Exists(targetPathBox!.Text) || string.IsNullOrEmpty(targetPathBox.Text))
            new LoadElementsIntoList(targetPathBox.Text!, targetListBox!);
        else
        {
            targetListBox!.Items.Clear();
            targetListBox.Items.Add(new TextBlock() { Text = "Path not found" });
        }
            
    }

    private void AddNewSearchTagButton_OnClick(object? sender, RoutedEventArgs e)
    {

        var searchTagString = UiElementsBinding.SearchTagTextBox!.Text;
        
        if (searchTagString != "Add new Search Tag" &&
            !string.IsNullOrEmpty(searchTagString))
        {
            UiElementsBinding.SearchTagListBox!.Items.Add(new SearchTagTab(searchTagString));
            UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
        }
    }

    private void TargetListBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        var selectedItem = UiElementsBinding.TargetListBox!.SelectedItem;
        if (selectedItem != null)
        {
            if (selectedItem is TargetFolderTab targetFolderTab)
            {
                var folderTab = (TargetFolderTab)selectedItem!;
                UiElementsBinding.SortingSettings.TargetDirectoryPath = Path.Combine(folderTab.FolderPath);
                UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
            }
            
        }
        
        
    }

    private void OpensSettingFile_OnClick(object? sender, RoutedEventArgs e)
    {
        OpensSettingsFile.OpenFile(Path.Combine(Directory.GetCurrentDirectory(),"appSettings.json"));
    }


    private void TargetSearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var curedTargetPath = UiElementsBinding.TargetPathBox!.Text!;
        var selectedTextBox = (TextBox)sender!;
        
        if(!string.IsNullOrEmpty(curedTargetPath))
        {
            if (!string.IsNullOrEmpty(selectedTextBox.Text))
            {

                var directory = DirectorySearch.GetDirectory(curedTargetPath, selectedTextBox.Text);
                
              UiElementsBinding.TargetListBox!.Items.Clear();
              foreach (var dir in directory)
              {
                  UiElementsBinding.TargetListBox.Items.Add(new TargetFolderTab(dir));
              }

            }
            
            else
            {
                UiElementsBinding.TargetListBox!.Items.Clear();
                new LoadElementsIntoList(curedTargetPath, UiElementsBinding.TargetListBox);

            }
            
        }
        
    }

    private void SourceSearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var curedSourcePath = UiElementsBinding.SourcePathBox!.Text!;
        var selectedTextBox = (TextBox)sender!;
        
        if(!string.IsNullOrEmpty(curedSourcePath))
        {

            if (!string.IsNullOrEmpty(selectedTextBox.Text))
            {
                var directoryInfos = DirectorySearch.GetDirectory(curedSourcePath,selectedTextBox.Text);
                    
                UiElementsBinding.SourceListBox!.Items.Clear();
                foreach (var dir in directoryInfos)
                {
                    UiElementsBinding.SourceListBox.Items.Add(new TargetFolderTab(dir));
                }
                    
            }
            else
            {
                UiElementsBinding.TargetListBox!.Items.Clear();
                new LoadElementsIntoList(curedSourcePath, UiElementsBinding.TargetListBox);

            }    
            
        }    
        
    }


   
}