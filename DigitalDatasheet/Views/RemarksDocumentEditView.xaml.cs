using DigitalDatasheet.Data;
using DigitalDatasheetContextLib;
using DigitalDatasheetEntityLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for RemarksDocumentEditView.xaml
    /// </summary>
    public partial class RemarksDocumentEditView : Window
    {
        public bool OkClick { get; set; } = false;
        public bool CancelClick { get; set; }
        public bool NewClick { get; set; }
        public bool SaveClick { get; set; }
        public string SelectedRemark { get; set; }
        public string UpdatedRemark { get; set; }
        public string SelectedSectionTitle { get; set; }
        public string UpdatedSectionTitle { get; set; }

        private int remarkID;
        public int RemarkID
        {
            get { return remarkID; }
            set { remarkID = value; }
        }
        public RemarksDocumentEditView()
        {
            InitializeComponent();
            _ = Get_Remarks_Doc();
        }
        private async Task Get_Remarks_Doc()
        {
            ComboBoxItem item;
            SolidColorBrush background;

            using (var db = new DigitalDatasheetContext())
            {
                var remarksDocument = await Task.Run(() => db.RemarksDocument.OrderBy(r => r.ID));
                var sectionTitles = remarksDocument.Select(r => new { r.SectionTitle }).Distinct();
                foreach (var section in sectionTitles)
                {
                    item = new ComboBoxItem()
                    {
                        Content = section.SectionTitle.ToUpper(),
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 15,
                        Padding = new Thickness(4),
                        IsEnabled = false,
                        MaxWidth = 1000
                    };
                    remarkSelectionInput.Items.Add(item);
                    int row = 0;
                    foreach (var remark in remarksDocument)
                    {
                        //if (string.IsNullOrEmpty(remark.Remark))
                        if (remark.Remark == remark.SectionTitle)
                            continue;
                        if (remark.SectionTitle == section.SectionTitle)
                        {
                            if (row++ % 2 == 0)
                                background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                            else
                                background = new SolidColorBrush(Colors.Transparent);

                            item = new ComboBoxItem()
                            {
                                Content = /*$"{remark.ID}. {remark.Remark}"*/remark.Remark,
                                Background = background,
                                MaxWidth = 1000
                            };
                            remarkSelectionInput.Items.Add(item);
                        }
                    }
                    remarkSelectionInput.Items.Add(new Separator());
                }
            }
        }
        private void Select_Click(object sender, RoutedEventArgs e)
        {
            if (remarkSelectionInput.SelectedIndex < 0)
            {
                MessageBox.Show("No remark has been selected for editing. Please choose a remark before clicking \"Select\"", "No Remark Selected", MessageBoxButton.OK, MessageBoxImage.Error);
                remarkEditInput.Text = string.Empty;
                sectionTitleInput.Items.Clear();
                return;
            }
            SelectedRemark = remarkSelectionInput.Text;
            //if (!int.TryParse(remarkSelection.Substring(0, remarkSelection.IndexOf('.')), out remarkID))
            //{
            //    MessageBox.Show("Error in selecting remark to edit. Please close and try again.", "Remark Selection Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            using (var db = new DigitalDatasheetContext())
            {
                RemarkID = db.RemarksDocument.Where(r => r.Remark == SelectedRemark).First().ID;
                // populate section titles
                ComboBoxItem item;
                List<string> sectionTitles = db.RemarksDocument.Where(r => !string.IsNullOrEmpty(r.Remark)).OrderBy(r => r.ID).Select(r => r.SectionTitle).Distinct().ToList();
                foreach (string section in sectionTitles)
                {
                    item = new ComboBoxItem()
                    {
                        Content = section.ToUpper(),
                        FontWeight = FontWeights.SemiBold,
                        FontSize = 15,
                        Padding = new Thickness(4),
                        MaxWidth = 1000
                    };
                    sectionTitleInput.Items.Add(item);
                }

                var remark = db.RemarksDocument.Find(RemarkID);
                SelectedSectionTitle = remark.SectionTitle;
                sectionTitleInput.Text = SelectedSectionTitle.ToUpper();
                //SelectedRemark = remark.Remark;
            }

            //SelectedRemark = remarkSelection.Substring(remarkSelection.IndexOf('.') + 2);
            remarkEditInput.Text = SelectedRemark;
            remarkSelectionInput.SelectedIndex = -1;
            //MessageBox.Show($"{RemarkID}");
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Add_Click(object sender, RoutedEventArgs e)
        {
            UpdatedSectionTitle = sectionTitleInput.Text;
            UpdatedRemark = remarkEditInput.Text;
            if (string.IsNullOrEmpty(UpdatedSectionTitle) || string.IsNullOrEmpty(UpdatedRemark))
            {
                MessageBox.Show("Section title and remark fields must both be filled. Please correct this issue and try again.", "Remark Add Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            using (var db = new DigitalDatasheetContext())
            {
                DocumentRemark newRemark = new DocumentRemark()
                {

                };
            }
        }
        private void Update_Click(object sender, RoutedEventArgs e)
        {
            UpdatedRemark = remarkEditInput.Text;
            if (UpdatedRemark == SelectedRemark)
            {
                MessageBox.Show("No update to the remark has been made. Please edit the remark before saving.", "No Update Made", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                return;
            }
            int updated = 0;
            using (var db = new DigitalDatasheetContext())
            {
                var remark = db.RemarksDocument.Find(RemarkID);
                remark.Remark = UpdatedRemark;
                updated = db.SaveChangesAsync().Result;
            }
            if (updated == 1)
            {
                MessageBox.Show("Remark updated.");
                SelectedRemark = UpdatedRemark;
                remarkSelectionInput.Items.Clear();
                _ = Get_Remarks_Doc();
            }
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            //if (!OkClick)
            //    CancelClick = true;
        }
    }
}
