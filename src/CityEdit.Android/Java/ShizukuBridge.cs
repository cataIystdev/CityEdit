using System;
using System.IO;
using Android.OS;
using Rikka.Shizuku;

namespace CityEdit.Android.Java;

/// <summary>
/// Мост к Shizuku через официальные AAR bindings.
/// 
/// Использует:
/// - Shizuku.PingBinder() для проверки доступности
/// - Shizuku.CheckSelfPermission() для проверки разрешений
/// - Рефлексию для вызова Shizuku.newProcess() (private method)
///   который внутри вызывает IShizukuService.newProcess()
/// </summary>
public class ShizukuBridge
{
    /// <summary>
    /// AIDL интерфейс дескриптор IShizukuService.
    /// </summary>
    private const string SHIZUKU_DESCRIPTOR = "moe.shizuku.server.IShizukuService";

    /// <summary>
    /// Callback при получении разрешения.
    /// </summary>
    public static event Action<bool>? OnPermissionResult;

    /// <summary>
    /// Был ли зарегистрирован listener.
    /// </summary>
    private static bool _listenerRegistered;

    /// <summary>
    /// Регистрирует Listener на результат запроса разрешения.
    /// Вызывается один раз при инициализации.
    /// </summary>
    public static void RegisterPermissionListener()
    {
        if (_listenerRegistered) return;
        _listenerRegistered = true;

        try
        {
            Shizuku.AddRequestPermissionResultListener(new PermissionResultListener());
        }
        catch { }
    }

    /// <summary>
    /// Проверяет, запущен ли Shizuku.
    /// </summary>
    public bool IsServiceRunning()
    {
        try
        {
            return Shizuku.PingBinder();
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Проверяет, есть ли разрешение на использование Shizuku.
    /// Только CheckSelfPermission — без shell-вызовов.
    /// </summary>
    public bool HasPermission()
    {
        try
        {
            if (!Shizuku.PingBinder()) return false;
            // PackageManager.PERMISSION_GRANTED = 0
            return Shizuku.CheckSelfPermission() == 0;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Запрашивает разрешение Shizuku.
    /// </summary>
    public void RequestPermission()
    {
        try
        {
            if (Shizuku.PingBinder())
            {
                Shizuku.RequestPermission(42);
            }
            else
            {
                // Открываем Shizuku для активации
                var context = global::Android.App.Application.Context;
                var intent = context.PackageManager?.GetLaunchIntentForPackage("moe.shizuku.privileged.api");
                if (intent != null)
                {
                    intent.AddFlags(global::Android.Content.ActivityFlags.NewTask);
                    context.StartActivity(intent);
                }
            }
        }
        catch { }
    }

    /// <summary>
    /// Проверяет существование файла через shell.
    /// </summary>
    public bool FileExists(string path)
    {
        try
        {
            global::Android.Util.Log.Info("ShizukuBridge", $"FileExists: checking '{path}'");
            string output = ExecuteShizukuShell($"test -f '{path}' && echo 'EXISTS' || echo 'NOTFOUND'");
            global::Android.Util.Log.Info("ShizukuBridge", $"FileExists result: '{output.Trim()}'");
            return output.Contains("EXISTS");
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("ShizukuBridge", $"FileExists exception: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Читает файл через Shizuku shell: cat -> base64 -> stdout -> decode.
    /// </summary>
    public byte[] ReadFile(string path)
    {
        global::Android.Util.Log.Info("ShizukuBridge", $"ReadFile: '{path}'");
        string result = ExecuteShizukuShell($"cat '{path}' | base64 2>&1");

        if (string.IsNullOrWhiteSpace(result))
            throw new IOException("Пустой результат при чтении файла");

        try
        {
            // Убираем возможные переносы строк в base64
            string cleanBase64 = result.Replace("\n", "").Replace("\r", "").Trim();
            byte[] data = Convert.FromBase64String(cleanBase64);
            global::Android.Util.Log.Info("ShizukuBridge", $"ReadFile: got {data.Length} bytes");
            return data;
        }
        catch (FormatException ex)
        {
            global::Android.Util.Log.Error("ShizukuBridge", $"Base64 decode error: {ex.Message}, output start: '{result.Substring(0, Math.Min(100, result.Length))}'");
            throw new IOException($"Ошибка декодирования файла: {ex.Message}");
        }
    }

    /// <summary>
    /// Записывает файл: File.WriteAllBytes -> temp -> Shizuku cp.
    /// После записи выполняет sync для гарантии flush на диск.
    /// </summary>
    public void WriteFile(string path, byte[] data)
    {
        // /data/local/tmp доступен для Shizuku shell (uid 2000)
        string tempFile = $"/data/local/tmp/ce_w_{Guid.NewGuid():N}";

        try
        {
            // Записываем во временный файл через Shizuku (base64)
            string base64 = Convert.ToBase64String(data);
            ExecuteShizukuShell($"echo '{base64}' | base64 -d > '{tempFile}'");
            string result = ExecuteShizukuShell(
                $"cp '{tempFile}' '{path}' 2>&1 && echo 'DONE'");

            if (!result.Contains("DONE"))
                throw new IOException($"Ошибка записи: {result.Trim()}");

            // Принудительно сбрасываем файловые буферы на диск
            ExecuteShizukuShell("sync");
        }
        finally
        {
            // Удаляем temp через Shizuku
            try { ExecuteShizukuShell($"rm -f '{tempFile}'"); } catch { }
        }
    }

    /// <summary>
    /// Публичный метод для выполнения shell-команды через Shizuku.
    /// </summary>
    public string ExecuteShell(string command)
    {
        return ExecuteShizukuShell(command);
    }

    /// <summary>
    /// Выполняет shell-команду через Shizuku.
    /// Использует рефлексию для вызова Shizuku.newProcess() (private метод),
    /// который внутри вызывает IShizukuService.newProcess().
    /// </summary>
    private static string ExecuteShizukuShell(string command)
    {
        global::Android.Util.Log.Info("ShizukuBridge", $"ExecuteShizukuShell: '{command}'");
        var cmd = new[] { "sh", "-c", command };

        try
        {
            // Вызываем private static метод Shizuku.newProcess через рефлексию
            var shizukuType = typeof(Shizuku);
            var newProcessMethod = shizukuType.GetMethod("NewProcess",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static,
                null,
                new[] { typeof(string[]), typeof(string[]), typeof(string) },
                null);

            if (newProcessMethod == null)
            {
                global::Android.Util.Log.Error("ShizukuBridge", "NewProcess method not found via reflection. Trying Java reflection...");
                return ExecuteShizukuShellJavaReflection(cmd);
            }

            global::Android.Util.Log.Info("ShizukuBridge", "Calling Shizuku.newProcess via .NET reflection...");
            var process = (ShizukuRemoteProcess?)newProcessMethod.Invoke(null, new object?[] { cmd, null, null });

            if (process == null)
                throw new IOException("ShizukuRemoteProcess is null");

            global::Android.Util.Log.Info("ShizukuBridge", "Got process, reading output...");
            using var inputStream = process.InputStream;
            if (inputStream == null)
                throw new IOException("InputStream is null");

            using var reader = new StreamReader(inputStream);
            string output = reader.ReadToEnd();
            process.WaitFor();
            global::Android.Util.Log.Info("ShizukuBridge", $"Shell output: '{output.Trim()}'");
            return output;
        }
        catch (Exception ex)
        {
            global::Android.Util.Log.Error("ShizukuBridge", $"ExecuteShizukuShell error: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Fallback: вызываем Shizuku.newProcess через Java рефлексию.
    /// </summary>
    private static string ExecuteShizukuShellJavaReflection(string[] cmd)
    {
        // Используем Java рефлексию на Shizuku.class
        var shizukuClass = global::Java.Lang.Class.FromType(typeof(Shizuku));
        var stringArrayClass = global::Java.Lang.Class.ForName("[Ljava.lang.String;");
        var stringClass = global::Java.Lang.Class.ForName("java.lang.String");

        var method = shizukuClass?.GetDeclaredMethod("newProcess",
            stringArrayClass, stringArrayClass, stringClass);

        if (method == null)
            throw new IOException("Cannot find Shizuku.newProcess via Java reflection");

        method.Accessible = true;
        global::Android.Util.Log.Info("ShizukuBridge", "Calling Shizuku.newProcess via Java reflection...");

        var jResult = method.Invoke(null, new global::Java.Lang.String[] { }
            .Length == 0 ? cmd : cmd, null, null);

        // newProcess returns ShizukuRemoteProcess (extends Process)
        var process = jResult as global::Java.Lang.Process;
        if (process == null)
            throw new IOException("Java reflection: process is null");

        using var inputStream = process.InputStream;
        if (inputStream == null)
            throw new IOException("InputStream is null");

        using var reader = new StreamReader(inputStream);
        string output = reader.ReadToEnd();

        // Читаем stderr для диагностики
        string errorOutput = "";
        using (var errorStream = process.ErrorStream)
        {
            if (errorStream != null)
            {
                using var errorReader = new StreamReader(errorStream);
                errorOutput = errorReader.ReadToEnd();
            }
        }

        process.WaitFor();
        global::Android.Util.Log.Info("ShizukuBridge", $"Shell output (Java refl): '{output.Trim()}'");
        if (!string.IsNullOrEmpty(errorOutput))
            global::Android.Util.Log.Warn("ShizukuBridge", $"Shell stderr: '{errorOutput.Trim()}'");
        return output + errorOutput;
    }

    /// <summary>
    /// Listener для результата запроса разрешения Shizuku.
    /// </summary>
    private class PermissionResultListener : global::Java.Lang.Object, Shizuku.IOnRequestPermissionResultListener
    {
        public void OnRequestPermissionResult(int requestCode, int grantResult)
        {
            // grantResult == 0 = PERMISSION_GRANTED
            bool granted = grantResult == 0;
            OnPermissionResult?.Invoke(granted);
        }
    }
}
