using System.Collections;
using System.Collections.Generic;


public static class PlayerData
{
    private static string playerName = "";
    private static float musicVolume;
    private static float sfxVolume;

    public static string PlayerName
    {
        get
        {
            return playerName;
        }
        set
        {
            playerName = value;
        }

    }
}
