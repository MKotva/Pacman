using System.IO;

public class GameFileBuilder
{
    Handlers handlers;
    string plan;
    public GameFileBuilder(string path, Handlers handlers)
    {
        this.handlers = handlers;
        this.plan = path;
    }

    /// <summary>
    /// Create new Game object. Reads map file and add them to our gameMap.
    /// </summary>
    /// <param name="form"></param>
    /// <param name="timerStep"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    public Game BuildGame(GameForm form, int timerStep, int width, int height)
    {
        var game = new Game(form, timerStep, width, height); // New game instance.
        int x = 0;

        using (var sr = new StreamReader(plan))
        {
            while (!sr.EndOfStream)
            {
                var chars = sr.ReadLine().ToCharArray();
                int y = 0;

                if (game.PlanWidth != chars.Length)
                    throw new System.Exception($"Wrong plan size at line {x}. Expected {game.PlanWidth}");

                foreach (var keyOfObject in chars)
                {
                    switch (keyOfObject)
                    {
                        case '#':
                            var wall = new Wall(game) { X = x, Y = y, IsSolid = true };
                            break;
                        case 'S':
                            game.CookieCounter++;
                            var star = new Star(game) { X = x, Y = y };
                            break;
                        case 'P':
                            var pacman = new Pacman(game, handlers) { X = x, Y = y };
                            break;
                        case 'M':
                            var monster = new Monster(game, handlers) { SpawnX = x, SpawnY = y };
                            break;
                        case 'C':
                            game.CookieCounter++;
                            var cookie = new Cookie(game) { X = x, Y = y };
                            break;
                        default:
                            throw new System.Exception($"Unknown char {keyOfObject}");
                    }
                    y++;
                }
                x++;
                if (x > game.PlanHeight)
                    throw new System.Exception($"Wrong plan size. Plan has {x} lines, but expected {game.PlanHeight}");
            }
        }
        return game;
    }
}