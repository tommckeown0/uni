using System;

namespace AmmoRule
{
    class Ammofuzzy
    {
        public static double low, high;
        public static int rockets, reload;

        static void Main(string[] args)
        {
            // Define initial ammo level and initial reload quantity
            rockets = 23;
            reload = 5;

            int rocketsfired;

            System.Random random = new System.Random();

            while (rockets > 0)
            {
                Console.WriteLine("Rockets remaining: " + rockets);
                Console.WriteLine("Reloading with: " + reload);
                rockets += reload;

                rocketsfired = random.Next(5, 35);
                if (rocketsfired > rockets)
                    rocketsfired = rockets;
                rockets -= rocketsfired;
                Console.WriteLine("Rockets fired: " + rocketsfired);

                // reset double values
                low = 0.0;
                high = 0.0;

                rules();

                Console.WriteLine(low + high);
                // defuzzification
                // set a default value if low + high == 0 to avoid errors in that case
                if (low + high == 0)
                {
                    reload = 5;
                }
                else
                {
                    // round the value from double to int
                    reload = (int)(((low * 25.0) + (high * 5.0)) / (low + high));
                }

                System.Threading.Thread.Sleep(1000);
            }

            Console.WriteLine("Out of ammo!!");
            Console.ReadKey();
        }

        public static void rules()
        {
            if (rockets < 50) //Rule 1
            {
                low = 1 - (rockets / 50.0);
            }

            if (rockets > 10) // Rule 2
            {
                high = (rockets - 10.0 / 30.0);
            }
        }
    }
}
