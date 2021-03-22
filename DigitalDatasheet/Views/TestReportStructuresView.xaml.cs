using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for TestReportStructuresView.xaml
    /// </summary>
    public partial class TestReportStructuresView : Window
    {
        private int dataPerPage;
        public List<string> StructureList { get; set; }
        public List<(string, string)> StructureToReportStructure { get; set; } = new List<(string, string)>();
        public int DataPerPage { get => dataPerPage; set => dataPerPage = value; }
        public bool OkClick { get; set; } = false;

        public TestReportStructuresView(List<string> structureList = null)
        {
            InitializeComponent();
            StructureList = structureList;
        }

        public void Set_Layout()
        {
            Height += 70;
            StackPanel optionStack;
            TextBlock structureBlock;
            TextBox reportStructureBox;

            foreach (string structure in StructureList)
            {
                optionStack = new StackPanel()
                {
                    Margin = new Thickness(0, 8, 0, 8)
                };
                structureBlock = new TextBlock()
                {
                    Text = structure,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 4)
                };
                reportStructureBox = new TextBox()
                {
                    Height = 25,
                    Padding = new Thickness(2),
                    FontSize = 14,
                };
                optionStack.Children.Add(structureBlock);
                optionStack.Children.Add(reportStructureBox);
                structure_stackpanel_set.Children.Add(optionStack);
                Height += 70;
            }
        }

        private bool Get_Structures()
        {
            if (StructureList != null)
            {
                foreach (StackPanel option_stack in structure_stackpanel_set.Children)
                {
                    if ((option_stack.Children[1] as TextBox).Text == "")
                        return false;
                    StructureToReportStructure.Add(((option_stack.Children[0] as TextBlock).Text, (option_stack.Children[1] as TextBox).Text));
                }
            }
            if (data_per_page_input.Text == "" || !int.TryParse(data_per_page_input.Text, out dataPerPage))
                return false;
            return true;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!Get_Structures())
            {
                MessageBox.Show("All Test Report parameters must be filled out");
                return;
            }
            OkClick = true;
            //cancel_click = false;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OkClick = false;
            Close();
        }
    }
}