//using UnityEngine;
//using System.Collections.Generic;

//public class GameFlowManager : MonoBehaviour
//{
//    public List<SoulData> allSouls; // Oyundaki tüm ruhlarýn listesi
//    private int currentSoulIndex = 0;

//    public int currentDay = 1;
//    public int soulsPerDay = 3; // Günde kaç ruh gelecek?

//    void Start()
//    {
//        SpawnNextSoul();
//    }

//    public void NextStep()
//    {
//        currentSoulIndex++;

//        if (currentSoulIndex < allSouls.Count)
//        {
//            // Gün kontrolü
//            if (currentSoulIndex % soulsPerDay == 0)
//            {
//                currentDay++;
//                Debug.Log("Yeni Gün Baþladý: " + currentDay);
//                // Burada "Gün X" yazan bir UI paneli gösterebilirsin
//            }

//            SpawnNextSoul();
//        }
//        else
//        {
//            Debug.Log("Oyun Bitti! Sonuçlar listeleniyor...");
//            // Final panelini açma kodu buraya gelecek
//        }
//    }

//    void SpawnNextSoul()
//    {
//        FindAnyObjectByType<DialogueManager>().LoadSoul(allSouls[currentSoulIndex]);
//    }
//}

using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameFlowManager : MonoBehaviour
{
    [Header("HUD (Oyun Ýçi Ekran)")]
    public TextMeshProUGUI hudCoinText; // Sol üstte veya sað üstte duracak para metni

    [Header("Geliþ Sýrasý")]
    public List<SoulData> initialSouls; // Oyun baþýnda hazýr olanlar
    private List<SoulData> activeQueue = new List<SoulData>(); // O gün gelecekler
    private Dictionary<int, List<SoulData>> futureSouls = new Dictionary<int, List<SoulData>>();

    [Header("Ekonomi")]
    public int totalPlayerCoins = 0;
    private int dailyEarnings = 0;
    private List<string> dailyReports = new List<string>();

    public int currentDay = 1;
    private int currentSoulIndex = 0;
    private const int SOULS_PER_DAY = 3;

    void Start()
    {
        UpdateHUD(); // Oyun baþlarken parayý yazdýr
        activeQueue.AddRange(initialSouls.Take(SOULS_PER_DAY));
        StartDay();
    }

    public void ProcessDecision(SoulData soul, ChoiceLife choice)
    {
        int gain = 0;
        string reportEntry = "";

        if (soul.soulCoins >= choice.coinCost)
        {
            gain = 10 + (soul.soulCoins - choice.coinCost);
            // Baþarýlý durumu yeþil yazdýralým
            reportEntry = $"<color=green>{soul.soulName}: Baþarýlý Eþleþme (+{gain})</color>";
        }
        else
        {
            gain = -5;
            // Ceza durumunu kýrmýzý yazdýralým
            reportEntry = $"<color=red>{soul.soulName}: Bütçe Yetersiz (-5)</color>";
        }

        dailyEarnings += gain;
        dailyReports.Add(reportEntry);

        UpdateHUD(); // Her karar sonrasý ekrandaki parayý güncelle

        // Gelecek gün için ruh ekleme mantýðý
        if (choice.bonusSoul != null)
        {
            int targetDay = currentDay + choice.appearanceDayOffset;
            if (!futureSouls.ContainsKey(targetDay)) futureSouls[targetDay] = new List<SoulData>();
            futureSouls[targetDay].Add(choice.bonusSoul);
        }
    }

    public void NextStep()
    {
        currentSoulIndex++;
        if (currentSoulIndex < activeQueue.Count)
        {
            SpawnNextSoul();
        }
        else
        {
            EndDay();
        }
    }

    // Ekrandaki para yazýsýný güncelleyen metod
    void UpdateHUD()
    {
        if (hudCoinText != null)
        {
            // Gün içi kazançlarý anlýk olarak kasada gösteriyoruz
            hudCoinText.text = /*"Para: " + */(totalPlayerCoins + dailyEarnings).ToString();
        }
    }

    void EndDay()
    {
        totalPlayerCoins += dailyEarnings;
        // UI Manager'a gün sonu verilerini gönder
        FindAnyObjectByType<DaySummaryUI>().ShowSummary(currentDay, dailyReports, dailyEarnings, totalPlayerCoins);
    }

    public void StartNextDay()
    {
        currentDay++;
        currentSoulIndex = 0;
        dailyEarnings = 0;
        dailyReports.Clear();

        // Yeni kuyruðu oluþtur
        activeQueue.Clear();

        // 1. O güne özel tetiklenen ruhlarý ekle
        if (futureSouls.ContainsKey(currentDay))
        {
            activeQueue.AddRange(futureSouls[currentDay]);
        }

        // 2. Ana listeden sýradaki ruhlarý ekle (Kuyruk 3 olana kadar)
        int remainingFromMain = initialSouls.Count - (currentDay - 1) * SOULS_PER_DAY;
        if (remainingFromMain > 0)
        {
            activeQueue.AddRange(initialSouls.Skip((currentDay - 1) * SOULS_PER_DAY).Take(SOULS_PER_DAY));
        }

        StartDay();
    }

    void SpawnNextSoul()
    {
        FindAnyObjectByType<DialogueManager>().LoadSoul(activeQueue[currentSoulIndex]);
    }

    void StartDay()
    {
        // Gün baþlangýcý animasyonlarý vs.
        SpawnNextSoul();
    }
}