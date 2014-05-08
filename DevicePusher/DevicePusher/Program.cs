using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Collections.Specialized; // need to add System.Management to your project references.
using System.Management;

namespace DevicePusher {

  class Program {
    static void Main(string[] args) {
      PushUSBDevices();
    }

    public static void PushUSBDevices() {

      ManagementObjectCollection collection;
      using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
        collection = searcher.Get();

      foreach (var device in collection) {
        string strDeviceName = device["Dependent"].ToString();
        string strQuotes = "'";
        strDeviceName = strDeviceName.Replace("\"", strQuotes);
        string[] arrDeviceName = strDeviceName.Split('=');
        strDeviceName = arrDeviceName[1];
        string Win32_PnPEntity = "Select * From Win32_PnPEntity "
              + "Where DeviceID =" + strDeviceName;
        ManagementObjectSearcher mySearcher = new ManagementObjectSearcher(Win32_PnPEntity);
        foreach (ManagementObject mobj in mySearcher.Get()) {
          string strDeviceID = mobj["DeviceID"].ToString();
          string[] arrDeviceID = strDeviceID.Split('\\');
          Console.WriteLine("Device Description = " + mobj["Description"].ToString());
          if (mobj["Manufacturer"] != null) {
            Console.WriteLine("Device Manufacturer = "
                + mobj["Manufacturer"].ToString());
          }
          Console.WriteLine("Device Version ID & Vendor ID = " + arrDeviceID[1]);
          Console.WriteLine("Device Name = " + mobj["Name"].ToString());
          Console.WriteLine("System = " + mobj["SystemName"].ToString());
          var devId = arrDeviceID[2].Trim('{', '}');
          var splitId = devId.Split('&');
          if (splitId.Length > 1) {
            devId = splitId[0] + splitId[1];
          }
          Console.WriteLine("Device ID = " + devId);
          Console.WriteLine("\n");
          try {
            using (var client = new System.Net.WebClient()) {
              var data = new NameValueCollection();
              data["device"] = mobj["Name"].ToString();
              data["system"] = mobj["SystemName"].ToString();
              data["id"] = devId;

              var response = client.UploadValues("http://lnz-Reinholdd:9000/setDevice/", "POST", data);
            }

          }
          catch (Exception) {
            Console.WriteLine("unknown device");
          }
        }
      }

      collection.Dispose();
    }
  }
}
