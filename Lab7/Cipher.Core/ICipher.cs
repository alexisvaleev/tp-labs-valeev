namespace Cipher.Core;

/// <summary>
/// Контракт шифра — интерфейс, от которого зависит вся программа.
/// Любой новый шифр просто реализует этот интерфейс.
/// </summary>
public interface ICipher
{
    string Name { get; }
    string Encrypt(string text);
    string Decrypt(string text);
}
