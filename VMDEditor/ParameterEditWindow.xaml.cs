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
        }
    }
}
