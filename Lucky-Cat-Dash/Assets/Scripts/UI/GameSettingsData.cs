using System;

[Serializable]
public class GameSettingsData
{
    public float masterVolume = 1f;
    public float musicVolume = 1f;
    public float sfxVolume = 1f;
    public bool fullscreen = true;
    public int qualityIndex = 2;
    public int resolutionIndex = 0;
}