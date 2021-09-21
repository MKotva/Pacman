using System.Windows.Forms;

public class Handlers
{
    private bool IsPacmanHunter;
    public string pacmanImage = "pacman.gif"; //Render normal pacman. 
    
    /// <summary>
    /// Handler of collision between pacman and monster.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="game"></param>
    public void OnMonsterCollision(GameObject cobj, GameObject obj, Game game)
    {
        Monster monster;
        Pacman pacman;

        if (obj is Monster)
        {
            monster = obj as Monster;
            pacman = cobj as Pacman;
        }
        else if (cobj is Monster)
        {
            monster = cobj as Monster;
            pacman = obj as Pacman;
        }
        else
            return;

        if (monster.IsVisible != true)
            return;

        if (!IsPacmanHunter) //Collision(Monster-Pacman) in non-hunter mode, resolute to gameover.
        {
            pacman.Lives--;
            if (pacman.Lives == 0)
            {
                string message = "Congratulations! You've died :).";
                string caption = "You are dead";
                MessageBoxButtons buttons = MessageBoxButtons.OK;
                DialogResult result;

                // Displays the MessageBox.
                result = MessageBox.Show(message, caption, buttons);
                if (result == DialogResult.OK)
                    System.Environment.Exit(1);
            }
            else
            {
                switch(pacman.Lives)
                {
                    case 1:
                         MessageBox.Show("You have one last live!", "Warning", MessageBoxButtons.OK);
                         break;
                    case 2:
                        MessageBox.Show("You have two more lives.", "Warning", MessageBoxButtons.OK);
                        break;
                }
            }
        }
        else
        {
            monster.IsVisible = false;
            game.SetAlarm(100, monster.OnRespawn); //Monster set position to spawn point
        }

        // Otherwise scream.
    }

    /// <summary>
    /// Handler of collision between pacman and cookie
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="game"></param>
    public void OnCookieCollision(GameObject cobj, GameObject obj, Game game)
    {
        if (obj.IsVisible)
        {
            obj.IsVisible = false;
            game.CookieCounter--;
        }
    }

    /// <summary>
    /// If Pacman eats the star(Pacman-Star Collision), change pacman to hunter mode.
    /// </summary>
    /// <param name="cobj"></param>
    /// <param name="obj"></param>
    /// <param name="game"></param>
    public void OnStarCollision(GameObject cobj, GameObject obj, Game game)
    {
        if (obj.IsVisible)
        {
            obj.IsVisible = false;
            pacmanImage = "angryPacman.gif"; //Change image render to huntermoded pacman.
            game.CookieCounter--;
            IsPacmanHunter = true;
            game.SetAlarm(100, SetPacmanToNonHunterHandler); //Set timer for hunterMode. If is not extendet, call set to no hunter.
        }
    }

    /// <summary>
    /// After huntermode time is out, change back to normal pacman.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="game"></param>
    public void SetPacmanToNonHunterHandler(Timer sender, Game game)
    {
        IsPacmanHunter = false;
        pacmanImage = "pacman.gif";
    }
}

