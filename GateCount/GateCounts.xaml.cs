using System.Windows.Controls;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GateCount
{
    public partial class GateCounts : Page
    {
        GateCounter gateCounterBeaumont;
        GateCounter gateCounterStadium;
        public GateCounts()
        {
            InitializeComponent();
            gateCounterBeaumont = new GateCounter();
            gateCounterStadium = new GateCounter();
        }

        private async void GetCounts_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            // Disable button to avoid double clicks.
            GetCounts.IsEnabled = false;

            //Clear fields by telling user we are contacting the gates
            BeaumontOutput.Text = "Fetching...";
            StadiumOutput.Text = "Fetching...";

            //Fetch counts
            var beaumontTask = gateCounterBeaumont.GetGateCount("http://35.8.222.8/js/count.xml");
            var stadiumTask = gateCounterStadium.GetGateCount("http://35.8.222.9/js/count.xml");

            var tasks = new List<Task<int>> { beaumontTask, stadiumTask };

            //Loop to check on our tasks, that way one can update before the other.
            while (tasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(tasks);
                tasks.Remove(completedTask);

                if (completedTask == beaumontTask)
                {
                    // Update UI with Beaumont count
                    BeaumontOutput.Text = completedTask.Result.ToString();
                }
                else if (completedTask == stadiumTask)
                {
                    // Update UI with Stadium count
                    StadiumOutput.Text = completedTask.Result.ToString();
                }
            }
            //Enable the button, we are finished
            GetCounts.IsEnabled = true;
        }
    }
}