using System;

namespace GrandTimerAuto
{
    class WarehouseState
    {
        public const double SAUNITRATE = 0.0001388888889;
        public const double SAPROFITRATE = 2.78;
        public const int SAMAXSECONDS = 72000;

        //public const double SAUNITRATE = .5;
        //public const double SAPROFITRATE = 2.5;
        //public const int SAMAXSECONDS = 20;

        public const double PUNITRATE = 0.0002777777778;
        public const double PPROFITRATE = 2.36;
        public const int PMAXSECONDS = 72000;

        public const double CUNITRATE = 0.0005555555556;
        public const double CPROFITRATE = 1.94;
        public const int CMAXSECONDS = 72000;

        public const double OUNITRATE = 0.0008333333333;
        public const double OPROFITRATE = 1.25;
        public const int OMAXSECONDS = 96000;

        public const double PCUNITRATE = 0.001111111111;
        public const double PCPROFITRATE = 1.11;
        public const int PCMAXSECONDS = 54000;

        public const double CGUNITRATE = 0.0002380952381;
        public const double CGPROFITRATE = 2.38;
        public const int CGMAXSECONDS = 210000;

        public const double SGUNITRATE = 0.0004166666667;
        public const double SGPROFITRATE = 2.08;
        public const int SGMAXSECONDS = 240000;
        public TimeSpan LastCasinoVisit { get; set; }

        public int TotalSASeconds { get; set; }
        public int TotalPSeconds { get; set; }
        public int TotalCSeconds { get; set; }
        public int TotalOSeconds { get; set; }
        public int TotalPCSeconds { get; set; }
        public int TotalCGSeconds { get; set; }
        public int TotalSGSeconds { get; set; }
        public int TotalUnits { get; set; }
        public double TotalValue { get; set; }

        public WarehouseState()
        {
            TotalSASeconds = 0;
        }
    }
}
