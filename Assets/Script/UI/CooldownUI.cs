using System;
using System.Xml;
using TMPro;
using UnityEngine;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] public TMPro.TextMeshProUGUI text;
    void Update()
    {
        // cập nhật đếm ngược; nhẹ nên để Update là được
        GameData.SyncLivesWithTime();

        var next = GameData.TimeUntilNextLife();
        if (GetLives()>= GameData.MaxLives)
        {
            text.text = $"Lượt: {GetLives()}/{GameData.MaxLives}";
        }
        else
        {
            text.text = $"Lượt: {GetLives()}/{GameData.MaxLives} Hồi sau: {next.Value.Minutes:D2}:{next.Value.Seconds:D2}";
        }
    }

    private int GetLives()
    {
        return GameData.getLives();
    } 
}