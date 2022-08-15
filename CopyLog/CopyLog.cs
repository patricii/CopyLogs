using System;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Drawing;
using System.Linq;

namespace CopyLog
{
    public partial class CopyLog : Form
    {
        string sourceDir = @"Q:\quality_data\factory_nextest_log";
        string errorMessageDriverQ = "Pasta origem não existe ou não foi possivel mapear o driver de rede Q:";
        string errorMessageFolder = "Pasta destino não existe!!!";
        string destinationDir = string.Empty;
        private System.Threading.Timer timer;

        public CopyLog()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }
        private void buttonCopy_Click(object sender, EventArgs e)
        {
            destinationDir = textBoxTo.Text;
            int nStatus = -1;

            if (!System.IO.Directory.Exists(sourceDir))
            {
                MessageBox.Show(errorMessageDriverQ);
                labelStatus.Text = "Offline";
                buttonLed.BackColor = Color.Red;
            }
            else if (!System.IO.Directory.Exists(destinationDir))
            {
                MessageBox.Show(errorMessageFolder);
            }
            else
            {
                buttonCopy.Text = "Running";
                labelStatus.Text = "Running...Green Light means Copying Logs";
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromSeconds(30);
                timer = new System.Threading.Timer((obj) =>
                {
                    nStatus = CopyFunction();

                }, null, startTimeSpan, periodTimeSpan);

            }

        }
        private int CopyFunction()
        {
            destinationDir = textBoxTo.Text;
            buttonLed.BackColor = Color.Green;
            buttonCopy.BackColor = Color.LightGreen;
            string measCode = textBoxMeas.Text;
            FileInfo fileInfo;
            try
            {
                foreach (string file_name in Directory.GetFiles(sourceDir, "*" + measCode + "*", System.IO.SearchOption.AllDirectories))
                {
                    fileInfo = new FileInfo(file_name);

                    if (!IsFileLocked(fileInfo))
                        File.Copy(file_name, destinationDir + file_name.Substring(sourceDir.Length), true);
                }
                buttonLed.BackColor = Color.Red;
            }
            catch (IOException)
            {
                return 1;
            }
            return 0;
        }

        protected virtual bool IsFileLocked(FileInfo file)
        {
            try
            {
                using (FileStream stream = file.Open(FileMode.Open, FileAccess.Read, FileShare.None))
                {
                    stream.Close();
                }
            }
            catch (IOException)
            {
                return true;
            }
            //file is not locked
            return false;
        }
        private void timerStarting() 
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 60000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            buttonCopy.PerformClick();
        }
        private void button1_Click_1(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                textBoxTo.Text = folderBrowserDialog1.SelectedPath;
            }
        }
    }
}
