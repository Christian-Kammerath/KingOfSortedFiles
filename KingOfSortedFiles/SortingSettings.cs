using System.Collections.Generic;
using Avalonia.Controls;
using KingOfSortedFiles.UiElements;

namespace KingOfSortedFiles;

public  class SortingSettings
{
    public  SortCheckBoxes? SortCheckBoxes { get; set; }
    public  SearchByCheckBoxes? SearchByCheckBoxes { get; set; } = new();

    public  List<string> SourceDirectoryPathList { get; set; } = new();

    public  string TargetDirectoryPath { get; set; } = null!;

    public  List<string> FileExtensionList { get; set; } = new();
    
    public  MoveAndOrCopy MoveAndOrCopy { get; set; } = null!;
}

public class MoveAndOrCopy
{
    public bool MoveOnly { get; set; }
    public bool CopyAndMove { get; set; }
    public MoveAndOrCopy(CheckBox moveOnly, CheckBox copyAndMove)
    {
        moveOnly.Click += (sender, args) =>
        {
            if (MoveOnly is false)
            {
                copyAndMove.IsChecked = false;
                moveOnly.IsChecked = true;
                MoveOnly = true;
                CopyAndMove = false;
            }
        };
        
        copyAndMove.IsCheckedChanged += (sender, args) =>
        {
            if (CopyAndMove is false)
            {
                moveOnly.IsChecked = false;
                copyAndMove.IsChecked = true;
                CopyAndMove = true;
                MoveOnly = false;   
            }
        };
    }
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
    public List<CheckBox> SortOneCheckBoxList { get; set; } = new();
    public List<CheckBox> SortTwoCheckBoxList { get; set; } = new();

    public SortCheckBoxes(CheckBox createdOne,CheckBox changedOne,CheckBox lastAccessTimeOne,CheckBox fileExtensionOne,CheckBox searchTagOne,
                           CheckBox createdTwo, CheckBox changedTwo, CheckBox lastAccessTimeTwo, CheckBox fileExtensionTwo, CheckBox searchTagTwo)
    {
        SortOneCheckBoxList =
        [
            createdOne,
            changedOne,
            lastAccessTimeOne,
            fileExtensionOne,
            searchTagOne
        ];

        SortTwoCheckBoxList =
        [
            createdTwo,
            changedTwo,
            lastAccessTimeTwo,
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