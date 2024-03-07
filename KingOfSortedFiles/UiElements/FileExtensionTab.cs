using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace KingOfSortedFiles.UiElements;

public class FileExtensionTab : StackPanel
{
    public FileExtensionTab(string[] fileExtensionArray, string categoryName)
    {
        Spacing = 5;
        Orientation = Orientation.Vertical;
        
        Children.Add(new TextBlock()
        {
            Text = categoryName,
            FontWeight = FontWeight.Bold,
            Foreground = Brushes.Azure
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
                lastStackPanel.Children.Add(new TextBlock()
                {
                    Text = fileExtensionArray[i],
                    FontWeight = FontWeight.Bold,
                    Foreground = Brushes.Azure
                });

                var checkBox = new CheckBox()
                {
                    Tag = fileExtensionArray[i],
                    FontWeight = FontWeight.Bold,
                };

                checkBox.IsCheckedChanged += (_, _) =>
                {
                    UiElementsBinding.LogListBox!.Items.Add(new SortingProcessTab());
                };
                
                lastStackPanel.Children.Add(checkBox);
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