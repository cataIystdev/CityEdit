using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityEdit.Models.GameData;

/// <summary>
/// Справочник тегов валют кошелька Subway Surfers City.
/// Маппинг числовых DataTag на человекочитаемые имена валют.
/// </summary>
public static class WalletTags
{
    /// <summary>
    /// DataTag монет.
    /// </summary>
    public const int Coins = -633437229;

    /// <summary>
    /// DataTag ключей.
    /// </summary>
    public const int Keys = -1085419483;

    /// <summary>
    /// DataTag жизней (возрождений).
    /// </summary>
    public const int Revives = 1669706535;

    /// <summary>
    /// DataTag токенов досок.
    /// </summary>
    public const int BoardTokens = -1509662453;

    /// <summary>
    /// DataTag токенов серферов.
    /// </summary>
    public const int SurferTokens = -1878560402;

    /// <summary>
    /// DataTag билетов.
    /// </summary>
    public const int Tickets = 722065917;

    /// <summary>
    /// Словарь: DataTag валюты -> отображаемое имя.
    /// Включает все известные типы валют.
    /// </summary>
    public static readonly ReadOnlyDictionary<int, string> Names = new(new Dictionary<int, string>
    {
        { Coins, "Coins" },
        { Keys, "Keys" },
        { Revives, "Revives" },
        { BoardTokens, "Board Tokens" },
        { SurferTokens, "Surfer Tokens" },
        { Tickets, "Tickets" },
        { 543173782, "Mystery Currency 1" },
        { 449284577, "Mystery Currency 2" },
        { 596132505, "Mystery Currency 3" },
        { -116637823, "Mystery Currency 4" },
        { -802311392, "Mystery Currency 5" }
    });

    /// <summary>
    /// Список основных (известных) тегов валют, которые отображаются на вкладке кошелька.
    /// Порядок определяет отображение в интерфейсе.
    /// </summary>
    public static readonly IReadOnlyList<int> PrimaryTags = new List<int>
    {
        Coins,
        Keys,
        Revives,
        BoardTokens,
        SurferTokens,
        Tickets
    };

    /// <summary>
    /// Возвращает имя валюты по её DataTag.
    /// </summary>
    /// <param name="dataTag">DataTag валюты.</param>
    /// <returns>Имя валюты или строка с DataTag.</returns>
    public static string GetName(int dataTag)
    {
        return Names.TryGetValue(dataTag, out var name) ? name : $"Currency ({dataTag})";
    }
}
