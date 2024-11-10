using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace FAR
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }
        public static readonly string FARPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\FAR";

        public static readonly string FARSmoothingPath = FARPath + "\\Smoothing.txt";
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

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            if (!File.Exists(FARSmoothingPath))
            {
                File.WriteAllText(FARSmoothingPath, "2" + Environment.NewLine + "0");
            }
            string[] curSettings = File.ReadAllLines(FARSmoothingPath);
            int t_TimeSmoothing = Convert.ToInt32(curSettings[0]);
            int t_YSmoothing = Convert.ToInt32(curSettings[1]);
            if (t_YSmoothing > 0)
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = false;
                checkBox1.Checked = false;
                checkBox2.Checked = true;
                numericUpDown3.Value = t_YSmoothing;
            }
            else
            {
                checkBox1.Enabled = false;
                checkBox2.Enabled = true;
                checkBox1.Checked = true;
                checkBox2.Checked = false;
                numericUpDown3.Value = t_TimeSmoothing;
            }
        }

        private void onPaint(object sender, PaintEventArgs e)
        {
            checkBox1.ForeColor = Color.White;
            checkBox2.ForeColor = Color.White;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                checkBox1.Enabled = false;
                checkBox2.Enabled = true;
                checkBox2.Checked = false;
            }
            Save(checkBox1.Checked, checkBox2.Checked, (int)numericUpDown3.Value);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                checkBox1.Enabled = true;
                checkBox2.Enabled = false;
                checkBox1.Checked = false;
            }
            Save(checkBox1.Checked, checkBox2.Checked, (int)numericUpDown3.Value);
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            Save(checkBox1.Checked, checkBox2.Checked, (int)numericUpDown3.Value);
        }

        public static void Save(bool ts, bool ys, int v)
        {
            if (ts)
            {
                File.WriteAllText(FARSmoothingPath, $"{v}\n0");
            }
            else
            {
                File.WriteAllText(FARSmoothingPath, $"0\n{v}");
            }
            Settings.TimeSmoothing = ts;
            Settings.YSmoothing = ys;
            Settings.SmoothingValue = v;
        }
    }
}
