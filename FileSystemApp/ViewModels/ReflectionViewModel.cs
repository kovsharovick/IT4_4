using FileSystemLib.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace FileSystemApp.ViewModels
{
    public class ReflectionViewModel : BaseViewModel
    {
        private string _assemblyPath = string.Empty;
        public string AssemblyPath
        {
            get => _assemblyPath;
            set { _assemblyPath = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Type> Classes { get; set; } = new();
        public ObservableCollection<ConstructorInfo> Constructors { get; set; } = new();
        public ObservableCollection<MethodInfo> Methods { get; set; } = new();
        public ObservableCollection<MethodParameterViewModel> ConstructorParameters { get; set; } = new();
        public ObservableCollection<MethodParameterViewModel> MethodParameters { get; set; } = new();

        private Type? _selectedClass;
        public Type? SelectedClass
        {
            get => _selectedClass;
            set
            {
                _selectedClass = value;
                OnPropertyChanged();
                LoadConstructors();
                LoadMethods();
                MethodParameters.Clear();
            }
        }

        private ConstructorInfo? _selectedConstructor;
        public ConstructorInfo? SelectedConstructor
        {
            get => _selectedConstructor;
            set
            {
                _selectedConstructor = value;
                OnPropertyChanged();
                LoadConstructorParameters();
            }
        }

        private MethodInfo? _selectedMethod;
        public MethodInfo? SelectedMethod
        {
            get => _selectedMethod;
            set
            {
                _selectedMethod = value;
                OnPropertyChanged();
                LoadMethodParameters();
            }
        }

        public RelayCommand LoadAssemblyCommand { get; }
        public RelayCommand ExecuteCommand { get; }

        public ReflectionViewModel()
        {
            LoadAssemblyCommand = new RelayCommand(_ => LoadAssembly());
            ExecuteCommand = new RelayCommand(_ => Execute());
        }

        private void LoadAssembly()
        {
            try
            {
                var assembly = Assembly.LoadFrom(AssemblyPath);
                var types = assembly.GetTypes()
                    .Where(t => typeof(IReflectable).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                Classes.Clear();
                foreach (var t in types)
                    Classes.Add(t);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сборки: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadConstructors()
        {
            Constructors.Clear();
            ConstructorParameters.Clear();
            if (SelectedClass == null) return;

            var ctors = SelectedClass.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            foreach (var ctor in ctors)
                Constructors.Add(ctor);
        }

        private void LoadConstructorParameters()
        {
            ConstructorParameters.Clear();
            if (SelectedConstructor == null) return;

            foreach (var p in SelectedConstructor.GetParameters())
            {
                ConstructorParameters.Add(new MethodParameterViewModel
                {
                    Name = p.Name ?? "?",
                    TypeName = p.ParameterType.Name,
                    ParameterInfo = p
                });
            }
        }

        private void LoadMethods()
        {
            Methods.Clear();
            if (SelectedClass == null) return;

            foreach (var m in SelectedClass.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                Methods.Add(m);
        }

        private void LoadMethodParameters()
        {
            MethodParameters.Clear();
            if (SelectedMethod == null) return;

            foreach (var p in SelectedMethod.GetParameters())
            {
                MethodParameters.Add(new MethodParameterViewModel
                {
                    Name = p.Name ?? "?",
                    TypeName = p.ParameterType.Name,
                    ParameterInfo = p
                });
            }
        }

        private void Execute()
        {
            try
            {
                if (SelectedClass == null)
                {
                    MessageBox.Show("Выберите класс", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                object instance;
                if (SelectedConstructor != null)
                {
                    object[] ctorArgs = new object[ConstructorParameters.Count];
                    for (int i = 0; i < ConstructorParameters.Count; i++)
                    {
                        var param = ConstructorParameters[i];
                        ctorArgs[i] = Convert.ChangeType(param.Value, param.ParameterInfo!.ParameterType);
                    }
                    instance = SelectedConstructor.Invoke(ctorArgs);
                }
                else
                {
                    instance = Activator.CreateInstance(SelectedClass)!;
                }

                if (SelectedMethod != null)
                {
                    object[] methodArgs = new object[MethodParameters.Count];
                    for (int i = 0; i < MethodParameters.Count; i++)
                    {
                        var param = MethodParameters[i];
                        methodArgs[i] = Convert.ChangeType(param.Value, param.ParameterInfo!.ParameterType);
                    }
                    var result = SelectedMethod.Invoke(instance, methodArgs);
                    MessageBox.Show($"Результат: {result ?? "void"}", "Выполнение метода", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"Объект типа {SelectedClass.Name} создан, метод не выбран.", "Создание объекта", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}