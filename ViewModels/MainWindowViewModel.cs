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

        public Grid ScheduleGridUI { get; set; }
        #region Commands
        public ICommand CoursesComboBox_SelectionChangedCmd { get; private set; }
        public ICommand AddCourseBtn_ClickCmd { get; private set; }
        public ICommand CourseGroups_SelectionChangedCmd { get; private set; }
        public ICommand RemoveCourseBtn_ClickCmd { get; private set; }
        public ICommand ModifyCourseBtn_ClickCmd { get; private set; }
        public ICommand CreateCourseBtn_ClickCmd { get; private set; }
        public ICommand MainWindow_ClosedCmd { get; private set; }
        public ICommand CourseGroups_LoadedCmd { get; private set; }
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

            ScheduleGridUI = new Grid() { ShowGridLines = true };
            SetupScheduleGrid1();

            CoursesComboBox_SelectionChangedCmd = new RelayCommand(coursesComboBox_SelectionChanged);
            AddCourseBtn_ClickCmd = new RelayCommand(AddCourseBtn_Click);
            CourseGroups_SelectionChangedCmd = new RelayCommand<Class>(CourseGroups_SelectionChanged);
            RemoveCourseBtn_ClickCmd = new RelayCommand<string>(RemoveCourseBtn_Click);
            ModifyCourseBtn_ClickCmd = new RelayCommand<Class>(ModifyCourseBtn_Click);
            CreateCourseBtn_ClickCmd = new RelayCommand(CreateCourseBtn_Click);
            MainWindow_ClosedCmd = new RelayCommand(MainWindow_Closed);
            CourseGroups_LoadedCmd = new RelayCommand<ComboBox>(CourseGroups_Loaded);

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


        private void AddToSchedule(TextBlock textBlock, int row, int column)
        {
            ScheduleChildren.Insert(row * COLUMNS + column, textBlock);
            ScheduleGrid[row, column] = textBlock;
        }

        //TODO: optimize this
        private void SetupScheduleGrid1()
        {
            //adds row and column definitions
            for (int i = 0; i < 18; i++)
            {
                ScheduleGridUI.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                if (i < 8)
                    ScheduleGridUI.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }

            //adds titles for each row and column
            ScheduleGridUI.Children.Add(new TextBlock() { Text = "Days/Period" });
            for (int i = 0; i <= 16; i++)
            {
                //column titles (periods 0-16)
                TextBlock periodTB = new TextBlock() { Text = i.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                Grid.SetColumn(periodTB, i + 1);
                ScheduleGridUI.Children.Add(periodTB);
                //row titles (days of the week)
                if (i < 7)
                {
                    TextBlock dayTB = new TextBlock() { Text = $"{(DayOfWeek)((i + 5) % 7)}", Margin = new Thickness(0, 10, 0, 0) };
                    Grid.SetRow(dayTB, i + 1);
                    ScheduleGridUI.Children.Add(dayTB);
                }
            }
            //adds empty textblocks(????) for courses to be added
            for (int i = 0; i < 7; i++)
            {
                for (int j = 0; j < 17; j++)
                {
                    ScheduleGrid[i, j] = new TextBlock();
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
                    TextBlock tb = ScheduleGrid[i, j];
                    Grid.SetRow(tb, i + 1);
                    Grid.SetColumn(tb, j + 1);
                    if (!ScheduleGridUI.Children.Contains(tb)) //???
                        ScheduleGridUI.Children.Add(tb);
                }
        }


        //TODO: remove this
        private void SetupScheduleGrid()
        {
            //ScheduleChildren.Add(new TextBlock() { Text = "Days/Period" });
            //adds row and column definitions
            for (int i = 0; i < ROWS; i++)
            {
                for (int j = 0; j < COLUMNS; j++)
                {

                    var textBlock = new TextBlock()
                    {
                        //Margin = new Thickness(5, 10, 0 , 0),

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


        //TODO: selection not working
        private void CourseGroups_SelectionChanged(Class? newClasses)
        {
            int pos = ClassesView!.Count - 1;
            //Class? newClasses = (sender as ComboBox)!.Tag as Class;
            var oldClass = ClassesView.First(classes => classes.Course == newClasses!.Course);
            pos = ClassesView.IndexOf(oldClass);
            ClassesView.Remove(oldClass);
            //addedCourses.Items.Refresh();
            ClassesView.Insert(pos, newClasses!.Group!.Classes.First(classes => classes.Course == newClasses!.Course));

        }
        private void RemoveCourseBtn_Click(string? courseCode)
        {

            // Button button = (sender as Button)!;
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

            //Class? newClasses = (sender as Button)!.Tag as Class;
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

        private void CreateCourseBtn_Click()
        {
            var createClassesWindow = new ClassCreationView();
            createClassesWindow.Show();
        }

        private void MainWindow_Closed()
        {
            DBSource.CloseConnection();
        }

        //TODO: binding causes an infinite loop and a crash after changing combo box selection several times
        private void CourseGroups_Loaded(ComboBox? comboBox)
        {
            var hasBinding = comboBox!.GetBindingExpression(ComboBox.SelectedItemProperty);
            if (hasBinding == null)
            {
                Binding binding = new Binding("Group");
                //var newClass = (sender as ComboBox)!.Tag as Class;
                //var oldClass = ClassesView!.First(classes => classes.Course == newClass!.Course);
                binding.Source = comboBox.Tag as Class;
                comboBox.SetBinding(ComboBox.SelectedItemProperty, binding);
            }
            //Task.WaitAll(new Task[] { Task.Delay(1000) });

        }

    }
}
