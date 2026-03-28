using System;
using System.Security.Cryptography;
using CityEdit.Core;

namespace CityEdit.Crypto;

/// <summary>
/// Реализация шифрования AES в режиме CTR (Counter Mode).
/// Subway Surfers City использует ручную реализацию CTR через AES-ECB:
/// для каждого 16-байтного блока данных счётчик шифруется через ECB,
/// затем результат XOR'ится с блоком открытого/зашифрованного текста.
/// Счётчик инкрементируется от младшего байта к старшему (позиции 15..8).
/// </summary>
public sealed class AesCtrCipher : IDisposable
{
    /// <summary>
    /// Экземпляр AES в режиме ECB для шифрования счётчика.
    /// </summary>
    private readonly Aes _aes;

    /// <summary>
    /// Создаёт экземпляр AES-CTR шифра с указанным ключом.
    /// </summary>
    /// <param name="key">Ключ шифрования длиной 16 байт (AES-128).</param>
    /// <exception cref="ArgumentException">Если длина ключа не равна 16 байтам.</exception>
    public AesCtrCipher(byte[] key)
    {
        if (key.Length != Constants.KeySize)
        {
            throw new ArgumentException(
                $"Длина ключа должна быть {Constants.KeySize} байт, получено {key.Length}.",
                nameof(key));
        }

        _aes = Aes.Create();
        _aes.Mode = CipherMode.ECB;
        _aes.Padding = PaddingMode.None;
        _aes.Key = key;
    }

    /// <summary>
    /// Преобразует данные с использованием AES-CTR.
    /// Операция симметрична: одна и та же функция используется для шифрования и дешифрования.
    /// </summary>
    /// <param name="iv">Вектор инициализации (начальное значение счётчика), 16 байт.</param>
    /// <param name="data">Входные данные для преобразования.</param>
    /// <returns>Результат XOR операции между данными и зашифрованным счётчиком.</returns>
    /// <exception cref="ArgumentException">Если длина IV не равна 16 байтам.</exception>
    public byte[] Transform(byte[] iv, byte[] data)
    {
        if (iv.Length != Constants.IvSize)
        {
            throw new ArgumentException(
                $"Длина IV должна быть {Constants.IvSize} байт, получено {iv.Length}.",
                nameof(iv));
        }

        var result = new byte[data.Length];
        var counter = new byte[Constants.AesBlockSize];
        Array.Copy(iv, counter, Constants.AesBlockSize);

        using var encryptor = _aes.CreateEncryptor();
        var encryptedCounter = new byte[Constants.AesBlockSize];

        for (int offset = 0; offset < data.Length; offset += Constants.AesBlockSize)
        {
            // Шифруем текущее значение счётчика через AES-ECB
            encryptor.TransformBlock(counter, 0, Constants.AesBlockSize, encryptedCounter, 0);

            // XOR блока данных с зашифрованным счётчиком
            int blockLength = Math.Min(Constants.AesBlockSize, data.Length - offset);
            for (int i = 0; i < blockLength; i++)
            {
                result[offset + i] = (byte)(data[offset + i] ^ encryptedCounter[i]);
            }

            // Инкремент счётчика (байты 15..8, от младшего к старшему)
            IncrementCounter(counter);
        }

        return result;
    }

    /// <summary>
    /// Инкрементирует счётчик CTR.
    /// Инкремент начинается с байта 15 и может распространяться до байта 8 (carry propagation).
    /// Это соответствует формату, используемому в Subway Surfers City.
    /// </summary>
    /// <param name="counter">Массив счётчика длиной 16 байт (модифицируется на месте).</param>
    private static void IncrementCounter(byte[] counter)
    {
        int carry = 1;
        for (int i = 15; i >= 8; i--)
        {
            int sum = counter[i] + carry;
            counter[i] = (byte)(sum & 0xFF);
            carry = sum >> 8;
            if (carry == 0)
            {
                break;
            }
        }
    }

    /// <summary>
    /// Освобождает ресурсы AES.
    /// </summary>
    public void Dispose()
    {
        _aes.Dispose();
    }
}
