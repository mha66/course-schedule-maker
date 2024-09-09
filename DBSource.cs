using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseScheduleMaker
{
    public class DBSource
    {
        //TODO: Insert into enum tables
        public static void Connect()
        {
            SQLiteConnection conn = CreateConnection();
            CreateTables(conn);
            //InsertData(conn);
            //ReadData(conn);
            conn.Close();
        }

        public static SQLiteConnection CreateConnection()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source=CoursesDB.db; Version = 3; New = True; Compress = True; ");

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

        public static void CreateTables(SQLiteConnection conn)
        {
            SQLiteCommand cmd = conn.CreateCommand();
            cmd.CommandText = """
                                 CREATE TABLE IF NOT EXISTS Course(
                                    CourseId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT,
                                    Code TEXT NOT NULL
                                 );

                                 CREATE TABLE IF NOT EXISTS "Group"(
                                    GroupId INTEGER NOT NULL PRIMARY KEY,
                                    Name TEXT
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
    }
}
