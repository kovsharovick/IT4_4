using System.Collections.ObjectModel;
using System.Linq;

namespace FileSystemLib.Models
{
    public class Folder : FileSystemElement, IReflectable
    {
        public ObservableCollection<FileSystemElement> Elements { get; set; } = new();

        public Folder() { }

        public Folder(string name, Folder? parent)
        {
            Name = name;
            ParentFolder = parent;
        }

        public override FileSystemElementType ElementType => FileSystemElementType.Folder;
        public override long Size => Elements.Sum(e => e.Size);

        public int GetCount() => Elements.Count;
    }
}