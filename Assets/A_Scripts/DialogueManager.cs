using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI dialogueText;
    public Image soulImage;
    public GameObject dialoguePanel;

    [Header("Settings")]
    public float typeSpeed = 0.05f;

    public SoulData currentSoul;

    private int lineIndex = 0;
    private bool isTyping;
    private Tween textTween;

    private bool isDialogueActive = false;
    // Test amaçlý: Inspector'dan bir ruh sürükle býrak
    public void Start()
    {
        // Hazýr olduðunda bir tuþla veya eventle çaðýrabilirsin
        //LoadSoul(currentSoul);
    }

    public void LoadSoul(SoulData data)
    {
        currentSoul = data;
        lineIndex = 0;

        dialogueText.text = "";

        // 1. Ruhun Giriþ Animasyonu
        soulImage.sprite = data.baseSprite;
        soulImage.color = new Color(1, 1, 1, 0); // Görünmez baþla
        soulImage.transform.localScale = Vector3.one * 0.8f; // Küçük baþla

        // Yerine: soulImage.DOFade(1, 1f);
        DOTween.To(() => soulImage.color, x => soulImage.color = x, new Color(1, 1, 1, 1), 1f).SetTarget(soulImage);
        soulImage.transform.DOScale(1f, 1f).SetEase(Ease.OutBack).OnComplete(() => {
            OpenDialoguePanel();
            isDialogueActive = true;
        });
    }

    void OpenDialoguePanel()
    {
        dialoguePanel.SetActive(true);
        dialoguePanel.transform.localScale = new Vector3(1, 0, 1); // Dikeyde kapalý
        dialoguePanel.transform.DOScaleY(1, 0.5f).SetEase(Ease.OutExpo).OnComplete(() => {
            ShowLine();
        });
    }

    void Update()
    {
        //// Ekrana týklandýðýnda veya Space basýldýðýnda
        //if ((Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)) && isDialogueActive)
        //{
        //    if (isTyping)
        //    {
        //        CompleteText(); // Yazý bitmediyse anýnda bitir
        //    }
        //    else
        //    {
        //        ShowLine(); // Yazý bittiyse sonrakine geç
        //    }
        //}

        if (!isDialogueActive) return;

        // Sadece boþluk tuþu ile de geçmek istersen klavye kontrolü burada kalabilir
        // Fare kontrolünü tamamen kaldýrdýk!
        if (Input.GetKeyDown(KeyCode.Space))
        {
            OnDialogueAreaClicked();
        }
    }

    // --- YENÝ METOD: Görünmez butona basýldýðýnda bu çalýþacak ---
    public void OnDialogueAreaClicked()
    {
        if (!isDialogueActive) return;

        if (isTyping)
        {
            CompleteText(); // Yazý bitmediyse anýnda bitir
        }
        else
        {
            ShowLine(); // Yazý bittiyse sonrakine geç
        }
    }

    void ShowLine()
    {
        if (lineIndex < currentSoul.dialogueLines.Count)
        {
            var line = currentSoul.dialogueLines[lineIndex];

            // Duyguya göre ruhu hareket ettir
            ApplyEmotion(line.emotion, line.expression);

            // Yazý yazma efekti
            dialogueText.text = "";
            isTyping = true;
            // Yerine: dialogueText.DOText(line.text, speed);
            string currentText = "";
            DOTween.To(() => currentText, x => {
                currentText = x;
                dialogueText.text = currentText;
            }, line.text, line.text.Length * typeSpeed).SetEase(Ease.Linear).SetTarget(dialogueText);

            lineIndex++;
        }
        else
        {
            EndDialogue();
        }
    }

    void ApplyEmotion(SoulEmotion emotion, Sprite expression)
    {
        if (expression != null) soulImage.sprite = expression;

        soulImage.transform.DOKill(); // Önceki animasyonu durdur

        soulImage.color = new Color(1, 1, 1, 1); // Rengi sýfýrla

        switch (emotion)
        {
            case SoulEmotion.Angry:
                soulImage.transform.DOShakePosition(0.5f, 15f);
                soulImage.color = Color.red;
                // Yerine: soulImage.DOColor(Color.red, 0.5f);
                DOTween.To(() => soulImage.color, x => soulImage.color = x, Color.red, 0.5f).SetTarget(soulImage);

                break;
            case SoulEmotion.Thinking:
                soulImage.transform.DOScale(1.1f, 0.8f).SetLoops(2, LoopType.Yoyo);
                break;
            case SoulEmotion.Sad:
                soulImage.transform.DOMoveY(soulImage.transform.position.y - 10f, 0.5f).SetLoops(2, LoopType.Yoyo);
                break;
        }
    }

    void CompleteText()
    {
        textTween.Kill();
        dialogueText.text = currentSoul.dialogueLines[lineIndex - 1].text;
        isTyping = false;
    }

    void EndDialogue()
    {
        Debug.Log("Diyalog Bitti. Karar aþamasýna geçiliyor...");
        isDialogueActive = false;

        // Hýzlý baðlantý için GameManager'daki DecisionManager'ý bul ve çaðýr
        FindAnyObjectByType<DecisionManager>().StartJudgementPhase(currentSoul);

        // Karar paneli titreyerek gelirken diyalog panelini kapatabiliriz veya metni silebiliriz
        dialoguePanel.SetActive(false); // Veya metni silip açýk býrak
    }


}