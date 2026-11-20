namespace Cipher.Core;

/// <summary>
/// Контракт «источника текста»: откуда читать и куда писать.
/// Вторая реализация интерфейса — FileTextStorage и MemoryTextStorage.
/// </summary>
public interface ITextStorage
{
    string Read(string path);
    void Write(string path, string text);
}
