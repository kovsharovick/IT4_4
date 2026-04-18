using FileSystemLib.Models;
using FileSystemApp.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace FileSystemApp
{
    public partial class MainWindow : Window
    {
        public MainWindow() => InitializeComponent();

        private void TreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm)
                vm.SelectedElement = e.NewValue as FileSystemElement;
        }

        private void TreeView_TargetFolder_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (DataContext is MainViewModel vm && e.NewValue is Folder folder)
                vm.SelectedTargetFolder = folder;
        }
    }
}