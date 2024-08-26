using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    internal class CourseClasses
    {
        public int Id { get; set; }
        public Course Course { get; set; }
        public List<Session> Sessions = new List<Session>(3);
        public Group Group { get; set; }
        
       public CourseClasses() 
        {
            Id = -1;
            Course = new Course();
            Group = new Group();
        }

        public CourseClasses(int id, Course course, Group classesGroup)
        {
            Id = id;
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
