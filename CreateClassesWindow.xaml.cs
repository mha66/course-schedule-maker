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
        public List<SessionTextBoxes> SessionBoxes { get; set; } = new List<SessionTextBoxes>();
        //public SessionType Kind { get; set; } = SessionType.Lec;
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
                    var newSessionBoxes = new SessionTextBoxes() {Uid = "session" + SessionTextBoxes.Count };
                    SessionTextBoxes.Count++;
                    oldCount++;
                    creationGrid.Children.Add(newSessionBoxes);
                    SessionBoxes.Add(newSessionBoxes);
                   
                    Grid.SetRow(newSessionBoxes, (oldCount < 3) ? 2 : 3);
                    Grid.SetColumn(newSessionBoxes, (oldCount + 1) % 2);
                }
            }
            else if (oldCount > newCount)
            {

                while (oldCount != newCount)
                {
                    int childrenCount = creationGrid.Children.Count;
                    //to avoid removing the Create button
                    creationGrid.Children.RemoveAt(
                        (creationGrid.Children[childrenCount - 1].Uid == "createBtn") ? childrenCount - 2 : childrenCount - 1);
                    SessionBoxes.RemoveAt(SessionBoxes.Count - 1);
                    SessionTextBoxes.Count--;
                    oldCount--;
                }

            }
        }


        private void CreateCourseBtn_Click(object sender, RoutedEventArgs e)
        {

            Course course = Course.ExistingOrNew(new Course(courseName.Text, courseCode.Text));
            Group group = Group.ExistingOrNew(new Group(courseGroup.Text, course));
            Class classes = new Class(course, group);
            foreach (SessionTextBoxes sessionBox in SessionBoxes)
            {
                classes.AddSession(new Session(sessionBox.SessionKind, sessionBox.Instructor, 
                    sessionBox.Day, sessionBox.Period));
            }
            DBSource.InsertData(classes);
            //MainWindow.UpdateCourseGroupViews();
            //foreach (Window window in Application.Current.Windows)
            //{
            //    //if (window.GetType() == typeof(MainWindow))
            //    //{
            //    //    var mainWindow = window as MainWindow;
            //    //    foreach (var oldClass in MainWindow.ClassesView!)
            //    //    {
            //    //        if (oldClass.Course.Id == course.Id)
            //    //        {
            //    //            //oldClass.Course.AddGroup(group);

            //    //            MainWindow.ClassesView.Remove(oldClass);
            //    //            MainWindow.ClassesView.Add(oldClass);
            //    //        }
            //    //    }
            //    //        mainWindow!.addedCourses.Items.Refresh();
                    
            //    //    break;
            //    //}
            //}
            MessageBox.Show("courses and groups are updated!");
        }
    }
}
