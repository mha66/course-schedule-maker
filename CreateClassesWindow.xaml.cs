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
    public partial class CreateClassesWindow : Window
    {
        public SessionType SessionKind { get; set; } = SessionType.Lec;
        public CreateClassesWindow()
        {
            InitializeComponent();
        }

        private void SessionsNum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (sender as ComboBox)!;
            int oldCount = SessionTextBoxes.Count, newCount = 0;
            newCount = int.Parse(comboBox.SelectedValue.ToString()!);

            //foreach (UIElement child in creationGrid.Children)
            //{
            //    if (child is SessionTextBoxes)
            //        oldCount++;
            //}
           // MessageBox.Show($"old: {oldCount} new: {newCount}");
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
                    for (int i = creationGrid.Children.Count - 1; i >= 0; i--)
                    {
                        //MessageBox.Show(creationGrid.Children.IndexOf(child).ToString());
                        if (creationGrid.Children[i] is SessionTextBoxes)
                        {
                            //TODO: fix a bug where remove doesn't work sometimes
                            creationGrid.Children.Remove(creationGrid.Children[i]);
                           // MessageBox.Show("HIT");
                            SessionTextBoxes.Count--;
                            oldCount--;
                            break;
                        }
                    }
                }
            }
        }

        private void SessionsNum_Loaded(object sender, RoutedEventArgs e)
        {
            ComboBox comboBox = (sender as ComboBox)!;
            int newCount = int.Parse(comboBox.SelectedValue.ToString()!);
            for (int i = 1; i <= newCount; i++)
            {
                {
                    var newSessionBoxes = new SessionTextBoxes();
                    creationGrid.Children.Add(newSessionBoxes);
                    Grid.SetRow(newSessionBoxes, (i < 3) ? 2 : 3);
                    Grid.SetColumn(newSessionBoxes, (i + 1) % 2);
                }
            }
            SessionTextBoxes.Count = newCount;
        }
    }
}
