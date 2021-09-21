using System;
using System.Drawing;

class Monster : GameObject
{
    Point spawn;

    public Monster(Game game, Handlers handlers) : base(game)
    {
        ImageId = "monster.gif";
        game.SetTicker(10, Move); //Set move ticker, to period moving after 10s
        game.SetTicker(15, ChangeDirection); //After every 15 seconds random change direction.
        SubscribeCollision<Pacman>(handlers.OnMonsterCollision); //When monster colide with Pacman, kill pacman or monster. Depends on huntermode.
    }

    /// <summary>
    /// Set monster spawn X position.
    /// </summary>
    public int SpawnX
    {
        get
        {
            return spawn.X;
        }
        set
        {
            X = value;
            spawn.X = value;
        }
    }

    /// <summary>
    /// Set monster spawn Y position.
    /// </summary>
    public int SpawnY
    {
        get
        {
            return spawn.Y;
        }
        set
        {
            Y = value;
            spawn.Y = value;
        }
    }

    /// <summary>
    /// Move Handler. When moster shloud move, move in setted direction (By direction enum).
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="game"></param>
    public void Move(Timer sender, Game game)
    {
        switch (this.Direction)
        {
            case Direction.Down:
                if (!game.IsSolid(X, Y + 1))
                    Y = Y + 1;
                break;
            case Direction.Up:
                if (!game.IsSolid(X, Y - 1))
                    Y = Y - 1;
                break;
            case Direction.Right:
                if (!game.IsSolid(X + 1, Y))
                    X = X + 1;
                break;
            case Direction.Left:
                if (!game.IsSolid(X - 1, Y))
                    X = X - 1;
                break;
        }
    }

    /// <summary>
    /// Change direction handler. When its triggered, changes the monster direction.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="game"></param>
    public void ChangeDirection(Timer sender, Game game)
    {
        var random = new Random();
        int newDir = random.Next(0, 4 * this.SpawnX + this.SpawnY) % 4;
        this.Direction = (Direction)newDir;
    }

    /// <summary>
    /// Respawning monster on the setted spawn.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="game"></param>
    public void OnRespawn(Timer sender, Game game)
    {
        IsVisible = true;
        X = SpawnX;
        Y = SpawnY;
    }
}
