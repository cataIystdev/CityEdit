using System;
using System.IO;
using System.Text;
using System.Text.Json;
using CityEdit.Crypto;
using Xunit;

namespace CityEdit.Tests.Crypto;

/// <summary>
/// Тесты AES-CTR шифра.
/// Проверяет корректность шифрования/дешифрования и инкремента счётчика.
/// </summary>
public class AesCtrCipherTests
{
    /// <summary>
    /// Проверяет, что шифрование + дешифрование возвращает исходные данные.
    /// AES-CTR симметричен: Transform(Transform(data)) == data.
    /// </summary>
    [Fact]
    public void Transform_EncryptThenDecrypt_ReturnsOriginalData()
    {
        // Arrange
        var key = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var iv = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        var plaintext = Encoding.UTF8.GetBytes("Hello, CityEdit! This is a test message for AES-CTR.");

        // Act
        using var cipher1 = new AesCtrCipher(key);
        var ciphertext = cipher1.Transform(iv, plaintext);

        using var cipher2 = new AesCtrCipher(key);
        var decrypted = cipher2.Transform(iv, ciphertext);

        // Assert
        Assert.Equal(plaintext, decrypted);
    }

    /// <summary>
    /// Проверяет, что разные IV дают разный шифротекст.
    /// </summary>
    [Fact]
    public void Transform_DifferentIVs_ProduceDifferentCiphertext()
    {
        var key = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var iv1 = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1 };
        var iv2 = new byte[16] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 2 };
        var plaintext = Encoding.UTF8.GetBytes("Same message encrypted with different IVs");

        using var cipher1 = new AesCtrCipher(key);
        var ct1 = cipher1.Transform(iv1, plaintext);

        using var cipher2 = new AesCtrCipher(key);
        var ct2 = cipher2.Transform(iv2, plaintext);

        Assert.NotEqual(ct1, ct2);
    }

    /// <summary>
    /// Проверяет обработку неполных блоков (данные не кратны 16 байтам).
    /// </summary>
    [Fact]
    public void Transform_PartialBlock_HandledCorrectly()
    {
        var key = new byte[16] { 10, 20, 30, 40, 50, 60, 70, 80, 90, 100, 110, 120, 130, 140, 150, 160 };
        var iv = new byte[16];
        var plaintext = new byte[] { 1, 2, 3, 4, 5 }; // 5 байт, неполный блок

        using var cipher1 = new AesCtrCipher(key);
        var ciphertext = cipher1.Transform(iv, plaintext);
        Assert.Equal(5, ciphertext.Length);

        using var cipher2 = new AesCtrCipher(key);
        var decrypted = cipher2.Transform(iv, ciphertext);
        Assert.Equal(plaintext, decrypted);
    }

    /// <summary>
    /// Проверяет, что пустой массив обрабатывается без ошибок.
    /// </summary>
    [Fact]
    public void Transform_EmptyData_ReturnsEmptyArray()
    {
        var key = new byte[16];
        var iv = new byte[16];

        using var cipher = new AesCtrCipher(key);
        var result = cipher.Transform(iv, Array.Empty<byte>());

        Assert.Empty(result);
    }

    /// <summary>
    /// Проверяет, что неправильная длина ключа вызывает исключение.
    /// </summary>
    [Fact]
    public void Constructor_InvalidKeyLength_ThrowsArgumentException()
    {
        Assert.Throws<ArgumentException>(() => new AesCtrCipher(new byte[15]));
        Assert.Throws<ArgumentException>(() => new AesCtrCipher(new byte[17]));
    }

    /// <summary>
    /// Проверяет, что неправильная длина IV вызывает исключение.
    /// </summary>
    [Fact]
    public void Transform_InvalidIVLength_ThrowsArgumentException()
    {
        var key = new byte[16];
        using var cipher = new AesCtrCipher(key);

        Assert.Throws<ArgumentException>(() => cipher.Transform(new byte[15], new byte[32]));
    }

    /// <summary>
    /// Проверяет обработку данных из нескольких блоков (больше 16 байт).
    /// </summary>
    [Fact]
    public void Transform_MultipleBlocks_CorrectlyProcessed()
    {
        var key = new byte[16] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
        var iv = new byte[16];

        // 48 байт = 3 полных блока
        var plaintext = new byte[48];
        for (int i = 0; i < 48; i++)
        {
            plaintext[i] = (byte)(i % 256);
        }

        using var cipher1 = new AesCtrCipher(key);
        var ciphertext = cipher1.Transform(iv, plaintext);

        using var cipher2 = new AesCtrCipher(key);
        var decrypted = cipher2.Transform(iv, ciphertext);

        Assert.Equal(plaintext, decrypted);
    }
}

/// <summary>
/// Тесты шифрования/дешифрования профиля.
/// Проверяет интеграцию с реальным файлом profile.
/// </summary>
public class ProfileCryptoTests
{
    /// <summary>
    /// Путь к реальному файлу профиля для интеграционного тестирования.
    /// </summary>
    private static readonly string TestProfilePath = Path.Combine(
        Directory.GetCurrentDirectory(), "..", "..", "..", "..", "..", "profile");

    /// <summary>
    /// Проверяет расшифровку реального файла профиля.
    /// Ожидает наличие поля "profile" с вложенным JSON.
    /// </summary>
    [Fact]
    public void Decrypt_RealProfile_ContainsExpectedFields()
    {
        if (!File.Exists(TestProfilePath))
        {
            // Пропускаем тест, если файл не существует
            return;
        }

        var result = ProfileCrypto.DecryptFromFile(TestProfilePath);

        Assert.NotNull(result);
        Assert.NotNull(result.Iv);
        Assert.NotNull(result.Key);
        Assert.Equal(16, result.Iv.Length);
        Assert.Equal(16, result.Key.Length);

        var root = result.JsonData.RootElement;
        Assert.True(root.TryGetProperty("profile", out _), "Поле 'profile' должно существовать");
        Assert.True(root.TryGetProperty("version", out _), "Поле 'version' должно существовать");
    }

    /// <summary>
    /// Проверяет, что шифрование + дешифрование возвращает те же данные.
    /// </summary>
    [Fact]
    public void EncryptDecrypt_RoundTrip_PreservesData()
    {
        if (!File.Exists(TestProfilePath))
        {
            return;
        }

        // Расшифровываем
        var original = ProfileCrypto.DecryptFromFile(TestProfilePath);
        var originalJson = original.JsonData.RootElement.GetRawText();

        // Шифруем обратно
        var encrypted = ProfileCrypto.Encrypt(original.Iv, original.Key, original.JsonData.RootElement);

        // Расшифровываем снова
        var restored = ProfileCrypto.Decrypt(encrypted);
        var restoredJson = restored.JsonData.RootElement.GetRawText();

        // Сравниваем
        Assert.Equal(originalJson, restoredJson);
    }

    /// <summary>
    /// Проверяет расшифровку внутреннего профиля (вложенный JSON).
    /// </summary>
    [Fact]
    public void Decrypt_RealProfile_InnerProfileContainsSurfers()
    {
        if (!File.Exists(TestProfilePath))
        {
            return;
        }

        var result = ProfileCrypto.DecryptFromFile(TestProfilePath);
        var root = result.JsonData.RootElement;
        string profileStr = root.GetProperty("profile").GetString()!;
        var profile = JsonDocument.Parse(profileStr);

        Assert.True(profile.RootElement.TryGetProperty("surferProfiles", out var surfers));
        Assert.True(surfers.GetArrayLength() > 0, "Должны быть серферы в профиле");

        Assert.True(profile.RootElement.TryGetProperty("wallet", out var wallet));
        Assert.True(wallet.GetArrayLength() > 0, "Должны быть элементы кошелька");
    }
}
