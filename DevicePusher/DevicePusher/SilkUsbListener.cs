using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Forms;
namespace DevicePusher {

  [Guid("3E454C94-2EEA-4F56-B5E2-EFE6FC1997B4")]
  public class SilkUsbListener : IHWEventHandler {
    protected const int S_OK = 0x0000;
    public SilkUsbListener() {
      MessageBox.Show("Dot Net Perls is awesome. 2");
      if (!EventLog.SourceExists("Silk")) {
        //An event log source should not be created and immediately used. 
        //There is a latency time to enable the source, it should be created 
        //prior to executing the application that uses the source. 
        //Execute this sample a second time to use the new source.
        EventLog.CreateEventSource("Silk", "log");
        Console.WriteLine("CreatedEventSource");
        Console.WriteLine("Exiting, execute the application a second time to use the source.");
        // The source is created.  Exit the application to allow it to be registered. 
        return;
      }

      // Create an EventLog instance and assign its source.
      EventLog myLog = new EventLog();
      myLog.Source = "Silk";

      // Write an informational entry to the event log.    
      myLog.WriteEntry("Writing to event log.");
      EventLog.WriteEntry("SilkUsbListener", "Constructor fired");
      //Program.PushUSBDevices();
    }


    #region IHWEventHandler Members
    public int Initialize([MarshalAs(UnmanagedType.LPWStr)] string initCmdLine) {
      MessageBox.Show("Dot Net Perls is awesome. 2");
      EventLog.WriteEntry("SilkUsbListener", "IHWEventHandler Initialize fired");
      //Program.PushUSBDevices();
      return S_OK;
    }

    public int HandleEvent([MarshalAs(UnmanagedType.LPWStr)] string deviceId, [MarshalAs(UnmanagedType.LPWStr)] string altDeviceId, [MarshalAs(UnmanagedType.LPWStr)] string eventType) {
      EventLog.WriteEntry("SilkUsbListener", "IHWEventHandler HandleEvent fired");
      return S_OK;
    }

    public int HandleEventWithContent([MarshalAs(UnmanagedType.LPWStr)] string deviceId, 
            [MarshalAs(UnmanagedType.LPWStr)] string altDeviceId, 
            [MarshalAs(UnmanagedType.LPWStr)] string eventType, 
            [MarshalAs(UnmanagedType.LPWStr)] string pDataObject) {
      EventLog.WriteEntry("SilkUsbListener", "IHWEventHandler HandleEventWithContent fired");
      return S_OK;
    }

    #endregion
  }

  #region Interface IHWEventHandler
  [ComImport(),
    InterfaceType(ComInterfaceType.InterfaceIsIUnknown),
    Guid("C1FB73D0-EC3A-4BA2-B512-8CDB9187B6D1")]
  public interface IHWEventHandler {
    [PreserveSig()]
    int Initialize(
        [MarshalAs(UnmanagedType.LPWStr)] string initCmdLine);

    [PreserveSig()]
    int HandleEvent(
        [MarshalAs(UnmanagedType.LPWStr)] string  deviceId,
        [MarshalAs(UnmanagedType.LPWStr)] string  altDeviceId,
        [MarshalAs(UnmanagedType.LPWStr)] string  eventType);

    [PreserveSig()]
    int HandleEventWithContent(
        [MarshalAs(UnmanagedType.LPWStr)] string  deviceId,
        [MarshalAs(UnmanagedType.LPWStr)] string  altDeviceId,
        [MarshalAs(UnmanagedType.LPWStr)] string  eventType,
        [MarshalAs(UnmanagedType.LPWStr)] string  pDataObject);
  }
  #endregion
}
