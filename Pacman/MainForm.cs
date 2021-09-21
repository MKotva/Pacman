using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

public partial class GameForm : Form
{
    Game game;
    public RenderingEngine engine;
    int fieldSize;
    int level = 0;
    string[] mapsPaths;

    private delegate void ReloadFormHandler(bool close);

    public GameForm(string path)
    {
        fieldSize = 32;
        mapsPaths = Directory.GetFiles(path); //Get levels maps.
        InitializeComponent();
    }

    /// <summary>
    /// Loading the first founded level.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void MainForm_Load(object sender, EventArgs e)
    {
        Init(mapsPaths[level]); //Load first level.
    }

    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        game.TriggerKeyPressed(sender, e);
    }

    /// <summary>
    /// Init selected level
    /// </summary>
    /// <param name="path"></param>
    private void Init(string path)
    {
        var builder = new GameFileBuilder(mapsPaths[level], new Handlers());
        game = builder.BuildGame(this, 50, 10/*this.Width / fieldSize*/, 10/*this.Height / fieldSize*/);

        var imageSource = new FileImageSource(@"C:\Users\MAIN\Documents\pacman\images");
        engine = new RenderingEngine(game, this, imageSource, fieldSize);
    }

    /// <summary>
    /// Initialize new gamemap and destroing old.
    /// </summary>
    public bool NextLevel
    {
        set
        {
            game.Dispose();
            level++;
            if (level != mapsPaths.Length) // If there is unplayed level. Load.
            {
                Init(mapsPaths[0]);
            }
            else
            {
                var winForm = new WinForm();
                winForm.ShowDialog();
                this.Close();
            }
                //System.Environment.Exit(1); // Else exit.
        }
    }
}

