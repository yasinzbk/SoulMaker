using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class BackstoryManager : MonoBehaviour
{
    [Header("Paneller")]
    public RectTransform mainScreen; // Ruhun oldugu ana ekran
    public RectTransform backstoryPanel; // Bilgi ekrani

    [Header("Bilgi Referanslarý")]
    public Image soulPortrait;
    public TextMeshProUGUI backstoryText;
    public TextMeshProUGUI coinText;

    public TextMeshProUGUI nameText; // Ruhun ismi icin ek bir text (istege bagli)

    private float screenWidth = 1920f; // Canvas scaler referansinla ayni olmali

    void Start()
    {
        // Baslangicta bilgi paneli solda gizli olmali
        backstoryPanel.anchoredPosition = new Vector2(-screenWidth, 0);
        backstoryPanel.gameObject.SetActive(false);
    }

    // Yesil oka basinca calisacak
    public void OpenBackstory()
    {
        backstoryPanel.gameObject.SetActive(true);
        // Su anki ruhun bilgilerini yukle
        SoulData currentSoul = FindAnyObjectByType<DialogueManager>().currentSoul;
        UpdateBackstoryUI(currentSoul);

        // Saga Kayma Animasyonu
        mainScreen.DOAnchorPos(new Vector2(screenWidth, 0), 0.6f).SetEase(Ease.OutCubic);
        backstoryPanel.DOAnchorPos(new Vector2(0, 0), 0.6f).SetEase(Ease.OutCubic);
    }

    // Bilgi ekranindaki geri okuna basinca calisacak   
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
        nameText.text = data.soulName; // Ruhun ismini goster   
        soulPortrait.sprite = data.baseSprite;
        backstoryText.text = data.fullBackstory;
        coinText.text =/* "Iyilik Parasi: " +*/ data.soulCoins.ToString();
    }
}