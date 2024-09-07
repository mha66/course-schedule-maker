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
using System.Windows.Shapes;

namespace CourseScheduleMaker
{
    /// <summary>
    /// Interaction logic for CreateClassesWindow.xaml
    /// </summary>
    /// 

    //TODO: add feat to fill all textboxes with last made course info
    public partial class CreateClassesWindow : Window
    {
        //public SessionType SessionKind { get; set; } = SessionType.Lec;
        public CreateClassesWindow()
        {
            InitializeComponent();
        }

        private void SessionsNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (sender as ComboBox)!;
            int oldCount = SessionTextBoxes.Count, newCount = 0;
            newCount = int.Parse(comboBox.SelectedValue.ToString()!);

            if (oldCount < newCount)
            {
                while (oldCount != newCount)
                {
                    var newSessionBoxes = new SessionTextBoxes();
                    SessionTextBoxes.Count++;
                    oldCount++;
                    creationGrid.Children.Add(newSessionBoxes);
                   
                    Grid.SetRow(newSessionBoxes, (oldCount < 3) ? 2 : 3);
                    Grid.SetColumn(newSessionBoxes, (oldCount + 1) % 2);
                }
            }
            else if (oldCount > newCount)
            {

                while (oldCount != newCount)
                {
                    creationGrid.Children.RemoveAt(creationGrid.Children.Count - 1 );
                    SessionTextBoxes.Count--;
                    oldCount--;
                }

            }
        }
    }
}
