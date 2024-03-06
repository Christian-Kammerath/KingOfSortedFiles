using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using KingOfSortedFiles.Views;

namespace KingOfSortedFiles.UiElements;

public class TargetFolderTab : StackPanel
{
    public string FolderName { get; set; }
    public string FolderPath { get; set; }

    public TargetFolderTab(DirectoryInfo directoryInfo,bool isSource)
    {

        FolderName = directoryInfo.Name;
        FolderPath = directoryInfo.FullName;
        
        Spacing = 100;
        
        var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "Icons", "Folder.ico");
        var icon = new Image() { Source = new Bitmap(iconPath), Width = 60, Height = 100,VerticalAlignment = VerticalAlignment.Center};
        
        icon.DoubleTapped += (sender, args) =>
        {
            if (isSource)
            {
                try
                {
                    UiElementsBinding.MainWindowReference.CancelTokenSource.Cancel();
                    UiElementsBinding.MainWindowReference.CancelTokenSource.Dispose();
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e);
                }
               
            }
            else
            {
                try
                {
                    UiElementsBinding.MainWindowReference.CancelTokenTarget.Cancel();
                    UiElementsBinding.MainWindowReference.CancelTokenTarget.Dispose();
                }
                catch (ObjectDisposedException e)
                {
                    Console.WriteLine(e);
                }
            }
            
            var listBox = Parent as ListBox;
            new LoadElementsIntoList(FolderPath, listBox!,false);
        };
        
        Orientation = Orientation.Horizontal;
        
        Children.Add(icon);
        Children.Add(new TextBlock()
        {
            Text = FolderName,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Azure,
            VerticalAlignment = VerticalAlignment.Center
        });

        
    }
    
}

public class SourceFolderTab : TargetFolderTab
{
    public SourceFolderTab(DirectoryInfo directoryInfo, bool isSource) : base(directoryInfo,isSource)
    {
        var checkBoxStack = new StackPanel(){Orientation = Orientation.Horizontal};
        var selectCheckBox = new CheckBox() { Tag = FolderPath, VerticalAlignment = VerticalAlignment.Center };

        selectCheckBox.IsCheckedChanged += (sender, args) =>
        {
            UiElementsBinding.SortingSettings.SourceDirectoryPathList.Add(FolderPath);
            UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
        };  
        
        checkBoxStack.Children.Add(selectCheckBox);
        checkBoxStack.Children.Add(new TextBlock(){Text = "Add folders to sort", Foreground = Brushes.Azure,
            FontWeight = FontWeight.Bold,VerticalAlignment = VerticalAlignment.Center});
        Children.Add(checkBoxStack);
    }
    
    
    
}