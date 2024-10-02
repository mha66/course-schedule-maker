using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CourseScheduleMaker.Models;
using CourseScheduleMaker.ViewModels;
using CourseScheduleMaker.Views;

namespace CourseScheduleMaker
{
    public class DBSource
    {
        private static SQLiteConnection Connection { get; set; } = CreateConnection();

        public static int GetMaxId<T>(Dictionary<int, T> dictionary)
        {
            int max = Int32.MinValue;
            foreach (var key in dictionary.Keys)
            {
                if (key > max)
                    max = key;
            }
            return max;
        }
        public static void Initialize()
        {
            
            CreateTables();
            InsertEnumData();
        }

        public static void CloseConnection()
        {
            Connection.Close();
        }
        public static SQLiteConnection CreateConnection()
        {
            var conn = new SQLiteConnection("Data Source=CoursesDB.db; Version = 3; New = True; Compress = True; ");

            try
            {
                conn.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            return conn;
        }

        public static void CreateTables()
        {
            SQLiteCommand cmd = Connection.CreateCommand();
            cmd.CommandText = """
                                 CREATE TABLE IF NOT EXISTS Course(
                                    CourseId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT,
                                    Code TEXT NOT NULL UNIQUE
                                 );

                                 CREATE TABLE IF NOT EXISTS "Group"(
                                    GroupId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT UNIQUE
                                 );

                                 CREATE TABLE IF NOT EXISTS CourseGroup (
                                    CourseId INTEGER NOT NULL,
                                    GroupId INTEGER NOT NULL,

                                    PRIMARY KEY (CourseId, GroupId),
                                    FOREIGN KEY (CourseId) REFERENCES Course (CourseId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION,
                                    FOREIGN KEY (GroupId) REFERENCES "Group" (GroupId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION
                                 );

                                 CREATE TABLE IF NOT EXISTS Class(
                                    ClassId INTEGER NOT NULL PRIMARY KEY,
                                   	CourseId INTEGER NOT NULL,
                                    GroupId INTEGER NOT NULL,

                                    FOREIGN KEY (CourseId) REFERENCES Course (CourseId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION,
                                    FOREIGN KEY (GroupId) REFERENCES "Group" (GroupId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION
                                 );

                                 CREATE TABLE IF NOT EXISTS SessionType(
                                    SessionTypeId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT NOT NULL
                                 );

                                 CREATE TABLE IF NOT EXISTS DayOfWeek(
                                    DayOfWeekId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT NOT NULL
                                 );

                                 CREATE TABLE IF NOT EXISTS Session(
                                    SessionId INTEGER NOT NULL PRIMARY KEY,
                                    Kind INTEGER,
                                    Instructor TEXT,
                                    Day INTEGER,
                                    ClassId INTEGER NOT NULL,
                                   	CourseId INTEGER NOT NULL,
                                    GroupId INTEGER NOT NULL,
                                    Period INTEGER,

                                    FOREIGN KEY (Day) REFERENCES DayOfWeek (DayOfWeekId) 
                                             ON DELETE SET NULL ON UPDATE NO ACTION,
                                    FOREIGN KEY (Kind) REFERENCES SessionType (SessionTypeId) 
                                             ON DELETE SET NULL ON UPDATE NO ACTION,
                                    FOREIGN KEY (CourseId) REFERENCES Course (CourseId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION,
                                    FOREIGN KEY (GroupId) REFERENCES "Group" (GroupId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION,
                                    FOREIGN KEY (ClassId) REFERENCES Class (ClassId) 
                                             ON DELETE CASCADE ON UPDATE NO ACTION
                                 );
                                 """;

            cmd.ExecuteNonQuery();
        }
        public static void InsertEnumData()
        {
            SQLiteCommand cmd = Connection.CreateCommand();

            cmd.CommandText = """
                                 INSERT OR IGNORE INTO SessionType (SessionTypeId, Name)
                                 VALUES
                                 	(0, 'Lec'),
                                 	(1, 'Sec'),
                                 	(2, 'Lab');
                                 """;

            cmd.ExecuteNonQuery();

            cmd.CommandText = """
                                 INSERT OR IGNORE INTO DayOfWeek (DayOfWeekId, Name)
                                 VALUES
                                    (0, 'Sunday'),
                                    (1, 'Monday'),
                                    (2, 'Tuesday'),
                                    (3, 'Wednesday'),
                                    (4, 'Thursday'),
                                    (5, 'Friday'),
                                    (6, 'Saturday');
                                 """;

            cmd.ExecuteNonQuery();

        }

        
        public static void InsertData(Class newClass)
        {
            Course course = newClass.Course!;
            Group group = newClass.Group!;

            SQLiteCommand cmd = Connection.CreateCommand();
            
            cmd.Parameters.AddWithValue("@CourseName", course.Name);
            cmd.Parameters.AddWithValue("@Code", course.Code);

            cmd.Parameters.AddWithValue("@GroupName", group.Name);
            cmd.Parameters.AddWithValue("@GroupName", group.Name);

            int i = 0;
            foreach (Session session in newClass.Sessions)
            {
                cmd.Parameters.AddWithValue($"@Session{i}Kind", (int)session.Kind);
                cmd.Parameters.AddWithValue($"@Session{i}Instructor", session.Instructor);
                cmd.Parameters.AddWithValue($"@Session{i}Day", (int)session.Day);
                cmd.Parameters.AddWithValue($"@Session{i}Period", session.Period);
                i++;
            }

            cmd.CommandText = """
                                 CREATE TEMP TABLE IF NOT EXISTS Variables (
                                   	CourseId INTEGER NOT NULL,
                                    GroupId INTEGER NOT NULL
                                 );

                                 INSERT OR IGNORE INTO Course (Name, Code)
                                 VALUES
                                 	(@CourseName, @Code);

                                 INSERT OR IGNORE INTO "Group" (Name)
                                 VALUES
                                 	(@GroupName);

                                 INSERT OR REPLACE INTO Variables (CourseId, GroupId)
                                 VALUES (
                                    (SELECT CourseId FROM Course WHERE Code = @Code),
                                    (SELECT GroupId FROM "Group" WHERE Name = @GroupName)
                                 );

                                 INSERT OR IGNORE INTO CourseGroup (CourseId, GroupId)
                                 VALUES (
                                    (SELECT CourseId FROM Variables),
                                    (SELECT GroupId FROM Variables)
                                 );

                                 INSERT OR IGNORE INTO Class (CourseId, GroupId)
                                 VALUES (
                                    (SELECT CourseId FROM Variables),
                                    (SELECT GroupId FROM Variables)
                                 );

                                 INSERT OR IGNORE INTO Session (Kind, Instructor, Day, Period, ClassId, CourseId, GroupId)
                                 VALUES                    
                                 """;
            for(int j = 0; j<i; j++)
            {
                cmd.CommandText += $"""
                                 (
                                 @Session{j}Kind, @Session{j}Instructor, @Session{j}Day, @Session{j}Period,

                                 (SELECT DISTINCT ClassId FROM Class 
                                 WHERE CourseId = (SELECT CourseId FROM Variables) AND GroupId = (SELECT GroupId FROM Variables)),

                                 (SELECT CourseId FROM Variables),
                                 (SELECT GroupId FROM Variables)
                                 )

                    """;
                cmd.CommandText += (j == i - 1) ? ";" : ",";
            }
            cmd.CommandText += "DROP TABLE IF EXISTS Variables;";
            cmd.ExecuteNonQuery();
     
        }
        
        public static void InsertIntoObjects(SQLiteCommand cmd, string columnNames, string tableName)
        {
            cmd.CommandText = $"SELECT {columnNames} FROM {tableName};";
            SQLiteDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                switch (tableName)
                {
                    case "Course":
                        var course = new Course(reader);
                        MainWindowViewModel.Courses.Add(course);
                        break;

                    case "\"Group\"":
                        var group = new Group(reader);
                        MainWindowViewModel.Groups.Add(group);
                        break;

                    case "CourseGroup":
                        int courseId = reader.GetInt32(0), groupId = reader.GetInt32(1);
                        Course.IdToCourse[courseId].AddGroup(Group.IdToGroup[groupId]);
                        break;

                    case "Class":
                        var classes = new Class(reader);
                        break;

                    case "Session":
                        var session = new Session(reader);
                        break;
                }
            }
            reader.Close();
        }
        public static void ReadData()
        {
            SQLiteCommand cmd = Connection.CreateCommand();

            InsertIntoObjects(cmd, "CourseId, Name, Code", "Course");

            InsertIntoObjects(cmd, "GroupId, Name", "\"Group\"");

            InsertIntoObjects(cmd, "CourseId, GroupId", "CourseGroup");

            InsertIntoObjects(cmd, "ClassId, CourseId, GroupId", "Class");

            InsertIntoObjects(cmd, "SessionId, Kind, Instructor, Day, Period, ClassId", "Session");

        }
    }
}
