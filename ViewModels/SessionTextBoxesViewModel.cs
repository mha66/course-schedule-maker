using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm;
using CommunityToolkit.Mvvm.ComponentModel;
using CourseScheduleMaker.Models;
namespace CourseScheduleMaker.ViewModels
{

    public class SessionTextBoxesViewModel : ObservableObject
    {
        public static int Count { get; set; } = 0;

        public SessionType SessionKind { get; set; }
        public string? Instructor { get; set; }
        public DayOfWeek Day { get; set; }
        public int Period { get; set; }
    }
}
