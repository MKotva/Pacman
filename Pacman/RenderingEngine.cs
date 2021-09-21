using System;
using System.Drawing;
using System.Windows.Forms;

public class RenderingEngine
{
    Game game;
    Control canvas;
    IImageSource imageSource;
    int fieldSize;

    public RenderingEngine(Game game, Control canvas, IImageSource imageSource, int fieldSize)
    {
        this.game = game;
        this.canvas = canvas;
        this.imageSource = imageSource;
        this.fieldSize = fieldSize;

        game.SetTicker(Render); //Set ticker for period render.
    }

    /// <summary>
    /// When this handler is invoked, refresh game graphic.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="game"></param>
    private void Render(Timer sender, Game game)
    {
        var image = new Bitmap(canvas.Width, canvas.Height);
        var g = Graphics.FromImage(image);

        foreach (var obj in game.Objects)
            if (obj.IsVisible)
                DrawImage(obj, g, fieldSize);

        canvas.BackgroundImage = image;
    }

    /// <summary>
    /// By the GameObject, select Imgage and draw it to the bitmap.
    /// </summary>
    /// <param name="obj">GameObject(Pacman,Monster,Wall,Star,Cookie)</param>
    /// <param name="g">Instance of graphic</param>
    /// <param name="fieldSize">Size of field in pixels.</param>
    private void DrawImage(GameObject obj, Graphics g, int fieldSize)
    {
        var img = imageSource.GetImage(obj.ImageId);
        img.RotateFlip(GetRotationEnum(obj.Direction)); //Transformation angle calculation
        g.DrawImage(img, fieldSize * obj.X, fieldSize * obj.Y, fieldSize, fieldSize);
    }

    /// <summary>
    /// Searchs rotation angle by direction
    /// </summary>
    /// <param name="direction">Point of actual view</param>
    /// <returns></returns>
    private RotateFlipType GetRotationEnum(Direction direction)
    {
        switch (direction)
        {
            case Direction.Right:
                return RotateFlipType.RotateNoneFlipNone;

            case Direction.Left:
                return RotateFlipType.Rotate180FlipY;

            case Direction.Up:
                return RotateFlipType.Rotate270FlipNone;

            case Direction.Down:
                return RotateFlipType.Rotate90FlipNone;
        }
        throw new Exception("Rotation failed!");
    }
}
