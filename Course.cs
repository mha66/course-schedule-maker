using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    public class Course : DBObject
    {
    
        public string Code { get; set; }

        // *****type could just be List<Group>
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public List<string> Professors { get; set; } = new List<string>();
        public List<string> TAs { get; set; } = new List<string>();

        
        public Course() : base(-1, "EXCOURSE")
        {
            MainWindow.Courses.Add(this);
            Code = "EX123";
        }

        public Course(int id, string name, string code) : base(id, name)
        {
            MainWindow.Courses.Add(this);
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public Course(int id, string name, string code, ObservableCollection<Group> groups, List<string> professors, List<string> tas)
        {
            MainWindow.Courses.Add(this);
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Groups = groups ?? throw new ArgumentNullException(nameof(groups));
            Professors = professors ?? throw new ArgumentNullException(nameof(professors));
            TAs = tas ?? throw new ArgumentNullException(nameof(tas));
        }


        //attach group to course only using this
        public void AddGroup(Group group)
        {
            Groups.Add(group);
            group.AddCourse(this);
        }

        public void RemoveGroup(Group group)
        {
            Groups.Remove(group);
        }

        public void AddProfessor(string professor)
        {
            Professors.Add(professor);
        }

        public void RemoveProfessor(string professor)
        {
            Professors.Remove(professor);
        }

        public void AddTA(string ta)
        {
            TAs.Add(ta);
        }

        public void RemoveTA(string ta)
        {
            TAs.Remove(ta);
        }

        public override string? ToString()
        {
            return $"({Code})";
        }
    }
}
