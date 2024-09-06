using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CourseScheduleMaker
{
    public enum SessionType
    {
        [Description("Lecture"), XmlEnum("Lecture")]
        Lec = 0,
        [Description("Section"), XmlEnum("Section")]
        Sec = 1,
        [Description("Laboratory"), XmlEnum("Lecture")]
        Lab = 2
    }
    public class Session : DBObject
    {
        public override string Name { get => $"{SessionClasses.Name} {SessionKind}"; }
        public Course SessionCourse { get; set; }
        public Group SessionGroup { get; set; }
        public GroupClasses SessionClasses { get; set; }
        public SessionType SessionKind { get; set; }
        public string Instructor {  get; set; }
        public DayOfWeek Day {  get; set; }
        public int Period { get; set; }

        public Session() : base(-1)
        {
            SessionKind = SessionType.Lec;
            Instructor = "John Doe";
            Day = new DayOfWeek();
            Period = 0;
        }

        public Session(int id, SessionType sessionKind, string instructor, DayOfWeek day, int period) : base(id)
        {
            SessionKind = sessionKind;
            Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
            Day = day;
            Period = period;
        }

        public Session(int id, GroupClasses sessionClasses, SessionType sessionKind, string instructor, DayOfWeek day, int period) : base(id)
        {
            SessionKind = sessionKind;
            Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
            Day = day;
            Period = period;
            sessionClasses.AddSession(this);
        }

        
        public override string? ToString()
        {
            return $"{SessionCourse.Name} ({SessionCourse.Code})\n{SessionKind} {SessionGroup.Name}\n{Instructor}";
        }
    }
}
