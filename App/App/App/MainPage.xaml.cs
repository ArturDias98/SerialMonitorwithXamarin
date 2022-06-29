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
        private double _analogData;
        private LineSeries serie;
        const int SPAN = 30;
        //private Timer timer = new Timer(50);
        //private Random random = new Random();
        private double step;
        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            ConfigureGraph();

            //timer.Elapsed += Timer_Elapsed;
            //timer.Start();  
        }

        //private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        //{
        //    Update(random.Next(0, 1023));
        //}

        private void ConfigureGraph()
        {
            Model = new PlotModel { Title = "Analog Data" };
            Model.Axes.Add(new DateTimeAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = DateTimeAxis.ToDouble(DateTime.Now),
                Maximum = DateTimeAxis.ToDouble(DateTime.Now + TimeSpan.FromSeconds(SPAN)),
                Title = "Time"
            });
            step = model.Axes[0].Maximum - model.Axes[0].Minimum;
            Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = 0,
                Maximum = 1050
            });

            serie = new LineSeries();

            Model.Series.Add(serie);
        }

        public void Update(double data)
        {
            AnalogData = data;
            var time = DateTimeAxis.ToDouble(DateTime.Now);
            var s = (LineSeries)Model.Series[0];
            s.Points.Add(new DataPoint(time, data));
            if (Model.Axes[0].Maximum < time)
            {
                Model.Axes[0].Maximum = time;
                Model.Axes[0].Minimum = time - step;
            }
            Model.InvalidatePlot(true);
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
        public double AnalogData
        {
            get 
            { 
                return _analogData; 
            }
            set 
            { 
                _analogData = value;
                OnPropertyChanged(nameof(AnalogData));
            }
        }


    }
}
