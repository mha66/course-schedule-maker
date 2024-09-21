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
using CourseScheduleMaker.Models;

namespace CourseScheduleMaker.Views
{
    /// <summary>
    /// Interaction logic for SessionTextBoxes.xaml
    /// </summary>
    public partial class SessionTextBoxes : UserControl
    {
        public static int Count { get; set; } = 0;

        public SessionType SessionKind { get => (SessionType)sessionClassification.SelectedItem; }
        public string Instructor { get => sessionInstructor.Text; }
        public DayOfWeek Day { get => (DayOfWeek)sessionDay.SelectedItem; }
        public int Period { get => (int)sessionPeriod.SelectedItem; }
        public SessionTextBoxes()
        {
            InitializeComponent();
        }

   
    }
}
