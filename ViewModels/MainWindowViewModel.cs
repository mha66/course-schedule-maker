using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseScheduleMaker.Models;
using CourseScheduleMaker.Views;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows;
using System.Configuration;

namespace CourseScheduleMaker.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public const int ROWS = 8, COLUMNS = 18;
        public ObservableCollection<Class>? _classesView = new ObservableCollection<Class>();
        //TODO: remove static
        public static ObservableCollection<Course>? CoursesView { get; set; }
        public Course? SelectedCourse { get; set; }
        public static ObservableCollection<Group>? GroupsView { get; set; }
        public Group? SelectedGroup { get; set; }

       // TextBlock[,] courseBlocks = new TextBlock[7, 17];
        TextBlock[,] ScheduleGrid { get; set; } = new TextBlock[ROWS, COLUMNS];
        public ObservableCollection<TextBlock> ScheduleChildren { get; set; } = new ObservableCollection<TextBlock>();
        //ViewModel
        public ObservableCollection<Class>? ClassesView 
        {
            get => _classesView;
            set => SetProperty(ref _classesView, value); 
        } 
        //DBSource
        public static ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public static ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        //public static ObservableCollection<Class> Classes { get; set; } = new ObservableCollection<Class>();


        private static void AddCourseView(Course course)
        {
            foreach (var courseView in CoursesView!)
            {
                if (courseView == course)
                    return;
            }
            CoursesView!.Add(course);
        }
        private void Courses_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //foreach (var course in Courses)
                //{
                //    if (course.Code == (e.NewItems[0] as Course).Code && )
                //}
                AddCourseView(Courses[^1]);
            }

        }

        private void Groups_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
                GroupsView!.Add(Groups[^1]);
        }
        public static void UpdateCourseGroupViews()
        {

            GroupsView!.Add(Groups[^1]);
            AddCourseView(Courses[^1]);

        }
        public MainWindowViewModel()
        {

            
            DBSource.Initialize();
            DBSource.ReadData();

            // TODO: find a better place for the following two lines
            GroupsView = new ObservableCollection<Group>(Groups);
            CoursesView = new ObservableCollection<Course>(Courses);


            ClassesView!.CollectionChanged += ClassesView_CollectionChanged!;
            Courses.CollectionChanged += Courses_CollectionChanged!;
            Groups.CollectionChanged += Groups_CollectionChanged!;

            SetupScheduleGrid();
            
        }

        private void AddToSchedule(TextBlock textBlock, int row, int column)
        {
            ScheduleChildren.Insert(row * COLUMNS + column, textBlock);
            ScheduleGrid[row, column] = textBlock;
        }

        //TODO: use uniform grid
        private void SetupScheduleGrid()
        {
            //ScheduleChildren.Add(new TextBlock() { Text = "Days/Period" });
            //adds row and column definitions
            for (int i = 0; i < ROWS; i++)
            {
                for(int j = 0; j < COLUMNS; j++)
                {

                    var textBlock = new TextBlock()
                    {
                        Margin = new Thickness(5, 10, 5, 0),

                        Text = (i == 0 && j == 0) ? "Days/Period" :
                        (i == 0) ? j.ToString() :
                        (j == 0) ? $"{(DayOfWeek)((i + 5) % 7)}" : ""
                    }; 

                    AddToSchedule(textBlock, i, j);
                }
            }

            //UpdateScheduleGrid();
        }

        //updates the grids with the objects in courseBlocks
        /*
        private void UpdateScheduleGrid()
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 17; j++)
                {
                    TextBlock tb = courseBlocks[i, j];
                    Grid.SetRow(tb, i + 1);
                    Grid.SetColumn(tb, j + 1);
                    if (!scheduleGrid.Children.Contains(tb)) //???
                        scheduleGrid.Children.Add(tb);
                }
        }*/

        private void RemoveSession(TextBlock sessionTextBlock, string? sessionToRemove)
        {
            var inlines = sessionTextBlock.Inlines;
            //a clash occurs if there's more than one Inline in the TextBlock
            if (inlines.Count > 1)
            {
                //index of current inline 
                int i = 0;
                foreach (var inline in inlines)
                {
                    var inlineText = new TextRange(inline.ContentStart, inline.ContentEnd);
                    if (inlineText.Text == sessionToRemove!.ToString())
                    {
                        //Remove Xs (RemoveAt() is missing for some reason) 
                        inlines.Remove(inlines.ElementAt((i == 0) ? 1 : i - 1));
                        //Remove session info
                        inlines.Remove(inline);
                        break;
                    }
                    i++;
                }
            }
            else
                inlines.Clear();
        }



        //gets called when ClassesView changes
        private void ClassesView_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //list[^1] is equivalent to list[list.Count - 1] (last element)
                Class? classes = (e.NewItems!.Count != 0) ? e.NewItems![^1] as Class : null;

                foreach (var session in classes!.Sessions)
                {
                    int day = (int)session.Day;
                    int i = (day == 5) ? 7 : (day - 5 + 7) % 7;
                    int j = session.Period + 1;

                    var inlines = ScheduleGrid[i, j].Inlines;
                    //even-indexed inlines contain session info, odd-indexed ones contain Xs
                    if (inlines.Count != 0)
                    {
                        inlines.Add("\nXXXXXXXXXX\n");
                    }
                    inlines.Add(session.ToString());
                }

            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems != null)
            {
                foreach (Class oldClasses in e.OldItems)
                {
                    //if left-hand operand of || evaluates to "true" then right-hand operand isn't evaluated
                    if (e.NewItems == null || !e.NewItems.Contains(oldClasses))
                    {
                        foreach (var session in oldClasses.Sessions)
                        {
                            int day = (int)session.Day;
                            int i = (day == 5) ? 7 : (day - 5 + 7) % 7;
                            int j = session.Period + 1;
                            RemoveSession(ScheduleGrid[i, j], session.ToString());
                        }
                        return;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var textBlock in ScheduleGrid)
                {
                    textBlock.Inlines.Clear();
                }
            }
        }


        private void coursesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            GroupsView!.Clear();
            foreach (Group g in SelectedCourse!.Groups)
                GroupsView.Add(g);
        }

        private void groupsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // TODO: do something?
        }

        private void AddCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Course? course = SelectedCourse;
                Group? group = SelectedGroup;
                if (course != null && group != null)
                {
                    foreach (Class viewedClasses in ClassesView!)
                    {

                        if (viewedClasses.Course == course && viewedClasses.Group != group)
                        {

                            ClassesView.Remove(viewedClasses);
                            //addedCourses.Items.Refresh();
                            break;
                        }
                        else if (viewedClasses.Course == course && viewedClasses.Group == group)
                            return;
                    }
                    foreach (Class classes in group.Classes)
                    {

                        if (classes.Course == course)
                        {
                            ClassesView.Add(classes);
                            //addedCourses.Items.Refresh();
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }


        private void CourseGroups_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int pos = ClassesView!.Count - 1;
            Class? newClasses = (sender as ComboBox)!.Tag as Class;
            var oldClass = ClassesView.First(classes => classes.Course == newClasses!.Course);
            pos = ClassesView.IndexOf(oldClass);
            ClassesView.Remove(oldClass);
            //addedCourses.Items.Refresh();
            ClassesView.Insert(pos, newClasses!.Group!.Classes.First(classes => classes.Course == newClasses!.Course));

        }
        private void RemoveCourseBtn_Click(object sender, RoutedEventArgs e)
        {

            Button button = (sender as Button)!;
            foreach (var classes in ClassesView!)
            {
                if (classes.Course!.Code == button.Tag.ToString())
                {
                    ClassesView.Remove(classes);
                    break;
                }
            }
        }


        //TODO: remove modify button
        private void ModifyCourseBtn_Click(object sender, RoutedEventArgs e)
        {

            Class? newClasses = (sender as Button)!.Tag as Class;
            foreach (Class classes in ClassesView!)
            {
                if (classes.Course == newClasses!.Course)
                {
                    ClassesView.Remove(classes);

                    //addedCourses.Items.Refresh();


                    break;
                }
            }


            foreach (Class classes in newClasses!.Group!.Classes)
            {
                if (classes.Course!.Code == newClasses.Course!.Code)
                {
                    ClassesView.Add(classes);
                    //addedCourses.Items.Refresh();

                    break;
                }
            }
        }

        private void CreateCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            var createClassesWindow = new ClassCreationView();
            createClassesWindow.Show();
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            DBSource.CloseConnection();
        }

        //TODO: binding causes an infinite loop and a crash after changing combo box selection several times
        private void CourseGroups_Loaded(object sender, RoutedEventArgs e)
        {
            var hasBinding = (sender as ComboBox)!.GetBindingExpression(ComboBox.SelectedItemProperty);
            if (hasBinding == null)
            {
                Binding binding = new Binding("Group");
                //var newClass = (sender as ComboBox)!.Tag as Class;
                //var oldClass = ClassesView!.First(classes => classes.Course == newClass!.Course);
                binding.Source = (sender as ComboBox)!.Tag as Class;
                (sender as ComboBox)!.SetBinding(ComboBox.SelectedItemProperty, binding);
            }
            //Task.WaitAll(new Task[] { Task.Delay(1000) });

        }

    }
}
