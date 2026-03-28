using System;
using System.IO;

namespace CityEdit.Android.Services;

/// <summary>
/// Android-реализация доступа к файлу профиля.
/// Использует ShizukuBridge для чтения/записи файлов
/// в защищённой директории Android/data через Shizuku API.
/// </summary>
public class AndroidFileAccessService : CityEdit.Services.IFileAccessService
{
    /// <summary>
    /// Абсолютный путь к файлу профиля Subway Surfers City.
    /// </summary>
    private const string GameProfilePath =
        "/storage/emulated/0/Android/data/com.sybogames.subway.surfers.game/files/enc/profile";

    /// <summary>
    /// Путь по умолчанию для "Сохранить как".
    /// </summary>
    private const string DefaultSaveDir =
        "/storage/emulated/0/Download/CityEdit";

    /// <summary>
    /// Callback, вызываемый когда разрешение Shizuku получено/отклонено.
    /// Используется ViewModel для автоматической реакции на результат.
    /// </summary>
    public event Action<bool>? PermissionResultReceived;

    public AndroidFileAccessService()
    {
        // Регистрируем listener один раз
        Java.ShizukuBridge.RegisterPermissionListener();

        // Подписываемся на результат
        Java.ShizukuBridge.OnPermissionResult += granted =>
        {
            PermissionResultReceived?.Invoke(granted);
        };
    }

    /// <summary>
    /// Android-платформа всегда требует Shizuku для доступа к Android/data.
    /// </summary>
    public bool RequiresShizuku => true;

    /// <summary>
    /// Проверяет доступность Shizuku (PingBinder).
    /// </summary>
    public bool IsShizukuAvailable
    {
        get
        {
            try
            {
                var bridge = new Java.ShizukuBridge();
                return bridge.IsServiceRunning();
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Проверяет наличие разрешения Shizuku (CheckSelfPermission).
    /// </summary>
    public bool HasShizukuPermission
    {
        get
        {
            try
            {
                var bridge = new Java.ShizukuBridge();
                return bridge.HasPermission();
            }
            catch
            {
                return false;
            }
        }
    }

    /// <summary>
    /// Проверяет, существует ли файл профиля.
    /// </summary>
    public bool CanAccessGameProfile()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            return bridge.FileExists(GameProfilePath);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Читает файл профиля через Shizuku.
    /// </summary>
    public byte[] ReadGameProfile()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            return bridge.ReadFile(GameProfilePath);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Не удалось прочитать файл профиля через Shizuku: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Записывает данные в файл профиля через Shizuku.
    /// </summary>
    public void WriteGameProfile(byte[] data)
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            bridge.WriteFile(GameProfilePath, data);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                $"Не удалось записать файл профиля через Shizuku: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Сохраняет данные по указанному пути (Download/CityEdit).
    /// </summary>
    public void SaveToPath(string path, byte[] data)
    {
        var dir = Path.GetDirectoryName(path);
        if (dir != null && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }
        File.WriteAllBytes(path, data);
    }

    /// <summary>
    /// Возвращает путь по умолчанию для "Сохранить как".
    /// </summary>
    public string? GetDefaultSavePath()
    {
        if (!Directory.Exists(DefaultSaveDir))
        {
            Directory.CreateDirectory(DefaultSaveDir);
        }
        return DefaultSaveDir;
    }

    /// <summary>
    /// Запрашивает разрешение Shizuku.
    /// </summary>
    public void RequestShizukuPermission()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            bridge.RequestPermission();
        }
        catch
        {
            // Ошибка запроса -- пользователь повторит
        }
    }

    /// <summary>
    /// Package name игры.
    /// </summary>
    private const string GamePackageName = "com.sybogames.subway.surfers.game";

    /// <summary>
    /// Останавливает процесс игры через Shizuku shell.
    /// Используем `am kill` вместо `am force-stop`:
    /// - force-stop очищает кэш приложения и ставит флаг «stopped»,
    ///   из-за чего следующий запуск — полный cold start (20-30 сек).
    /// - kill только убивает процесс, сохраняя кэш, что даёт быстрый перезапуск.
    /// </summary>
    public void KillGameProcess()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            // am kill — мягкое завершение, сохраняет кэш приложения
            bridge.ExecuteShell($"am kill {GamePackageName}");
        }
        catch
        {
            // Игнорируем ошибки — игра могла быть не запущена
        }
    }

    /// <summary>
    /// Принудительная остановка игры (полная очистка).
    /// Используется только когда нужно гарантировать, что игра перечитает профиль.
    /// </summary>
    public void ForceStopGame()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            bridge.ExecuteShell($"am force-stop {GamePackageName}");
        }
        catch
        {
            // Игнорируем ошибки
        }
    }

    /// <summary>
    /// Запускает игру через Shizuku shell (am start).
    /// Быстрее чем Intent — выполняется напрямую через shell,
    /// не зависит от состояния «stopped» после force-stop.
    /// </summary>
    public void LaunchGame()
    {
        try
        {
            var bridge = new Java.ShizukuBridge();
            // Запускаем через monkey — самый быстрый способ запустить launcher activity
            bridge.ExecuteShell($"monkey -p {GamePackageName} -c android.intent.category.LAUNCHER 1");
        }
        catch
        {
            // Fallback: через Intent
            try
            {
                var context = global::Android.App.Application.Context;
                var intent = context.PackageManager?.GetLaunchIntentForPackage(GamePackageName);
                if (intent != null)
                {
                    intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
            }
            catch
            {
                // Игнорируем — игра может быть не установлена
            }
        }
    }
}
