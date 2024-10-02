using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public SessionType SessionKind { get; set; }
        public string? Instructor { get; set; }
        public DayOfWeek Day { get; set; }
        public string? Period { get; set; }

        public static ObservableCollection<string?> Periods { get; set; } = new ObservableCollection<string?>()
        {
            "1--2",
            "3--4",
            "5--6",
            "7--8",
            "9--10",
            "11--12",
            "13--14",
            "15--16"
        };
    }
}
