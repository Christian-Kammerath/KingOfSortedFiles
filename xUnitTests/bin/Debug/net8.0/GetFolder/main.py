import os
import sys

path = sys.argv[1]
search_string = sys.argv[2]


def Get_Folder(verzeichnis):

    for root, dirs, files in os.walk(verzeichnis):
        if search_string is not "null":
            for item in dirs:
                if search_string in item:
                    print(os.path.join(root,item))
        else:
            print(root)


if __name__ == "__main__":
    Get_Folder(path)