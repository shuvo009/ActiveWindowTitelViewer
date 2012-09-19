using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
namespace Active_Window_Titel_Viewer
{
    public class DataProperty  : INotifyPropertyChanged
    {
        public string Time
        {
            get { return this._Time; }
            set { this._Time = value; this.onPropertyChanged("Time"); }
        }
        public string AppName
        {
            get { return this._AppName; }
            set { this._AppName = value; this.onPropertyChanged("AppName"); }
        }

        public string WindowTitle
        {
            get { return this._WindowTitle; }
            set { this._WindowTitle = value; this.onPropertyChanged("WindowTitle"); }
        }

        public string PorcessName
        {
            get { return this._PorcessName; }
            set { this._PorcessName = value; this.onPropertyChanged("PorcessName"); }
        }

        public string FileName
        {
            get { return this._FileName; }
            set { this._FileName = value; this.onPropertyChanged("FileName"); }
        }

        public string Handel
        {
            get { return this._Handel; }
            set { this._Handel = value; this.onPropertyChanged("Handel"); }
        }

        private string _Time;
        private string _AppName;
        private string _WindowTitle;
        private string _PorcessName;
        private string _FileName;
        private string _Handel;

        public event PropertyChangedEventHandler  PropertyChanged;
        private void onPropertyChanged(string propertyName)
        {
            if (PropertyChanged!=null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

    }
}
