using System;
using System.Drawing;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System.IO;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Text;
using System.Net.Configuration;
using System.Net.Http;

namespace WhisperX_ListenerTest
{
    public partial class formMain : Form
    {
        private WaveInEvent waveIn;
        private Timer flashTimer;
        private WaveFileWriter waveFileWriter;
        private int fileCount = 0;
        private TcpClient tcpClient;
        private NetworkStream stream;
        private bool isListening = false;
        private bool isStreaming = false;
        private bool formMainIsClosing = false;
        private float gain = 0.95f;
        private long totalBytesSent = 0;

        public formMain()
        {
            InitializeComponent();
            InitializeNetwork();

            //PopulateAudioInputs(comboInputs);
            InitializeAudioComponents(); // Ensure audio components are initialized

            flashTimer = new Timer
            {
                Interval = 500 // 0.5 seconds
            };
            flashTimer.Tick += FlashTimer_Tick;

            // Move these to outside of InitializeComponent
            cbStream.MouseDown += cbStream_MouseDown;
            cbStream.MouseUp += cbStream_MouseUp;
            cbMomentary.MouseDown += cbMomentary_MouseDown;
            cbMomentary.MouseUp += cbMomentary_MouseUp;

            cbMomentary.FlatStyle = FlatStyle.Flat; // Ensure it visually behaves like a button
            cbStream.FlatStyle = FlatStyle.Flat; // Ensure it visually behaves like a button
        }

        private void InitializeNetwork()
        {

            try
            {
                tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));
                stream = tcpClient.GetStream();
                cbConnected.Checked = tcpClient.Connected;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            if(tcpClient != null && tcpClient.Connected)
            {
                StartReceivingData(tcpClient);
            }

            DataReceived += (sender, e) =>
            {
                // Handle the received data
                // Update the TextBox on the UI thread
                this.Invoke(new Action(() =>
                {
                    string receivedText = Encoding.UTF8.GetString(e.Data);
                    textOutput.Text += receivedText;
                    textOutput.Text += Environment.NewLine;
                }));
            };

        }

        private void InitializeAudioComponents()
        {
            var enumerator = new MMDeviceEnumerator();
            comboInputs.Items.Clear(); // Clear existing items

            // Enumerate only audio input devices
            var captureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
            foreach (var device in captureDevices)
            {
                comboInputs.Items.Add(device.FriendlyName);
            }

            // Set event handler for device selection change
            comboInputs.SelectedIndexChanged += ComboInputs_SelectedIndexChanged;

            // Select a default device if available
            if (comboInputs.Items.Count > 0)
            {
                comboInputs.SelectedIndex = 0; // Select the first device by default
            }

            // After populating the combo box, select the USB2.0 device if present
            int usbDeviceIndex = comboInputs.Items.IndexOf(comboInputs.Items.Cast<string>().FirstOrDefault(item => item.Contains("USB2.0")));
            if (usbDeviceIndex != -1)
            {
                comboInputs.SelectedIndex = usbDeviceIndex;
            }

        }

        private void ComboInputs_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboInputs.SelectedIndex >= 0)
            {
                waveIn = null; // Reset waveIn to null before reinitializing
                waveIn = new WaveInEvent();
                waveIn.DeviceNumber = comboInputs.SelectedIndex;
                waveIn.WaveFormat = new WaveFormat(16000, 16, 1); // 16 kHz, 16 bit, mono
            }
        }

        private void cbStream_MouseDown(object sender, MouseEventArgs e)
        {
            StartStreaming(e);
        }

        private void cbStream_MouseUp(object sender, MouseEventArgs e)
        {
            StopStreaming(e);
        }

        private void StartStreaming(MouseEventArgs e)
        {
            if (waveIn == null)
            {
                MessageBox.Show("Audio input not initialized.");
                return;
            }

            if (e.Button == MouseButtons.Left)
            {
                waveIn.DataAvailable -= WaveIn_DataAvailable; // Remove the previous event handler
                waveIn.DataAvailable += WaveIn_StreamingDataAvailable;

                try
                {
                    // tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));

                    //SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                    //socketAsyncEventArgs.Completed += tcpClientRecieveData();

                    //tcpClient.Client.ReceiveFromAsync(new SocketAsyncEventArgs());

                    //stream = tcpClient.GetStream();
                    //cbConnected.Checked = tcpClient.Connected;
                    isStreaming = true; // Set flag to true when starting
                    cbStream.BackColor = Color.Red;
                    flashTimer.Start();
                    waveIn.StartRecording();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting stream: {ex.Message}");
                }
            }
        }


        private void StopStreaming(MouseEventArgs e)
        {
            if (isStreaming)
            {
                waveIn.StopRecording();

                // CloseConnection();
                stream.Flush();
                SendRemainingSilence();
                //stream.Close();
                //stream = null;

                cbStream.BackColor = default;
                isStreaming = false;

                progressBarAudioLevel.Invoke(new Action(() =>
                {
                    progressBarAudioLevel.Value = 0;
                }));
            }

            // Close the stream and TCP client after ensuring all data has been sent
            // CloseConnection();

        }

        private void SendRemainingSilence()
        {
            const int PACKET_SIZE = 65536;
            long remainingBytes = PACKET_SIZE - (totalBytesSent % PACKET_SIZE);
            if (remainingBytes > 0 && remainingBytes < PACKET_SIZE)
            {
                byte[] silence = new byte[remainingBytes];
                stream.Write(silence, 0, silence.Length);
            }
        }

        private void WaveIn_StreamingDataAvailable(object sender, WaveInEventArgs e)
        {
            if (isStreaming && stream != null && tcpClient.Connected && waveIn != null) // Added null check for waveIn
            {
                stream.Write(e.Buffer, 0, e.BytesRecorded);
                totalBytesSent += e.BytesRecorded;
            }

            //  for "audio meter" functionality
            int max = 0;
            for (int index = 0; index < e.BytesRecorded; index += 2)
            {
                short sample = (short)((e.Buffer[index + 1] << 8) | e.Buffer[index]);
                int absSample = Math.Abs((int)sample);
                int volSAmple = (int)(absSample * gain);  // gain scalilng from mic gain fader
                if (volSAmple > max) max = volSAmple;
            }

            progressBarAudioLevel.Invoke(new Action(() =>
            {
                progressBarAudioLevel.Value = Math.Min(max, progressBarAudioLevel.Maximum);
            }));
        }

        private void StartListening()
        {
            if (isListening || waveIn == null) return; // Check if waveIn is null

            waveIn.DataAvailable -= WaveIn_DataAvailable; // Remove the previous event handler
            waveIn.DataAvailable += WaveIn_StreamingDataAvailable;

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

            if (waveIn == null) return; // Check if waveIn is null before proceeding

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

                // After closing the file, run the WhisperX command asynchronously
                RunWhisperXAsync(fullPath);
            }
        }

        private void FlashTimer_Tick(object sender, EventArgs e)
        {
            // Use isStreaming flag to control flashing
            if (isStreaming)
            {
                cbStream.BackColor = cbStream.BackColor == Color.Red ? default : Color.Red;
            }
            else
            {
                cbStream.BackColor = default;
                //flashTimer.Stop();
            }

            if (isListening)
            {
                cbMomentary.BackColor = cbStream.BackColor == Color.Red ? default : Color.Red;
            }
            else
            {
                cbMomentary.BackColor = default;
                //flashTimer.Stop();
            }
        }

        private void cbMomentary_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left && !isListening)
            {
                StartListening();
                cbMomentary.BackColor = Color.Red; // Start with red to indicate active listening
                flashTimer.Start();
            }
        }

        private void cbMomentary_MouseUp(object sender, MouseEventArgs e)
        {
            if (isListening)
            {
                StopListening();
                cbMomentary.BackColor = default; // Revert back to default color
            }
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
                int volSAmple = (int)(absSample * gain);
                if (volSAmple > max) max = volSAmple;
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
                //waveIn.Dispose();
                //waveIn = null;
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
        private async void RunWhisperXAsync(string filePath)
        {
            await Task.Run(() =>
            {
                string outputDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "WhisperXOutputs");
                Directory.CreateDirectory(outputDirectory); // Ensure the output directory exists

                string command = $"activate whisperx && whisperx \"{filePath}\" --output_dir \"{outputDirectory}\" --model tiny --no_align --output_format txt --language en";
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
                        textOutput.Text += $"Transcription time: {elapsedTimeFormatted}" + Environment.NewLine + Environment.NewLine;
                    }));
                }
            });
        }

        private void btnListen_Click(object sender, EventArgs e)
        {
            StartListening();
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            StopListening();
        }

        private void tbGain_Scroll(object sender, EventArgs e)
        {
            // Calculate the gain factor from the trackbar value
            // Assuming tbGain's value range is 0 to 100 for 0% to 100% volume
            gain = tbGain.Value / 100f;
        }

        private void formMain_FormClosing(object sender, FormClosingEventArgs e)
        {

            formMainIsClosing = true;

            CloseConnection();

        }

        public event EventHandler<DataReceivedEventArgs> DataReceived;

        protected virtual void OnDataReceived(byte[] data)
        {
            DataReceived?.Invoke(this, new DataReceivedEventArgs(data));
        }

        public class DataReceivedEventArgs : EventArgs
        {
            public byte[] Data { get; }

            public DataReceivedEventArgs(byte[] data)
            {
                Data = data;
            }
        }

        private void StartReceivingData(TcpClient tcpClient)
        {
            Task.Run(async () =>
            {
                try
                {
                    NetworkStream stream = tcpClient.GetStream();
                    while (tcpClient.Connected)
                    {
                        if (stream.DataAvailable)
                        {
                            byte[] buffer = new byte[tcpClient.ReceiveBufferSize];
                            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                            byte[] actualData = new byte[bytesRead];
                            Array.Copy(buffer, actualData, bytesRead);
                            OnDataReceived(actualData);
                        }
                        else
                        {
                            await Task.Delay(100); // Wait a bit before checking again to reduce CPU usage
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions (e.g., connection closed)
                    Console.WriteLine($"Error receiving data: {ex.Message}");
                }
            });
        }

        private void cbConnected_CheckedChanged(object sender, EventArgs e)
        {

            if (!cbConnected.Checked)
            {
                CloseConnection();
            }
            else
            {
                OpenTCPConnection();
            }
        }

        private void OpenTCPConnection()
        {
            // cbConnected.Checked = tcpClient.Connected;
            if (tcpClient == null || !tcpClient.Connected)
            {
                try
                {
                    tcpClient = null;
                    tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));
                    // stream = tcpClient.GetStream();
                    cbConnected.Checked = tcpClient.Connected;
                    //tcpClient.GetStream();
                    //isStreaming = true; // Set flag to true when starting
                    //cbStream.BackColor = Color.Red;
                    //flashTimer.Start();
                    //waveIn.StartRecording();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error starting stream: {ex.Message}");
                }
            }
        }
        private void CloseConnection()
        {
            if (stream != null && tcpClient.Connected)
            {
                //stream.Write(endOfTransmissionSignal, 0, endOfTransmissionSignal.Length);
                stream.Flush();
                SendRemainingSilence();
                // Wait for the server to acknowledge
            }

            if (stream != null)
            {
                stream.Close();
                stream = null;
            }
            if (formMainIsClosing && tcpClient != null)
            {
                tcpClient.Close();
                tcpClient = null;
            }

            // if (!formMainIsClosing) { cbConnected.Checked = false; }

            if (formMainIsClosing && waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
        }
    }
}
