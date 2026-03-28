using Android.App;
using Android.Content.PM;
using Android.OS;
using Avalonia;
using Avalonia.Android;
using CityEdit.Android.Services;
using CityEdit.ViewModels;

namespace CityEdit.Android;

/// <summary>
/// Точка входа Android-приложения CityEdit.
/// Наследует AvaloniaMainActivity для интеграции Avalonia UI с Android Activity lifecycle.
/// Инициализирует Shizuku-сервис файлового доступа и передаёт его в ViewModel.
/// </summary>
[Activity(
    Label = "CityEdit",
    Theme = "@style/MyTheme.NoActionBar",
    Icon = "@drawable/icon",
    MainLauncher = true,
    ConfigurationChanges = ConfigChanges.Orientation |
                           ConfigChanges.ScreenSize |
                           ConfigChanges.UiMode)]
public class MainActivity : AvaloniaMainActivity<App>
{
    /// <summary>
    /// Настраивает AppBuilder Avalonia с платформозависимыми компонентами.
    /// </summary>
    protected override AppBuilder CustomizeAppBuilder(AppBuilder builder)
    {
        return base.CustomizeAppBuilder(builder)
            .WithInterFont()
            .With(new AndroidPlatformOptions
            {
                // Используем GPU-ускоренный рендеринг для плавного скроллинга
                RenderingMode = new[]
                {
                    AndroidRenderingMode.Vulkan,
                    AndroidRenderingMode.Egl,
                }
            })
            .LogToTrace()
            .AfterSetup(_ => InitializeServices());
    }

    /// <summary>
    /// Инициализирует платформозависимые сервисы.
    /// Создаёт AndroidFileAccessService и внедряет его в ViewModel.
    /// Подключает callback для автоматической реакции на разрешение Shizuku.
    /// </summary>
    private void InitializeServices()
    {
        var service = new AndroidFileAccessService();
        App.FileAccessService = service;

        // Подключаем callback: при получении разрешения Shizuku -> перепроверить и загрузить
        service.PermissionResultReceived += granted =>
        {
            App.MobileViewModel?.OnShizukuPermissionGranted(granted);
        };
    }
}
