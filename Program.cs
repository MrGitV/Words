using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

class Program
{
    // Начальное слово, заданное игроком
    static string? originalWord;

    // Список использованных слов
    static readonly List<string> usedWords = [];

    // Таймер для отсчёта времени хода
    static Timer? timer;

    // Переменные для контроля состояния игры
    static bool timeIsUp;
    static int timeLeft = 15;
    static int currentPlayer = 1;
    static string? language;
    static string? playAgain;

    static void Main()
    {
        // Выбор языка
        do
        {
            Console.WriteLine("Выберите язык / Choose language (ru/en):");
            language = Console.ReadLine()?.ToLower();
        } while (language != "ru" && language != "en");

        StartGame();
    }

    static void StartGame()
    {
        // Запрос начального слова
        do
        {
            Console.WriteLine(language == "ru" ? "Введите исходное слово (8-30 символов):" : "Enter the original word (8-30 characters):");
            originalWord = Console.ReadLine()?.ToLower();
        } while (string.IsNullOrEmpty(originalWord) || originalWord.Length is < 8 or > 30 || !originalWord.All(char.IsLetter));

        // Инициализация переменных
        usedWords.Clear();
        usedWords.Add(originalWord);
        timeIsUp = false;
        timeLeft = 15;
        currentPlayer = 1;

        // Запуск таймера
        timer = new Timer(_ =>
        {
            if (--timeLeft <= 0)
            {
                timeIsUp = true;
                timer?.Dispose();
            }
        }, null, 1000, 1000);

        // Основной цикл игры
        while (!timeIsUp)
        {
            Console.WriteLine(language == "ru"
                ? $"Игрок {currentPlayer}, введите слово (осталось {timeLeft} секунд):"
                : $"Player {currentPlayer}, enter a word ({timeLeft} seconds left):");
            var input = Console.ReadLine()?.ToLower();

            // Проверка, не истекло ли время перед обработкой ввода
            if (timeIsUp)
            {
                break;
            }

            // Проверка введённого слова
            if (string.IsNullOrEmpty(input) || !IsWordValid(input) || usedWords.Contains(input))
            {
                Console.WriteLine(language == "ru"
                    ? "Неверное слово. Повторите попытку."
                    : "Invalid word. Try again.");
                continue;
            }

            usedWords.Add(input);               // Добавление слова в список использованных
            currentPlayer = 3 - currentPlayer;  // Смена игрока
            timeLeft = 15;                      // Сброс времени
        }

        // Окончание игры
        Console.WriteLine(language == "ru"
            ? $"Время вышло! Игрок {currentPlayer} проиграл."
            : $"Time's up! Player {currentPlayer} loses.");
        timer?.Dispose();

        // Запрос на повтор игры
        do
        {
            Console.WriteLine(language == "ru" ? "Хотите сыграть еще раз? (да/нет)" : "Do you want to play again? (yes/no)");
            playAgain = Console.ReadLine()?.ToLower();
            if (playAgain == "да" || playAgain == "yes")
            {
                StartGame();
            }
        } while (playAgain != "да" && playAgain != "нет" && playAgain != "yes" && playAgain != "no");
    }

        // Проверка корректности слова
        static bool IsWordValid(string word) =>
        word.GroupBy(c => c).All(g => originalWord!.Count(c => c == g.Key) >= g.Count());
}
