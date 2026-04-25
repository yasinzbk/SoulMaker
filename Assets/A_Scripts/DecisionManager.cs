using DG.Tweening;
using System; // Hover iþlemleri için gerekli
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class DecisionManager : MonoBehaviour
{
    [Header("Zorunlu Referanslar")]
    [SerializeField] private GameObject decisionUIRoot; // Tüm Karar UI'ýnýn ana paneli
    [SerializeField] private TextMeshProUGUI descriptionTextTarget; // Eski Diyalog Paneli'ndeki TMP Text objesi (Hover metni buraya yazýlacak)
    [SerializeField] private GameObject descriptionPanel;


    [Header("Yargý Sütunu (Sütun)")]
    [SerializeField] private RectTransform columnRect; // Sütunun RectTransform'u
    [SerializeField] private Button columnButton; // Sütunun üzerindeki buton
    [SerializeField] private float columnEnterY = 50f; // Sütunun yukarý çýktýðýnda duracaðý Y konumu

    [Header("Parþömenler (Hayatlar)")]
    [SerializeField] private GameObject scrollsRoot; // Parþömenlerin ana tutucusu
    [SerializeField] private RectTransform scrollARect; // Sol parþömen Rect
    [SerializeField] private RectTransform scrollBRect; // Sað parþömen Rect
    [SerializeField] private TextMeshProUGUI scrollALabel; // Sol parþömen TMP
    [SerializeField] private TextMeshProUGUI scrollBLabel; // Sað parþömen TMP

    [SerializeField] private TextMeshProUGUI scrollACoin; // Sol parþömen TMP
    [SerializeField] private TextMeshProUGUI scrollBCoin; // Sað parþömen TMP

    // Parþömenlerin baþlangýç yerlerini tutmak için (Animasyon sonrasý geri dönmek için)
    private Vector2 scrollAStartPos;
    private Vector2 scrollBStartPos;

    private SoulData currentSoulData;

    // Mevcut referanslarýna ek olarak:
    [SerializeField] private Image soulImage; // Ruhun üzerine gitmesi için ruhun Image referansý
    [SerializeField] private List<SoulResult> allResults = new List<SoulResult>();

    private bool isProcessing = false;

    private Tween idleTweenA;
    private Tween idleTweenB;
    [SerializeField] private float idleAmount = 15f; // Sallanma mesafesi
    [SerializeField] private float idleDuration = 2f; // Sallanma hýzý

    private void Awake()
    {
        // Baþlangýç pozisyonlarýný kaydet
        scrollAStartPos = scrollARect.anchoredPosition;
        scrollBStartPos = scrollBRect.anchoredPosition;
    }

    private void Start()
    {
        ResetJudgementUI();

        // Oyun baþýnda kapalý olduklarýndan emin olalým
        decisionUIRoot.SetActive(false);
        //scrollsRoot.SetActive(false);
        //descriptionPanel.SetActive(false);
        //columnRect.anchoredPosition = new Vector2(0, -1000f); // Ekranýn altýnda
    }

    private void ResetJudgementUI()
    {
        idleTweenA?.Kill();
        idleTweenB?.Kill();

        // Sütunu ekranýn altýna gönder
        columnRect.anchoredPosition = new Vector2(0, -1000f);
        scrollsRoot.SetActive(false);


        // Parþömenlerin yerini, ölçeðini ve rengini sýfýrla
        scrollARect.anchoredPosition = scrollAStartPos;
        scrollBRect.anchoredPosition = scrollBStartPos;
        scrollARect.localScale = Vector3.one;
        scrollBRect.localScale = Vector3.one;
        scrollARect.gameObject.SetActive(true);
        scrollBRect.gameObject.SetActive(true);

        // Alpha deðerlerini (renkleri) 1 yap (DOTween.To ile þeffaflaþtýrdýðýmýz için)
        Image imgA = scrollARect.GetComponent<Image>();
        Image imgB = scrollBRect.GetComponent<Image>();
        if (imgA != null) imgA.color = Color.white;
        if (imgB != null) imgB.color = Color.white;

        descriptionPanel.SetActive(false);
    }

    // Diyalog bittiðinde bu metod çaðýrýlacak
    public void StartJudgementPhase(SoulData data)
    {
        currentSoulData = data;
        isProcessing = false;

        //UI'ý her yeni ruhta resetliyoruz
        ResetJudgementUI();

        // Önce temizleyelim (Diyalog paneli TMP'sini siliyoruz)
        descriptionTextTarget.text = "";

        decisionUIRoot.SetActive(true);
        columnButton.gameObject.SetActive(true); // Butonu açalým
        scrollsRoot.SetActive(false); // Parþömenler henüz gelmesin

        // Animasyon 1: Sütun aþaðýdan titreyerek yukarý çýkar
        // (OutBack veya OutElastic bounce hissi için iyidir)
        columnRect.DOAnchorPos(new Vector2(0, columnEnterY), 1.5f).SetEase(Ease.OutBounce).OnComplete(() =>
        {
            // Animasyon tamamlandýðýnda sütun bir kez daha "shake" yapsýn (vurgu için)
            columnRect.DOShakePosition(0.3f, 8f, 7, 90f, false, true);
        });


        //// anim 2
        //columnRect.DOAnchorPos(new Vector2(0, columnEnterY), 1f).SetEase(Ease.OutBounce).OnComplete(() => {
        //    columnRect.DOShakePosition(0.3f, 10f);
        //});
    }

    // Sütun butonuna týklanýnca çaðýrýlýr (Inspector'dan Button -> OnClick'e baðla)
    public void OnJudgementButtonClick()
    {
        columnButton.gameObject.SetActive(false); // Butonu kapat

        // Parþömen yazýlarýný SO'dan yükle
        scrollALabel.text = currentSoulData.optionA.lifeLabel;
        scrollBLabel.text = currentSoulData.optionB.lifeLabel;

        scrollACoin.text = currentSoulData.optionA.coinCost.ToString();
        scrollBCoin.text = currentSoulData.optionB.coinCost.ToString();

        // Parþömenleri aç ve bir animasyonla göster (Scale Up)
        scrollsRoot.SetActive(true);
        scrollsRoot.transform.localScale = Vector3.zero;
        //scrollsRoot.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);

        // Parþömenler açýlýnca Idle animasyonunu baþlat
        scrollsRoot.transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).OnComplete(() => {
            StartIdleAnimations();
        });
    }

    private void StartIdleAnimations()
    {
        // Sol parþömen için sonsuz döngü sallanma
        idleTweenA = scrollARect.DOAnchorPosY(scrollARect.anchoredPosition.y + idleAmount, idleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Sað parþömen için (biraz gecikmeli ki senkronize olmasýnlar, daha doðal durur)
        idleTweenB = scrollBRect.DOAnchorPosY(scrollBRect.anchoredPosition.y + idleAmount, idleDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo)
            .SetDelay(0.2f);
    }

    public void OnScrollSelected(int index) // 0: Option A, 1: Option B
    {
        if (isProcessing) return;
        isProcessing = true;

        RectTransform selectedScroll = (index == 0) ? scrollARect : scrollBRect;
        RectTransform otherScroll = (index == 0) ? scrollBRect : scrollARect;
        otherScroll.gameObject.SetActive(false);
        //GameObject selectedScroll = (index == 0) ? scrollALabel.transform.parent.gameObject : scrollBLabel.transform.parent.gameObject;
        ChoiceLife selectedLife = (index == 0) ? currentSoulData.optionA : currentSoulData.optionB;

        //Kararý GameFlowManager'a bildiriyoruz
        FindAnyObjectByType<GameFlowManager>().ProcessDecision(currentSoulData, selectedLife);

        // Seçimi kaydet
        allResults.Add(new SoulResult
        {
            soul = currentSoulData,
            selectedLife = selectedLife,
            day = FindAnyObjectByType<GameFlowManager>().currentDay
        });





        // Parþömen Verme Sequence
        Sequence giveSeq = DOTween.Sequence();

        // 1. Önce parþömen biraz büyür (Vurgu)
        giveSeq.Append(selectedScroll.DOScale(1.2f, 0.3f).SetEase(Ease.OutBack));

        // 2. Ruha doðru uçar ve þeffaflaþýr
        giveSeq.Append(selectedScroll.DOMove(soulImage.transform.position, 0.6f).SetEase(Ease.InBack));

        // Manuel Fade (Kendi kullandýðýn DOTween.To yöntemiyle)
        Image sImg = selectedScroll.GetComponent<Image>();
        giveSeq.Join(DOTween.To(() => sImg.color, x => sImg.color = x, new Color(1, 1, 1, 0), 0.5f));

        // 3. Ruh da ayný anda þeffaflaþýp gider
        giveSeq.Join(DOTween.To(() => soulImage.color, x => soulImage.color = x, new Color(1, 1, 1, 0), 0.5f));

        giveSeq.OnComplete(() => {
            decisionUIRoot.SetActive(false);
            // BURADA: Yeni ruhu çaðýrma kodunu tetikle
            Debug.Log("Yeni ruh çaðrýlýyor...");
            FindAnyObjectByType<GameFlowManager>().NextStep();
        });
    }

    // --- HOVER LOGIC (Fare Üzerine Gelince) ---
    // Bu metodlarý Inspector'daki EventTrigger bileþeniyle baðlayacaðýz

    public void OnPointerEnterScrollA()
    {
        idleTweenA.Pause(); // Sallanmayý durdur
        scrollARect.DOScale(1.15f, 0.3f).SetEase(Ease.OutBack); // Büyüt

        // Sol parþömen açýklamasý üst TMP'ye anýnda yazýlýr (veya Typewriter ile)
        //descriptionTextTarget.text = currentSoulData.optionA.lifeDescription;
        OpenDialoguePanel(currentSoulData.optionA.lifeDescription); // Parþömen A'ya gelince panel açýlýp açýklama gösterilsin
    }

    public void OnPointerEnterScrollB()
    {
        idleTweenB.Pause(); // Sallanmayý durdur
        scrollBRect.DOScale(1.15f, 0.3f).SetEase(Ease.OutBack); // Büyüt

        //descriptionTextTarget.text = currentSoulData.optionB.lifeDescription;
        OpenDialoguePanel(currentSoulData.optionB.lifeDescription); // Parþömen B'ye gelince panel açýlýp açýklama gösterilsin
    }

    public void OnPointerExitScroll()
    {
        descriptionTextTarget.text = ""; // Fare gidince metni sil
            descriptionPanel.SetActive(false); // Paneli kapat

        // Eski boyuta dön ve sallanmaya devam et
        scrollARect.DOScale(1f, 0.3f).SetEase(Ease.OutSine);
        scrollBRect.DOScale(1f, 0.3f).SetEase(Ease.OutSine);

        idleTweenA.Play();
        idleTweenB.Play();
    }

    void OpenDialoguePanel(string dialogue)
    {
        descriptionPanel.SetActive(true);
        descriptionPanel.transform.localScale = new Vector3(1, 0, 1); // Dikeyde kapalý
        descriptionPanel.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutExpo).OnComplete(() => {
            ShowLine(dialogue);
        });
    }

    private void ShowLine(string dialogue)
    {
        descriptionTextTarget.text = dialogue;
    }
}
