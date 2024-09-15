using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
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
        public override string Name { get => $"{Class.Name} {Kind}"; }
        public Course Course { get; set; }
        public Group Group { get; set; }
        public Class Class { get; set; }
        public SessionType Kind { get; set; }
        public string Instructor {  get; set; }
        public DayOfWeek Day {  get; set; }
        public int Period { get; set; }

        public Session() : base(-1)
        {
            Kind = SessionType.Lec;
            Instructor = "John Doe";
            Day = new DayOfWeek();
            Period = 0;
        }

        public Session(int id, SessionType sessionKind, string instructor, DayOfWeek day, int period) : base(id)
        {
            Kind = sessionKind;
            Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
            Day = day;
            Period = period;
        }

        public Session(int id, Class sessionClasses, SessionType sessionKind, string instructor, DayOfWeek day, int period) : base(id)
        {
            Kind = sessionKind;
            Instructor = instructor ?? throw new ArgumentNullException(nameof(instructor));
            Day = day;
            Period = period;
            sessionClasses.AddSession(this);
        }
        public Session(SQLiteDataReader reader)
        {
            Id = reader.GetInt32(0);
            Kind = (SessionType)reader.GetInt32(1);
            Instructor = reader.GetString(2);
            Day = (DayOfWeek)reader.GetInt32(3);
            Period = reader.GetInt32(4);
            Class.IdToClass[reader.GetInt32(5)].AddSession(this);
        }


        public override string? ToString()
        {
            return $"{Course.Name} ({Course.Code})\n{Kind} {Group.Name}\n{Instructor}";
        }
    }
}
