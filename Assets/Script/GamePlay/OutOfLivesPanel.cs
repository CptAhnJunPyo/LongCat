using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;// bạn đã dùng DOTween
using System;

public class OutOfLivesPanel : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private CanvasGroup cg;
    [SerializeField] private Button watchAdButton;
    [SerializeField] private Button buyButton;
    [SerializeField] private Button closeButton;
    [SerializeField] private TextMeshProUGUI messageText;

    [Header("Config")]
    [SerializeField] private int buyPackAmount = 5;    // mua thêm +5 lượt
    [SerializeField] private int adRewardAmount = 1;   // xem quảng cáo +1 lượt

    // Bạn sẽ gắn service thực tế sau (Unity Ads/IAP). Tạm làm callback giả lập.
    public Func<bool> ShowRewardedAd;  // return true nếu xem xong
    public Func<bool> PurchaseLives;   // return true nếu mua thành công

    private void Awake()
    {
        if (!cg) cg = GetComponent<CanvasGroup>();
        HideInstant();
        watchAdButton.onClick.AddListener(OnClickWatchAd);
        buyButton.onClick.AddListener(OnClickBuy);
        closeButton.onClick.AddListener(Close);
    }

    public void Show(TimeSpan? remain = null)
    {
        messageText.text = remain.HasValue
            ? $"Bạn đã hết lượt!\nHồi sau: {remain.Value.Minutes:D2}:{remain.Value.Seconds:D2}"
            : "Bạn đã hết lượt!";

        cg.DOFade(1f, 0.2f);
        cg.interactable = true;
        cg.blocksRaycasts = true;
    }

    public void Close()
    {
        cg.DOFade(0f, 0.2f).OnComplete(() => {
            cg.interactable = false;
            cg.blocksRaycasts = false;
        // Ẩn UI sau khi fade out
            HideInstant();
        });
    }

    private void HideInstant()
    {
        cg.alpha = 0f;
        cg.interactable = false;
        cg.blocksRaycasts = false;
    }

    private void OnClickWatchAd()
    {
        if (ShowRewardedAd == null || ShowRewardedAd.Invoke())
        {
            GameData.SyncLivesWithTime();
            // +1 turn, k access vao NextLifeReadyAtUtc
            typeof(GameData).GetProperty("Lives", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .SetValue(null, GameData.Lives + 1);
            Close();
        }
    }

    private void OnClickBuy()
    {
        if (PurchaseLives == null || PurchaseLives.Invoke())
        {
            GameData.SyncLivesWithTime();
            typeof(GameData).GetProperty("Lives", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                .SetValue(null, GameData.Lives + 5);
            Close();
        }
    }
}
