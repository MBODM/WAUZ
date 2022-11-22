using WAUZ.BL;

namespace WAUZ
{
    public partial class MainForm : Form
    {
        private readonly IBusinessLogic businessLogic;
        private readonly IPathHelper pathHelper;

        private CancellationToken unzipCancellationToken = CancellationToken.None;

        public MainForm(IBusinessLogic businessLogic, IPathHelper pathHelper)
        {
            this.businessLogic = businessLogic ?? throw new ArgumentNullException(nameof(businessLogic));
            this.pathHelper = pathHelper ?? throw new ArgumentNullException(nameof(pathHelper));

            InitializeComponent();

            MinimumSize = Size;

            textBoxSource.PlaceholderText = "The folder which contains the addon zip files. Normally some temporary download folder.";
            textBoxDest.PlaceholderText = "The folder to unzip the addons into. Normally the World of Warcraft AddOns folder.";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            businessLogic.LoadSettings();

            textBoxSource.Text = businessLogic.SourceFolder;
            textBoxDest.Text = businessLogic.DestFolder;
        }

        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            businessLogic.SourceFolder = textBoxSource.Text;
            businessLogic.DestFolder = textBoxDest.Text;

            businessLogic.SaveSettings();
        }

        private void ButtonSource_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxSource.Text))
            {
                SelectFolder(textBoxSource, textBoxSource.Text);
            }
            else
            {
                SelectFolder(textBoxSource, GetDesktopFolder());
            }
        }

        private void ButtonDest_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(textBoxDest.Text))
            {
                SelectFolder(textBoxDest, textBoxDest.Text);
            }
            else
            {
                var wowAddonsDefaultFolder = @"C:\Program Files (x86)\World of Warcraft\_retail_\Interface\AddOns";

                if (Directory.Exists(wowAddonsDefaultFolder))
                {
                    SelectFolder(textBoxDest, wowAddonsDefaultFolder);
                }
                else
                {
                    SelectFolder(textBoxDest, GetDesktopFolder());
                }
            }
        }

        private async void ButtonUnzip_Click(object sender, EventArgs e)
        {
            if (unzipCancellationToken == CancellationToken.None)
            {
                businessLogic.SourceFolder = textBoxSource.Text;
                businessLogic.DestFolder = textBoxDest.Text;

                try
                {
                    businessLogic.ValidateSettings();
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);

                    return;
                }

                progressBar.Maximum = businessLogic.GetSourceFolderZipFiles().Count();
                progressBar.Value = progressBar.Minimum;

                try
                {
                    await businessLogic.Unzip(new Progress<ProgressData>(_ =>
                    {
                        progressBar.Value++;
                        labelProgressBar.Text = $"Progress: Unzip {progressBar.Value} / {progressBar.Maximum} addons.";
                    }));
                }
                catch (Exception ex)
                {
                    ShowError(ex.Message);
                }

                labelProgressBar.Text = "Progress: All addons successfully unzipped.";
            }
            else
            {
                // Todo: Cancel operation
            }
        }

        private void SelectFolder(TextBox textBox, string startFolder)
        {
            using var dialog = new FolderBrowserDialog
            { 
                InitialDirectory = startFolder
            };

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                textBox.Text = pathHelper.TrimEndingDirectorySeparatorChar(dialog.SelectedPath);
            }
        }

        private static string GetDesktopFolder()
        {
            return Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
        }

        private static void ShowError(string errorText)
        {
            MessageBox.Show(errorText, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
