using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

public class User
{
    public string Name { get; set; }
    public int CharactersPerMinute { get; set; }
    public int CharactersPerSecond { get; set; }
}

public class Leaderboard
{
    private const string leaderboardFileName = "leaderboard.json";
    private List<User> leaderboard;

    public Leaderboard()
    {
        leaderboard = LoadLeaderboard();
    }

    public void AddToLeaderboard(User user)
    {
        leaderboard.Add(user);
        leaderboard = leaderboard.OrderByDescending(u => u.CharactersPerMinute).ToList();
        SaveLeaderboard();
    }

    public void DisplayLeaderboard()
    {
        Console.WriteLine("таблица рекордов:");
        Console.WriteLine("Name\tCPM\tCPS");

        foreach (var user in leaderboard)
        {
            Console.WriteLine($"{user.Name}\t{user.CharactersPerMinute}\t{user.CharactersPerSecond}");
        }
    }

    private List<User> LoadLeaderboard()
    {
        if (File.Exists(leaderboardFileName))
        {
            string json = File.ReadAllText(leaderboardFileName);
            return JsonConvert.DeserializeObject<List<User>>(json);
        }
        return new List<User>();
    }

    private void SaveLeaderboard()
    {
        string json = JsonConvert.SerializeObject(leaderboard);
        File.WriteAllText(leaderboardFileName, json);
    }
}

public class TypingTest
{
    private const string sampleText = "Я в своем познании настолько преисполнился, что я как будто бы уже\r\n\r\nсто триллионов миллиардов лет проживаю на триллионах и\r\n\r\nтриллионах таких же планет, как эта Земля, мне этот мир абсолютно\r\n\r\nпонятен, и я здесь ищу только одного - покоя, умиротворения и\r\n\r\nвот этой гармонии, от слияния с бесконечно вечным, от созерцания\r\n\r\nвеликого фрактального подобия и от вот этого замечательного всеединства\r\n\r\nсущества, бесконечно вечного,";

    public void StartTest(string userName)
    {
        Console.WriteLine($"велком, {userName}! давай проверим, как быстро ты печатаешь");

        Console.WriteLine("внимание, шедевр:");
        Console.WriteLine(sampleText);

        Console.WriteLine("нажми Enter чтобы начать");
        Console.ReadLine();

        var stopwatch = Stopwatch.StartNew();
        string userTypedText = Console.ReadLine();
        stopwatch.Stop();

        CalculateResults(userName, userTypedText, stopwatch.Elapsed);
    }

    private void CalculateResults(string userName, string userTypedText, TimeSpan elapsedTime)
    {
        int totalCharacters = sampleText.Length;
        int correctCharacters = CalculateCorrectCharacters(userTypedText);

        double charactersPerMinute = (correctCharacters / elapsedTime.TotalMinutes);
        double charactersPerSecond = (correctCharacters / elapsedTime.TotalSeconds);

        Console.WriteLine($"{userName}, твой результат:");
        Console.WriteLine($"правильные символы: {correctCharacters}");
        Console.WriteLine($"символов за минуту: {charactersPerMinute}");
        Console.WriteLine($"символов за секунду: {charactersPerSecond}");

        new Leaderboard().AddToLeaderboard(new User
        {
            Name = userName,
            CharactersPerMinute = (int)charactersPerMinute,
            CharactersPerSecond = (int)charactersPerSecond
        });

        new Leaderboard().DisplayLeaderboard();
    }

    private int CalculateCorrectCharacters(string userTypedText)
    {
        int correctCharacters = 0;
        int minLength = Math.Min(sampleText.Length, userTypedText.Length);

        for (int i = 0; i < minLength; i++)
        {
            if (sampleText[i] == userTypedText[i])
            {
                correctCharacters++;
            }
        }

        return correctCharacters;
    }
}

public class Program
{
    public static void Main()
    {
        while (true)
        {
            Console.Write("введите погоняло: ");
            string userName = Console.ReadLine();

            new TypingTest().StartTest(userName);

            new Leaderboard().DisplayLeaderboard();

            Console.Write("еще разок? (y/n): ");
            string again = Console.ReadLine();

            if (again.ToLower() != "y")
            {
                break;
            }
        }
    }
}


