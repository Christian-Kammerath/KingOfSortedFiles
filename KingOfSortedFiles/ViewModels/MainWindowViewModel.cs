using Avalonia.Media;

namespace KingOfSortedFiles.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IBrush ButtonColor => new SolidColorBrush(Color.Parse("#323233"));
    public IBrush ListBoxColor => new SolidColorBrush(Color.Parse("#2a2a2b"));

    public IBrush TextColor => new SolidColorBrush(Colors.Azure);

}