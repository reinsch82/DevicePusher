using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Application = System.Windows.Application;

namespace DevicePusher.UI {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window {

    private readonly System.Windows.Forms.NotifyIcon _notifyIcon;

    public MainWindow() {
      InitializeComponent();
      _notifyIcon = new NotifyIcon { BalloonTipIcon = ToolTipIcon.Info, Text = Properties.Resources.NotifyIcon };
      SetNotifyIcon("MobileDevice_16x16.png");

    }

    private void SetNotifyIcon(string sIcon) {
      var iconStream = Application.GetResourceStream(new Uri(@"pack://application:,,/" + sIcon)).Stream;
      var bmp = new Bitmap(iconStream);
      _notifyIcon.Icon = System.Drawing.Icon.FromHandle(bmp.GetHicon());
      _notifyIcon.Visible = true;
      _notifyIcon.ContextMenuStrip = CreateContextMenu();
    }

    private ContextMenuStrip CreateContextMenu() {
      var close = new ToolStripMenuItem { Text = Properties.Resources.ContextMenu_Close };
      close.Click += (sender, args) => Application.Current.Shutdown();
      close.Visible = true;

      var push = new ToolStripMenuItem { Text = Properties.Resources.ContextMenu_Push };
      push.Click += (sender, args) => Push();
      push.Visible = true;

      var contextMenuStrip = new ContextMenuStrip();
      contextMenuStrip.Items.Add(push);
      contextMenuStrip.Items.Add(close);

      return contextMenuStrip;
    }

    private void Push() {
      Task.Factory.StartNew(() => {
        _notifyIcon.ShowBalloonTip(1000, Properties.Resources.NotifyIcon, Properties.Resources.NotifyIcon_StartPush, ToolTipIcon.Info);
        new Pusher(Properties.Settings.Default.DashboardUrl).Push();
        _notifyIcon.ShowBalloonTip(1000, Properties.Resources.NotifyIcon, Properties.Resources.NotifyIcon_EndPush, ToolTipIcon.Info);
      });
    }

    private void MainWindow_OnSourceInitialized(object sender, EventArgs e) {
      var source = HwndSource.FromHwnd(new WindowInteropHelper(this).Handle);
      var windowHandle = source.Handle;
      source.AddHook(UsbNotification.HwndHandler);
      UsbNotification.RegisterUsbDeviceNotification(windowHandle);
      UsbNotification.UsbDeviceAdded += Push;
      Visibility = Visibility.Hidden;

      Push();
    }
  }
}
