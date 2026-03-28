using System;
using System.Text;
using System.Text.Json;
using CityEdit.Core;

namespace CityEdit.Crypto;

/// <summary>
/// Сервис шифрования и дешифрования файлов профиля Subway Surfers City.
/// Формат файла: [IV: 16 байт][KEY: 16 байт][ENCRYPTED_DATA: остальное].
/// Данные шифруются с помощью AES-CTR.
/// </summary>
public static class ProfileCrypto
{
    /// <summary>
    /// Результат расшифровки файла профиля.
    /// </summary>
    /// <param name="Iv">Вектор инициализации (16 байт).</param>
    /// <param name="Key">Ключ шифрования (16 байт).</param>
    /// <param name="JsonData">Расшифрованные данные в виде JSON-документа.</param>
    public record DecryptedProfile(byte[] Iv, byte[] Key, JsonDocument JsonData);

    /// <summary>
    /// Расшифровывает файл профиля из массива байтов.
    /// </summary>
    /// <param name="rawData">Сырые данные файла профиля.</param>
    /// <returns>Расшифрованный профиль с IV, ключом и JSON-данными.</returns>
    /// <exception cref="ArgumentException">Если файл слишком мал (менее 32 байт).</exception>
    /// <exception cref="JsonException">Если расшифрованные данные не являются корректным JSON.</exception>
    public static DecryptedProfile Decrypt(byte[] rawData)
    {
        if (rawData.Length < Constants.MinProfileFileSize)
        {
            throw new ArgumentException(
                $"Файл профиля повреждён: размер {rawData.Length} байт, " +
                $"минимально допустимый {Constants.MinProfileFileSize} байт.");
        }

        // Извлекаем IV (первые 16 байт) и KEY (следующие 16 байт)
        var iv = new byte[Constants.IvSize];
        var key = new byte[Constants.KeySize];
        Array.Copy(rawData, 0, iv, 0, Constants.IvSize);
        Array.Copy(rawData, Constants.IvSize, key, 0, Constants.KeySize);

        // Извлекаем зашифрованные данные (всё после IV и KEY)
        int encryptedLength = rawData.Length - Constants.MinProfileFileSize;
        var encryptedData = new byte[encryptedLength];
        Array.Copy(rawData, Constants.MinProfileFileSize, encryptedData, 0, encryptedLength);

        // Дешифруем через AES-CTR
        using var cipher = new AesCtrCipher(key);
        byte[] decryptedBytes = cipher.Transform(iv, encryptedData);

        // Преобразуем в JSON
        string jsonString = Encoding.UTF8.GetString(decryptedBytes);
        var jsonDocument = JsonDocument.Parse(jsonString);

        return new DecryptedProfile(iv, key, jsonDocument);
    }

    /// <summary>
    /// Расшифровывает файл профиля из файла на диске.
    /// </summary>
    /// <param name="filePath">Путь к файлу профиля.</param>
    /// <returns>Расшифрованный профиль с IV, ключом и JSON-данными.</returns>
    public static DecryptedProfile DecryptFromFile(string filePath)
    {
        byte[] rawData = System.IO.File.ReadAllBytes(filePath);
        return Decrypt(rawData);
    }

    /// <summary>
    /// Шифрует данные профиля и возвращает массив байтов для записи в файл.
    /// </summary>
    /// <param name="iv">Вектор инициализации (16 байт).</param>
    /// <param name="key">Ключ шифрования (16 байт).</param>
    /// <param name="jsonData">JSON-данные для шифрования.</param>
    /// <returns>Массив байтов в формате [IV][KEY][ENCRYPTED_DATA].</returns>
    public static byte[] Encrypt(byte[] iv, byte[] key, JsonElement jsonData)
    {
        // Сериализуем JSON в строку
        string jsonString = jsonData.GetRawText();
        byte[] plainBytes = Encoding.UTF8.GetBytes(jsonString);

        // Шифруем через AES-CTR
        using var cipher = new AesCtrCipher(key);
        byte[] encryptedBytes = cipher.Transform(iv, plainBytes);

        // Собираем результат: IV + KEY + зашифрованные данные
        var result = new byte[Constants.IvSize + Constants.KeySize + encryptedBytes.Length];
        Array.Copy(iv, 0, result, 0, Constants.IvSize);
        Array.Copy(key, 0, result, Constants.IvSize, Constants.KeySize);
        Array.Copy(encryptedBytes, 0, result, Constants.MinProfileFileSize, encryptedBytes.Length);

        return result;
    }

    /// <summary>
    /// Шифрует данные профиля и записывает в файл.
    /// </summary>
    /// <param name="iv">Вектор инициализации (16 байт).</param>
    /// <param name="key">Ключ шифрования (16 байт).</param>
    /// <param name="jsonData">JSON-данные для шифрования.</param>
    /// <param name="outputPath">Путь для записи зашифрованного файла.</param>
    public static void EncryptToFile(byte[] iv, byte[] key, JsonElement jsonData, string outputPath)
    {
        byte[] result = Encrypt(iv, key, jsonData);
        System.IO.File.WriteAllBytes(outputPath, result);
    }
}
