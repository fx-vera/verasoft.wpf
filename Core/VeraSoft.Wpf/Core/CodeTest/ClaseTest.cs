using System;
using System.Linq;
//extern alias MyAlias;

namespace MediaManager
{
    public class ProgramLauncher : IDisposable
    {
        const int dimelo = 3;
        int misterio(int uno, int dos)
        {
            int calculo = 1;
            for (int x = 0; x < dos; x++)
            {
                calculo = calculo * uno;
            }
            return calculo;
        }


        public ProgramLauncher()
        {
            var res = misterio(4, 3);

            int x = 10;
            int y = 0;
            while (y < x)
            {
                y += x;
            }
            Console.WriteLine(y);

            //int value = 4;

            //switch (value % 2)
            //{
            //    case 0:
            //        //Console.WriteLine("Par");
            //    case 1:
            //        //Console.WriteLine("Impar");
            //    default: return;
            //        //Console.WriteLine("Impar");
            //        //break;
            //}






            //int[] arr = new int[5];
            //int[] arr2 = new int[5] { 1, 2, 3, 4, 5 };
            //int[] arr3 = new int[] { 1, 2, 3, 4, 5 };
            //int[] arr4 = { 1, 2, 3, 4, 5 };
            //int[][,] ar1 = new int[5][,];
            //int[,] ar2 = new int[5, 1];

            //HashSet<int> hs = new HashSet<int>()
            //{
            //    1,1,2,3,3 //1,2,3
            //};
            //int?[] arr = new int?[5];

            //Console.WriteLine(Convert.ToInt32('A')); //65
            //Console.WriteLine("hola");

            int steps = 10;

            //string path = "UDDDUDUU";
            string path = "DUDUUDUDD";

            int result = Result2.countingValleys(steps, path);

            Console.WriteLine(result);
        }

        //class Person<T> where T: class
        //{
        //    public int Add(int value)
        //    {
        //        return this.BaseValue + value;

        //    }
        //    public T BaseValue { get; set; }
        //}


        public static void Main()
        {
            //Test();//error

        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }


    class Result2
    {

        /*
         * Complete the 'countingValleys' function below.
         *
         * The function is expected to return an INTEGER.
         * The function accepts following parameters:
         *  1. INTEGER steps
         *  2. STRING path
         */

        public static int countingValleys(int steps, string path)
        {
            if (steps < 2) return 0;
            var res = path.ToCharArray().ToList();
            int valley = 0;
            int sum = 0;
            bool startCounter = false;
            foreach (var step in res)
            {
                sum = step == 'U' ? sum + 1 : sum - 1;

                if (sum == -1 && !startCounter)
                {
                    startCounter = true;
                    continue;
                }
                if (startCounter && sum == 0)
                {
                    valley++;
                    startCounter = false;
                }
            }
            return valley;
        }

    }
}