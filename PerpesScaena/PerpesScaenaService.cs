using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Timers;

namespace PerpesScaena
{
    public partial class PerpesScaenaService : ServiceBase
    {
        private readonly System.Diagnostics.EventLog _eventLogger;

        private const int RefreshRate = 60*60*1000;

        [DllImport("user32.dll")]
        public static extern int SystemParametersInfo(
               uint action, uint uParam, string vParam, uint winIni);

        public static readonly uint SpiSetdeskwallpaper = 0x14;
        public static readonly uint SpifUpdateinifile = 0x01;
        public static readonly uint SpifSendwininichange = 0x02;

        public PerpesScaenaService()
        {
            InitializeComponent();
            _eventLogger = new System.Diagnostics.EventLog();
            if (!System.Diagnostics.EventLog.SourceExists("PerpesScaenaSource"))
            {
                System.Diagnostics.EventLog.CreateEventSource(
                    "PerpesScaenaSource", "PerpesScaenaLog");
            }
            _eventLogger.Source = "PerpesScaenaSource";
            _eventLogger.Log = "PerpesScaenaLog";
        }

        protected override void OnStart(string[] args)
        {
            _eventLogger.WriteEntry("Starting up.");
            var timer = new Timer {Interval = RefreshRate};
            timer.Elapsed += OnTimer;
            timer.Start();
            OnTimer(null, null);
        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            _eventLogger.WriteEntry("Changing background image.");
            SystemParametersInfo(SpiSetdeskwallpaper, 0, @"C:/image.bmp",
                    SpifUpdateinifile | SpifSendwininichange);
        }

        protected override void OnStop()
        {
        }
    }
}
