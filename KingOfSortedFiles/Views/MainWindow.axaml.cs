using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles.Views;

public partial class MainWindow : Window
{
    public CancellationTokenSource CancelTokenSource = new();
    public CancellationTokenSource CancelTokenTarget = new();


    private Task SearchTaskSource = new(()=>{});
    private Task SearchTaskTarget = new(()=>{});

    // initial Window
    public MainWindow()
    {
        
        InitializeComponent();

        //creates a log system with file in bin folder
        CustomLogSystem
            .BindListBox(this.Find<ListBox>("LogListBox"))
            .BindLogFile(Path.Combine(Directory.GetCurrentDirectory(),"Log.txt"));
        
        CustomLogSystem.Debug("Start ProgramStartRoutine",false);
        
        //binds mainwindow
        UiElementsBinding.BindUiElements(this);
        
        //Starts the start rotines
        new ProgramStartRoutine();
        
        
        CustomLogSystem.Debug("ProgramStartRoutine finish",false);

        
        CustomLogSystem.Informational("Program is ready to start",true);
        
    }

    private void SourcePathBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var sourcePathBox = UiElementsBinding.SourcePathBox;
        var sourceListBox = UiElementsBinding.SourceListBox;

        if (Directory.Exists(sourcePathBox!.Text) || string.IsNullOrEmpty(sourcePathBox.Text))
            new LoadElementsIntoList(sourcePathBox.Text!, sourceListBox!,false);
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
            new LoadElementsIntoList(targetPathBox.Text!, targetListBox!,false);
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
            var listBox = UiElementsBinding.SearchTagListBox;
            listBox!.Items.Add(new SearchTagTab(searchTagString));
            UiElementsBinding.SortingSettings.SearchTagList = new SearchTags(listBox!);
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
                
                var searchText = selectedTextBox.Text;
                DirectorySearch.SearchString = searchText;

                
                if (SearchTaskTarget.Status != TaskStatus.RanToCompletion)
                {
                    var listBox = UiElementsBinding.TargetListBox!;
                    listBox.Items.Clear();

                    var token = CancelTokenTarget.Token;
                    
                    SearchTaskTarget = new Task(async () =>
                    {
                        await DirectorySearch.ReadFolder(curedTargetPath,false,listBox,token);
                    },CancelTokenTarget.Token);
                    
                    SearchTaskTarget.Start();
                }
           
            }
            
            else
            {
                UiElementsBinding.TargetListBox!.Items.Clear();
                new LoadElementsIntoList(curedTargetPath, UiElementsBinding.TargetListBox,false);

            }
            
        }
        
    }

    private void  SourceSearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        var curedSourcePath = UiElementsBinding.SourcePathBox?.Text;
        var selectedTextBox = (TextBox)sender!;
        
        if(!string.IsNullOrEmpty(curedSourcePath))
        {
            
            if (!string.IsNullOrEmpty(selectedTextBox.Text))
            {
                
                var searchText = selectedTextBox.Text;
                DirectorySearch.SearchString = searchText;

                
                if (SearchTaskSource.Status != TaskStatus.RanToCompletion)
                {
                    var listBox = UiElementsBinding.SourceListBox!;
                    listBox.Items.Clear();

                    var token = CancelTokenSource.Token;
                    
                    CustomLogSystem.Informational($"Directory search running: {listBox.Name}",true);
                    
                    SearchTaskSource = new Task(async () =>
                    {
                        await DirectorySearch.ReadFolder(curedSourcePath,true,listBox,token);
                    },CancelTokenSource.Token);
                    
                    SearchTaskSource.Start();
                }
                
            }
            else
            {
                UiElementsBinding.SourceListBox?.Items.Clear();
                new LoadElementsIntoList(curedSourcePath, UiElementsBinding.SourceListBox!,false);

            }    
            
        }    
        
    }


   
}