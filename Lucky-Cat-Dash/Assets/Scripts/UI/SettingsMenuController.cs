using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenuController : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private TMP_Text masterValueText;
    [SerializeField] private TMP_Text musicValueText;
    [SerializeField] private TMP_Text sfxValueText;

    [Header("Video")]
    [SerializeField] private Toggle fullscreenToggle;
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown qualityDropdown;

    private Resolution[] resolutions;
    private GameSettingsData current;
    private readonly List<Resolution> filtered = new();

    private void Start()
    {
        current = SettingsSystem.Load();

        SetupQualityDropdown();
        SetupResolutionDropdown();
        BindUI();
        PopulateUIFromData(current);
        ApplySettings(current, save: false);
    }

    public void OpenSettings()
    {
        mainButtonsPanel.SetActive(false);
        settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        settingsPanel.SetActive(false);
        mainButtonsPanel.SetActive(true);
    }

    public void ApplyPressed()
    {
        ReadUIToData(current);
        ApplySettings(current, save: true);
    }

    private void SetupQualityDropdown()
    {
        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
    }

    private void SetupResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        filtered.Clear();

        HashSet<string> seen = new();
        foreach (var r in resolutions)
        {
            string key = $"{r.width}x{r.height}";
            if (seen.Add(key)) filtered.Add(r);
        }

        var opts = new List<string>();
        for (int i = 0; i < filtered.Count; i++)
            opts.Add($"{filtered[i].width} x {filtered[i].height}");

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(opts);
    }

    private void BindUI()
    {
        masterSlider.onValueChanged.AddListener(v => masterValueText.text = Mathf.RoundToInt(v * 100) + "%");
        musicSlider.onValueChanged.AddListener(v => musicValueText.text = Mathf.RoundToInt(v * 100) + "%");
        sfxSlider.onValueChanged.AddListener(v => sfxValueText.text = Mathf.RoundToInt(v * 100) + "%");
    }

    private void PopulateUIFromData(GameSettingsData d)
    {
        masterSlider.value = d.masterVolume;
        musicSlider.value = d.musicVolume;
        sfxSlider.value = d.sfxVolume;

        fullscreenToggle.isOn = d.fullscreen;

        qualityDropdown.value = Mathf.Clamp(d.qualityIndex, 0, QualitySettings.names.Length - 1);
        resolutionDropdown.value = Mathf.Clamp(d.resolutionIndex, 0, Mathf.Max(0, filtered.Count - 1));

        masterValueText.text = Mathf.RoundToInt(masterSlider.value * 100) + "%";
        musicValueText.text = Mathf.RoundToInt(musicSlider.value * 100) + "%";
        sfxValueText.text = Mathf.RoundToInt(sfxSlider.value * 100) + "%";
    }

    private void ReadUIToData(GameSettingsData d)
    {
        d.masterVolume = masterSlider.value;
        d.musicVolume = musicSlider.value;
        d.sfxVolume = sfxSlider.value;
        d.fullscreen = fullscreenToggle.isOn;
        d.qualityIndex = qualityDropdown.value;
        d.resolutionIndex = resolutionDropdown.value;
    }

    private void ApplySettings(GameSettingsData d, bool save)
    {
        // Global volume (will route music/sfx through AudioMixer later)
        AudioListener.volume = d.masterVolume;

        QualitySettings.SetQualityLevel(d.qualityIndex);

        int idx = Mathf.Clamp(d.resolutionIndex, 0, Mathf.Max(0, filtered.Count - 1));
        if (filtered.Count > 0)
        {
            var r = filtered[idx];
            Screen.SetResolution(r.width, r.height, d.fullscreen);
        }
        else
        {
            Screen.fullScreen = d.fullscreen;
        }

        if (save) SettingsSystem.Save(d);
    }
}