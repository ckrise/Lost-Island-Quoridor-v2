using System.Collections;
using System.Collections.Generic;


public static class PlayerData
{
    private static string playerName = "";
    private static float musicVolume = 1;
    private static float sfxVolume = 1;

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
    public static float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public static float SfxVolume { get => sfxVolume; set => sfxVolume = value; }
}
