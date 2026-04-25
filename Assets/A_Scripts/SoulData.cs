using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ChoiceLife
{
    public string lifeLabel; // Parþömenin üstünde ne yazýyor? (örn: "Zengin Tüccar")
    [TextArea(3, 10)] public string lifeDescription; // Fare ile üzerine gelince üst panelde ne yazacak?
    public int coinCost; // Bu hayatýn maliyeti

    [Header("Gelecek Etkisi")]
    public SoulData bonusSoul; // Bu seçilirse gelecek olan yeni ruh
    public int appearanceDayOffset; // Kaç gün sonra gelecek? (1 ise yarýn, 2 ise öbür gün)
}

[System.Serializable]
public class DialogueLine
{
    [TextArea(3, 5)] public string text; // Ne söylüyor?
    public SoulEmotion emotion;          // Hangi duyguyla?
    public Sprite expression;      // O anki yüz ifadesi (Opsiyonel)
}

[System.Serializable]
public class SoulResult
{
    public SoulData soul;
    public ChoiceLife selectedLife;
    public int day; // Hangi gün karar verildi?
}

[CreateAssetMenu(fileName = "NewSoulData", menuName = "Game/Soul Data")]
public class SoulData : ScriptableObject
{
    public string soulName;
    public Sprite baseSprite;            // Ruhun varsayýlan hali
    [TextArea(10, 20)] public string fullBackstory; // Detaylý geçmiþ hikaye
    public int soulCoins; // Ruhun sahip olduðu iyilik parasý
    public List<DialogueLine> dialogueLines;

    [Header("---- Diyalog Sonrasý Karar ----")]
    public ChoiceLife optionA; // Sol parþömen
    public ChoiceLife optionB; // Sað parþömen
}