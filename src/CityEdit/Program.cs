#if !ANDROID
using Avalonia;
using System;

namespace CityEdit;

/// <summary>
/// Точка входа десктопного приложения CityEdit.
/// На Android используется MainActivity из проекта CityEdit.Android.
/// </summary>
sealed class Program
{
    /// <summary>
    /// Запуск Avalonia UI с классическим десктопным жизненным циклом.
    /// </summary>
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    /// <summary>
    /// Конфигурация Avalonia. Используется также визуальным дизайнером.
    /// </summary>
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}
#endif
