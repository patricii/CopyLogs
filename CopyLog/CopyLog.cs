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
            if (!System.IO.Directory.Exists(sourceDir))
            {
                MessageBox.Show("Pasta origem não existe ou não foi possivel mapear o driver de rede Q:");
                labelStatus.Text = "Offline";
                buttonLed.BackColor = Color.Red;
            }
            else
            {
                labelStatus.Text = "Running...Green Light means Copying Logs";
                var startTimeSpan = TimeSpan.Zero;
                var periodTimeSpan = TimeSpan.FromMinutes(0.3);

                var timer = new System.Threading.Timer((p) =>
                {
                    CopyFunction();

                }, null, startTimeSpan, periodTimeSpan);
            }
        }

        private void CopyFunction()
        {
            string destinationDir = textBoxTo.Text;
            buttonLed.BackColor = Color.Green;
            string measCode = textBoxMeas.Text;
            try
            {
                FileInfo fileInfo;
                if (!System.IO.Directory.Exists(destinationDir))
                {
                    MessageBox.Show("Pasta destino não existe!!!");
                }
                else
                {
                    foreach (string file_name in Directory.GetFiles(sourceDir, "*" + measCode + "*", System.IO.SearchOption.AllDirectories))
                    {
                        fileInfo = new FileInfo(file_name);

                        if (!IsFileLocked(fileInfo))
                            File.Copy(file_name, destinationDir + file_name.Substring(sourceDir.Length), true);
                    }
                    buttonLed.BackColor = Color.Red;
                }
            }
            catch (Exception exc)
            {
                MessageBox.Show("!! Erro ao tentar copiar os logs" + exc);
            }
            finally
            {
                timerStarting();
            }

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
        public void timerStarting()
        {
            System.Windows.Forms.Timer timer = new System.Windows.Forms.Timer();
            timer.Interval = 15000;
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
        }
        public void timer_Tick(object sender, EventArgs e)
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
