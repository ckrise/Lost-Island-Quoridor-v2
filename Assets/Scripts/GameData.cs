using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static bool isAIGame = true;
    private static bool isTutorial = false;
    private static bool playerGoesFirst = true;
    private static string aiDifficulty = "easy";
    private static NetworkController networkController;
    private static string scene = "TempleScene";
    private static string playerMove = "";
    private static string aiMove = "";
    private static bool inAdventureMode = false;
    private static int adventureProgress = 0;

    public static int AdventureProgress
    {
        get
        {
            return adventureProgress;
        }
        set
        {
            adventureProgress = value;
        }
    }
    public static bool InAdventureMode
    {
        get
        {
            return inAdventureMode;
        }
        set
        {
            inAdventureMode = value;
        }
    }
    public static string PlayerMove
    {
        get
        {
            return playerMove;
        }
        set
        {
            playerMove = value;
        }
    }
    public static string AIMove
    {
        get
        {
            return aiMove;
        }
        set
        {
            aiMove = value;
        }
    }
    public static string AIDifficulty
    {
        get
        {
            return aiDifficulty;
        }
        set
        {
            aiDifficulty = value;
        }
    }
   
    public static bool IsAIGame
    {
        get
        {
            return isAIGame;
        }
        set
        {
            isAIGame = value;
        }
    }
    public static bool PlayerGoesFirst
    {
        get
        {
            return playerGoesFirst;
        }
        set
        {
            playerGoesFirst = value;
        }
    }
    public static NetworkController NetworkController { get => networkController; set => networkController = value; }
    public static string Scene { get => scene; set => scene = value; }
    public static bool IsTutorial { get => isTutorial; set => isTutorial = value; }
}
