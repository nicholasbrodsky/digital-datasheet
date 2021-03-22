using DigitalDatasheet.Data;
using DigitalDatasheetContextLib;
using DigitalDatasheetEntityLib;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DigitalDatasheet.Views
{
    /// <summary>
    /// Interaction logic for JobNotesView.xaml
    /// </summary>
    public partial class JobNotesView : Window, INotifyPropertyChanged
    {
        #region Data Biding Elements
        private string user;
        private string note;

        public string User
        {
            get { return user; }
            set { user = value; OnPropertyChanged(); }
        }
        public string Note
        {
            get { return note; }
            set { note = value; OnPropertyChanged(); }
        }
        #endregion Data Biding Elements

        private DateTime dateUpdated;
        private List<(string, string, DateTime, DateTime)> noteSetList;
        public DateTime DateUpdated
        {
            get { return dateUpdated; }
            set { dateUpdated = value; OnPropertyChanged(); }
        }
        public List<(string user, string note, DateTime dateAdded, DateTime dateUpdated)> NoteSetList
        {
            get => noteSetList;
            set { noteSetList = value; OnPropertyChanged(); }
        }
        public string WorkOrderNumber { get; set; }
        public string TestCondition { get; set; }
        public string TestPerformedOn { get; set; }
        public DateTime DateAdded { get; set; }
        private int noteIndex = -1;

        public JobNotesView(string workOrderNumber, string testCondition, string testPerformedOn)
        {
            WorkOrderNumber = workOrderNumber;
            TestCondition = testCondition;
            TestPerformedOn = testPerformedOn;
            NoteSetList = new List<(string user, string note, DateTime dateAdded, DateTime dateUpdated)>();

            DataContext = this;
            InitializeComponent();

            _ = GetNotes();
        }
        private async Task GetNotes()
        {
            if (notesGrid.Children.Count > 0)
                notesGrid.Children.Clear();
            if (notesGrid.RowDefinitions.Count > 0)
                notesGrid.RowDefinitions.Clear();
            NoteSetList.Clear();

            StackPanel imageStackPanel;
            Image completeImage, editImage, deleteImage;
            Button completeButton, editButton, deleteButton;
            TextBlock date, user, note;
            RowDefinition rowDefinition;
            SolidColorBrush background;

            await using (var db = new DigitalDatasheetContext())
            {
                var jobNotes = await db.JobNotes
                    .Where(jobNote => jobNote.WorkOrderNumber.Equals(WorkOrderNumber) && jobNote.TestCondition.Equals(TestCondition) && jobNote.TestPerformedOn.Equals(TestPerformedOn))
                    .OrderBy(jobNote => jobNote.DateAdded)
                    .ToListAsync();
                int row = 0;
                jobNotes.ForEach(jobNote =>
                {
                    //NoteSetList.Add((user: jobNote.User, note: jobNote.Note, dateAdded: jobNote.DateAdded, dateUpdated: jobNote.DateUpdated));
                    NoteSetList.Insert(row, (user: jobNote.User, note: jobNote.Note, dateAdded: jobNote.DateAdded, dateUpdated: jobNote.DateUpdated));

                    if (row % 2 == 0)
                        background = new SolidColorBrush(Color.FromRgb(243, 243, 243));
                    else
                        background = new SolidColorBrush(Colors.Transparent);

                    imageStackPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        Margin = new Thickness(2),
                        VerticalAlignment = VerticalAlignment.Top
                    };
                    completeImage = new Image
                    {
                        Source = new BitmapImage(new Uri("../Images/success_icon.png", UriKind.Relative)),
                        //Height = 15,
                        //Width = 15,
                        //Cursor = Cursors.Hand,
                        //VerticalAlignment = VerticalAlignment.Top
                    };
                    completeButton = new Button
                    {
                        Content = completeImage,
                        Height = 20,
                        Width = 20,
                        Cursor = Cursors.Hand,
                        Margin = new Thickness(0, 0, 2, 0),
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        Tag = $"{row}",
                        ToolTip = new ToolTip
                        {
                            Content = "Set or unset note as completed",
                            Padding = new Thickness(4)
                        }
                    };
                    completeButton.Click += new RoutedEventHandler(CompleteNote_Click);

                    editImage = new Image
                    {
                        Source = new BitmapImage(new Uri("../Images/edit_icon.png", UriKind.Relative)),
                        //Height = 15,
                        //Width = 15,
                        //Cursor = Cursors.Hand,
                        //VerticalAlignment = VerticalAlignment.Top
                    };
                    editButton = new Button
                    {
                        Content = editImage,
                        Height = 20,
                        Width = 20,
                        Cursor = Cursors.Hand,
                        Margin = new Thickness(0, 0, 2, 0),
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        Tag = $"{row}",
                        ToolTip = new ToolTip
                        {
                            Content = "Edit this note",
                            Padding = new Thickness(4)
                        }
                    };
                    editButton.Click += new RoutedEventHandler(EditNote_Click);

                    deleteImage = new Image
                    {
                        Source = new BitmapImage(new Uri("../Images/delete_icon.png", UriKind.Relative)),
                        //Height = 15,
                        //Width = 15,
                        //Cursor = Cursors.Hand,
                        //VerticalAlignment = VerticalAlignment.Top
                    };
                    deleteButton = new Button
                    {
                        Content = deleteImage,
                        Height = 20,
                        Width = 20,
                        Cursor = Cursors.Hand,
                        Background = new SolidColorBrush(Colors.Transparent),
                        BorderBrush = new SolidColorBrush(Colors.LightGray),
                        Tag = $"{row}",
                        ToolTip = new ToolTip
                        {
                            Content = "Permanently remove this note",
                            Padding = new Thickness(4)
                        }
                    };
                    deleteButton.Click += new RoutedEventHandler(DeleteNote_Click);

                    imageStackPanel.Children.Add(completeButton);
                    imageStackPanel.Children.Add(editButton);
                    imageStackPanel.Children.Add(deleteButton);
                    date = new TextBlock
                    {
                        Text = $"{jobNote.DateUpdated}",
                        FontSize = 13,
                        Padding = new Thickness(1, 4, 0, 4),
                        Background = background
                    };
                    user = new TextBlock
                    {
                        Text = jobNote.User,
                        FontSize = 13,
                        Padding = new Thickness(1, 4, 0, 4),
                        Background = background
                    };
                    note = new TextBlock
                    {
                        Text = jobNote.Note,
                        FontSize = 13,
                        Padding = new Thickness(1, 4, 0, 4),
                        Background = background,
                        TextWrapping = TextWrapping.Wrap,
                        ToolTip = new ToolTip
                        {
                            Content = $"Note added: {jobNote.DateAdded}",
                            Padding = new Thickness(4)
                        },
                        TextDecorations = jobNote.Completed ? TextDecorations.Strikethrough : null
                    };

                    rowDefinition = new RowDefinition
                    {
                        Height = new GridLength(0, GridUnitType.Auto)
                    };
                    notesGrid.RowDefinitions.Add(rowDefinition);

                    Grid.SetRow(imageStackPanel, row);
                    Grid.SetColumn(imageStackPanel, 0);
                    Grid.SetRow(date, row);
                    Grid.SetColumn(date, 1);
                    Grid.SetRow(user, row);
                    Grid.SetColumn(user, 2);
                    Grid.SetRow(note, row);
                    Grid.SetColumn(note, 3);

                    notesGrid.Children.Add(imageStackPanel);
                    notesGrid.Children.Add(date);
                    notesGrid.Children.Add(user);
                    notesGrid.Children.Add(note);
                    row++;
                });
            }
        }
        private async void AddNote_Click(object sender, RoutedEventArgs e)
        {
            if (!userInput.IsEnabled)
            {
                MessageBox.Show("A note is currently being edited. Click Update to complete the process or Clear to get out of edit mode.");
                return;
            }
            if (string.IsNullOrEmpty(User) || string.IsNullOrEmpty(Note))
            {
                MessageBox.Show($"Both User and Note fields must be filled to continue.");
                return;
            }
            DateUpdated = DateTime.Now;

            await using (var db = new DigitalDatasheetContext())
            {
                var jobNotes = db.JobNotes.Where(note => note.Note.Equals(Note));
                if (jobNotes.Count() != 0)
                {
                    MessageBox.Show($"This note already exists for this job.");
                    return;
                }
                JobNote jobNote = new JobNote
                {
                    WorkOrderNumber = WorkOrderNumber,
                    TestCondition = TestCondition,
                    TestPerformedOn = TestPerformedOn,
                    User = User,
                    Note = Note,
                    DateAdded = DateUpdated,
                    DateUpdated = DateUpdated,
                    Completed = false
                };
                await db.JobNotes.AddAsync(jobNote);
                int affected = await db.SaveChangesAsync();
                if (affected == 1)
                {
                    User = string.Empty;
                    Note = string.Empty;
                }
                else
                {
                    MessageBox.Show("Error adding note");
                    return;
                }
            }
            await GetNotes();
        }
        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Tag is null) return;
            bool success = int.TryParse(button.Tag.ToString(), out noteIndex);
            if (!success)
            {
                MessageBox.Show("edit error");
                return;
            }
            var noteSet = NoteSetList[noteIndex];
            User = noteSet.user;
            Note = noteSet.note;
            userInput.IsEnabled = false;
        }
        private async void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (noteIndex == -1)
            {
                MessageBox.Show("Select the edit icon next to the note you want to edit before updating.");
                return;
            }
            var noteSet = NoteSetList[noteIndex];

            await using (var db = new DigitalDatasheetContext())
            {
                JobNote jobNote = await db.JobNotes.FindAsync(WorkOrderNumber, TestCondition, TestPerformedOn, User, noteSet.dateAdded);
                if (jobNote is null) return;
                jobNote.DateUpdated = DateTime.Now;
                jobNote.Note = Note;
                jobNote.Completed = false;
                int affected = await db.SaveChangesAsync();
                if (affected != 1)
                    MessageBox.Show("Note was not updated");
            }
            userInput.IsEnabled = true;
            User = string.Empty;
            Note = string.Empty;
            noteIndex = -1;

            await GetNotes();

        }
        private async void DeleteNote_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to permantly remove this note?", "Delete Note", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.No) return;
            Button button = sender as Button;
            if (button.Tag is null) return;
            bool success = int.TryParse(button.Tag.ToString(), out noteIndex);
            if (!success)
            {
                MessageBox.Show("delete error");
                return;
            }
            var noteSet = NoteSetList[noteIndex];
            await using (var db = new DigitalDatasheetContext())
            {
                JobNote jobNote = await db.JobNotes.FindAsync(WorkOrderNumber, TestCondition, TestPerformedOn, noteSet.user, noteSet.dateAdded);
                if (jobNote is null) return;
                db.JobNotes.Remove(jobNote);
                int affected = await db.SaveChangesAsync();
                if (affected != 1)
                    MessageBox.Show("Note deletion error");
            }
            User = string.Empty;
            Note = string.Empty;
            userInput.IsEnabled = true;
            noteIndex = -1;
            await GetNotes();
        }
        private async void CompleteNote_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            if (button.Tag is null) return;
            bool success = int.TryParse(button.Tag.ToString(), out noteIndex);
            if (!success)
            {
                MessageBox.Show("Note completion error");
                return;
            }
            var noteSet = NoteSetList[noteIndex];

            await using (var db = new DigitalDatasheetContext())
            {
                JobNote jobNote = await db.JobNotes.FindAsync(WorkOrderNumber, TestCondition, TestPerformedOn, noteSet.user, noteSet.dateAdded);
                jobNote.Completed = !jobNote.Completed;
                jobNote.DateUpdated = DateTime.Now;
                int affected = await db.SaveChangesAsync();
                if (affected != 1)
                    MessageBox.Show("Note deletion error");
            }
            noteIndex = -1;
            await GetNotes();
        }
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            userInput.IsEnabled = true;
            User = string.Empty;
            Note = string.Empty;
            noteIndex = -1;
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
