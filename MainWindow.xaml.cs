using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.CodeDom;
using System.ComponentModel;
using System.Diagnostics;
using System.DirectoryServices;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.Swift;
using System.Windows;
using zalupa.DataModel;
namespace zalupa
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Type? currentClass;
        private PropertyInfo? currentSortProperty;
        private bool currentSortDirectionBool = true;              // true = DESC
        public MainWindow()
        {
            InitializeComponent();
            LoadAllModelTypes();
            currentClass = typeof(Film);
            currentSortDirectionBool = true;
            LoadInitialData();
        }
        public async void LoadInitialData()
        {
            using (AbsoluteCinemaContext context = new AbsoluteCinemaContext())
            {
                dgview.ItemsSource = await context.GetListByTypeAsync(currentClass);
            }
        }
        //Получаем все классы с DataModel
        public List<Type> GetClassesInNamespace(Assembly assembly, string nameSpace)
        {
            return assembly.GetTypes()
                .Where(t => t.IsClass && t.IsPublic && !t.IsAbstract && t.Namespace == nameSpace && !typeof(DbContext).IsAssignableFrom(t)) 
                .ToList();
        }
        //Загружаем все классы в ComboBox
        public void LoadAllModelTypes()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string nameSpace = "zalupa.DataModel";
            TablesComboBox.ItemsSource = GetClassesInNamespace(assembly, nameSpace);
        }
        public List<PropertyInfo> LoadAllProps(Type type)
        {
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance;
            return type.GetProperties(flags)
                .Where(t => IsSimpleType(t.PropertyType))
                .ToList();
        }
        //Проверяем тип (должен быть простым типом а не навигационным свойством)
        private bool IsSimpleType(Type type)
        {
            // Распаковываем Nullable-типы (например, int? превращаем в int)
            Type actualType = Nullable.GetUnderlyingType(type) ?? type;

            return actualType.IsPrimitive               // int, double, bool и т.д.
                || actualType.IsEnum                    // перечисления (enum)
                || actualType == typeof(string)         // строки
                || actualType == typeof(decimal)        // деньги/точные числа
                || actualType == typeof(DateTime)       // дата и время
                || actualType == typeof(DateOnly)       // только дата (.NET 6+)
                || actualType == typeof(TimeOnly)       // только время (.NET 6+)
                || actualType == typeof(Guid);          // уникальные идентификаторы
        }
        //GENERIC
        public List<T> LoadAll<T>(AbsoluteCinemaContext context) where T : class
        {
            return context.Set<T>().ToList();
        }
        //Получаем все свойства класса
        public List<T> LoadAllSorted<T>(AbsoluteCinemaContext context, string propertyName, string sortDirection) where T : class
        {
            PropertyInfo? property = typeof(T).GetProperty(propertyName)
                ?? throw new ArgumentException($"Нет свойства {propertyName} у {typeof(T).Name}");

            return context.Set<T>()
                .OrderBy($"{property.Name} {sortDirection}")
                .ToList();
        }
        private void Sort(object sender, RoutedEventArgs e)
        {
            using (AbsoluteCinemaContext context = new AbsoluteCinemaContext())
            {
                if (currentClass != null)
                {
                    if (currentSortProperty != null)
                    {
                        string sortDirection = currentSortDirectionBool ? "DESC" : "ASC";
                        dgview.ItemsSource = LoadAllSorted<Film>(context, currentSortProperty.Name, sortDirection);
                    }
                    else
                    {
                        dgview.ItemsSource = LoadAll<Film>(context);
                    }
                }
                else
                {
                    dgview.ItemsSource = LoadAll<Film>(context);
                }
            }
        }

        //Изменение выбранного свойства
        private void SortBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            PropertyInfo? property = SortBox.SelectedItem as PropertyInfo;
            if (property != null)
            {
                if (property.DeclaringType == currentClass)
                {
                    //ОК
                    currentSortProperty = SortBox.SelectedItem as PropertyInfo;
                }
                else
                {
                    //Не совпадает класс
                    throw new Exception("Property type =/= current type.");
                }
            }
            else
            {
                //Не выбран тип
                currentSortProperty = null;
            }
        }
        public void UpdateSortComboBox()
        {
            if(currentClass != null)
            {
                SortBox.ItemsSource = LoadAllProps(currentClass);
            }
            else
            {
                //Ничего не выбрано просто
            }
        }
        //Изменение выбранного класса/таблицы
        private void TablesComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            currentClass = TablesComboBox.SelectedItem as Type;
            UpdateSortComboBox();
        }

        private void ascRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            currentSortDirectionBool = false;
        }

        private void descRadioButton_Checked(object sender, RoutedEventArgs e)
        {
            currentSortDirectionBool = true;
        }

    }
}