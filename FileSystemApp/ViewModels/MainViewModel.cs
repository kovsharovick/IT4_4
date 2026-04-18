using FileSystemLib.Models;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace FileSystemApp.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        public ObservableCollection<FileSystemElement> RootElements { get; set; } = new();

        private FileSystemElement? _selectedElement;
        public FileSystemElement? SelectedElement
        {
            get => _selectedElement;
            set { _selectedElement = value; OnPropertyChanged(); }
        }

        private Folder? _selectedTargetFolder;
        public Folder? SelectedTargetFolder
        {
            get => _selectedTargetFolder;
            set { _selectedTargetFolder = value; OnPropertyChanged(); }
        }

        public RelayCommand MoveCommand { get; }
        public RelayCommand CopyCommand { get; }
        public RelayCommand OpenReflectionCommand { get; }

        public MainViewModel()
        {
            var root = new Folder("Root", null);
            var documents = new Folder("Documents", root);
            root.Elements.Add(documents);
            var file1 = new File("file1.txt", 1200, documents);
            documents.Elements.Add(file1);
            var images = new Folder("Images", root);
            root.Elements.Add(images);
            var image1 = new File("photo.png", 4500, images);
            images.Elements.Add(image1);

            RootElements.Add(root);

            FileSystemElement.OnMoved += (element, folder) =>
            {
                MessageBox.Show($"{element.Name} был перемещён в {folder.Path}", "Move", MessageBoxButton.OK, MessageBoxImage.Information);
            };
            FileSystemElement.OnCopied += (element, folder) =>
            {
                MessageBox.Show($"{element.Name} был скопирован в {folder.Path}", "Copy", MessageBoxButton.OK, MessageBoxImage.Information);
            };

            MoveCommand = new RelayCommand(
                o => { if (SelectedElement != null && SelectedTargetFolder != null) FileSystemElement.Move(SelectedElement, SelectedTargetFolder); },
                o => SelectedElement != null && SelectedTargetFolder != null);

            CopyCommand = new RelayCommand(
                o => { if (SelectedElement != null && SelectedTargetFolder != null) FileSystemElement.Copy(SelectedElement, SelectedTargetFolder); },
                o => SelectedElement != null && SelectedTargetFolder != null);

            OpenReflectionCommand = new RelayCommand(_ =>
            {
                var window = new ReflectionWindow();
                window.Owner = Application.Current.MainWindow;
                window.ShowDialog();
            });
        }
    }
}