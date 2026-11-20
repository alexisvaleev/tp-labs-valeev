namespace Cipher.Core;

/// <summary>
/// Сервис-«машинка»: шифрует и расшифровывает текст и файлы.
/// Сам ничего не знает ни про конкретный шифр, ни про файлы —
/// обе реализации ему подсовывают через конструктор.
/// </summary>
public class CipherService
{
    private readonly ICipher _cipher;
    private readonly ITextStorage _storage;

    public CipherService(ICipher cipher, ITextStorage storage)
    {
        _cipher = cipher;
        _storage = storage;
    }

    public ICipher Cipher => _cipher;

    public string Encrypt(string text) => _cipher.Encrypt(text);

    public string Decrypt(string text) => _cipher.Decrypt(text);

    // шифруем файл целиком: прочитали -> преобразовали -> записали
    public void EncryptFile(string inputPath, string outputPath)
        => _storage.Write(outputPath, _cipher.Encrypt(_storage.Read(inputPath)));

    public void DecryptFile(string inputPath, string outputPath)
        => _storage.Write(outputPath, _cipher.Decrypt(_storage.Read(inputPath)));
}
