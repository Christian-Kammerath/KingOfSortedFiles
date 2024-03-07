using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace KingOfSortedFiles.UiElements;


//creates an overview of the settings made
public class SettingsShow : StackPanel
{
    
    //adds the individually created elements
    public SettingsShow()
    {
        Orientation = Orientation.Horizontal;
            
        Children.Add(GetExtensionsListBox());
        
        Children.Add(GetSearchByCheckBoxList());
        
        Children.Add(GetSortingCheckBoxesListArray()[0]);
        
        Children.Add(GetSortingCheckBoxesListArray()[1]);

        Children.Add(GetSourceDirectoryListBox());
        
        Children.Add(GetTargetDirectoryListBox());
    }

    //created a ListBox with the Selected TargetDirectoryPath
    private ListBox GetTargetDirectoryListBox()
    {
        var targetDirectoryListBox = new ListBox(){Height = 200, Width = 150};

        targetDirectoryListBox.Items.Add(new TextBlock() { Text = $"[Target Directory] \n\n{Path.GetFileName(UiElementsBinding.SortingSettings.TargetDirectoryPath)}" });

        return targetDirectoryListBox;
    }

    //created a ListBox with all Selected SourceDirectoryPath

    private ListBox GetSourceDirectoryListBox()
    {
        var sourceDirectoryListBox = new ListBox(){Height = 200, Width = 150};

        sourceDirectoryListBox.Items.Add(new TextBlock() { Text = "[Source Directory]" });
            
        UiElementsBinding.SortingSettings.SourceDirectoryPathList.ForEach(f =>
        {
            sourceDirectoryListBox.Items.Add(new TextBlock() { Text = Path.GetFileName(f) });
        });

        return sourceDirectoryListBox;
    }

    //created a ListBox with all Selected Fileextensions
    private static ListBox GetExtensionsListBox()
    {
        var extensionsList = new ListBox(){Height = 200, Width = 130};

        extensionsList.Items.Add(new TextBlock() { Text = "[File extensions]"});
            
        GetAllIsCheckedFileExtensions().ForEach(c =>
        {
            extensionsList.Items.Add(new TextBlock(){Text = c.Tag!.ToString()});
        });

        return extensionsList;
        
    }
    //created a ListBox with the file search settings and SearchTags
    private static ListBox GetSearchByCheckBoxList()
    {
        var searchByCheckBoxList = new ListBox(){Height = 200, Width = 150};
        var searchByCheckBox = UiElementsBinding.SortingSettings.SearchByCheckBoxes;

        searchByCheckBoxList.Items.Add(new TextBlock() { Text = "[Search by]"});
                
        if ((bool)searchByCheckBox!.SearchTags.IsChecked!)
            searchByCheckBoxList.Items.Add(new TextBlock(){Text = searchByCheckBox.SearchTags.Tag!.ToString()});
        if ((bool)searchByCheckBox.FileExtensions.IsChecked!)
            searchByCheckBoxList.Items.Add(new TextBlock(){Text = searchByCheckBox.FileExtensions.Tag!.ToString()});


        var searchTagsListbox = UiElementsBinding.SearchTagListBox;

        if (searchTagsListbox!.Items.Count > 0)
        {
            searchByCheckBoxList.Items.Add(new TextBlock() { Text = "[SearchTags]" });

            foreach (SearchTagTab? item in searchTagsListbox.Items)
            {
                searchByCheckBoxList.Items.Add(new TextBlock(){Text = item!.Text});
            }
        }

        return searchByCheckBoxList;
    }

    //created a ListBox with the settings of first first sort folders by for example created time and the same for second sort folders
    private static ListBox[] GetSortingCheckBoxesListArray()
    {
        var sortingCheckBoxList = UiElementsBinding.SortingSettings
            .SortCheckBoxes!;

        var sortingCheckBoxOneListBox = new ListBox(){Height = 200, Width = 180};

        sortingCheckBoxOneListBox.Items.Add(new TextBlock() { Text = "[first sort folders by]"});
            
            
        sortingCheckBoxList.SortOneCheckBoxList.ForEach(f =>
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(f!=null)
                if ((bool)f.IsChecked!)
                    sortingCheckBoxOneListBox.Items.Add(new TextBlock() { Text = f.Tag!.ToString() });
        });
            
                
        var sortingCheckBoxTwoListBox = new ListBox(){Height = 200, Width = 200};

        sortingCheckBoxTwoListBox.Items.Add(new TextBlock() { Text = "[second sort folders by]"});
            
            
        sortingCheckBoxList.SortTwoCheckBoxList.ForEach(f =>
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if(f != null)
                if ((bool)f.IsChecked!)
                    sortingCheckBoxTwoListBox.Items.Add(new TextBlock() { Text = f.Tag!.ToString() });
        });

        return [sortingCheckBoxOneListBox, sortingCheckBoxTwoListBox];

    }
    
    //Get a list of all Checked FileExtensions CheckBoxen

    private static List<CheckBox> GetAllIsCheckedFileExtensions()
    {
        var fileExtensionList = UiElementsBinding.FileExtensionListBox?.Items.OfType<FileExtensionTab>()
            .SelectMany(stack => stack.Children.OfType<StackPanel>())
            .SelectMany(stackPanel => stackPanel.Children.OfType<CheckBox>().Where(cb => cb.IsChecked == true))
            .ToList()!;

        UiElementsBinding.SortingSettings.FileExtensionList = fileExtensionList.Select(e => e.Tag!.ToString())
            .ToList()!;

        return fileExtensionList;

    }
    
}

//created a StackPanel with start button and ProgressBar
public class StartAndProgress : StackPanel
{
    public ProgressBar ProgressBar { get; set; }

    public StartAndProgress()
    {
        Orientation = Orientation.Horizontal;
        
        
        ProgressBar = new (){Minimum = 0,
            Maximum = 100,VerticalAlignment = VerticalAlignment.Stretch,
            HorizontalAlignment = HorizontalAlignment.Stretch,
        };

        var startButton = new Button() { Content = "Start", Background = Brushes.Green,HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center};
        startButton.Click += (_, _) =>
        {
            new Sorting(UiElementsBinding.SortingSettings).StartSorting();
        };
        
        
        Children.Add(ProgressBar);
        Children.Add(startButton);
        
    }
    
}

//Builds the ProcessTab
public class SortingProcessTab : StackPanel
{
    public StartAndProgress StartAndProgress { get; set; }
    public SortingProcessTab()
    {
        var logListBox = UiElementsBinding.LogListBox!;
        
        var lastSortingProcessTab = logListBox.Items
            .OfType<SortingProcessTab>().SingleOrDefault();

       
        logListBox.Items.Remove(lastSortingProcessTab);

        var settings = new SettingsShow();
        StartAndProgress = new StartAndProgress();
        
        Children.Add(settings);
        Children.Add(StartAndProgress);
        UiElementsBinding.SortingProcessTab = this;
    }
    
}

