using System.Drawing;

public enum Direction
{
    Right,
    Up,
    Left,
    Down
}

public abstract class GameObject
{
    public Game Game { get; set; }
    public Direction Direction { get; set; }
    public bool IsSolid { get; set; }
    public bool IsVisible { get; set; } = true;
    public virtual string ImageId { get; set; }

    Point position;

    // Use this if changing both coordinates.
    public Point Position
    {
        get
        {
            return position;
        }
        set
        {
            var lastPos = position;
            position = value;
            Game.TriggerPositionChanged(this, lastPos);
        }
    }

    public int X
    {
        get
        {
            return Position.X;
        }
        set
        {
            Position = new Point(value, Position.Y);
        }
    }

    public int Y
    {
        get
        {
            return Position.Y;
        }
        set
        {
            Position = new Point(Position.X, value);
        }
    }

    public void SubscribeCollision<T>(CollisionEventHandler handler) where T : GameObject
    {
        // TODO: Again check that this.GetType() returns the type of bottom-most derived class type (not the base class type).
        Game.SubscribeCollision<T>(this.GetType(), handler);
    }

    public GameObject(Game game)
    {
        Game = game;
    }
}
