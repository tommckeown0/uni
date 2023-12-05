using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

public class Footballer
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public string Club { get; set; }
    public decimal Value { get; set; }
    public string Foot { get; set; }
    public int InternationalReputation { get; set; }
    public double Probability { get; set; }
    public string Position { get; set; }
    public int Rating { get; set; }
}


public class BayesianNetwork
{
    private List<Footballer> footballers;
    private string topNationality, topPosition;
    private int meanRating, meanAge;

    public BayesianNetwork(List<Footballer> footballers)
    {
        this.footballers = footballers;
        InitializeProbabilities();
    }

    private void InitializeProbabilities()
    {
        foreach (var footballer in footballers)
        {
            footballer.Probability = 1.0 / footballers.Count;
        }
    }

    public void PlayGame()
    {
        Console.WriteLine("Welcome to the Footballer Bayesian Network!");
        Console.WriteLine("Think of a footballer, and I'll try to guess who it is.");

        bool readyToGuess = false;
        while (!readyToGuess)
        {
            foreach (var question in GetQuestions())
            {
                Console.WriteLine(question.Value);
                string answer = Console.ReadLine().ToLower();

                // Update probabilities based on user responses
                UpdateProbabilities(question.Key, answer, meanAge, topNationality, topPosition, meanRating);

                var topTwo = footballers.OrderByDescending(f => f.Probability).Take(2).ToList();
                //print out toptwo
                Console.WriteLine($"Top two: {topTwo[0].Name} {topTwo[0].Probability}, {topTwo[1].Name} {topTwo[1].Probability}");
                if (topTwo.Count < 2 || topTwo[0].Probability > topTwo[1].Probability + 0.001)
                {
                    // If there's only one footballer left, or if the footballer with the highest probability
                    // has a probability higher than the second highest, make a guess
                    Console.WriteLine("I'm ready to make a guess!");
                    readyToGuess = true;
                    break;
                }
            }
            //Console.WriteLine("I've exhausted all questions but I'll guess...");
            //readyToGuess = true;
        }

        // Make a guess based on the highest probability
        Footballer guessedFootballer = MakeGuess();
        Console.WriteLine($"Is it {guessedFootballer.Name}? (yes/no)");

        if (Console.ReadLine().ToLower() == "yes")
        {
            Console.WriteLine("I guessed correctly! Thanks for playing!");
        }
        else
        {
            Console.WriteLine("I need to learn more. Who was the footballer?");
            string newFootballerName = Console.ReadLine();
            // Update probabilities with the new information (not implemented in this example)
            Console.WriteLine($"Thanks for teaching me about {newFootballerName}!");
        }
    }

    private Dictionary<int, string> GetQuestions()
    {
        meanAge = (int)(CalculateMeanAge());
        topNationality = FindTopNationality();
        topPosition = FindTopPosition();
        meanRating = (int)(CalculateMeanRating());

        return new Dictionary<int, string>()
        {
            {1, $"Is your footballer under {meanAge} years old?"},
            {2, "Is your footballer worth less than 80000000EUR?"},
            {3, $"Is your player's position {topPosition}?"},
            {4, $"Is your footballer from {topNationality}?"},
            {5, "Is your footballer's international reputation higher than 1?"},
            {6, "Is your footballer right foot dominant?"},
            {7, "Does your footballer have an overall rating of {meanRating} or higher?"}
        };
    }

    private void UpdateProbabilities(int questionKey, string answer, int meanAge, string topNationality, string topPosition, int meanRating)
    {
        double totalProbability = 0;

        foreach (var footballer in footballers)
        {
            double probability = GetProbability(footballer);

            // Adjust probabilities based on user responses
            if (questionKey == 1 && answer == "yes" && footballer.Age < meanAge)
            {
                // Adjust probability for being under meanAge
                probability *= 2; // Double the probability (for simplicity)
            }
            else if (questionKey == 1 && answer == "no" && footballer.Age >= meanAge)
            {
                // Adjust probability for being older than meanAge
                probability *= 2; // Double the probability (for simplicity)
            }
            else if (questionKey == 2 && answer == "yes" && footballer.Value < 80000000)
            {
                // Adjust probability for being worth less than 80000000
                probability *= 2;
            }
            else if (questionKey == 2 && answer == "no" && footballer.Value >= 80000000)
            {
                // Adjust probability for being worth 80000000 or more
                probability *= 2;
            }
            else if (questionKey == 3 && answer == "yes" && footballer.Position.ToLower() == topPosition.ToLower())
            {
                // Adjust probability for being topPosition
                probability *= 2;
            }
            else if (questionKey == 3 && answer == "no" && footballer.Position.ToLower() != topPosition.ToLower())
            {
                // Adjust probability for not being topPosition
                probability *= 2;
            }
            else if (questionKey == 4 && answer == "yes" && footballer.Nationality.ToLower() == topNationality.ToLower())
            {
                // Adjust probability for being nationality
                probability *= 2;
            }
            else if (questionKey == 4 && answer == "no" && footballer.Nationality.ToLower() != topNationality.ToLower())
            {
                // Adjust probability for not being nationality
                probability *= 2;
            }
            else if (questionKey == 5 && answer == "yes" && footballer.InternationalReputation > 1)
            {
                // Adjust probability for having international reputation higher than 1
                probability *= 2;
            }
            else if (questionKey == 5 && answer == "no" && footballer.InternationalReputation <= 1)
            {
                // Adjust probability for having international reputation 1 or lower
                probability *= 2;
            }
            else if (questionKey == 6 && answer == "yes" && footballer.Foot.ToLower() == "right")
            {
                // Adjust probability for having right foot dominant
                probability *= 2;
            }
            else if (questionKey == 6 && answer == "no" && footballer.Foot.ToLower() == "left")
            {
                // Adjust probability for having left foot dominant
                probability *= 2;
            }
            else if (questionKey == 7 && answer == "yes" && footballer.Rating >= meanRating)
            {
                // Adjust probability for having overall rating of meanRating or higher
                probability *= 2;
            }
            else if (questionKey == 7 && answer == "no" && footballer.Rating < meanRating)
            {
                // Adjust probability for having overall rating lower than meanRating
                probability *= 2;
            }

            SetProbability(footballer, probability);
            totalProbability += probability;
        }

        // Normalize probabilities to ensure they sum to 1
        foreach (var footballer in footballers)
        {
            double normalizedProbability = GetProbability(footballer) / totalProbability;
            SetProbability(footballer, normalizedProbability);
        }
    }

    private double GetProbability(Footballer footballer)
    {
        // Return the current probability for the footballer
        return footballer.Probability;
    }

    private void SetProbability(Footballer footballer, double probability)
    {
        // Update the probability for the footballer
        footballer.Probability = probability;
        //show probability
    }

    private Footballer MakeGuess()
    {
        // Return the footballer with the highest probability
        return footballers.OrderByDescending(f => f.Probability).First();
    }

    private double CalculateMeanAge()
    {
        return footballers.Average(f => f.Age);
    }

    private string FindTopNationality()
    {
        return footballers
            .GroupBy(f => f.Nationality)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    private string FindTopPosition()
    {
        return footballers
            .GroupBy(f => f.Position)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    private double CalculateMeanRating()
    {
        return footballers.Average(f => f.Rating);
    }
}

class Program
{
    static void Main()
    {
        /* List<Footballer> footballers = new List<Footballer>
        {
            new Footballer { Name = "L. Messi", Age = 32, Nationality = "Argentina", Club = "FC Barcelona", Value = 95500000, Foot = "Left", InternationalReputation = 5, Position = "RW", Rating = 94 },
            new Footballer { Name = "Cristiano Ronaldo", Age = 34, Nationality = "Portugal", Club = "Juventus", Value = 58500000, Foot = "Right", InternationalReputation = 5, Position = "LW", Rating = 93 },
            new Footballer { Name = "Neymar Jr", Age = 27, Nationality = "Brazil", Club = "Paris Saint-Germain", Value = 105500000, Foot = "Right", InternationalReputation = 5, Position = "CAM", Rating = 92 },
            new Footballer { Name = "J. Oblak", Age = 26, Nationality = "Slovenia", Club = "Atlético Madrid", Value = 77500000, Foot = "Right", InternationalReputation = 3, Position = "GK", Rating = 91 },
            new Footballer { Name = "E. Hazard", Age = 28, Nationality = "Belgium", Club = "Real Madrid", Value = 90000000, Foot = "Right", InternationalReputation = 4, Position = "LW", Rating = 91 },
            new Footballer { Name = "K. De Bruyne", Age = 28, Nationality = "Belgium", Club = "Manchester City", Value = 90000000, Foot = "Right", InternationalReputation = 4, Position = "RCM", Rating = 91 },
            new Footballer { Name = "M. ter Stegen", Age = 27, Nationality = "Germany", Club = "FC Barcelona", Value = 67500000, Foot = "Right", InternationalReputation = 3, Position = "GK", Rating = 90 },
            new Footballer { Name = "V. van Dijk", Age = 27, Nationality = "Netherlands", Club = "Liverpool", Value = 78000000, Foot = "Right", InternationalReputation = 3, Position = "LCB", Rating = 90 },
            new Footballer { Name = "L. Modric", Age = 33, Nationality = "Croatia", Club = "Real Madrid", Value = 45000000, Foot = "Right", InternationalReputation = 4, Position = "RCM", Rating = 90 },
            new Footballer { Name = "M. Salah", Age = 27, Nationality = "Egypt", Club = "Liverpool", Value = 80500000, Foot = "Left", InternationalReputation = 3, Position = "RW", Rating = 90 },
        }; */

        static List<Footballer> LoadFootballersFromCSV(string path)
        {
            List<Footballer> footballers = new List<Footballer>();

            using (TextFieldParser parser = new TextFieldParser(path))
            {
                parser.Delimiters = new string[] { "," };
                parser.HasFieldsEnclosedInQuotes = true;

                // Skip header
                parser.ReadLine();

                while (!parser.EndOfData)
                {
                    string[] values = parser.ReadFields();

                    Footballer footballer = new Footballer
                    {
                        Name = values[2],
                        Age = int.Parse(values[4]),
                        Nationality = values[8],
                        Club = values[9],
                        Value = int.Parse(values[12]),
                        Foot = values[15],
                        InternationalReputation = int.Parse(values[16]),
                        Position = values[24],
                        Rating = int.Parse(values[10])
                    };

                    footballers.Add(footballer);
                }
            }
            return footballers;
        }

        List<Footballer> footballers = LoadFootballersFromCSV("footballers.csv");

        BayesianNetwork akinator = new BayesianNetwork(footballers);
        akinator.PlayGame();
    }
}
