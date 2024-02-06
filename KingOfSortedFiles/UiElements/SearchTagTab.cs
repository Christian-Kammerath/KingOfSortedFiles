using Avalonia.Controls;
using Avalonia.Media;

namespace KingOfSortedFiles.UiElements;

public class SearchTagTab: TextBlock
{
    public string SearchTag { get; set;}
    
    public SearchTagTab(string searchTag)
    {
        SearchTag = searchTag;

        Text = SearchTag;
        Foreground = Brushes.Azure;
        FontWeight = FontWeight.Bold;
        
    }
}