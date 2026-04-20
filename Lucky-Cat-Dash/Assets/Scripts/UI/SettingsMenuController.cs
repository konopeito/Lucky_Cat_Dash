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

        // Standard startup: settings hidden, main shown
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void OpenSettings()
    {
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(false);
        if (settingsPanel != null) settingsPanel.SetActive(true);
    }

    public void CloseSettings()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (mainButtonsPanel != null) mainButtonsPanel.SetActive(true);
    }

    public void ApplyPressed()
    {
        ReadUIToData(current);
        ApplySettings(current, save: true);
    }

    public void ResetToDefaults()
    {
        current = new GameSettingsData();
        PopulateUIFromData(current);
        ApplySettings(current, save: true);
    }

    private void SetupQualityDropdown()
    {
        if (qualityDropdown == null) return;

        qualityDropdown.ClearOptions();
        qualityDropdown.AddOptions(new List<string>(QualitySettings.names));
    }

    private void SetupResolutionDropdown()
    {
        if (resolutionDropdown == null) return;

        filtered.Clear();

        HashSet<string> seen = new();
        foreach (var r in Screen.resolutions)
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
        if (masterSlider != null && masterValueText != null)
            masterSlider.onValueChanged.AddListener(v => masterValueText.text = $"{Mathf.RoundToInt(v * 100)}%");

        if (musicSlider != null && musicValueText != null)
            musicSlider.onValueChanged.AddListener(v => musicValueText.text = $"{Mathf.RoundToInt(v * 100)}%");

        if (sfxSlider != null && sfxValueText != null)
            sfxSlider.onValueChanged.AddListener(v => sfxValueText.text = $"{Mathf.RoundToInt(v * 100)}%");
    }

    private void PopulateUIFromData(GameSettingsData d)
    {
        if (masterSlider != null) masterSlider.value = d.masterVolume;
        if (musicSlider != null) musicSlider.value = d.musicVolume;
        if (sfxSlider != null) sfxSlider.value = d.sfxVolume;

        if (fullscreenToggle != null) fullscreenToggle.isOn = d.fullscreen;

        if (qualityDropdown != null && QualitySettings.names.Length > 0)
            qualityDropdown.value = Mathf.Clamp(d.qualityIndex, 0, QualitySettings.names.Length - 1);

        if (resolutionDropdown != null && filtered.Count > 0)
            resolutionDropdown.value = Mathf.Clamp(d.resolutionIndex, 0, filtered.Count - 1);

        if (masterValueText != null && masterSlider != null)
            masterValueText.text = $"{Mathf.RoundToInt(masterSlider.value * 100)}%";
        if (musicValueText != null && musicSlider != null)
            musicValueText.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
        if (sfxValueText != null && sfxSlider != null)
            sfxValueText.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
    }

    private void ReadUIToData(GameSettingsData d)
    {
        if (masterSlider != null) d.masterVolume = masterSlider.value;
        if (musicSlider != null) d.musicVolume = musicSlider.value;
        if (sfxSlider != null) d.sfxVolume = sfxSlider.value;

        if (fullscreenToggle != null) d.fullscreen = fullscreenToggle.isOn;
        if (qualityDropdown != null) d.qualityIndex = qualityDropdown.value;
        if (resolutionDropdown != null) d.resolutionIndex = resolutionDropdown.value;
    }

    private void ApplySettings(GameSettingsData d, bool save)
    {
        // Audio
        AudioListener.volume = d.masterVolume;

        // Quality
        if (QualitySettings.names.Length > 0)
        {
            int q = Mathf.Clamp(d.qualityIndex, 0, QualitySettings.names.Length - 1);
            QualitySettings.SetQualityLevel(q, true);
            d.qualityIndex = q;
        }

        // Resolution + Fullscreen
        if (filtered.Count > 0)
        {
            int idx = Mathf.Clamp(d.resolutionIndex, 0, filtered.Count - 1);
            var r = filtered[idx];
            Screen.SetResolution(r.width, r.height, d.fullscreen);
            d.resolutionIndex = idx;
        }
        else
        {
            Screen.fullScreen = d.fullscreen;
        }

        if (save)
            SettingsSystem.Save(d);
    }
}