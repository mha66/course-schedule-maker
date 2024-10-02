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
using System.Windows.Input;
using System.Windows.Media;

namespace CourseScheduleMaker.ViewModels
{
    public class MainWindowViewModel : ObservableObject
    {
        public const int ROWS = 8, COLUMNS = 9;

        public ObservableCollection<Class>? _classesView = new ObservableCollection<Class>();
        //TODO: remove static
        public static ObservableCollection<Course>? CoursesView { get; set; }
        public Course? SelectedCourse { get; set; }
        public static ObservableCollection<Group>? GroupsView { get; set; }
        public Group? SelectedGroup { get; set; }

        private TextBlock[,] ScheduleGrid { get; set; } = new TextBlock[ROWS, COLUMNS];

        public ObservableCollection<Class>? ClassesView
        {
            get => _classesView;
            set => SetProperty(ref _classesView, value);
        }
        public static ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public static ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public Grid? ScheduleGridUI { get; set; }

        #region Commands
        public ICommand CoursesComboBox_SelectionChangedCmd { get; private set; }
        public ICommand AddCourseBtn_ClickCmd { get; private set; }
        public ICommand CourseGroups_SelectionChangedCmd { get; private set; }
        public ICommand RemoveCourseBtn_ClickCmd { get; private set; }
        public ICommand ModifyCourseBtn_ClickCmd { get; private set; }
        public ICommand CreateCourseBtn_ClickCmd { get; private set; }
        public ICommand MainWindow_ClosedCmd { get; private set; }
        #endregion

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

            CoursesComboBox_SelectionChangedCmd = new RelayCommand(coursesComboBox_SelectionChanged);
            AddCourseBtn_ClickCmd = new RelayCommand(AddCourseBtn_Click);
            CourseGroups_SelectionChangedCmd = new RelayCommand<object>(CourseGroups_SelectionChanged);
            RemoveCourseBtn_ClickCmd = new RelayCommand<string>(RemoveCourseBtn_Click);
            ModifyCourseBtn_ClickCmd = new RelayCommand<Class>(ModifyCourseBtn_Click);
            CreateCourseBtn_ClickCmd = new RelayCommand(CreateCourseBtn_Click);
            MainWindow_ClosedCmd = new RelayCommand(MainWindow_Closed);

        }

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


        private void AddToSchedule(TextBlock textBlock, Border border, int row, int column)
        {
            ScheduleGridUI!.Children.Add(border);
            Grid.SetRow(border, row);
            Grid.SetColumn(border, column);
            ScheduleGrid[row, column] = textBlock;
        }


        private void SetupScheduleGrid()
        {
            ScheduleGridUI = new Grid() {HorizontalAlignment = HorizontalAlignment.Center };
            //adds row and column definitions
            for (int i = 0; i < COLUMNS; i++)
            {
                ScheduleGridUI.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                if (i < ROWS)
                    ScheduleGridUI.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            //adds TextBlocks to the entire grid 
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {
                    var textBlock = new TextBlock()
                    {
                        Margin = new Thickness(5),

                        Text = (i == 0 && j == 0) ? "Days/Period" :
                        (i == 0) ? $"{j * 2 - 1}--{j * 2}" :
                        (j == 0) ? $"{(DayOfWeek)((i + 5) % 7)}" : "",

                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    var border = new Border()
                    {
                        Child = textBlock,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(0.5)
                    };
                    AddToSchedule(textBlock, border, i, j);
                }
            }
        }

       

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
                    int j = (session.Period + 1)/2;

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
                            int j = (session.Period + 1) / 2;
                            RemoveSession(ScheduleGrid[i, j], session.ToString());
                        }
                        return;
                    }
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                for (int i = 1; i < ROWS; i++)
                {
                    for(int j = 1; j < COLUMNS; j++)
                    {
                        ScheduleGrid[i,j].Inlines.Clear();
                    }
                }
            }
        }


        private void coursesComboBox_SelectionChanged()
        {

            GroupsView!.Clear();
            foreach (Group g in SelectedCourse!.Groups)
                GroupsView.Add(g);
        }

        private void groupsComboBox_SelectionChanged()
        {
            // TODO: do something?
        }

        private void AddCourseBtn_Click()
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
                            RefreshAddedCourses();
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
                            break;
                        }
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex.ToString()); }
        }


        private void RefreshAddedCourses()
        {
            var temp = new ObservableCollection<Class?>();
            foreach(var classes in ClassesView!)
            {
                temp.Add(classes);
            }
            ClassesView.Clear();
            foreach (var classes in temp)
            {
                ClassesView.Add(classes!);
            }
            
        }

        private void CourseGroups_SelectionChanged(object? parameter)
        {
            var values = (object[]) parameter!;
            Group newGroup = (values[0] as Group)!;
            Class oldClass = (values[1] as Class)!;
            foreach(var @class in oldClass.Course!.Classes)
            {
                if(@class.Group == newGroup)
                {
                    var oldClassView = ClassesView!.First(classes => classes.Course == oldClass!.Course);
                    int pos = ClassesView!.IndexOf(oldClassView);
                    ClassesView.Remove(oldClassView);
                    RefreshAddedCourses();
                    ClassesView.Insert(pos, @class);
                    break;
                }
            }

        }
        private void RemoveCourseBtn_Click(string? courseCode)
        {

            foreach (var classes in ClassesView!)
            {
                if (classes.Course!.Code == courseCode)
                {
                    ClassesView.Remove(classes);
                    break;
                }
            }
        }


        //TODO: remove modify button
        private void ModifyCourseBtn_Click(Class? newClasses)
        {

            foreach (Class classes in ClassesView!)
            {
                if (classes.Course == newClasses!.Course)
                {
                    ClassesView.Remove(classes);
                    break;
                }
            }


            foreach (Class classes in newClasses!.Group!.Classes)
            {
                if (classes.Course!.Code == newClasses.Course!.Code)
                {
                    ClassesView.Add(classes);
                    break;
                }
            }
        }

        private void CreateCourseBtn_Click()
        {
            var createClassesWindow = new ClassCreationView();
            createClassesWindow.Show();
        }

        private void MainWindow_Closed()
        {
            DBSource.CloseConnection();
        }


    }
}
