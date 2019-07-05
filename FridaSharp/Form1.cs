using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Threading;

namespace FridaSharp
{
    public partial class Form1 : Form
    {
        static Frida.DeviceManager deviceManager;
        static List<Frida.Device> Devices;
        static List<Frida.Process> Processes;
        static Frida.Session session;
        static Frida.Script script;
        
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
           
        }

        private void Hook(string targetProcess, string scriptLocation)
        {
            try
            {
                Devices = new List<Frida.Device>();
                Processes = new List<Frida.Process>();
                deviceManager = new Frida.DeviceManager(Dispatcher.CurrentDispatcher);
                var devices = deviceManager.EnumerateDevices();
                //we want the local device
                var device = devices.Where(x => x.Name == "Local System").FirstOrDefault();
                if (device == null)
                {
                    Console.WriteLine("[*] Failed to select device!");
                    return;
                }
                var processes = device.EnumerateProcesses();
                foreach (var process in processes)
                {
                    Processes.Add(process);
                }
                Console.WriteLine(String.Format("[*] Got {0} processes...", Processes.Count));

                //find the process
                var target = Processes.Where(x => x.Name == targetProcess).FirstOrDefault();
                if (target == null)
                {
                    foreach (Frida.Process p in Processes)
                    {
                        Console.WriteLine(p.Name);
                    }
                    Console.WriteLine("[*] Failed to identify target process!");
                    return;
                }
                Console.WriteLine(String.Format("[*] Process {0} found...", targetProcess));
                session = device.Attach(target.Pid);
                Console.WriteLine(String.Format("[*] Hooked process with PID {0}", target.Pid.ToString()));
                //Inject the script...
                Console.WriteLine("[*] Injecting script...");
                try
                {
                    Console.WriteLine(String.Format("[*] Loading script from {0}...", scriptLocation));
                    string scriptText = File.ReadAllText(scriptLocation);
                    script = session.CreateScript(scriptText);
                    Console.WriteLine("[*] Script loaded...");
                    script.Message += new Frida.ScriptMessageHandler(script_Message);
                    script.Load();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("[*] Error loading script!");
                    Console.WriteLine(ex.ToString());
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[*] Fail!");
            }
        }
        private void script_Message(object sender, Frida.ScriptMessageEventArgs e)
        {
            if (sender == script)
            {
                Console.WriteLine(String.Format("Message from Script: {0}", e.Message));
                Console.WriteLine(String.Format("Data: {0}", e.Data == null ? "null" : String.Join(", ", e.Data)));
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnHook_Click(object sender, EventArgs e)
        {
            string targetProcess = txtProcessName.Text;
            string scriptLocation = txtScriptLocation.Text;

            Hook(targetProcess, scriptLocation);
        }
    }
}
