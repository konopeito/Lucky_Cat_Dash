using UnityEngine;

/// <summary>
/// Manages the player's cat skin selection.
/// Skins are stored by ID in SaveData.unlockedSkins.
/// **note to Attach this to the player GameObject alongside a SpriteRenderer.
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class SkinSelector : MonoBehaviour
{
    [System.Serializable]
    public struct CatSkin
    {
        public string id;
        public Sprite sprite;
    }

    public CatSkin[] availableSkins;
    public string defaultSkinId = "default";

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        SaveData data = SaveSystem.Load();
        ApplySkin(defaultSkinId);
    }

    public void ApplySkin(string skinId)
    {
        foreach (var skin in availableSkins)
        {
            if (skin.id == skinId)
            {
                spriteRenderer.sprite = skin.sprite;
                return;
            }
        }
        // Fall back to first skin if ID not found
        if (availableSkins.Length > 0)
            spriteRenderer.sprite = availableSkins[0].sprite;
    }

    public bool IsSkinUnlocked(string skinId)
    {
        SaveData data = SaveSystem.Load();
        string[] ids = data.unlockedSkins.Split(',');
        foreach (var id in ids)
            if (id.Trim() == skinId) return true;
        return false;
    }

    public void UnlockAndApplySkin(string skinId)
    {
        SaveSystem.UnlockSkin(skinId);
        ApplySkin(skinId);
    }
}
