using System.Collections;
using System.Collections.Generic;


public static class PlayerData
{
    private static string playerName = "";
    private static float musicVolume = 0.5f;
    private static float sfxVolume = 0.5f;

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
