using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Bu satır, scripti attığın objede otomatik olarak Button olmasını zorunlu kılar (Hata yapmanı engeller)
[RequireComponent(typeof(Button))]
public class ButtonSoundHelper : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
    }

    // Fare butonun ÜZERİNE GELDİĞİNDE otomatik çalışır
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Buton tıklanabilir (interactable) durumdaysa hover sesi çal
        if (myButton != null && myButton.interactable)
        {
            AudioManager.Instance.PlayHoverSound();
        }
    }

    // Fare butona TIKLADIĞINDA otomatik çalışır
    public void OnPointerClick(PointerEventData eventData)
    {
        // Buton tıklanabilir durumdaysa click sesi çal
        if (myButton != null && myButton.interactable)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }
}