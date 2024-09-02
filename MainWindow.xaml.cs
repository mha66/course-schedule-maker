using System;
using System.Linq;
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
using static System.Collections.Specialized.BitVector32;
using System.Collections;
using static System.Net.Mime.MediaTypeNames;

namespace CourseScheduleMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        
        TextBlock[,] courseBlocks = new TextBlock[7, 17];
        //ViewModel
        public static ObservableCollection<Course>? CoursesView { get; set; }
        public static ObservableCollection<Group>? GroupsView { get; set; }
        public static ObservableCollection<GroupClasses>? ClassesView { get; set; } = new ObservableCollection<GroupClasses>();
        //DBSource
        public static ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public static ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public static ObservableCollection<GroupClasses> Classes { get; set; } = new ObservableCollection<GroupClasses>();
        public MainWindow()
        {
            new Course(1, "Math", "BA232");
            Courses[0].AddGroup(new Group(1, "08CC05"));
            Courses[0].Groups[0].AddClasses(new GroupClasses(1, Courses[0]));
            Courses[0].Groups[0].Classes[0].AddSessions(new List<Session>(){ new Session(), new Session(1,SessionType.Sec,"John Smith",DayOfWeek.Tuesday, 4)});
            Courses[0].AddGroup(new Group(2, "06ME03"));
            Courses[0].Groups[1].AddClasses(new GroupClasses(2, Courses[0]));
            Courses[0].Groups[1].Classes[0].AddSessions(new List<Session>()
            {
                new Session(1, SessionType.Lec,"Jim",DayOfWeek.Wednesday,10),
                new Session(2, SessionType.Sec, "John Smith", DayOfWeek.Tuesday, 6),
                new Session(3, SessionType.Lab, "Mark", DayOfWeek.Monday, 4)
            });

            new Course(2, "Physics", "CC456");
            Courses[1].AddGroup(new Group(2, "04EE02"));
            Courses[1].Groups[0].AddClasses(new GroupClasses(2, Courses[1]));
            Courses[1].Groups[0].Classes[0].AddSessions(new List<Session>() 
            {
                new Session(1, SessionType.Lec, "Mark Roberts", DayOfWeek.Thursday, 2),
                new Session(2, SessionType.Sec, "Jack Johnson", DayOfWeek.Tuesday, 6),
            });

            //*****find a better place for the following two lines
            GroupsView = new ObservableCollection<Group>(Groups);
            CoursesView = new ObservableCollection<Course>(Courses);


            ClassesView!.CollectionChanged += ClassesView_CollectionChanged!;


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

        private void RemoveSession(TextBlock sessionTextBlock, string? sessionToRemove)
        {
            var Inlines = sessionTextBlock.Inlines;
            //a clash occurs if there's more than one Inline in the TextBlock
            if (Inlines.Count > 1)
            {
                //index of current inline 
                int i = 0;
                foreach (var inline in Inlines)
                {
                    var inlineText = new TextRange(inline.ContentStart, inline.ContentEnd);
                    if (inlineText.Text == sessionToRemove!.ToString())
                    {
                        //Remove Xs (RemoveAt() is missing for some reason) 
                        Inlines.Remove(Inlines.ElementAt((i == 0) ? 1 : i - 1));
                        //Remove session info
                        Inlines.Remove(inline);
                        break;
                    }
                    i++;
                }
            }
            else
                Inlines.Clear();
        }

        private void AddCourseRow(GroupClasses addedCourseClasses)
        {
            addedCourses.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto});
            for (int i = 0; i < addedCourses.ColumnDefinitions.Count; i++)
            {
                var textBlock = new TextBlock();
                textBlock.Text = addedCourseClasses[i];
                addedCourses.Children.Add(textBlock);   
                Grid.SetRow(textBlock, addedCourses.RowDefinitions.Count - 1);
                Grid.SetColumn(textBlock, i);
            }
        }
        //gets called when ClassesView changes
        private void ClassesView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add) 
            {
                //list[^1] is equivalent to list[list.Count - 1] (last element)
                GroupClasses? classes = (e.NewItems!.Count != 0) ? e.NewItems![^1] as GroupClasses : null;
                
                foreach (var session in classes!.Sessions)
                {
                    int i = ((int)session.Day - 5 + 7) % 7, j = session.Period;
                    //even-indexed inlines contain session info, odd-indexed ones contain Xs
                    if (courseBlocks[i, j].Inlines.Count != 0)
                    {
                        courseBlocks[i, j].Inlines.Add("\nXXXXXXXXXX\n");
                    }
                    courseBlocks[i, j].Inlines.Add(session.ToString());
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (GroupClasses oldClasses in e.OldItems)
                {
                    //if left-hand operand of || evaluates to "true" then right-hand operand isn't evaluated
                    if(e.NewItems == null || !e.NewItems.Contains(oldClasses)) 
                    { 
                        foreach (var session in oldClasses.Sessions)
                        {
                           int i = ((int)session.Day - 5 + 7) % 7, j = session.Period;
                           RemoveSession(courseBlocks[i, j], session.ToString());
                        }
                        return;
                    }
                }
            }
        }
        private void AddCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Course? course = coursesComboBox.SelectedItem as Course;
                Group? group = groupsComboBox.SelectedItem as Group;
                if (course != null && group != null)
                {
                    foreach (GroupClasses viewedClasses in ClassesView!)
                    {
                        if (viewedClasses.Course == course && viewedClasses.Group != group)
                        {
                            ClassesView.Remove(viewedClasses);
                            break;
                        }
                        else if (viewedClasses.Course == course && viewedClasses.Group == group)
                            return;
                    }
                    foreach (GroupClasses classes in group.Classes)
                    {

                        if (classes.Course == course)
                        {
                            ClassesView.Add(classes);
                            AddCourseRow(classes);
                            break;
                        }
                    }

                }
            } catch (Exception ex) {Console.WriteLine(ex.ToString()); }
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
            //do something?
        }
    }
}