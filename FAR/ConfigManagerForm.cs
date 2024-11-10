using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FAR
{
    public partial class ConfigManagerForm : Form
    {
        public ConfigManagerForm()
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

        private void RefreshConfigs()
        {
            comboBox2.Items.Clear();
            comboBox2.Refresh();
            List<string> temp = ConfigManager.GetAll(noDefault: false);
            foreach (string v in temp)
            {
                comboBox2.Items.Add(v.ToString());
            }
            comboBox2.Refresh();
        }

        private void ConfigManagerForm_Load(object sender, EventArgs e)
        {
            RefreshConfigs();
            ShowAndHide(isNew: false);
        }

        private void ShowAndHide(bool isNew)
        {
            comboBox2.Enabled = !isNew;
            button4.Enabled = !isNew;
            textBox1.Enabled = !isNew;
            button5.Enabled = !isNew;
            dataGridView1.Enabled = isNew;
            numericUpDown3.Enabled = isNew;
            numericUpDown3.Enabled = isNew;
            button6.Enabled = isNew;
            button3.Enabled = isNew;
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            numericUpDown3.Value = 0;
            if (!isNew)
            {
                label2.Text = "n/a";
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
            {
                MessageBox.Show("No Config Selected!");
                return;
            }
            label2.Text = comboBox2.Text;
            ConfigManager.Config oldConfig = ConfigManager.Get(label2.Text);
            ShowAndHide(isNew: true);
            int[][] data = oldConfig.Data;
            foreach (int[] Dat in data)
            {
                dataGridView1.Rows.Add(Dat[0], Dat[1], Dat[2]);
            }
            numericUpDown3.Value = oldConfig.firstBullet;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            List<string> allConfigs = ConfigManager.GetAll(noDefault: false);
            if (allConfigs.Contains(textBox1.Text))
            {
                MessageBox.Show("This Config Already Exists!");
                return;
            }
            if (textBox1.Text.Contains("/") || textBox1.Text.Contains("\\"))
            {
                MessageBox.Show("Invalid Config Name");
                return;
            }
            ShowAndHide(isNew: true);
            label2.Text = textBox1.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            ShowAndHide(isNew: false);
            textBox1.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            List<int[]> DataList = new List<int[]>();
            foreach (DataGridViewRow tRow in (IEnumerable)dataGridView1.Rows)
            {
                DataGridViewCellCollection Cells = tRow.Cells;
                int X = Convert.ToInt32(Cells[0].Value);
                int Y = Convert.ToInt32(Cells[1].Value);
                if (Y < 0)
                {
                    Y = 0;
                }
                int Z = Convert.ToInt32(Cells[2].Value);
                if (Z == 0)
                {
                    Z = 20;
                }
                if (X != 0 || Y != 0)
                {
                    DataList.Add(new int[3] { X, Y, Z });
                }
            }
            int[][] Data = DataList.ToArray();
            int firstBullet = 0;
            firstBullet = ((int)numericUpDown3.Value);
            ConfigManager.Config newConfig = new ConfigManager.Config
            {
                Data = Data,
                firstBullet = firstBullet
            };
            ConfigManager.Create(label2.Text, newConfig);
            ShowAndHide(isNew: false);
            textBox1.Text = "";
            RefreshConfigs();
            MessageBox.Show("Created Config!");
        }
    }
}
