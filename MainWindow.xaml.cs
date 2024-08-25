using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseScheduleMaker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        TextBlock[,] courseBlocks = new TextBlock[7, 17];
        public MainWindow()
        {
            InitializeComponent();
            SetupScheduleGrid();

        }

        private void SetupScheduleGrid()
        {
            //adds row and column definitions
            for (int i = 0; i < 18; i++)
            {
                scheduleGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
                if(i<8)
                    scheduleGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            }
            
            //adds titles for each row and column
            scheduleGrid.Children.Add(new TextBlock() { Text = "Days/Period" });
            for (int i = 0; i <= 16; i++)
            {
                //column titles (periods 0-16)
                TextBlock periodTB = new TextBlock() { Text = i.ToString(), Margin = new Thickness(5, 0, 5, 0) };
                Grid.SetColumn(periodTB, i + 1);
                scheduleGrid.Children.Add(periodTB);
                //row titles (days of the week)
                if (i < 7)
                {
                    TextBlock dayTB = new TextBlock() { Text = $"{(DayOfWeek)((i + 5) % 7)}", Margin = new Thickness(0, 10, 0, 0) };
                    Grid.SetRow(dayTB, i + 1);
                    scheduleGrid.Children.Add(dayTB);
                }
            }
            
            //adds empty textblocks(????) for courses to be added
            for(int i=0; i<7; i++)
            {
                for(int j=0; j<17; j++)
                {
                    courseBlocks[i, j] = new TextBlock();
                }
            }
            UpdateScheduleGrid();
        }

        //updates the grids with the objects in courseBlocks
        private void UpdateScheduleGrid()
        {
            for (int i = 0; i < 7; i++)
                for (int j = 0; j < 17; j++)
                {
                    TextBlock tb = courseBlocks[i, j];
                    Grid.SetRow(tb, i + 1);
                    Grid.SetColumn(tb, j + 1);
                    if(!scheduleGrid.Children.Contains(tb)) //???
                        scheduleGrid.Children.Add(tb);
                }
        }
        private void AddCourseBtn_Click(object sender, RoutedEventArgs e)
        {
            
        }
    }
}