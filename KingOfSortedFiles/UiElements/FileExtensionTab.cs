using Avalonia.Controls;
using Avalonia.Layout;

namespace KingOfSortedFiles.UiElements;

public class FileExtensionTab : StackPanel
{
    public FileExtensionTab(string[] fileExtensionArray, string categoryName)
    {
        Spacing = 5;
        Orientation = Orientation.Vertical;
        
        Children.Add(new TextBlock()
        {
            Text = categoryName
        });

        var increase = 2;
        
        var lastStackPanel = new StackPanel()
        {
            Orientation = Orientation.Horizontal,
            Spacing = 5
        };
        
        for(int i = 0; i < fileExtensionArray.Length; i++) 
        {
            if (i <= increase)
            {
                lastStackPanel.Children.Add(new TextBlock(){Text = fileExtensionArray[i]});
                lastStackPanel.Children.Add(new CheckBox(){Tag = fileExtensionArray[i]});
            }
            else
            {
                increase += 3;
                
                Children.Add(lastStackPanel);
                lastStackPanel = new StackPanel(){ Orientation = Orientation.Horizontal,Spacing = 5};
                i--;
            }
            
        }
        if(lastStackPanel.Children.Count > 0)
            Children.Add(lastStackPanel);

    }
}