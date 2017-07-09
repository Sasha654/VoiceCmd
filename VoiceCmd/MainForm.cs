using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Microsoft.Speech.Recognition;
using System.Diagnostics;
using System.Configuration;
using System.Collections.Specialized;

namespace VoiceCmd
{
    public partial class MainForm : Form
    {
        private NameValueCollection allAppSettings;
        private List<Command> listCommands;
        SpeechRecognitionEngine sre;
        string[] commandTextArray;
        private bool isExit = false;
        private bool isLesten = false;

        public MainForm()
        {
            InitializeComponent();

            this.notifyIcon.Icon = Properties.Resources.voiceOff;
            this.Icon = Properties.Resources.voiceOff;

            InitializedataGridView();
        }

        private void InitializedataGridView()
        {
            dataGridView1.Rows.Clear();
            allAppSettings = ConfigurationManager.AppSettings;

            try
            {
                for (int i = 0; i < allAppSettings.Count; i++)
                {
                    string[] parse = allAppSettings[i].Split('^');
                    dataGridView1.Rows.Add(allAppSettings.Keys[i], parse[0], parse[1]);
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Wrong App.config!", "Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeAction()
        {
            listCommands = new List<Command>()
            {
                 new Command
                {
                    Action = () =>
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd");
                        psi.Arguments = @"/c rundll32.exe user32.dll, LockWorkStation";
                        psi.UseShellExecute = false;
                        psi.RedirectStandardOutput = true;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                },
                new Command
                {
                    Action = () =>
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd");
                        psi.Arguments = @"/c shutdown -l";
                        psi.UseShellExecute = false;
                        psi.RedirectStandardOutput = true;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                },
                new Command
                {
                    Action = () => 
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd");
                        psi.Arguments = @"/c shutdown -r";
                        psi.UseShellExecute = false;
                        psi.RedirectStandardOutput = true;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                },
                new Command
                {
                    Action = () => 
                    {
                        ProcessStartInfo psi = new ProcessStartInfo("cmd");
                        psi.Arguments = @"/c shutdown -s";
                        psi.UseShellExecute = false;
                        psi.RedirectStandardOutput = true;
                        psi.CreateNoWindow = true;
                        Process.Start(psi);
                    }
                }
            };

            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                listCommands[i].CommandText = (string)dataGridView1[1, i].Value;
                listCommands[i].CommandEnable = Convert.ToBoolean(dataGridView1[2, i].Value);
            }
        }

        private void RecognizeRun()
        {
            commandTextArray = new string[allAppSettings.Count];
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                commandTextArray[i] = (string)dataGridView1[1, i].Value ?? "==No command==";

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("ru-ru");
            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-us");      //Необходимо установить English Speech Platform 
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += new EventHandler<SpeechRecognizedEventArgs>(sre_SpeechRecognized);
            Choices numbers = new Choices();
            numbers.Add(commandTextArray);
            GrammarBuilder gb = new GrammarBuilder();
            gb.Culture = ci;
            gb.Append(numbers);
            Grammar g = new Grammar(gb);
            sre.LoadGrammar(g);
            sre.RecognizeAsync(RecognizeMode.Multiple);
        }

        private void RecognizeFinish()
        {
            sre.RecognizeAsyncStop();
        }

        private void sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            if (e.Result.Confidence > 0.6 && isLesten)
            {
                foreach (var item in listCommands)
                {
                    if (item.CommandText == e.Result.Text && item.CommandEnable)
                    {
                        notifyIcon.ShowBalloonTip(1000, null, $"Command text: {item.CommandText}", ToolTipIcon.None);
                        item.Action.Invoke();
                        break;
                    }
                }
            }
        }

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            config.AppSettings.Settings[ConfigurationManager.AppSettings.Keys[e.RowIndex]].Value =
                dataGridView1["Column2", e.RowIndex].Value +
                "^" + dataGridView1["Column3", e.RowIndex].Value;

            config.Save(ConfigurationSaveMode.Modified);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            if (isLesten)       //Finish listening
            {
                isLesten = false;
                btnStart.Text = "Run Recognize";
                this.Icon = Properties.Resources.voiceOff;
                this.notifyIcon.Icon = Properties.Resources.voiceOff;
                dataGridView1.ReadOnly = false;
                pictureBoxRedBlink.Visible = false;
                RecognizeFinish();
            }
            else                //Start listening
            {
                isLesten = true;
                btnStart.Text = "Finish Recognize";
                this.Icon = Properties.Resources.voiceOn;
                this.notifyIcon.Icon = Properties.Resources.voiceOn;
                dataGridView1.ReadOnly = true;
                pictureBoxRedBlink.Visible = true;
                InitializeAction();
                RecognizeRun();
            }
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Minimized)
                this.Hide();
        }

        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.Show();
            WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isExit = true;
            this.Close();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            //if (!isExit)
            //    e.Cancel = true;

            //this.Hide();
        }
    }
}
