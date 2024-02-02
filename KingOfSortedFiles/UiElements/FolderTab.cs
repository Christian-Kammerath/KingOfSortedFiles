using System.IO;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace KingOfSortedFiles.UiElements;

public class TargetFolderTab : StackPanel
{
    public string FolderName { get; set; }
    public string FolderPath { get; set; }

    public TargetFolderTab(DirectoryInfo directoryInfo)
    {

        FolderName = directoryInfo.Name;
        FolderPath = directoryInfo.FullName;
        
        Spacing = 100;
        
        var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Folder.ico");
        var icon = new Image() { Source = new Bitmap(iconPath), Width = 60, Height = 100,VerticalAlignment = VerticalAlignment.Center};
        
        icon.DoubleTapped += (sender, args) =>
        {
            var listBox = Parent as ListBox;
            new LoadElementsIntoList(FolderPath, listBox!);
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
    public SourceFolderTab(DirectoryInfo directoryInfo) : base(directoryInfo)
    {
        var checkBoxStack = new StackPanel(){Orientation = Orientation.Horizontal};
        var selectCheckBox = new CheckBox() { Tag = FolderPath, VerticalAlignment = VerticalAlignment.Center };

        selectCheckBox.IsCheckedChanged += (sender, args) =>
        {
            SortingSettings.SourceDirectoryPathList.Add(Path.Combine(FolderPath,FolderName));
            UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
        };  
        
        checkBoxStack.Children.Add(selectCheckBox);
        checkBoxStack.Children.Add(new TextBlock(){Text = "Add folders to sort", Foreground = Brushes.Azure,
            FontWeight = FontWeight.Bold,VerticalAlignment = VerticalAlignment.Center});
        Children.Add(checkBoxStack);
    }
    
    
    
}