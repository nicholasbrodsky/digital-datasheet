using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for StructureRequirementsView.xaml
    /// </summary>
    public partial class StructureRequirementsView : Window
    {
        List<string> StructureList { get; set; }
        public List<(string, string)> StructureRequirement { get; set; } = new List<(string, string)>();
        public bool OkClick { get; set; } = false;

        public StructureRequirementsView(List<string> structureList)
        {
            InitializeComponent();
            StructureList = structureList;
        }

        public void Set_Layout()
        {
            StackPanel option_stack;
            TextBlock structure_block;
            TextBox requirement_box;

            foreach (string structure in StructureList)
            {
                option_stack = new StackPanel()
                {
                    Margin = new Thickness(0, 8, 0, 8)
                };
                structure_block = new TextBlock()
                {
                    Text = structure,
                    FontSize = 16,
                    Margin = new Thickness(0, 0, 0, 4)
                };
                requirement_box = new TextBox()
                {
                    Height = 25,
                    Padding = new Thickness(2),
                    FontSize = 14,
                };
                option_stack.Children.Add(structure_block);
                option_stack.Children.Add(requirement_box);
                structure_stackpanel_set.Children.Add(option_stack);
                Height += 70;
            }
        }

        private bool Get_Requirements()
        {
            foreach (StackPanel option_stack in structure_stackpanel_set.Children)
            {
                if ((option_stack.Children[1] as TextBox).Text == "")
                    continue;
                StructureRequirement.Add(((option_stack.Children[0] as TextBlock).Text, (option_stack.Children[1] as TextBox).Text));
            }
            return true;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            if (!Get_Requirements())
            {
                MessageBox.Show("All requirements must be filled out");
                return;
            }
            OkClick = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            OkClick = false;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {

        }
    }
}