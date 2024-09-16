using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CourseScheduleMaker
{
    public class Group : DBObject
    {

        public static Dictionary<int, Group> IdToGroup = new Dictionary<int, Group>();
        public static int MaxId { get => DBSource.GetMaxId(IdToGroup); }
        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public List<Class> Classes { get; set; } = new List<Class>();

        public Group() : base(-1, "EXGROUP") { MainWindow.Groups.Add(this); }
        public Group(int id, string name) : base(id, name)
        {
            ExistingOrNew(this);
        }
        public Group(int id, string name, Course initialCourse) : base(id, name)
        {
            initialCourse.AddGroup(this);
            ExistingOrNew(this);
        }
        public Group(string name, Course initialCourse) : base(name)
        {
            initialCourse.AddGroup(this);
            ExistingOrNew(this);
        }
        public Group(SQLiteDataReader reader)
        {
            Id = reader.GetInt32(0);
            Name = reader.GetString(1);
            IdToGroup.Add(Id, this);
        }
        public static Group ExistingOrNew(Group group)
        {
            foreach (var existingGroup in MainWindow.Groups)
            {
                if (existingGroup == group)
                    return existingGroup;
            }
            group.Id = MaxId + 1;
            IdToGroup.Add(group.Id, group);
            MainWindow.Groups.Add(group);
            return group;
        }
        public void AddCourse(Course course)
        {
            Courses.Add(course); 
        }
        public void RemoveCourse(Course course)
        {
            Courses.Remove(course);
        }

        //attaches classes to the group and if the classes' course isn't in the list then it will add that course to it
        //attach classes to group only using this
        public void AddClasses(Class courseClasses)
        {
            Classes.Add(courseClasses);
            courseClasses.Group = this;
            //if(!Courses.Contains(courseClasses.Course))
            //{
            //   Courses.Add(courseClasses.Course);
            //   courseClasses.Course.Groups.Add(this);
            //}
            
        }

        //***
        public void RemoveClasses(Class courseClasses)
        {
            Classes.Remove(courseClasses);
        }

        public override string? ToString()
        {
            return Name;
        }

        public override bool Equals(object? obj)
        {
            return obj is Group group &&
                   Id == group.Id &&
                   Name == group.Name;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name);
        }
    }
}
