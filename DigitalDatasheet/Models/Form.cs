using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;

namespace DigitalDatasheet.Models
{
    public class Form : INotifyPropertyChanged
    {
        private int coupons, bareBoards, customerMounts, assembledBoards, class2Assessment;
        private string testPerformedOn;
        private DateTime? dateTested;
        private string testedBy;
        private string checkedBy;
        private string workOrderNo, workOrderNoDash, fullWorkOrder;
        private string partNumber;
        private string lotNumber;
        private string customer;
        private string dateCode;
        private string specification1;
        private string specification2;
        private string boardType;
        private string testProcedure;
        private int drawingProvided;
        private string evaluatedBy;
        private DateTime? dateEvaluated;
        private string bakeTimeIn;
        private string bakeTimeOut;
        private int? totalTime;
        private string testCondition;
        private ComboBoxItem testConditionSelection;
        private int? testTemp;
        private int? solderFloats;
        private string lastSaved;
        private string currentJobTitle;

        public int Coupons
        {
            get { return coupons; }
            set { coupons = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); OnPropertyChanged("CurrentJobTitle"); }
        }
        public int BareBoards
        {
            get { return bareBoards; }
            set { bareBoards = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); OnPropertyChanged("CurrentJobTitle"); }
        }
        public int CustomerMounts
        {
            get { return customerMounts; }
            set { customerMounts = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); OnPropertyChanged("CurrentJobTitle"); }
        }
        public int? CustomerMountQty { get; set; }
        public int AssembledBoards
        {
            get { return assembledBoards; }
            set { assembledBoards = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); OnPropertyChanged("CurrentJobTitle"); }
        }
        public int Class2Assessment
        {
            get { return class2Assessment; }
            set { class2Assessment = value; OnPropertyChanged(); OnPropertyChanged("TestPerformedOn"); OnPropertyChanged("CurrentJobTitle"); }
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
        public DateTime? DateTested { get => dateTested; set { dateTested = value; OnPropertyChanged(); } }
        public string TestedBy { get => testedBy; set { testedBy = value; OnPropertyChanged(); } }
        public string CheckedBy { get => checkedBy; set { checkedBy = value; OnPropertyChanged(); } }
        public string WorkOrderNo
        {
            get { return workOrderNo; }
            set { workOrderNo = value; OnPropertyChanged(); OnPropertyChanged("CurrentJobTitle"); }
        }
        public string WorkOrderNoDash { get => workOrderNoDash; set { workOrderNoDash = value; OnPropertyChanged(); OnPropertyChanged("CurrentJobTitle"); } }
        public string FullWorkOrder
        {
            get { return fullWorkOrder = string.IsNullOrEmpty(WorkOrderNoDash) ? $"{WorkOrderNo}" : $"{WorkOrderNo}-{WorkOrderNoDash}"; }
            set { fullWorkOrder = value; OnPropertyChanged(); }
        }
        public string PartNumber { get => partNumber; set { partNumber = value; OnPropertyChanged(); } }
        public string LotNumber { get => lotNumber; set { lotNumber = value; OnPropertyChanged(); } }
        public string Customer { get => customer; set { customer = value; OnPropertyChanged(); OnPropertyChanged("CurrentJobTitle"); } }
        public string DateCode { get => dateCode; set { dateCode = value; OnPropertyChanged(); } }
        public string Specification1 { get => specification1; set { specification1 = value; OnPropertyChanged(); } }
        public string Specification2 { get => specification2; set { specification2 = value; OnPropertyChanged(); } }
        public string BoardType { get => boardType; set { boardType = value; OnPropertyChanged(); } }
        public string TestProcedure { get => testProcedure; set { testProcedure = value; OnPropertyChanged(); } }
        public int DrawingProvided { get => drawingProvided; set { drawingProvided = value; OnPropertyChanged(); } }
        public string EvaluatedBy { get => evaluatedBy; set { evaluatedBy = value; OnPropertyChanged(); } }
        public DateTime? DateEvaluated { get => dateEvaluated; set { dateEvaluated = value; OnPropertyChanged(); } }
        public string BakeTimeIn { get => bakeTimeIn; set { bakeTimeIn = value; OnPropertyChanged(); } }
        public string BakeTimeOut { get => bakeTimeOut; set { bakeTimeOut = value; OnPropertyChanged(); } }
        public int? TotalTime { get => totalTime; set { totalTime = value; OnPropertyChanged(); } }
        public ComboBoxItem TestConditionSelection
        {
            get { return testConditionSelection; }
            set { testConditionSelection = value; OnPropertyChanged(); OnPropertyChanged("TestCondition"); OnPropertyChanged("CurrentJobTitle"); }
        }
        public string TestCondition
        {
            get { return testConditionSelection != null ? testConditionSelection.Content.ToString() : string.Empty; }
            set { testCondition = value; OnPropertyChanged(); } }
        public int? TestTemp { get => testTemp; set { testTemp = value; OnPropertyChanged(); } }
        public int? SolderFloats { get => solderFloats; set { solderFloats = value; OnPropertyChanged(); } }
        public string LastSaved { get => lastSaved; set { lastSaved = value; OnPropertyChanged(); } }
        public string CurrentJobTitle
        {
            get { return $" {FullWorkOrder}{(!string.IsNullOrEmpty(FullWorkOrder) ? " - " : string.Empty)}" +
                    $"{Customer}{(!string.IsNullOrEmpty(Customer) ? " - " : string.Empty)}" +
                    $"{TestCondition}{(!string.IsNullOrEmpty(TestPerformedOn) ? " - " : string.Empty)}{TestPerformedOn}"; }
            set { currentJobTitle = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}