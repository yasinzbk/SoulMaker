using UnityEngine;
using UnityEngine.EventSystems; // Fare etkileşimi için gerekli
using DG.Tweening;

public class UIInteractiveAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Idle (Durur) Animasyon Ayarları")]
    public float idleDuration = 1.5f;
    public Ease idleEaseType = Ease.InOutSine;

    [Header("Idle - Floating (Yükselip Alçalma)")]
    public bool useFloating = true;
    public float floatingAmount = 20f;

    [Header("Idle - Scaling (Nefes Alma)")]
    public bool useScaling = true;
    public float idleScaleAmount = 1.1f;

    [Header("Idle - Rotation (Sallanma)")]
    public bool useRotation = false;
    public float rotationAngle = 5f;

    [Header("Hover (Üzerine Gelince) Ayarları")]
    public float hoverScaleTarget = 1.25f; // Üzerine gelince ne kadar büyüsün?
    public float hoverDuration = 0.25f;    // Büyüme hızı
    public Ease hoverEaseType = Ease.OutBack; // Hafif bir geri tepme efekti güzel durur

    private Sequence idleSequence;
    private Tween hoverTween;
    private Vector3 initialScale;
    private RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        initialScale = transform.localScale;
    }

    void Start()
    {
        // Oyun başlayınca Idle animasyonunu başlat
        StartIdleAnimation();
    }

    // --- IDLE ANIMASYONU ---
    public void StartIdleAnimation()
    {
        // Varsa eski animasyonları temizle
        KillAllTweens();

        idleSequence = DOTween.Sequence();
        Vector3 initialPos = rectTransform.anchoredPosition;

        // --- Floating ---
        if (useFloating)
        {
            idleSequence.Join(rectTransform.DOAnchorPosY(initialPos.y + floatingAmount, idleDuration)
                .SetEase(idleEaseType)
                .SetLoops(-1, LoopType.Yoyo));
        }

        // --- Scaling ---
        if (useScaling)
        {
            // Idle scale'i mevcut scale üzerinden değil, ilk scale üzerinden hesapla
            idleSequence.Join(transform.DOScale(initialScale * idleScaleAmount, idleDuration)
                .SetEase(idleEaseType)
                .SetLoops(-1, LoopType.Yoyo));
        }

        // --- Rotation ---
        if (useRotation)
        {
            idleSequence.Join(transform.DORotate(new Vector3(0, 0, rotationAngle), idleDuration)
                .SetEase(idleEaseType)
                .SetLoops(-1, LoopType.Yoyo));
        }
    }

    // --- INTERAKSIYON ARAYÜZLERİ (Interfaces) ---

    // Fare üzerine gelince çalışır
    public void OnPointerEnter(PointerEventData eventData)
    {
        // 1. Idle animasyonunu pürüzsüzce durdur
        idleSequence?.Pause();

        // 2. Varsa eski büyüme tween'ini temizle
        hoverTween?.Kill();

        // 3. Belirlenen scale'e pürüzsüzce büyü
        hoverTween = transform.DOScale(initialScale * hoverScaleTarget, hoverDuration)
            .SetEase(hoverEaseType)
            .SetUpdate(true); // Oyun duraklasa bile çalışsın (opsiyonel)
    }

    // Fare üzerinden çekilince çalışır
    public void OnPointerExit(PointerEventData eventData)
    {
        // 1. Varsa büyüme tween'ini temizle
        hoverTween?.Kill();

        // 2. Eski haline (idle başlangıcına) geri dön ve Idle'ı devam ettir
        hoverTween = transform.DOScale(initialScale * (useScaling ? idleScaleAmount : 1f), hoverDuration)
            .SetEase(Ease.OutSine)
            .OnComplete(() =>
            {
                // Geri dönme tamamlanınca Idle'ı oynat
                idleSequence?.Play();
            });
    }

    private void KillAllTweens()
    {
        idleSequence?.Kill();
        hoverTween?.Kill();
    }

    void OnDestroy()
    {
        KillAllTweens();
    }

    // Obje deaktif olursa animasyonları durdur (bellek yönetimi)
    void OnDisable()
    {
        KillAllTweens();
    }
}