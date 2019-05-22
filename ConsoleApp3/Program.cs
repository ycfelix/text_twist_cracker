using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace ConsoleApp3
{
    class Program
    {
        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("User32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll", EntryPoint = "FindWindowEx")]
        private static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpClassName, string lpWindowName);

        private static uint WM_KEYDOWN = 0x100, WM_KEYUP = 0x101;

        public static List<string> CrackText(string input)
        {
            if (input.Length == 2)
            {
                List<string> result = new List<string>();
                result.Add(input[0].ToString() + input[1].ToString());
                result.Add(input[1].ToString() + input[0].ToString());
                return result;
            }
            else if (input.Length > 2)
            {
                List<string> result = new List<string>();
                for (int i = 0; i < input.Length; i++)
                {
                    List<string> keys = CrackText(input.Remove(i,1));
                    if (keys != null)
                    {
                        for (int j = 0; j < keys.Count; j++)
                        {
                            keys[j] = input[i].ToString()+ keys[j];
                        }
                        result.AddRange(keys);
                    }
                }
                return result;
            }
            return null;
        }


        public static void SendText(IntPtr hwnd,string key)
        {
            if (hwnd != IntPtr.Zero)
            {
                if (SetForegroundWindow(hwnd))
                {   
                    SendKeys.Send(key);
                    //SendMessage(hwnd, WM_KEYDOWN, new IntPtr(0x41), new IntPtr(0));
                    //Thread.Sleep(50);
                    //SendMessage(hwnd, WM_KEYUP, new IntPtr(0x41), new IntPtr(0));
                    Debug.WriteLine("success");
                }
                //SendKeys.SendWait("{Enter}");
                
            }
            else
            {
                Debug.WriteLine("fail, nullptr");
            }
            
        }
        static void Main(string[] args)
        {
            Process p = Process.GetProcessById(0x36DC);//get the process of the game

            //SendText(p.MainWindowHandle, "ED");
            string input_command = null;
            do
            {
                Console.WriteLine("input chars to crack, 0 to exit...");
                input_command = Console.ReadLine();
                var t = CrackText(input_command);
                foreach (var key in t)
                {
                    SendText(p.MainWindowHandle, key);
                    SendKeys.SendWait("{Enter}");
                }
            }while (input_command != "0");
            var x = Console.ReadLine();
            
        }
    }
}
