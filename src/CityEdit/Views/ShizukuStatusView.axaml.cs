using Avalonia.Controls;

namespace CityEdit.Views;

/// <summary>
/// Экран статуса Shizuku.
/// Отображается на Android, когда Shizuku недоступен или не выдано разрешение.
/// Предоставляет инструкции для пользователя и кнопки действий.
/// </summary>
public partial class ShizukuStatusView : UserControl
{
    /// <summary>
    /// Конструктор.
    /// </summary>
    public ShizukuStatusView()
    {
        InitializeComponent();
    }
}
