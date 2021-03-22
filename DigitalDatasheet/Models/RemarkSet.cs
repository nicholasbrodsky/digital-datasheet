using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Media;

namespace DigitalDatasheet.Models
{
    public class RemarkSet : INotifyPropertyChanged
    {
        private string remark;
        private bool reject = true;
        private int row;
        private Brush backgroundColor = new SolidColorBrush();
        public string Remark
        {
            get { return remark; }
            set { remark = value; OnPropertyChanged(); }
        }
        public bool Reject
        {
            get
            {
                if (((SolidColorBrush)BackgroundColor).Color == Colors.Yellow)
                    reject = true;
                return reject;
            }
            set
            {
                reject = value;
                OnPropertyChanged();
            }
        }
        public int Row
        {
            get { return row; }
            set
            {
                row = value;
                OnPropertyChanged();
                OnPropertyChanged("BackgroundColor");
            }
        }
        public Brush BackgroundColor
        {
            get
            {
                return backgroundColor;
            }
            set
            {
                backgroundColor = value;
                OnPropertyChanged();
                OnPropertyChanged("Reject");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}