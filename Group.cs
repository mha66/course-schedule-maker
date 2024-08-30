using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    public class Group : DBObject
    {
 
        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public List<GroupClasses> Classes { get; set; } = new List<GroupClasses>();

        public Group() : base(-1, "EXGROUP") { MainWindow.Groups.Add(this); }
        public Group(int id, string name) : base(id, name)
        {
            MainWindow.Groups.Add(this);
        }
        public Group(int id, string name, Course initialCourse) : base(id, name)
        {
            initialCourse.AddGroup(this);
            MainWindow.Groups.Add(this);
        }

        public void AddCourse(Course course)
        {
            Courses.Add(course); 
        }
        public void RemoveCourse(Course course)
        {
            Courses.Remove(course);
        }

        //attaches classes to the group and if the classes' course isn't in the list then it will add that course to it***
        //attach classes to group only using this
        public void AddClasses(GroupClasses courseClasses)
        {
            Classes.Add(courseClasses);
            courseClasses.Group = this;
            if(!Courses.Contains(courseClasses.Course))
            {
               Courses.Add(courseClasses.Course);
               courseClasses.Course.Groups.Add(this);
            }
            
        }

        //***
        public void RemoveClasses(GroupClasses courseClasses)
        {
            Classes.Remove(courseClasses);
        }

        public override string? ToString()
        {
            return Name;
        }
    }
}
