using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameData
{
    private static bool isAIGame = true;
    private static bool playerGoesFirst = true;
    private static string aiDifficulty = "Easy";
    private static NetworkController networkController;
    private static string scene = "AztecScene";

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
}
