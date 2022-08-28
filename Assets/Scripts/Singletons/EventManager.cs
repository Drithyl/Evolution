using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EventManager
{
    public delegate void TurnPassed(object sender, TurnPassedArgs args);
    public delegate void MonthPassed(object sender, MonthPassedArgs args);




    public static EventManager Instance => instance;
    private static readonly EventManager instance = new EventManager();

    // Explicit static constructor to tell compiler
    // not to mark type as beforefieldinit (see
    // https://csharpindepth.com/articles/singleton)
    static EventManager() { }

    private EventManager() { }
}

public class TurnPassedArgs : EventArgs
{
    public int TurnNumber { get; set; }
}

public class MonthPassedArgs : EventArgs
{
    public int MonthNumber { get; set; }
}