using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;

namespace App
{
    public partial class MainPage : ContentPage, INotifyPropertyChanged
    {
        private PlotModel model;
        private LineSeries serie;
        private Timer plotTimer;
        private Random random = new Random();  
        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            ConfigureGraph();

            plotTimer = new Timer(1000);
            plotTimer.Elapsed += PlotTimer_Elapsed;
            plotTimer.Start();
        }

        private void PlotTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            var s = (LineSeries)Model.Series[0];
            s.Points.Add(new DataPoint(DateTimeAxis.ToDouble(DateTime.Now), random.Next(0, 1023)));
            Model.InvalidatePlot(true);
        }
        private void ConfigureGraph()
        {
            Model = new PlotModel { Title = "Analog Data" };
            Model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now + TimeSpan.FromMinutes(5)),
                Title = "Time"
            });
            Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 1050
            });

            serie = new LineSeries();

            Model.Series.Add(serie);
        }
        public PlotModel Model
        {
            get
            { 
                return model; 
            }
            set
            {
                model = value; 
                OnPropertyChanged();
            }
        }

    }
}
