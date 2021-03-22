using DigitalDatasheet.Data;
using DigitalDatasheetContextLib;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for RemarksDocumentView.xaml
    /// </summary>
    public partial class RemarksDocumentView : Window
    {
        public bool OkClick { get; set; } = false;
        public bool CancelClick { get; set; }
        public string SelectedRemark { get; set; }

        public RemarksDocumentView()
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
                    remark_selection.Items.Add(item);
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
                                Content = remark.Remark,
                                Background = background,
                                MaxWidth = 1000
                            };
                            remark_selection.Items.Add(item);
                        }
                    }
                    remark_selection.Items.Add(new Separator());
                }
            }
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedRemark = remark_selection.Text;
            if (SelectedRemark == "")
            {
                Close();
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
    }
}