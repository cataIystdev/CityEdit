using System.IO;

namespace CityEdit.Services;

/// <summary>
/// Десктопная реализация доступа к файлу профиля.
/// Использует стандартный File IO без каких-либо ограничений.
/// Shizuku-функциональность заглушена (не требуется на десктопе).
/// </summary>
public class DesktopFileAccessService : IFileAccessService
{
    /// <summary>
    /// Путь к файлу профиля, задаётся пользователем через диалог.
    /// </summary>
    private string? _profilePath;

    /// <summary>
    /// Shizuku не требуется на десктопе.
    /// </summary>
    public bool RequiresShizuku => false;

    /// <summary>
    /// На десктопе Shizuku всегда "доступен" (заглушка).
    /// </summary>
    public bool IsShizukuAvailable => true;

    /// <summary>
    /// На десктопе разрешение Shizuku всегда "выдано" (заглушка).
    /// </summary>
    public bool HasShizukuPermission => true;

    /// <summary>
    /// Устанавливает путь к файлу профиля (вызывается после выбора через диалог).
    /// </summary>
    /// <param name="path">Путь к файлу.</param>
    public void SetProfilePath(string path)
    {
        _profilePath = path;
    }

    /// <summary>
    /// Проверяет доступность файла профиля.
    /// </summary>
    public bool CanAccessGameProfile()
    {
        return _profilePath != null && File.Exists(_profilePath);
    }

    /// <summary>
    /// Читает файл профиля.
    /// </summary>
    public byte[] ReadGameProfile()
    {
        if (_profilePath == null || !File.Exists(_profilePath))
        {
            throw new FileNotFoundException("Файл профиля не задан или не найден.", _profilePath);
        }
        return File.ReadAllBytes(_profilePath);
    }

    /// <summary>
    /// Записывает данные в файл профиля.
    /// </summary>
    public void WriteGameProfile(byte[] data)
    {
        if (_profilePath == null)
        {
            throw new System.InvalidOperationException("Путь к файлу профиля не задан.");
        }
        File.WriteAllBytes(_profilePath, data);
    }

    /// <summary>
    /// Сохраняет данные по указанному пути.
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
    /// На десктопе путь по умолчанию отсутствует (используется диалог).
    /// </summary>
    public string? GetDefaultSavePath() => null;

    /// <summary>
    /// Заглушка -- на десктопе не используется.
    /// </summary>
    public void RequestShizukuPermission() { }

    /// <summary>
    /// Заглушка -- на десктопе не используется.
    /// </summary>
    public void KillGameProcess() { }

    /// <summary>
    /// Заглушка -- на десктопе не используется.
    /// </summary>
    public void ForceStopGame() { }

    /// <summary>
    /// Заглушка -- на десктопе не используется.
    /// </summary>
    public void LaunchGame() { }
}
