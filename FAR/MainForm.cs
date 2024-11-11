using FAR.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static FAR.ConfigManager;
using static FAR.Settings;

namespace FAR
{
    public partial class MainForm : Form
    {
        private Thread recoilThread;
        private Thread toggleThread;
        private Thread webAppThread;
        Process webServerProcess = new Process();
        // UPDATE STATUS VOID
        public void UpdateFormStatus()
        {
            if (Settings.Recoil.Enabled)
            {
                label5.Text = "ON";
                label5.ForeColor = System.Drawing.Color.Green;
            }
            else
            {
                label5.Text = "OFF";
                label5.ForeColor = System.Drawing.Color.Red;
            }
        }
        public void UpdateAllStatus()
        {
            // TRAY
            notifyIcon1.Icon.Dispose();
            if (Settings.Recoil.Enabled)
            {
                label5.Text = "ON";
                label5.ForeColor = Color.Green;
                notifyIcon1.Icon = Properties.Resources.retard_fish_green;
            }
            else
            {
                label5.Text = "OFF";
                label5.ForeColor = Color.Red;
                notifyIcon1.Icon = Properties.Resources.retard_fish_red;
            }
            notifyIcon1.ContextMenu.MenuItems[notifyIcon1.ContextMenu.MenuItems.Count - 2].Text = $"Status: {label5.Text}";
        }
        public void timer1_Tick(object sender, EventArgs e)
        {
            UpdateAllStatus();

            if (WebApp.receivedData != null)
            {
                string t = WebApp.receivedData; WebApp.receivedData = null;
                t = t.Replace("[", "").Replace("]", "");
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(t);

                int nX = (int)jObject["X"]; int nY = (int)jObject["Y"]; int nZ = (int)jObject["Z"];

                button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
                button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
                Settings.Mode = "UNIVERSAL";
                comboBox2.ResetText();
                comboBox2.SelectedIndex = -1;
                comboBox2.Enabled = false;
                button5.Enabled = false;
                numericUpDown1.Value = nX;
                numericUpDown2.Value = nY;
                numericUpDown3.Value = nZ;

                Loop.RunningConfig = ConfigManager.Get("UNIVERSAL");
                Loop.RunningConfig.Data[0][0] = nX;
                Loop.RunningConfig.Data[0][1] = nY;
                Loop.RunningConfig.Data[0][2] = nZ;
            } else if (WebApp.receivedConfig != null)
            {
                string t = WebApp.receivedConfig; WebApp.receivedConfig = null;
                t = t.Replace("[", "").Replace("]", "");
                var jObject = Newtonsoft.Json.Linq.JObject.Parse(t);

                string nConfig = (string)jObject["Config"];

                button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
                button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
                Settings.Mode = "CONFIG";
                comboBox2.Enabled = true;
                comboBox2.SelectedIndex = comboBox2.FindStringExact(nConfig);
                button5.Enabled = true;

                Loop.RunningConfig = ConfigManager.Get(nConfig);
                numericUpDown1.Value = Loop.RunningConfig.Data[0][0];
                numericUpDown2.Value = Loop.RunningConfig.Data[0][1];
                numericUpDown3.Value = Loop.RunningConfig.Data[0][2];
            }
        }
        public MainForm()
        {
            ConfigManager.Init();
            InitializeComponent();
            Settings.Load();
            foreach (object item in comboBox1.Items)
            {
                if (item.ToString() == Settings.Keybind)
                {
                    comboBox1.SelectedItem = item;
                }
            }
            if (Settings.Mode == "CONFIG")
            {
                button3.BackColor = Color.FromArgb(32, 32, 32);
                button4.BackColor = Color.FromArgb(35, 35, 35);
                comboBox2.Enabled = true;
            }
            else
            {
                button4.BackColor = Color.FromArgb(32, 32, 32);
                button3.BackColor = Color.FromArgb(35, 35, 35);
                ConfigManager.Config temp = ConfigManager.Get("UNIVERSAL");
                Settings.Recoil.X = temp.Data[0][0];
                Settings.Recoil.Y = temp.Data[0][1];
                Settings.Recoil.Sleep = 20;
                try
                {
                    Settings.Recoil.Sleep = temp.Data[0][2];
                }
                catch
                {
                }
                numericUpDown1.Value = Settings.Recoil.X;
                numericUpDown2.Value = Settings.Recoil.Y;
                numericUpDown3.Value = Settings.Recoil.Sleep;
            }
            UpdateFormStatus();
            timer1.Start();
            comboBox2.Enabled = false;
        }
        // START RECOIL LOOP
        private void Form1_Load(object sender, EventArgs e)
        {
            recoilThread = new Thread(Loop.RecoilMethod);
            recoilThread.Start();
            toggleThread = new Thread(Loop.ToggleMethod);
            toggleThread.Start();
            
            MenuItem ConfigMenu = new MenuItem("Configs", TrayMenu_ConfigMode);
            foreach (string Config in ConfigManager.GetAll())
            {
                comboBox2.Items.Add(Config.ToString());
                ConfigMenu.MenuItems.Add(Config.ToString(), TrayMenu_ConfigSelected);
            }
            notifyIcon1.ContextMenu = new ContextMenu();
            notifyIcon1.ContextMenu.MenuItems.Add("Universal", TrayMenu_UniversalMode);
            notifyIcon1.ContextMenu.MenuItems.Add(ConfigMenu);
            notifyIcon1.ContextMenu.MenuItems.Add("Status: OFF");
            notifyIcon1.ContextMenu.MenuItems.Add("Exit", TrayMenu_Exit);
            int ttl = notifyIcon1.ContextMenu.MenuItems.Count - 1;
            notifyIcon1.ContextMenu.MenuItems[ttl - 1].Enabled = false;
            notifyIcon1.Visible = false;
            UpdateAllStatus();

            // START WEB SERVER
            webAppThread = new Thread(WebApp.socketIoManager);
            webServerProcess.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR\\FAR-WebServer.exe";
            webServerProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            if (Settings.WebServerEnabled) { webAppThread.Start(); webServerProcess.Start(); };
        }
        // TRAY MENU FUNCTINOS
        private void TrayMenu_Exit(object sender, EventArgs e)
        {

            recoilThread.Abort();
            toggleThread.Abort();
            if (Settings.WebServerEnabled)
            {
                webServerProcess.Kill();
                webAppThread.Abort();
            }
            Environment.Exit(0);
        }
        void TrayMenu_UniversalMode(object sender, EventArgs e)
        {
            button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            Settings.Mode = "UNIVERSAL";
            comboBox2.ResetText();
            comboBox2.SelectedIndex = -1;
            comboBox2.Enabled = false;
            Loop.RunningConfig = ConfigManager.Get("UNIVERSAL");
            numericUpDown1.Value = Loop.RunningConfig.Data[0][0];
            numericUpDown1.Value = Loop.RunningConfig.Data[0][1];
            numericUpDown1.Value = Loop.RunningConfig.Data[0][2];
        }
        void TrayMenu_ConfigMode(object sender, EventArgs e)
        {
            button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            Settings.Mode = "CONFIG";
            comboBox2.Enabled = true;
            button5.Enabled = true;
        }
        void TrayMenu_ConfigSelected(object sender, EventArgs e)
        {
            if (Settings.Mode != "CONFIG")
            {
                button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
                button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
                Settings.Mode = "CONFIG";
                comboBox2.Enabled = true;
                button5.Enabled = true;
            }
            string selectedConfig = sender.GetType().GetProperty("Text").GetValue(sender, null).ToString();
            comboBox2.SelectedIndex = comboBox2.FindStringExact(selectedConfig);
            Loop.RunningConfig = ConfigManager.Get(selectedConfig);
            numericUpDown1.Value = Loop.RunningConfig.Data[0][0];
            numericUpDown2.Value = Loop.RunningConfig.Data[0][1];
            numericUpDown3.Value = Loop.RunningConfig.Data[0][2];
        }
// LOAD UP SETTINGS
private void Form1_Shown(object sender, EventArgs e)
        {
            if (Settings.Mode == "CONFIG")
            {
                button3.BackColor = Color.FromArgb(32, 32, 32);
                button4.BackColor = Color.FromArgb(35, 35, 35);
                comboBox2.Enabled = true;
            }
            else
            {
                int[] temp = ConfigManager.Get("UNIVERSAL").Data[0];
                Settings.Recoil.X = temp[0];
                Settings.Recoil.Y = temp[1];
            }
        }
        // MOVABLE WINDOW
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        private void Form1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Show();
            this.WindowState = FormWindowState.Normal;
            notifyIcon1.Visible = false;
        }
        // CLOSE & MINIMIZE
        private void Form1_Closed(object sender, EventArgs e)
        {
            recoilThread.Abort();
            toggleThread.Abort();
            if (Settings.WebServerEnabled)
            {
                webServerProcess.Kill();
                webAppThread.Abort();
            }
            Environment.Exit(0);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            recoilThread.Abort();
            toggleThread.Abort();
            if (Settings.WebServerEnabled)
            {
                webServerProcess.Kill();
                webAppThread.Abort();
            }
            Environment.Exit(0);
        }
        private void button2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            Hide();
            notifyIcon1.Visible = true;
        }
        // UNIVERSAL OR CONFIG MODE
        private void button3_Click(object sender, EventArgs e)
        {
            button4.BackColor = Color.FromArgb(32, 32, 32);
            button3.BackColor = Color.FromArgb(35, 35, 35);
            Settings.Mode = "UNIVERSAL";
            comboBox2.ResetText();
            comboBox2.SelectedIndex = -1;
            comboBox2.Enabled = false;
            Loop.RunningConfig = ConfigManager.Get("UNIVERSAL");
            int[] cfg = Loop.RunningConfig.Data[0];
            Settings.Recoil.X = cfg[0];
            Settings.Recoil.Y = cfg[1];
            try
            {
                Settings.Recoil.Sleep = cfg[2];
            }
            catch (Exception)
            {
                Settings.Recoil.Sleep = 20;
            }
            numericUpDown3.Value = Settings.Recoil.Sleep;
            numericUpDown1.Value = Settings.Recoil.X;
            numericUpDown2.Value = Settings.Recoil.Y;
            numericUpDown1.Enabled = true;
            numericUpDown2.Enabled = true;
            numericUpDown3.Enabled = true;
            button5.Enabled = false;
            comboBox2.Enabled = false;
        }
        private void button4_Click(object sender, EventArgs e)
        {
            button3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(32)))), ((int)(((byte)(32)))));
            button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            Settings.Mode = "CONFIG";
            comboBox2.Enabled = true;
            //////////Settings.UpdateFile();
        }
        // SETTINGS CHANGE UNIVERSAL
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown1.Enabled)
            {
                return;
            }
            int X = (Settings.Recoil.X = Convert.ToInt32(numericUpDown1.Value));
            try
            {
                Loop.RunningConfig.Data[0][0] = X;
            }
            catch
            {
            }
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown2.Enabled)
            {
                return;
            }
            int Y = (Settings.Recoil.Y = Convert.ToInt32(numericUpDown2.Value));
            try
            {
                Loop.RunningConfig.Data[0][1] = Y;
            }
            catch
            {
            }
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (!numericUpDown3.Enabled)
            {
                return;
            }
            int Z = (Settings.Recoil.Sleep = Convert.ToInt32(numericUpDown3.Value));
            try
            {
                Loop.RunningConfig.Data[0][2] = Z;
            }
            catch
            {
            }
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Keybind = comboBox1.Text;
            Loop.UpdateToggle();
            Storage.SaveSettings();
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                return;
            }
            string txt = comboBox2.Text;
            ConfigManager.Config selectedConfig = (Loop.RunningConfig = ConfigManager.Get(txt));
            Settings.Recoil.isSpray = selectedConfig.Data.Length > 1;
            Settings.currentConfigName = txt;
            if (!Settings.Recoil.isSpray)
            {
                int[] cfg = selectedConfig.Data[0];
                Settings.Recoil.X = cfg[0];
                Settings.Recoil.Y = cfg[1];
                try
                {
                    Settings.Recoil.Sleep = cfg[2];
                }
                catch (Exception)
                {
                    Settings.Recoil.Sleep = 20;
                }
                numericUpDown3.Value = Settings.Recoil.Sleep;
                numericUpDown1.Value = Settings.Recoil.X;
                numericUpDown2.Value = Settings.Recoil.Y;
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            comboBox2.Items.Clear();
            comboBox2.Refresh();
            List<string> temp = ConfigManager.GetAll();
            ////////////MenuItem ConfigMenu = new MenuItem("Configs", TrayMenu_ConfigMode);
            //foreach (string v in temp)
            //{
            //    comboBox2.Items.Add(v.ToString());
            //    ConfigMenu.MenuItems.Add(v.ToString(), TrayMenu_ConfigSelected);
            //}
            //comboBox2.Refresh();
            //notifyIcon1.ContextMenu = new ContextMenu();
            //notifyIcon1.ContextMenu.MenuItems.Add("Universal", TrayMenu_UniversalMode);
            //notifyIcon1.ContextMenu.MenuItems.Add(ConfigMenu);
            //notifyIcon1.ContextMenu.MenuItems.Add("Status: OFF");
            //notifyIcon1.ContextMenu.MenuItems.Add("Exit", TrayMenu_Exit);
            //int ttl = notifyIcon1.ContextMenu.MenuItems.Count - 1;
            //notifyIcon1.ContextMenu.MenuItems[ttl - 1].Enabled = false;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            ConfigManagerForm ConfigManagerForm = new ConfigManagerForm();
            ConfigManagerForm.Closed += delegate
            {
                button5.PerformClick();
            };
            ConfigManagerForm.Show();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            SettingsForm SettingsForm = new SettingsForm();
            SettingsForm.Show();
        }
    }
}
