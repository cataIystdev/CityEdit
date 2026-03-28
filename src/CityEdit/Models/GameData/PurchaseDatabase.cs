using System.Collections.Generic;

namespace CityEdit.Models.GameData;

/// <summary>
/// Справочник идентификаторов покупок Subway Surfers City.
/// Содержит предустановленные ProductID для быстрого добавления покупок.
/// </summary>
public static class PurchaseDatabase
{
    /// <summary>
    /// Список распространённых идентификаторов покупок.
    /// Порядок определяет отображение в выпадающем списке интерфейса.
    /// </summary>
    public static readonly IReadOnlyList<string> CommonPurchaseIds = new List<string>
    {
        "shop.currency.keypackfree",
        "shop.currency.keypack6",
        "shop.box.ad",
        "shop.box.super",
        "shop.box.super.free",
        "shop.box.admeter",
        "premium_pass",
        "home.offer.jakestarterpack",
        "home.offer.missmaiaintermediatepack",
        "home.offer.freshexpertpack",
        "shop.offer.billyunlock001",
        "shop.offer.ellaunlock001",
        "shop.offer.tagbotunlock001",
        "shop.offer.rosalitaunlock001",
        "shop.offer.yutaniunlock001",
        "shop.offer.tashaunlock001",
        "shop.offer.lilahunlock001",
        "shop.offer.zaraunlock001",
        "shop.offer.jaewoounlock001",
        "shop.offer.ashunlock001",
        "shop.offer.jennyunlock001",
        "shop.offer.weiunlock001",
        "shop.offer.princekunlock001",
        "shop.offer.moniqueunlock001",
        "shop.offer.noonunlock001",
        "shop.offer.lucyunlock001",
        "shop.offer.georgieunlock001",
        "shop.offer.v3ctorunlock001"
    };

    /// <summary>
    /// ProductID для удаления рекламы.
    /// </summary>
    public const string RemoveAdsId = "shop.currency.keypack6";

    /// <summary>
    /// ProductID для премиум-пропуска.
    /// </summary>
    public const string PremiumPassId = "premium_pass";

    /// <summary>
    /// Шаблон ProductID для бонусных треков.
    /// Формат: districttrial.premiumladder.{номер:D3}
    /// </summary>
    public const string BonusTrackTemplate = "districttrial.premiumladder.{0:D3}";
}
