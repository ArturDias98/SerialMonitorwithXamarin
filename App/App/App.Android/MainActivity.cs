using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.OS;
using Android.Hardware.Usb;
using Hoho.Android.UsbSerial.Driver;
using System.Collections.Generic;
using Hoho.Android.UsbSerial.Extensions;
using Android.Content;
using System.Linq;
using Hoho.Android.UsbSerial.Util;
using System.Threading.Tasks;

[assembly: UsesFeature("android.hardware.usb.host")]
namespace App.Droid
{
    [Activity(Label = "App", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize )]
    [IntentFilter(new[] { UsbManager.ActionUsbDeviceAttached })]
    [MetaData(UsbManager.ActionUsbDeviceAttached, Resource = "@xml/device_filter")]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        UsbManager usbManager;
        UsbSerialPort selectedPort;
        List<UsbSerialPort> portList;
        SerialInputOutputManager serialIoManager;
        App main;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            usbManager = GetSystemService(Context.UsbService) as UsbManager;

            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            OxyPlot.Xamarin.Forms.Platform.Android.Forms.Init();

            main = new App();
            LoadApplication(main);
        }
        protected override async void OnResume()
        {
            base.OnResume();
            portList = new List<UsbSerialPort>();
            await PopulateListAsync();
            await Connect();
        }
        async Task PopulateListAsync()
        {
            var drivers = await FindAllDriversAsync(usbManager);
            portList.Clear();
            foreach (var driver in drivers)
            {
                var ports = driver.Ports;
                foreach (var port in ports)
                {
                    portList.Add(port);
                }
            }
        }
        internal static Task<IList<IUsbSerialDriver>> FindAllDriversAsync(UsbManager usbManager)
        {
            var table = UsbSerialProber.DefaultProbeTable;
            var prober = new UsbSerialProber(table);
            return prober.FindAllDriversAsync(usbManager);
        }
        async Task Connect()
        {
            selectedPort = portList.FirstOrDefault();
            if (selectedPort == null)
            {
                return;
            }
            var permission = await usbManager.RequestPermissionAsync(selectedPort.Driver.Device, this);
            if (permission)
            {
                serialIoManager = new SerialInputOutputManager(selectedPort)
                {
                    BaudRate = 115200,
                    DataBits = 8,
                    StopBits = StopBits.One,
                    Parity = Parity.None,
                };

                serialIoManager.DataReceived += (sender, e) => {
                    RunOnUiThread(() => {
                        UpdateReceivedData(e.Data);
                    });
                };

                try
                {
                    serialIoManager.Open(usbManager);
                }
                catch (Java.IO.IOException e)
                {
                    //TODO:
                    return;
                }
            }
        }
        void UpdateReceivedData(byte[] data)
        {
            main.Update(data);
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}