namespace CityEdit.Services;

/// <summary>
/// Интерфейс доступа к файлу профиля игры.
/// Абстрагирует платформозависимый доступ к файловой системе.
/// На десктопе используется стандартный File IO.
/// На Android используется Shizuku для доступа к Android/data.
/// </summary>
public interface IFileAccessService
{
    /// <summary>
    /// Проверяет, доступен ли файл профиля игры для чтения.
    /// На Android проверяет наличие Shizuku и соответствующих разрешений.
    /// </summary>
    /// <returns>true, если файл профиля доступен для чтения.</returns>
    bool CanAccessGameProfile();

    /// <summary>
    /// Читает файл профиля игры в виде массива байтов.
    /// На Android читает через Shizuku из Android/data.
    /// </summary>
    /// <returns>Содержимое файла профиля.</returns>
    /// <exception cref="System.IO.FileNotFoundException">Файл профиля не найден.</exception>
    /// <exception cref="System.InvalidOperationException">Нет доступа к файлу.</exception>
    byte[] ReadGameProfile();

    /// <summary>
    /// Записывает данные в файл профиля игры.
    /// На Android записывает через Shizuku в Android/data.
    /// </summary>
    /// <param name="data">Данные для записи.</param>
    /// <exception cref="System.InvalidOperationException">Нет доступа к файлу.</exception>
    void WriteGameProfile(byte[] data);

    /// <summary>
    /// Сохраняет данные в указанный путь.
    /// Для команды "Сохранить как" -- использует стандартные пути (Download/CityEdit).
    /// </summary>
    /// <param name="path">Путь для сохранения.</param>
    /// <param name="data">Данные для записи.</param>
    void SaveToPath(string path, byte[] data);

    /// <summary>
    /// Возвращает путь по умолчанию для "Сохранить как".
    /// На десктопе возвращает null (используется диалог выбора файла).
    /// На Android возвращает Download/CityEdit/.
    /// </summary>
    string? GetDefaultSavePath();

    /// <summary>
    /// Признак того, что сервис требует Shizuku (только Android).
    /// На десктопе всегда false.
    /// </summary>
    bool RequiresShizuku { get; }

    /// <summary>
    /// Проверяет, доступен ли Shizuku (только Android).
    /// На десктопе всегда true.
    /// </summary>
    bool IsShizukuAvailable { get; }

    /// <summary>
    /// Проверяет, выдано ли разрешение Shizuku (только Android).
    /// На десктопе всегда true.
    /// </summary>
    bool HasShizukuPermission { get; }

    /// <summary>
    /// Запрашивает разрешение Shizuku (только Android).
    /// На десктопе ничего не делает.
    /// </summary>
    void RequestShizukuPermission();

    /// <summary>
    /// Мягко останавливает процесс игры (am kill).
    /// Сохраняет кэш для быстрого перезапуска.
    /// </summary>
    void KillGameProcess();

    /// <summary>
    /// Принудительно останавливает игру (am force-stop).
    /// Очищает кэш, следующий запуск будет cold start.
    /// Нужно когда необходимо гарантировать перечитывание профиля.
    /// </summary>
    void ForceStopGame();

    /// <summary>
    /// Запускает игру.
    /// </summary>
    void LaunchGame();
}
