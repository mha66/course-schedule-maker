﻿using System;
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
        public ObservableCollection<GroupClasses> Classes { get; set; } = new ObservableCollection<GroupClasses>();
        public List<string> Professors { get; set; } = new List<string>();
        public List<string> TAs { get; set; } = new List<string>();

        
        public Course() : base(-1, "EXCOURSE")
        {
            ExistingOrNew(this);
            Code = "EX123";
        }

        public Course(int id, string name, string code) : base(id, name)
        {
            ExistingOrNew(this);
            Code = code ?? throw new ArgumentNullException(nameof(code));
        }

        public Course(int id, string name, string code, ObservableCollection<Group> groups, List<string> professors, List<string> tas)
        {
            ExistingOrNew(this);
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Code = code ?? throw new ArgumentNullException(nameof(code));
            Groups = groups ?? throw new ArgumentNullException(nameof(groups));
            Professors = professors ?? throw new ArgumentNullException(nameof(professors));
            TAs = tas ?? throw new ArgumentNullException(nameof(tas));
        }

        public static Course ExistingOrNew(Course course)
        {
            foreach (var existingCourse in MainWindow.Courses)
            {
                if(existingCourse == course)
                    return existingCourse;
            }
            MainWindow.Courses.Add(course); 
            return course;
        }

        //attach group to course only using this
        public void AddClasses(GroupClasses classes)
        {
            Classes.Add(classes);
        }

        public void RemoveClasses(GroupClasses classes)
        {
            Classes.Remove(classes);
        }
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

        public override bool Equals(object? obj)
        {
            return obj is Course course &&
                   Id == course.Id &&
                   Name == course.Name &&
                   Code == course.Code;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Code);
        }

        public static bool operator ==(Course? left, Course? right)
        {
            return EqualityComparer<Course>.Default.Equals(left, right);
        }

        public static bool operator !=(Course? left, Course? right)
        {
            return !(left == right);
        }
    }
}
