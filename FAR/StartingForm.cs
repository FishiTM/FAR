﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
using System.Net;

namespace FAR
{
    public partial class StartingForm : Form
    {
        public StartingForm()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(base.Handle, 161, 2, 0);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            base.WindowState = FormWindowState.Minimized;
            Hide();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked) { 
                checkBox1.BackColor = System.Drawing.Color.Lime;
                checkBox1.Text = "Enabled";
            } else { 
                checkBox1.BackColor = System.Drawing.Color.Red;
                checkBox1.Text = "Disabled";
            }
        }

        public void ShowMessageBox(string Message)
        {
            var thread = new Thread(
              () =>
              {
                  MessageBox.Show(Message);
              });
            thread.Start();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            Settings.WebServerEnabled = checkBox1.Checked;

            // CHECK HASH
            string webServerPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR\\FAR-WebServer.exe";
            string savedHash = "DF07C329A3C46E4C5A6AB8AEDDD983AA";
            string checkedHash = "";
            if (Settings.WebServerEnabled)
            {
                
                if (File.Exists(webServerPath))
                { // CHECK SERVER EXISTENCE
                    using (var md5 = MD5.Create())
                    {
                        using (var stream = File.OpenRead(@"C:\Users\admin\source\repos\FARWebServer\PKG\FAR-Server.exe"))
                        {
                            checkedHash = BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", "");
                        }
                    }
                }
                if (savedHash != checkedHash || !File.Exists(webServerPath))
                { // DOWNLOAD FILE
                    using (var client = new WebClient())
                    {
                        client.DownloadFile("https://tmpfiles.org/dl/15458864/far-server.exe", webServerPath);
                    }
                }
            }

            // // START PROGRAM
            this.Hide();
            MainForm MainForm = new MainForm();
            MainForm.Closed += (s, args) => this.Close();
            MainForm.Show();
        }
    }
}