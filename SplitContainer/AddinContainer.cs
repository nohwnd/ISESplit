using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using Microsoft.PowerShell.Host.ISE;

namespace SplitPanel
{
    /// <summary>
    ///     Interaction logic for AddinContainer.xaml
    /// </summary>
    public partial class AddinContainer : IAddOnToolHostObject
    {
        public AddinContainer()
        {
            InitializeComponent();
            IseAddins = new ObservableCollection<Type>();
            Loaded += (s, e) =>
            {
                LoadIseAddins();
                DataContext = IseAddins;
            };
        }

        public ObservableCollection<Type> IseAddins { get; private set; }
        public ObjectModelRoot HostObject { get; set; }

        private void LoadIseAddins()
        {
            var thisAssembly = Assembly.GetExecutingAssembly();
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.Equals(thisAssembly));

            foreach (var type in loadedAssemblies.SelectMany(assembly => assembly.GetTypes()
                .Where(t => !t.IsInterface && t.GetInterfaces().Contains(typeof (IAddOnToolHostObject)))))
            {
                IseAddins.Add(type);
            }
        }

        private void PanelSelector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var type = (Type) e.AddedItems[0];

            var instance = (UserControl) Activator.CreateInstance(type);
            if (HostObject != null) ((IAddOnToolHostObject) instance).HostObject = HostObject;

            var senderTag = ((ComboBox) sender).Tag.ToString();
            var grid = (Grid) FindName(senderTag);

            if (grid != null)
            {
                grid.Children.Clear();
                grid.Children.Add(instance);
            }
        }

        private void RefreshButton_OnClick(object sender, RoutedEventArgs e)
        {
            LoadIseAddins();
        }
    }
}