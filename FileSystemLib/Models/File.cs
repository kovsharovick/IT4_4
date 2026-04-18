namespace FileSystemLib.Models
{
    public class File : FileSystemElement, IReflectable
    {
        public long FileSize { get; set; }

        public File() { }

        public File(string name, long size, Folder? parent)
        {
            Name = name;
            FileSize = size;
            ParentFolder = parent;
        }

        public override FileSystemElementType ElementType => FileSystemElementType.File;
        public override long Size => FileSize;

        public string GetInfo() => $"{Name} ({Size} bytes)";
    }
}