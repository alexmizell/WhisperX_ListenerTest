using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Diagnostics;
using System.Linq;

namespace WhisperX_ListenerTest
{
    public partial class formMain : Form
    {
        private WaveInEvent waveIn;
        private Timer flashTimer;
        private bool isListening = false;
        private WaveFileWriter waveFileWriter;
        private int fileCount = 0;

        public formMain()
        {
            InitializeComponent();

            PopulateAudioInputs(comboInputs);

            flashTimer = new Timer
            {
                Interval = 500 // 0.5 seconds
            };
            flashTimer.Tick += FlashTimer_Tick;

            cbMomentary.Appearance = Appearance.Button;
            cbMomentary.MouseDown += cbMomentary_MouseDown;
            cbMomentary.MouseUp += cbMomentary_MouseUp;
            cbMomentary.FlatStyle = FlatStyle.Flat; // Ensure it visually behaves like a button
        }

        private void StartListening()
        {
            if (isListening) return;

            waveIn = new WaveInEvent
            {
                DeviceNumber = comboInputs.SelectedIndex,
                WaveFormat = new WaveFormat(16000, 16, 1)
            };
            waveIn.DataAvailable += WaveIn_DataAvailable;
            waveIn.RecordingStopped += WaveIn_RecordingStopped;
            waveIn.StartRecording();

            // Generate a new file name and create a WaveFileWriter instance
            string fileName = GenerateFileName();
            waveFileWriter = new WaveFileWriter(fileName, waveIn.WaveFormat);

            isListening = true;
            flashTimer.Start();
        }
        private void StopListening()
        {
            waveIn?.StopRecording();
            isListening = false;
            flashTimer.Stop();
            cbMomentary.BackColor = default;

            // Finalize and close the WaveFileWriter
            if (waveFileWriter != null)
            {
                string fullPath = waveFileWriter.Filename;
                waveFileWriter.Dispose();
                waveFileWriter = null;

                // After closing the file, run the WhisperX command
                RunWhisperX(fullPath);
            }
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            cbMomentary.BackColor = cbMomentary.BackColor == Color.Red ? default : Color.Red;
        }

        private void cbMomentary_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                StartListening();
            }
        }

        private void cbMomentary_MouseUp(object sender, MouseEventArgs e)
        {
            StopListening();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Space)
            {
                if (!isListening)
                {
                    StartListening();
                }
                else
                {
                    StopListening();
                }
                return true; // Indicate that the key press has been handled
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        public void PopulateAudioInputs(ComboBox comboInputs)
        {
            var enumerator = new MMDeviceEnumerator();
            int selectedIndex = 0;

            foreach (var endpoint in enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active))
            {
                comboInputs.Items.Add(endpoint.FriendlyName);
                if (endpoint.FriendlyName.IndexOf("USB2.0", StringComparison.OrdinalIgnoreCase) >= 0 && selectedIndex == 0)
                {
                    selectedIndex = comboInputs.Items.Count - 1;
                }
            }

            if (comboInputs.Items.Count > 0)
            {
                comboInputs.SelectedIndex = selectedIndex;
            }
        }

        private void WaveIn_DataAvailable(object sender, WaveInEventArgs e)
        {
            if (waveFileWriter != null)
            {
                waveFileWriter.Write(e.Buffer, 0, e.BytesRecorded);
                waveFileWriter.Flush();
            }

            int max = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                int absSample = Math.Abs((int)sample);
                if (absSample > max) max = absSample;
            }

            progressBarAudioLevel.Invoke(new Action(() =>
            {
                progressBarAudioLevel.Value = Math.Min(max, progressBarAudioLevel.Maximum);
            }));
        }

        private void WaveIn_RecordingStopped(object sender, StoppedEventArgs e)
        {
            if (waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
                progressBarAudioLevel.Invoke(new Action(() =>
                {
                    progressBarAudioLevel.Value = 0;
                }));
            }
        }
        private string GenerateFileName()
        {
            string directory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Recordings");
            Directory.CreateDirectory(directory); // Ensure the directory exists

            string fileName;
            do
            {
                fileName = Path.Combine(directory, $"Recording_{++fileCount}.wav");
            } while (File.Exists(fileName));

            return fileName;
        }
        private void RunWhisperX(string filePath)
        {
            string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WhisperXOutputs");
            Directory.CreateDirectory(outputDirectory); // Ensure the output directory exists

            string command = $"activate whisperx && whisperx \"{filePath}\" --output_dir \"{outputDirectory}\" --model tiny --compute_type float16 --no_align --output_format txt --language en";
            ProcessStartInfo startInfo = new ProcessStartInfo()
            {
                FileName = "cmd.exe",
                Arguments = $"/C {command}",
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new Process()
            {
                StartInfo = startInfo
            };

            // Start timing
            Stopwatch stopwatch = Stopwatch.StartNew();

            process.Start();
            process.WaitForExit();

            // Stop timing
            stopwatch.Stop();

            // After the process completes, find the latest text file in the output directory
            var outputFile = new DirectoryInfo(outputDirectory)
                .GetFiles("*.txt")
                .OrderByDescending(f => f.LastWriteTime)
                .FirstOrDefault();

            if (outputFile != null)
            {
                // Read the contents of the latest file
                string textOutputFile = File.ReadAllText(outputFile.FullName);

                // Calculate the elapsed time and format it
                TimeSpan elapsedTime = stopwatch.Elapsed;
                string elapsedTimeFormatted = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                    elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds,
                    elapsedTime.Milliseconds / 10);

                // Update the TextBox on the UI thread
                this.Invoke(new Action(() =>
                {
                    textOutput.Text += textOutputFile + Environment.NewLine;
                    textOutput.Text += $"Transcription Time: {elapsedTimeFormatted}" + Environment.NewLine + Environment.NewLine;
                }));
            }
        }
    }
}
