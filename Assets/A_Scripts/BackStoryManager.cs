using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BackstoryManager : MonoBehaviour
{
    [Header("Paneller")]
    public RectTransform mainScreen; // Ruhun olduðu ana ekran
    public RectTransform backstoryPanel; // Bilgi ekraný

    [Header("Bilgi Referanslarý")]
    public Image soulPortrait;
    public TextMeshProUGUI backstoryText;
    public TextMeshProUGUI coinText;

    private float screenWidth = 1920f; // Canvas scaler referansýnla ayný olmalý

    void Start()
    {
        // Baþlangýçta bilgi paneli solda gizli olmalý
        backstoryPanel.anchoredPosition = new Vector2(-screenWidth, 0);
        backstoryPanel.gameObject.SetActive(false);
    }

    // Yeþil oka basýnca çalýþacak
    public void OpenBackstory()
    {
        backstoryPanel.gameObject.SetActive(true);
        // Þu anki ruhun bilgilerini yükle
        SoulData currentSoul = FindAnyObjectByType<DialogueManager>().currentSoul;
        UpdateBackstoryUI(currentSoul);

        // Saða Kayma Animasyonu
        mainScreen.DOAnchorPos(new Vector2(screenWidth, 0), 0.6f).SetEase(Ease.OutCubic);
        backstoryPanel.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.OutCubic);
    }

    // Bilgi ekranýndaki geri okuna basýnca çalýþacak
    public void CloseBackstory()
    {
        // Sola (Eski yerine) Kayma Animasyonu
        mainScreen.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.InCubic);
        backstoryPanel.DOAnchorPos(new Vector2(-screenWidth, 0), 0.6f).SetEase(Ease.InCubic).OnComplete(() =>
        {
            backstoryPanel.gameObject.SetActive(false);
        });
    }

    void UpdateBackstoryUI(SoulData data)
    {
        soulPortrait.sprite = data.baseSprite;
        backstoryText.text = data.fullBackstory;
        coinText.text =/* "Ýyilik Parasý: " +*/ data.soulCoins.ToString();
    }
}