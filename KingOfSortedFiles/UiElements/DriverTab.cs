using System.IO;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace KingOfSortedFiles.UiElements;

public class DriverTab : StackPanel
{
    public DriverTab(Drive driveInfo)
    {
        Spacing = 100;

        var iconPath = Path.Combine(Directory.GetCurrentDirectory(), "Icons", "Drive.ico");
        var icon = new Image() { Source = new Bitmap(iconPath), Width = 30, Height = 50, };
        
        icon.DoubleTapped += (sender, args) =>
        {
            var listBox = Parent as ListBox;
            new LoadElementsIntoList(driveInfo.MountPoint, listBox!,false);
        };

        Orientation = Orientation.Horizontal;

        Children.Add(icon);
        Children.Add(new TextBlock()
        {
            Text = driveInfo.Name,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Azure,
            VerticalAlignment = VerticalAlignment.Center
        });

        Children.Add(new TextBlock()
        {
            Text = driveInfo.MountPoint,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Azure,
            VerticalAlignment = VerticalAlignment.Center
        });
    }
}
