# БОНУС: Диаграмма зависимостей

```mermaid
graph TD
    App["Cipher.App (консоль)"] --> Service["CipherService"]
    Service --> ICipher["ICipher (интерфейс)"]
    Service --> IStorage["ITextStorage (интерфейс)"]
    ICipher -.реализуют.- Caesar["CaesarCipher"]
    ICipher -.реализуют.- Vigenere["VigenereCipher"]
    IStorage -.реализуют.- File["FileTextStorage"]
    IStorage -.реализуют.- Memory["MemoryTextStorage"]
    Tests["Cipher.Tests (xUnit)"] --> Service
    Tests --> Caesar
    Tests --> Vigenere
    Tests --> Memory
```

- `Cipher.App` знает только про `CipherService` и конкретные классы
  в одном месте — при создании (пункт «сборка»).
- `CipherService` зависит только от интерфейсов: добавить новый шифр —
  написать класс и выбрать его при старте.
- `MemoryTextStorage` используется в тестах, `FileTextStorage` — в приложении.
