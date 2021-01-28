using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Text.RegularExpressions;

namespace SimpleTVFileRenamer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        public void ChooseDirSource()
        {
            FolderBrowserDialog fileDialog = new FolderBrowserDialog();
            DialogResult result = fileDialog.ShowDialog();

            if (result == System.Windows.Forms.DialogResult.OK)
            {
                DirectoryBox.Text = fileDialog.SelectedPath + @"\";
                LoadFiles();
            }
        }

        public void LoadFiles()
        {
            try
            {
                SourceFiles.Items.Clear();
                DestinationFiles.Items.Clear();

                DirectoryInfo d = new DirectoryInfo(DirectoryBox.Text);
                FileInfo[] Files = d.GetFiles();
                if (Files.Length < 1)
                {
                    System.Windows.MessageBox.Show("No files were found in this directory.", "Empty Directory");
                    DirectoryBox.Text = "";
                }
                else
                {
                    foreach (FileInfo file in Files)
                    {
                        SourceFiles.Items.Add(file.Name);
                        DestinationFiles.Items.Add(file.Name);
                    }
                }
            }
            catch { }
        }

        public void SetFileName()
        {
            int selectedIndex = SourceFiles.SelectedIndex;

            if (selectedIndex < 0)
            {
                System.Windows.MessageBox.Show("No file selected. Please select a file to be renamed.", "File Not Selected");
                return;
            }
            if (String.IsNullOrWhiteSpace(EpisodeTitleTextBox.Text) || String.IsNullOrWhiteSpace(SeasonNumTextBox.Text) || String.IsNullOrWhiteSpace(EpisodeNumTextBox.Text))
            {
                System.Windows.MessageBox.Show("Cannot set file name. Please fill in all required values.", "Empty Values Found");
                return;
            }

            int seasonNum = int.Parse(SeasonNumTextBox.Text);
            int episodeNum = int.Parse(EpisodeNumTextBox.Text);
            string episodeTitle = Regex.Replace(EpisodeTitleTextBox.Text.Trim().ToString(), @"[<>:/|?*\\""]", ""); // Regex checks for characters illegal in Windows file naming
            string ext = Path.GetExtension(DirectoryBox.Text + SourceFiles.Items[selectedIndex]);
            string output = "";

            if (!String.IsNullOrWhiteSpace(ShowNameTextBox.Text) && !String.IsNullOrWhiteSpace(YearTextBox.Text))
            {
                string showName = Regex.Replace(ShowNameTextBox.Text.Trim().ToString(), @"[<>:/|?*\\""]", "");
                int year = int.Parse(YearTextBox.Text);
                output += showName + " (" + year + ") - ";
            }
            else if (!String.IsNullOrWhiteSpace(ShowNameTextBox.Text))
            {
                string showName = Regex.Replace(ShowNameTextBox.Text.Trim().ToString(), @"[<>:/|?*\\""]", "");
                output += showName + " - ";
            }
            else if (!String.IsNullOrWhiteSpace(YearTextBox.Text))
            {
                int year = int.Parse(YearTextBox.Text);
                output += "(" + year + ") - ";
            }

            if (seasonNum <= 9)
            {
                output += "S0" + SeasonNumTextBox.Text;
            }
            else
            {
                output += "S" + SeasonNumTextBox.Text;
            }

            if (episodeNum <= 9)
            {
                output += "E0" + EpisodeNumTextBox.Text;
            }
            else
            {
                output += "E" + EpisodeNumTextBox.Text;
            }
            output += " - " + episodeTitle + ext;


            DestinationFiles.Items[selectedIndex] = output;
            EpisodeTitleTextBox.Clear();

            if (selectedIndex < (SourceFiles.Items.Count - 1))
            {
                if (EpisodeIncCheckBox.IsChecked == true)
                {
                    SourceFiles.SelectedIndex = selectedIndex + 1;
                    EpisodeNumTextBox.Text = $"{int.Parse(EpisodeNumTextBox.Text) + 1}";
                    EpisodeTitleTextBox.Focus();
                }
                else
                {
                    EpisodeNumTextBox.Clear();
                    EpisodeNumTextBox.Focus();
                }
            }
        }

        public void RenameFiles()
        {
            if (String.IsNullOrEmpty(DirectoryBox.Text))
            {
                System.Windows.MessageBox.Show("Cannot complete job. No directory was chosen.", "Directory Not Selected");
                return;
            }

            try
            {
                DirectoryInfo d = new DirectoryInfo(DirectoryBox.Text);
                FileInfo[] Files = d.GetFiles();

                int i = 0;
                foreach (FileInfo file in Files)
                {
                    System.IO.File.Move(file.FullName, $"{DirectoryBox.Text}{DestinationFiles.Items[i]}");
                    i++;
                }
            }
            catch { }

            LoadFiles();
            System.Windows.Forms.MessageBox.Show("Job completed successfully.", "Success");
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PathButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseDirSource();
        }

        private void SetFileNameButton_Click(object sender, RoutedEventArgs e)
        {
            SetFileName();
        }

        private void RenameButton_Click(object sender, RoutedEventArgs e)
        {
            RenameFiles();
        }
    }
}
