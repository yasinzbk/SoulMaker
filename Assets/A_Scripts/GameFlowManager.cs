using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using TMPro;

public class GameFlowManager : MonoBehaviour
{
    [Header("HUD (Oyun Ýçi Ekran)")]
    public TextMeshProUGUI hudCoinText; // Sol ustte veya sag ustte duracak para metni

    [Header("Geliþ Sýrasý")]
    public List<SoulData> initialSouls; // Oyun basinda hazir olanlar
    private List<SoulData> activeQueue = new List<SoulData>(); // O gun gelecekler
    private Dictionary<int, List<SoulData>> futureSouls = new Dictionary<int, List<SoulData>>();

    [Header("Ekonomi")]
    public int totalPlayerCoins = 0;
    private int dailyEarnings = 0;
    private List<string> dailyReports = new List<string>();

    public int currentDay = 1;
    private int currentSoulIndex = 0;
    private const int SOULS_PER_DAY = 3;

    public List<SoulResult> allPickedResults = new List<SoulResult>();

    void Start()
    {
        UpdateHUD(); // Oyun baslarken parayi yazdir
        activeQueue.AddRange(initialSouls.Take(SOULS_PER_DAY));
        StartDay();
    }

    public void ProcessDecision(SoulData soul, ChoiceLife choice)
    {
        // ... ekonomi hesaplamalari

        int gain = 0;
        string reportEntry = "";

        if (soul.soulCoins >= choice.coinCost)
        {
            gain = 10 + (soul.soulCoins - choice.coinCost);
            // Basarili durumu yesil yazdiralim
            reportEntry = $"<color=green>{soul.soulName}: Baþarýlý Eþleþme (+{gain})</color>";
        }
        else
        {
            gain = -5;
            // Ceza durumunu kirmizi yazdiralim
            reportEntry = $"<color=red>{soul.soulName}: Bütçe Yetersiz (-5)</color>";
        }

        dailyEarnings += gain;
        dailyReports.Add(reportEntry);

        UpdateHUD(); // Her karar sonrasi ekrandaki parayi guncelle


        // Karari listeye ekleyelim
        allPickedResults.Add(new SoulResult
        {
            soul = soul,
            selectedLife = choice,
            day = currentDay
        });

        // Gelecek gun icin ruh ekleme mantigi
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
        UpdateHUD(); // Kasaya net eklendi, HUD yenile

        // EÐER OYUN BÝTTÝYSE (Sýrada ruh kalmadýysa)
        if (CheckIfGameIsOver())
        {
            // Günlük özet yerine direkt FÝNAL ekranýný çaðýrýyoruz
            FindAnyObjectByType<EndGameManager>().ShowFinalResults();
        }
        else
        {
            // Sýrada hala ruhlar var, normal gün özetini göster
            FindAnyObjectByType<DaySummaryUI>().ShowSummary(currentDay, dailyReports, dailyEarnings, totalPlayerCoins);
        }

    }

    private bool CheckIfGameIsOver()
    {
        // 1. Ana listede (initialSouls) sýrasý gelmemiþ ruh kaldý mý?
        int alreadyProcessedMainSouls = currentDay * SOULS_PER_DAY;
        bool isMainListFinished = alreadyProcessedMainSouls >= initialSouls.Count;

        // 2. Gelecek günler için (bonus seçimlerden) bekleyen ruh var mý?
        bool isFutureListEmpty = true;
        foreach (int dayKey in futureSouls.Keys)
        {
            if (dayKey > currentDay) // Eðer bulunduðumuz günden sonraki bir güne ruh eklendiyse
            {
                isFutureListEmpty = false;
                break;
            }
        }

        // Eðer hem ana liste bitmiþse HEM DE gelecekte bekleyen hiçbir bonus ruh yoksa: OYUN BÝTMÝÞTÝR (true döndür)
        return isMainListFinished && isFutureListEmpty;
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