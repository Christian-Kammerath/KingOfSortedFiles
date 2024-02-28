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

    private DirectorySearch SourceDirectorySearch { get; set; } = new();
    private DirectorySearch TargetDirectorySearch { get; set; } = new();

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

    //If the path in the TextBox changes, the content in the folder is determined and loaded into the list
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

    //    //If the path in the TextBox changes, the content in the folder is determined and loaded into the list
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

    //Adds a new SearchTag
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

    //Adds the selected destination folder to the sortingSettings.
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

    //opens the settings file and saves changes
    private void OpensSettingFile_OnClick(object? sender, RoutedEventArgs e)
    {
        OpensSettingsFile.OpenFile(Path.Combine(Directory.GetCurrentDirectory(),"appSettings.json"));
    }


    //searches for hits with regex when entering changes to folders in the current directory
    private void TargetSearchBox_OnTextChanged(object? sender, TextChangedEventArgs e)
    {
        //loads the target ListBox and the search TextBox 
        var curedTargetPath = UiElementsBinding.TargetPathBox!.Text!;
        var selectedTextBox = (TextBox)sender!;
        
        //checks whether the current path is empty
        if(!string.IsNullOrEmpty(curedTargetPath))
        {
            //checks whether the text from the Search Box is empty, if so the ListBox is loaded and the elements from the current path are loaded into the ListBox
            if (!string.IsNullOrEmpty(selectedTextBox.Text))
            {
                //reads the text from the search TextBox and passes the search string to the  DirectorySearcher class
                var searchText = selectedTextBox.Text;
                TargetDirectorySearch.SearchString = searchText;

                
                if (SearchTaskTarget.Status != TaskStatus.RanToCompletion)
                {
                    var listBox = UiElementsBinding.TargetListBox!;
                    listBox.Items.Clear();

                    CustomLogSystem.Informational($"Directory search running: {listBox.Name}",true);

                    var token = CancelTokenTarget.Token;
                    
                    SearchTaskTarget = new Task(async () =>
                    {
                        await TargetDirectorySearch.ReadFolder(curedTargetPath,false,listBox,token);
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
                SourceDirectorySearch.SearchString = searchText;

                
                if (SearchTaskSource.Status != TaskStatus.RanToCompletion)
                {
                    var listBox = UiElementsBinding.SourceListBox!;
                    listBox.Items.Clear();

                    var token = CancelTokenSource.Token;
                    
                    CustomLogSystem.Informational($"Directory search running: {listBox.Name}",true);
                    
                    SearchTaskSource = new Task(async () =>
                    {
                        await SourceDirectorySearch.ReadFolder(curedSourcePath,true,listBox,token);
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