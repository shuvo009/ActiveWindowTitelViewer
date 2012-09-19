using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Threading;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using System.Windows.Controls.Primitives;
using Microsoft.Win32;
namespace Active_Window_Titel_Viewer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window ,INotifyPropertyChanged
    {
        #region Wuindows API
        [DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);

        [DllImport("user32.dll")]
        static extern UInt32 GetWindowThreadProcessId(Int32 hWnd, out Int32 lpdwprocessId);
        #endregion
        private ObservableCollection<DataProperty> _ClickHistory;
        private DataProperty _TempDataProperty;
        DispatcherTimer activitiMonitor = new DispatcherTimer();

        private bool minimize = true;
        private IntPtr privousHandel = IntPtr.Zero;
        private string previousWindowTitle = string.Empty;

        /// <summary>
        /// Store Click History and use it to store History in file.
        /// </summary>
        public ObservableCollection<DataProperty> ClickHistory
        {
            get { return this._ClickHistory; }
            set { this._ClickHistory = value;  }
        }
        /// <summary>
        /// this is use for show info on TextBlocks and add to ClickHistory
        /// </summary>
        public DataProperty TempDataProperty
        {
            get { return this._TempDataProperty; }
            set { this._TempDataProperty = value; this.onPropertyChanged("TempDataProperty");
           
            }
        }
        public MainWindow()
        {
            ClickHistory = new ObservableCollection<DataProperty>();
            TempDataProperty = new DataProperty();
            InitializeComponent();
            
           activitiMonitor.Tick += new EventHandler(activitiMonitor_Tick);
           activitiMonitor.Interval = new TimeSpan(0, 0, 0, 0, 100);
           activitiMonitor.Start();
          
        }

        void activitiMonitor_Tick(object sender, EventArgs e)
        {
            this.getActiveWindowInfo();
            this.textBlock1.Text = DateTime.Now.ToString("hh:mm:ss tt");
        }

        /// <summary>
        /// Get Process ID Of A Process
        /// </summary>
        /// <param name="hwnd"></param>
        /// <returns></returns>
        private Int32 getWindowsProccessId(Int32 hwnd)
        {
            Int32 processID = 1;
            GetWindowThreadProcessId(hwnd, out processID);
            return processID;
        }

        private void getActiveWindowInfo()
        {
            Int32 hwnd = 0;
            const int nChars = 256;
            IntPtr handel = IntPtr.Zero;
            StringBuilder buffer = new StringBuilder(nChars);
            handel = GetForegroundWindow();
            hwnd = handel.ToInt32();
            if (GetWindowText(handel,buffer,nChars)>0)
            {
                if (privousHandel != handel || previousWindowTitle!=buffer.ToString())
                {
                    this.TempDataProperty.WindowTitle = buffer.ToString();
                    this.TempDataProperty.Handel = handel.ToString();
                    var processInfo = Process.GetProcessById(this.getWindowsProccessId(hwnd));
                    this.TempDataProperty.PorcessName = processInfo.ProcessName;
                    this.TempDataProperty.FileName = processInfo.MainModule.FileVersionInfo.FileName;
                    this.TempDataProperty.AppName = this.TempDataProperty.FileName.Substring(this.TempDataProperty.FileName.LastIndexOf(@"\") + 1);
                    this.ClickHistory.Add(new DataProperty { WindowTitle = this.TempDataProperty.WindowTitle, AppName = this.TempDataProperty.AppName, FileName = this.TempDataProperty.FileName, PorcessName = this.TempDataProperty.PorcessName, Handel = this.TempDataProperty.Handel, Time = DateTime.Now.ToString("hh:mm:ss tt")});
                    privousHandel = handel;
                    string saveXamlFilePath =System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AtWTVHis", string.Format("{0}.xml", DateTime.Today.ToString("yyyy-MM-dd")));
                    previousWindowTitle = buffer.ToString();
                    if (!File.Exists(saveXamlFilePath))
                    {
                        if (!Directory.Exists(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AtWTVHis")))
                        {
                            Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AtWTVHis"));
                        }
                        { File.Create(saveXamlFilePath).Dispose(); }
                        XDocument fileSetUp = new XDocument(new XElement("Root"));
                        fileSetUp.Save(saveXamlFilePath);
                    }
                    XDocument wrightDoc = XDocument.Load(saveXamlFilePath);
                    wrightDoc.Descendants("Root").Single().Add(new XElement("History", new XAttribute("Time", DateTime.Now.ToString("hh:mm:ss tt")), new XAttribute("AppName", this.TempDataProperty.AppName), new XAttribute("Title", this.TempDataProperty.WindowTitle), new XAttribute("Process_Name", this.TempDataProperty.PorcessName), new XAttribute("File_Path", this.TempDataProperty.FileName), new XAttribute("Handel", this.TempDataProperty.Handel)));
                    wrightDoc.Save(saveXamlFilePath);
                }
                
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void onExport_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("explorer.exe", System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AtWTVHis"));
        }

        private void onExit_Click(object sender, RoutedEventArgs e)
        {
           MessageBoxResult temResult= MessageBox.Show("Are you sure are you want to exit ?", "Active Window Title Viewer", MessageBoxButton.YesNo, MessageBoxImage.Question);
           if (temResult.Equals(MessageBoxResult.Yes))
           {
               this.minimize = false;
                Application.Current.Shutdown();
           }
        }

        private void onClear_Click(object sender, RoutedEventArgs e)
        {
            this.ClickHistory.Clear();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (minimize)
            {
                this.WindowState = WindowState.Minimized;
                this.ShowInTaskbar = false;
                NotifyWindow ballon = new NotifyWindow();
                this.notifyInSysTry.ShowCustomBalloon(ballon, PopupAnimation.Slide, 4000);
            }
            else
            {
                notifyInSysTry.Dispose();
                base.OnClosing(e);  
            }
            e.Cancel = minimize = true;

        }

        private void onRestore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            this.ShowInTaskbar = true;
        }

        private void window_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (Environment.OSVersion.Version.Major > 5)
            {
                using (MemoryStream iconStream = new MemoryStream())
                {
                    //Icon icon = new Icon("/Images/ActIcon.ico");
                    //   icon.Save(iconStream);
                    //     iconStream.Seek(0, SeekOrigin.Begin);
                    //    this.Icon = System.Windows.Media.Imaging.BitmapFrame.Create(iconStream);
                }
            }
        }

        private void stratWitStrat_Checked(object sender, RoutedEventArgs e)
        {
            Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true).SetValue("AcWiTiVi", System.Reflection.Assembly.GetEntryAssembly().Location, RegistryValueKind.String);
        }

        private void stratWitStrat_Unchecked(object sender, RoutedEventArgs e)
        {
            try
            {
                using (RegistryKey regKey =Registry.CurrentUser.OpenSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run", true))
                {
                    if (regKey!=null)
                    {
                        regKey.DeleteValue("AcWiTiVi");
                    }
                }
            }
            catch
            {
                
            }
        }
    }
}
