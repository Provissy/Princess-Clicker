using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace priconn_clicker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private bool isInjecting = false;
        private Thread aClickerThread;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern bool SetCursorPos(int x, int y);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern void mouse_event(int dwFlags, int dx, int dy, int cButtons, int dwExtraInfo);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [DllImport("User32.dll")]
        public static extern short GetAsyncKeyState(System.Windows.Forms.Keys vKey);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point { public Int32 X; public Int32 Y; }

        public const int MOUSEEVENTF_LEFTDOWN = 0x02;
        public const int MOUSEEVENTF_LEFTUP = 0x04;

        public MainWindow()
        {
            InitializeComponent();
            btn_ClickSwitchOff.IsEnabled = false;
        }

        private void btn_ClickSwitchOn_Click(object sender, RoutedEventArgs e)
        {
            startMultiClicker();
            btn_ClickSwitchOn.IsEnabled = false;
            btn_ClickSwitchOff.IsEnabled = true;
        }

        public static void LeftMouseClick(int xpos, int ypos)
        {
            SetCursorPos(xpos, ypos);
            mouse_event(MOUSEEVENTF_LEFTDOWN, xpos, ypos, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, xpos, ypos, 0, 0);
        }

        private void btn_test_Click(object sender, RoutedEventArgs e)
        {

        }

        private void startMultiClicker()
        {
            aClickerThread = new Thread(() =>
            {
                Win32Point w32Mouse = new Win32Point();
                while (true)
                {
                    // Update cur pointer pos.
                    GetCursorPos(ref w32Mouse);
                    this.Dispatcher.Invoke(() => tbl_test.Text = w32Mouse.X.ToString() + ", " + w32Mouse.Y.ToString());

                    // Get cur RBTN status.
                    if (GetAsyncKeyState(System.Windows.Forms.Keys.RButton) == Int16.MinValue)
                    {
                        this.Dispatcher.Invoke(() => ecl_Status.Visibility = Visibility.Visible);
                        isInjecting = true;
                    }

                    // START INJECTING MULTIPLE CLICKS UNTIL BTTON IS RELEASED.
                    if (isInjecting)
                    {
                        GetCursorPos(ref w32Mouse);
                        LeftMouseClick(w32Mouse.X, w32Mouse.Y);
                        Thread.Sleep(10);
                        if (GetAsyncKeyState(System.Windows.Forms.Keys.RButton) != Int16.MinValue)
                        {
                            isInjecting = false;
                        }
                    }

                    else
                    {
                        this.Dispatcher.Invoke(() => { tbl_test2.Text = " "; ecl_Status.Visibility = Visibility.Hidden; }) ;
                    }
                }
            });
            aClickerThread.Start();
        }

        private void btn_ClickSwitchOff_Click(object sender, RoutedEventArgs e)
        {
            aClickerThread.Abort();
            btn_ClickSwitchOff.IsEnabled = false;
            btn_ClickSwitchOn.IsEnabled = true;
        }
    }
}
