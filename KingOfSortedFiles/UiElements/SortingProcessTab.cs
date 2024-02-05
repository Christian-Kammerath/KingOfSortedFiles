using System.Collections.Generic;
using System.IO;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace KingOfSortedFiles.UiElements;


public class SettingsShow : StackPanel
{
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

    private ListBox GetTargetDirectoryListBox()
    {
        var targetDirectoryListBox = new ListBox(){Height = 200, Width = 150};

        targetDirectoryListBox.Items.Add(new TextBlock() { Text = $"[Target Directory] \n\n{Path.GetFileName(UiElementsBinding.SortingSettings.TargetDirectoryPath)}" });

        return targetDirectoryListBox;
    }

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

    private static ListBox[] GetSortingCheckBoxesListArray()
    {
        var sortingCheckBoxList = UiElementsBinding.SortingSettings
            .SortCheckBoxes!;

        var sortingCheckBoxOneListBox = new ListBox(){Height = 200, Width = 180};

        sortingCheckBoxOneListBox.Items.Add(new TextBlock() { Text = "[first sort folders by]"});
            
            
        sortingCheckBoxList.SortOneCheckBoxList.ForEach(f =>
        {
            if(f!=null)
                if ((bool)f.IsChecked!)
                    sortingCheckBoxOneListBox.Items.Add(new TextBlock() { Text = f.Tag!.ToString() });
        });
            
                
        var sortingCheckBoxTwoListBox = new ListBox(){Height = 200, Width = 180};

        sortingCheckBoxTwoListBox.Items.Add(new TextBlock() { Text = "[second sort folders by]"});
            
            
        sortingCheckBoxList.SortTwoCheckBoxList.ForEach(f =>
        {
            if(f != null)
                if ((bool)f.IsChecked!)
                    sortingCheckBoxTwoListBox.Items.Add(new TextBlock() { Text = f.Tag!.ToString() });
        });

        return [sortingCheckBoxOneListBox, sortingCheckBoxTwoListBox];

    }
    
    
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

public class StartStopAndProgress : StackPanel
{
    private ProgressBar ProgressBar { get; set; }

    public StartStopAndProgress()
    {
        Orientation = Orientation.Horizontal;
        
        ProgressBar = new (){Minimum = 0,
            Maximum = 100,VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Left};

        var startButton = new Button() { Content = "Start", Background = Brushes.Green,HorizontalAlignment = HorizontalAlignment.Right,
            VerticalAlignment = VerticalAlignment.Center};
        startButton.Click += (sender, args) =>
        {
            new Sorting(UiElementsBinding.SortingSettings).StartSorting();
        };
        
        Children.Add(ProgressBar);
        Children.Add(startButton);
        
    }

    public void SetProgressValue(double value)
    {
        ProgressBar.Value = value;
    }


}

public class SortingProcessTab : StackPanel
{
    public SortingProcessTab()
    {
        var logListBox = UiElementsBinding.LogListBox!;
        
        var lastSortingProcessTab = logListBox.Items
            .OfType<SortingProcessTab>().SingleOrDefault();

       
        logListBox.Items.Remove(lastSortingProcessTab);
        
        Children.Add(new SettingsShow());
        Children.Add(new StartStopAndProgress());
        
    }
    
}