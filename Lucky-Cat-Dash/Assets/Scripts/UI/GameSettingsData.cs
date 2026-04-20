using System;

[Serializable]
public class GameSettingsData
{
    public bool fullscreen = true;
    public int resolutionIndex = 0;
    public int qualityIndex = 2;

   
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
}