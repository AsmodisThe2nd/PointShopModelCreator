using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Threading;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;

namespace PSMC
{


    class MDLpreview{

            public Bitmap getPreview(object sender, EventArgs e)
            {
                //   IntPtr HWND = FindWindow(null,"Half-Life Model Viewer v1.22");
                var proc = Process.GetProcessesByName("hlmv")[0];
                IntPtr HWND = proc.MainWindowHandle;
                MessageBox.Show(HWND.ToString("X8"));

                if (HWND != IntPtr.Zero)
                {
                    User32.SetWindowText(HWND, "gotcha!");
                }
                else
                {
                    MessageBox.Show("Open Task Manager First !");
                }

                IntPtr hWndControl = User32.FindWindowByIndex(HWND, 1);
                MessageBox.Show(hWndControl.ToString("X8"));
                return User32.CaptureApplication(HWND, hWndControl);
            }

        }

        class User32
        {
            #region structs und imports
                [StructLayout(LayoutKind.Sequential)]
                public struct Rect
                {
                    public int left;
                    public int top;
                    public int right;
                    public int bottom;
                }

                [DllImport("user32.dll")]
                private static extern int SetForegroundWindow(IntPtr hWnd);

                private const int SW_RESTORE = 9;

                [DllImport("user32.dll")]
                private static extern IntPtr ShowWindow(IntPtr hWnd, int nCmdShow);

                [DllImport("user32.dll")]
                public static extern IntPtr GetWindowRect(IntPtr hWnd, ref Rect rect);

                [DllImport("user32.dll")]
                public static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);

                [DllImport("user32.dll")]
                public static extern IntPtr FindWindow(string ClassName, string WindowName);

                [DllImport("user32.dll")]
                public static extern IntPtr SetWindowText(IntPtr HWND, string Text);

                [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
                static extern int SendMessage3(IntPtr hwndControl, uint Msg, int wParam, StringBuilder strBuffer); // get text

                [DllImport("user32.dll", EntryPoint = "SendMessage", CharSet = CharSet.Auto)]
                static extern int SendMessage4(IntPtr hwndControl, uint Msg, int wParam, int lParam);  // text length

                [DllImport("user32.dll", CharSet = CharSet.Unicode)]
                static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string lclassName, string windowTitle);
            #endregion

            public static int GetTextBoxTextLength(IntPtr hTextBox)
            {
                // helper for GetTextBoxText
                uint WM_GETTEXTLENGTH = 0x000E;
                int result = SendMessage4(hTextBox, WM_GETTEXTLENGTH,
                  0, 0);
                return result;
            }

            public static IntPtr FindWindowByIndex(IntPtr hWndParent, int index)
            {
                if (index == 0)
                    return hWndParent;
                else
                {
                    int ct = 0;
                    IntPtr result = IntPtr.Zero;
                    do
                    {
                        result = FindWindowEx(hWndParent, result, "mx_class", null);
                        if (result != IntPtr.Zero)
                            ++ct;
                    }
                    while (ct < index && result != IntPtr.Zero);
                    return result;
                }
            }

            public static string GetTextBoxText(IntPtr hTextBox)
            {
                uint WM_GETTEXT = 0x000D;
                int len = GetTextBoxTextLength(hTextBox);
                if (len <= 0) return null;  // no text
                StringBuilder sb = new StringBuilder(len + 1);
                SendMessage3(hTextBox, WM_GETTEXT, len + 1, sb);
                return sb.ToString();
            }

            public static Bitmap CaptureApplication(IntPtr owner, IntPtr hwnd)
            {
                // You need to focus on the application
                SetForegroundWindow(owner);
                ShowWindow(owner, SW_RESTORE);

                // You need some amount of delay, but 1 second may be overkill
                Thread.Sleep(10);

                Rect rect = new Rect();
                IntPtr error = GetWindowRect(hwnd, ref rect);

                // sometimes it gives error.
                while (error == (IntPtr)0)
                {
                    error = GetWindowRect(hwnd, ref rect);
                }

                int width = rect.right - rect.left;
                int height = rect.bottom - rect.top;

                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics.FromImage(bmp).CopyFromScreen(rect.left,
                                                       rect.top,
                                                       0,
                                                       0,
                                                       new Size(width, height),
                                                       CopyPixelOperation.SourceCopy);
                return bmp;

            }

            public static Bitmap PrintWindow(IntPtr hwnd)
            {
                Rect rc = new Rect();

                GetWindowRect(hwnd, ref rc);
                int width = rc.right - rc.left;
                int height = rc.bottom - rc.top;
                Bitmap bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
                Graphics gfxBmp = Graphics.FromImage(bmp);
                IntPtr hdcBitmap = gfxBmp.GetHdc();

                PrintWindow(hwnd, hdcBitmap, 0);

                gfxBmp.ReleaseHdc(hdcBitmap);
                gfxBmp.Dispose();

                return bmp;
            }

        }

    }
