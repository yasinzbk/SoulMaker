using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class EndGameManager : MonoBehaviour
{
    public GameObject endScreenPanel;
    public Transform contentParent; // ScrollView içindeki Content objesi
    public GameObject endingCardPrefab; // Görsel + Metin içeren küçük kart

    public void ShowFinalResults()
    {
        endScreenPanel.SetActive(true);
        var gameFlow = FindAnyObjectByType<GameFlowManager>();

        foreach (var result in gameFlow.allPickedResults)
        {
            // İSTİSNA KONTROLÜ: Eğer bu seçimin bir bonusSoul'u varsa, 
            // bu bir "ara seçim"dir. Bunu listede gösterme.
            if (result.selectedLife.bonusSoul != null) continue;

            // Kartı oluştur ve bilgilerini doldur
            GameObject card = Instantiate(endingCardPrefab, contentParent);
            
            // Kartın içindeki Image ve Text bileşenlerini bul (İsimlerine göre)
            card.transform.Find("ResultImage").GetComponent<Image>().sprite = result.selectedLife.endingSprite;
            card.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = 
                $"<b>{result.soul.soulName}</b>\n{result.selectedLife.endingText}";
            
            // Küçük bir animasyonla kartı göster
            card.transform.localScale = Vector3.zero;
            card.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }
    }
}