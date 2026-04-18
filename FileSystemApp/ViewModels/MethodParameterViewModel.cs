using System.Reflection;

namespace FileSystemApp.ViewModels
{
    public class MethodParameterViewModel : BaseViewModel
    {
        public string Name { get; set; }
        public string TypeName { get; set; }

        private string _value;
        public string Value
        {
            get => _value;
            set { _value = value; OnPropertyChanged(); }
        }

        public ParameterInfo ParameterInfo { get; set; }
    }
}