namespace Cipher.Core;

/// <summary>
/// Шифр Виженера: как Цезарь, но сдвиг меняется по ключевому слову.
/// Каждая буква ключа задаёт сдвиг для очередной буквы текста.
/// </summary>
public class VigenereCipher : ICipher
{
    private readonly string _key;

    public string Name => "Виженер";

    public VigenereCipher(string key)
    {
        if (string.IsNullOrWhiteSpace(key) || key.Any(c => !Alphabet.IsLetter(c)))
            throw new ArgumentException("Ключ должен состоять из букв.", nameof(key));
        _key = key;
    }

    public string Encrypt(string text) => Apply(text, decrypt: false);

    public string Decrypt(string text) => Apply(text, decrypt: true);

    // идём по тексту; для каждой буквы берём сдвиг из ключа
    private string Apply(string text, bool decrypt)
    {
        char[] result = text.ToCharArray();
        int keyPos = 0;
        for (int i = 0; i < result.Length; i++)
        {
            if (!Alphabet.IsLetter(result[i]))
                continue; // сдвигаем только буквы — ключ не тратится

            int delta = Alphabet.Position(_key[keyPos]);
            if (decrypt)
                delta = -delta;
            result[i] = Alphabet.ShiftChar(result[i], delta);
            keyPos = (keyPos + 1) % _key.Length;
        }
        return new string(result);
    }
}
