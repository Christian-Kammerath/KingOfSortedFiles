using System.IO;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media.Imaging;

namespace KingOfSortedFiles.UiElements;

public class DriverTab: StackPanel
{
    public DriverTab(Drive driveInfo)
    {

        Spacing = 100;
        
        var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "Assets", "Drive.ico");
        var icon = new Image() { Source = new Bitmap(iconPath), Width = 30, Height = 50};
        
        Orientation = Orientation.Horizontal;

        var iconAndNameStackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5,
        };
        
        
        iconAndNameStackPanel.Children.Add(icon);
        iconAndNameStackPanel.Children.Add(new TextBlock()
        {
            Text = driveInfo.Name
        });
        
        Children.Add(iconAndNameStackPanel);
        Children.Add(new TextBlock()
        {
            Text = driveInfo.MountPoint,
        });
    }
}