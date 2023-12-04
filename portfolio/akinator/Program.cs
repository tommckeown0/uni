using System;
using System.Collections.Generic;
using System.Linq;

public class Footballer
{
    public string Name { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public string Club { get; set; }
    public decimal Value { get; set; }
    public string Foot { get; set; }
    public int InternationalReputation { get; set; }
    public double Probability { get; set; } // Ensure this property is present
}


public class BayesianNetwork
{
    private List<Footballer> footballers;

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

        foreach (var question in GetQuestions())
        {
            Console.WriteLine(question.Value);
            string answer = Console.ReadLine().ToLower();

            // Update probabilities based on user responses
            UpdateProbabilities(question.Key, answer);
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
        return new Dictionary<int, string>()
        {
            {1, "Is your footballer under 30 years old?"},
            {2, "Is your footballer worth less than 80000000EUR?"},
            {3, "Is your footballer Argentinian?"},
            {4, "Is your footballer Brazilian?"},
            {5, "Is your footballer's international reputation higher than 1?"},
            {6, "Is your footballer right foot dominant?"}
        };
    }

    private void UpdateProbabilities(int questionKey, string answer)
    {
        double totalProbability = 0;

        foreach (var footballer in footballers)
        {
            double probability = GetProbability(footballer);

            // Adjust probabilities based on user responses
            if (questionKey == 1 && answer == "yes" && footballer.Age < 30)
            {
                // Adjust probability for being under 30
                probability *= 2; // Double the probability (for simplicity)
            }
            else if (questionKey == 1 && answer == "no" && footballer.Age >= 30)
            {
                // Adjust probability for being 30 or older
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
            else if (questionKey == 3 && answer == "yes" && footballer.Nationality.ToLower() == "argentinian")
            {
                // Adjust probability for being Argentinian
                probability *= 2;
            }
            else if (questionKey == 3 && answer == "no" && footballer.Nationality.ToLower() != "argentinian")
            {
                // Adjust probability for not being Argentinian
                probability *= 2;
            }
            else if (questionKey == 4 && answer == "yes" && footballer.Nationality.ToLower() == "brazilian")
            {
                // Adjust probability for being Brazilian
                probability *= 2;
            }
            else if (questionKey == 4 && answer == "no" && footballer.Nationality.ToLower() != "brazilian")
            {
                // Adjust probability for not being Brazilian
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
    }

    private Footballer MakeGuess()
    {
        // Return the footballer with the highest probability
        return footballers.OrderByDescending(f => f.Probability).First();
    }
}

class Program
{
    static void Main()
    {
        List<Footballer> footballers = new List<Footballer>
        {
            new Footballer { Name = "L. Messi", Age = 32, Nationality = "Argentina", Club = "FC Barcelona", Value = 95500000, Foot = "Left", InternationalReputation = 5 },
            new Footballer { Name = "Cristiano Ronaldo", Age = 34, Nationality = "Portugal", Club = "Juventus", Value = 58500000, Foot = "Right", InternationalReputation = 5 },
            new Footballer { Name = "Neymar Jr", Age = 27, Nationality = "Brazil", Club = "Paris Saint-Germain", Value = 105500000, Foot = "Right", InternationalReputation = 5 },
            new Footballer { Name = "J. Oblak", Age = 26, Nationality = "Slovenia", Club = "Atlético Madrid", Value = 77500000, Foot = "Right", InternationalReputation = 3 },
            new Footballer { Name = "E. Hazard", Age = 28, Nationality = "Belgium", Club = "Real Madrid", Value = 90000000, Foot = "Right", InternationalReputation = 4 },
            new Footballer { Name = "K. De Bruyne", Age = 28, Nationality = "Belgium", Club = "Manchester City", Value = 90000000, Foot = "Right", InternationalReputation = 4 },
            new Footballer { Name = "M. ter Stegen", Age = 27, Nationality = "Germany", Club = "FC Barcelona", Value = 67500000, Foot = "Right", InternationalReputation = 3 },
            new Footballer { Name = "V. van Dijk", Age = 27, Nationality = "Netherlands", Club = "Liverpool", Value = 78000000, Foot = "Right", InternationalReputation = 3 },
            new Footballer { Name = "L. Modric", Age = 33, Nationality = "Croatia", Club = "Real Madrid", Value = 45000000, Foot = "Right", InternationalReputation = 4 },
            new Footballer { Name = "M. Salah", Age = 27, Nationality = "Egypt", Club = "Liverpool", Value = 80500000, Foot = "Left", InternationalReputation = 3 },
        };

        BayesianNetwork akinator = new BayesianNetwork(footballers);
        akinator.PlayGame();
    }
}
