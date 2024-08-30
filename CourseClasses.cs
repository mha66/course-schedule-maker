using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    public class CourseClasses : DBObject
    {
        public override string Name { get => $"{Course.Code} {Group.Name}"; }
        public Course Course { get; set; }
        public List<Session> Sessions = new List<Session>(3);
        public Group Group { get; set; }
        
       public CourseClasses() : base(-1)
        {
            Course = new Course();
            Group = new Group();
        }

        public CourseClasses(int id, Course course, Group classesGroup) : base(id)
        {
            Course = course;
            classesGroup.AddClasses(this);
        }

        public void AddSession(Session session)
        {
            Sessions.Add(session);
            session.SessionCourse = Course;
            session.SessionClasses = this;
            session.SessionGroup = Group;
        }
        public void RemoveSession(Session session) 
        {
            Sessions.Remove(session);
        }
    }
}
