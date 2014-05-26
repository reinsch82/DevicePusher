﻿using System;
using System.Runtime.InteropServices;
namespace DevicePusher {

  public static class UsbNotification {


    public delegate void UsbDeviceRemovedDelegate();
    public static event UsbDeviceRemovedDelegate UsbDeviceRemoved;

    public delegate void UsbDeviceAddedDelegate();
    public static event UsbDeviceAddedDelegate UsbDeviceAdded;

    public const int DbtDevicearrival = 0x8000; // system detected a new device        
    public const int DbtDeviceremovecomplete = 0x8004; // device is gone      
    public const int WmDevicechange = 0x0219; // device change event      
    private const int DbtDevtypDeviceinterface = 5;
    private static readonly Guid GuidDevinterfaceUSBDevice = new Guid("A5DCBF10-6530-11D2-901F-00C04FB951ED"); // USB devices
    private static IntPtr notificationHandle;

    /// <summary>
    /// Registers a window to receive notifications when USB devices are plugged or unplugged.
    /// </summary>
    /// <param name="windowHandle">Handle to the window receiving notifications.</param>
    public static void RegisterUsbDeviceNotification(IntPtr windowHandle) {
      var dbi = new DevBroadcastDeviceinterface {
        DeviceType = DbtDevtypDeviceinterface,
        Reserved = 0,
        ClassGuid = GuidDevinterfaceUSBDevice,
        Name = 0
      };

      dbi.Size = Marshal.SizeOf(dbi);
      var buffer = Marshal.AllocHGlobal(dbi.Size);
      Marshal.StructureToPtr(dbi, buffer, true);

      notificationHandle = RegisterDeviceNotification(windowHandle, buffer, 0);
    }
    
    /// <summary>
    /// Unregisters the window for USB device notifications
    /// </summary>
    public static void UnregisterUsbDeviceNotification() {
      UnregisterDeviceNotification(notificationHandle);
    }

    public static IntPtr HwndHandler(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam, ref bool handled) {
      if (msg == UsbNotification.WmDevicechange) {
        switch ((int)wparam) {
          case UsbNotification.DbtDeviceremovecomplete:
            if (UsbDeviceRemoved != null) {
              UsbDeviceRemoved();  
            }
            break;
          case UsbNotification.DbtDevicearrival:
            if (UsbDeviceAdded != null) {
              UsbDeviceAdded();  
            }
            break;
        }
      }

      handled = false;
      return IntPtr.Zero;
    }


    [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    private static extern IntPtr RegisterDeviceNotification(IntPtr recipient, IntPtr notificationFilter, int flags);

    [DllImport("user32.dll")]
    private static extern bool UnregisterDeviceNotification(IntPtr handle);

    [StructLayout(LayoutKind.Sequential)]
    private struct DevBroadcastDeviceinterface {
      internal int Size;
      internal int DeviceType;
      internal int Reserved;
      internal Guid ClassGuid;
      internal short Name;
    }
  }
}