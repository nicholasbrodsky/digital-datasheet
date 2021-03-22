using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace DigitalDatasheet.Models
{
    public class Structure : INotifyPropertyChanged
    {
        private string structureTitle;
        private int structureOrder;

        public string StructureTitle { get => structureTitle; set { structureTitle = value; OnPropertyChanged(); } }
        public int StructureOrder { get => structureOrder; set { structureOrder = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class Record : INotifyPropertyChanged
    {
        #region private vars
        private Structure structureInfo;
        private string location = string.Empty;
        private string serialNumber;
        private RecordGroup holeCuPlating = new RecordGroup();
        private RecordGroup externalConductor = new RecordGroup();
        private RecordGroup surfaceCladCu = new RecordGroup();
        //private RecordGroup selectivePlate = new RecordGroup();
        private RecordGroup wrapCu = new RecordGroup();
        private RecordGroup capCu = new RecordGroup();
        private RecordGroup internalCladCu = new RecordGroup();
        private RecordGroup minEtchback = new RecordGroup();
        private RecordGroup maxEtchback = new RecordGroup();
        private RecordGroup internalAnnularRing = new RecordGroup();
        private RecordGroup externalAnnularRing = new RecordGroup();
        private RecordGroup dielectric = new RecordGroup();
        private RecordGroup wicking = new RecordGroup();
        private string innerlayerSeparation;
        private string platingCrack;
        private string platingVoid;
        //private string foilCrack;
        private string delamBlisters;
        private string laminateVoidCrack;
        private string acceptReject;
        private int row;
        #endregion private vars

        public Structure StructureInfo { get => structureInfo; set { structureInfo = value; OnPropertyChanged(); } }
        public string Location { get => location; set { location = value; OnPropertyChanged(); } }
        public string SerialNumber { get => serialNumber; set { serialNumber = value; OnPropertyChanged(); } }
        public RecordGroup HoleCuPlating { get => holeCuPlating; set { holeCuPlating = value; OnPropertyChanged(); } }
        public RecordGroup ExternalConductor { get => externalConductor; set { externalConductor = value; OnPropertyChanged(); } }
        public RecordGroup SurfaceCladCu { get => surfaceCladCu; set { surfaceCladCu = value; OnPropertyChanged(); } }
        //public RecordGroup SelectivePlate { get => selectivePlate; set { selectivePlate = value; OnPropertyChanged(); } }
        public RecordGroup WrapCu { get => wrapCu; set { wrapCu = value; OnPropertyChanged(); } }
        public RecordGroup CapCu { get => capCu; set { capCu = value; OnPropertyChanged(); } }
        public RecordGroup InternalCladCu { get => internalCladCu; set { internalCladCu = value; OnPropertyChanged(); } }
        public RecordGroup MinEtchback { get => minEtchback; set { minEtchback = value; OnPropertyChanged(); } }
        public RecordGroup MaxEtchback { get => maxEtchback; set { maxEtchback = value; OnPropertyChanged(); } }
        public RecordGroup InternalAnnularRing { get => internalAnnularRing; set { internalAnnularRing = value; OnPropertyChanged(); } }
        public RecordGroup ExternalAnnularRing { get => externalAnnularRing; set { externalAnnularRing = value; OnPropertyChanged(); } }
        public RecordGroup Dielectric { get => dielectric; set { dielectric = value; OnPropertyChanged(); } }
        public RecordGroup Wicking { get => wicking; set { wicking = value; OnPropertyChanged(); } }
        public string InnerlayerSeparation { get => innerlayerSeparation; set { innerlayerSeparation = value; OnPropertyChanged(); } }
        public string PlatingCrack { get => platingCrack; set { platingCrack = value; OnPropertyChanged(); } }
        public string PlatingVoid { get => platingVoid; set { platingVoid = value; OnPropertyChanged(); } }
        //public string FoilCrack { get => foilCrack; set { foilCrack = value; OnPropertyChanged(); } }
        public string DelamBlisters { get => delamBlisters; set { delamBlisters = value; OnPropertyChanged(); } }
        public string LaminateVoidCrack { get => laminateVoidCrack; set { laminateVoidCrack = value; OnPropertyChanged(); } }
        public string AcceptReject { get => acceptReject; set { acceptReject = value; OnPropertyChanged(); } }
        public int Row { get => row; set { row = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class RecordGroup : INotifyPropertyChanged
    {
        private string measurement;
        private Brush backgroundColor = new SolidColorBrush(Color.FromRgb(243, 243, 243));
        private string note;
        private Visibility noteShow = Visibility.Hidden;

        public string Measurement
        {
            get { return measurement; }
            set { measurement = value; OnPropertyChanged(); }
        }
        public Brush BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; OnPropertyChanged(); }
        }
        public string Note
        {
            get { return note; }
            set { note = value; OnPropertyChanged(); }
        }
        public Visibility NoteShow
        {
            get { return noteShow; }
            set { noteShow = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}