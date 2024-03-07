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

//classe for searching and outputting directories
public  class DirectorySearch
{
    public  string SearchString { get; set; } = null!;
    private  bool _searchIsRunning = true;

    public  async Task ReadFolder(string path, bool isSource, ListBox listBox, CancellationToken token)
    {
        try
        {
            //checks whether the task has received an abort command
            if (token.IsCancellationRequested)
            {
                if (_searchIsRunning)
                {
                    await Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        CustomLogSystem.Informational($"Directory search ended: {listBox.Name}", true);
                        _searchIsRunning = false;
                    });
                    
                }
                
                token.ThrowIfCancellationRequested();
                
            }
            
            //reads the directorys from the current path
            var dirList = Directory.GetDirectories(path).ToList();

            foreach (var dir in dirList)
            {
                var folderFirst = new DirectoryInfo(dir);
                
                
                if (
                    (folderFirst.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (folderFirst.Attributes & FileAttributes.System) == FileAttributes.System ||
                    (folderFirst.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    continue;
                }
                
                //If the name of the directory contains the search string, the result is loaded into the corresponding ListBox
                if (Regex.IsMatch(folderFirst.Name.ToUpper(), SearchString.ToUpper()))
                {
                    await LoadInListBoxAsync(listBox,folderFirst,isSource);
                }
                
                //Deletes all old results from the listbox that no longer match the searchString
                await CleanListAsync(listBox,isSource);
                
                //Calls the recursive function to search the subfolders
                await ReadFolder(folderFirst.FullName, isSource, listBox,token);
                
            }
        }
        catch (Exception)
        {
            //Deletes all old results from the listbox that no longer match the searchString
            await CleanListAsync(listBox,isSource);
        }
    }

    //Loads directory into ListBox 
    public  async Task LoadInListBoxAsync(ListBox listBox, DirectoryInfo directoryInfo, bool isSource)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            var folderTab = isSource ? new SourceFolderTab(directoryInfo,true): new TargetFolderTab(directoryInfo,false);
            listBox.Items.Add(folderTab);
        });
    }

    //Clears list box
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

    


