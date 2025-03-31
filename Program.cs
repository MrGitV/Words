class Program
{
    // Game configuration constants
    private static class GameSettings
    {
        public const int DefaultTimeLeft = 15;
        public const int TimerInterval = 1000;
        public const int MinWordLength = 8;
        public const int MaxWordLength = 30;
    }

    // Game state variables
    private static string originalWord = null!;
    private static readonly List<string> usedWords = [];
    private static Timer timer = null!;
    private static bool timeIsUp;
    private static int timeLeft = GameSettings.DefaultTimeLeft;
    private static int currentPlayer = 1;
    private static string language = null!;
    private static string playAgain = null!;
    internal static readonly string[] sourceArray = ["да", "нет", "yes", "no"];

    static void Main()
    {
        SelectLanguage();
        StartGame();
    }

    // Handles language selection process
    private static void SelectLanguage()
    {
        do
        {
            Console.WriteLine("Выберите язык / Choose language (ru/en):");
            language = Console.ReadLine()?.ToLower() ?? "";
        } while (language != "ru" && language != "en");
    }

    // Initializes game state and starts main game loop
    private static void StartGame()
    {
        InitializeGame();
        SetupTimer();
        GameLoop();
        HandleGameEnd();
    }

    // Sets up initial game state and prompts for original word
    private static void InitializeGame()
    {
        originalWord = GetValidOriginalWord(language);
        ResetGameState();
    }

    // Prompts user for valid original word until correct input is received
    private static string GetValidOriginalWord(string language)
    {
        string? word;
        do
        {
            Console.WriteLine(LocalizationManager.GetMessage("EnterOriginalWord", language));
            word = Console.ReadLine()?.ToLower();
        } while (!IsOriginalWordValid(word));

        return word!;
    }

    // Validates original word against length and character requirements
    private static bool IsOriginalWordValid(string? word) =>
        !string.IsNullOrEmpty(word) &&
        word.Length >= GameSettings.MinWordLength &&
        word.Length <= GameSettings.MaxWordLength &&
        word.All(char.IsLetter);

    // Resets game state variables to initial values
    private static void ResetGameState()
    {
        usedWords.Clear();
        usedWords.Add(originalWord);
        timeIsUp = false;
        timeLeft = GameSettings.DefaultTimeLeft;
        currentPlayer = 1;
    }

    // Configures and starts the game timer
    private static void SetupTimer()
    {
        timer = new Timer(_ =>
        {
            if (--timeLeft <= 0)
            {
                timeIsUp = true;
                timer?.Dispose();
            }
        }, null, GameSettings.TimerInterval, GameSettings.TimerInterval);
    }

    // Main game loop handling player input and game logic
    private static void GameLoop()
    {
        while (!timeIsUp)
        {
            PromptCurrentPlayer();
            var input = Console.ReadLine()?.ToLower();

            if (timeIsUp) break;

            if (IsInputValid(input))
            {
                ProcessValidInput(input!);
            }
        }
    }

    // Displays current player prompt with time remaining
    private static void PromptCurrentPlayer()
    {
        Console.WriteLine(LocalizationManager.GetMessage("PlayerPrompt", language)
            .Replace("{player}", currentPlayer.ToString())
            .Replace("{time}", timeLeft.ToString()));
    }

    // Validates player input against game rules
    private static bool IsInputValid(string? input) =>
        !string.IsNullOrEmpty(input) &&
        IsWordValid(input) &&
        !usedWords.Contains(input);

    // Processes valid player input and updates game state
    private static void ProcessValidInput(string input)
    {
        usedWords.Add(input);
        currentPlayer = 3 - currentPlayer;
        timeLeft = GameSettings.DefaultTimeLeft;
    }

    // Handles game end sequence and restart logic
    private static void HandleGameEnd()
    {
        timer?.Dispose();
        DisplayGameResult();
        HandleRestartPrompt();
    }

    // Displays game over message with losing player
    private static void DisplayGameResult()
    {
        Console.WriteLine(LocalizationManager.GetMessage("TimeUp", language)
            .Replace("{player}", currentPlayer.ToString()));
    }

    // Handles restart prompt and either restarts game or exits
    private static void HandleRestartPrompt()
    {
        do
        {
            Console.WriteLine(LocalizationManager.GetMessage("PlayAgain", language));
            playAgain = Console.ReadLine()?.ToLower() ?? "";
        } while (!IsValidRestartResponse(playAgain));

        if (playAgain == "да" || playAgain == "yes")
        {
            StartGame();
        }
    }

    // Validates restart prompt response
    private static bool IsValidRestartResponse(string response) => sourceArray.Contains(response);

    // Validates if player's word can be formed from original word
    private static bool IsWordValid(string? word) =>
        !string.IsNullOrEmpty(word) &&
        word.GroupBy(c => c).All(g =>
            originalWord.Count(c => c == g.Key) >= g.Count());
}

// Provides localized messages for different game components
static class LocalizationManager
{
    private static readonly Dictionary<string, Dictionary<string, string>> _translations = new()
    {
        {
            "ru", new Dictionary<string, string>
            {
                {"EnterOriginalWord", "Введите исходное слово (8-30 символов):"},
                {"PlayerPrompt", "Игрок {player}, введите слово (осталось {time} секунд):"},
                {"InvalidWord", "Неверное слово. Повторите попытку."},
                {"TimeUp", "Время вышло! Игрок {player} проиграл."},
                {"PlayAgain", "Хотите сыграть еще раз? (да/нет)"}
            }
        },
        {
            "en", new Dictionary<string, string>
            {
                {"EnterOriginalWord", "Enter the original word (8-30 characters):"},
                {"PlayerPrompt", "Player {player}, enter a word ({time} seconds left):"},
                {"InvalidWord", "Invalid word. Try again."},
                {"TimeUp", "Time's up! Player {player} loses."},
                {"PlayAgain", "Do you want to play again? (yes/no)"}
            }
        }
    };

    // Retrieves localized message for specified key and language
    public static string GetMessage(string key, string language)
    {
        if (_translations.TryGetValue(language, out var languageDict) &&
            languageDict.TryGetValue(key, out var message))
        {
            return message;
        }
        return string.Empty;
    }
}