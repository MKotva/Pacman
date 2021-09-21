using Microsoft.Win32.SafeHandles;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

enum TimerKind
{
    Alarm,
    Ticker
}

public delegate void TimerEventHandler(Timer sender, Game game);
public delegate void KeyPressEventHandler(Keys key, Game game);
public delegate void CollisionEventHandler(GameObject cobj, GameObject obj, Game game); //Two objects in collision

public class Timer
{
    public int Step { get; set; }
    public int Duration { get; set; }
    public TimerEventHandler Handler { get; set; }
}

public class Collision
{
    public Type OtherType { get; set; }
    public CollisionEventHandler Handler { get; set; }
}

public class Game : IDisposable
{
    System.Windows.Forms.Timer timer;
    List<Timer> alarms;
    List<Timer> tickers;
    Dictionary<Keys, List<KeyPressEventHandler>> keyEvents;
    Dictionary<Type, List<Collision>> collisionEvents;
    List<GameObject>[,] map;
    HashSet<GameObject> movedObjects;
    GameForm myform;
    bool disposed = false;
    bool exited = false;
    SafeHandle handle = new SafeFileHandle(IntPtr.Zero, true);

    public Game(GameForm myform, int timerStep, int planWidth, int planHeight)
    {
        timer = new System.Windows.Forms.Timer();
        timer.Interval = timerStep;
        timer.Tick += TimerTick;
        timer.Enabled = true;
        alarms = new List<Timer>();
        tickers = new List<Timer>();
        keyEvents = new Dictionary<Keys, List<KeyPressEventHandler>>();
        collisionEvents = new Dictionary<Type, List<Collision>>();
        PlanWidth = planWidth;
        PlanHeight = planHeight;
        map = InitMap();
        movedObjects = new HashSet<GameObject>();
        SubscribeKeyPress(Keys.P, PauseGameEventHandler);
        this.myform = myform;
    }

    /// <summary>
    /// Returns all objects in game at single list.
    /// </summary>
    public IList<GameObject> Objects //List of all objects in game
    {
        get
        {
            var objects = new List<GameObject>();
            foreach (var obj in map)
                objects = objects.Concat(obj).ToList();

            return objects;
        }
    }

    public int CookieCounter { get; set; } //Count of cookies in game
    public int PlanWidth { get; private set; } // TODO: Use Size instead?
    public int PlanHeight { get; private set; }
    public ICollection<GameObject> this[int x, int y]
    {
        get
        {
            return map[x, y]; // TODO: What about index out-of-range?
        }
    }

    /// <summary>
    /// Add gameobject, to the map.
    /// </summary>
    /// <param name="obj"></param>
    public void AddObject(GameObject obj)
    {
        map[obj.X, obj.Y].Add(obj);
    }

    /// <summary>
    /// Checks if position near by is wall.
    /// </summary>
    /// <param name="newX"></param>
    /// <param name="newY"></param>
    /// <returns></returns>
    public bool IsSolid(int newX, int newY)
    {
        var newPosInPlan = map[newX, newY];
        if (newPosInPlan.Count != 0)
            if (newPosInPlan.First().IsSolid)
                return true;

        return false;
    }

    /// <summary>
    /// Initialize local map of playground. Create list for each field.
    /// </summary>
    /// <returns></returns>
    private List<GameObject>[,] InitMap()
    {
        var map = new List<GameObject>[PlanWidth, PlanHeight];

        for (int i = 0; i < PlanWidth; i++)
            for (int j = 0; j < PlanHeight; j++)
                map[i, j] = new List<GameObject>();

        return map;
    }

    /// <summary>
    /// Checks all changes of objects positions and triggers handlers if there is some collision. 
    /// </summary>
    private void CheckCollisions()
    {
        foreach (var cobj in movedObjects)
        {
            var t = cobj.GetType();
            if (map[cobj.X, cobj.Y].Count > 1 && collisionEvents.ContainsKey(t))
            {
                var collisions = collisionEvents[t]; // List of collision subscriptions for object `obj`.
                foreach (var obj in map[cobj.X, cobj.Y])
                {
                    var collision = collisions.Find((c) => (c.OtherType == obj.GetType()));
                    if (collision != null)
                    {
                        timer.Stop();
                        collision.Handler(cobj, obj, this);
                        timer.Start();
                    }
                }
            }
        }
        movedObjects.Clear(); //Removes all records about changes          
    }

    /// <summary>
    /// Subscribe handler to collision event. Searchs by type.
    /// </summary>
    /// <typeparam name="T">Type of GameObject</typeparam>
    /// <param name="t"></param>
    /// <param name="handler">Handler of collision with this type.</param>
    public void SubscribeCollision<T>(Type t, CollisionEventHandler handler) where T : GameObject
    {
        if (!collisionEvents.ContainsKey(t))
            collisionEvents[t] = new List<Collision>();

        var collision = new Collision() { OtherType = typeof(T), Handler = handler };
        collisionEvents[t].Add(collision);
    }

    /// <summary>
    /// When position setter is trigged, this method will move this object to new position and set new object to collision checklist.
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="lastPos"></param>
    public void TriggerPositionChanged(GameObject obj, Point lastPos)
    {
        map[lastPos.X, lastPos.Y].Remove(obj);
        map[obj.X, obj.Y].Add(obj);
        movedObjects.Add(obj);
    }

    /// <summary>
    /// Adds handlers of keypress.
    /// </summary>
    /// <param name="key">Key witch is expected</param>
    /// <param name="handler">Handler of keypress</param>
    public void SubscribeKeyPress(Keys key, KeyPressEventHandler handler)
    {
        if (!keyEvents.ContainsKey(key))
            keyEvents[key] = new List<KeyPressEventHandler>();

        keyEvents[key].Add(handler);
    }

    /// <summary>
    /// Finds handler of keypress event.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public void TriggerKeyPressed(object sender, KeyEventArgs e)//Fix
    {
        if (keyEvents.ContainsKey(e.KeyCode))
        {
            foreach (var handler in keyEvents[e.KeyCode])
            {
                if (handler != null)
                    handler(e.KeyCode, this); //Invoke handler.
            }
        }
    }

    /// <summary>
    /// Adds handler of timer tick.
    /// </summary>
    /// <param name="timers"></param>
    /// <param name="duration"></param>
    /// <param name="handler"></param>
    private void SetTimer(List<Timer> timers, int duration, TimerEventHandler handler)
    {
        timers.Add(new Timer() { Step = duration, Duration = duration, Handler = handler });
    }

    /// <summary>
    /// Alarm handle just once.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="handler"></param>
    public void SetAlarm(int duration, TimerEventHandler handler)
    {
        SetTimer(alarms, duration, handler);
    }

    public void SetAlarm(TimerEventHandler handler)
    {
        SetAlarm(1, handler);
    }

    /// <summary>
    /// Timer handle until game is ended.
    /// </summary>
    /// <param name="duration">After n steps invoke handler</param>
    /// <param name="handler"></param>
    public void SetTicker(int duration, TimerEventHandler handler)
    {
        SetTimer(tickers, duration, handler);
    }

    public void SetTicker(TimerEventHandler handler)
    {
        SetTicker(1, handler);
    }

    /// <summary>
    /// Handler of ticker tick.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void TimerTick(object sender, EventArgs e)
    {
        TouchTimers(alarms, true);
        TouchTimers(tickers, false);
    }

    /// <summary>
    /// If step is 0, invoke handler.
    /// </summary>
    /// <param name="timers"></param>
    /// <param name="removeOnExpire"></param>
    private void TouchTimers(List<Timer> timers, bool removeOnExpire)
    {
        for (int i = 0; i < timers.Count; i++)
        {
            timers[i].Step--;

            if (timers[i].Step == 0)
            {
                timers[i].Handler(timers[i], this);
                if (removeOnExpire) // TODO: Reimplement this (at least rename removeOnExpire).
                    timers.RemoveAt(i);
                else
                    timers[i].Step = timers[i].Duration;
            }
        }
        CheckCollisions();
        if (CookieCounter == 0 && !exited)
        {
            GameFinish();
        }
    }

    /// <summary>
    /// Pauses ticker when you want to pause game. 
    /// </summary>
    private void PauseGameEventHandler(Keys key, Game game)
    {
        if (timer.Enabled)
            timer.Stop();
        else
            timer.Start();
    }

    /// <summary>
    /// Stops the game, calls dispose
    /// </summary>
    public void GameFinish()
    {
        exited = true;
        timer.Tick -= TimerTick;
        timer.Stop();
        timer.Dispose();
        tickers.Clear();
        myform.NextLevel = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (disposed)
            return;

        if (disposing)
            handle.Dispose();

        disposed = true;
    }
}

