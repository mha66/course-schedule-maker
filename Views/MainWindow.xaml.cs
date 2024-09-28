using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Collections.Specialized.BitVector32;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using CourseScheduleMaker.Models;
using CourseScheduleMaker.ViewModels;


namespace CourseScheduleMaker.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
        }

    }
}
