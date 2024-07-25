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
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

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
        private bool isReceiving = false;
        private bool formMainIsClosing = false;
        private float gain = 0.95f;
        private long totalBytesSent = 0;

        public formMain()
        {
            InitializeComponent();
            InitializeNetwork();
            InitializeAudioComponents(); 

            flashTimer = new Timer
            {
                Interval = 500 // 0.5 seconds
            };
            flashTimer.Tick += FlashTimer_Tick;

            cbStream.MouseDown += cbStream_MouseDown;
            cbStream.MouseUp += cbStream_MouseUp;
            cbMomentary.MouseDown += cbMomentary_MouseDown;
            cbMomentary.MouseUp += cbMomentary_MouseUp;
            cbLatch.CheckedChanged += cbLatch_CheckedChanged;
            cbStream.Click += cbStream_Click; // Handle Click for latching logic
            

            cbMomentary.FlatStyle = FlatStyle.Flat; // Ensure it visually behaves like a button
            cbStream.FlatStyle = FlatStyle.Flat; // Ensure it visually behaves like a button
        }

        private void InitializeNetwork()
        {

            try
            {
                tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));
                cbConnected.Checked = tcpClient.Connected;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            GetStream();

            if (tcpClient != null && tcpClient.Connected && !isReceiving)
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

                    receivedText = receivedText.Replace(".", "." + Environment.NewLine);
                    receivedText = receivedText.Replace("!", "." + Environment.NewLine);
                    receivedText = receivedText.Replace("?", "." + Environment.NewLine);

                    // textOutput.Text += Environment.NewLine;
                    textOutput.Text += receivedText + " ";
                }));
            };

        }

        private void GetStream()
        {
            try
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    stream = tcpClient.GetStream();
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
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
        private void cbLatch_CheckedChanged(object sender, EventArgs e)
        {
            // If cbLatch is unchecked and cbStream is checked, revert cbStream to unchecked
            if (!cbLatch.Checked && cbStream.Checked)
            {
                cbStream.Checked = false;
                MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
                StopStreaming(mouseEventArgs); // Ensure streaming is stopped when switching back to momentary behavior
            }
        }

        private void cbStream_MouseDown(object sender, MouseEventArgs e)
        {
            // Check if the left mouse button is pressed
            if (e.Button == MouseButtons.Left)
            {
                // If cbLatch is not checked, start streaming immediately on mouse down
                if (!cbLatch.Checked)
                {
                    cbStream.Checked = true; // Momentarily check cbStream
                    StartStreaming(e);
                }
                // If cbLatch is checked, we toggle the state in cbStream_Click instead
            }
        }

        private void cbStream_MouseUp(object sender, MouseEventArgs e)
        {
            // Check if the left mouse button was released
            if (e.Button == MouseButtons.Left)
            {
                // If cbLatch is not checked, stop streaming and uncheck cbStream on mouse up
                if (!cbLatch.Checked)
                {
                    StopStreaming(e);
                    cbStream.Checked = false; // Uncheck cbStream after streaming stops
                }
                // If cbLatch is checked, do nothing here as the toggle is handled in cbStream_Click
            }
        }

        private void cbStream_Click(object sender, EventArgs e)
        {
            // This event is triggered after the checkbox's checked state has been toggled
            if (cbLatch.Checked)
            {
                // Latching behavior: Toggle streaming based on the cbStream.Checked state
                if (cbStream.Checked)
                {
                    MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
                    StartStreaming(mouseEventArgs);
                }
                else
                {
                    MouseEventArgs mouseEventArgs = new MouseEventArgs(MouseButtons.Left, 0, 0, 0, 0);
                    StopStreaming(mouseEventArgs);
                }
            }
            // If cbLatch is not checked, the streaming start/stop is handled in MouseDown/Up events
        }

        private void StartStreaming(MouseEventArgs e)
        {
            //if (waveIn == null)
            //{
            //    MessageBox.Show("Audio input not initialized.");
            //    return;
            //}

            if (e.Button == MouseButtons.Left)
            {
                //waveIn.DataAvailable -= WaveIn_DataAvailable; // Remove the previous event handler

                try
                {
                    // tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));
                    //SocketAsyncEventArgs socketAsyncEventArgs = new SocketAsyncEventArgs();
                    //socketAsyncEventArgs.Completed += tcpClientRecieveData();
                    //tcpClient.Client.ReceiveFromAsync(new SocketAsyncEventArgs());
                    //stream = tcpClient.GetStream();
                    //cbConnected.Checked = tcpClient.Connected;

                    if (comboInputs.SelectedIndex >= 0)
                    {
                        waveIn?.Dispose();
                        waveIn = null; // Reset waveIn to null before reinitializing
                        waveIn = new WaveInEvent();
                        waveIn.DeviceNumber = comboInputs.SelectedIndex;
                        waveIn.WaveFormat = new WaveFormat(16000, 16, 1); // 16 kHz, 16 bit, mono
                        waveIn.DataAvailable += WaveIn_StreamingDataAvailable;
                        waveIn.StartRecording();
                        isStreaming = true; // Set flag to true when starting
                        cbStream.BackColor = Color.Red;
                        flashTimer.Start();
                    }
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

                // stream.Flush();
                SendRemainingSilence();
                waveIn?.Dispose();

                waveIn = null; // Reset waveIn to null before reinitializing
                //stream.Close();
                //stream = null;

                cbStream.BackColor = default;
                isStreaming = false;

                progressBarAudioLevel.Invoke(new Action(() =>
                {
                    progressBarAudioLevel.Value = 0;
                }));
            }
        }

        private void SendRemainingSilence()
        {
            const int PACKET_SIZE = 65536;
            long remainingBytes = PACKET_SIZE - (totalBytesSent % PACKET_SIZE);
            if (remainingBytes > 0 && remainingBytes < PACKET_SIZE)
            {
                byte[] silence = new byte[remainingBytes];
                try
                {
                    stream.Write(silence, 0, silence.Length);
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
        }

        private void WaveIn_StreamingDataAvailable(object sender, WaveInEventArgs e)
        {

            if(stream == null) { GetStream(); }

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
            if (isListening) return;

            if (comboInputs.SelectedIndex >= 0)
            {
                waveIn?.Dispose();
                waveIn = null; // Reset waveIn to null before reinitializing
                waveIn = new WaveInEvent();

                waveIn.DataAvailable += WaveIn_DataAvailable;
                waveIn.RecordingStopped += WaveIn_RecordingStopped;

                waveIn.DeviceNumber = comboInputs.SelectedIndex;
                waveIn.WaveFormat = new WaveFormat(16000, 16, 1); // 16 kHz, 16 bit, mono
                waveIn.StartRecording();

                cbMomentary.BackColor = Color.Red; // Start with red to indicate active listening
                flashTimer.Start();
            }

            // Generate a new file name and create a WaveFileWriter instance
            string fileName = GenerateFileName();
            waveFileWriter = new WaveFileWriter(fileName, waveIn.WaveFormat);

            isListening = true;
            flashTimer.Start();
        }
        private void StopListening()
        {
            if (waveIn == null) return; // Check if waveIn is null before proceeding

            waveIn.StopRecording();
            // waveIn.Dispose();

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
            }
        }

        private void cbMomentary_MouseUp(object sender, MouseEventArgs e)
        {
            if (isListening)
            {
                StopListening();
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
                waveIn.Dispose();
                waveIn = null;
            }

            progressBarAudioLevel.Invoke(new Action(() =>
            {
                progressBarAudioLevel.Value = 0;
            }));
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

            isReceiving = true;
        }

        private void cbConnected_CheckedChanged(object sender, EventArgs e)
        {

            if (!cbConnected.Checked)
            {
                CloseConnection();
            }
            else
            {
                //OpenTCPConnection();
                InitializeNetwork();
            }
        }

        //private void OpenTCPConnection()
        //{
        //    // cbConnected.Checked = tcpClient.Connected;
        //    if (tcpClient == null || !tcpClient.Connected)
        //    {
        //        try
        //        {
        //            tcpClient = null;
        //            tcpClient = new TcpClient(textHost.Text, int.Parse(textPort.Text));
        //            // stream = tcpClient.GetStream();
        //            cbConnected.Checked = tcpClient.Connected;
        //            //tcpClient.GetStream();
        //            //isStreaming = true; // Set flag to true when starting
        //            //cbStream.BackColor = Color.Red;
        //            //flashTimer.Start();
        //            //waveIn.StartRecording();
        //        }
        //        catch (Exception ex)
        //        {
        //            MessageBox.Show($"Error starting stream: {ex.Message}");
        //        }
        //    }
        //}
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
                cbConnected.Checked = false;
            }

            if (formMainIsClosing && waveIn != null)
            {
                waveIn.Dispose();
                waveIn = null;
            }
        }

        private void textOutput_TextChanged(object sender, EventArgs e)
        {
            textOutput.SelectionStart = textOutput.Text.Length;
            textOutput.ScrollToCaret();
        }
    }
}
