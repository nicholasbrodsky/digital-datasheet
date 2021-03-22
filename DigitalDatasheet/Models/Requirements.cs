using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DigitalDatasheet.Models
{
    public class Requirements : INotifyPropertyChanged
    {
        private string holeCuPlating;
        private string externalConductor;
        private string surfaceCladCu;
        private string wrapCu;
        private string capCu;
        private string internalCladCu;
        private string minEtchback;
        private string maxEtchback;
        private string internalAnnularRing;
        private string externalAnnularRing;
        private string dielectric;
        private string wicking;

        public string HoleCuPlating { get => holeCuPlating; set { holeCuPlating = value; OnPropertyChanged(); } }
        public string ExternalConductor { get => externalConductor; set { externalConductor = value; OnPropertyChanged(); } }
        public string SurfaceCladCu { get => surfaceCladCu; set { surfaceCladCu = value; OnPropertyChanged(); } }
        public string WrapCu { get => wrapCu; set { wrapCu = value; OnPropertyChanged(); } }
        public string CapCu { get => capCu; set { capCu = value; OnPropertyChanged(); } }
        public string InternalCladCu { get => internalCladCu; set { internalCladCu = value; OnPropertyChanged(); } }
        public string MinEtchback { get => minEtchback; set { minEtchback = value; OnPropertyChanged(); } }
        public string MaxEtchback { get => maxEtchback; set { maxEtchback = value; OnPropertyChanged(); } }
        public string InternalAnnularRing { get => internalAnnularRing; set { internalAnnularRing = value; OnPropertyChanged(); } }
        public string ExternalAnnularRing { get => externalAnnularRing; set { externalAnnularRing = value; OnPropertyChanged(); } }
        public string Dielectric { get => dielectric; set { dielectric = value; OnPropertyChanged(); } }
        public string Wicking { get => wicking; set { wicking = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}