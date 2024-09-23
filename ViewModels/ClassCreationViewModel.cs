using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseScheduleMaker.Views;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.Windows;
using CourseScheduleMaker.Models;

namespace CourseScheduleMaker.ViewModels
{
    //TODO: add feat to fill all textboxes with last made course info
    public class ClassCreationViewModel : ObservableObject
    {
        //when changing the default value of SelectedSessionsNum, also change the default collection of SessionBoxes
        public ObservableCollection<SessionTextBoxes> SessionBoxes { get; set; } = new ObservableCollection<SessionTextBoxes>() { new SessionTextBoxes(), new SessionTextBoxes() };

        //TODO: put SessionsNum_SelectionChanged() logic inside setter and remove that method/command
        public int SelectedSessionsNum { get; set; } = 2;
        //SetProperty(ref _selectedSessionsNum, value);
        public ObservableCollection<int> SessionsNums { get; set; } = new ObservableCollection<int>() { 1, 2, 3, 4 };

        public string? CourseName { get; set; }
        public string? CourseCode { get; set; }
        public string? CourseGroup { get; set; }

        public ICommand SessionsNum_SelectionChangedCmd { get; private set; }
        public ICommand CreateCourse_ClickCmd { get; private set; }

        public ClassCreationViewModel() 
        {
            SessionsNum_SelectionChangedCmd = new RelayCommand(SessionsNum_SelectionChanged);
            CreateCourse_ClickCmd = new RelayCommand(CreateCourseBtn_Click);
           
        }
        private void SessionsNum_SelectionChanged()
        {
            
            int oldCount = SessionBoxes.Count, newCount = SelectedSessionsNum;

            if (oldCount < newCount)
            {
                while (oldCount != newCount)
                {
                    var newSessionBoxes = new SessionTextBoxes() { Uid = "session" + SessionTextBoxesViewModel.Count };
                    SessionTextBoxesViewModel.Count++;
                    oldCount++;
                    SessionBoxes.Add(newSessionBoxes);
                }
            }
            else if (oldCount > newCount)
            {

                while (oldCount != newCount)
                {
                    SessionBoxes.RemoveAt(SessionBoxes.Count - 1);
                    SessionTextBoxesViewModel.Count--;
                    oldCount--;
                }

            }
        }
        private void CreateCourseBtn_Click()
        {

            Course course = Course.ExistingOrNew(new Course(CourseName!, CourseCode!));
            Group group = Group.ExistingOrNew(new Group(CourseGroup!, course));
            Class classes = new Class(course, group);
            foreach (SessionTextBoxes sessionBox in SessionBoxes)
            {
                var sessionViewModel = sessionBox.DataContext as SessionTextBoxesViewModel;
                classes.AddSession(new Session(sessionViewModel!.SessionKind, sessionViewModel.Instructor!,
                    sessionViewModel.Day, sessionViewModel.Period));
            }
            DBSource.InsertData(classes);
            MessageBox.Show("Courses and groups are updated!");
        }
    }
}
