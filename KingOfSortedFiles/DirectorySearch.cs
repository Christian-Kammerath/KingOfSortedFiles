using System;
using System.Collections.Generic;
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
        //batch collection for performance - update UI less frequently
        var batchFolders = new List<DirectoryInfo>();
        const int batchSize = 50; //update UI every 50 folders instead of every single one
        
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
            
            //use EnumerateDirectories instead of GetDirectories for lazy loading (better memory usage)
            var directories = Directory.EnumerateDirectories(path);

            foreach (var dir in directories)
            {
                var folderFirst = new DirectoryInfo(dir);
                
                
                if (
                    (folderFirst.Attributes & FileAttributes.Hidden) == FileAttributes.Hidden ||
                    (folderFirst.Attributes & FileAttributes.System) == FileAttributes.System ||
                    (folderFirst.Attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint)
                {
                    continue;
                }
                
                //If the name of the directory contains the search string, add to batch
                if (Regex.IsMatch(folderFirst.Name.ToUpper(), SearchString.ToUpper()))
                {
                    batchFolders.Add(folderFirst);
                    
                    //flush batch when it reaches batchSize
                    if (batchFolders.Count >= batchSize)
                    {
                        await LoadBatchInListBoxAsync(listBox, batchFolders, isSource);
                        batchFolders.Clear();
                    }
                }
                
                //Calls the recursive function to search the subfolders
                await ReadFolder(folderFirst.FullName, isSource, listBox, token);
                
            }
            
            //flush remaining folders in batch
            if (batchFolders.Count > 0)
            {
                await LoadBatchInListBoxAsync(listBox, batchFolders, isSource);
                batchFolders.Clear();
            }
            
            //IMPORTANT: only clean list ONCE at the end, not after every folder!
            await CleanListAsync(listBox, isSource);
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
    
    //Loads multiple directories into ListBox in a single UI update (much faster!)
    private async Task LoadBatchInListBoxAsync(ListBox listBox, List<DirectoryInfo> directories, bool isSource)
    {
        await Dispatcher.UIThread.InvokeAsync(() =>
        {
            foreach (var dir in directories)
            {
                var folderTab = isSource ? new SourceFolderTab(dir, true) : new TargetFolderTab(dir, false);
                listBox.Items.Add(folderTab);
            }
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

    


