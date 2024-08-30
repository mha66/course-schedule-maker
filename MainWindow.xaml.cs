using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseScheduleMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        TextBlock[,] courseBlocks = new TextBlock[7, 17];
        public static ObservableCollection<Course>? CoursesView { get; set; }
        public static ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public static ObservableCollection<Group>? GroupsView { get; set; }
        public static ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public MainWindow()
        {
            new Course(1, "Math", "BA232");
            Courses[0].AddGroup(new Group(1, "08CC05"));
            new Course(2, "Physics", "CC456");
            Courses[1].AddGroup(new Group(2, "04EE02"));

            //*****find a better place for the following two lines
            GroupsView = new ObservableCollection<Group>(Groups);
            CoursesView = new ObservableCollection<Course>(Courses);

            InitializeComponent();
            SetupScheduleGrid();

        }
       
        private void SetupScheduleGrid()
        {
            //adds row and column definitions
            for (int i = 0; i < 18; i++)
            {
                scheduleGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                if(i<8)
                    scheduleGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            
            //adds titles for each row and column
            scheduleGrid.Children.Add(new TextBlock() { Text = "Days/Period" });
            for (int i = 0; i <= 16; i++)
            {
                //column titles (periods 0-16)
                TextBlock periodTB = new TextBlock() { Text = i.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                Grid.SetColumn(periodTB, i + 1);
                scheduleGrid.Children.Add(periodTB);
                //row titles (days of the week)
                if (i < 7)
                {
                    TextBlock dayTB = new TextBlock() { Text = $"{(DayOfWeek)((i + 5) % 7)}", Margin = new Thickness(0, 10, 0, 0) };
                    Grid.SetRow(dayTB, i + 1);
                    scheduleGrid.Children.Add(dayTB);
                }
            }
            
            //adds empty textblocks(????) for courses to be added
            for(int i=0; i<7; i++)
            {
                for(int j=0; j<17; j++)
                {
                    courseBlocks[i, j] = new TextBlock();
                }
            }
            UpdateScheduleGrid();
        }

        //updates the grids with the objects in courseBlocks
        private void UpdateScheduleGrid()
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 17; j++)
                {
                    TextBlock tb = courseBlocks[i, j];
                    Grid.SetRow(tb, i + 1);
                    Grid.SetColumn(tb, j + 1);
                    if(!scheduleGrid.Children.Contains(tb)) //???
                        scheduleGrid.Children.Add(tb);
                }
        }
        private void AddCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            /*
            //Test
            int i = 2, j = 5;
            if (String.IsNullOrEmpty(courseBlocks[i, j].Text ))
            {
                Course course = new Course(1, "Math", "BA222");
                Group group = new Group(1, "07CC01", course);
                CourseClasses classes = new CourseClasses(1, course, group);
                Session Lecture = new Session(1, classes, SessionType.Lec, "Mark Smith", DayOfWeek.Sunday, 3);
                Session Section = new Session(2, classes, SessionType.Sec, "John Doe", DayOfWeek.Tuesday, 1);

                courseBlocks[i, j].Text = Lecture.ToString();
            }
            */
        
        }

        private void coursesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Course c = (Course) coursesComboBox.SelectedItem;
            GroupsView!.Clear();
            foreach(Group g in c.Groups)
                GroupsView.Add(g);
        }

        private void groupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}