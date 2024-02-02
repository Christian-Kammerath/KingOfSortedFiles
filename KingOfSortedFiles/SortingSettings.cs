using System.Collections.Generic;
using Avalonia.Controls;
using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles;

public static class SortingSettings
{
    public static SortCheckBoxes? SortCheckBoxes { get; set; }
    public static SearchByCheckBoxes? SearchByCheckBoxes { get; set; } = new();

    public static List<string> SourceDirectoryPathList { get; set; } = new();

    public static string TargetDirectoryPath { get; set; } = null!;
}

public class SearchByCheckBoxes
{
    private CheckBox _fileExtensions = null!;
    public CheckBox FileExtensions
    {
        get => _fileExtensions;
        set
        {
            _fileExtensions = value;

            _fileExtensions.IsCheckedChanged += (sender, args) =>
            {
                UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
            };
        }
    }

    private CheckBox _searchTags = null!;
    public CheckBox SearchTags
    {
        get => _searchTags;
        set
        {
            _searchTags = value;

            _searchTags.IsCheckedChanged += (sender, args) =>
            {
                UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
            };
        }
    }
}

public  class SortCheckBoxes

{
    public List<CheckBox> SortOneCheckBoxList { get; set; } = null!;
    public List<CheckBox> SortTwoCheckBoxList { get; set; } = null!;

    public SortCheckBoxes(CheckBox createdOne,CheckBox changedOne,CheckBox lastUpdateOne,CheckBox fileExtensionOne,CheckBox searchTagOne,
                           CheckBox createdTwo, CheckBox changedTwo, CheckBox lastUpdateTwo, CheckBox fileExtensionTwo, CheckBox searchTagTwo)
    {
        SortOneCheckBoxList =
        [
            createdOne,
            changedOne,
            lastUpdateOne,
            fileExtensionOne,
            searchTagOne
        ];

        SortTwoCheckBoxList =
        [
            createdTwo,
            changedTwo,
            lastUpdateTwo,
            fileExtensionTwo,
            searchTagTwo
        ];

        SortOneCheckBoxList?.ForEach(c =>
        {
            if (c != null)
            {
                c.Click += (sender, args) =>
                {
                    var currentCheckBox = (CheckBox)sender!;

                    SortOneCheckBoxList?.ForEach(cb =>
                    {
                        if (cb != null && cb != currentCheckBox)
                            cb.IsChecked = false;
                    });

                    UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
                };
            }
        });
        
        SortTwoCheckBoxList?.ForEach(c =>
        {
            if (c != null)
            {
                c.Click += (sender, args) =>
                {
                    var currentCheckBox = (CheckBox)sender!;

                    SortTwoCheckBoxList?.ForEach(cb =>
                    {
                        if (cb != null && cb != currentCheckBox)
                            cb.IsChecked = false;
                    });

                    UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
                };
            }
        });

    }
    
}