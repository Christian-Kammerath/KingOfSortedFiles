using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Threading;
using KingOfSortedFiles.UiElements;


namespace KingOfSortedFiles;

public  class DirectorySearch
{
    public  string SearchString { get; set; }
    private  bool SearchIsRunning = true;

    public  async Task ReadFolder(string path, bool isSource, ListBox listBox, CancellationToken token)
    {
        try
        {
            
            if (token.IsCancellationRequested)
            {
                if (SearchIsRunning)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CustomLogSystem.Informational($"Directory search ended: {listBox.Name}", true);
                        SearchIsRunning = false;
                    });
                    
                }
                
                token.ThrowIfCancellationRequested();
                
            }
            
            var dir = Directory.GetDirectories(path).ToList();
            
            for(int i = 0; i < dir.Count; i++)
            {
                var folderFirst = new DirectoryInfo(dir[i]);
                
                if (
                    (folderFirst.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (folderFirst.Attributes & FileAttributes.System) == FileAttributes.System ||
                    (folderFirst.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    continue;
                }
                
                if (Regex.IsMatch(folderFirst.Name.ToUpper(), SearchString.ToUpper()))
                {
                    await LoadInListBoxAsync(listBox,folderFirst,isSource);
                }

                await CleanListAsync(listBox,isSource);
                await ReadFolder(folderFirst.FullName, isSource, listBox,token);
                
            }
        }
        catch (Exception e)
        {
            await CleanListAsync(listBox,isSource);
        }
    }

    public  async Task LoadInListBoxAsync(ListBox listBox, DirectoryInfo directoryInfo, bool isSource)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var folderTab = isSource ? new SourceFolderTab(directoryInfo,true): new TargetFolderTab(directoryInfo,false);
            listBox.Items.Add(folderTab);
        });
    }

    public  async Task CleanListAsync(ListBox listBox, bool isSource)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var searchText = UiElementsBinding.SourceSearchBox.Text;
            
            var folderTabType = isSource ? typeof(SourceFolderTab) : typeof(TargetFolderTab);

            var itemsToRemove = listBox.Items
                .Cast<object>() 
                .Where(d => d.GetType() == folderTabType && !Regex.IsMatch(((dynamic)d).FolderName.ToUpper(), searchText!.ToUpper()))
                .ToList();


            foreach (var itemToRemove in itemsToRemove)
            {
                listBox.Items.Remove(itemToRemove);
            }
        });
    }
}

    


