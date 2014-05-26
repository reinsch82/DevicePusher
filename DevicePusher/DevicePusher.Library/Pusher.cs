using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;

namespace DevicePusher {
  public class Pusher {

    private readonly string _url;
    private readonly string _debugSystemName;

    public Pusher(string url, string debugSystemName = null) {
      _url = url;
      _debugSystemName = debugSystemName;
    }

    public void Push() {
      ManagementObjectCollection collection;
      using (var searcher = new ManagementObjectSearcher(@"Select * From Win32_USBControllerDevice"))
        collection = searcher.Get();

      foreach (var device in collection) {
        var strDeviceName = device["Dependent"].ToString();
        const string strQuotes = "'";
        strDeviceName = strDeviceName.Replace("\"", strQuotes);
        var arrDeviceName = strDeviceName.Split('=');
        strDeviceName = arrDeviceName[1];
        var Win32_PnPEntity = "Select * From Win32_PnPEntity " + "Where DeviceID =" + strDeviceName;
        var mySearcher = new ManagementObjectSearcher(Win32_PnPEntity);
        foreach (var mobj in mySearcher.Get()) {
          var strDeviceID = mobj["DeviceID"].ToString();
          var arrDeviceID = strDeviceID.Split('\\');

          var devId = arrDeviceID[2].Trim('{', '}');
          var splitId = devId.Split('&');
          if (splitId.Length > 1) {
            devId = splitId[1];
          }
          try {
            using (var client = new System.Net.WebClient()) {
              var data = new NameValueCollection();
              data["device"] = mobj["Name"].ToString();
              data["system"] = mobj["SystemName"].ToString();
              data["id"] = devId;

              if (!string.IsNullOrEmpty(_debugSystemName)) {
                data["system"] = _debugSystemName;
              }

              client.UploadValues(_url + "/setDevice/", "POST", data);
            }

          }
          catch (Exception) {
          }
        }
      }
      collection.Dispose();
    }

  }
}
