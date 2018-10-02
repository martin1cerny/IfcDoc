using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace DocExporter
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Model _model;

        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            var lastFile = Properties.Settings.Default.LastFile;
            if (!string.IsNullOrWhiteSpace(lastFile) && File.Exists(lastFile))
            {
                SetUpModel(lastFile);
            }
        }



        public IEnumerable<EntityNode> EntitiesTree
        {
            get { return (IEnumerable<EntityNode>)GetValue(EntitiesProperty); }
            set { SetValue(EntitiesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Entities.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntitiesProperty =
            DependencyProperty.Register("EntitiesTree", typeof(IEnumerable<EntityNode>), typeof(MainWindow), new PropertyMetadata(null));




        public IEnumerable<EntityNode> EntitiesList
        {
            get { return (IEnumerable<EntityNode>)GetValue(EntitiesListProperty); }
            set { SetValue(EntitiesListProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EntitiesList.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EntitiesListProperty =
            DependencyProperty.Register("EntitiesList", typeof(IEnumerable<EntityNode>), typeof(MainWindow), new PropertyMetadata(null));



        private void SetUpModel(string file)
        {
            _model = new Model();
            _model.Open(file);
            EntitiesTree = EntityNode.GetRoots(_model);
            EntitiesList = EntityNode.GetFlat(EntitiesTree).OrderBy(e => e.Name).ToList();
        }


        private void OpenFileClick(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "IFCDOC|*.ifcdoc",
                CheckFileExists = true
            };
            if (dlg.ShowDialog() == true)
            {
                SetUpModel(dlg.FileName);
                Properties.Settings.Default.LastFile = dlg.FileName;
                Properties.Settings.Default.Save();
            }
        }

        private void txtFilter_KeyUp(object sender, System.Windows.Input.KeyEventArgs arg)
        {
            var filter = txtFilter.Text?.ToUpperInvariant();
            if (string.IsNullOrWhiteSpace(filter))
            {
                EntitiesList = EntityNode.GetFlat(EntitiesTree).OrderBy(e => e.Name).ToList();
                return;
            }

            EntitiesList = EntityNode.GetFlat(EntitiesTree)
                .Where(e => e.Name.ToUpperInvariant().Contains(filter))
                .OrderBy(e => e.Name).ToList();

        }
    }
}
