using DigitalDatasheet.Data;
using DigitalDatasheetContextLib;
using DigitalDatasheetEntityLib;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for OpenJobView.xaml
    /// </summary>
    public partial class OpenJobView : Window, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private OpenJobForm openJobData;
        public OpenJobForm OpenJobData
        {
            get { return openJobData; }
            set { openJobData = value; OnPropertyChanged(); }
        }
        public bool WoCheck { get; set; } = false;
        public bool OkClick { get; set; } = false;
        public bool CancelClick { get; set; } = false;
        public OpenJobView(string winOption="")
        {
            DataContext = this;
            InitializeComponent();
            wo_number_input.Focus();

            OpenJobData = new OpenJobForm();

            if (winOption == "this") Title += " - Current Window";
            else if (winOption == "new") Title += " - New Window";
            //else Title = "Open Job";
        }
        private void Check_Work_Order(object sender, KeyEventArgs e)
        {
            if (string.IsNullOrEmpty(OpenJobData.WorkOrderNo))
            {
                WoCheck = false;
                wo_check_icon.Visibility = Visibility.Hidden;
                return;
            }
            Regex regex = new Regex(@"^[0-9]*$");
            if (!regex.IsMatch(OpenJobData.WorkOrderNo) || !string.IsNullOrEmpty(OpenJobData.WorkOrderNoDash) && !regex.IsMatch(OpenJobData.WorkOrderNoDash))
            {
                WoCheck = false;
                wo_check_icon.Source = new BitmapImage(new Uri("../Images/error_icon.png", UriKind.Relative));
                wo_check_icon.Visibility = Visibility.Visible;
                return;
            }

            CheckJobForm();
        }
        private void Condition_Input_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CheckJobForm();
        }
        private void Testing_Performed_On_Checked(object sender, RoutedEventArgs e)
        {
            CheckJobForm();
        }
        private void CheckJobForm()
        {
            using (var db = new DigitalDatasheetContext())
            {
                JobForm jobForm = db.JobForms.Find(OpenJobData.FullWorkOrder, OpenJobData.TestCondition, OpenJobData.TestPerformedOn);
                if (jobForm == null)
                {
                    WoCheck = false;
                    wo_check_icon.Source = new BitmapImage(new Uri("../Images/error_icon.png", UriKind.Relative));
                }
                else
                {
                    WoCheck = true;
                    wo_check_icon.Source = new BitmapImage(new Uri("../Images/success_icon.png", UriKind.Relative));
                }
            }
            if (wo_check_icon.Visibility == Visibility.Hidden)
                wo_check_icon.Visibility = Visibility.Visible;
        }
        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!WoCheck)
            {
                MessageBox.Show("You must enter a valid input combination for the \"Work Order Number\", \"Test Condition\", and \"Testing Performed On\" or Cancel", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            OkClick = true;
            CancelClick = false;
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OkClick = false;
            Close();
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            if (!OkClick)
                CancelClick = true;
        }
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class OpenJobForm : INotifyPropertyChanged
    {
        private string workOrderNo;
        private string workOrderNoDash;
        private string fullWorkOrder;
        private string testCondition;
        private ComboBoxItem testConditionSelection = new ComboBoxItem();
        private int coupons, bareBoards, customerMounts, assembledBoards, class2Assessment;
        private string testPerformedOn;

        public string WorkOrderNo { get => workOrderNo; set { workOrderNo = value; OnPropertyChanged(); OnPropertyChanged("FullWorkOrder"); } }
        public string WorkOrderNoDash { get => workOrderNoDash; set { workOrderNoDash = value; OnPropertyChanged(); OnPropertyChanged("FullWorkOrder"); } }
        public string FullWorkOrder
        {
            get { return fullWorkOrder = string.IsNullOrEmpty(WorkOrderNoDash) ? $"{WorkOrderNo}" : $"{WorkOrderNo}-{WorkOrderNoDash}"; }
            set { fullWorkOrder = value; OnPropertyChanged(); }
        }
        public ComboBoxItem TestConditionSelection
        {
            get { return testConditionSelection; }
            set { testConditionSelection = value; OnPropertyChanged(); OnPropertyChanged("TestCondition"); }
        }
        public string TestCondition
        {
            get
            {
                if (testConditionSelection.HasContent)
                    return testConditionSelection.Content.ToString();
                else
                    return string.Empty;
            }
            set { testCondition = value; OnPropertyChanged(); }
        }
        public int Coupons
        {
            get { return coupons; }
            set { coupons = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); }
        }
        public int BareBoards
        {
            get { return bareBoards; }
            set { bareBoards = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); }
        }
        public int CustomerMounts
        {
            get { return customerMounts; }
            set { customerMounts = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); }
        }
        public int? CustomerMountQty { get; set; }
        public int AssembledBoards
        {
            get { return assembledBoards; }
            set { assembledBoards = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); }
        }
        public int Class2Assessment
        {
            get { return class2Assessment; }
            set { class2Assessment = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); }
        }
        public string TestPerformedOn
        {
            get
            {
                if (Coupons == 1)
                    return "Coupons";
                if (BareBoards == 1)
                    return "BareBoards";
                if (CustomerMounts == 1)
                    return "CustomerMounts";
                if (AssembledBoards == 1)
                    return "AssembledBoards";
                if (Class2Assessment == 1)
                    return "Class2Assessment";

                return testPerformedOn;
            }
            set { testPerformedOn = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}