namespace Cipher.Core;

/// <summary>
/// Хранилище в памяти — «файлы» живут в словаре.
/// Удобно для юнит-тестов: не трогаем настоящий диск.
/// </summary>
public class MemoryTextStorage : ITextStorage
{
    private readonly Dictionary<string, string> _files = new();

    public string Read(string path)
    {
        if (!_files.TryGetValue(path, out string? text))
            throw new FileNotFoundException($"Нет файла \"{path}\" в памяти.", path);
        return text;
    }

    public void Write(string path, string text) => _files[path] = text;
}
