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
        private double _temperature;
        private double _ddp_Ads;
        private double _ddp_Intern;

        private LineSeries serie;
        const int SPAN = 30;
        private double step;
        public MainPage()
        {
            InitializeComponent();

            BindingContext = this;

            ConfigureGraph();

        }

        private void ConfigureGraph()
        {
            Model = new PlotModel { Title = "Monitor" };
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
                Maximum = 50,
                Minimum = 0
            });

            serie = new LineSeries();

            Model.Series.Add(serie);
        }

        public void Update(double temperature, double ddp_ads, double ddp_intern)
        {
            Temperature = temperature;
            Ddp_Ads = ddp_ads;
            Ddp_Intern = ddp_intern; 
            UpdateGraph();
        }

        private void UpdateGraph() 
        {
            var time = DateTimeAxis.ToDouble(DateTime.Now);
            var s = (LineSeries)Model.Series[0];
            s.Points.Add(new DataPoint(time, Temperature));
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
        public double Temperature
        {
            get 
            { 
                return _temperature; 
            }
            set 
            { 
                _temperature = value;
                OnPropertyChanged(nameof(Temperature));
            }
        }      
        public double Ddp_Ads
        {
            get 
            { 
                return _ddp_Ads; 
            }
            set 
            { 
                _ddp_Ads = value;
                OnPropertyChanged(nameof(Ddp_Ads));
            }
        }
        public double Ddp_Intern
        {
            get 
            { 
                return _ddp_Intern; 
            }
            set 
            { 
                _ddp_Intern = value;
                OnPropertyChanged(nameof(Ddp_Intern));
            }
        }


    }
}
