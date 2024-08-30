using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseScheduleMaker
{
    public enum SessionType
    {
        [Description("Lecture")]
        Lec,
        [Description("Section")]
        Sec,
        [Description("Laboratory")]
        Lab
    }
    public class Session : DBObject
    {
        public override string Name { get => $"{SessionClasses.Name} {SessionKind}"; }
        public Course SessionCourse { get; set; }
        public Group SessionGroup { get; set; }
        public CourseClasses SessionClasses { get; set; }
        public SessionType SessionKind { get; set; }
        public string Instructor {  get; set; }
        public DayOfWeek Day {  get; set; }
        public int Period { get; set; }

        public Session() : base(-1)
        {
            SessionCourse = new Course();
            SessionGroup = new Group();
            SessionClasses = new CourseClasses();
            SessionKind = SessionType.Lec;
            Instructor = "John Doe";
            Day = new DayOfWeek();
            Period = 0;
        }

        public Session(int id, CourseClasses sessionClasses, SessionType sessionKind, string instructor, DayOfWeek day, int period) : base(id)
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
