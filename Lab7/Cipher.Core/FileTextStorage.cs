namespace Cipher.Core;

/// <summary>Читает и пишет обычные текстовые файлы (UTF-8).</summary>
public class FileTextStorage : ITextStorage
{
    public string Read(string path) => File.ReadAllText(path);

    public void Write(string path, string text) => File.WriteAllText(path, text);
}
