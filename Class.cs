using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    public class Class : DBObject
    {
        public override string Name { get => $"{Course!.Code} {Group!.Name}"; }
        public Course? Course { get; set; }
        public List<Session> Sessions = new List<Session>(3);


        public Group? Group {  get; set; }

       public Class() : base(-1)
        {
            Course = new Course();
            Group = new Group();
            Course.AddClasses(this);

        }

        //TODO: maybe remove????
        public Class(int id, Course course) : base(id)
        {
            Course = course;
            Course.AddClasses(this);

        }
        public Class(int id, Course course, Group classesGroup) : base(id)
        {
            Course = course;
            Course.AddClasses(this);
            classesGroup.AddClasses(this);
        }

        public Class(Class classes) : base(classes)
        {
            Course = classes.Course;
            //TODO: not safe
            Group = classes.Group;
            Sessions = classes.Sessions;
        }
        public Class(SQLiteDataReader reader)
        {
            Id = reader.GetInt32(0);
        }

        public void AddSession(Session session)
        {
            Sessions.Add(session);
            session.Course = Course!;
            session.Class = this;
            session.Group = Group!;
        }

        public void AddSessions(IEnumerable<Session> sessions)
        {
            foreach (Session session in sessions)
            {
                Sessions.Add(session);
                session.Course = Course!;
                session.Class = this;
                session.Group = Group!;
            }
        }
        public void RemoveSession(Session session) 
        {
            Sessions.Remove(session);
        }
    }
}
