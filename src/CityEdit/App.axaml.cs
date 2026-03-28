using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using CityEdit.Services;
using CityEdit.ViewModels;
using CityEdit.Views;

namespace CityEdit;

/// <summary>
/// Основной класс приложения CityEdit.
/// Поддерживает два режима запуска:
/// - Desktop: IClassicDesktopStyleApplicationLifetime (MainWindow)
/// - Android: ISingleViewApplicationLifetime (MobileMainView)
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Платформозависимый сервис доступа к файлам.
    /// Устанавливается из MainActivity (Android) перед инициализацией UI.
    /// </summary>
    public static IFileAccessService? FileAccessService { get; set; }

    /// <summary>
    /// ViewModel мобильного режима. Доступен из platform-specific кода
    /// для подключения callback'ов (Shizuku permission и т.д.).
    /// </summary>
    public static MainWindowViewModel? MobileViewModel { get; private set; }

    /// <summary>
    /// Инициализирует компоненты AXAML.
    /// </summary>
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    /// <summary>
    /// Вызывается при готовности фреймворка.
    /// Определяет тип платформы и создаёт соответствующее окно/view.
    /// </summary>
    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Десктопный режим: используем MainWindow с боковой панелью
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            // Мобильный режим: используем MobileMainView с горизонтальной навигацией
            var vm = new MainWindowViewModel(isMobile: true);

            // Внедряем платформозависимый сервис файлового доступа
            if (FileAccessService != null)
            {
                vm.SetFileAccessService(FileAccessService);
            }

            // Сохраняем ViewModel для подключения callback'ов из platform-specific кода
            MobileViewModel = vm;

            singleView.MainView = new MobileMainView
            {
                DataContext = vm
            };
        }

        base.OnFrameworkInitializationCompleted();
    }
}