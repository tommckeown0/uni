using System;
using System.Text;

class Program
{
    static void Main(string[] args)
    {
        string input = "*101*";
        int cell = input.Length - 1;
        StringBuilder tape = new StringBuilder();
        tape.Append(input);
        string state = "START";
        char read;
        Console.WriteLine(input + " " + state);

        do
        {
            read = tape[cell];
            if (state == "START" && read == '*')
            {
                tape[cell] = '*';
                cell--;
                state = "ADD";
            }

            //ADD
            else if (state == "ADD" && read == '0')
            {
                tape[cell] = '1';
                cell++;
                state = "RETURN";
            }
            else if (state == "ADD" && read == '1')
            {
                tape[cell] = '0';
                cell--;
                state = "CARRY";
            }
            else if (state == "ADD" && read == '*')
            {
                tape[cell] = '*';
                cell++;
                state = "HALT";
            }

            //CARRY
            else if (state == "CARRY" && read == '0')
            {
                tape[cell] = '1';
                cell++;
                state = "RETURN";
            }
            else if (state == "CARRY" && read == '1')
            {
                tape[cell] = '0';
                cell--;
                state = "CARRY";
            }
            else if (state == "CARRY" && read == '*')
            {
                tape[cell] = '1';
                cell--;
                state = "OVERFLOW";
            }

            //OVERFLOW
            else if (state == "OVERFLOW" && read == '*')
            {
                tape[cell] = '*';
                cell++;
                state = "RETURN";
            }

            //RETURN
            else if (state == "RETURN" && read == '0')
            {
                tape[cell] = '0';
                cell++;
                state = "RETURN";
            }
            else if (state == "RETURN" && read == '1')
            {
                tape[cell] = '1';
                cell++;
                state = "RETURN";
            }
            else if (state == "RETURN" && read == '*')
            {
                tape[cell] = '*';
                state = "HALT";
            }
        } while (state != "HALT");

        Console.WriteLine(tape.ToString());
    }
}
