#region Usings
using DigitalDatasheet.Data;
using DigitalDatasheet.Documents;
using DigitalDatasheet.Models;
using DigitalDatasheet.Views;
using DigitalDatasheetContextLib;
using DigitalDatasheetEntityLib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
#endregion Usings

namespace DigitalDatasheet
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		#region Data Binding Elements
		private Form formSet;
		private List<Structure> structureTitles;
		private List<Record> recordSet;
		private Requirements requirement;
		private List<RemarkSet> remarks;
		private bool autoSave;
		private string autoSaveLabel = "Auto Save OFF";
		private Brush autoSaveDecoration = new SolidColorBrush(Colors.Red);
		private string dueDateInfoLabel;
		private Brush dueDateInfoColor = new SolidColorBrush(Colors.Black);
		private Visibility expediteIconShow = Visibility.Hidden;

		public Form FormSet
		{
			get { return formSet; }
			set { formSet = value; OnPropertyChanged(); }
		}
		public List<Structure> StructureTitles
		{
			get { return structureTitles; }
			set { structureTitles = value; OnPropertyChanged(); }
		}
		public List<Record> RecordSet
		{
			get { return recordSet; }
			set { recordSet = value; OnPropertyChanged(); }
		}
		public Requirements Requirement
		{
			get { return requirement; }
			set { requirement = value; OnPropertyChanged(); }
		}
		public List<RemarkSet> Remarks
		{
			get { return remarks; }
			set { remarks = value; OnPropertyChanged(); }
		}
		public bool AutoSave
		{
			get { return autoSave; }
			set { autoSave = value; OnPropertyChanged(); }
		}
		public string AutoSaveLabel
		{
			get { return autoSaveLabel; }
			set { autoSaveLabel = value; OnPropertyChanged(); }
		}
		public Brush AutoSaveDecoration
		{
			get { return autoSaveDecoration; }
			set { autoSaveDecoration = value; OnPropertyChanged(); }
		}
        public string DueDateInfoLabel
        {
            get { return dueDateInfoLabel; }
            set { dueDateInfoLabel = value; OnPropertyChanged(); }
        }
        public Brush DueDateInfoColor
        {
            get { return dueDateInfoColor; }
            set { dueDateInfoColor = value; OnPropertyChanged(); }
        }
		public Visibility ExpediteIconShow
        {
			get { return expediteIconShow; }
			set { expediteIconShow = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

		#endregion Data Binding Elements

		#region UIElement Names
		public List<string> StructureTitleNames { get; set; } = new List<string>();
		public List<string> MGridNames { get; set; } = new List<string>();
		public List<string> OGridNames { get; set; } = new List<string>();
		public List<string> AddGridBtnNames { get; set; } = new List<string>();
		public List<string> RemarkNames { get; set; } = new List<string>();
		#endregion UIElement Names

		private bool WoCheck { get; set; } = false;
		//public readonly string error_log_file_path = $@"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\Digital Datasheet Error Log\{DateTime.Now:D}.txt";
		//public StreamWriter error_log_sw;
		public bool NetConn { get; set; } = true;
		public bool JobConflictWarning { get; set; } = false;
		//public bool IsOpened { get; set; }
		public bool IsOpening { get; set; }
        public bool IsReadyOnly { get; set; }
        public bool IsClosed { get; set; }
        public List<decimal?> DefaultCheckList { get; set; }

        #region Unsaved Changes Check
        public bool UnsavedChanges { get; set; }
        public bool UnsavedForm { get; set; }
        public bool UnsavedData { get; set; }
        public bool UnsavedRequirements { get; set; }
        public bool UnsavedRemarks { get; set; }
		#endregion Unsaved Changes Check

		public bool DuplicateDataRow { get; set; }
        public bool ZoomWinOpen { get; set; }
        public MainWindow()
		{
			DataContext = this;
			InitializeComponent();

			#region Initialize Data Binding Elements
			FormSet = new Form();
			StructureTitles = new List<Structure> { new Structure { StructureTitle = "Hole Structure 1", StructureOrder = 1 } };
			RecordSet = new List<Record> { new Record { StructureInfo = StructureTitles[0], Row = 1 } };
			Requirement = new Requirements { InternalCladCu = "Layers:\n" };
			Remarks = new List<RemarkSet>();
			#endregion Initialize Data Binding Elements

			#region Add Initial UIElement Names
			StructureTitleNames.Add("struct_0");
			MGridNames.Add("m_grid_0");
			OGridNames.Add("o_grid_0");
			AddGridBtnNames.Add("add_grid_0");
			#endregion Add Initial UIElement Names

			#region Network Check
			if (!Is_Network_Available())
			{
				if (MessageBox.Show("Currently unable to access network files. Any data that can be saved will be stored on your local desktop. Would you like to continue?",
					"Network Connection ERROR!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
				{
					Close();
					return;
				}
				NetConn = false;
				//error_log_dir = $@"C:\Users\{user}\Documents";
				//if(!Directory.Exists($@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log"))
				//{
				//    Directory.CreateDirectory($@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log");
				//}
				//error_log_file_path = $@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log\{DateTime.Now:D}.txt";
			}
			#endregion Network Check

			#region Error Log Check
			//try
			//{
			//    error_log_file_path = $@"\\ptlsrvr4\PTLOffice\Digital Datasheet Forms\Digital Datasheet Error Log\{DateTime.Now:D}.txt";
			//    error_log_sw = new StreamWriter(error_log_file_path, true);
			//    error_log_sw.Close();
			//}
			//catch (Exception err)
			//{
			//    if (MessageBox.Show("Currently unable to access network files. Any data saved will be stored on your local desktop. Would you like to continue?", "Network Connection ERROR!", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
			//    {
			//        Close();
			//        return;
			//    }
			//    network_connection = false;
			//    //error_log_dir = $@"C:\Users\{user}\Documents";
			//    if (!Directory.Exists($@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log"))
			//    {
			//        Directory.CreateDirectory($@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log");
			//    }
			//    error_log_file_path = $@"C:\Users\{Environment.UserName.ToLower()}\Documents\Local Error Log\{DateTime.Now:D}.txt";
			//    error_log_sw = new StreamWriter(error_log_file_path, true);
			//    error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Constructor -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
			//    error_log_sw.Close();
			//}
			#endregion Error Log Check

			_ = Get_Customers();
			_ = Get_Specification();
			_ = Load_Recent_Jobs();

			DefaultCheckList = new List<decimal?>
			{
				0.005m, 0.005m, 0.005m, 0.005m, 0.005m, 0.005m, null, null, null, null, null, null
			};

			condition_input.SelectedIndex = 0;
			zoom_grid.Width = Width - 20;
		}

		#region UI Format Events
		/// <summary>
		/// Add/Remove new structure set and data row
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Add_Exam_Grid(object sender, RoutedEventArgs e)
		{
			try
			{
				int current_rows = complete_grid.RowDefinitions.Count;

				Binding binding;

				RowDefinition title_row, data_row;
				ColumnDefinition col;
				Button add_btn, remove_btn;
				TextBox struct_title;
				Grid m_grid, o_grid;
				StackPanel btn_container;

				int row_num = current_rows;
				int exam_row_count = MGridNames.Count;

				title_row = new RowDefinition
				{
					Height = new GridLength(0, GridUnitType.Auto)
				};
				complete_grid.RowDefinitions.Add(title_row);

				data_row = new RowDefinition
				{
					Height = new GridLength(0, GridUnitType.Auto)
				};
				complete_grid.RowDefinitions.Add(data_row);
				StructureTitleNames.Add("struct_" + StructureTitleNames.Count);
				struct_title = new TextBox()
				{
					TextAlignment = TextAlignment.Center,
					FontSize = 20,
					Padding = new Thickness(6),
					BorderThickness = new Thickness(1, 0, 1, 0)
				};
				struct_title.LostFocus += new RoutedEventHandler(Structure_Title_LostFocus);
				struct_title.Name = StructureTitleNames[exam_row_count];
				RegisterName(StructureTitleNames[exam_row_count], struct_title);
				//binding.Source = Measurement;
				int structureTitleCount = StructureTitles.Count;
				StructureTitles.Add(new Structure { StructureTitle = $"Hole Structure {structureTitleCount + 1}", StructureOrder = structureTitleCount + 1 });
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"StructureTitles[{structureTitleCount}].StructureTitle"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				struct_title.SetBinding(TextBox.TextProperty, binding);

				btn_container = new StackPanel()
				{
					Orientation = Orientation.Horizontal,
					VerticalAlignment = VerticalAlignment.Top
				};
				AddGridBtnNames.Add("add_grid_" + AddGridBtnNames.Count);
				add_btn = new Button()
				{
					Height = 25,
					Width = 25,
					Tag = exam_row_count,
					Content = new Image()
					{
						Source = new BitmapImage(new Uri("Images/add_icon.png", UriKind.Relative))
					},
					Name = AddGridBtnNames[exam_row_count],
					ToolTip = "Add a new data row within the current structure section."
				};
				RegisterName(AddGridBtnNames[exam_row_count], add_btn);
				add_btn.Click += new RoutedEventHandler(Add_Data_Row);
				remove_btn = new Button()
				{
					Height = 25,
					Width = 25,
					Tag = exam_row_count,
					Margin = new Thickness(0, 0, 3, 0),
					Content = new Image()
					{
						Source = new BitmapImage(new Uri("Images/remove_icon.png", UriKind.Relative))
					},
					ToolTip = "Remove the last data row within the current structure section."
				};
				remove_btn.Click += new RoutedEventHandler(Remove_Data_Row);
				btn_container.Children.Add(remove_btn);
				btn_container.Children.Add(add_btn);

				MGridNames.Add("m_grid_" + exam_row_count);
				m_grid = new Grid
				{
					Name = MGridNames[exam_row_count]
				};
				RegisterName(MGridNames[exam_row_count], m_grid);

				for (int i = 0; i < 13; i++)
				{
					col = new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					};
					m_grid.ColumnDefinitions.Add(col);
				}

				OGridNames.Add("o_grid_" + exam_row_count);
				o_grid = new Grid
				{
					Name = OGridNames[exam_row_count]
				};
				RegisterName(OGridNames[exam_row_count], o_grid);

				for (int i = 0; i < 6; i++)
				{
					col = new ColumnDefinition
					{
						Width = new GridLength(1, GridUnitType.Star)
					};
					o_grid.ColumnDefinitions.Add(col);
				}

				Grid.SetRow(struct_title, row_num);
				Grid.SetColumn(struct_title, 1);
				Grid.SetColumnSpan(struct_title, 3);
				Grid.SetRow(btn_container, row_num + 1);
				Grid.SetColumn(btn_container, 0);
				Grid.SetRow(m_grid, row_num + 1);
				Grid.SetColumn(m_grid, 1);
				Grid.SetRow(o_grid, row_num + 1);
				Grid.SetColumn(o_grid, 2);

				complete_grid.Children.Add(struct_title);
				complete_grid.Children.Add(btn_container);
				complete_grid.Children.Add(m_grid);
				complete_grid.Children.Add(o_grid);

				add_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
				struct_title.Focus();
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Add_Exam_Grid -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to add new structure title to the grid.\n{ex}", "Add Structure Row ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void Remove_Exam_Grid(object sender, RoutedEventArgs e)
		{
			int current_rows = complete_grid.RowDefinitions.Count;

			if (MGridNames.Count == 1)
				return;

			if (!IsOpening)
			{
				bool dataCheck = false;
				bool remove = false;
				RecordSet.Where(record => record.StructureInfo.StructureOrder == MGridNames.Count).ToList()
					.ForEach(record =>
					{
						if (remove) return;
						foreach (var item in record.GetType().GetProperties())
						{
							var currentRecord = item.GetValue(record, null);
							if (currentRecord is RecordGroup)
							{
								if (!string.IsNullOrEmpty((currentRecord as RecordGroup).Measurement) || !string.IsNullOrEmpty((currentRecord as RecordGroup).Note))
								{
									if (MessageBox.Show("The section you are attempting to remove contains data that will be lost. Would you like to continue?",
										"Remove Structure Section", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
										dataCheck = true;
									else
                                    {
										remove = true;
										return;
                                    }
								}
							}
						}
					});
				if (dataCheck) return;
			}

			// remove all elements of last two rows
			UIElement element = null;
			for (int i = complete_grid.Children.Count - 1; i >= 0; i--)
			{
				element = complete_grid.Children[i];
				if (Grid.GetRow(element) == current_rows - 1 || Grid.GetRow(element) == current_rows - 2)
					complete_grid.Children.Remove(element);
			}
			// remove last two row definitions
			RowDefinition row = null;
			for (int i = complete_grid.RowDefinitions.Count - 1, j = 0; j < 2; j++, i--)
			{
				row = complete_grid.RowDefinitions[i];
				complete_grid.RowDefinitions.Remove(row);
			}
			// unregister the names of the struct title and two grids removed and pop them off the array of grid names
			UnregisterName(StructureTitleNames[^1]);
			UnregisterName(MGridNames[^1]);
			UnregisterName(OGridNames[^1]);
			UnregisterName(AddGridBtnNames[^1]);
			StructureTitleNames.RemoveAt(StructureTitleNames.Count - 1);
			MGridNames.RemoveAt(MGridNames.Count - 1);
			OGridNames.RemoveAt(OGridNames.Count - 1);
			AddGridBtnNames.RemoveAt(AddGridBtnNames.Count - 1);

			RecordSet.RemoveAll(record => record.StructureInfo.Equals(StructureTitles.Last()));
			StructureTitles.Remove(StructureTitles.Last());
		}
		/// <summary>
		/// Automatically format following structure data rows to match first set's row number, serial numbers, and locations
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Auto_Set_Data_Row(object sender, RoutedEventArgs e)
		{
			try
			{
				if (MessageBox.Show("You are about to duplicate the first structure's serial number/location list to remaining structures. Would you like to continue?", "Duplicate Serial Numbers", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
				{
					// get number of rows of first measurement/observation grid
					Grid m_grid = FindName(MGridNames[0]) as Grid;
					int rows = m_grid.RowDefinitions.Count;

					var records = RecordSet.Where(r => r.StructureInfo.StructureOrder == 1).OrderBy(r => r.Row).ToList();
					var recordCount = records.Count;
					for (int i = 1; i < StructureTitles.Count; i++)
					{
						Button add_row_btn = FindName(AddGridBtnNames[i]) as Button;
						var nextRecordSet = RecordSet.Where(r => r.StructureInfo.StructureOrder == i + 1).ToList();
						var nextRecordSetCount = nextRecordSet.Count;
						var recordCountDiff = recordCount - nextRecordSetCount;
						for (int j = 0; j < recordCountDiff; j++)
							add_row_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

						nextRecordSet = RecordSet.Where(r => r.StructureInfo.StructureOrder == i + 1).OrderBy(r => r.Row).ToList();
						for (int j = 0; j < recordCount; j++)
						{
							nextRecordSet[j].Location = records[j].Location;
							nextRecordSet[j].SerialNumber = records[j].SerialNumber;
						}
					}
				}
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Auto_Set_Data_Row -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to automatically set number of rows per structure title.\n{ex}", "Data Row Auto Generation ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// Add data row to current structure set
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Add_Data_Row(object sender, RoutedEventArgs e)
		{
			//Grid testGrid = (Grid)complete_grid.Children.Cast<UIElement>().First(ee => Grid.GetRow(ee) == 1 && Grid.GetColumn(ee) == 1);

			try
			{
				Binding binding, colorBinding, noteBinding, noteShowBinding;

				Button btn = (Button)sender;
				string tagStr = btn.Tag.ToString();
				int.TryParse(tagStr, out int tag);

				Grid m_grid = FindName(MGridNames[tag]) as Grid;
				Grid o_grid = FindName(OGridNames[tag]) as Grid;

				int current_rows = m_grid.RowDefinitions.Count;
				//MessageBox.Show(current_rows.ToString());

				RecordSet.Add(new Record
				{
					StructureInfo = StructureTitles[tag],
					Row = current_rows + 1
				});

				SolidColorBrush background;
				RowDefinition measurement_row, observation_row;
				Border info_container;
				StackPanel outer, inner_top, inner_bottom;
				TextBlock loc, sn;
				TextBox hole_plating, external_conductor, surface_clad, /*selective_plate,*/ wrap, cap, internal_clad, min_etch, max_etch, internal_ring, external_ring, dielectric, wicking, loc_input, sn_input;
				TextBox hcpNote, extConNote, sCladNote, /*sPlateNote,*/ wrapNote, capNote, intCladNote, minEtchNote, maxEtchNote, intRingNote, extRingNote, diNote, wickNote;
				TextBox innerlayer_sep, plating_crack, plating_void, /*foil_crack, */delam_blisters, lam_void, accept_reject;
				int row_num = current_rows;
				double dataFontSize = 15;
				double obsFontSize = 18;

				if (row_num % 2 == 0)
					background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
				else
					background = new SolidColorBrush(Colors.Transparent);

				measurement_row = new RowDefinition
				{
					Height = new GridLength(0, GridUnitType.Auto)
				};
				m_grid.RowDefinitions.Add(measurement_row);

				outer = new StackPanel
				{
					Margin = new Thickness(1, 0, 1, 0)
				};

				inner_top = new StackPanel()
				{
					Orientation = Orientation.Horizontal,
					Margin = new Thickness(0, 1, 0, 1)
				};
				loc = new TextBlock()
				{
					Text = "loc ",
					Padding = new Thickness(3),
					Foreground = new SolidColorBrush(Colors.Red)
				};
				loc_input = new TextBox()
				{
					Width = 55,
					FontSize = 12,
					Padding = new Thickness(1),
					Foreground = new SolidColorBrush(Colors.Red)
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].Location"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				loc_input.SetBinding(TextBox.TextProperty, binding);
				inner_top.Children.Add(loc);
				inner_top.Children.Add(loc_input);

				inner_bottom = new StackPanel
				{
					Orientation = Orientation.Horizontal
				};
				sn = new TextBlock()
				{
					Text = "S/N ",
					Padding = new Thickness(1)
				};
				sn_input = new TextBox()
				{
					Width = 55,
					FontSize = 12,
					Padding = new Thickness(1)
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].SerialNumber"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				sn_input.SetBinding(TextBox.TextProperty, binding);
				inner_bottom.Children.Add(sn);
				inner_bottom.Children.Add(sn_input);

				outer.Children.Add(inner_top);
				outer.Children.Add(inner_bottom);

				info_container = new Border
				{
					BorderBrush = new SolidColorBrush(Colors.DarkGray),
					BorderThickness = new Thickness(1),
					Background = background,
					Child = outer
				};
				#region Hole Cu Plating
				hole_plating = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				hcpNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].HoleCuPlating,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].HoleCuPlating.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].HoleCuPlating,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].HoleCuPlating,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].HoleCuPlating,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				hole_plating.SetBinding(TextBox.TextProperty, binding);
				hole_plating.SetBinding(BackgroundProperty, colorBinding);
				hcpNote.SetBinding(TextBox.TextProperty, noteBinding);
				hcpNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Hole Cu Plating
				#region External Conductor Thickness
				external_conductor = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				extConNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalConductor,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].ExternalConductor.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalConductor,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalConductor,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalConductor,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				external_conductor.SetBinding(TextBox.TextProperty, binding);
				external_conductor.SetBinding(BackgroundProperty, colorBinding);
				extConNote.SetBinding(TextBox.TextProperty, noteBinding);
				extConNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion External Conductor Thickness
				#region Surface Clad Cu
				surface_clad = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				sCladNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].SurfaceCladCu,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].SurfaceCladCu.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].SurfaceCladCu,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].SurfaceCladCu,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].SurfaceCladCu,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				surface_clad.SetBinding(TextBox.TextProperty, binding);
				surface_clad.SetBinding(BackgroundProperty, colorBinding);
				sCladNote.SetBinding(TextBox.TextProperty, noteBinding);
				sCladNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion  Surface Clad Cu
				#region Selective Plate
				//selective_plate = new TextBox
				//{
				//    TextWrapping = TextWrapping.Wrap,
				//    TextAlignment = TextAlignment.Center,
				//    Padding = new Thickness(0, 0, 0, 5),
				//    VerticalContentAlignment = VerticalAlignment.Bottom,
				//    AcceptsReturn = true,
				//    FontSize = 14,
				//    Height = 48,
				//    ContextMenu = FindResource("measurement_menu") as ContextMenu
				//};
				//sPlateNote = new TextBox()
				//{
				//    Padding = new Thickness(-1, -2, -1, 0),
				//    Background = new SolidColorBrush(Colors.AliceBlue),
				//    FontSize = 10,
				//    VerticalAlignment = VerticalAlignment.Top,
				//    HorizontalAlignment = HorizontalAlignment.Right,
				//    MinWidth = 15,
				//    TextAlignment = TextAlignment.Center,
				//    Tag = "note"
				//};
				//binding = new Binding
				//{
				//    Mode = BindingMode.TwoWay,
				//    Source = RecordSet[RecordSet.Count - 1].SelectivePlate,
				//    Path = new PropertyPath($"Measurement")
				//};
				//RecordSet[RecordSet.Count - 1].SelectivePlate.BackgroundColor = background;
				//colorBinding = new Binding
				//{
				//    Mode = BindingMode.TwoWay,
				//    Source = RecordSet[RecordSet.Count - 1].SelectivePlate,
				//    Path = new PropertyPath($"BackgroundColor")
				//};
				//noteBinding = new Binding
				//{
				//    Mode = BindingMode.TwoWay,
				//    Source = RecordSet[RecordSet.Count - 1].SelectivePlate,
				//    Path = new PropertyPath($"Note")
				//};
				//noteShowBinding = new Binding
				//{
				//    Mode = BindingMode.TwoWay,
				//    Source = RecordSet[RecordSet.Count - 1].SelectivePlate,
				//    Path = new PropertyPath($"NoteShow")
				//};
				//selective_plate.SetBinding(TextBox.TextProperty, binding);
				//selective_plate.SetBinding(BackgroundProperty, colorBinding);
				//sPlateNote.SetBinding(TextBox.TextProperty, noteBinding);
				//sPlateNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Selective Plate
				#region Wrap Cu
				wrap = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				wrapNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].WrapCu,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].WrapCu.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].WrapCu,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].WrapCu,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].WrapCu,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				wrap.SetBinding(TextBox.TextProperty, binding);
				wrap.SetBinding(BackgroundProperty, colorBinding);
				wrapNote.SetBinding(TextBox.TextProperty, noteBinding);
				wrapNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Wrap Cu
				#region Cap Cu
				cap = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				capNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].CapCu,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].CapCu.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].CapCu,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].CapCu,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].CapCu,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				cap.SetBinding(TextBox.TextProperty, binding);
				cap.SetBinding(BackgroundProperty, colorBinding);
				capNote.SetBinding(TextBox.TextProperty, noteBinding);
				capNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Cap Cu
				#region Internal Clad
				internal_clad = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				intCladNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalCladCu,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].InternalCladCu.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalCladCu,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalCladCu,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalCladCu,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				internal_clad.SetBinding(TextBox.TextProperty, binding);
				internal_clad.SetBinding(BackgroundProperty, colorBinding);
				intCladNote.SetBinding(TextBox.TextProperty, noteBinding);
				intCladNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Internal Clad
				#region Min Etchback
				min_etch = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				minEtchNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MinEtchback,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].MinEtchback.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MinEtchback,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MinEtchback,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MinEtchback,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				min_etch.SetBinding(TextBox.TextProperty, binding);
				min_etch.SetBinding(BackgroundProperty, colorBinding);
				minEtchNote.SetBinding(TextBox.TextProperty, noteBinding);
				minEtchNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Min Etchback
				#region Max Etchback
				max_etch = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				maxEtchNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MaxEtchback,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].MaxEtchback.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MaxEtchback,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MaxEtchback,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].MaxEtchback,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				max_etch.SetBinding(TextBox.TextProperty, binding);
				max_etch.SetBinding(BackgroundProperty, colorBinding);
				maxEtchNote.SetBinding(TextBox.TextProperty, noteBinding);
				maxEtchNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Max Etchback
				#region Internal Ring
				internal_ring = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				intRingNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalAnnularRing,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].InternalAnnularRing.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalAnnularRing,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalAnnularRing,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].InternalAnnularRing,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				internal_ring.SetBinding(TextBox.TextProperty, binding);
				internal_ring.SetBinding(BackgroundProperty, colorBinding);
				intRingNote.SetBinding(TextBox.TextProperty, noteBinding);
				intRingNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Internal Ring
				#region External Ring
				external_ring = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				extRingNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalAnnularRing,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].ExternalAnnularRing.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalAnnularRing,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalAnnularRing,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].ExternalAnnularRing,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				external_ring.SetBinding(TextBox.TextProperty, binding);
				external_ring.SetBinding(BackgroundProperty, colorBinding);
				extRingNote.SetBinding(TextBox.TextProperty, noteBinding);
				extRingNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion External Ring
				#region Dielectric
				dielectric = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu
				};
				diNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Dielectric,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].Dielectric.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Dielectric,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Dielectric,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Dielectric,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				dielectric.SetBinding(TextBox.TextProperty, binding);
				dielectric.SetBinding(BackgroundProperty, colorBinding);
				diNote.SetBinding(TextBox.TextProperty, noteBinding);
				diNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Dielectric
				#region Wicking
				wicking = new TextBox
				{
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					Padding = new Thickness(0, 0, 0, 5),
					VerticalContentAlignment = VerticalAlignment.Bottom,
					AcceptsReturn = true,
					FontSize = dataFontSize,
					Height = 48,
					ContextMenu = FindResource("measurement_menu") as ContextMenu,
					BorderThickness = new Thickness(1, 1, 3, 1)
				};
				wickNote = new TextBox()
				{
					Padding = new Thickness(-1, -2, -1, 0),
					Background = new SolidColorBrush(Colors.AliceBlue),
					FontSize = 10,
					VerticalAlignment = VerticalAlignment.Top,
					HorizontalAlignment = HorizontalAlignment.Right,
					MinWidth = 15,
					TextAlignment = TextAlignment.Center,
					Tag = "note"
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Wicking,
					Path = new PropertyPath($"Measurement"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				RecordSet[^1].Wicking.BackgroundColor = background;
				colorBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Wicking,
					Path = new PropertyPath($"BackgroundColor"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Wicking,
					Path = new PropertyPath($"Note"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				noteShowBinding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1].Wicking,
					Path = new PropertyPath($"NoteShow"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				wicking.SetBinding(TextBox.TextProperty, binding);
				wicking.SetBinding(BackgroundProperty, colorBinding);
				wickNote.SetBinding(TextBox.TextProperty, noteBinding);
				wickNote.SetBinding(VisibilityProperty, noteShowBinding);
				#endregion Wicking

				sn_input.LostFocus += new RoutedEventHandler(SN_Location_Change);
				loc_input.LostFocus += new RoutedEventHandler(SN_Location_Change);
				hole_plating.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				external_conductor.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				surface_clad.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				wrap.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				cap.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				internal_clad.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				min_etch.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				max_etch.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				internal_ring.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				external_ring.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				dielectric.LostFocus += new RoutedEventHandler(Measurement_LostFocus);
				wicking.LostFocus += new RoutedEventHandler(Measurement_LostFocus);

				#region Update Measurement Grid
				Grid.SetRow(info_container, row_num);
				Grid.SetColumn(info_container, 0);

				Grid.SetRow(hole_plating, row_num);
				Grid.SetColumn(hole_plating, 1);
				Grid.SetRow(hcpNote, row_num);
				Grid.SetColumn(hcpNote, 1);

				Grid.SetRow(external_conductor, row_num);
				Grid.SetColumn(external_conductor, 2);
				Grid.SetRow(extConNote, row_num);
				Grid.SetColumn(extConNote, 2);

				Grid.SetRow(surface_clad, row_num);
				Grid.SetColumn(surface_clad, 3);
				Grid.SetRow(sCladNote, row_num);
				Grid.SetColumn(sCladNote, 3);

				//Grid.SetRow(selective_plate, row_num);
				//Grid.SetColumn(selective_plate, 4);

				Grid.SetRow(wrap, row_num);
				Grid.SetColumn(wrap, 4);
				Grid.SetRow(wrapNote, row_num);
				Grid.SetColumn(wrapNote, 4);

				Grid.SetRow(cap, row_num);
				Grid.SetColumn(cap, 5);
				Grid.SetRow(capNote, row_num);
				Grid.SetColumn(capNote, 5);

				Grid.SetRow(internal_clad, row_num);
				Grid.SetColumn(internal_clad, 6);
				Grid.SetRow(intCladNote, row_num);
				Grid.SetColumn(intCladNote, 6);

				Grid.SetRow(min_etch, row_num);
				Grid.SetColumn(min_etch, 7);
				Grid.SetRow(minEtchNote, row_num);
				Grid.SetColumn(minEtchNote, 7);

				Grid.SetRow(max_etch, row_num);
				Grid.SetColumn(max_etch, 8);
				Grid.SetRow(maxEtchNote, row_num);
				Grid.SetColumn(maxEtchNote, 8);

				Grid.SetRow(internal_ring, row_num);
				Grid.SetColumn(internal_ring, 9);
				Grid.SetRow(intRingNote, row_num);
				Grid.SetColumn(intRingNote, 9);

				Grid.SetRow(external_ring, row_num);
				Grid.SetColumn(external_ring, 10);
				Grid.SetRow(extRingNote, row_num);
				Grid.SetColumn(extRingNote, 10);

				Grid.SetRow(dielectric, row_num);
				Grid.SetColumn(dielectric, 11);
				Grid.SetRow(diNote, row_num);
				Grid.SetColumn(diNote, 11);

				Grid.SetRow(wicking, row_num);
				Grid.SetColumn(wicking, 12);
				Grid.SetRow(wickNote, row_num);
				Grid.SetColumn(wickNote, 12);

				m_grid.Children.Add(info_container);
				m_grid.Children.Add(hole_plating);
				m_grid.Children.Add(hcpNote);
				m_grid.Children.Add(external_conductor);
				m_grid.Children.Add(extConNote);
				m_grid.Children.Add(surface_clad);
				m_grid.Children.Add(sCladNote);
				//m_grid.Children.Add(selective_plate);
				m_grid.Children.Add(wrap);
				m_grid.Children.Add(wrapNote);
				m_grid.Children.Add(cap);
				m_grid.Children.Add(capNote);
				m_grid.Children.Add(internal_clad);
				m_grid.Children.Add(intCladNote);
				m_grid.Children.Add(min_etch);
				m_grid.Children.Add(minEtchNote);
				m_grid.Children.Add(max_etch);
				m_grid.Children.Add(maxEtchNote);
				m_grid.Children.Add(internal_ring);
				m_grid.Children.Add(intRingNote);
				m_grid.Children.Add(external_ring);
				m_grid.Children.Add(extRingNote);
				m_grid.Children.Add(dielectric);
				m_grid.Children.Add(diNote);
				m_grid.Children.Add(wicking);
				m_grid.Children.Add(wickNote);
				#endregion Update Measurement Grid

				observation_row = new RowDefinition
				{
					Height = new GridLength(0, GridUnitType.Auto)
				};
				o_grid.RowDefinitions.Add(observation_row);

				innerlayer_sep = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					Height = 48,
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 1
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].InnerlayerSeparation"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				innerlayer_sep.SetBinding(TextBox.TextProperty, binding);

				plating_crack = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					Height = 48,
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 1
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].PlatingCrack"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				plating_crack.SetBinding(TextBox.TextProperty, binding);

				plating_void = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					Height = 48,
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 1
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].PlatingVoid"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				plating_void.SetBinding(TextBox.TextProperty, binding);

				//foil_crack = new TextBox()
				//{
				//    Background = background,
				//    TextWrapping = TextWrapping.Wrap,
				//    TextAlignment = TextAlignment.Center,
				//    VerticalContentAlignment = VerticalAlignment.Bottom,
				//    Padding = new Thickness(0, 0, 0, 5),
				//    FontSize = 14,
				//    Height = 48,
				//    CharacterCasing = CharacterCasing.Upper,
				//    MaxLength = 1
				//};
				//binding = new Binding
				//{
				//    Mode = BindingMode.TwoWay,
				//    Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].FoilCrack")
				//};
				//foil_crack.SetBinding(TextBox.TextProperty, binding);

				delam_blisters = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					Height = 48,
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 1
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].DelamBlisters"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				delam_blisters.SetBinding(TextBox.TextProperty, binding);

				lam_void = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					Height = 48,
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 1
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Path = new PropertyPath($"RecordSet[{RecordSet.Count - 1}].LaminateVoidCrack"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				lam_void.SetBinding(TextBox.TextProperty, binding);

				accept_reject = new TextBox()
				{
					Background = background,
					TextWrapping = TextWrapping.Wrap,
					TextAlignment = TextAlignment.Center,
					VerticalContentAlignment = VerticalAlignment.Bottom,
					Padding = new Thickness(0, 0, 0, 5),
					FontSize = obsFontSize,
					FontWeight = FontWeights.Medium,
					Height = 48,
					BorderBrush = new SolidColorBrush(Colors.Black),
					CharacterCasing = CharacterCasing.Upper,
					MaxLength = 2
				};
				binding = new Binding
				{
					Mode = BindingMode.TwoWay,
					Source = RecordSet[^1],
					Path = new PropertyPath($"AcceptReject"),
					UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
				};
				accept_reject.SetBinding(TextBox.TextProperty, binding);

				innerlayer_sep.TextChanged += new TextChangedEventHandler(Observation_Input);
				plating_crack.TextChanged += new TextChangedEventHandler(Observation_Input);
				plating_void.TextChanged += new TextChangedEventHandler(Observation_Input);
				//foil_crack.TextChanged += new TextChangedEventHandler(Observation_Input);
				delam_blisters.TextChanged += new TextChangedEventHandler(Observation_Input);
				lam_void.TextChanged += new TextChangedEventHandler(Observation_Input);
				accept_reject.TextChanged += new TextChangedEventHandler(Observation_Input);

				Grid.SetRow(innerlayer_sep, row_num);
				Grid.SetColumn(innerlayer_sep, 0);
				Grid.SetRow(plating_crack, row_num);
				Grid.SetColumn(plating_crack, 1);
				Grid.SetRow(plating_void, row_num);
				Grid.SetColumn(plating_void, 2);
				//Grid.SetRow(foil_crack, row_num);
				//Grid.SetColumn(foil_crack, 3);
				Grid.SetRow(delam_blisters, row_num);
				Grid.SetColumn(delam_blisters, 3);
				Grid.SetRow(lam_void, row_num);
				Grid.SetColumn(lam_void, 4);
				Grid.SetRow(accept_reject, row_num);
				Grid.SetColumn(accept_reject, 5);

				o_grid.Children.Add(innerlayer_sep);
				o_grid.Children.Add(plating_crack);
				o_grid.Children.Add(plating_void);
				//o_grid.Children.Add(foil_crack);
				o_grid.Children.Add(delam_blisters);
				o_grid.Children.Add(lam_void);
				o_grid.Children.Add(accept_reject);
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Add_Data_Row -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to add a new data row.\n{ex}", "Add Data Row ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// Remove data row from current structure set
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Remove_Data_Row(object sender, RoutedEventArgs e)
		{
			try
			{
				Button btn = (Button)sender;
				string tagStr = btn.Tag.ToString();
				int.TryParse(tagStr, out int tag);

				Grid m_grid = FindName(MGridNames[tag]) as Grid;
				Grid o_grid = FindName(OGridNames[tag]) as Grid;
				int total_rows = m_grid.RowDefinitions.Count;
				if (total_rows == 1)
					return;
				if (!IsOpening)
				{
					bool dataCheck = false;
					RecordSet.Where(record => record.Row == total_rows && record.StructureInfo.StructureOrder.Equals(tag + 1)).ToList()
						.ForEach(record =>
						{
							foreach (var item in record.GetType().GetProperties())
							{
								var currentRecord = item.GetValue(record, null);
								if (currentRecord is RecordGroup)
								{
									if (!string.IsNullOrEmpty((currentRecord as RecordGroup).Measurement) || !string.IsNullOrEmpty((currentRecord as RecordGroup).Note))
									{
										var dataCheckMsg = MessageBox.Show("The row you are attempting to remove contains data that will be lost. Would you like to continue?",
											"Remove Data Row", MessageBoxButton.YesNo, MessageBoxImage.Question);
										if (dataCheckMsg == MessageBoxResult.No)
										{
											dataCheck = true;
											return;
										}
										return;
									}
								}
							}
						});
					if (dataCheck) return;
				}
				m_grid.Children.RemoveRange(m_grid.Children.Count - 25, 25);

				//UIElement m_element;
				//for (int i = m_grid.Children.Count - 1; i >= 0; i--)
				//{
				//    m_element = m_grid.Children[i];
				//    if (Grid.GetRow(m_element) == total_rows - 1)
				//        m_grid.Children.Remove(m_element);
				//}
				UIElement o_element = null;
				for (int i = o_grid.Children.Count - 1; i >= 0; i--)
				{
					o_element = o_grid.Children[i];
					if (Grid.GetRow(o_element) == total_rows - 1)
						o_grid.Children.Remove(o_element);
				}

				// remove last two row definitions
				RowDefinition m_row = m_grid.RowDefinitions[total_rows - 1];
				m_grid.RowDefinitions.Remove(m_row);
				RowDefinition o_row = o_grid.RowDefinitions[total_rows - 1];
				o_grid.RowDefinitions.Remove(o_row);

				RecordSet.RemoveAll(record => record.StructureInfo.Equals(StructureTitles[tag]) && record.Row == total_rows);
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Remove_Data_Row -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to remove a data row.\n{ex}", "Remove Data Row ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// show or hide job information form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Toggle_Form(object sender, RoutedEventArgs e)
		{
			try
			{
				if (job_form.Visibility == Visibility.Visible)
				{
					arrow_img.Source = new BitmapImage(new Uri("Images/up_icon.png", UriKind.Relative));
					job_form.Visibility = Visibility.Collapsed;
					test_condition_grid.Visibility = Visibility.Collapsed;
				}
				else
				{
					arrow_img.Source = new BitmapImage(new Uri("Images/down_icon.png", UriKind.Relative));
					job_form.Visibility = Visibility.Visible;
					test_condition_grid.Visibility = Visibility.Visible;
				}
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Toggle_Form -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to toggle the job form.\n{ex}", "Toggle Form ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// determine whether job condition is As Received or Thermal Stress
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private async void Condition_Selected(object sender, SelectionChangedEventArgs e)
		{
			try
			{
				if (condition_input.SelectedIndex == 0)
				{
					thermal_info.Visibility = Visibility.Hidden;
					bake_info_sp.Visibility = Visibility.Hidden;
				}
				if (condition_input.SelectedIndex == 1)
				{
					thermal_info.Visibility = Visibility.Visible;
					bake_info_sp.Visibility = Visibility.Visible;
				}
				await Potential_Overwrite_Check();
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Condition_Selected -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to select the testing condition.\n{ex}", "Test Condition Selection ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async void Testing_Performed_On_Checked(object sender, RoutedEventArgs e)
		{
			await Potential_Overwrite_Check();
		}
		/// <summary>
		/// edit column width for specific requirements
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Requirements_Grid_Edit(object sender, RoutedEventArgs e)
		{
			try
			{
				var cm = FindResource("requirement_menu") as ContextMenu;
				TextBox req = cm.PlacementTarget as TextBox;
				int col = Grid.GetColumn(req);
				if (col == 3 || col == 5 || col == 6 || col == 8 || col == 10 || col == 11 || col == 12)
					return;
				var nextReq = requirements_grid.FindName($"req{col + 1}") as TextBox;

				//var property = col switch
				//{
				//    1 => Requirement.HoleCuPlating,
				//    2 => Requirement.SurfaceCuPlating,
				//    5 => Requirement.WrapCu,
				//    8 => Requirement.MinEtchback,
				//    10 => Requirement.InternalAnnularRing,
				//    _ => null
				//};

				int col_span = Grid.GetColumnSpan(req);
				if (col_span == 1/* && col != 3 && col != 6 && col != 7 && col != 9 && col != 11 && col != 12 && col != 13*/)
				{
					if (nextReq.Text != "")
					{
						MessageBox.Show("The next requirement must be empty before combining with current requirement.");
						return;
					}
					Grid.SetColumnSpan(req, 2);
					if (nextReq is TextBox)
					{
						nextReq.Visibility = Visibility.Collapsed;
						nextReq.Text = "collapsed";
					}
				}
				else if (col_span == 2)
				{
					Grid.SetColumnSpan(req, 1);
					if (nextReq is TextBox)
					{
						nextReq.Visibility = Visibility.Visible;
						if (nextReq.Text == "collapsed")
							nextReq.Text = "";
					}
				}

				var reqBinding = req.GetBindingExpression(TextBox.TextProperty);
				reqBinding.UpdateSource();
				var nextReqBinding = nextReq.GetBindingExpression(TextBox.TextProperty);
				nextReqBinding.UpdateSource();
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Requirements_Grid_Edit -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to edit the requirements grid.\n{ex}", "Requirements Grid Edit ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// Add/Remove row from remarks set
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Add_Remarks_Row(object sender, RoutedEventArgs e)
		{
			if (Remarks.Count > 0)
			{
				RemarkSet lastRemark = Remarks.Last();
				if (string.IsNullOrEmpty(lastRemark.Remark))
					return;
			}

			Binding textBinding, colorBinding;

			SolidColorBrush background;
			if (remarks_list.Children.Count % 2 == 0)
				background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
			else
				background = new SolidColorBrush(Colors.Transparent);

			TextBox remark = new TextBox()
			{
				Height = 25,
				Padding = new Thickness(2),
				FontSize = 14,
				Margin = new Thickness(0, 2, 0, 0),
				ContextMenu = FindResource("remark_menu") as ContextMenu
			};
			int remarkCount = Remarks.Count;
			Remarks.Add(new RemarkSet { Remark = string.Empty, Reject = false, Row = remarkCount + 1 });

			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = Remarks[remarkCount],
				Path = new PropertyPath("Remark"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = Remarks[remarkCount],
				Path = new PropertyPath("BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			remark.SetBinding(TextBox.TextProperty, textBinding);
			remark.SetBinding(BackgroundProperty, colorBinding);
			Remarks[remarkCount].BackgroundColor = background;
			Remarks[remarkCount].Remark = $"Remark {remarkCount + 1}";

			int curr_count = RemarkNames.Count;
			RemarkNames.Add($"remark_{curr_count}");
			remark.Name = RemarkNames[curr_count];
			RegisterName(RemarkNames[curr_count], remark);
			remarks_list.Children.Add(remark);
		}
		private void Remove_Remarks_Row(object sender, RoutedEventArgs e)
		{
			if (remarks_list.Children.Count == 0)
				return;
			int last_index = RemarkNames.Count - 1;
			if (!IsOpening)
			{
				if (!string.IsNullOrEmpty((remarks_list.Children[last_index] as TextBox).Text))
				{
					var dataCheckMsg = MessageBox.Show("The remark you are attempting to remove contains data that will be lost. Would you like to continue?",
						"Remove Remark", MessageBoxButton.YesNo, MessageBoxImage.Question);
					if (dataCheckMsg == MessageBoxResult.No) return;
				}
			}
			remarks_list.Children.Remove(remarks_list.Children[last_index]);
			UnregisterName(RemarkNames[last_index]);
			RemarkNames.RemoveAt(last_index);

			Remarks.RemoveAt(Remarks.Count - 1);
		}
		/// <summary>
		/// Determine whether current structure title conflicts with any other previous structure titles (same name)
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Structure_Title_LostFocus(object sender, RoutedEventArgs e)
		{
			TextBox current_structure_title_box = sender as TextBox;
			string current_box_name = current_structure_title_box.Name;
			string current_structure_title = current_structure_title_box.Text;
			if (current_structure_title == "" || StructureTitleNames.Count == 1)
				return;

			foreach (string structure_name in StructureTitleNames)
			{
				if (structure_name == current_box_name)
					continue;

				TextBox structure_title_box = FindName(structure_name) as TextBox;
				string structure_title = structure_title_box.Text;
				if (structure_title == current_structure_title)
				{
					MessageBox.Show("This structure title has already been created. Please edit this structure title and avoid duplicates.", "Duplicate Structure Title!", MessageBoxButton.OK, MessageBoxImage.Error);
					current_structure_title_box.Text += "***Duplicate***";
					var binding = current_structure_title_box.GetBindingExpression(TextBox.TextProperty);
					if (binding != null)
						binding.UpdateSource();
					return;
				}
			}
		}
		private void SN_Location_Change(object sender, RoutedEventArgs e)
		{
			TextBox textBox = sender as TextBox;
			var snLoc = RecordSet.Where(r => !string.IsNullOrEmpty(r.SerialNumber)).Select(r => new { r.StructureInfo.StructureOrder, r.SerialNumber, r.Location });
			int totalCount = snLoc.Count();
			int distinctCount = snLoc.Distinct().Count();
			if (totalCount != distinctCount)
			{
				DuplicateDataRow = true;
				MessageBox.Show("The current structure contains a duplicate SN / Location combo");
				textBox.Text = $"*{textBox.Text}*";
			}
			else DuplicateDataRow = false;
		}
		/// <summary>
		/// check measurement input and put in proper format
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Measurement_LostFocus(object sender, RoutedEventArgs e)
		{
			// get textbox to format
			TextBox box = sender as TextBox;
			Measurement_Format_Text(box);
			Check_Default_Mistype_List(box);
		}
		private void Check_Default_Mistype_List(TextBox textBox)
		{
			string boxContent = textBox.Text;
			if (string.IsNullOrEmpty(boxContent)) return;
			if (!(textBox.Parent is Grid)) return;
			int column = Grid.GetColumn(textBox);
			if (column < 1 || column > 12) return;
			string boxTag = "";
			if (textBox.Tag != null)
				boxTag = textBox.Tag.ToString();
			if (boxTag == "note") return;
			int index = column - 1;
			decimal? valueCheck = DefaultCheckList[index];
			if (valueCheck is null) return;
			if (boxContent.Contains("\n"))
            {
				string[] contentSplit = boxContent.Split("\n");
                foreach (var item in contentSplit)
				{
					if (decimal.TryParse(item, out decimal measurement))
					{
						if (measurement > valueCheck)
                        {
							if (MessageBox.Show($"The max value set for this column is \"{valueCheck:0.0000}\" and your current measurement is \"{measurement}\". Was this a mistype?",
								"Possible Mistype!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                            {
								textBox.BorderBrush = new SolidColorBrush(Colors.Red);
								return;
                            }
                        }
					}
				}
            }
			else
			{
				if (decimal.TryParse(boxContent, out decimal measurement))
                {
					if (measurement > valueCheck)
					{
						if (MessageBox.Show($"The max value set for this column is \"{valueCheck:0.0000}\" and your current measurement is \"{measurement}\". Was this a mistype?",
							"Possible Mistype!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
						{
							textBox.BorderBrush = new SolidColorBrush(Colors.OrangeRed);
						}
					}
				}
			}

		}
		private void Measurement_Format_Text(TextBox box)
		{
			try
			{
				bool isZoom = false;
				// get contents from textbox
				string contents = box.Text;
				string boxTag = "";
				if (box.Tag != null)
					boxTag = box.Tag.ToString();
				// get row and column of textbox
				int column = Grid.GetColumn(box);
				int row = Grid.GetRow(box);
				Record record;
				int structureOrder = 0;
				if (!(box.Parent is Grid mGrid)) return;

				if (mGrid.Name == "zoom_grid")
				{
					isZoom = true;
					string structureTitle = structureZoom.Text;
					string serialNumber = snZoom.Text;
					string location = locZoom.Text;
					record = RecordSet.Find(data => data.StructureInfo.StructureTitle == structureTitle && data.SerialNumber == serialNumber && data.Location == location);
				}
				else
				{
					int index = MGridNames.IndexOf(mGrid.Name);
					structureOrder = index + 1;
					record = RecordSet.Find(data => data.StructureInfo.StructureOrder == structureOrder && /*data.SerialNumber == serialNumber && data.Location == location*/ data.Row == row + 1);
				}

				if (record is null)
					return;

				// set border color back to normal (in case it was previously sed to red from decimal parse error) and set input string to new formatted value
				box.BorderBrush = new SolidColorBrush(Colors.DarkGray);

				RecordGroup property = column switch
				{
					1 => record.HoleCuPlating,
					2 => record.ExternalConductor,
					3 => record.SurfaceCladCu,
					4 => record.WrapCu,
					5 => record.CapCu,
					6 => record.InternalCladCu,
					7 => record.MinEtchback,
					8 => record.MaxEtchback,
					9 => record.InternalAnnularRing,
					10 => record.ExternalAnnularRing,
					11 => record.Dielectric,
					12 => record.Wicking,
					_ => null,
				};
				if (property is null)
					return;
				// get contents from textbox
				// if current measurement box has 'explicit_format' tag, the background has been set manually so skip auto formatting and reset tag
				if (boxTag == "explicit_format")
				{
					box.Tag = "";
					return;
				}
				if (contents == "")
				{
					if (record.Row % 2 != 0)
						property.BackgroundColor = new SolidColorBrush(Color.FromRgb(243, 243, 243));
					else
						property.BackgroundColor = new SolidColorBrush(Colors.Transparent);
					return;
				}
				// if contents of textbox is empty, set border and background to normal color (in case previously changed from error or reject)
				if (contents == "*" || contents == "N/A")
				{
					if (record.Row % 2 != 0)
						property.BackgroundColor = new SolidColorBrush(Color.FromRgb(243, 243, 243));
					else
						property.BackgroundColor = new SolidColorBrush(Colors.Transparent);
					if (isZoom) return;
					foreach (var childRecord in RecordSet.Where(r => r.StructureInfo.StructureOrder == structureOrder))
					{
						RecordGroup childProperty = column switch
						{
							1 => childRecord.HoleCuPlating,
							2 => childRecord.ExternalConductor,
							3 => childRecord.SurfaceCladCu,
							4 => childRecord.WrapCu,
							5 => childRecord.CapCu,
							6 => childRecord.InternalCladCu,
							7 => childRecord.MinEtchback,
							8 => childRecord.MaxEtchback,
							9 => childRecord.InternalAnnularRing,
							10 => childRecord.ExternalAnnularRing,
							11 => childRecord.Dielectric,
							12 => childRecord.Wicking,
							_ => null,
						};
						if (childProperty is null)
							return;
						childProperty.Measurement = string.IsNullOrEmpty(childProperty.Measurement) ? contents : childProperty.Measurement;
						//childProperty.Note = string.Empty;
					}
					return;
				}
				// if contents are just 0, keep it that way
				if (contents == "0")
				{

				}
				// if contents of textbox contains two lines, separate and format individually
				else if (contents.Contains("\n"))
				{
					// separate textbox contents at new line char and store in individual variables - upper, lower
					string upper, lower, new_upper = "", new_lower = "";
					string[] upper_lower = contents.Split('\n');
					upper = upper_lower[0];
					lower = upper_lower[1];
					// attempt to turn upper and lower input string into decimal format - set textbox border red if either fails
					if (!decimal.TryParse(upper, out decimal upper_num) || !decimal.TryParse(lower, out decimal lower_num))
					{
						box.BorderBrush = new SolidColorBrush(Colors.Red);
						return;
					}
					else
					{
						// check if either input string contains a '.' -- format converted number to four decimal places otherwise divide number by 10000
						if (upper.Contains("."))
						{
							if (upper.Length < 6)
								new_upper = string.Format("{0:0.0000}", upper_num);
							else
								new_upper = string.Format("{0}", upper_num);
						}
						else
							new_upper = string.Format("{0:0.0000}", upper_num / 10000);
						if (lower.Contains("."))
						{
							if (lower.Length < 6)
								new_lower = string.Format("{0:0.0000}", lower_num);
							else
								new_lower = string.Format("{0}", lower_num);
						}
						else
							new_lower = string.Format("{0:0.0000}", lower_num / 10000);
					}
					// set border color back to normal (in case it was previously sed to red from decimal parse error) and set input string to new formatted value
					//box.BorderBrush = new SolidColorBrush(Colors.DarkGray);
					box.Text = string.Format("{0}\n{1}", new_upper, new_lower);
				}
				// if contents of textbox contains single line, format input
				else
				{
					// attempt to parse string into decimal format - set border red if fails
					if (!decimal.TryParse(contents, out decimal num))
					{
						box.BorderBrush = new SolidColorBrush(Colors.Red);
						return;
					}
					// if input contains a '.' format number to four decimal places else divide number by 10000
					else if (contents.Contains("."))
					{
						if (contents.Length < 6)
							box.Text = string.Format("{0:0.0000}", num);
					}
					else
						box.Text = string.Format("{0:0.0000}", num / 10000);
				}
				Check_Measurement(box, column, record, property);

				var binding = box.GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to format the measurement.\n{ex}",
					"Measurement Format ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// TextChanged Event - accepting/rejecting particular observation
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Observation_Input(object sender, TextChangedEventArgs e)
		{
			try
			{
				TextBox box = sender as TextBox;
				int row = Grid.GetRow(box);
				int col = Grid.GetColumn(box);
				if (!(box.Parent is Grid parent)) return;

				if (col == parent.ColumnDefinitions.Count - 1)
				{
					if (box.Text == "A")
					{
						for (int i = 0; i < parent.ColumnDefinitions.Count - 1; i++)
						{
							TextBox observations = (TextBox)parent.Children.Cast<UIElement>().First(curr_box => Grid.GetRow(curr_box) == row && Grid.GetColumn(curr_box) == i);
							observations.Text = "A";
							if (row % 2 == 0)
								observations.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
							else
								observations.Background = new SolidColorBrush(Colors.Transparent);

							var obsBinding = observations.GetBindingExpression(TextBox.TextProperty);
							obsBinding.UpdateSource();
						}
						if (row % 2 == 0)
							box.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
						else
							box.Background = new SolidColorBrush(Colors.Transparent);
					}
					else if (box.Text == "R")
						box.Background = new SolidColorBrush(Colors.Yellow);
					else
					{
						if (row % 2 == 0)
							box.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
						else
							box.Background = new SolidColorBrush(Colors.Transparent);

						if (box.Text != "?" && box.Text != "*" && box.Text != "A*")
							box.Text = "";
					}
				}
				else
				{
					if (box.Text == "R")
					{
						box.Background = new SolidColorBrush(Colors.Yellow);

						TextBox accept_reject = (TextBox)parent.Children.Cast<UIElement>().First(curr_box => Grid.GetRow(curr_box) == row && Grid.GetColumn(curr_box) == 5);
						accept_reject.Text = "R";
						accept_reject.Background = new SolidColorBrush(Colors.Yellow);

						var arBinding = accept_reject.GetBindingExpression(TextBox.TextProperty);
						arBinding.UpdateSource();
					}
					else
					{
						if (row % 2 == 0)
							box.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
						else
							box.Background = new SolidColorBrush(Colors.Transparent);

						if (box.Text != "A" && box.Text != "?" && box.Text != "*")
							box.Text = "";
					}
				}

				var binding = box.GetBindingExpression(TextBox.TextProperty);
				binding.UpdateSource();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to format the observation input.\n{ex}",
					"Observation Format ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#region Requirement Check
		/// <summary>
		/// determine if current measurement box and observation needs to be formatted as a reject
		/// </summary>
		/// <param name="box"></param>
		private void Check_Measurement(TextBox box, int column, Record record, RecordGroup property)
		{
			try
			{
				if (box.Text == "" || box.Text == "N/A" || box.Text == "*")
					return;
				// check measurement of textbox against proper requirement
				int result = Check_Requirement(box, column, record);
				if (result == -1)
				{
					// if measurement fails set background of measurement textbox to yellow
					property.BackgroundColor = new SolidColorBrush(Colors.Yellow);
					record.AcceptReject = "R";

					//var binding = obs.GetBindingExpression(TextBox.TextProperty);
					//binding.UpdateSource();
				}
				else if (result == 1)
				{
					// if measurement does not fail, set measurement TextBox background back to original color
					if (record.Row % 2 != 0)
						box.Background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
					else
						box.Background = new SolidColorBrush(Colors.Transparent);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to check the measurement.\n{ex}",
					"Measurement Check ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// check current measurement box against proper requirement to determine accept or reject
		/// </summary>
		/// <param name="value_box"></param>
		/// <param name="col"></param>
		/// <returns></returns>
		private int Check_Requirement(TextBox box, int column, Record record)
		{
			try
			{
				int ret_val = 0;
				// get row of measurement box
				int row = Grid.GetRow(box);
				//int row = record.Row - 1;
				// get contents of measurement textbox and requirement textbox
				string measurement = box.Text;

				string requirement = "";
				TextBox req = (TextBox)requirements_grid.Children.Cast<UIElement>().First(curr_box => Grid.GetColumn(curr_box) == column);
				req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
				// check if requirement column is collapsed and set requirement to previous column TextBox
				if (req.Visibility == Visibility.Collapsed)
				{
					TextBox prev_req = (TextBox)requirements_grid.Children.Cast<UIElement>().First(curr_box => Grid.GetColumn(curr_box) == column - 1);
					requirement = prev_req.Text;
				}
				else
					requirement = req.Text;
				// if there is no requirement, set border to red and return
				if (requirement == "")
				{
					req.BorderBrush = new SolidColorBrush(Colors.Red);
					ret_val = 1;
				}
				else if (requirement.StartsWith("Layers:"))
				{
					return 0;
				}
				// if there are two measurements to only one requirement
				else if (measurement.Contains("\n") && !requirement.Contains("\n"))
				{
					ret_val = Requirement_6(box, req);
				}
				// first check is whether the current requirement is separated based on structure
				else if (Check_Structure_Requirement(requirement))
				{
					//MessageBox.Show("structure, requirement match");
					ret_val = Requirement_5(box, req, record);
				}
				// check if the requirement spans two columns (2x single (multi-col) measurement --> single (multi-col) requirement)
				else if ((Grid.GetColumnSpan(req) == 2/* && column == 2*/) || (req.Visibility == Visibility.Collapsed/* && column == 3*/))
					ret_val = Requirement_4(box, req, row, column);
				// check if measurement box contents and requirement box contents both have more than one line (double measurement --> double requirement)
				else if (measurement.Contains("\n") && requirement.Contains("\n"))
					ret_val = Requirement_3(box, req);
				// (single measurement --> double requirement)
				else if (!measurement.Contains("\n") && requirement.Contains("\n"))
				{
					ret_val = Requirement_2(box, req);
				}
				// (single measurement --> single requirement)
				else
					return Requirement_1(box, req);
				return ret_val;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to check the requirement.\n{ex}",
					"Requirement Check ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return 0;
			}
		}
		/// <summary>
		/// Use Structure Requirement View form to set requirements to each structure
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Set_Structure_Requirement(object sender, RoutedEventArgs e)
		{
			try
			{
				var cm = FindResource("requirement_menu") as ContextMenu;
				TextBox req = cm.PlacementTarget as TextBox;

				List<string> structureList = new List<string>();
				//_ = new List<(string, string)>();
				foreach (var structureInfo in StructureTitles)
				{
					if (structureInfo.StructureTitle == "" || !structureInfo.StructureTitle.Contains(";"))
					{
						MessageBox.Show("All structure titles must be set before adding requriements. Fill out any remaining structure titles and try again", "Incomplete Structure Titles", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					string fullStructure = structureInfo.StructureTitle;
					string[] fullStructureSplit = fullStructure.Split(';');
					string holeStructure = fullStructureSplit[1].Trim();
					structureList.Add(holeStructure);
				}
				structureList = structureList.Distinct().ToList();

				StructureRequirementsView win = new StructureRequirementsView(structureList);
				win.Set_Layout();
				win.ShowDialog();

				if (!win.OkClick)
					return;

				List<(string, string)> structureRequirement = win.StructureRequirement;
				//int col = Grid.GetColumn(req);
				//string structure_text = "";
				//string requirement_text = "";

				string completeRequirement = "";
				(string structure, string requirement) = structureRequirement[0];
				string structureText = structure;
				string requirementText = requirement;

				for (int i = 1; i < structureRequirement.Count; i++)
				{
					(structure, requirement) = structureRequirement[i];
					if (requirement == structureRequirement[i - 1].Item2)
					{
						structureText += $", {structure}";
					}
					else
					{
						completeRequirement += $"{structureText}: {requirementText}\n";
						structureText = structure;
						requirementText = requirement;
					}
				}
				completeRequirement += $"{structureText}: {requirementText}";
				req.Text = completeRequirement;

				var binding = req.GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to set structure requirement.\n{ex}",
					"Structure Requirement Set ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		/// <summary>
		/// Check whether the particular requirement is set per structure title
		/// </summary>
		private bool Check_Structure_Requirement(string requirement)
		{
			try
			{

				if (!requirement.Contains("\n"))
					return false;
				int index = requirement.IndexOf(':');
				if (index <= 0)
					return false;
				string first_req_struct = "";
				int index2 = requirement.IndexOf(',');
				if (index2 <= 0)
					first_req_struct = requirement.Substring(0, index);
				else
					first_req_struct = requirement.Substring(0, Math.Min(index, index2));

				foreach (var structure in StructureTitles)
				{
					//TextBox struct_box = FindName(struct_box_name) as TextBox;
					string structure_title = structure.StructureTitle;
					if (!structure_title.Contains(';')) return false;
					string[] struct_split = structure_title.Split(';');
					string current_struct_req = struct_split[1].Trim();

					if (first_req_struct == current_struct_req)
						return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to check for structure requirements.\n{ex}",
					"Structure Requirement Check ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
		}
		private int Requirement_6(TextBox box, TextBox req)
		{
			try
			{
				bool pass = false;

				string measurement = box.Text;
				string requirement = req.Text;
				requirement = requirement.Trim(' ');

				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);
				// separate measurements to check against correct requirement
				string[] mes1_mes2 = measurement.Split('\n');
				// loop through each measurement and requirement checking each upper and each lower
				for (int i = 0; i < 2; i++)
				{
					// get decimal format for current measurement
					decimal.TryParse(mes1_mes2[i], out decimal mes_num);
					string[] req_split = requirement.Split(' ');
					if (requirement.Contains(" min"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (mes_num < req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
					else if (requirement.Contains(" max"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (mes_num > req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{
				throw;
			}
		}
		/// <summary>
		/// Check current measurement against requirement associated with the same structure
		/// </summary>
		/// <param name="box"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		private int Requirement_5(TextBox box, TextBox req, Record record)
		{
			try
			{
				bool pass = false;
				// get structure of current measurement box
				string structureTitle = record.StructureInfo.StructureTitle;
				string[] structureTileSplit = structureTitle.Split(';');
				string measurementStructure = structureTileSplit[1].Trim(); // structure of current measurement

				decimal requirementNum = 0;
				// get measurement text
				string measurement = box.Text;
				decimal.TryParse(measurement, out decimal measurementNum);
				// create 2 tuple array of structure tilte and corresponding requirement
				List<(string, decimal)> structure_requirement = new List<(string, decimal)>();
				string requirement = req.Text;
				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);
				string[] requirement_set = requirement.Split('\n');
				// loop through each requirement to determine associated structure(s)
				foreach (string current_requirement in requirement_set)
				{
					// get structure(s) of particular requirement
					int index = current_requirement.IndexOf(':');
					string current_structures = current_requirement.Substring(0, index).Trim();
					// if current requirement has more than one associated structure, find correct one
					if (current_structures.Contains(","))
					{
						string[] structure_split = current_structures.Split(',');
						foreach (string structure in structure_split)
						{
							//structure_requirement.Add((structure, requirement_num));
							if (measurementStructure == structure.Trim())
							{
								if (current_requirement.Contains(" min"))
								{
									// get decimal number of current requirement
									string[] requirement_split = current_requirement.Split(' ');
									foreach (string word in requirement_split)
									{
										if (!decimal.TryParse(word, out requirementNum))
											req.BorderBrush = new SolidColorBrush(Colors.Red);
										else
										{
											req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
											if (measurementNum < requirementNum)
												return -1;
											else
												return 1;
											//break;
										}
									}
								}
								else if (current_requirement.Contains(" max"))
								{
									// get decimal number of current requirement
									string[] requirement_split = current_requirement.Split(' ');
									foreach (string word in requirement_split)
									{
										if (!decimal.TryParse(word, out requirementNum))
											req.BorderBrush = new SolidColorBrush(Colors.Red);
										else
										{
											req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
											if (measurementNum > requirementNum)
												return -1;
											else
												return 1;
											//break;
										}
									}
								}
							}
						}
					}
					// if there is one structure for the current requirement, check against measurement structure for a match
					else if (measurementStructure == current_structures)
					{
						if (current_requirement.Contains(" min"))
						{
							// get decimal number of current requirement
							string[] requirement_split = current_requirement.Split(' ');
							foreach (string word in requirement_split)
							{
								if (!decimal.TryParse(word, out requirementNum))
									req.BorderBrush = new SolidColorBrush(Colors.Red);
								else
								{
									req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
									if (measurementNum < requirementNum)
										return -1;
									else
										return 1;
									//    pass = true;
									//break;
								}
							}
						}
						else if (current_requirement.Contains(" max"))
						{
							// get decimal number of current requirement
							string[] requirement_split = current_requirement.Split(' ');
							foreach (string word in requirement_split)
							{
								if (!decimal.TryParse(word, out requirementNum))
									req.BorderBrush = new SolidColorBrush(Colors.Red);
								else
								{
									req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
									if (measurementNum > requirementNum)
										return -1;
									else
										return 1;
									//    pass = true;
									//break;
								}
							}
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// Check sum of multi-column measurements against combined two-column requirements
		/// (single (multi-column sum) measurement --> single (multi-column combination) requirement)
		/// </summary>
		/// <param name="box"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		private int Requirement_4(TextBox box, TextBox req, int row, int column)
		{
			try
			{
				bool pass = false;

				string measurement = box.Text;
				decimal.TryParse(measurement, out decimal curr_num);
				decimal additional_num = 0;
				string requirement = "";
				if (req.Visibility == Visibility.Collapsed)
				{
					TextBox prev_req = (TextBox)requirements_grid.Children.Cast<UIElement>().First(curr_box => Grid.GetColumn(curr_box) == column - 1);
					requirement = prev_req.Text;
				}
				else
					requirement = req.Text;
				requirement = requirement.Trim(' ');

				// if requirement has parenthesis, remove them
				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);
				if (requirement.Contains("\n"))
					requirement = requirement.Replace('\n', ' ');

				// get parent grid to get new child TextBox
				Grid m_grid = box.Parent as Grid;
				// determine whether you are in the first or second of the two columns and get appropriate measurement to add against current measurement column
				if (Grid.GetColumnSpan(req) == 2/* && column == 2*/)
				{
					// get measurement of the second column (same row, column + 1) or return until available
					TextBox next_measurement_box = (TextBox)m_grid.Children.Cast<UIElement>().First(curr_box => Grid.GetRow(curr_box) == row && Grid.GetColumn(curr_box) == column + 1);
					string next_measurement = next_measurement_box.Text;
					if (next_measurement == "")
						return 0;
					// get decimal format of each measurement and add them together
					decimal.TryParse(next_measurement, out additional_num);
				}
				else if (req.Visibility == Visibility.Collapsed/* && column == 3*/)
				{
					// get measurement of the first column (same row, column - 1) or return until available
					TextBox prev_measurement_box = (TextBox)m_grid.Children.Cast<UIElement>().First(curr_box => Grid.GetRow(curr_box) == row && Grid.GetColumn(curr_box) == column - 1);
					string prev_measurement = prev_measurement_box.Text;
					if (prev_measurement == "")
						return 0;
					// get decimal format of each measurement and add them together
					decimal.TryParse(prev_measurement, out additional_num);
				}
				decimal total_num = curr_num + additional_num;

				// split strings in requirement to look at individually
				requirement = requirement.Trim(' ');
				string[] req_split = requirement.Split(' ');
				// check whether requirement contains min or max to determine how to check against measurements
				if (requirement.Contains(" min"))
				{
					foreach (string word in req_split)
					{
						// find required measurement to try and parse into decimal format
						if (!decimal.TryParse(word, out decimal req_num))
							req.BorderBrush = new SolidColorBrush(Colors.Red);
						else
						{
							req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
							if (req_num >= 1)
								continue;
							if (total_num < req_num)
								return -1;
							else
								pass = true;
							break;
						}
					}
				}
				else if (requirement.Contains(" max"))
				{
					foreach (string word in req_split)
					{
						// find required measurement to try and parse into decimal format
						if (!decimal.TryParse(word, out decimal req_num))
							req.BorderBrush = new SolidColorBrush(Colors.Red);
						else
						{
							req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
							if (req_num >= 1)
								continue;
							if (total_num > req_num)
								return -1;
							else
								pass = true;
							break;
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// check current set of measurements against appropriate set of requirements.
		/// (double measurements --> double requirements)
		/// </summary>
		/// <param name="box"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		private int Requirement_3(TextBox box, TextBox req)
		{
			try
			{
				bool pass = false;

				string measurement = box.Text;
				string requirement = req.Text;
				requirement = requirement.Trim(' ');
				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);

				// separate measurements to check against correct requirement
				string[] mes1_mes2 = measurement.Split('\n');
				// separate requirements (each line)
				string[] line_split = requirement.Split('\n');
				// loop through each measurement and requirement checking each upper and each lower
				for (int i = 0; i < 2; i++)
				{
                    // get decimal format for current measurement
                    decimal.TryParse(mes1_mes2[i], out decimal mes_num);
					string[] req_split = line_split[i].Split(' ');
					if (line_split[i].Contains(" min"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (mes_num < req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
					else if (line_split[i].Contains(" max"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (mes_num > req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// check current measurement box against specified requirements.
		/// (single measurement --> double requirement)
		/// </summary>
		/// <param name="box"></param>
		/// <param name="req"></param>
		/// <returns></returns>
		private int Requirement_2(TextBox box, TextBox req)
		{
			try
			{
				bool pass = false;
				string measurement = box.Text;
				// checking requirement for single measurement
				decimal.TryParse(measurement, out decimal measurement_num);
				string requirement = req.Text;
				// split strings in requirement to look at individually
				requirement = requirement.Trim(' ');
				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);

				// separate each requirement (each line)
				string[] line_split = requirement.Split('\n');
				foreach (string line in line_split)
				{
					// for each requirement, extract the information by splitting it by each word
					string[] req_split = line.Split(' ');
					// determine whether requirement is a 'min' or 'max'
					if (line.Contains(" min"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (measurement_num < req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
					else if (line.Contains(" max"))
					{
						foreach (string word in req_split)
						{
							// find required measurement to try and parse into decimal format
							if (!decimal.TryParse(word, out decimal req_num))
								req.BorderBrush = new SolidColorBrush(Colors.Red);
							else
							{
								req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
								if (measurement_num > req_num)
									return -1;
								else
									pass = true;
								break;
							}
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{

				throw;
			}
		}
		/// <summary>
		/// check current measurement box against specified requriement.
		/// (single measurement --> single requirement)
		/// </summary>
		/// <param name="box"></param>
		/// <param name="column"></param>
		/// <returns></returns>
		private int Requirement_1(TextBox box, TextBox req)
		{
			try
			{
				bool pass = false;
				string measurement = box.Text;
				// checking requirement for single measurement
				decimal.TryParse(measurement, out decimal measurement_num);
				string requirement = req.Text;
				if (requirement.Contains("("))
					requirement = requirement.Remove(requirement.IndexOf('('), 1);
				if (requirement.Contains(")"))
					requirement = requirement.Remove(requirement.IndexOf(')'), 1);
				// split strings in requirement to look at individually
				requirement = requirement.Trim(' ');
				string[] req_split = requirement.Split(' ');
				// if the requirement does not contain a 'min' or 'max' the measurement will not be compared against it and no change will be made to either the measurement box or requirement box
				// check whether requirement contains min or max to determine how to check against measurements
				if (requirement.Contains(" min"))
				{
					foreach (string word in req_split)
					{
						// find required measurement to try and parse into decimal format
						if (!decimal.TryParse(word, out decimal req_num))
							req.BorderBrush = new SolidColorBrush(Colors.Red);
						else
						{
							req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
							if (measurement_num < req_num)
								return -1;
							else
								pass = true;
							break;
						}
					}
				}
				else if (requirement.Contains(" max"))
				{
					foreach (string word in req_split)
					{
						// find required measurement to try and parse into decimal format
						if (!decimal.TryParse(word, out decimal req_num))
							req.BorderBrush = new SolidColorBrush(Colors.Red);
						else
						{
							req.BorderBrush = new SolidColorBrush(Colors.DarkGray);
							if (measurement_num > req_num)
								return -1;
							else
								pass = true;
							break;
						}
					}
				}
				return pass ? 1 : 0;
			}
			catch (Exception)
			{

				throw;
			}
		}
		#endregion Requirement Check

		private void Measurement_Note_Edit(object sender, RoutedEventArgs e)
		{
			try
			{
				var cm = FindResource("measurement_menu") as ContextMenu;
				TextBox box = cm.PlacementTarget as TextBox;
				int row = Grid.GetRow(box);
				int column = Grid.GetColumn(box);
				Grid mGrid = box.Parent as Grid;
				Record record;
				string gridName = mGrid.Name;
				string serialNumber, location;

				if (gridName == "zoom_grid")
				{
					string structureTitle = structureZoom.Text;
					serialNumber = snZoom.Text;
					location = locZoom.Text;
					record = RecordSet.Find(data => data.StructureInfo.StructureTitle == structureTitle && data.SerialNumber == serialNumber && data.Location == location);
				}
				else
				{
					int index = MGridNames.IndexOf(gridName);
					int structureOrder = index + 1;

					TextBox loc = (((mGrid.Children.Cast<UIElement>().First(curr => Grid.GetRow(curr) == row && Grid.GetColumn(curr) == 0) as Border).Child as StackPanel).Children[0] as StackPanel).Children[1] as TextBox;
					location = loc.Text;
					TextBox sn = (((mGrid.Children.Cast<UIElement>().First(curr => Grid.GetRow(curr) == row && Grid.GetColumn(curr) == 0) as Border).Child as StackPanel).Children[1] as StackPanel).Children[1] as TextBox;
					serialNumber = sn.Text;

					record = RecordSet.Find(data => data.StructureInfo.StructureOrder == structureOrder && data.SerialNumber == serialNumber && data.Location == location);
				}
				if (record == null)
					return;
				RecordGroup property = column switch
				{
					1 => record.HoleCuPlating,
					2 => record.ExternalConductor,
					3 => record.SurfaceCladCu,
					//case 4:
					//    property = record.SelectivePlate;
					//    break;
					4 => record.WrapCu,
					5 => record.CapCu,
					6 => record.InternalCladCu,
					7 => record.MinEtchback,
					8 => record.MaxEtchback,
					9 => record.InternalAnnularRing,
					10 => record.ExternalAnnularRing,
					11 => record.Dielectric,
					12 => record.Wicking,
					_ => null,
				};
				if (property is null)
					return;
				// get children of Grid matching current row and column
				var boxes = mGrid.Children.Cast<UIElement>().Where(element => Grid.GetRow(element) == row && Grid.GetColumn(element) == column).ToList();
				foreach (TextBox tBox in boxes)
				{
					if (tBox.Tag != null && tBox.Tag.Equals("note"))
					{
						if (property.NoteShow == Visibility.Hidden)
						{
							property.NoteShow = Visibility.Visible;
							tBox.Focus();
						}
						else
						{
							property.Note = string.Empty;
							property.NoteShow = Visibility.Hidden;
						}
						//MessageBox.Show($"{RecordSet[0].HoleCuPlating.Note}\n{RecordSet[0].HoleCuPlating.NoteShow}");
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to toggle the measurement note.\n{ex}",
					"Measurement Note ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void Measurement_Format_Background(object sender, RoutedEventArgs e)
		{
			try
			{
				var menu = sender as MenuItem;
				//MessageBox.Show(menu.Header.ToString());
				string menu_tag = menu.Tag.ToString();

				var cm = FindResource("measurement_menu") as ContextMenu;
				TextBox box = cm.PlacementTarget as TextBox;
				int row = Grid.GetRow(box);
				int column = Grid.GetColumn(box);
				Grid mGrid = box.Parent as Grid;
				Record record;
				string gridName = mGrid.Name;
				string serialNumber, location;

				if (gridName == "zoom_grid")
				{
					string structureTitle = structureZoom.Text;
					serialNumber = snZoom.Text;
					location = locZoom.Text;
					record = RecordSet.Find(data => data.StructureInfo.StructureTitle == structureTitle && data.SerialNumber == serialNumber && data.Location == location);
				}
				else
				{
					int index = MGridNames.IndexOf(gridName);
					int structureOrder = index + 1;

					TextBox loc = (((mGrid.Children.Cast<UIElement>().First(curr => Grid.GetRow(curr) == row && Grid.GetColumn(curr) == 0) as Border).Child as StackPanel).Children[0] as StackPanel).Children[1] as TextBox;
					location = loc.Text;
					TextBox sn = (((mGrid.Children.Cast<UIElement>().First(curr => Grid.GetRow(curr) == row && Grid.GetColumn(curr) == 0) as Border).Child as StackPanel).Children[1] as StackPanel).Children[1] as TextBox;
					serialNumber = sn.Text;

					record = RecordSet.Find(data => data.StructureInfo.StructureOrder == structureOrder && data.SerialNumber == serialNumber && data.Location == location);
				}
				if (record == null)
					return;
				RecordGroup property = column switch
				{
					1 => record.HoleCuPlating,
					2 => record.ExternalConductor,
					3 => record.SurfaceCladCu,
					4 => record.WrapCu,
					5 => record.CapCu,
					6 => record.InternalCladCu,
					7 => record.MinEtchback,
					8 => record.MaxEtchback,
					9 => record.InternalAnnularRing,
					10 => record.ExternalAnnularRing,
					11 => record.Dielectric,
					12 => record.Wicking,
					_ => null,
				};
				if (property is null)
					return;

				box.Tag = "explicit_format";
				switch (menu_tag)
				{
					case "yellow_background":
						property.BackgroundColor = new SolidColorBrush(Colors.Yellow);
						record.AcceptReject = "R";
						break;
					case "standard_background":
						if (record.Row % 2 != 0)
							property.BackgroundColor = new SolidColorBrush(Color.FromRgb(243, 243, 243));
						else
							property.BackgroundColor = new SolidColorBrush(Colors.Transparent);
						break;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to format the measurement background.\n{ex}",
					"Measurement Format Background ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void Requirement_LostFocus(object sender, RoutedEventArgs e)
		{
			try
			{
				TextBox textBox = sender as TextBox;
				textBox.BorderBrush = new SolidColorBrush(Colors.DarkGray);
				int column = Grid.GetColumn(textBox);
				foreach (var mGridName in MGridNames)
				{
					Grid mGrid = FindName(mGridName) as Grid;
					foreach (var child in mGrid.Children)
					{
						if (child is Border)
							continue;
						string boxTag = "";
						if ((child as TextBox).Tag != null)
							boxTag = (child as TextBox).Tag.ToString();
						int childCol = Grid.GetColumn(child as TextBox);
						if (childCol == column & boxTag != "note" && boxTag != "explicit_format")
							Measurement_Format_Text(child as TextBox);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to measurement rows for current requirement.\n{ex}",
					"Requirement Check ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void Remark_Format_Background(object sender, RoutedEventArgs e)
		{
			try
			{
				var menu = sender as MenuItem;
				//MessageBox.Show(menu.Header.ToString());
				string menu_tag = menu.Tag.ToString();

				var cm = FindResource("remark_menu") as ContextMenu;
				TextBox box = cm.PlacementTarget as TextBox;
				//string name = box.Name;
				int row = RemarkNames.IndexOf(box.Name) + 1;

				RemarkSet remark = Remarks.Find(r => r.Row == row);

				switch (menu_tag)
				{
					case "yellow_background":
						// set background to yellow indicating a 'reject' and mark 'R'
						remark.BackgroundColor = new SolidColorBrush(Colors.Yellow);
						break;
					case "standard_background":
						if (row % 2 != 0)
							remark.BackgroundColor = new SolidColorBrush(Color.FromRgb(243, 243, 243));
						else
							remark.BackgroundColor = new SolidColorBrush(Colors.Transparent);
						break;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to format the remark background.\n{ex}",
					"Remark Format Background ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		#endregion UI Format Events

		#region Populate Elements Events
		private async Task Load_Recent_Jobs()
		{
			try
			{
				await using var db = new DigitalDatasheetContext();
				var recentJobs = db.JobForms.Where(r => r.LastSaved >= DateTime.Now.AddDays(-14))
					.OrderByDescending(r => r.LastSaved)
					.Select(r => new { r.WorkOrderNumber, r.TestPerformedOn, r.TestCondition, r.Customer });

				recent_jobs_menu.Items.Clear();
				MenuItem menuItem;
				foreach (var job in recentJobs)
				{
					string abrTestCondition = job.TestCondition == "As Received" ? "AR" : "TS";
					string jobDisplay = $"{job.WorkOrderNumber} - {job.Customer} - {abrTestCondition} - {job.TestPerformedOn}";

					menuItem = new MenuItem
					{
						Header = jobDisplay,
						Background = new SolidColorBrush(Colors.White),
						BorderThickness = new Thickness(0),
						Padding = new Thickness(0, 6, 0, 6),
						Tag = "recent"
					};
					menuItem.Click += new RoutedEventHandler(Open_Menu_Click);
					recent_jobs_menu.Items.Add(menuItem);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to recent work orders.\n{ex}",
					"Recent Job ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async Task Get_Customers()
		{
			try
			{
				var customers = await new AccessDb().GetCustomers();
				foreach (var customer in customers)
					customer_input.Items.Add(customer);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to retrieve the customer list.\n{ex}",
					"Customer Retrieval ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async Task Get_Specification()
		{
			try
			{
				await using (var db = new DigitalDatasheetContext())
				{
					var specifications = db.SpecificationRequirementsTable.OrderBy(s => s.Specification).Select(s => new { s.Specification });
					foreach (var spec in specifications)
					{
						spec1_input.Items.Add(spec.Specification);
						spec2_input.Items.Add(spec.Specification);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to retrieve the specification list.\n{ex}",
					"Specification Retrieval ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async void Get_Spec_Requirements(object sender, RoutedEventArgs e)
		{
			try
			{
				if (string.IsNullOrEmpty(FormSet.Specification1))
				{
					MessageBox.Show($"First specification must be selected to auto-fill requirements.", "No Specification Selected", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					return;
				}
				if (!NetConn)
					return;
				await using (var db = new DigitalDatasheetContext())
				{
					var spec1Requirements = await db.SpecificationRequirementsTable.FindAsync(FormSet.Specification1);
					if (spec1Requirements == null)
						MessageBox.Show($"{FormSet.Specification1} specification not found. Only specifications selected from dropdown list can be used to auto-fill requirements.", "Specification Not Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					else
					{
						Requirement.HoleCuPlating = string.IsNullOrEmpty(Requirement.HoleCuPlating) ? spec1Requirements.HoleCuPlating : Requirement.HoleCuPlating;
						Requirement.WrapCu = string.IsNullOrEmpty(Requirement.WrapCu) ? spec1Requirements.WrapCu : Requirement.WrapCu;
						Requirement.CapCu = string.IsNullOrEmpty(Requirement.CapCu) ? spec1Requirements.CapCu : Requirement.CapCu;
						Requirement.MinEtchback = string.IsNullOrEmpty(Requirement.MinEtchback) ? spec1Requirements.MinEtchback : Requirement.MinEtchback;
						Requirement.MaxEtchback = string.IsNullOrEmpty(Requirement.MaxEtchback) ? spec1Requirements.MaxEtchback : Requirement.MaxEtchback;
						Requirement.InternalAnnularRing = string.IsNullOrEmpty(Requirement.InternalAnnularRing) ? spec1Requirements.InternalAnnularRing : Requirement.InternalAnnularRing;
						Requirement.ExternalAnnularRing = string.IsNullOrEmpty(Requirement.ExternalAnnularRing) ? spec1Requirements.ExternalAnnularRing : Requirement.ExternalAnnularRing;
						Requirement.Dielectric = string.IsNullOrEmpty(Requirement.Dielectric) ? spec1Requirements.Dielectric : Requirement.Dielectric;
						if (spec1Requirements.WickingNote)
						{
							if (!string.IsNullOrEmpty(Requirement.MaxEtchback) && !string.IsNullOrEmpty(spec1Requirements.Wicking))
							{
								string maxEtch = Requirement.MaxEtchback.Split(' ')[0].Trim();
								string wicking = spec1Requirements.Wicking.Split(' ')[0].Trim();

								if (!decimal.TryParse(maxEtch, out decimal maxEtchVal))
									return;
								if (!decimal.TryParse(wicking, out decimal wickingVal))
									return;
								decimal totalWicking = maxEtchVal + wickingVal;

								Requirement.Wicking = string.IsNullOrEmpty(Requirement.Wicking) ? $"{totalWicking:G29} max" : Requirement.Wicking;
							}
						}
						else
							Requirement.Wicking = string.IsNullOrEmpty(Requirement.Wicking) ? spec1Requirements.Wicking : Requirement.Wicking;
					}

					if (string.IsNullOrEmpty(FormSet.Specification2)) return;
					var spec2Requirements = await db.SpecificationRequirementsTable.FindAsync(FormSet.Specification2);
					if (spec2Requirements == null)
						MessageBox.Show($"{FormSet.Specification2} specification not found. Only specifications selected from dropdown list can be used to auto-fill requirements.", "Specification Not Found", MessageBoxButton.OK, MessageBoxImage.Exclamation);
					else
					{
						Requirement.HoleCuPlating = string.IsNullOrEmpty(Requirement.HoleCuPlating) ? spec2Requirements.HoleCuPlating : Requirement.HoleCuPlating;
						Requirement.WrapCu = string.IsNullOrEmpty(Requirement.WrapCu) ? spec2Requirements.WrapCu : Requirement.WrapCu;
						Requirement.CapCu = string.IsNullOrEmpty(Requirement.CapCu) ? spec2Requirements.CapCu : Requirement.CapCu;
						Requirement.MinEtchback = string.IsNullOrEmpty(Requirement.MinEtchback) ? spec2Requirements.MinEtchback : Requirement.MinEtchback;
						Requirement.MaxEtchback = string.IsNullOrEmpty(Requirement.MaxEtchback) ? spec2Requirements.MaxEtchback : Requirement.MaxEtchback;
						Requirement.InternalAnnularRing = string.IsNullOrEmpty(Requirement.InternalAnnularRing) ? spec2Requirements.InternalAnnularRing : Requirement.InternalAnnularRing;
						Requirement.ExternalAnnularRing = string.IsNullOrEmpty(Requirement.ExternalAnnularRing) ? spec2Requirements.ExternalAnnularRing : Requirement.ExternalAnnularRing;
						Requirement.Dielectric = string.IsNullOrEmpty(Requirement.Dielectric) ? spec2Requirements.Dielectric : Requirement.Dielectric;
						if (spec2Requirements.WickingNote)
						{
							if (!string.IsNullOrEmpty(Requirement.MaxEtchback) && !string.IsNullOrEmpty(spec2Requirements.Wicking))
							{
								string maxEtch = Requirement.MaxEtchback.Split(' ')[0].Trim();
								string wicking = spec2Requirements.Wicking.Split(' ')[0].Trim();

								if (!decimal.TryParse(maxEtch, out decimal maxEtchVal))
									return;
								if (!decimal.TryParse(wicking, out decimal wickingVal))
									return;
								decimal totalWicking = maxEtchVal + wickingVal;

								Requirement.Wicking = string.IsNullOrEmpty(Requirement.Wicking) ? $"{totalWicking:G29} max" : Requirement.Wicking;
							}
						}
						else
							Requirement.Wicking = string.IsNullOrEmpty(Requirement.Wicking) ? spec2Requirements.Wicking : Requirement.Wicking;
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to auto-fill requirements.\n{ex}",
					"Requirements Auto-Fill ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		#endregion Populate Element Events

		#region Measurement Zoom
		private void MeasurementRow_Zoom_Click(object sender, RoutedEventArgs e)
		{
			var emptySNRecord = RecordSet.Find(record => string.IsNullOrEmpty(record.SerialNumber));
			if (emptySNRecord != null)
            {
				MessageBox.Show("All Serial Numbers must be filled out before opening the zoom window.");
				return;
            }

			zoom_canvas.Visibility = Visibility.Visible;
			complete_grid.Opacity = 0.4;
			complete_grid.IsEnabled = false;
			toggle_form_btn.IsEnabled = false;
			if (job_form.Visibility == Visibility.Visible)
			{
				arrow_img.Source = new BitmapImage(new Uri("Images/up_icon.png", UriKind.Relative));
				job_form.Visibility = Visibility.Collapsed;
				test_condition_grid.Visibility = Visibility.Collapsed;
			}
			ZoomWinOpen = true;

			var cm = FindResource("measurement_menu") as ContextMenu;
			TextBox textBox = cm.PlacementTarget as TextBox;

			MeasurementRow_Zoom_Create(textBox: textBox);
		}
		private void MeasurementRow_Zoom_Create(TextBox textBox = null, int direction = 0)
		{
			Record record = null;
			if (textBox == null && direction != 0)
			{
				string structureTitle = structureZoom.Text;
				string serialNumber = snZoom.Text;
				string location = locZoom.Text;
				Record currentRecord = RecordSet.Find(data => data.StructureInfo.StructureTitle == structureTitle && data.SerialNumber == serialNumber && data.Location == location);
				if (currentRecord == null) return;
				if (direction == 1)
				{
					record = RecordSet.Find(r => r.StructureInfo.StructureTitle == structureTitle && r.Row == currentRecord.Row + 1);
					if (record == null)
						record = RecordSet.Find(r => r.StructureInfo.StructureOrder == currentRecord.StructureInfo.StructureOrder + 1 && r.Row == 1);
					if (record == null) return;
				}
				else if (direction == -1)
				{
					record = RecordSet.Find(r => r.StructureInfo.StructureTitle == structureTitle && r.Row == currentRecord.Row - 1);
					if (record == null)
					{
						record = RecordSet.Find(r => r.StructureInfo.StructureOrder == currentRecord.StructureInfo.StructureOrder - 1);
						if (record == null) return;
						record = RecordSet.Where(r => r.StructureInfo.StructureOrder == currentRecord.StructureInfo.StructureOrder - 1).OrderBy(r => r.Row).Last();
					}
					if (record == null) return;
				}
			}
			else
			{
				int recordRow = Grid.GetRow(textBox) + 1;
				int column = Grid.GetColumn(textBox);

				if (zoom_grid.Children.Cast<UIElement>().First(element => Grid.GetColumn(element) == column && Grid.GetRow(element) == 2) is TextBox zoomBox)
					zoomBox.Focus();

				Grid mGrid = textBox.Parent as Grid;
				if (mGrid.Name == "zoom_grid") return;
				int structureOrder = MGridNames.IndexOf(mGrid.Name) + 1;
				record = RecordSet.Find(r => r.StructureInfo.StructureOrder == structureOrder && r.Row == recordRow);
			}


			Binding textBinding, colorBinding, noteBinding, noteShowBinding;

			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.StructureInfo,
				Path = new PropertyPath($"StructureTitle")
			};
			structureZoom.SetBinding(TextBlock.TextProperty, textBinding);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record,
				Path = new PropertyPath($"Location")
			};
			locZoom.SetBinding(TextBlock.TextProperty, textBinding);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record,
				Path = new PropertyPath($"SerialNumber")
			};
			snZoom.SetBinding(TextBlock.TextProperty, textBinding);

			#region Hole Cu Plating
			hcpZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.HoleCuPlating,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.HoleCuPlating,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.HoleCuPlating,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.HoleCuPlating,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			hcpZoom.SetBinding(TextBox.TextProperty, textBinding);
			hcpZoom.SetBinding(BackgroundProperty, colorBinding);
			hcpNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			hcpNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Hole Cu Plating
			#region External Conductor Thickness
			extConZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalConductor,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalConductor,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalConductor,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalConductor,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			extConZoom.SetBinding(TextBox.TextProperty, textBinding);
			extConZoom.SetBinding(BackgroundProperty, colorBinding);
			extConNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			extConNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion External Conductor Thickness
			#region Surface Clad Cu
			sCladZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.SurfaceCladCu,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.SurfaceCladCu,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.SurfaceCladCu,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.SurfaceCladCu,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			sCladZoom.SetBinding(TextBox.TextProperty, textBinding);
			sCladZoom.SetBinding(BackgroundProperty, colorBinding);
			sCladNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			sCladNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion  Surface Clad Cu
			#region Selective Plate
			//textBinding = new Binding
			//{
			//    Mode = BindingMode.TwoWay,
			//    Source = record.SelectivePlate,
			//    Path = new PropertyPath($"Measurement")
			//};
			//colorBinding = new Binding
			//{
			//    Mode = BindingMode.TwoWay,
			//    Source = record.SelectivePlate,
			//    Path = new PropertyPath($"BackgroundColor")
			//};
			//noteBinding = new Binding
			//{
			//    Mode = BindingMode.TwoWay,
			//    Source = record.SelectivePlate,
			//    Path = new PropertyPath($"Note")
			//};
			//noteShowBinding = new Binding
			//{
			//    Mode = BindingMode.TwoWay,
			//    Source = record.SelectivePlate,
			//    Path = new PropertyPath($"NoteShow")
			//};
			//selPlateZoom.SetBinding(TextBox.TextProperty, textBinding);
			//selPlateZoom.SetBinding(BackgroundProperty, colorBinding);
			//selPlateNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			//selPlateNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Selective Plate
			#region Wrap Cu
			wrapZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.WrapCu,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.WrapCu,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.WrapCu,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.WrapCu,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			wrapZoom.SetBinding(TextBox.TextProperty, textBinding);
			wrapZoom.SetBinding(BackgroundProperty, colorBinding);
			wrapNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			wrapNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Wrap Cu
			#region Cap Cu
			capZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.CapCu,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.CapCu,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.CapCu,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.CapCu,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			capZoom.SetBinding(TextBox.TextProperty, textBinding);
			capZoom.SetBinding(BackgroundProperty, colorBinding);
			capNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			capNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Cap Cu
			#region Internal Clad
			iCladZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalCladCu,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalCladCu,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalCladCu,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalCladCu,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			iCladZoom.SetBinding(TextBox.TextProperty, textBinding);
			iCladZoom.SetBinding(BackgroundProperty, colorBinding);
			iCladNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			iCladNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Internal Clad
			#region Min Etchback
			minEtchZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MinEtchback,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MinEtchback,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MinEtchback,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MinEtchback,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			minEtchZoom.SetBinding(TextBox.TextProperty, textBinding);
			minEtchZoom.SetBinding(BackgroundProperty, colorBinding);
			minEtchNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			minEtchNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Min Etchback
			#region Max Etchback
			maxEtchZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MaxEtchback,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MaxEtchback,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MaxEtchback,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.MaxEtchback,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			maxEtchZoom.SetBinding(TextBox.TextProperty, textBinding);
			maxEtchZoom.SetBinding(BackgroundProperty, colorBinding);
			maxEtchNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			maxEtchNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Max Etchback
			#region Internal Ring
			iRingZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalAnnularRing,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalAnnularRing,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalAnnularRing,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.InternalAnnularRing,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			iRingZoom.SetBinding(TextBox.TextProperty, textBinding);
			iRingZoom.SetBinding(BackgroundProperty, colorBinding);
			iRingNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			iRingNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Internal Ring
			#region External Ring
			eRingZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalAnnularRing,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalAnnularRing,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalAnnularRing,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.ExternalAnnularRing,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			eRingZoom.SetBinding(TextBox.TextProperty, textBinding);
			eRingZoom.SetBinding(BackgroundProperty, colorBinding);
			eRingNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			eRingNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion External Ring
			#region Dielectric
			diZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Dielectric,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Dielectric,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Dielectric,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Dielectric,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			diZoom.SetBinding(TextBox.TextProperty, textBinding);
			diZoom.SetBinding(BackgroundProperty, colorBinding);
			diNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			diNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Dielectric
			#region Wicking
			wickZoom.BorderBrush = new SolidColorBrush(Colors.DarkGray);
			textBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Wicking,
				Path = new PropertyPath($"Measurement"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			colorBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Wicking,
				Path = new PropertyPath($"BackgroundColor"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Wicking,
				Path = new PropertyPath($"Note"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			noteShowBinding = new Binding
			{
				Mode = BindingMode.TwoWay,
				Source = record.Wicking,
				Path = new PropertyPath($"NoteShow"),
				UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
			};
			wickZoom.SetBinding(TextBox.TextProperty, textBinding);
			wickZoom.SetBinding(BackgroundProperty, colorBinding);
			wickNoteZoom.SetBinding(TextBox.TextProperty, noteBinding);
			wickNoteZoom.SetBinding(VisibilityProperty, noteShowBinding);
			#endregion Wicking
		}
		private async void MeasurementRow_Zoom_Close_Click(object sender, RoutedEventArgs e)
		{
			zoom_canvas.Visibility = Visibility.Collapsed;
			complete_grid.Opacity = 1;
			complete_grid.IsEnabled = true;
			toggle_form_btn.IsEnabled = true;
			ZoomWinOpen = false;

			if (AutoSave && (UnsavedChanges = await Unsaved_Changes_CheckAsync())) await Save_Job();
		}
		private async void Next_Zoom_Click(object sender, RoutedEventArgs e)
		{
			MeasurementRow_Zoom_Create(direction: 1);
			if (AutoSave && (UnsavedChanges = await Unsaved_Changes_CheckAsync())) await Save_Job();
		}
		private async void Prev_Zoom_Click(object sender, RoutedEventArgs e)
		{
			MeasurementRow_Zoom_Create(direction: -1);
			if (AutoSave && (UnsavedChanges = await Unsaved_Changes_CheckAsync())) await Save_Job();
		}
		#endregion Measurement Zoom

		private static bool Is_Network_Available()
		{
			if (!NetworkInterface.GetIsNetworkAvailable())
				return false;
			foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
			{
				// discard because of standard reasons
				if ((ni.OperationalStatus != OperationalStatus.Up) ||
					(ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
					(ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel))
					continue;

				// this allow to filter modems, serial, etc.
				// I use 10000000 as a minimum speed for most cases
				if (ni.Speed < 10000000)
					continue;

				// discard virtual cards (virtual box, virtual pc, etc.)
				if ((ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
					(ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
					continue;

				// discard "Microsoft Loopback Adapter", it will not show as NetworkInterfaceType.Loopback but as Ethernet Card.
				if (ni.Description.Equals("Microsoft Loopback Adapter", StringComparison.OrdinalIgnoreCase))
					continue;

				return true;
			}
			return false;
		}
		private async void Check_Work_Order_Log(object sender, TextChangedEventArgs e)
		{
			if (!NetConn)
			{
				//MessageBox.Show("No network connection");
				wo_check_icon.Source = new BitmapImage(new Uri("../Images/error_icon.png", UriKind.Relative));
				wo_check_icon.Visibility = Visibility.Visible;
				return;
			}
			try
			{
				Regex regex = new Regex(@"^[0-9]*$");
				if (!regex.IsMatch(wo_number_input.Text) || !regex.IsMatch(wo_number_dash_input.Text))
				{
					WoCheck = false;
					wo_check_icon.Source = new BitmapImage(new Uri("../Images/error_icon.png", UriKind.Relative));
					wo_check_icon.Visibility = Visibility.Visible;
					return;
				}
				if (wo_number_input.Text == "")
				{
					WoCheck = false;
					wo_check_icon.Visibility = Visibility.Hidden;
					return;
				}
				WoCheck = true;
				await Potential_Overwrite_Check();
			}
			catch (Exception ex)
			{
				//error_log_sw = new StreamWriter(error_log_file_path, true);
				//error_log_sw.WriteLine($"{DateTime.Now.ToShortTimeString()}\nMainWindow Check_Work_Order_Log -- {err.Source}; {err.TargetSite}\n{err.Message}\n");
				//error_log_sw.Close();
				MessageBox.Show($"An error has occurred while attemping to check the work order log.\n{ex}",
					"Work Order Log ERROR", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async void WorkOrder_LostFocus(object sender, RoutedEventArgs e)
		{
			await Check_DueDate();
		}
		private async Task Check_DueDate()
		{
			var (dueDate, expedite) = await new AccessDb().GetDueDateInfo(FormSet.FullWorkOrder);
			if (dueDate.HasValue)
			{
				DueDateInfoLabel = $"Due Date: {dueDate.Value.ToShortDateString()}";
				DueDateInfoColor = new SolidColorBrush(Colors.Black);
				ExpediteIconShow = Visibility.Hidden;
				if (expedite)
				{
					DueDateInfoLabel += $" - EXPEDITE!";
					DueDateInfoColor = new SolidColorBrush(Colors.Red);
					ExpediteIconShow = Visibility.Visible;
				}
			}
			else
			{
				DueDateInfoLabel = string.Empty;
				ExpediteIconShow = Visibility.Hidden;
			}
		}
		private async Task Potential_Overwrite_Check()
		{
			try
			{
				if (IsOpening) return;
				await using (var db = new DigitalDatasheetContext())
				{
					JobForm jobForm = await db.JobForms.FindAsync(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
					if (jobForm == null)
					{
						JobConflictWarning = false;
						wo_check_icon.Visibility = Visibility.Hidden;
					}
					else
					{
						wo_check_icon.Source = new BitmapImage(new Uri("../Images/warning_icon.png", UriKind.Relative));
						wo_check_icon.Visibility = Visibility.Visible;
						JobConflictWarning = true;
						MessageBox.Show($"WARNING: The current values for" +
							$"\n\n\t{$"Work Order Number:"} {FormSet.FullWorkOrder,10}" +
							$"\n\t{$"Test Condition:"} {FormSet.TestCondition,10}" +
							$"\n\tTesting Performed On: {FormSet.TestPerformedOn,10}" +
							$"\n\nhave been set to an already existing job. If you save the job, any changes made will completely override the previous job data.", "Job Override Warning!",
							MessageBoxButton.OK, MessageBoxImage.Warning);
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to check for potential overwrite.\n{ex}",
					"Overwrite Check ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private void View_Standard_Remarks(object sender, RoutedEventArgs e)
		{
			try
			{
				RemarksDocumentView r_view = new RemarksDocumentView();
				r_view.ShowDialog();
				if (r_view.CancelClick)
					return;
				// get selected remark from remark list
				string remark = r_view.SelectedRemark;
				// go through children of remark_list and determine first blank one to put remark in
				if (Remarks.Count > 0 && string.IsNullOrEmpty(Remarks.Last().Remark))
					Remarks.Last().Remark = remark;
				else
				{
					add_remark_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
					Remarks.Last().Remark = remark;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to view standard remarks document.\n{ex}",
					"Remarks Document View ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		#region Menu Functions

		#region File Menu
		private /*async*/ void New_Job(object sender, RoutedEventArgs e)
		{
			if (!NetConn)
			{
				MessageBox.Show("No network connection");
				return;
			}
			try
			{
				//MenuItem selection = sender as MenuItem;
				//string win_option = selection.Tag.ToString();

				MainWindow win = new MainWindow();
				//if (win_option == "this")
				//{
    //                MessageBoxResult? new_job = null;
    //                if (await Unsaved_Changes_CheckAsync())
				//	{
				//		new_job = MessageBox.Show("Your current job and any new information will be lost. Would you like to save it first?", "Save Current Job", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				//		if (new_job == MessageBoxResult.Yes)
				//		{
				//			if (string.IsNullOrEmpty(FormSet.WorkOrderNo) || condition_input.SelectedIndex < 0 /*|| customer_input.SelectedIndex < 0*/)
				//			{
				//				MessageBox.Show("Necessary information is missing. Please correct issue before saving the job", "Missing Information", MessageBoxButton.OK, MessageBoxImage.Error);
				//				return;
				//			}
				//			await Save_Job();
				//		}
				//		else if (new_job == MessageBoxResult.Cancel)
				//		{
				//			win.Close();
				//			return;
				//		}
				//	}
				//	win.Show();
				//	Close();
				//}
				//else
				win.Show();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to open a new window.\n{ex}",
					"New Window ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async void Open_Menu_Click(object sender, RoutedEventArgs e)
		{
			if (UnsavedChanges = await Unsaved_Changes_CheckAsync())
			{
				MessageBoxResult saveBeforeClose = MessageBox.Show("There may currently be unsaved changes in this job. Would you like to save before opening a new job?",
					   "Unsaved Changes!",
					   MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				if (saveBeforeClose == MessageBoxResult.Yes)
				{
					if (!Valid_Save_Check())
						return;
					await Save_Job();
				}
				else if (saveBeforeClose == MessageBoxResult.Cancel)
					return;
				UnsavedChanges = false;
			}

			MenuItem selection = sender as MenuItem;
			if (selection.Tag == null) return;
			string winOption = selection.Tag.ToString();
			string workOrderNumber, workOrderNumberDash = string.Empty, fullWorkOrderNumber, testCondition, testPerformedOn;
			IsOpening = true;

			try
			{
				if (winOption == "recent")
				{
					string header = selection.Header.ToString();
					string[] headerSplit = header.Split(" - ");
					fullWorkOrderNumber = headerSplit[0].Trim();
					testCondition = headerSplit[2].Trim() == "AR" ? "As Received" : "After Thermal Stress";
					testPerformedOn = headerSplit[3].Trim();

					if (!fullWorkOrderNumber.Contains("-"))
						workOrderNumber = fullWorkOrderNumber;
					else
					{
						string[] workOrderSplit = fullWorkOrderNumber.Split('-');
						workOrderNumber = workOrderSplit[0].Trim();
						workOrderNumberDash = workOrderSplit[1].Trim();
					}

					await Open_Job(workOrderNumber, workOrderNumberDash, fullWorkOrderNumber, testPerformedOn, testCondition);
				}
				else
				{
					OpenJobView popup = new OpenJobView(winOption);
					popup.ShowDialog();
					if (popup.CancelClick)
						return;
					workOrderNumber = popup.OpenJobData.WorkOrderNo;
					workOrderNumberDash = popup.OpenJobData.WorkOrderNoDash;
					if (string.IsNullOrEmpty(workOrderNumberDash))
						fullWorkOrderNumber = workOrderNumber;
					else
						fullWorkOrderNumber = popup.OpenJobData.FullWorkOrder;
					testCondition = popup.OpenJobData.TestCondition;
					testPerformedOn = popup.OpenJobData.TestPerformedOn;

					if (winOption == "new")
					{
						var win = new MainWindow();
						win.Show();
						await win.Open_Job(workOrderNumber, workOrderNumberDash, fullWorkOrderNumber, testPerformedOn, testCondition);
					}
					else
						await Open_Job(workOrderNumber, workOrderNumberDash, fullWorkOrderNumber, testPerformedOn, testCondition);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to open the job.\n{ex}",
					"Open Job ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				IsOpening = false;
				return;
			}
		}
		private async Task Open_Job(string workOrderNumber, string workOrderNumberDash, string fullWorkOrderNumber, string testPerformedOn, string testCondition)
		{
			if (!NetConn)
			{
				MessageBox.Show("No network connection");
				return;
			}
			#region Get Job Form Data
			try
			{
				await using (var db = new DigitalDatasheetContext())
				{
					JobForm jobForm = await db.JobForms.FindAsync(fullWorkOrderNumber, testCondition, testPerformedOn);
					
					// check if job is currently in use
					//if (jobForm.IsOpen)
     //               {
					//	if (MessageBox.Show("This job is currently being worked on by another user. Would you like to open the job as read-only?",
					//		"Open Read-Only?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
					//		IsReadyOnly = true;
					//	else
					//		return;
     //               }
					//else
     //               {
					//	jobForm.IsOpen = true;
					//	int affected = await db.SaveChangesAsync();
					//	if (affected != 1)
     //                   {
					//		MessageBox.Show("ERROR setting job to open");
					//		return;
     //                   }
     //               }

					FormSet.WorkOrderNo = workOrderNumber;
					FormSet.WorkOrderNoDash = workOrderNumberDash;
					condition_input.SelectedIndex = testCondition == "As Received" ? 0 : 1;
					FormSet.DateTested = jobForm.DateTested;
					FormSet.TestedBy = jobForm.TestedBy;
					FormSet.CheckedBy = jobForm.CheckedBy;
					FormSet.PartNumber = jobForm.PartNumber;
					FormSet.LotNumber = jobForm.LotNumber;
					FormSet.Customer = jobForm.Customer;
					FormSet.DateCode = jobForm.DateCode;
					FormSet.Specification1 = jobForm.Specification1;
					FormSet.Specification2 = jobForm.Specification2;
					FormSet.BoardType = jobForm.BoardType;
					FormSet.TestProcedure = jobForm.TestProcedure;
					FormSet.DrawingProvided = jobForm.DrawingProvided;
					FormSet.EvaluatedBy = jobForm.EvaluatedBy;
					FormSet.DateEvaluated = jobForm.DateEvaluated;
					FormSet.BakeTimeIn = jobForm.BakeTimeIn;
					FormSet.BakeTimeOut = jobForm.BakeTimeOut;
					FormSet.TotalTime = jobForm.TotalTime;
					FormSet.TestTemp = jobForm.TestTemp;
					FormSet.SolderFloats = jobForm.SolderFloats;
					FormSet.LastSaved = jobForm.LastSaved.ToString();
					switch (testPerformedOn)
					{
						case "Coupons":
							FormSet.Coupons = 1;
							break;
						case "BareBoards":
							FormSet.BareBoards = 1;
							break;
						case "CustomerMounts":
							FormSet.CustomerMounts = 1;
							FormSet.CustomerMountQty = jobForm.CustomerMountQty;
							break;
						case "AssembledBoards":
							FormSet.AssembledBoards = 1;
							break;
						case "Class2Assessment":
							FormSet.Class2Assessment = 1;
							break;
						default:
							break;
					}
				}
				//IsOpened = false;
				wo_check_icon.Visibility = Visibility.Hidden;
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to access job form data.\n{ex}",
					"Job Form ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				//IsOpening = false;
				//return;
			}
			#endregion Get Job Form Data
			#region Get Job Measurements/Observations Data
			try
			{
				// clear current job data
				int structureCount = StructureTitles.Count;
				for (int i = 1; i < structureCount; i++)
				{
					remove_exam_grid_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
				}
				int recordCount = RecordSet.Count;
				for (int i = 1; i < recordCount; i++)
				{
					remove_grid_0.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
				}

				await using (var db = new DigitalDatasheetContext())
				{
					var jobData = db.JobDataTable
						.Where(data => data.WorkOrderNumber.Equals(fullWorkOrderNumber) && data.TestCondition.Equals(testCondition) && data.TestPerformedOn.Equals(testPerformedOn))
						.OrderBy(data => data.StructureOrder)
						.ThenBy(data => data.Row)
						.ToList();
					var structureInfo = jobData.Select(data => new { data.StructureTitle, data.StructureOrder }).Distinct().OrderBy(data => data.StructureOrder).ToList();

					recordCount = 0;
					for (int i = 0; i < structureInfo.Count; i++)
					{
						StructureTitles[i].StructureTitle = structureInfo[i].StructureTitle;
						StructureTitles[i].StructureOrder = structureInfo[i].StructureOrder;

						var currentStructData = db.JobDataTable.Where(data => data.WorkOrderNumber.Equals(fullWorkOrderNumber) && data.TestCondition.Equals(testCondition) && data.TestPerformedOn.Equals(testPerformedOn) && data.StructureTitle.Equals(structureInfo[i].StructureTitle))
							.OrderBy(data => data.Row)
							.ToList();

						Button add_row_btn = FindName(AddGridBtnNames[i]) as Button;
						for (int j = 0; j < currentStructData.Count; j++)
						{
							RecordSet[recordCount].StructureInfo = StructureTitles[i];
							RecordSet[recordCount].Location = currentStructData[j].Location;
							RecordSet[recordCount].SerialNumber = currentStructData[j].SerialNumber;

							#region Hole Cu Plating
							string hcp = currentStructData[j].HoleCuPlating ?? string.Empty;
							if (hcp.StartsWith("R"))
							{
								RecordSet[recordCount].HoleCuPlating.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								hcp = hcp.Remove(0, 1);
							}
							if (hcp.Contains("^"))
							{
								RecordSet[recordCount].HoleCuPlating.NoteShow = Visibility.Visible;
								RecordSet[recordCount].HoleCuPlating.Note = hcp[(hcp.IndexOf("^") + 1)..];
								hcp = hcp.Remove(hcp.IndexOf("^"));
							}
							RecordSet[recordCount].HoleCuPlating.Measurement = hcp;
							#endregion Hole Cu Plating
							#region External Conductor Thickness
							string ect = currentStructData[j].ExternalConductor ?? string.Empty;
							if (ect.StartsWith("R"))
							{
								RecordSet[recordCount].ExternalConductor.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								ect = ect.Remove(0, 1);
							}
							if (ect.Contains("^"))
							{
								RecordSet[recordCount].ExternalConductor.NoteShow = Visibility.Visible;
								RecordSet[recordCount].ExternalConductor.Note = ect[(ect.IndexOf("^") + 1)..];
								ect = ect.Remove(ect.IndexOf("^"));
							}
							RecordSet[recordCount].ExternalConductor.Measurement = ect;
							#endregion External Conductor Thickness
							#region Surface Clad Cu
							string sClad = currentStructData[j].SurfaceCladCu ?? string.Empty;
							if (sClad.StartsWith("R"))
							{
								RecordSet[recordCount].SurfaceCladCu.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								sClad = sClad.Remove(0, 1);
							}
							if (sClad.Contains("^"))
							{
								RecordSet[recordCount].SurfaceCladCu.NoteShow = Visibility.Visible;
								RecordSet[recordCount].SurfaceCladCu.Note = sClad[(sClad.IndexOf("^") + 1)..];
								sClad = sClad.Remove(sClad.IndexOf("^"));
							}
							RecordSet[recordCount].SurfaceCladCu.Measurement = sClad;
							#endregion Surface Clad Cu
							#region Selective Plate
							//string sp = currentStructData[j].SelectivePlate ?? string.Empty;
							//if (sp.StartsWith("R"))
							//{
							//    RecordSet[recordCount].SelectivePlate.BackgroundColor = new SolidColorBrush(Colors.Yellow);
							//    sp = sp.Remove(0, 1);
							//}
							//if (sp.Contains("^"))
							//{
							//    RecordSet[recordCount].SelectivePlate.NoteShow = Visibility.Visible;
							//    RecordSet[recordCount].SelectivePlate.Note = sp.Substring(sp.IndexOf("^") + 1);
							//    sp = sp.Remove(sp.IndexOf("^"));
							//}
							//RecordSet[recordCount].SelectivePlate.Measurement = sp;
							#endregion Selective Plate
							#region Wrap Cu
							string wrap = currentStructData[j].WrapCu ?? string.Empty;
							if (wrap.StartsWith("R"))
							{
								RecordSet[recordCount].WrapCu.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								wrap = wrap.Remove(0, 1);
							}
							if (wrap.Contains("^"))
							{
								RecordSet[recordCount].WrapCu.NoteShow = Visibility.Visible;
								RecordSet[recordCount].WrapCu.Note = wrap[(wrap.IndexOf("^") + 1)..];
								wrap = wrap.Remove(wrap.IndexOf("^"));
							}
							RecordSet[recordCount].WrapCu.Measurement = wrap;
							#endregion Wrap Cu
							#region Cap Cu
							string cap = currentStructData[j].CapCu ?? string.Empty;
							if (cap.StartsWith("R"))
							{
								RecordSet[recordCount].CapCu.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								cap = cap.Remove(0, 1);
							}
							if (cap.Contains("^"))
							{
								RecordSet[recordCount].CapCu.NoteShow = Visibility.Visible;
								RecordSet[recordCount].CapCu.Note = cap[(cap.IndexOf("^") + 1)..];
								cap = cap.Remove(cap.IndexOf("^"));
							}
							RecordSet[recordCount].CapCu.Measurement = cap;
							#endregion Cap Cu
							#region Internal Clad Cu
							string intClad = currentStructData[j].InternalCladCu ?? string.Empty;
							if (intClad.StartsWith("R"))
							{
								RecordSet[recordCount].InternalCladCu.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								intClad = intClad.Remove(0, 1);
							}
							if (intClad.Contains("^"))
							{
								RecordSet[recordCount].InternalCladCu.NoteShow = Visibility.Visible;
								RecordSet[recordCount].InternalCladCu.Note = intClad[(intClad.IndexOf("^") + 1)..];
								intClad = intClad.Remove(intClad.IndexOf("^"));
							}
							RecordSet[recordCount].InternalCladCu.Measurement = intClad;
							#endregion Internal Clad Cu
							#region Min Etchback
							string minEtch = currentStructData[j].MinEtchback ?? string.Empty;
							if (minEtch.StartsWith("R"))
							{
								RecordSet[recordCount].MinEtchback.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								minEtch = minEtch.Remove(0, 1);
							}
							if (minEtch.Contains("^"))
							{
								RecordSet[recordCount].MinEtchback.NoteShow = Visibility.Visible;
								RecordSet[recordCount].MinEtchback.Note = minEtch[(minEtch.IndexOf("^") + 1)..];
								minEtch = minEtch.Remove(minEtch.IndexOf("^"));
							}
							RecordSet[recordCount].MinEtchback.Measurement = minEtch;
							#endregion Min Etchback
							#region Max Etchback
							string maxEtch = currentStructData[j].MaxEtchback ?? string.Empty;
							if (maxEtch.StartsWith("R"))
							{
								RecordSet[recordCount].MaxEtchback.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								maxEtch = maxEtch.Remove(0, 1);
							}
							if (maxEtch.Contains("^"))
							{
								RecordSet[recordCount].MaxEtchback.NoteShow = Visibility.Visible;
								RecordSet[recordCount].MaxEtchback.Note = maxEtch[(maxEtch.IndexOf("^") + 1)..];
								maxEtch = maxEtch.Remove(maxEtch.IndexOf("^"));
							}
							RecordSet[recordCount].MaxEtchback.Measurement = maxEtch;
							#endregion Max Etchback
							#region Internal Ring
							string intRing = currentStructData[j].InternalAnnularRing ?? string.Empty;
							if (intRing.StartsWith("R"))
							{
								RecordSet[recordCount].InternalAnnularRing.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								intRing = intRing.Remove(0, 1);
							}
							if (intRing.Contains("^"))
							{
								RecordSet[recordCount].InternalAnnularRing.NoteShow = Visibility.Visible;
								RecordSet[recordCount].InternalAnnularRing.Note = intRing[(intRing.IndexOf("^") + 1)..];
								intRing = intRing.Remove(intRing.IndexOf("^"));
							}
							RecordSet[recordCount].InternalAnnularRing.Measurement = intRing;
							#endregion Internal Ring
							#region External Ring
							string extRing = currentStructData[j].ExternalAnnularRing ?? string.Empty;
							if (extRing.StartsWith("R"))
							{
								RecordSet[recordCount].ExternalAnnularRing.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								extRing = extRing.Remove(0, 1);
							}
							if (extRing.Contains("^"))
							{
								RecordSet[recordCount].ExternalAnnularRing.NoteShow = Visibility.Visible;
								RecordSet[recordCount].ExternalAnnularRing.Note = extRing[(extRing.IndexOf("^") + 1)..];
								extRing = extRing.Remove(extRing.IndexOf("^"));
							}
							RecordSet[recordCount].ExternalAnnularRing.Measurement = extRing;
							#endregion External Ring
							#region Dielectric
							string dielectric = currentStructData[j].Dielectric ?? string.Empty;
							if (dielectric.StartsWith("R"))
							{
								RecordSet[recordCount].Dielectric.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								dielectric = dielectric.Remove(0, 1);
							}
							if (dielectric.Contains("^"))
							{
								RecordSet[recordCount].Dielectric.NoteShow = Visibility.Visible;
								RecordSet[recordCount].Dielectric.Note = dielectric[(dielectric.IndexOf("^") + 1)..];
								dielectric = dielectric.Remove(dielectric.IndexOf("^"));
							}
							RecordSet[recordCount].Dielectric.Measurement = dielectric;
							#endregion Dielectric
							#region Wicking
							string wicking = currentStructData[j].Wicking ?? string.Empty;
							if (wicking.StartsWith("R"))
							{
								RecordSet[recordCount].Wicking.BackgroundColor = new SolidColorBrush(Colors.Yellow);
								wicking = wicking.Remove(0, 1);
							}
							if (wicking.Contains("^"))
							{
								RecordSet[recordCount].Wicking.NoteShow = Visibility.Visible;
								RecordSet[recordCount].Wicking.Note = wicking[(wicking.IndexOf("^") + 1)..];
								wicking = wicking.Remove(wicking.IndexOf("^"));
							}
							RecordSet[recordCount].Wicking.Measurement = wicking;
							#endregion Wicking

							RecordSet[recordCount].InnerlayerSeparation = currentStructData[j].InnerlayerSeparation;
							RecordSet[recordCount].PlatingCrack = currentStructData[j].PlatingCrack;
							RecordSet[recordCount].PlatingVoid = currentStructData[j].PlatingVoid;
							RecordSet[recordCount].DelamBlisters = currentStructData[j].DelamBlisters;
							RecordSet[recordCount].LaminateVoidCrack = currentStructData[j].LaminateVoidCrack;
							RecordSet[recordCount].AcceptReject = currentStructData[j].AcceptReject;

							RecordSet[recordCount].Row = currentStructData[j].Row;
							recordCount++;

							if (j < currentStructData.Count - 1)
								add_row_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
						}
						if (i < structureInfo.Count - 1)
							add_exam_grid_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to access job measurement & observation data.\n{ex}",
					"Job Open ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				//IsOpening = false;
				//return;
			}
			#endregion Get Job Measurements/Observations Data
			#region Get Requirements
			try
			{
				await using (var db = new DigitalDatasheetContext())
				{
					JobRequirements requirements = await db.JobRequirementsTable.FindAsync(fullWorkOrderNumber, testCondition, testPerformedOn);
					if (requirements != null)
					{
						Requirement.HoleCuPlating = requirements.HoleCuPlating;
						Requirement.ExternalConductor = requirements.ExternalConductor;
						Requirement.SurfaceCladCu = requirements.SurfaceCladCu;
						Requirement.WrapCu = requirements.WrapCu;
						Requirement.CapCu = requirements.CapCu;
						Requirement.InternalCladCu = requirements.InternalCladCu;
						Requirement.MinEtchback = requirements.MinEtchback;
						Requirement.MaxEtchback = requirements.MaxEtchback;
						Requirement.InternalAnnularRing = requirements.InternalAnnularRing;
						Requirement.ExternalAnnularRing = requirements.ExternalAnnularRing;
						Requirement.Dielectric = requirements.Dielectric;
						Requirement.Wicking = requirements.Wicking;
					}
				}
				for (int i = 2; i < requirements_grid.Children.Count; i++)
				{
					if (requirements_grid.Children[i] is TextBox)
					{
						TextBox textBox = requirements_grid.Children[i] as TextBox;
						if (textBox.Text == "collapsed")
						{
							TextBox prevTextBox = requirements_grid.Children[i - 1] as TextBox;
							textBox.Visibility = Visibility.Collapsed;
							Grid.SetColumnSpan(prevTextBox, 2);
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to access job requirements.\n{ex}",
					"Job Open ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				//IsOpening = false;
				//return;
			}
			#endregion Get Requirements
			#region Get Remarks
			try
			{
				await using (var db = new DigitalDatasheetContext())
				{
					int remarkCount = Remarks.Count;
					for (int j = 0; j < remarkCount; j++)
						remove_remark_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));

					var jobRemarks = db.JobRemarksTable.Where(remark => remark.WorkOrderNumber.Equals(fullWorkOrderNumber) && remark.TestCondition.Equals(testCondition) && remark.TestPerformedOn.Equals(testPerformedOn))
						.Select(remark => new { remark.Remark, remark.Reject, remark.Row })
						.OrderBy(remark => remark.Row);
					if (jobRemarks.Count() > 0)
					{
						int i = 0;
						foreach (var remark in jobRemarks)
						{
							add_remark_btn.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
							Remarks[i].Remark = remark.Remark;
							if (remark.Reject)
								Remarks[i].BackgroundColor = new SolidColorBrush(Colors.Yellow);
							//Remarks[i].Reject = remark.Reject;
							Remarks[i].Row = remark.Row;
							i++;
						}
					}
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to access job remarks.\n{ex}",
					"Job Open ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				//IsOpening = false;
				//return;
			}
			#endregion Get Remarks
			JobConflictWarning = false;
			UnsavedChanges = false;
			UnsavedForm = false;
			UnsavedData = false;
			UnsavedRequirements = false;
			UnsavedRemarks = false;
			IsOpening = false;
			await Check_DueDate();
			//IsOpened = true;
		}
		private async void Save_Menu_Click(object sender, RoutedEventArgs e)
		{
			if (UnsavedChanges = await Unsaved_Changes_CheckAsync())
                await Save_Job();
		}
		private bool Valid_Save_Check()
		{
			if (!WoCheck)
			{
				MessageBox.Show($"{FormSet.FullWorkOrder} is not a valid work order number. You must correct this before saving.",
					   "Invalid Work Order!", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			if (string.IsNullOrEmpty(FormSet.TestPerformedOn))
			{
				//if (AutoSaveOn) auto_save_menu.RaiseEvent(new RoutedEventArgs(MenuItem.ClickEvent));
				MessageBox.Show("Test Performed On must be selected before being able to save. Please fix this and try again.", "Invalid Fields", MessageBoxButton.OK, MessageBoxImage.Error);
				return false;
			}
			if (JobConflictWarning)
			{
				if (MessageBox.Show("The current job you are about to save has been set to an already existing job. Saving will completely override the previous job data. Would you like to continue?",
					"Job Conflict Warning!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					return false;
				JobConflictWarning = false;
			}
			return true;
		}
		private async Task<bool> Unsaved_Changes_CheckAsync()
		{
			await using (var db = new DigitalDatasheetContext())
			{
				#region Form Check
				var jobForm = await db.JobForms.FindAsync(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
				if (jobForm == null)
                {
					if (!string.IsNullOrEmpty(FormSet.FullWorkOrder))
						UnsavedForm = true;
                }
				else
				{
					PropertyInfo datasheetItem;
					foreach (var dbItem in jobForm.GetType().GetProperties())
					{
						if (dbItem.Name.Equals("WorkOrderNumber"))
							datasheetItem = FormSet.GetType().GetProperty("FullWorkOrder");
						else if (dbItem.Name.Equals("LastSaved"))
							continue;
						else
							datasheetItem = FormSet.GetType().GetProperty(dbItem.Name);

						if (datasheetItem == null) continue;
						if (dbItem.GetValue(jobForm, null) == null)
						{
							if (datasheetItem.GetValue(FormSet, null) != null)
							{
								UnsavedForm = true;
								break;
							}
						}
						else if (!dbItem.GetValue(jobForm, null).Equals(datasheetItem.GetValue(FormSet, null)))
						{
							UnsavedForm = true;
							break;
						}
					}
				}
				#endregion Form Check

				#region Measurement/Observation Check
				IQueryable<JobData> jobDataRows = db.JobDataTable
							.Where(data => data.WorkOrderNumber.Equals(FormSet.FullWorkOrder) && data.TestCondition.Equals(FormSet.TestCondition) && data.TestPerformedOn.Equals(FormSet.TestPerformedOn))
							.OrderBy(data => data.StructureOrder)
							.ThenBy(data => data.Row);
				//.ToListAsync();
				if (jobDataRows.Count() != RecordSet.Count)
                {
					if (jobDataRows.Count() == 0 && RecordSet.Count == 1)
                    {
						var record = RecordSet[0];
						foreach (var recordProp in record.GetType().GetProperties())
                        {
							if (recordProp is null) continue;
							var currentRecord = recordProp.GetValue(record, null);
							if (currentRecord is null) continue;
							if (currentRecord is RecordGroup)
							{
								string formattedData = (currentRecord as RecordGroup).Measurement;
								if (((SolidColorBrush)(currentRecord as RecordGroup).BackgroundColor).Color == Colors.Yellow)
									formattedData = $"R{formattedData}";
								if (!string.IsNullOrEmpty((currentRecord as RecordGroup).Note))
									formattedData += $"^{(currentRecord as RecordGroup).Note}";

								if (!string.IsNullOrEmpty(formattedData))
								{
									UnsavedData = true;
									break;
								}
							}
							else if (currentRecord is string r)
                            {
								if (!string.IsNullOrEmpty(r))
								{
									UnsavedData = true;
									break;
								}
							}
						}
                    }
					else
						UnsavedData = true;
                }
				else
				{
					await jobDataRows.ForEachAsync(jobData =>
					{
						PropertyInfo datasheetItem;
						RecordGroup currentRecord;
						var record = RecordSet.Find(r =>
							r.StructureInfo.StructureOrder.Equals(jobData.StructureOrder) &&
							r.StructureInfo.StructureTitle.Equals(jobData.StructureTitle) &&
							r.SerialNumber.Equals(jobData.SerialNumber) &&
							r.Location.Equals(jobData.Location));
						if (record == null)
							UnsavedData = true;
						else
						{
							foreach (var dbItem in jobData.GetType().GetProperties())
							{
								#region Measurement Check
								if (dbItem.Name.Equals("HoleCuPlating") || dbItem.Name.Equals("ExternalConductor") || dbItem.Name.Equals("SurfaceCladCu")
														|| dbItem.Name.Equals("WrapCu") || dbItem.Name.Equals("CapCu") || dbItem.Name.Equals("InternalCladCu")
														|| dbItem.Name.Equals("MinEtchback") || dbItem.Name.Equals("MaxEtchback") || dbItem.Name.Equals("InternalAnnularRing")
														|| dbItem.Name.Equals("ExternalAnnularRing") || dbItem.Name.Equals("Dielectric") || dbItem.Name.Equals("Wicking"))
								{
									datasheetItem = record.GetType().GetProperty(dbItem.Name);
									currentRecord = datasheetItem.GetValue(record, null) as RecordGroup;

									if (datasheetItem == null || currentRecord == null) continue;

									#region Note/Failure Check
									string formattedData = currentRecord.Measurement;
									if (((SolidColorBrush)currentRecord.BackgroundColor).Color == Colors.Yellow)
										formattedData = $"R{formattedData}";
									if (!string.IsNullOrEmpty(currentRecord.Note))
										formattedData += $"^{currentRecord.Note}";
									#endregion Note/Failure Check

									string dbRecord = (string)dbItem.GetValue(jobData, null);
									if (string.IsNullOrEmpty(dbRecord))
									{
										if (!string.IsNullOrEmpty(formattedData))
										{
											UnsavedData = true;
											break;
										}
									}
									else if (!dbRecord.Equals(formattedData))
									{
										UnsavedData = true;
										break;
									}
								}
								#endregion Measurement Check
								
								#region Observation Check
								else if (dbItem.Name.Equals("InnerlayerSeparation") || dbItem.Name.Equals("PlatingCrack") || dbItem.Name.Equals("PlatingVoid")
															|| dbItem.Name.Equals("DelamBlisters") || dbItem.Name.Equals("LaminateVoidCrack") || dbItem.Name.Equals("AcceptReject"))
								{
									datasheetItem = record.GetType().GetProperty(dbItem.Name);
									if (datasheetItem == null)
										continue;
									if (dbItem.GetValue(jobData, null) == null)
									{
										if (datasheetItem.GetValue(record, null) != null)
										{
											UnsavedData = true;
											break;
										}
									}
									else if (!dbItem.GetValue(jobData, null).Equals(datasheetItem.GetValue(record, null)))
									{
										UnsavedData = true;
										break;
									}
								}
								#endregion Observation Check
							}
						}
					});
				}
				#endregion Measurement/Observation Check

				#region Requirements Check
				JobRequirements jobRequirements = await db.JobRequirementsTable.FindAsync(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
				if (jobRequirements == null)
				{
					if (!string.IsNullOrEmpty(FormSet.FullWorkOrder))
						UnsavedRequirements = true;
				}
				else
				{
					PropertyInfo datasheetItem;
					foreach (var dbItem in jobRequirements.GetType().GetProperties())
					{
						if (dbItem.Name.Equals("WorkOrderNumber") || dbItem.Name.Equals("TestCondition") || dbItem.Name.Equals("TestPerformedOn"))
							continue;
						datasheetItem = Requirement.GetType().GetProperty(dbItem.Name);
						if (datasheetItem == null) continue;
						if (dbItem.GetValue(jobRequirements, null) == null)
						{
							if (datasheetItem.GetValue(Requirement, null) != null)
							{
								UnsavedRequirements = true;
								break;
							}
						}
						else if (!dbItem.GetValue(jobRequirements, null).Equals(datasheetItem.GetValue(Requirement, null)))
						{
							UnsavedRequirements = true;
							break;
						}
					}
				}
				#endregion Requirements Check

				#region Remarks Check
				List<JobRemark> jobRemarks = await db.JobRemarksTable
										.Where(remark => remark.WorkOrderNumber.Equals(FormSet.FullWorkOrder) && remark.TestCondition.Equals(FormSet.TestCondition) && remark.TestPerformedOn.Equals(FormSet.TestPerformedOn))
										.OrderBy(remark => remark.Row)
										.ToListAsync();
				if (jobRemarks.Count != Remarks.Count)
					UnsavedRemarks = true;
				else
				{
					jobRemarks.ForEach(jobRemark =>
					{
						PropertyInfo datasheetItem;
						foreach (var dbItem in jobRemark.GetType().GetProperties())
						{
							if (dbItem.Name.Equals("WorkOrderNumber") || dbItem.Name.Equals("TestCondition") || dbItem.Name.Equals("TestPerformedOn") || dbItem.Name.Equals("Row"))
								continue;
							var remark = Remarks.Find(r => r.Row.Equals(jobRemark.Row));
                            if (remark == null)
                            {
                                UnsavedRemarks = true;
                                break;
                            }
							datasheetItem = remark.GetType().GetProperty(dbItem.Name);
							if (datasheetItem == null) continue;
							if (dbItem.GetValue(jobRemark, null) == null)
							{
								if (datasheetItem.GetValue(remark, null) != null)
								{
									UnsavedRemarks = true;
									break;
								}
							}
							else if (!dbItem.GetValue(jobRemark, null).Equals(datasheetItem.GetValue(remark, null)))
							{
								UnsavedRemarks = true;
								break;
							}
						}
					});
				}
				#endregion Remarks Check

			}
			return UnsavedForm || UnsavedData || UnsavedRequirements || UnsavedRemarks;
		}
		private async Task Save_Job()
		{
			if (!NetConn)
			{
				MessageBox.Show("No network connection");
				return;
			}
			// make sure last changed element is updated despite TextBox not losing focus
			var focusObj = FocusManager.GetFocusedElement(this);
			if (focusObj != null && focusObj is TextBox)
			{
				var binding = (focusObj as TextBox).GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
				Measurement_Format_Text(focusObj as TextBox);
			}
			if (!Valid_Save_Check()) return;

			FormSet.LastSaved = "Save In Progress...";
			wo_check_icon.Visibility = Visibility.Hidden;
			DateTime saveTime = DateTime.Now;
			string fullWorkOrder = IsClosed && UnsavedChanges ? $"*{FormSet.FullWorkOrder}" : FormSet.FullWorkOrder;
			int affected = 0;
			#region Job Form
			try
			{
				if (UnsavedForm)
				{
					await using (var db = new DigitalDatasheetContext())
					{
						JobForm jobForm = await db.JobForms.FindAsync(fullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
						if (jobForm != null)
						{
							jobForm.CustomerMountQty = FormSet.CustomerMountQty;
							jobForm.DateTested = FormSet.DateTested;
							jobForm.TestedBy = FormSet.TestedBy;
							jobForm.CheckedBy = FormSet.CheckedBy;
							jobForm.PartNumber = FormSet.PartNumber;
							jobForm.LotNumber = FormSet.LotNumber;
							jobForm.Customer = FormSet.Customer;
							jobForm.DateCode = FormSet.DateCode;
							jobForm.Specification1 = FormSet.Specification1;
							jobForm.Specification2 = FormSet.Specification2;
							jobForm.BoardType = FormSet.BoardType;
							jobForm.TestProcedure = FormSet.TestProcedure;
							jobForm.DrawingProvided = FormSet.DrawingProvided;
							jobForm.EvaluatedBy = FormSet.EvaluatedBy;
							jobForm.DateEvaluated = FormSet.DateEvaluated;
							jobForm.BakeTimeIn = FormSet.BakeTimeIn;
							jobForm.BakeTimeOut = FormSet.BakeTimeOut;
							jobForm.TotalTime = FormSet.TotalTime;
							jobForm.TestTemp = FormSet.TestTemp;
							jobForm.SolderFloats = FormSet.SolderFloats;
						}
						else
						{
							JobForm newJobForm = new JobForm
							{
								WorkOrderNumber = FormSet.FullWorkOrder,
								TestCondition = FormSet.TestCondition,
								TestPerformedOn = FormSet.TestPerformedOn,
								CustomerMountQty = FormSet.CustomerMountQty,
								DateTested = FormSet.DateTested,
								TestedBy = FormSet.TestedBy,
								CheckedBy = FormSet.CheckedBy,
								PartNumber = FormSet.PartNumber,
								LotNumber = FormSet.LotNumber,
								Customer = FormSet.Customer,
								DateCode = FormSet.DateCode,
								Specification1 = FormSet.Specification1,
								Specification2 = FormSet.Specification2,
								BoardType = FormSet.BoardType,
								TestProcedure = FormSet.TestProcedure,
								DrawingProvided = FormSet.DrawingProvided,
								EvaluatedBy = FormSet.EvaluatedBy,
								DateEvaluated = FormSet.DateEvaluated,
								BakeTimeIn = FormSet.BakeTimeIn,
								BakeTimeOut = FormSet.BakeTimeOut,
								TotalTime = FormSet.TotalTime,
								TestTemp = FormSet.TestTemp,
								SolderFloats = FormSet.SolderFloats
							};
							await db.JobForms.AddAsync(newJobForm);
						}
						affected = await db.SaveChangesAsync();
						if (affected != 1)
						{
							MessageBox.Show("The current job was not saved.");
							return;
						}
					}
					UnsavedForm = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to save job form data.\n{ex}",
					"Job Save ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				FormSet.LastSaved = "Job Form Save ERROR!";
				return;
			}
			#endregion Job Form
			#region Job Data
			try
			{
				if (UnsavedData)
				{

					// check that all grids have associated structure title
					//Structure noTitleStruct = StructureTitles.Find(data => string.IsNullOrEmpty(data.StructureTitle));
					if (StructureTitles.Exists(s => string.IsNullOrEmpty(s.StructureTitle)))
					{
						MessageBox.Show($"One or more groups does not contain a structure title. Please make sure all recording groups have an associated structure title before saving.", "Structure Title(s) Missing", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					if (StructureTitles.Select(s => new { s.StructureTitle }).Distinct().Count() != StructureTitles.Count)
					{
						MessageBox.Show($"Duplicate structure titles found. Please make sure all structure titles are unique before saving.", "Structure Title Duplicates", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					// check that all data rows have associated serial number
					//Record noSnData = RecordSet.Find(data => string.IsNullOrEmpty(data.SerialNumber));
					if (RecordSet.Exists(data => string.IsNullOrEmpty(data.SerialNumber)) || DuplicateDataRow)
					{
						MessageBox.Show($"One or more rows contain duplicate or empty serial numbers. Please make sure all recordings have an associated, unique serial number before saving.", "S/N Error!", MessageBoxButton.OK, MessageBoxImage.Error);
						return;
					}
					await using (var db = new DigitalDatasheetContext())
					{
						IQueryable<JobData> jobData = db.JobDataTable
							.Where(key => key.WorkOrderNumber.Equals(fullWorkOrder) && key.TestCondition.Equals(FormSet.TestCondition) && key.TestPerformedOn.Equals(FormSet.TestPerformedOn));
						//MessageBox.Show($"{jobData.Count()}");
						if (jobData.Count() > 0)
						{
							// go through database and remove and data rows that are in the db, but no longer on the current datasheet
							List<JobData> removableData = new List<JobData>();
							jobData.ToList().ForEach(record =>
							{
								var recordCheck = RecordSet.Find(r => r.StructureInfo.StructureTitle == record.StructureTitle && r.SerialNumber == record.SerialNumber && r.Location == record.Location);
								if (recordCheck == null)
									removableData.Add(record);
							});
							db.JobDataTable.RemoveRange(removableData);
							//int affected = await db.SaveChangesAsync();
							//MessageBox.Show($"removed from db: {affected}");

							//db.JobDataTable.RemoveRange(jobData);
							//int affected = await Task.Run(() => db.SaveChangesAsync().Result);
						}
						foreach (Record dataRow in RecordSet)
						{
							#region Check Failures and Notes
							#region Hole Cu Plating
							string hcp = dataRow.HoleCuPlating.Measurement;
							if (((SolidColorBrush)dataRow.HoleCuPlating.BackgroundColor).Color == Colors.Yellow)
								hcp = $"R{hcp}";
							if (!string.IsNullOrEmpty(dataRow.HoleCuPlating.Note))
								hcp += $"^{dataRow.HoleCuPlating.Note}";
							#endregion Hole Cu Plating
							#region External Conductor Thickness
							string ect = dataRow.ExternalConductor.Measurement;
							if (((SolidColorBrush)dataRow.ExternalConductor.BackgroundColor).Color == Colors.Yellow)
								ect = $"R{ect}";
							if (!string.IsNullOrEmpty(dataRow.ExternalConductor.Note))
								ect += $"^{dataRow.ExternalConductor.Note}";
							#endregion External Conductor Thickness
							#region Surface Clad
							string sClad = dataRow.SurfaceCladCu.Measurement;
							if (((SolidColorBrush)dataRow.SurfaceCladCu.BackgroundColor).Color == Colors.Yellow)
								sClad = $"R{sClad}";
							if (!string.IsNullOrEmpty(dataRow.SurfaceCladCu.Note))
								sClad += $"^{dataRow.SurfaceCladCu.Note}";
							#endregion Surface Clad
							#region Selective Plate
							//string sp = dataRow.SelectivePlate.Measurement;
							//if (((SolidColorBrush)dataRow.SelectivePlate.BackgroundColor).Color == Colors.Yellow)
							//    sp = $"R{sp}";
							//if (!string.IsNullOrEmpty(dataRow.SelectivePlate.Note))
							//    sp += $"^{dataRow.SelectivePlate.Note}";
							#endregion Selective Plate
							#region Wrap Cu
							string wrap = dataRow.WrapCu.Measurement;
							if (((SolidColorBrush)dataRow.WrapCu.BackgroundColor).Color == Colors.Yellow)
								wrap = $"R{wrap}";
							if (!string.IsNullOrEmpty(dataRow.WrapCu.Note))
								wrap += $"^{dataRow.WrapCu.Note}";
							#endregion Wrap Cu
							#region Cap Cu
							string cap = dataRow.CapCu.Measurement;
							if (((SolidColorBrush)dataRow.CapCu.BackgroundColor).Color == Colors.Yellow)
								cap = $"R{cap}";
							if (!string.IsNullOrEmpty(dataRow.CapCu.Note))
								cap += $"^{dataRow.CapCu.Note}";
							#endregion Cap Cu
							#region Internal Clad
							string intClad = dataRow.InternalCladCu.Measurement;
							if (((SolidColorBrush)dataRow.InternalCladCu.BackgroundColor).Color == Colors.Yellow)
								intClad = $"R{intClad}";
							if (!string.IsNullOrEmpty(dataRow.InternalCladCu.Note))
								intClad += $"^{dataRow.InternalCladCu.Note}";
							#endregion Internal Clad
							#region Min Etch
							string minEtch = dataRow.MinEtchback.Measurement;
							if (((SolidColorBrush)dataRow.MinEtchback.BackgroundColor).Color == Colors.Yellow)
								minEtch = $"R{minEtch}";
							if (!string.IsNullOrEmpty(dataRow.MinEtchback.Note))
								minEtch += $"^{dataRow.MinEtchback.Note}";
							#endregion Min Etch
							#region Max Etch
							string maxEtch = dataRow.MaxEtchback.Measurement;
							if (((SolidColorBrush)dataRow.MaxEtchback.BackgroundColor).Color == Colors.Yellow)
								maxEtch = $"R{maxEtch}";
							if (!string.IsNullOrEmpty(dataRow.MaxEtchback.Note))
								maxEtch += $"^{dataRow.MaxEtchback.Note}";
							#endregion Max Etch
							#region Internal Annular
							string intRing = dataRow.InternalAnnularRing.Measurement;
							if (((SolidColorBrush)dataRow.InternalAnnularRing.BackgroundColor).Color == Colors.Yellow)
								intRing = $"R{intRing}";
							if (!string.IsNullOrEmpty(dataRow.InternalAnnularRing.Note))
								intRing += $"^{dataRow.InternalAnnularRing.Note}";
							#endregion Internal Annular
							#region External Annular
							string extRing = dataRow.ExternalAnnularRing.Measurement;
							if (((SolidColorBrush)dataRow.ExternalAnnularRing.BackgroundColor).Color == Colors.Yellow)
								extRing = $"R{extRing}";
							if (!string.IsNullOrEmpty(dataRow.ExternalAnnularRing.Note))
								extRing += $"^{dataRow.ExternalAnnularRing.Note}";
							#endregion External Annular
							#region Dielectric
							string dielectric = dataRow.Dielectric.Measurement;
							if (((SolidColorBrush)dataRow.Dielectric.BackgroundColor).Color == Colors.Yellow)
								dielectric = $"R{dielectric}";
							if (!string.IsNullOrEmpty(dataRow.Dielectric.Note))
								dielectric += $"^{dataRow.Dielectric.Note}";
							#endregion Dielectric
							#region Wicking
							string wicking = dataRow.Wicking.Measurement;
							if (((SolidColorBrush)dataRow.Wicking.BackgroundColor).Color == Colors.Yellow)
								wicking = $"R{wicking}";
							if (!string.IsNullOrEmpty(dataRow.Wicking.Note))
								wicking += $"^{dataRow.Wicking.Note}";
							#endregion Wicking
							#endregion Check Failures and Notes

							JobData dbCheck = await db.JobDataTable.FindAsync(fullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn, dataRow.StructureInfo.StructureTitle, dataRow.SerialNumber, dataRow.Location);
							if (dbCheck == null)
							{
								JobData newDataRow = new JobData
								{
									WorkOrderNumber = fullWorkOrder,
									TestCondition = FormSet.TestCondition,
									TestPerformedOn = FormSet.TestPerformedOn,
									StructureTitle = dataRow.StructureInfo.StructureTitle,
									SerialNumber = dataRow.SerialNumber,
									Location = dataRow.Location,
									HoleCuPlating = hcp,
									ExternalConductor = ect,
									SurfaceCladCu = sClad,
									WrapCu = wrap,
									CapCu = cap,
									InternalCladCu = intClad,
									MinEtchback = minEtch,
									MaxEtchback = maxEtch,
									InternalAnnularRing = intRing,
									ExternalAnnularRing = extRing,
									Dielectric = dielectric,
									Wicking = wicking,
									InnerlayerSeparation = dataRow.InnerlayerSeparation,
									PlatingCrack = dataRow.PlatingCrack,
									PlatingVoid = dataRow.PlatingVoid,
									DelamBlisters = dataRow.DelamBlisters,
									LaminateVoidCrack = dataRow.LaminateVoidCrack,
									AcceptReject = dataRow.AcceptReject,
									Row = dataRow.Row,
									StructureOrder = dataRow.StructureInfo.StructureOrder
								};
								await db.JobDataTable.AddAsync(newDataRow);
							}
							else
							{
								dbCheck.HoleCuPlating = hcp;
								dbCheck.ExternalConductor = ect;
								dbCheck.SurfaceCladCu = sClad;
								dbCheck.WrapCu = wrap;
								dbCheck.CapCu = cap;
								dbCheck.InternalCladCu = intClad;
								dbCheck.MinEtchback = minEtch;
								dbCheck.MaxEtchback = maxEtch;
								dbCheck.InternalAnnularRing = intRing;
								dbCheck.ExternalAnnularRing = extRing;
								dbCheck.Dielectric = dielectric;
								dbCheck.Wicking = wicking;
								dbCheck.InnerlayerSeparation = dataRow.InnerlayerSeparation;
								dbCheck.PlatingCrack = dataRow.PlatingCrack;
								dbCheck.PlatingVoid = dataRow.PlatingVoid;
								dbCheck.DelamBlisters = dataRow.DelamBlisters;
								dbCheck.LaminateVoidCrack = dataRow.LaminateVoidCrack;
								dbCheck.AcceptReject = dataRow.AcceptReject;
								dbCheck.Row = dataRow.Row;
								dbCheck.StructureOrder = dataRow.StructureInfo.StructureOrder;
							}
						}
						affected = await db.SaveChangesAsync();
					}
					UnsavedData = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to save job measurement & observation data.\n{ex}",
					"Job Save ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				FormSet.LastSaved = "Job Measurments/Observations Save ERROR!";
				return;
			}
			#endregion Job Data
			#region Requirements
			try
			{
				if (UnsavedRequirements)
				{
					await using (var db = new DigitalDatasheetContext())
					{
						JobRequirements requirement = await db.JobRequirementsTable.FindAsync(fullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
						if (requirement != null)
						{
							requirement.HoleCuPlating = Requirement.HoleCuPlating;
							requirement.ExternalConductor = Requirement.ExternalConductor;
							requirement.SurfaceCladCu = Requirement.SurfaceCladCu;
							requirement.WrapCu = Requirement.WrapCu;
							requirement.CapCu = Requirement.CapCu;
							requirement.InternalCladCu = Requirement.InternalCladCu;
							requirement.MinEtchback = Requirement.MinEtchback;
							requirement.MaxEtchback = Requirement.MaxEtchback;
							requirement.InternalAnnularRing = Requirement.InternalAnnularRing;
							requirement.ExternalAnnularRing = Requirement.ExternalAnnularRing;
							requirement.Dielectric = Requirement.Dielectric;
							requirement.Wicking = Requirement.Wicking;
						}
						else
						{
							JobRequirements newRequirements = new JobRequirements
							{
								WorkOrderNumber = fullWorkOrder,
								TestCondition = FormSet.TestCondition,
								TestPerformedOn = FormSet.TestPerformedOn,
								HoleCuPlating = Requirement.HoleCuPlating,
								ExternalConductor = Requirement.ExternalConductor,
								SurfaceCladCu = Requirement.SurfaceCladCu,
								WrapCu = Requirement.WrapCu,
								CapCu = Requirement.CapCu,
								InternalCladCu = Requirement.InternalCladCu,
								MinEtchback = Requirement.MinEtchback,
								MaxEtchback = Requirement.MaxEtchback,
								InternalAnnularRing = Requirement.InternalAnnularRing,
								ExternalAnnularRing = Requirement.ExternalAnnularRing,
								Dielectric = Requirement.Dielectric,
								Wicking = Requirement.Wicking
							};
							await db.JobRequirementsTable.AddAsync(newRequirements);
							//int affected = await Task.Run(() => db.SaveChangesAsync().Result);
							//MessageBox.Show($"{success}");
						}
						affected = await db.SaveChangesAsync();
						bool success = (affected == 1);
					}
					UnsavedRequirements = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to save job requirements.\n{ex}",
					"Job Save ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				FormSet.LastSaved = "Job Requirements Save ERROR!";
				return;
			}
			#endregion Requirements
			#region Remarks
			try
			{
				if (UnsavedRemarks)
				{
					foreach (var remark in Remarks)
					{
						if (string.IsNullOrEmpty(remark.Remark))
						{
							remark.Remark = $"Remark {remark.Row}";
							//MessageBox.Show("Please remove ");
						}
					}
					await using (var db = new DigitalDatasheetContext())
					{
						IQueryable<JobRemark> remarks = db.JobRemarksTable
							.Where(remark => remark.WorkOrderNumber.Equals(fullWorkOrder) && remark.TestCondition.Equals(FormSet.TestCondition) && remark.TestPerformedOn.Equals(FormSet.TestPerformedOn));
						if (remarks.Count() > 0)
						{
							List<JobRemark> removableRemarks = new List<JobRemark>();
							remarks.ToList().ForEach(remark =>
							{
								var remarkCheck = Remarks.Find(r => r.Remark == remark.Remark);
								if (remarkCheck == null)
									removableRemarks.Add(remark);
							});

							db.JobRemarksTable.RemoveRange(removableRemarks);
							//affected = await db.SaveChangesAsync();
						}
						//affected = 0;
						foreach (RemarkSet remark in Remarks)
						{
							JobRemark dbCheck = await db.JobRemarksTable.FindAsync(fullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn, remark.Remark);
							if (dbCheck == null)
							{
								JobRemark newJobRemark = new JobRemark
								{
									WorkOrderNumber = fullWorkOrder,
									TestCondition = FormSet.TestCondition,
									TestPerformedOn = FormSet.TestPerformedOn,
									Remark = remark.Remark,
									Reject = remark.Reject,
									Row = remark.Row
								};
								await db.JobRemarksTable.AddAsync(newJobRemark);
							}
							else
							{
								dbCheck.Reject = remark.Reject;
								dbCheck.Row = remark.Row;
							}
							//affected += await db.SaveChangesAsync();
						}
						affected = await db.SaveChangesAsync();
					}
					UnsavedRemarks = false;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to save job remarks.\n{ex}",
					"Job Save ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
				FormSet.LastSaved = "Job Remarks Save ERROR!";
				return;
			}
			#endregion Remarks
			// if any changes were saved in db, update last save time in db and on datasheet
			if (affected > 0)
            {
				await using (var db = new DigitalDatasheetContext())
                {
					JobForm jobForm = await db.JobForms.FindAsync(fullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
					jobForm.LastSaved = saveTime;
					await db.SaveChangesAsync();
				}
				FormSet.LastSaved = saveTime != null ? saveTime.ToString() : string.Empty;
			}
			UnsavedChanges = false;
			await Load_Recent_Jobs();
		}
		private void AutoSave_Click(object sender, RoutedEventArgs e)
		{
			AutoSaveLabel = AutoSave ? "Auto Save ON" : "Auto Save OFF";
			AutoSaveDecoration = AutoSave ? new SolidColorBrush(Colors.Green) : new SolidColorBrush(Colors.Red);
		}
		private void Close_Window(object sender, RoutedEventArgs e)
		{
			Close();
		}
		#endregion File Menu

		#region Edit Menu
		private void Edit_StructureOrder_Click(object sender, RoutedEventArgs e)
        {

        }
		private void Edit_RemarksDocument_Menu_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				RemarksDocumentEditView editWin = new RemarksDocumentEditView();
				editWin.ShowDialog();
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to edit the remarks document.\n{ex}",
					"Remarks Document Edit ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		private async void Edit_SpecificationRequirements_Menu_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				SpecificationRequirementsEditView editWin = new SpecificationRequirementsEditView();
				editWin.ShowDialog();
				if (editWin.ResetSpecifications)
				{
					spec1_input.Items.Clear();
					spec2_input.Items.Clear();

					await Get_Specification();
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show($"An error has occurred while attemping to edit the specification requirements.\n{ex}",
					"Specification Edit ERROR",
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
		#endregion Edit Menu

		#region View Menu
		private void View_Notes_Click(object sender, RoutedEventArgs e)
		{
			if (string.IsNullOrEmpty(FormSet.FullWorkOrder) || string.IsNullOrEmpty(FormSet.TestCondition) || string.IsNullOrEmpty(FormSet.TestPerformedOn))
				return;
			JobNotesView noteWin = new JobNotesView(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
			noteWin.Show();
		}
		#endregion View Menu

		#region Create Menu
		private async void Create_Test_Report_Async(object sender, RoutedEventArgs e)
		{
			// make sure last changed element is updated despite TextBox not losing focus
			var focusObj = FocusManager.GetFocusedElement(this);
			if (focusObj != null && focusObj is TextBox)
			{
				var binding = (focusObj as TextBox).GetBindingExpression(TextBox.TextProperty);
				if (binding != null)
					binding.UpdateSource();
			}
			if (string.IsNullOrEmpty(FormSet.FullWorkOrder) || string.IsNullOrEmpty(FormSet.TestPerformedOn)) return;
			if (await Check_Incomplete_Notes())
			{
				if (MessageBox.Show("There are job notes that have not yet been completed. Would you like to continue anyway?", "Incomplete Job Notes!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					return;
			}
			if (UnsavedChanges = await Unsaved_Changes_CheckAsync())
			{
				MessageBoxResult saveJob = MessageBox.Show("To create a Test Report, you must first save the current job. Would you like to continue?",
					"Save & Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (saveJob == MessageBoxResult.No) return;
				await Save_Job();
			}
			try
			{
				var testReport = new TestReport(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.PartNumber, FormSet.DateCode, FormSet.TestPerformedOn);
				int dataPerPage;
				// check for distinct serial numbers
				bool distinctSn = RecordSet.Where(r => r.StructureInfo.StructureOrder == 1).Select(data => new { data.SerialNumber }).Distinct().Count() == RecordSet.Where(r => r.StructureInfo.StructureOrder == 1).Count();
				if (StructureTitles.Count == 1)
				{
					dataPerPage = RecordSet.Count;
					if (dataPerPage > 6)
					{
						TestReportStructuresView win = new TestReportStructuresView();
						win.ShowDialog();
						if (!win.OkClick)
							return;
						dataPerPage = win.DataPerPage;
					}
					await using (var db = new DigitalDatasheetContext())
					{
						db.JobDataTable
							.Where(data => data.WorkOrderNumber.Equals(FormSet.FullWorkOrder) && data.TestCondition.Equals(FormSet.TestCondition) && data.TestPerformedOn.Equals(FormSet.TestPerformedOn))
							.OrderBy(data => data.Row)
							.ToList()
							.ForEach(data =>
							{
								List<string> dataRow = new List<string>
								{
									data.HoleCuPlating,
									data.ExternalConductor,
									data.SurfaceCladCu,
									data.WrapCu,
									data.CapCu,
									data.InternalCladCu,
									data.MinEtchback,
									data.MaxEtchback,
									data.InternalAnnularRing,
									data.ExternalAnnularRing,
									data.Dielectric,
									data.Wicking
								};
								testReport.Set_Single_Structure_Data_Row(data.SerialNumber, data.Location, dataRow, data.AcceptReject, distinctSn, dataPerPage);
							});
					}
					//await Task.Run(() => testReport.Set_Job_Info(StructureTitles[0].StructureTitle));
					testReport.Set_Job_Info(StructureTitles[0].StructureTitle);
				}
				else
				{
					List<string> structureList = new List<string>();
					foreach (var structure in StructureTitles)
					{
						if (string.IsNullOrEmpty(structure.StructureTitle) || !structure.StructureTitle.Contains(";"))
						{
							MessageBox.Show("All structure titles must be set before creating test report. Fill out any remaining structure titles and try again", "Incomplete Structure Titles", MessageBoxButton.OK, MessageBoxImage.Error);
							return;
						}
						structureList.Add(structure.StructureTitle);
					}
					TestReportStructuresView win = new TestReportStructuresView(structureList);
					win.Set_Layout();
					win.ShowDialog();
					if (!win.OkClick)
						return;

					// list of structure titles and their corresponding titles for the test report
					List<(string, string)> structureToReportStructure = win.StructureToReportStructure;
					List<string> reportStructures = new List<string>();
					foreach ((_, string report_structure) in structureToReportStructure)
						reportStructures.Add(report_structure);
					dataPerPage = win.DataPerPage;

					int snCount = RecordSet
						.Where(r => r.StructureInfo.StructureOrder == 1)
						.Count();
					var snRecords = RecordSet
						.Where(r => r.StructureInfo.StructureOrder == 1)
						.OrderBy(r => r.Row)
						.Select(data => new { data.SerialNumber, data.Row })
						.ToList();
					for (int i = 0; i < snCount; i++)
					{
						List<string> locations = new List<string>();
						List<List<string>> dataRows = new List<List<string>>();
						List<string> acceptRejects = new List<string>();
						/*var locRecords = */
						using (var db = new DigitalDatasheetContext())
						{
							db.JobDataTable
								.Where(data => data.WorkOrderNumber.Equals(FormSet.FullWorkOrder) && data.TestCondition.Equals(FormSet.TestCondition) && data.TestPerformedOn.Equals(FormSet.TestPerformedOn)
								&& data.SerialNumber == snRecords[i].SerialNumber && data.Row == snRecords[i].Row)
								.OrderBy(data => data.StructureOrder)
								.ToList()
								.ForEach(data =>
								{
									locations.Add(data.Location);
									dataRows.Add(new List<string>
									{
										data.HoleCuPlating,
										data.ExternalConductor,
										data.SurfaceCladCu,
										data.WrapCu,
										data.CapCu,
										data.InternalCladCu,
										data.MinEtchback,
										data.MaxEtchback,
										data.InternalAnnularRing,
										data.ExternalAnnularRing,
										data.Dielectric,
										data.Wicking
									});
									acceptRejects.Add(data.AcceptReject);
								});
						}
						await Task.Run(() => testReport.Set_Multiple_Structure_Data_Rows(snRecords[i].SerialNumber, locations, reportStructures, dataRows, acceptRejects, distinctSn, dataPerPage * reportStructures.Count));
					}
					await Task.Run(testReport.Underline_Serial_Number_Titles);
					await Task.Run(() => testReport.Set_Job_Info());
				}
				foreach (var remark in Remarks)
				{
					if (remark.Remark.ToLower().StartsWith("internal layers:"))
					{
						await Task.Run(() => testReport.Set_Internal_Layers(remark.Remark));
						break;
					}
				}
				await Task.Run(testReport.Set_Reject_Background_Color);
				List<string> requirementList = new List<string>
				{
					Requirement.HoleCuPlating,
					Requirement.ExternalConductor,
					Requirement.SurfaceCladCu,
					Requirement.WrapCu,
					Requirement.CapCu,
					Requirement.InternalCladCu,
					Requirement.MinEtchback,
					Requirement.MaxEtchback,
					Requirement.InternalAnnularRing,
					Requirement.ExternalAnnularRing,
					Requirement.Dielectric,
					Requirement.Wicking
				};
				await Task.Run(() => testReport.Set_Requirements(requirementList));
				await testReport.Save_And_Close(FormSet.Customer);
			}
			catch (Exception)
			{
				MessageBox.Show("Test Report creation error. Please check error log file for details.");
			}
		}
		private async void Create_Hard_Copy_Async(object sender, RoutedEventArgs e)
		{
			if (!NetConn)
			{
				MessageBox.Show("No network connection");
				return;
			}
            if (string.IsNullOrEmpty(FormSet.FullWorkOrder) || string.IsNullOrEmpty(FormSet.TestPerformedOn)) return;
			if (await Check_Incomplete_Notes())
            {
				if (MessageBox.Show("There are job notes that have not yet been completed. Would you like to continue anyway?", "Incomplete Job Notes!", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No)
					return;
            }
            //if (!Valid_Save_Check()) return;
            if (UnsavedChanges = await Unsaved_Changes_CheckAsync())
			{
				MessageBoxResult saveJob = MessageBox.Show("To create a Hard Copy, you must first save the current job. Would you like to continue?",
					"Save & Continue?", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (saveJob == MessageBoxResult.No) return;
				await Save_Job();
			}

			List<string> headerInfo = new List<string>
			{
				FormSet.DateTested.HasValue ? FormSet.DateTested.Value.ToShortDateString() : string.Empty,
				FormSet.TestedBy,
				FormSet.CheckedBy
			};
			List<string> requirements = new List<string>
			{
				Requirement.HoleCuPlating,
				Requirement.ExternalConductor,
				Requirement.SurfaceCladCu,
				Requirement.WrapCu,
				Requirement.CapCu,
				Requirement.InternalCladCu,
				Requirement.MinEtchback,
				Requirement.MaxEtchback,
				Requirement.InternalAnnularRing,
				Requirement.ExternalAnnularRing,
				Requirement.Dielectric,
				Requirement.Wicking
			};
			List<(string, bool)> remarkSet = Remarks.OrderBy(r => r.Row).Select(r => (r.Remark, r.Reject)).ToList();
			try
			{
				HardCopy hardCopy = new HardCopy(FormSet.FullWorkOrder, FormSet.TestCondition, FormSet.TestPerformedOn);
				await Task.Run(() => hardCopy.SetJobInfo(FormSet));
				await hardCopy.SetData();
				await Task.Run(() => hardCopy.SetRequirements(requirements));
				await Task.Run(() => hardCopy.SetHeaderInfo(headerInfo));
				await Task.Run(() => hardCopy.SetRemarks(remarkSet));
				await hardCopy.SaveAndClose(FormSet.Customer);
			}
			catch
			{
				MessageBox.Show($"Hard Copy error. Please check error log file for details.");
			}
		}
		#endregion Create Menu

		#endregion Menu Functions

		private async void Window_Closing(object sender, EventArgs e)
		{
			//IsClosed = true;
			if (UnsavedChanges = await Unsaved_Changes_CheckAsync())
			{
				MessageBoxResult saveBeforeClose = MessageBox.Show("There may currently be unsaved changes in this job. Would you like to save before closing?",
					"Unsaved Changes!",
					MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
				if (saveBeforeClose == MessageBoxResult.Yes)
				{
					if (!Valid_Save_Check())
					{
						((CancelEventArgs)e).Cancel = true;
						return;
					}
					await Save_Job();
				}
				else if (saveBeforeClose == MessageBoxResult.Cancel)
				{
					((CancelEventArgs)e).Cancel = true;
					return;
				}
				UnsavedChanges = false;
			}
		}
		private /*async*/ void Window_Closed(object sender, EventArgs e)
		{
			IsClosed = true;
			//if (UnsavedChanges)
			//{
			//	//var saveTask = Save_Job();
			//	//saveTask.Wait();
			//	await Save_Job();
			//}
		}
		private void Job_Grid_KeyDown(object sender, KeyEventArgs e)
		{
            //try
            //{
            //    // if a key was press in any TextBox, mark that there has been a change
            //    //UnsavedChanges = await Unsaved_Changes_CheckAsync();
            //    if (UnsavedChanges)
            //        return;
            //    var focusObj = FocusManager.GetFocusedElement(this);
            //    if (focusObj != null && focusObj is TextBox)
            //    {
            //        UnsavedChanges = true;
            //        //IsSaved = false;
            //    }
            //}
            //catch (Exception) { }
        }
		private async Task<bool> Check_Incomplete_Notes()
        {
			await using (var db = new DigitalDatasheetContext())
            {
				var jobNotes = await db.JobNotes
					.Where(jobNote => jobNote.WorkOrderNumber.Equals(FormSet.FullWorkOrder) && jobNote.TestCondition.Equals(FormSet.TestCondition) && jobNote.TestPerformedOn.Equals(FormSet.TestPerformedOn))
					.OrderBy(jobNote => jobNote.DateAdded)
					.ToListAsync();
				if (jobNotes.Count == 0) return false;
				var incJobNote = jobNotes.Find(jobNote => !jobNote.Completed);
				if (incJobNote is null)
					return false;
			}
			return true;
        }
		private void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
    }
}
