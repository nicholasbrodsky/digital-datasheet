using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for DataCheckValueEditView.xaml
    /// </summary>
    public partial class DataCheckValueEditView : Window, INotifyPropertyChanged
    {
        private List<decimal?> _initialValues = new List<decimal?>();
        private List<decimal?> _defaultCheckList;
        public List<decimal?> DefaultCheckList
        {
            get { return _defaultCheckList; }
            set { _defaultCheckList = value; OnPropertyChanged(); }
        }

        public DataCheckValueEditView(List<decimal?> defaultCheckList)
        {
            DataContext = this;
            DefaultCheckList = defaultCheckList;
            InitializeComponent();

            foreach (var item in defaultCheckList)
            {
                _initialValues.Add(item);
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < _initialValues.Count; i++)
            {
                DefaultCheckList[i] = _initialValues[i];
            }
            Close();
        }
        private void Window_Closed(object sender, System.EventArgs e)
        {
            for (int i = 0; i < _initialValues.Count; i++)
            {
                DefaultCheckList[i] = _initialValues[i];
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
