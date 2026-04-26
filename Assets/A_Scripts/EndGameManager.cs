using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using DG.Tweening;

public class EndGameManager : MonoBehaviour
{
    public GameObject endScreenPanel;
    public Transform contentParent; // ScrollView icindeki Content objesi
    public GameObject endingCardPrefab; // Gorsel Metin iceren kucuk kart

    public void ShowFinalResults()
    {
        endScreenPanel.SetActive(true);
        var gameFlow = FindAnyObjectByType<GameFlowManager>();

        foreach (var result in gameFlow.allPickedResults)
        {
            // Eger bu secimin bir bonusSoul'u varsa, 
            // bu bir "ara secim"dir. Bunu listede gosterme.
            if (result.selectedLife.bonusSoul != null) continue;

            // Kartı olustur ve bilgilerini doldur
            GameObject card = Instantiate(endingCardPrefab, contentParent);
            
            // Kartın icindeki Image ve Text bilesenlerini bul (isimlerine göre)
            card.transform.Find("ResultImage").GetComponent<Image>().sprite = result.selectedLife.endingSprite;
            card.transform.Find("ResultText").GetComponent<TextMeshProUGUI>().text = 
                $"<b>{result.soul.soulName}</b>\n{result.selectedLife.endingText}";
            
            // Kucuk bir animasyonla kartı göster
            card.transform.localScale = Vector3.zero;
            card.transform.DOScale(1, 0.5f).SetEase(Ease.OutBack);
        }
    }
}