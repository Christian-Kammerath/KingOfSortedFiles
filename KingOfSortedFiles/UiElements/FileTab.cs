using System;
using System.Collections.Generic;
using System.IO;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace KingOfSortedFiles.UiElements;

public class FileTab : StackPanel
{
    private Dictionary<string, string> ExtensionIconDictionary { get; set; } = new()
    {
        {".pdf","pdf.ico"},
        {".txt","Txt.ico"}
    };

    private string FileName { get; set; }

    public FileTab(FileInfo fileInfo)
    {
        FileName = fileInfo.Name;
        
        Spacing = 100;
        
        var iconPath = GetIconPath(fileInfo.Extension);
        var icon = new Image() { Source = new Bitmap(iconPath), Width = 60, Height = 100,VerticalAlignment = VerticalAlignment.Center};
        
        Orientation = Orientation.Horizontal;
        
        Children.Add(icon);
        Children.Add(new TextBlock()
        {
            Text = FileName,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Azure,
            VerticalAlignment = VerticalAlignment.Center
        });
        
    }

    private string GetIconPath(string fileExtension)
    {
        try
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Icons","FileIcons", ExtensionIconDictionary[fileExtension]);

        }
        catch (Exception e)
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Icons","FileIcons", "PlaceHolder.ico");

        }
        
    }
    
}