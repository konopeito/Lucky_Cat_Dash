using UnityEngine;

public class HowToPlayPanelController : MonoBehaviour
{
    [SerializeField] private GameObject mainButtonsPanel;
    [SerializeField] private GameObject howToPlayPanel;

    public void Open()
    {
        if (mainButtonsPanel) mainButtonsPanel.SetActive(false);
        if (howToPlayPanel) howToPlayPanel.SetActive(true);
    }

    public void Close()
    {
        if (howToPlayPanel) howToPlayPanel.SetActive(false);
        if (mainButtonsPanel) mainButtonsPanel.SetActive(true);
    }
}