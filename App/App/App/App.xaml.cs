﻿using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App
{
    public partial class App : Application
    {
        Protocol protocol;
        MainPage mainPage;
        public App()
        {
            InitializeComponent();
            mainPage = new MainPage();
            MainPage = mainPage;

            protocol = new Protocol(0xAB, 0xCD, 0xAF, 0xCF, 2);
            protocol.OnDataFromatedEvent += OndataFormatted;
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }

        private void OndataFormatted(double data)
        {
            mainPage.Update(data);
        }

        public void Update(IEnumerable<byte> buffer)
        {
            protocol.Add(buffer);
        }
    }
}
