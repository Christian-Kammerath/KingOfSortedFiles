using System.IO;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace KingOfSortedFiles.Views;

public partial class SettingWindow : Window
{
    public SettingWindow()
    {
        InitializeComponent();
    }

    private void SaveButton_OnClick(object? sender, RoutedEventArgs e)
    {
        OpensSettingsFile.SaveFile(Path.Combine(Directory.GetCurrentDirectory()
            ,"appSettings.json"));
    }
}