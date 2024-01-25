using KingOfSortedFiles;

namespace TestProject1;

public class DriveInfoLinuxTest
{
    [Fact]
    public void Simulate_lsblk_Call_Returns_List_of_Drive()
    {
        //Arrange
        var allMounted = @"NAME              SIZE FSTYPE      MOUNTPOINT
                                loop0               0B             
                                loop1               0B             
                                loop2               0B             
                                loop3               0B             
                                loop4               0B             
                                loop5               0B             
                                loop6               0B             
                                loop7               0B             
                                sda             119,2G             
                                ├─sda1          117,7G ntfs        /media/Bob/280E51320E50FA70
                                └─sda2            525M ntfs        /media/Bob/CE3690DD3690C7BB
                                sdb             931,5G             
                                ├─sdb1             99M             
                                └─sdb2          931,4G ntfs        /mnt/mydrive
                                sdc             447,1G             
                                └─sdc1          447,1G ntfs        /media/Bob/Volume1
                                zram0              16G             [SWAP]
                                nvme0n1         465,8G             
                                ├─nvme0n1p1      1022M vfat        /boot/efi
                                ├─nvme0n1p2         4G vfat        /recovery
                                ├─nvme0n1p3     456,8G crypto_LUKS 
                                │ └─cryptdata   456,7G LVM2_member 
                                │   └─data-root 456,7G ext4        /
                                └─nvme0n1p4         4G swap        
                                  └─cryptswap       4G swap        [SWAP]
                                ";

        var expectedList = new List<Drive>()
        {
            new Drive(){FsType = "ntfs",MountPoint = "/media/Bob/280E51320E50FA70",Name = "sda1",Size = "117,7G"},
            new Drive(){FsType = "ntfs",MountPoint = "/media/Bob/CE3690DD3690C7BB",Name = "sda2",Size ="525M" },
            new Drive(){FsType = "ntfs",MountPoint ="/mnt/mydrive",Name = "sdb2",Size = "931,4G"},
            new Drive(){FsType = "ntfs",MountPoint = "/media/Bob/Volume1",Name = "sdc1",Size = "447,1G"},
            new Drive(){FsType = "vfat",MountPoint = "/boot/efi",Name="nvme0n1p1",Size = "1022M"},
            new Drive(){FsType = "vfat",MountPoint = "/recovery",Name = "nvme0n1p2",Size = "4G"},
            new Drive(){FsType = "ext4",MountPoint = "/",Name = "data-root",Size = "456,7G"}
        };

        
        //Act
        var result = allMounted.StringManipulator().CreateDriveList();

        //Assert
        Assert.True(IsListIdentical(expectedList,result),
            "The lists are not identical.");
        
    }

    private bool IsListIdentical(List<Drive> listOne, List<Drive> listTwo)
    {
        return !listOne.Where((t, i) =>
            listOne.Count != listTwo.Count ||
            t.MountPoint != listTwo[i].MountPoint ||
            t.Name != listTwo[i].Name ||
            t.FsType != listTwo[i].FsType ||
            t.Size != listTwo[i].Size).Any();
    }

}