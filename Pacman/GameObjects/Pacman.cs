using System.Windows.Forms;

class Pacman : GameObject
{
    Handlers handlers;

    /// <summary>
    /// Pacman must handle keypresses. After keypress, pacman change direction of view(rotate image)
    /// and change position in this direction
    /// </summary>
    /// <param name="game"></param>
    /// <param name="handlers"></param>
    public Pacman(Game game, Handlers handlers) : base(game)
    {
        this.handlers = handlers;
        //Add handlers to Keypress events.
        Game.SubscribeKeyPress(Keys.A, OnAPressed);
        Game.SubscribeKeyPress(Keys.S, OnSPressed);
        game.SubscribeKeyPress(Keys.D, OnDPressed);
        game.SubscribeKeyPress(Keys.W, OnWPressed);
        //When pacman is on position of other GameObject, handle collision.
        SubscribeCollision<Monster>(handlers.OnMonsterCollision);
        SubscribeCollision<Cookie>(handlers.OnCookieCollision);
        SubscribeCollision<Star>(handlers.OnStarCollision);
    }

    public override string ImageId { get => handlers.pacmanImage; }
    public int Lives { get; set; } = 3;

    /// <summary>
    /// Key A press handler, move pacman LEFT
    /// </summary>
    /// <param name="key"></param>
    /// <param name="game"></param>
    private void OnAPressed(Keys key, Game game)
    {
        if (!game.IsSolid(X - 1, Y))
            this.X = this.X - 1;
        this.Direction = Direction.Left;
    }

    /// <summary>
    /// Key s press handler, move pacman DOWN
    /// </summary>
    /// <param name="key"></param>
    /// <param name="game"></param>
    private void OnSPressed(Keys key, Game game)
    {
        if (!game.IsSolid(X, Y + 1))
            this.Y = this.Y + 1;
        this.Direction = Direction.Down;
    }

    /// <summary>
    /// Key D press handler, move pacman RIGHT.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="game"></param>
    private void OnDPressed(Keys key, Game game)
    {
        if (!game.IsSolid(X + 1, Y))
            this.X = this.X + 1;
        this.Direction = Direction.Right;
    }

    /// <summary>
    /// Key W press handler, move pacman UP.
    /// </summary>
    /// <param name="key"></param>
    /// <param name="game"></param>
    private void OnWPressed(Keys key, Game game)
    {
        if (!game.IsSolid(X, Y - 1))
            this.Y = this.Y - 1;
        this.Direction = Direction.Up;
    }
}
