﻿using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

public class Footballer
{
    // Footballer class to store information about each footballer
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
    public int JerseyNumber { get; set; }
    public List<string> Tags { get; set; }
}


public class BayesianNetwork
{
    private List<Footballer> footballers;
    private List<string> tagsList;
    private List<string> chosenTags = new List<string>();
    private string topNationality, topPosition, tag;
    private int medianRating, medianAge, medianValue, medianJerseyNumber, questionCount = 0;
    private Footballer randomFootballer;
    private double probabilityThreshold;
    HashSet<int> correctlyAnsweredQuestions = new HashSet<int>();


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
        SelectRandomFootballer();
        UpdateTagsFromProbableFootballers();

        Console.WriteLine($"Tags: {string.Join(", ", tagsList)}");
        Console.WriteLine($"Number of tags: {tagsList.Count}");

        bool readyToGuess = false;

        foreach (var question in GetOneOffQuestions())
        {
            questionCount++;
            Console.WriteLine($"\n\nNumber of footballers with probability higher than 0: {getHighProbabilityFootballers()}");
            DisplayRandomFootballer();
            Console.WriteLine($"\nQuestion count: {questionCount}");
            //DisplayFirstFootballer();
            Console.WriteLine(question.Value);
            Console.WriteLine("Yes [y] No [n] Probably [p] Probably not [r] I don't know [d]: ");
            string answer = Console.ReadLine().ToLower();
            // Update probabilities based on user responses
            UpdateProbabilities(question.Key, answer);

            var topTwo = footballers.OrderByDescending(f => f.Probability).Take(2).ToList();
            //print out toptwo
            Console.WriteLine($"Top two: {topTwo[0].Name} {topTwo[0].Probability}, {topTwo[1].Name} {topTwo[1].Probability}");
        }

        while (!readyToGuess)
        {
            foreach (var question in GetLoopQuestions())
            {
                questionCount++;
                if (correctlyAnsweredQuestions.Contains(question.Key))
                {
                    // Skip these questions if it has been correctly answered
                    questionCount--;
                    continue;
                }
                Console.WriteLine($"\n\nNumber of footballers with probability higher than 0: {getHighProbabilityFootballers()}");
                DisplayRandomFootballer();
                Console.WriteLine($"\nQuestion count: {questionCount}");
                //DisplayFirstFootballer();
                Console.WriteLine(question.Value);
                Console.WriteLine("Yes [y] No [n] Probably [p] Probably not [r] I don't know [d]: ");
                string answer = Console.ReadLine().ToLower();

                // Update probabilities based on user responses
                UpdateProbabilities(question.Key, answer);

                var topTwo = footballers.OrderByDescending(f => f.Probability).Take(2).ToList();
                //print out toptwo
                Console.WriteLine($"Top two: {topTwo[0].Name} {topTwo[0].Probability}, {topTwo[1].Name} {topTwo[1].Probability}");
                if (topTwo.Count < 2 || topTwo[0].Probability > topTwo[1].Probability + 0.001 || questionCount == 20)
                {
                    if (questionCount == 20)
                    {
                        Console.WriteLine("I've asked 20 questions, I'm ready to make a guess!");
                    }
                    else
                    {
                        Console.WriteLine("I'm ready to make a guess!");
                    }
                    // If there's only one footballer left, or if the footballer with the highest probability
                    // has a probability higher than the second highest, make a guess
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
            Console.WriteLine($"Number of questions asked: {questionCount}");
        }
        else
        {
            Console.WriteLine("I couldn't figure it out :( Thanks for playing!");
            Console.ReadLine();
        }
    }

    private Dictionary<int, string> GetOneOffQuestions()
    {
        return new Dictionary<int, string>()
        {
            {5, "Is your footballer's international reputation greater than or equal to 2?"},
            {6, "Is your footballer right foot dominant?"},
            {10, "Does your footballer have any tags?"}
        };
    }

    private Dictionary<int, string> GetLoopQuestions()
    {
        medianAge = (int)(CalculateMedianAge());
        topNationality = FindTopNationality();
        topPosition = FindTopPosition();
        medianRating = (int)(CalculateMedianRating());
        medianValue = (int)(CalculateMedianValue());
        medianJerseyNumber = (int)(CalculateMedianJerseyNumber());
        UpdateTagsFromProbableFootballers();

        //loop through chosenTags and remove from tagsList
        foreach (var tag in chosenTags)
        {
            tagsList.Remove(tag);
        }
        //output chosenTags
        Console.WriteLine($"Chosen tags: {string.Join(", ", chosenTags)}");
        Console.WriteLine($"Tags: {string.Join(", ", tagsList)}");

        if (tagsList.Count > 0)
        {
            // Choose a random tag from the list
            Random random = new Random();
            int randomTagIndex = random.Next(tagsList.Count);
            Console.WriteLine($"Random tag index: {randomTagIndex}");
            tag = tagsList[randomTagIndex];
            Console.WriteLine($"Tag: {tag}");
            chosenTags.Add(tag);
        }

        return new Dictionary<int, string>()
        {
            {1, $"Is your footballer greater than or equal to {medianAge} years old?"},
            {2, $"Is your footballer's value greater than or equal to {medianValue.ToString("N0")}EUR?"},
            {3, $"Is your player's position {topPosition}?"},
            {4, $"Is your footballer from {topNationality}?"},
            {7, $"Is your footballer's overall rating greater than or equal to {medianRating}?"},
            {8, $"Is your footballer's jersey number greater than or equal to {medianJerseyNumber}?"},
            {9, $"Does your footballer have the tag {tag}?"}
        };
    }


    /* private void UpdateProbabilities(int questionKey, string answer)
    {
        double totalProbability = 0;

        foreach (var footballer in footballers)
        {
            double probability = GetProbability(footballer);

            // Adjust probabilities based on user responses
            if (questionKey == 1 && answer == "yes" && footballer.Age >= medianAge)
            {
                // Adjust probability for being under medianAge
                probability *= 2; // Double the probability (for simplicity)
            }
            else if (questionKey == 1 && answer == "no" && footballer.Age < medianAge)
            {
                // Adjust probability for being older than medianAge
                probability *= 2; // Double the probability (for simplicity)
            }
            else if (questionKey == 2 && answer == "yes" && footballer.Value >= medianValue)
            {
                // Adjust probability for being worth less than 80000000
                probability *= 2;
            }
            else if (questionKey == 2 && answer == "no" && footballer.Value < medianValue)
            {
                // Adjust probability for being worth 80000000 or more
                probability *= 2;
            }
            else if (questionKey == 3 && answer == "yes" && footballer.Position.ToLower() == topPosition.ToLower())
            {
                // Adjust probability for being topPosition
                probability *= 2;
                correctlyAnsweredQuestions.Add(questionKey);
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
            else if (questionKey == 5 && answer == "yes" && footballer.InternationalReputation >= 2)
            {
                // Adjust probability for having international reputation higher than 1
                probability *= 2;
            }
            else if (questionKey == 5 && answer == "no" && footballer.InternationalReputation < 2)
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
            else if (questionKey == 7 && answer == "yes" && footballer.Rating >= medianRating)
            {
                // Adjust probability for having overall rating of medianRating or higher
                probability *= 2;
            }
            else if (questionKey == 7 && answer == "no" && footballer.Rating < medianRating)
            {
                // Adjust probability for having overall rating lower than medianRating
                probability *= 2;
            }
            else if (questionKey == 8 && answer == "yes" && footballer.JerseyNumber >= medianJerseyNumber)
            {
                // Adjust probability for having overall rating of medianRating or higher
                probability *= 2;
            }
            else if (questionKey == 8 && answer == "no" && footballer.JerseyNumber < medianJerseyNumber)
            {
                // Adjust probability for having overall rating lower than medianRating
                probability *= 2;
            }
            else if (questionKey == 9 && answer == "yes" && footballer.Tags.Contains(tag))
            {
                probability *= 2;

            }
            else if (questionKey == 9 && answer == "no" && !footballer.Tags.Contains(tag))
            {
                probability *= 2;
            }
            else if (questionKey == 10 && answer == "yes" && footballer.Tags.Count > 0)
            {
                probability *= 2;
            }
            else if (questionKey == 10 && answer == "no" && footballer.Tags.Count == 0)
            {
                probability *= 2;
                correctlyAnsweredQuestions.Add(9);
            }
            else
            {
                // If the footballer doesn't fit the criteria for any question, set their probability to 0
                probability = 0;
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
    } */

    private void UpdateProbabilities(int questionKey, string answer)
    {
        double totalProbability = 0;

        var conditions = new List<(Func<Footballer, bool> condition, double factor, bool correctAnswer)>
        {
            (f => questionKey == 1 && answer == "y" && f.Age >= medianAge, 2, false),
            (f => questionKey == 1 && answer == "n" && f.Age < medianAge, 2, false),
            (f => questionKey == 1 && answer == "p" && f.Age >= medianAge, 1.5, false),
            (f => questionKey == 1 && answer == "r" && f.Age < medianAge, 0.5, false),
            (f => questionKey == 1 && answer == "d", 1, false),
            (f => questionKey == 2 && answer == "y" && f.Value >= medianValue, 2, false),
            (f => questionKey == 2 && answer == "n" && f.Value < medianValue, 2, false),
            (f => questionKey == 2 && answer == "p" && f.Value >= medianValue, 1.5, false),
            (f => questionKey == 2 && answer == "r" && f.Value < medianValue, 0.5, false),
            (f => questionKey == 2 && answer == "d", 1, false),
            (f => questionKey == 3 && answer == "y" && f.Position.ToLower() == topPosition.ToLower(), 2, true),
            (f => questionKey == 3 && answer == "n" && f.Position.ToLower() != topPosition.ToLower(), 2, false),
            (f => questionKey == 3 && answer == "p" && f.Position.ToLower() == topPosition.ToLower(), 1.5, false),
            (f => questionKey == 3 && answer == "r" && f.Position.ToLower() != topPosition.ToLower(), 0.5, false),
            (f => questionKey == 3 && answer == "d", 1, false),
            (f => questionKey == 4 && answer == "y" && f.Nationality.ToLower() == topNationality.ToLower(), 2, false),
            (f => questionKey == 4 && answer == "n" && f.Nationality.ToLower() != topNationality.ToLower(), 2, false),
            (f => questionKey == 4 && answer == "p" && f.Nationality.ToLower() == topNationality.ToLower(), 1.5, false),
            (f => questionKey == 4 && answer == "r" && f.Nationality.ToLower() != topNationality.ToLower(), 0.5, false),
            (f => questionKey == 4 && answer == "d", 1, false),
            (f => questionKey == 5 && answer == "y" && f.InternationalReputation >= 2, 2, false),
            (f => questionKey == 5 && answer == "n" && f.InternationalReputation < 2, 2, false),
            (f => questionKey == 5 && answer == "p" && f.InternationalReputation >= 2, 1.5, false),
            (f => questionKey == 5 && answer == "r" && f.InternationalReputation < 2, 0.5, false),
            (f => questionKey == 5 && answer == "d", 1, false),
            (f => questionKey == 6 && answer == "y" && f.Foot.ToLower() == "right", 2, false),
            (f => questionKey == 6 && answer == "n" && f.Foot.ToLower() == "left", 2, false),
            (f => questionKey == 6 && answer == "p" && f.Foot.ToLower() == "right", 1.5, false),
            (f => questionKey == 6 && answer == "r" && f.Foot.ToLower() == "left", 0.5, false),
            (f => questionKey == 6 && answer == "d", 1, false),
            (f => questionKey == 7 && answer == "y" && f.Rating >= medianRating, 2, false),
            (f => questionKey == 7 && answer == "n" && f.Rating < medianRating, 2, false),
            (f => questionKey == 7 && answer == "p" && f.Rating >= medianRating, 1.5, false),
            (f => questionKey == 7 && answer == "r" && f.Rating < medianRating, 0.5, false),
            (f => questionKey == 7 && answer == "d", 1, false),
            (f => questionKey == 8 && answer == "y" && f.JerseyNumber >= medianJerseyNumber, 2, false),
            (f => questionKey == 8 && answer == "n" && f.JerseyNumber < medianJerseyNumber, 2, false),
            (f => questionKey == 8 && answer == "p" && f.JerseyNumber >= medianJerseyNumber, 1.5, false),
            (f => questionKey == 8 && answer == "r" && f.JerseyNumber < medianJerseyNumber, 0.5, false),
            (f => questionKey == 8 && answer == "d", 1, false),
            (f => questionKey == 9 && answer == "y" && f.Tags.Contains(tag), 2, false),
            (f => questionKey == 9 && answer == "n" && !f.Tags.Contains(tag), 2, false),
            (f => questionKey == 9 && answer == "p" && f.Tags.Contains(tag), 1.5, false),
            (f => questionKey == 9 && answer == "r" && !f.Tags.Contains(tag), 0.5, false),
            (f => questionKey == 9 && answer == "d", 1, false),
            (f => questionKey == 10 && answer == "y" && f.Tags.Count > 0, 2, false),
            (f => questionKey == 10 && answer == "n" && f.Tags.Count == 0, 2, true),
            (f => questionKey == 10 && answer == "p" && f.Tags.Count > 0, 1.5, false),
            (f => questionKey == 10 && answer == "r" && f.Tags.Count == 0, 0.5, false),
            (f => questionKey == 10 && answer == "d", 1, false)
        };

        foreach (var footballer in footballers)
        {
            double probability = GetProbability(footballer);
            bool conditionMet = false;

            foreach (var (condition, factor, correctAnswer) in conditions)
            {
                if (condition(footballer))
                {
                    probability *= factor;
                    conditionMet = true;
                    if (correctAnswer)
                    {
                        if (questionKey == 10 && answer == "n" && footballer.Tags.Count == 0)
                        {
                            correctlyAnsweredQuestions.Add(9);
                        }
                        else
                        {
                            correctlyAnsweredQuestions.Add(questionKey);
                        }
                    }
                    break;
                }
            }

            if (!conditionMet)
            {
                probability = 0;
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
        // Order by probability first, then by value
        return footballers
            .OrderByDescending(f => f.Probability)
            .ThenByDescending(f => f.Value)
            .First();
    }

    private double CalculateMedianAge()
    {
        var ages = footballers
            .Where(f => f.Probability > 0)
            .Select(f => (double)f.Age)
            .ToList();
        return CalculateMedian(ages);
    }
    private double CalculateMedian(List<double> numbers)
    {
        var sortedNumbers = numbers.OrderBy(n => n).ToList();

        if (!sortedNumbers.Any())
        {
            throw new InvalidOperationException("No numbers provided.");
        }

        double median;
        int middleIndex = sortedNumbers.Count / 2;
        if (sortedNumbers.Count % 2 == 0)
        {
            // If there is an even number of observations, the median is the average of the two middle numbers
            median = (sortedNumbers[middleIndex - 1] + sortedNumbers[middleIndex]) / 2.0;
        }
        else
        {
            // If there is an odd number of observations, the median is the middle number
            median = sortedNumbers[middleIndex];
        }

        return median;
    }

    private string FindTopNationality()
    {
        return footballers
            .Where(f => f.Probability > 0)
            .GroupBy(f => f.Nationality)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    private string FindTopPosition()
    {
        return footballers
            .Where(f => f.Probability > 0)
            .GroupBy(f => f.Position)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;
    }

    private double CalculateMedianRating()
    {
        var ratings = footballers
            .Where(f => f.Probability > 0)
            .Select(f => (double)f.Rating)
            .ToList();
        return CalculateMedian(ratings);
    }

    private double CalculateMedianValue()
    {
        var values = footballers
            .Where(f => f.Probability > 0)
            .Select(f => (double)f.Value)
            .ToList();
        return CalculateMedian(values);
    }

    private double CalculateMedianJerseyNumber()
    {
        var jerseyNumbers = footballers
            .Where(f => f.Probability > 0)
            .Select(f => (double)f.JerseyNumber)
            .ToList();
        return CalculateMedian(jerseyNumbers);
    }

    private void UpdateTagsFromProbableFootballers()
    {
        tagsList = footballers
            .Where(f => GetProbability(f) > 0)
            .SelectMany(f => f.Tags)
            .Distinct()
            .ToList();
    }

    private int getHighProbabilityFootballers()
    {
        int count = 0;
        foreach (var footballer in footballers)
        {
            if (footballer.Probability > 0)
            {
                count++;
            }
        }
        return count;
    }

    private void SelectRandomFootballer()
    {
        Random random = new Random();
        int index = random.Next(footballers.Count);
        randomFootballer = footballers[index];
    }

    private void DisplayRandomFootballer()
    {
        Console.WriteLine($"Randomly selected footballer, you can use this for reference or choose your own!");
        Console.WriteLine($"Name: {randomFootballer.Name}");
        Console.WriteLine($"Age: {randomFootballer.Age}");
        Console.WriteLine($"Nationality: {randomFootballer.Nationality}");
        Console.WriteLine($"Club: {randomFootballer.Club}");
        Console.WriteLine($"Value: {randomFootballer.Value.ToString("N0")}");
        Console.WriteLine($"Foot: {randomFootballer.Foot}");
        Console.WriteLine($"International Reputation: {randomFootballer.InternationalReputation}");
        Console.WriteLine($"Position: {randomFootballer.Position}");
        Console.WriteLine($"Rating: {randomFootballer.Rating}");
        Console.WriteLine($"Player probability: {randomFootballer.Probability}");
        if (randomFootballer.JerseyNumber > 0)
        {
            Console.WriteLine($"Jersey Number: {randomFootballer.JerseyNumber}");
        }
        else
        {
            Console.WriteLine($"Jersey Number: Unknown");
        }
        if (randomFootballer.Tags.Count > 0)
        {
            Console.WriteLine($"Tags: {string.Join(", ", randomFootballer.Tags)}");
        }
        else
        {
            Console.WriteLine("No tags");
        }
    }

    private void DisplayFirstFootballer()
    {
        var firstFootballer = footballers[0];

        Console.WriteLine($"First footballer: {firstFootballer.Name}");
        Console.WriteLine($"Age: {firstFootballer.Age}");
        Console.WriteLine($"Nationality: {firstFootballer.Nationality}");
        Console.WriteLine($"Club: {firstFootballer.Club}");
        Console.WriteLine($"Value: {firstFootballer.Value.ToString("N0")}");
        Console.WriteLine($"Foot: {firstFootballer.Foot}");
        Console.WriteLine($"International Reputation: {firstFootballer.InternationalReputation}");
        Console.WriteLine($"Position: {firstFootballer.Position}");
        Console.WriteLine($"Rating: {firstFootballer.Rating}");
        Console.WriteLine($"Player probability: {firstFootballer.Probability}");
        if (firstFootballer.JerseyNumber > 0)
        {
            Console.WriteLine($"Jersey Number: {firstFootballer.JerseyNumber}");
        }
        else
        {
            Console.WriteLine($"Jersey Number: Unknown");
        }
        Console.WriteLine($"Tags: {string.Join(", ", firstFootballer.Tags)}");
    }
}

class Program
{
    static void Main()
    {
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
                        Rating = int.Parse(values[10]),
                        JerseyNumber = int.TryParse(values[25], out int jerseyNumber) ? jerseyNumber : -1,
                        Tags = values[23].Split(',')
                                         .Select(tag => tag.Trim().Replace("#", ""))
                                         .Where(tag => !string.IsNullOrWhiteSpace(tag))
                                         .ToList()
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
