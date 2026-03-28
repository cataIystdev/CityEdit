using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace CityEdit.Models.GameData;

/// <summary>
/// Информация о скине серфера.
/// </summary>
/// <param name="OwnerId">DataTag серфера-владельца скина.</param>
/// <param name="DisplayName">Отображаемое имя скина.</param>
/// <param name="ImageKey">Ключ изображения скина.</param>
public record SkinInfo(int OwnerId, string DisplayName, string ImageKey);

/// <summary>
/// Справочник скинов серферов Subway Surfers City.
/// Содержит маппинг DataTag скина на информацию о скине:
/// владелец, имя, ключ изображения.
/// </summary>
public static class SkinDatabase
{
    /// <summary>
    /// Словарь: DataTag скина -> информация о скине.
    /// Все 80+ скинов игры.
    /// </summary>
    public static readonly ReadOnlyDictionary<int, SkinInfo> Skins = new(new Dictionary<int, SkinInfo>
    {
        // Jake
        { -56800546, new SkinInfo(-1836944478, "Jake Default", "jake_default") },
        { -960954917, new SkinInfo(-1836944478, "Jake Red/Black", "jake_red_black") },
        { 213457242, new SkinInfo(-1836944478, "Jake Google", "jake_google") },
        { -848432295, new SkinInfo(-1836944478, "Jake Apple", "jake_apple") },
        { -1186298654, new SkinInfo(-1836944478, "Jake Yellow", "jake_yellow") },
        { -1882385935, new SkinInfo(-1836944478, "Jake Incognito", "jake_incognito") },
        { -1087452012, new SkinInfo(-1836944478, "Jake Legendary", "jake_legendary") },

        // Tricky
        { -72574065, new SkinInfo(1900660162, "Tricky Default", "tricky_default") },
        { 1564209559, new SkinInfo(1900660162, "Tricky Red/Black", "tricky_red_black") },
        { -820187263, new SkinInfo(1900660162, "Tricky Neon", "tricky_neon") },
        { -1583432582, new SkinInfo(1900660162, "Tricky Legendary", "tricky_legendary") },

        // Fresh
        { 1202249944, new SkinInfo(2129411796, "Fresh Default", "fresh_default") },
        { -1292355720, new SkinInfo(2129411796, "Fresh Red/Black", "fresh_red_black") },
        { 772685907, new SkinInfo(2129411796, "Fresh Purple", "fresh_purple") },
        { 1310049346, new SkinInfo(2129411796, "Fresh Legendary", "fresh_legendary") },

        // Miss Maia
        { -1083767387, new SkinInfo(1614866432, "Miss Maia Default", "miss_maia_default") },
        { 1154305282, new SkinInfo(1614866432, "Miss Maia Red/Black", "miss_maia_red_black") },
        { 775756685, new SkinInfo(1614866432, "Miss Maia Orange/Blue", "miss_maia_orange_blue") },
        { 577316046, new SkinInfo(1614866432, "Miss Maia Legendary", "miss_maia_legendary") },

        // Ninja One
        { 1189151463, new SkinInfo(1804257387, "Ninja One Default", "ninja_one_default") },
        { -672918755, new SkinInfo(1804257387, "Ninja One Gray", "ninja_one_gray") },
        { -1253901889, new SkinInfo(1804257387, "Ninja One Legendary", "ninja_one_legendary") },

        // Yutani
        { 460586275, new SkinInfo(1663244716, "Yutani Default", "yutani_default") },
        { -258476380, new SkinInfo(1663244716, "Yutani Red/Black", "yutani_red_black") },
        { 1478947346, new SkinInfo(1663244716, "Yutani Legendary", "yutani_legendary") },

        // Spike
        { 922134811, new SkinInfo(849273384, "Spike Default", "spike_default") },
        { -759788578, new SkinInfo(849273384, "Spike Red/Ash", "spike_red_ash") },
        { -1655542994, new SkinInfo(849273384, "Spike Legendary", "spike_legendary") },

        // Ella
        { -989991611, new SkinInfo(1200047034, "Ella Default", "ella_default") },
        { 1508371367, new SkinInfo(1200047034, "Ella Purple", "ella_purple") },
        { -93943772, new SkinInfo(1200047034, "Ella Yellow/Black", "ella_yellow_black") },
        { 1990369193, new SkinInfo(1200047034, "Ella Legendary", "ella_legendary") },

        // Jay
        { -2145465549, new SkinInfo(1120354844, "Jay Default", "jay_default") },
        { -152602730, new SkinInfo(1120354844, "Jay Cali", "jay_cali") },
        { -1566346654, new SkinInfo(1120354844, "Jay Legendary", "jay_legendary") },

        // Billy
        { 27667358, new SkinInfo(581326566, "Billy Default", "billy_default") },
        { -1959135014, new SkinInfo(581326566, "Billy Jungle Camo", "billy_jungle_camo") },
        { 2025202852, new SkinInfo(581326566, "Billy Flames", "billy_flames") },
        { -1067636066, new SkinInfo(581326566, "Billy Legendary", "billy_legendary") },

        // Rosalita
        { -1389912410, new SkinInfo(-1505268145, "Rosalita Default", "rosalita_default") },
        { 389528075, new SkinInfo(-1505268145, "Rosalita Black/Burgundy", "rosalita_black_burgundy") },
        { -1487295210, new SkinInfo(-1505268145, "Rosalita Legendary", "rosalita_legendary") },

        // Tasha
        { -503301639, new SkinInfo(299562833, "Tasha Default", "tasha_default") },
        { -1312184589, new SkinInfo(299562833, "Tasha Green", "tasha_green") },
        { 288125499, new SkinInfo(299562833, "Tasha Red/White", "tasha_red_white") },
        { -889435486, new SkinInfo(299562833, "Tasha Legendary", "tasha_legendary") },

        // Jaewoo
        { -1312276630, new SkinInfo(-1733051898, "Jaewoo Default", "jaewoo_default") },
        { -1360992679, new SkinInfo(-1733051898, "Jaewoo Black/Magenta", "jaewoo_black_magenta") },
        { -1505271108, new SkinInfo(-1733051898, "Jaewoo Green/Purple", "jaewoo_green_purple") },
        { 374299079, new SkinInfo(-1733051898, "Jaewoo Legendary", "jaewoo_legendary") },

        // Tagbot
        { -1236251198, new SkinInfo(1887684367, "Tagbot Default", "tagbot_default") },
        { 1864637985, new SkinInfo(1887684367, "Tagbot Red", "tagbot_red") },
        { -209858400, new SkinInfo(1887684367, "Tagbot Legendary", "tagbot_legendary") },

        // Lilah
        { -789829533, new SkinInfo(-2116248615, "Lilah Default", "lilah_default") },
        { -1597270950, new SkinInfo(-2116248615, "Lilah Green Camo", "lilah_green_camo") },

        // Zara
        { 1093072756, new SkinInfo(-2075784936, "Zara Default", "zara_default") },
        { 1870421642, new SkinInfo(-2075784936, "Zara Holo Zebra", "zara_holo_zebra") },

        // Ash
        { 816980720, new SkinInfo(966716028, "Ash Default", "ash_default") },
        { 1150862217, new SkinInfo(966716028, "Ash Green/Orange", "ash_green_orange") },

        // Jenny
        { 362958220, new SkinInfo(1363767693, "Jenny Default", "jenny_default") },
        { -1287581543, new SkinInfo(1363767693, "Jenny Color Patches", "jenny_color_patches") },

        // Wei
        { -2092832313, new SkinInfo(-518167090, "Wei Default", "wei_default") },
        { -871825427, new SkinInfo(-518167090, "Wei Teal/Orange", "wei_teal_orange") },

        // Prince K
        { -1112152883, new SkinInfo(1936280213, "Prince K Default", "prince_k_default") },
        { 1781172277, new SkinInfo(1936280213, "Prince K Red", "prince_k_red") },

        // Monique
        { -1415696609, new SkinInfo(135046766, "Monique Default", "monique_default") },
        { 1335121268, new SkinInfo(135046766, "Monique Green/Mustard", "monique_green_mustard") },

        // Noon
        { 1962475607, new SkinInfo(-502265868, "Noon Default", "noon_default") },
        { 748483822, new SkinInfo(-502265868, "Noon Blue", "noon_blue") },

        // V3ctor
        { 666084587, new SkinInfo(-1534276928, "V3ctor Default", "v3ctor_default") },
        { -2129570054, new SkinInfo(-1534276928, "V3ctor VR", "v3ctor_vr") },
        { -1857764588, new SkinInfo(-1534276928, "V3ctor Pink", "v3ctor_pink") },
        { 1014902922, new SkinInfo(-1534276928, "V3ctor Legendary", "v3ctor_legendary") },

        // Georgie
        { 2085271833, new SkinInfo(-2125407733, "Georgie Default", "georgie_default") },
        { -1648267179, new SkinInfo(-2125407733, "Georgie Chicago Bulls", "georgie_chicago_bulls") },
        { 1136726127, new SkinInfo(-2125407733, "Georgie Bling", "georgie_bling") },

        // Lucy
        { 726823100, new SkinInfo(852717139, "Lucy Default", "lucy_default") },
        { 1076495376, new SkinInfo(852717139, "Lucy Legendary", "lucy_legendary") },
        { 1973379545, new SkinInfo(852717139, "Lucy Grave Queen", "lucy_grave_queen") },
        { -122811436, new SkinInfo(852717139, "Lucy Purple", "lucy_purple") },

        // Harini
        { -1041693778, new SkinInfo(823378763, "Harini Default", "harini_default") }
    });

    /// <summary>
    /// Возвращает список скинов, принадлежащих указанному серферу.
    /// </summary>
    /// <param name="surferDataTag">DataTag серфера-владельца.</param>
    /// <returns>Список пар (DataTag скина, информация о скине).</returns>
    public static List<KeyValuePair<int, SkinInfo>> GetSkinsForSurfer(int surferDataTag)
    {
        return Skins
            .Where(kv => kv.Value.OwnerId == surferDataTag)
            .ToList();
    }

    /// <summary>
    /// Возвращает имя скина по его DataTag.
    /// </summary>
    /// <param name="dataTag">DataTag скина.</param>
    /// <returns>Имя скина или строка с ID.</returns>
    public static string GetName(int dataTag)
    {
        return Skins.TryGetValue(dataTag, out var info) ? info.DisplayName : dataTag.ToString();
    }
}
