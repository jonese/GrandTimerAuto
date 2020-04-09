using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace GrandTimerAuto
{
    public partial class Form1 : Form
    {
        private const string _serviceName = "GTA";
        private System.Windows.Forms.Timer _timer;
        private bool _timerRunning = false;

        private DateTime _warehouseStartTime = DateTime.MinValue;
        private DateTime _sessionStartTime = DateTime.MinValue;

        private TimeSpan _currentElapsedTime = TimeSpan.Zero;
        private TimeSpan _totalElapsedTime = TimeSpan.Zero;

        private TimeSpan _currentSessionTime = TimeSpan.Zero;
        private TimeSpan _totalSessionTime = TimeSpan.Zero;

        private TimeSpan _FeeDeadline = new TimeSpan(0, 48, 0);


        private WarehouseState ws = new WarehouseState();

        // Goods
        private DateTime _currentSouthAmericanStartTime = DateTime.MinValue;
        private DateTime _totalSouthAmericanTime = DateTime.MinValue;

        public Form1()
        {
            InitializeComponent();

            // Set up a timer and fire the Tick event every second (1000 ms)
            _timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _timer.Tick += new EventHandler(_timer_Tick);
        }

        void _timer_Tick(object sender, EventArgs e)
        {
            // We do this to 'chop off' any stray milliseconds
            // resulting from the Timer's inherent inaccuracy,
            // with the bonus that the TimeSpan.ToString() method
            // will now show the correct HH:MM:SS format
            var timeSinceStartTime = DateTime.Now - _warehouseStartTime;
            timeSinceStartTime = new TimeSpan(timeSinceStartTime.Hours,
                                              timeSinceStartTime.Minutes,
                                              timeSinceStartTime.Seconds);

            var timeSinceNewSession = DateTime.Now - _sessionStartTime;
            timeSinceNewSession = new TimeSpan(timeSinceNewSession.Hours,
                                              timeSinceNewSession.Minutes,
                                              timeSinceNewSession.Seconds);

            // The current elapsed time is the time since the start button
            // was clicked, plus the total time elapsed since the last reset
            _currentElapsedTime = timeSinceStartTime + _totalElapsedTime;
            _currentSessionTime = timeSinceNewSession + _totalSessionTime;

            // These are just two Label controls which display the current
            // elapsed time and total elapsed time
            lblTotalElapsedTime.Text = _currentElapsedTime.ToString();
            lblSessionTimer.Text = _currentSessionTime.ToString();

            ServiceGoods(1);
            UpdateStatuses();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // TODO implement load from file logic
            // TODO setup goods display

            SendMessage(progressBar1.Handle,
                0x400 + 16, //WM_USER + PBM_SETSTATE
                0x0003, //PBST_PAUSED
                0);

            SendMessage(progressBar2.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBar3.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBar4.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBar5.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBar6.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBar7.Handle,
              0x400 + 16, //WM_USER + PBM_SETSTATE
              0x0003, //PBST_PAUSED
              0);

            SendMessage(progressBarTotal.Handle,
            0x400 + 16, //WM_USER + PBM_SETSTATE
            0x0003, //PBST_PAUSED
            0);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            // If the timer isn't already running
            if (!_timerRunning)
            {
                // Set the start time to Now
                _warehouseStartTime = DateTime.Now;
                _sessionStartTime = DateTime.Now;

                // Store the total elapsed time so far
                _totalElapsedTime = _currentElapsedTime;
                _totalSessionTime = _currentSessionTime;

                _timer.Start();
                _timerRunning = true;
                btnStart.Text = "Stop";
            }
            else // If the timer is already running
            {
                _timer.Stop();
                _timerRunning = false;
                btnStart.Text = "Start";
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            // Stop and reset the timer if it was running
            _timer.Stop();
            _timerRunning = false;

            // Reset the elapsed time TimeSpan objects
            _totalElapsedTime = TimeSpan.Zero;
            _totalSessionTime = TimeSpan.Zero;
            _currentElapsedTime = TimeSpan.Zero;
            _currentSessionTime = TimeSpan.Zero;

            lblSessionTimer.Text = "";
            lblTotalElapsedTime.Text = "";
            lblVehicle.Text = "";
            lblOver48.Text = "";

            btnStart.Text = "Start";

            //TODO Reset goods
        }

        private void btnSession_Click(object sender, EventArgs e)
        {
            _sessionStartTime = DateTime.Now;
            _currentSessionTime = TimeSpan.Zero;
            _totalSessionTime = TimeSpan.Zero;
            lblSessionTimer.Text = "";
        }

        private void ServiceGoods(int interval)
        {
            int totalUnits = 0;
            double totalValue = 0;

            if (cbSouthAmerican.Checked)
            {
                ws.TotalSASeconds += interval;
                if (ws.TotalSASeconds > WarehouseState.SAMAXSECONDS)
                {
                    cbSouthAmerican.Checked = false;
                    cbSouthAmerican.Enabled = false;
                }
                else
                {
                    double units = ws.TotalSASeconds * WarehouseState.SAUNITRATE;
                    //int countingUnits = Convert.ToInt32(units);
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits1.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalSASeconds * WarehouseState.SAPROFITRATE;
                    lblValue1.Text = '$' + value.ToString();

                    lblElaspedTime1.Text = new TimeSpan(0, 0, ws.TotalSASeconds).ToString();

                    if (progressBar1.Value < countingUnits)
                    {
                        progressBar1.Value = countingUnits;
                        //progressBar2.PerformStep();
                    }
                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbPham.Checked)
            {
                ws.TotalPSeconds += interval;
                if (ws.TotalPSeconds > WarehouseState.PMAXSECONDS)
                {
                    cbPham.Checked = false;
                    cbPham.Enabled = false;
                }
                else
                {
                    double units = ws.TotalPSeconds * WarehouseState.PUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits2.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalPSeconds * WarehouseState.PPROFITRATE;
                    lblValue2.Text = '$' + value.ToString();

                    lblElaspedTime2.Text = new TimeSpan(0, 0, ws.TotalPSeconds).ToString();
                    progressBar2.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbCash.Checked)
            {
                ws.TotalCSeconds += interval;
                if (ws.TotalCSeconds > WarehouseState.CMAXSECONDS)
                {
                    cbCash.Checked = false;
                    cbCash.Enabled = false;
                }
                else
                {
                    double units = ws.TotalCSeconds * WarehouseState.CUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits3.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalCSeconds * WarehouseState.CPROFITRATE;
                    lblValue3.Text = '$' + value.ToString();

                    lblElaspedTime3.Text = new TimeSpan(0, 0, ws.TotalCSeconds).ToString();
                    progressBar3.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbOrganic.Checked)
            {
                ws.TotalOSeconds += interval;
                if (ws.TotalOSeconds > WarehouseState.OMAXSECONDS)
                {
                    cbOrganic.Checked = false;
                    cbOrganic.Enabled = false;
                }
                else
                {
                    double units = ws.TotalOSeconds * WarehouseState.OUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits4.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalOSeconds * WarehouseState.OPROFITRATE;
                    lblValue4.Text = '$' + value.ToString();

                    lblElaspedTime4.Text = new TimeSpan(0, 0, ws.TotalOSeconds).ToString();
                    progressBar4.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbPrinting.Checked)
            {
                ws.TotalPCSeconds += interval;
                if (ws.TotalPCSeconds > WarehouseState.PCMAXSECONDS)
                {
                    cbPrinting.Checked = false;
                    cbPrinting.Enabled = false;
                }
                else
                {
                    double units = ws.TotalPCSeconds * WarehouseState.PCUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits5.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalPCSeconds * WarehouseState.PCPROFITRATE;
                    lblValue5.Text = '$' + value.ToString();

                    lblElaspedTime5.Text = new TimeSpan(0, 0, ws.TotalPCSeconds).ToString();
                    progressBar5.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbCargo.Checked)
            {
                ws.TotalCGSeconds += interval;
                if (ws.TotalCGSeconds > WarehouseState.CGMAXSECONDS)
                {
                    cbCargo.Checked = false;
                    cbCargo.Enabled = false;
                }
                else
                {
                    double units = ws.TotalCGSeconds * WarehouseState.CGUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits6.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalCGSeconds * WarehouseState.CGPROFITRATE;
                    lblValue6.Text = '$' + value.ToString();

                    lblElaspedTime6.Text = new TimeSpan(0, 0, ws.TotalCGSeconds).ToString();
                    progressBar6.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }

            if (cbSporting.Checked)
            {
                ws.TotalSGSeconds += interval;
                if (ws.TotalSGSeconds > WarehouseState.SGMAXSECONDS)
                {
                    cbSporting.Checked = false;
                    cbSporting.Enabled = false;
                }
                else
                {
                    double units = ws.TotalSGSeconds * WarehouseState.SGUNITRATE;
                    int countingUnits = Convert.ToInt32(Math.Floor(units));
                    lblUnits7.Text = String.Format("{0:.###}", units);

                    double value = ws.TotalSGSeconds * WarehouseState.SGPROFITRATE;
                    lblValue7.Text = '$' + value.ToString();

                    lblElaspedTime7.Text = new TimeSpan(0, 0, ws.TotalSGSeconds).ToString();
                    progressBar7.Value = countingUnits;

                    totalUnits += countingUnits;
                    totalValue += value;
                }
            }
            ws.TotalUnits = totalUnits;
            ws.TotalValue = totalValue;
        }
        private void UpdateStatuses()
        {
            if (ws.TotalUnits <= 90)
            {
                lblVehicle.Text = "Speedo";
            }
            else
            {
                if (ws.TotalUnits <= 180)
                {
                    lblVehicle.Text = "Mule";
                }
                else
                {
                    lblVehicle.Text = "Pounder";
                }
            }

            if (TimeSpan.Compare(_currentSessionTime, _FeeDeadline) == 1)
            {
                lblOver48.Text = "Fees Active";
            }
            else
            {
                var freeTimeRemaining = _FeeDeadline - _currentSessionTime;
                freeTimeRemaining = new TimeSpan(freeTimeRemaining.Hours,
                                                  freeTimeRemaining.Minutes,
                                                  freeTimeRemaining.Seconds);
                lblOver48.Text = freeTimeRemaining.Minutes.ToString() + ":"
                    + freeTimeRemaining.Seconds.ToString() + " remaining";
            }
            lblTotalValue.Text = '$' + ws.TotalValue.ToString();
            progressBarTotal.Value = ws.TotalUnits;
        }

        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void btClearLobby_Click(object sender, EventArgs e)
        {
            Process[] processes = Process.GetProcessesByName(_serviceName);
            foreach (Process process in processes)
            {
                if (process.ProcessName == _serviceName)
                {
                    try
                    {
                        Pause.Suspend(process);
                        Thread.Sleep(10000);
                        Pause.Resume(process);
                        ServiceGoods(10);
                    }
                    catch (Exception ex)
                    {
                        string errorHelper = "" + ex.ToString();
                    }
                }
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
    }
}