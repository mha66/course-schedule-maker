using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    internal class Course : DBObject
    {
    
        public string Code { get; set; }
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<string> Professors { get; set; } = new List<string>();
        public List<string> TAs { get; set; } = new List<string>();

        
        public Course() : base(-1, "EXCOURSE")
        {
            Code = "EX123";
        }

        public Course(int id, string name, string code) : base(id, name)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public Course(int id, string name, string code, List<Group> groups, List<string> professors, List<string> tas)
        {
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
    }
}
