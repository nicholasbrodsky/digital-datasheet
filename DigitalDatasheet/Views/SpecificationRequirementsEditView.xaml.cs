using DigitalDatasheetContextLib;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for SpecificationRequirementsEditView.xaml
    /// </summary>
    public partial class SpecificationRequirementsEditView : Window, INotifyPropertyChanged
    {
        private string specification;
        private string newSpecification;
        private SpecificationRequirements specificationRequirements = new SpecificationRequirements();

        public string Specification
        {
            get { return specification; }
            set { specification = value; OnPropertyChanged(); }
        }
        public string NewSpecification
        {
            get { return newSpecification; }
            set { newSpecification = value; OnPropertyChanged(); }
        }
        public SpecificationRequirements SpecificationRequirements
        {
            get { return specificationRequirements; }
            set { specificationRequirements = value; OnPropertyChanged(); }
        }
        public bool SpecificationSelected { get; set; } = false;
        public bool ResetSpecifications { get; set; } = false;
        public SpecificationRequirementsEditView()
        {
            DataContext = this;
            InitializeComponent();
            _ = Get_Specifications();
        }
        private async Task Get_Specifications()
        {
            using (var db = new DigitalDatasheetContext())
            {
                var specifications = await Task.Run(() => db.SpecificationRequirementsTable.OrderBy(s => s.Specification).Select(s => new { s.Specification }));
                foreach (var spec in specifications)
                {
                    specificationSelectInput.Items.Add(spec.Specification);
                }
            }
        }
        private async void Select_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Specification))
            {
                MessageBox.Show($"Specification must be selected to view requirements.", "No Specification Selected", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Specification = string.Empty;
                NewSpecification = string.Empty;
                SpecificationRequirements = new SpecificationRequirements();
                SpecificationSelected = false;
                return;
            }
            using (var db = new DigitalDatasheetContext())
            {
                var specRequirements = await Task.Run(() => db.SpecificationRequirementsTable.Find(Specification));
                if (specRequirements == null)
                {
                    MessageBox.Show($"{Specification} specification not found. Only specifications selected from dropdown list can be used to view requirements.", "Specification Not Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    Specification = string.Empty;
                    NewSpecification = string.Empty;
                    SpecificationRequirements = new SpecificationRequirements();
                    SpecificationSelected = false;
                }
                else
                {
                    NewSpecification = Specification;
                    SpecificationRequirements.HoleCuPlating = specRequirements.HoleCuPlating;
                    SpecificationRequirements.WrapCu = specRequirements.WrapCu;
                    SpecificationRequirements.CapCu = specRequirements.CapCu;
                    SpecificationRequirements.MinEtchback = specRequirements.MinEtchback;
                    SpecificationRequirements.MaxEtchback = specRequirements.MaxEtchback;
                    SpecificationRequirements.InternalAnnularRing = specRequirements.InternalAnnularRing;
                    SpecificationRequirements.ExternalAnnularRing = specRequirements.ExternalAnnularRing;
                    SpecificationRequirements.Dielectric = specRequirements.Dielectric;
                    SpecificationRequirements.Wicking = specRequirements.Wicking;
                    SpecificationRequirements.WickingNote = specRequirements.WickingNote;

                    SpecificationSelected = true;
                }
            }
        }
        private async void Update_Click(object sender, RoutedEventArgs e)
        {
            if (!SpecificationSelected)
                return;
            //if (Specification != NewSpecification)
            //{
            //    if (MessageBox.Show($"The name of the specification will be changed from {Specification} to {NewSpecification}, along with the updated requirements (if any). Would you like to continue?", "Specification Name Change", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
            //        return;
            //    MessageBox.Show($"The Specification you are trying to add already exists. Select \"Update\" to change requirements for {NewSpecification} otherwise change the name of the new specification you want to add", "Specification Already Exists", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            using (var db = new DigitalDatasheetContext())
            {
                DigitalDatasheetEntityLib.SpecificationRequirements specificationRequirements = await Task.Run(() => db.SpecificationRequirementsTable.Find(Specification));
                //specificationRequirements.Specification = NewSpecification;
                specificationRequirements.HoleCuPlating = SpecificationRequirements.HoleCuPlating;
                specificationRequirements.WrapCu = SpecificationRequirements.WrapCu;
                specificationRequirements.CapCu = SpecificationRequirements.CapCu;
                specificationRequirements.MinEtchback = SpecificationRequirements.MinEtchback;
                specificationRequirements.MaxEtchback = SpecificationRequirements.MaxEtchback;
                specificationRequirements.InternalAnnularRing = SpecificationRequirements.InternalAnnularRing;
                specificationRequirements.ExternalAnnularRing = SpecificationRequirements.ExternalAnnularRing;
                specificationRequirements.Dielectric = SpecificationRequirements.Dielectric;
                specificationRequirements.Wicking = SpecificationRequirements.Wicking;
                specificationRequirements.WickingNote = SpecificationRequirements.WickingNote;
                int affected = await Task.Run(() => db.SaveChangesAsync().Result);
                if (affected == 1)
                {
                    Specification = string.Empty;
                    NewSpecification = string.Empty;
                    SpecificationRequirements = new SpecificationRequirements();
                    SpecificationSelected = false;
                    MessageBox.Show("Specification successfully updated.", "Specification Updated!", MessageBoxButton.OK, MessageBoxImage.Information);
                    //ResetSpecifications = true;
                    //specificationSelectInput.Items.Clear();
                    //await Get_Specifications();
                }
                else
                {
                    MessageBox.Show($"The specification was unable to be added. Please close and try again.", "Specification Add Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new DigitalDatasheetContext())
            {
                DigitalDatasheetEntityLib.SpecificationRequirements specCheck = await Task.Run(() => db.SpecificationRequirementsTable.Find(Specification));
                if (specCheck != null)
                {
                    MessageBox.Show($"The Specification you are trying to add already exists. Select \"Update\" to change requirements for {NewSpecification} otherwise change the name of the new specification you want to add", "Specification Already Exists", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                DigitalDatasheetEntityLib.SpecificationRequirements specificationRequirements = new DigitalDatasheetEntityLib.SpecificationRequirements
                {
                    Specification = NewSpecification,
                    HoleCuPlating = SpecificationRequirements.HoleCuPlating,
                    WrapCu = SpecificationRequirements.WrapCu,
                    CapCu = SpecificationRequirements.CapCu,
                    MinEtchback = SpecificationRequirements.MinEtchback,
                    MaxEtchback = SpecificationRequirements.MaxEtchback,
                    InternalAnnularRing = SpecificationRequirements.InternalAnnularRing,
                    ExternalAnnularRing = SpecificationRequirements.ExternalAnnularRing,
                    Dielectric = SpecificationRequirements.Dielectric,
                    Wicking = SpecificationRequirements.Wicking,
                    WickingNote = SpecificationRequirements.WickingNote
                };
                await db.SpecificationRequirementsTable.AddAsync(specificationRequirements);
                int affected = await Task.Run(() => db.SaveChangesAsync().Result);
                if (affected == 1)
                {
                    Specification = string.Empty;
                    NewSpecification = string.Empty;
                    SpecificationRequirements = new SpecificationRequirements();
                    MessageBox.Show("Specification successfully added.", "Specification Added!", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetSpecifications = true;
                    specificationSelectInput.Items.Clear();
                    await Get_Specifications();
                }
                else
                {
                    MessageBox.Show($"The specification was unable to be added. Please close and try again.", "Specification Add Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private async void Delete_Click(object sender, RoutedEventArgs e)
        {
            if (!SpecificationSelected)
                return;
            if (MessageBox.Show($"You are about to remove specification {Specification} from this list. Would you like to continue?", "Specification Removal", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
                return;
            using (var db = new DigitalDatasheetContext())
            {
                DigitalDatasheetEntityLib.SpecificationRequirements specificationRequirements = await Task.Run(() => db.SpecificationRequirementsTable.Find(Specification));
                db.SpecificationRequirementsTable.Remove(specificationRequirements);
                int affected = await Task.Run(() => db.SaveChangesAsync().Result);
                if (affected == 1)
                {
                    specificationSelectInput.Items.Remove(specificationSelectInput.SelectedItem);
                    Specification = string.Empty;
                    NewSpecification = string.Empty;
                    SpecificationRequirements = new SpecificationRequirements();
                    SpecificationSelected = false;
                    MessageBox.Show("Specification successfully removed.", "Specification Removed!", MessageBoxButton.OK, MessageBoxImage.Information);
                    ResetSpecifications = true;
                    //specificationSelectInput.Items.Clear();
                    //await Get_Specifications();
                }
                else
                {
                    MessageBox.Show($"The specification was unable to be removed. Please close and try again.", "Specification Removal Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    public class SpecificationRequirements : INotifyPropertyChanged
    {
        private string holeCuPlating;
        private string wrapCu;
        private string capCu;
        private string minEtchback;
        private string maxEtchback;
        private string internalAnnularRing;
        private string externalAnnularRing;
        private string dielectric;
        private string wicking;
        private bool wickingNote;

        public string HoleCuPlating
        {
            get { return holeCuPlating; }
            set { holeCuPlating = value; OnPropertyChanged(); }
        }
        public string WrapCu
        {
            get { return wrapCu; }
            set { wrapCu = value; OnPropertyChanged(); }
        }
        public string CapCu
        {
            get { return capCu; }
            set { capCu = value; OnPropertyChanged(); }
        }
        public string MinEtchback
        {
            get { return minEtchback; }
            set { minEtchback = value; OnPropertyChanged(); }
        }
        public string MaxEtchback
        {
            get { return maxEtchback; }
            set { maxEtchback = value; OnPropertyChanged(); }
        }
        public string InternalAnnularRing
        {
            get { return internalAnnularRing; }
            set { internalAnnularRing = value; OnPropertyChanged(); }
        }
        public string ExternalAnnularRing
        {
            get { return externalAnnularRing; }
            set { externalAnnularRing = value; OnPropertyChanged(); }
        }
        public string Dielectric
        {
            get { return dielectric; }
            set { dielectric = value; OnPropertyChanged(); }
        }
        public string Wicking
        {
            get { return wicking; }
            set { wicking = value; OnPropertyChanged(); }
        }
        public bool WickingNote
        {
            get { return wickingNote; }
            set { wickingNote = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
