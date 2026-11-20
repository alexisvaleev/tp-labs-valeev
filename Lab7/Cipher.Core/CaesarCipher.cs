namespace Cipher.Core;

/// <summary>
/// Шифр Цезаря: каждая буква сдвигается в алфавите на N позиций.
/// Работает и с русским, и с латинским алфавитом; регистр сохраняется,
/// цифры и знаки препинания не трогаем.
/// </summary>
public class CaesarCipher : ICipher
{
    private readonly int _shift;

    public string Name => "Цезарь";

    public CaesarCipher(int shift)
    {
        _shift = shift;
    }

    public string Encrypt(string text) => Shift(text, _shift);

    public string Decrypt(string text) => Shift(text, -_shift);

    // сдвигаем каждый символ на delta по его алфавиту
    private static string Shift(string text, int delta)
    {
        char[] result = text.ToCharArray();
        for (int i = 0; i < result.Length; i++)
            result[i] = Alphabet.ShiftChar(result[i], delta);
        return new string(result);
    }
}
