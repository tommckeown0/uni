using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualBasic.FileIO;

public class Player
{
    /* public string Name { get; set; }
    public decimal Cost { get; set; }
    public int Speed { get; set; }
    public int Age { get; set; }
    // Add other attributes as needed */
    public int FifaId { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Nationality { get; set; }
    public string Club { get; set; }
    public int Value { get; set; }
    public string Foot { get; set; }
    public int IntlRep { get; set; }
    public string Position { get; set; }
}

class Program
{
    static void Main()
    {
        var questions = new Dictionary<int, string>(){
            {1, "Is your footballer under 30 years old?"},
            {2, "Is your footballer worth less than 800000EUR?"},
            {3, "Is your footballer Argentinian?"},
            {4, "Is your footballer Brazilian?"},
            {5, "Is your footballer's international reputation higher than 1?"},
            {6, "Is your footballer right foot domainant?"}
        };

        List<Player> players = LoadPlayersFromCSV("footballers.csv");

        for (int i = 0; i < 10; i++)
        {
            Console.WriteLine(players[i].Name);
            Console.WriteLine(players[i].Age);
            Console.WriteLine(players[i].Nationality);
            Console.WriteLine(players[i].Club);
            Console.WriteLine(players[i].Value);
            Console.WriteLine(players[i].Foot);
            Console.WriteLine(players[i].IntlRep);
            Console.WriteLine(players[i].Position);
        }
    }

    static List<Player> LoadPlayersFromCSV(string path)
    {
        List<Player> players = new List<Player>();

        using (TextFieldParser parser = new TextFieldParser(path))
        {
            parser.Delimiters = new string[] { "," };
            parser.HasFieldsEnclosedInQuotes = true;

            // Skip header
            parser.ReadLine();

            while (!parser.EndOfData)
            {
                string[] values = parser.ReadFields();

                Player player = new Player
                {
                    FifaId = int.Parse(values[0]),
                    Name = values[2],
                    Age = int.Parse(values[4]),
                    Nationality = values[8],
                    Club = values[9],
                    Value = int.Parse(values[12]),
                    Foot = values[15],
                    IntlRep = int.Parse(values[16]),
                    Position = values[24]
                };

                players.Add(player);
            }
        }
        return players;
    }
}
