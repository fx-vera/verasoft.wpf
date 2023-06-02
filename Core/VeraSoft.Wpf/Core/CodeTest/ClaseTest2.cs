using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace Test
{

    public class Result2
    {
        public Result2()
        {
            int n = 5;
            int m = 3;

            List<List<int>> queries = new List<List<int>>();

            queries.Add(new List<int>() { 1, 2, 100 });
            queries.Add(new List<int>() { 2, 5, 100 });
            queries.Add(new List<int>() { 3, 4, 100 });

            long result = Result2.arrayManipulation(n, queries);

            Console.WriteLine(result);
        }

        /*
         * Complete the 'arrayManipulation' function below.
         *
         * The function is expected to return a LONG_INTEGER.
         * The function accepts following parameters:
         *  1. INTEGER n
         *  2. 2D_INTEGER_ARRAY queries
         */

        public static long arrayManipulation(int n, List<List<int>> queries)
        {
            if (!(3 <= n && n <= 100000000))
                return 0;
            if (!(1 <= queries.Count && queries.Count <= 2000000))
                return 0;
            int[,] ar1 = new int[queries.Count + 1, n];
            int max = 0;
            int queryFile = 0;
            foreach (var query in queries)
            {
                queryFile++;
                for (int i = 1; i <= queries.Count; i++)
                {
                    for (int k = 0; k < n; k++)
                    {
                        if (!(1 <= query[0] && query[0] <= query[1] && query[1] <= n))
                            return 0;
                        if (!(0 <= query[2] && (long)query[2] <= 10000000000))
                            return 0;
                        if (k >= (query[0] - 1) && k <= (query[1] - 1) && i >= queryFile)
                        {
                            ar1[i, k] = ar1[i, k] + query[2];
                            if (ar1[i, k] >= max)
                                max = ar1[i, k];
                        }
                    }
                }
            }
            return max;
        }
    }

    class Solution
    {
        public static void Main(string[] args)
        {
            TextWriter textWriter = new StreamWriter(@System.Environment.GetEnvironmentVariable("OUTPUT_PATH"), true);

            string[] firstMultipleInput = Console.ReadLine().TrimEnd().Split(' ');

            int n = Convert.ToInt32(firstMultipleInput[0]);

            int m = Convert.ToInt32(firstMultipleInput[1]);

            List<List<int>> queries = new List<List<int>>();

            for (int i = 0; i < m; i++)
            {
                queries.Add(Console.ReadLine().TrimEnd().Split(' ').ToList().Select(queriesTemp => Convert.ToInt32(queriesTemp)).ToList());
            }

            long result = Result2.arrayManipulation(n, queries);

            textWriter.WriteLine(result);

            textWriter.Flush();
            textWriter.Close();
        }
    }
}
