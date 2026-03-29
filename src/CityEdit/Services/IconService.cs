using System;
using System.Collections.Generic;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace CityEdit.Services;

/// <summary>
/// Сервис загрузки иконок серферов и досок из embedded Avalonia ресурсов.
/// Иконки хранятся в Assets/Icons/{surfers,boards}/ как AvaloniaResource.
/// </summary>
public static class IconService
{
    private static readonly Dictionary<string, Bitmap?> _cache = new();

    /// <summary>
    /// Маппинг имён серферов на имена файлов иконок.
    /// </summary>
    private static readonly Dictionary<string, string> SurferFileNames = new()
    {
        { "Jake", "Jake" }, { "Tricky", "Tricky" }, { "Fresh", "Fresh" },
        { "Prince K", "PrinceK" }, { "Miss Maia", "MissMaia" },
        { "Monique", "Monique" }, { "Yutani", "Yutani" }, { "Harini", "Harini" },
        { "Ninja One", "NinjaOne" }, { "Noon", "Noon" }, { "Jenny", "Jenny" },
        { "Wei", "Wei" }, { "Spike", "Spike" }, { "Ella", "Ella" },
        { "Jay", "Jay" }, { "Billy", "Billy" }, { "Rosalita", "Rosalita" },
        { "Tasha", "Tasha" }, { "Jaewoo", "Jaewoo" }, { "Tagbot", "Tagbot" },
        { "Lucy", "Lucy" }, { "Georgie", "Georgie" }, { "V3ctor", "V3ctor" },
        { "Zara", "Zara" }, { "Lilah", "Lilah" }, { "Ash", "Ash" }
    };

    /// <summary>
    /// Маппинг имён досок на имена файлов иконок (по владельцу).
    /// </summary>
    private static readonly Dictionary<string, string> BoardFileNames = new()
    {
        { "Electric Blue", "Classic" }, { "Home Runner", "Ash" },
        { "Trasher", "Jake" }, { "Peace Of Grind", "Jenny" },
        { "Naughty & Nice", "Tricky" }, { "Globetrotter", "Wei" },
        { "Grandmaster", "Fresh" }, { "Djinn's Fortune", "PrinceK" },
        { "Honeycomb", "MissMaia" }, { "Flame Tamer", "Monique" },
        { "Wakizashi", "NinjaOne" }, { "Knockout", "Noon" },
        { "Spaced Invader", "Yutani" }, { "Pawsome", "Harini" },
        { "Dog City", "Spike" }, { "Sub Surf Classic", "Classic" },
        { "Vaunted", "Ella" }, { "Zephyr Cruiser", "Jake" },
        { "Eye Of The Viper", "Billy" }, { "Day Of The Shred", "Rosalita" },
        { "Sweet Street", "Tasha" }, { "G-Tiger", "Jaewoo" },
        { "Cy-Board", "Tagbot" }, { "Cobweb", "Lucy" },
        { "Super Hooper", "Georgie" }, { "H4X0R", "V3ctor" },
        { "Pouncer", "Zara" }, { "Aquiline", "Lilah" }
    };

    /// <summary>
    /// Загружает Bitmap из Avalonia embedded ресурса.
    /// </summary>
    private static Bitmap? TryLoadAsset(string subPath)
    {
        try
        {
            var uri = new Uri($"avares://CityEdit/Assets/Icons/{subPath}");
            using var stream = AssetLoader.Open(uri);
            return new Bitmap(stream);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Получает Bitmap портрета серфера.
    /// </summary>
    public static Bitmap? GetSurferIcon(string surferName)
    {
        var key = $"s:{surferName}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        Bitmap? bmp = null;
        if (SurferFileNames.TryGetValue(surferName, out var fn))
            bmp = TryLoadAsset($"surfers/{fn}.png");

        _cache[key] = bmp;
        return bmp;
    }

    /// <summary>
    /// Получает Bitmap иконки доски.
    /// </summary>
    public static Bitmap? GetBoardIcon(string boardName)
    {
        var key = $"b:{boardName}";
        if (_cache.TryGetValue(key, out var cached)) return cached;

        Bitmap? bmp = null;
        if (BoardFileNames.TryGetValue(boardName, out var fn))
            bmp = TryLoadAsset($"boards/{fn}.png");

        _cache[key] = bmp;
        return bmp;
    }
}
