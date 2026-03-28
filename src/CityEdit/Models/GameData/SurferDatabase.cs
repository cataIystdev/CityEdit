using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace CityEdit.Models.GameData;

/// <summary>
/// Справочник серферов (персонажей) Subway Surfers City.
/// Содержит маппинг числовых идентификаторов DataTag на имена персонажей.
/// Данные извлечены из игровых файлов.
/// </summary>
public static class SurferDatabase
{
    /// <summary>
    /// Словарь: DataTag серфера -> имя серфера.
    /// Все 26 игровых персонажей.
    /// </summary>
    public static readonly ReadOnlyDictionary<int, string> Names = new(new Dictionary<int, string>
    {
        { -1836944478, "Jake" },
        { 1900660162, "Tricky" },
        { 2129411796, "Fresh" },
        { 1936280213, "Prince K" },
        { 1614866432, "Miss Maia" },
        { 135046766, "Monique" },
        { 1663244716, "Yutani" },
        { 823378763, "Harini" },
        { 1804257387, "Ninja One" },
        { -502265868, "Noon" },
        { 1363767693, "Jenny" },
        { -518167090, "Wei" },
        { 849273384, "Spike" },
        { 1200047034, "Ella" },
        { 1120354844, "Jay" },
        { 581326566, "Billy" },
        { -1505268145, "Rosalita" },
        { 299562833, "Tasha" },
        { -1733051898, "Jaewoo" },
        { 1887684367, "Tagbot" },
        { 852717139, "Lucy" },
        { -2125407733, "Georgie" },
        { -1534276928, "V3ctor" },
        { -2075784936, "Zara" },
        { -2116248615, "Lilah" },
        { 966716028, "Ash" }
    });

    /// <summary>
    /// Возвращает имя серфера по его DataTag.
    /// Если серфер не найден, возвращает строковое представление ID.
    /// </summary>
    /// <param name="dataTag">Числовой идентификатор серфера.</param>
    /// <returns>Имя серфера или строка с ID.</returns>
    public static string GetName(int dataTag)
    {
        return Names.TryGetValue(dataTag, out var name) ? name : dataTag.ToString();
    }
}
