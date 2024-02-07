using Common;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Advertisement;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace AppScan {
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window {
        private const string TAG = "MainWindow";
        private BluetoothLEAdvertisementWatcher watcher = new();
        private BluetoothLEAdvertisementPublisher pub = new();

        public MainWindow() {
            this.InitializeComponent();
            StartAdv();
        }
        public void StartDeviceWatcher() {
            Log.D(TAG, "StartDeviceWatcher called");
            watcher.ScanningMode = BluetoothLEScanningMode.Active;
            watcher.Received += AdvertisementReceiver;
            watcher.Start();
        }

        private async void AdvertisementReceiver(BluetoothLEAdvertisementWatcher sender,
       BluetoothLEAdvertisementReceivedEventArgs e) {
            using BluetoothLEDevice bleDevice =
                await BluetoothLEDevice.FromBluetoothAddressAsync(e.BluetoothAddress);
            if (bleDevice != null) {
                Log.D(TAG,
                    $"BleDeviceScanner : {e.Advertisement.LocalName} Id- {bleDevice.DeviceId.ToUpper()} Rx- {e.RawSignalStrengthInDBm}");
         
            }
        }

        private void ScanButtonClick(object sender, RoutedEventArgs e) {
            if (btn_scan.Content.ToString() == "Start Scanning") {
                StartDeviceWatcher();
                btn_scan.Content = "Stop Scanning";
            }
            else if (btn_scan.Content.ToString() == "Stop Scanning") {
                watcher.Stop();
                watcher.Received -= AdvertisementReceiver;
                Log.D(TAG, "StopDeviceScanning");
                btn_scan.Content = "Start Scanning";
          
            }
        }

        private void StartAdv() {
            pub.StatusChanged += StatChange;
            pub.Advertisement.ManufacturerData.Add(createData("hello"));
            pub.Start();
        }

        private BluetoothLEManufacturerData createData(string data) {
            var datawriter = new DataWriter();
            datawriter.WriteInt32(data.Length);
            datawriter.WriteString(data);
            return new BluetoothLEManufacturerData(0x7005, datawriter.DetachBuffer());
        }

        private void StatChange(BluetoothLEAdvertisementPublisher sender, BluetoothLEAdvertisementPublisherStatusChangedEventArgs args) {
            DispatcherQueue.TryEnqueue(DispatcherQueuePriority.Normal, () => 
            { myText.Text = $"{args.Status.ToString()}, {args.Error.ToString()}";
            });
        }
    }
}
