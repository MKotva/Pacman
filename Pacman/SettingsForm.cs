using System;
using System.Windows.Forms;

public partial class SettingsForm : Form
{
    public SettingsForm()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Launch folder finder.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void button1_Click(object sender, EventArgs e)
    {
        var folderBrowser = new FolderBrowserDialog();
        if (folderBrowser.ShowDialog() == System.Windows.Forms.DialogResult.OK)
        {
            textBox1.Text = folderBrowser.SelectedPath;
        }
    }

    /// <summary>
    /// Launch game with setted path.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Start_Click(object sender, EventArgs e)
    {
        if(textBox1.Text == "")
        {
            var gameForm = new GameForm(@"C:\Users\MAIN\Pictures\pacman\maps");
            gameForm.ShowDialog();
        }
        else
        {
            var gameForm = new GameForm(textBox1.Text);
            gameForm.ShowDialog();
        }
    }
}
