using UnityEngine;
using System;

public static class GameData
{
    private const string LivesKey = "Lives";
    private const string NextLifeReadyAtKey = "NextLifeReadyAtUtc"; // lưu UTC ISO

    public static int MaxLives { get; set; } = 1;
    public static int RefillMinutes { get; set; } = 5;

    public static int Lives
    {
        get => PlayerPrefs.GetInt(LivesKey, MaxLives);
        set => PlayerPrefs.SetInt(LivesKey, Mathf.Clamp(value, 0, MaxLives));
    }

    private static DateTime NextLifeReadyAtUtc
    {
        get
        {
            if (!PlayerPrefs.HasKey(NextLifeReadyAtKey)) return DateTime.MinValue;
            var s = PlayerPrefs.GetString(NextLifeReadyAtKey);
            if (DateTime.TryParse(s, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out var t))
                return DateTime.SpecifyKind(t, DateTimeKind.Utc);
            return DateTime.MinValue;
        }
        set => PlayerPrefs.SetString(NextLifeReadyAtKey, value.ToUniversalTime().ToString("o"));
    }

    private static DateTime UtcNow => DateTime.UtcNow;

    /// Gọi hàm này mỗi lần vào game/scene, trước khi hiển thị UI.
    public static void SyncLivesWithTime()
    {
        // Nếu đang full thì không có timer
        if (Lives >= MaxLives)
        {
            NextLifeReadyAtUtc = DateTime.MinValue;
            return;
        }

        if (NextLifeReadyAtUtc == DateTime.MinValue)
        {
            // Trường hợp hiếm: bị thiếu mốc, đặt mốc từ bây giờ
            NextLifeReadyAtUtc = UtcNow.AddMinutes(RefillMinutes);
        }

        // Hồi nhiều lượt nếu đã qua nhiều chu kỳ
        while (Lives < MaxLives && UtcNow >= NextLifeReadyAtUtc)
        {
            Lives++;
            NextLifeReadyAtUtc = NextLifeReadyAtUtc.AddMinutes(RefillMinutes);
        }

        // Nếu đã full sau khi hồi → xóa mốc
        if (Lives >= MaxLives)
            NextLifeReadyAtUtc = DateTime.MinValue;
    }

    public static bool TryConsumeLife()
    {
        SyncLivesWithTime();

        if (Lives > 0)
        {
             return true;
        }

        return false;
    }

    public static TimeSpan? TimeUntilNextLife()
    {
        SyncLivesWithTime();
        if (Lives >= MaxLives) return null;
        var remain = NextLifeReadyAtUtc - UtcNow;
        return remain < TimeSpan.Zero ? TimeSpan.Zero : remain;
    }

    public static int LivesToFull()
    {
        SyncLivesWithTime();
        return MaxLives - Lives;
    }
    public static int getLives()
    {
        return Lives;
    }
}
