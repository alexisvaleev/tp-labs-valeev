namespace Cipher.Core;

/// <summary>
/// Общие алфавиты и сдвиг буквы. Общий код вынес сюда,
/// чтобы не дублировать в обоих шифрах.
/// </summary>
public static class Alphabet
{
    private const string Ru = "абвгдеёжзийклмнопрстуфхцчшщъыьэюя";
    private const string En = "abcdefghijklmnopqrstuvwxyz";

    public static bool IsLetter(char c)
        => Ru.Contains(char.ToLower(c)) || En.Contains(char.ToLower(c));

    // позиция буквы в её алфавите (для ключа Виженера)
    public static int Position(char c)
    {
        char lower = char.ToLower(c);
        int pos = Ru.IndexOf(lower);
        if (pos >= 0) return pos;
        return En.IndexOf(lower);
    }

    // сдвиг буквы внутри её алфавита с замыканием по кругу
    public static char ShiftChar(char c, int delta)
    {
        char lower = char.ToLower(c);
        string? alphabet = Ru.Contains(lower) ? Ru
                         : En.Contains(lower) ? En : null;
        if (alphabet == null)
            return c; // не буква — не трогаем

        int n = alphabet.Length;
        int pos = alphabet.IndexOf(lower);
        int shifted = ((pos + delta) % n + n) % n; // % бывает отрицательным
        char result = alphabet[shifted];
        return char.IsUpper(c) ? char.ToUpper(result) : result;
    }
}
