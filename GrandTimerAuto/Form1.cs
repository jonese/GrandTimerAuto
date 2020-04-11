using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace GrandTimerAuto
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer _timer;
        private bool _warehouseTimerRunning = false;

        private TimeSpan _FeeDeadline = new TimeSpan(0, 48, 0);
        private DateTime _sessionExpiration;
        private int _sessionSeconds = 0;

        public Form1()
        {
            InitializeComponent();
            dateTimePicker1.Value = (DateTime)Properties.Settings.Default["lastCasinoVisit"];

            ClearForm();
            ServiceGoods(0);

            _sessionExpiration = DateTime.Now + _FeeDeadline;

            // Set up a timer and fire the Tick event every second (1000 ms)
            _timer = new System.Windows.Forms.Timer
            {
                Interval = 1000
            };
            _timer.Tick += new EventHandler(_timer_Tick);
            _timer.Start();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
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

        #region Buttons
        // ========================================
        // Buttons
        // ========================================
        private void btnClearLobby_Click(object sender, EventArgs e)
        {
            string serviceName = "GTA5";
            Process[] processes = Process.GetProcessesByName(serviceName);
            foreach (Process process in processes)
            {
                if (process.ProcessName == serviceName)
                {
                    try
                    {
                        Pause.Suspend(process);
                        Thread.Sleep(10000);
                        Pause.Resume(process);
                    }
                    catch (Exception ex)
                    {
                        string errorHelper = "" + ex.ToString();
                    }
                }
            }
        }
        private void btnResetCasino_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["lastCasinoVisit"] = DateTime.Now;
            Properties.Settings.Default.Save();
            dateTimePicker1.Value = (DateTime)Properties.Settings.Default["lastCasinoVisit"];
        }
        private void btnReset_Click(object sender, EventArgs e)
        {
            _warehouseTimerRunning = false;
            btnStart.Text = "Start";

            Properties.Settings.Default["totalWarehouseSeconds"] = 0;

            for (int i = 1; i < 8; i++)
            {
                string seconds = "seconds" + i.ToString();
                Properties.Settings.Default[seconds] = 0;

                string cbName = "cb" + i.ToString();
                CheckBox ctn = (CheckBox)this.Controls[cbName];
                ctn.Enabled = true;
                ctn.Checked = false;
            }
            Properties.Settings.Default.Save();
            ClearWarehouse();
            ServiceGoods(0);
        }
        private void btnSession_Click(object sender, EventArgs e)
        {
            _sessionExpiration = DateTime.Now + _FeeDeadline;
        }
        private void btnSetCasino_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default["lastCasinoVisit"] = dateTimePicker1.Value;
            Properties.Settings.Default.Save();
        }
        private void btnStart_Click(object sender, EventArgs e)
        {
            // If the timer isn't already running
            if (!_warehouseTimerRunning)
            {
                _warehouseTimerRunning = true;
                btnStart.Text = "Stop";
            }
            else // If the timer is already running
            {
                _warehouseTimerRunning = false;
                btnStart.Text = "Start";
            }
        }
        #endregion

        #region Actions
        // ========================================
        // Actions
        // ========================================

        private void ClearForm()
        {
            lblCasinoTime.Text = "";
            lblSessionTimer.Text = "";
            lblOver48.Text = "";
            ClearWarehouse();
        }
        private void ClearWarehouse()
        {
            lblTotalElapsedTime.Text = "";
            lblTotalValue.Text = "";
            lblVehicle.Text = "";
            lblFees.Text = "";
            lblNet.Text = "";

            for (int i = 1; i < 8; i++)
            {
                string lblUnitsName = "lblUnits" + i.ToString();
                string lblValueName = "lblValue" + i.ToString();
                string lblElapsedTimeName = "lblElapsedTime" + i.ToString();

                Label lblUnits = (Label)this.Controls[lblUnitsName];
                Label lblValue = (Label)this.Controls[lblValueName];
                Label lblElapsedTime = (Label)this.Controls[lblElapsedTimeName];

                lblUnits.Text = "";
                lblValue.Text = "";
                lblElapsedTime.Text = "";
            }
        }
        private void ServiceGoods(int interval)
        {
            int totalUnits = 0;
            double totalValue = 0;

            for (int i = 1; i < 8; i++)
            {
                string propertyName = "Seconds" + i;
                int totalSeconds = (int)Properties.Settings.Default[propertyName];

                string cbName = "cb" + i.ToString();
                CheckBox ctn = (CheckBox)this.Controls[cbName];
                ctn.Checked = (bool)Properties.Settings.Default["checked" + i.ToString()];

                string etcName = "lblETC" + i.ToString();
                Label lblETC = (Label)this.Controls[etcName];

                if (ctn.Checked)
                {
                    totalSeconds += interval;
                    int etc = Constants.MAXSECONDS[i - 1] - totalSeconds;
                    lblETC.Text = (DateTime.Now + (new TimeSpan(0, 0, etc))).ToString();
                }
                else
                {
                    lblETC.Text = "";
                }
                if (totalSeconds > 0)
                {
                    if (totalSeconds > Constants.MAXSECONDS[i - 1])
                    {
                        ctn.Checked = false;
                        ctn.Enabled = false;
                    }
                    else
                    {
                        double units = totalSeconds * Constants.UNITRATE[i - 1];
                        int countingUnits = Convert.ToInt32(Math.Floor(units));
                        double value = totalSeconds * Constants.PROFITRATE[i - 1];

                        string lblUnitsName = "lblUnits" + i.ToString();
                        string lblValueName = "lblValue" + i.ToString();
                        string lblElapsedTimeName = "lblElapsedTime" + i.ToString();
                        string progressBarName = "progressBar" + i.ToString();

                        Label lblUnits = (Label)this.Controls[lblUnitsName];
                        Label lblValue = (Label)this.Controls[lblValueName];
                        Label lblElapsedTime = (Label)this.Controls[lblElapsedTimeName];
                        ProgressBar pb = (ProgressBar)this.Controls[progressBarName];

                        lblUnits.Text = String.Format("{0:.###}", units);
                        lblValue.Text = '$' + value.ToString();
                        lblElapsedTime.Text = new TimeSpan(0, 0, totalSeconds).ToString();
                        Properties.Settings.Default[propertyName] = totalSeconds;
                        Properties.Settings.Default.Save();

                        if (pb.Value < countingUnits)
                        {
                            pb.Value = countingUnits;
                            pb.Value = 0;
                        }
                        totalUnits += countingUnits;
                        totalValue += value;
                    }
                }
            }

            if (totalUnits <= 90)
            {
                lblVehicle.Text = "Speedo";
            }
            else
            {
                if (totalUnits <= 180)
                {
                    lblVehicle.Text = "Mule";
                }
                else
                {
                    lblVehicle.Text = "Pounder";
                }
            }

            progressBarTotal.Value = totalUnits;
            progressBarTotal.Value = 0;

            lblTotalValue.Text = '$' + totalValue.ToString();

            lblTotalElapsedTime.Text = new TimeSpan(0, 0, (int)Properties.Settings.Default["totalWarehouseSeconds"]).ToString();

            int feePeriods = (int)Properties.Settings.Default["totalWarehouseSeconds"] / 2880;
            int fees = feePeriods * (int)Properties.Settings.Default["dailyFees"];
            lblFees.Text = "$ " + fees.ToString();
            lblNet.Text = "$" + (totalValue - fees).ToString();
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        static extern uint SendMessage(IntPtr hWnd, uint Msg, uint wParam, uint lParam);
        #endregion

        #region Tick methods
        // ========================================
        // Tick Methods
        // ========================================
        void _timer_Tick(object sender, EventArgs e)
        {
            Warehouse_Service();
            Session_Service();
            Casino_Service();
        }
        private void Casino_Service()
        {
            DateTime nextCasinoVisit = ((DateTime)Properties.Settings.Default["lastCasinoVisit"]).AddHours(24);
            if (nextCasinoVisit > DateTime.Now)
            {
                TimeSpan timeLeft = (nextCasinoVisit - DateTime.Now);
                TimeSpan remainingTime = new TimeSpan(timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
                lblCasinoTime.Text = remainingTime.ToString();
            }
            else
            {
                lblCasinoTime.Text = "GO NOW";
            }
        }
        private void Session_Service()
        {
            _sessionSeconds++;
            if (DateTime.Compare(DateTime.Now, _sessionExpiration) == 1)
            {
                lblOver48.Text = "Fees Active";
            }
            else
            {
                var freeTimeRemaining = _sessionExpiration - DateTime.Now;
                freeTimeRemaining = new TimeSpan(freeTimeRemaining.Hours,
                                                      freeTimeRemaining.Minutes,
                                                      freeTimeRemaining.Seconds);
                lblOver48.Text = freeTimeRemaining.Minutes.ToString() + ":"
                    + freeTimeRemaining.Seconds.ToString() + " remaining";
            }
            var startTime = _sessionExpiration - _FeeDeadline;
            var elapsedTime = DateTime.Now - (_sessionExpiration - _FeeDeadline);
            elapsedTime = new TimeSpan(elapsedTime.Hours, elapsedTime.Minutes, elapsedTime.Seconds);
            lblSessionTimer.Text = elapsedTime.ToString();
        }
        private void Warehouse_Service()
        {
            int totalSeconds = (int)Properties.Settings.Default["totalWarehouseSeconds"];
            if (_warehouseTimerRunning)
            {
                totalSeconds++;
                Properties.Settings.Default["totalWarehouseSeconds"] = totalSeconds;
                Properties.Settings.Default.Save();
                ServiceGoods(1);
            }
            lblTotalElapsedTime.Text = new TimeSpan(0, 0, totalSeconds).ToString();
        }
        #endregion

        #region cb clicks
        private void cb1_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked1"] = cb1.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb2_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked2"] = cb2.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb3_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked3"] = cb3.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb4_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked4"] = cb4.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb5_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked5"] = cb5.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb6_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked6"] = cb6.Checked;
            Properties.Settings.Default.Save();
        }

        private void cb7_CheckedChanged(object sender, EventArgs e)
        {
            Properties.Settings.Default["checked7"] = cb7.Checked;
            Properties.Settings.Default.Save();
        }
        #endregion
    }
}