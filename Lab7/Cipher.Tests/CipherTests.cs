using Cipher.Core;

namespace Cipher.Tests;

// Тесты шифраторов и сервиса. Имена: Что_Условия_Ожидание.
public class CipherTests
{
    // ---------- Цезарь ----------

    [Theory]
    [InlineData("абв", 1, "бвг")]
    [InlineData("abc", 3, "def")]
    [InlineData("я", 1, "а")]          // конец алфавита заворачивается по кругу
    [InlineData("z", 1, "a")]
    public void Caesar_Encrypt_Letters_ShiftedCorrectly(string input, int shift, string expected)
    {
        var cipher = new CaesarCipher(shift);
        Assert.Equal(expected, cipher.Encrypt(input));
    }

    [Fact]
    public void Caesar_EncryptThenDecrypt_ReturnsOriginal()
    {
        var cipher = new CaesarCipher(5);
        string secret = "Привет, World!";
        Assert.Equal(secret, cipher.Decrypt(cipher.Encrypt(secret)));
    }

    [Fact]
    public void Caesar_Encrypt_NonLetters_StaySame()
    {
        var cipher = new CaesarCipher(3);
        Assert.Equal("123, ! ?", cipher.Encrypt("123, ! ?"));
    }

    [Fact]
    public void Caesar_Encrypt_UpperCase_StaysUpper()
    {
        var cipher = new CaesarCipher(1);
        Assert.Equal("БВГ Bcd", cipher.Encrypt("АБВ Abc"));
    }

    // ---------- Виженер ----------

    [Fact]
    public void Vigenere_Encrypt_KeyWord_ShiftPerLetter()
    {
        // ключ "бв" -> сдвиги 1 и 2: а+1=б, б+2=г, в+1=г
        var cipher = new VigenereCipher("бв");
        Assert.Equal("бгг", cipher.Encrypt("абв"));
    }

    [Fact]
    public void Vigenere_EncryptThenDecrypt_ReturnsOriginal()
    {
        var cipher = new VigenereCipher("ключ");
        string secret = "Секретное сообщение!";
        Assert.Equal(secret, cipher.Decrypt(cipher.Encrypt(secret)));
    }

    [Fact]
    public void Vigenere_Encrypt_SpacesDontSpendKey()
    {
        // пробел не буква — ключ не должен тратиться на него
        var cipher = new VigenereCipher("б");
        Assert.Equal("б ч", cipher.Encrypt("а ц"));
    }

    [Fact]
    public void Vigenere_EmptyKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => new VigenereCipher(""));
    }

    [Fact]
    public void Vigenere_KeyWithDigits_Throws()
    {
        Assert.Throws<ArgumentException>(() => new VigenereCipher("кл2ч"));
    }

    // ---------- Сервис и хранилища ----------

    [Fact]
    public void Service_EncryptFile_MemoryStorage_FileEncrypted()
    {
        var storage = new MemoryTextStorage();
        storage.Write("in.txt", "абв");

        var service = new CipherService(new CaesarCipher(1), storage);
        service.EncryptFile("in.txt", "out.txt");

        Assert.Equal("бвг", storage.Read("out.txt"));
    }

    [Fact]
    public void Service_EncryptFileThenDecrypt_ReturnsOriginal()
    {
        var storage = new MemoryTextStorage();
        storage.Write("plain.txt", "Привет, мир!");

        var service = new CipherService(new VigenereCipher("кот"), storage);
        service.EncryptFile("plain.txt", "enc.txt");
        service.DecryptFile("enc.txt", "dec.txt");

        Assert.Equal("Привет, мир!", storage.Read("dec.txt"));
    }

    [Fact]
    public void MemoryStorage_ReadMissingFile_Throws()
    {
        var storage = new MemoryTextStorage();
        Assert.Throws<FileNotFoundException>(() => storage.Read("no.txt"));
    }
}
