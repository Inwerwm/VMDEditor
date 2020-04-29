using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace VMDEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>


    public partial class ParameterEditWindow : Window
    {
        ViewModel vm = new ViewModel();

        private TimeLineWindow timeLineWindow;

        public ParameterEditWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => { timeLineWindow.Owner = this; };

            DataContext = vm;

            timeLineWindow = new TimeLineWindow(vm);

            vm.IsTimeLineWindowVisible.Subscribe(v => timeLineWindow.Visibility = v ? Visibility.Visible : Visibility.Hidden);

            vm.setWindow(this, timeLineWindow);
        }

        private void WindowParameterEdit_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
            e.Handled = e.Data.GetDataPresent(DataFormats.FileDrop);
        }

        private void WindowParameterEdit_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            if(files.Count() > 1)
            {
                MessageBox.Show("投入するファイルは1つだけにしてください。");
                return;
            }

            vm.LoadVMD(files[0]);
        }

        private void ButtonDeleteArticle_Click(object sender, RoutedEventArgs e)
        {
            vm.Articles.Remove(vm.SelectedArticle.Value);
            timeLineWindow.DeleteArticleLine();
        }
    }
}
