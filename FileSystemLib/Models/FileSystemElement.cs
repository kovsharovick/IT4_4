using System;

namespace FileSystemLib.Models
{
    public abstract class FileSystemElement
    {
        public string Name { get; set; } = string.Empty;
        public Folder? ParentFolder { get; set; }

        public static event Action<FileSystemElement, Folder>? OnMoved;
        public static event Action<FileSystemElement, Folder>? OnCopied;

        public string Path => ParentFolder == null ? Name : System.IO.Path.Combine(ParentFolder.Path, Name);

        public abstract FileSystemElementType ElementType { get; }
        public abstract long Size { get; }

        public static void Move(FileSystemElement element, Folder newParent)
        {
            element.ParentFolder?.Elements.Remove(element);
            newParent.Elements.Add(element);
            element.ParentFolder = newParent;
            OnMoved?.Invoke(element, newParent);
        }

        public static void Copy(FileSystemElement element, Folder newParent)
        {
            if (element is File file)
            {
                var copy = new File(file.Name, file.FileSize, newParent);
                newParent.Elements.Add(copy);
                OnCopied?.Invoke(copy, newParent);
            }
            else if (element is Folder folder)
            {
                var copy = new Folder(folder.Name, newParent);
                newParent.Elements.Add(copy);
                OnCopied?.Invoke(copy, newParent);
                foreach (var child in folder.Elements)
                    Copy(child, copy);
            }
        }
    }
}