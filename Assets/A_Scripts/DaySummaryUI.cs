using UnityEngine;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class DaySummaryUI : MonoBehaviour {
    public GameObject summaryPanel;
    public TextMeshProUGUI dayTitleText;
    public TextMeshProUGUI reportListText;
    public TextMeshProUGUI dailyTotalText;
    public TextMeshProUGUI netBalanceText;

    public void ShowSummary(int day, List<string> reports, int dailyEarn, int total) {
        summaryPanel.SetActive(true);
        dayTitleText.text = $"GÜN {day} ÖZETİ";
        
        reportListText.text = string.Join("\n", reports);
        dailyTotalText.text = $"Günlük Maaş: {dailyEarn} Para";
        netBalanceText.text = $"Toplam Kasa: {total} Para";

        // Animasyonlu giriş
        summaryPanel.transform.localScale = Vector3.zero;
        summaryPanel.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
    }

    public void OnNextDayButtonClicked() {
        summaryPanel.transform.DOScale(0, 0.3f).OnComplete(() => {
            summaryPanel.SetActive(false);
            FindAnyObjectByType<GameFlowManager>().StartNextDay();
        });
    }
}