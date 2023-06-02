class Result
{

    /*
     * Complete the 'Calculate' function below.
     *
     * The function is expected to return an INTEGER.
     * The function accepts following parameters:
     *  1. INTEGER a
     *  2. INTEGER b
     *  3. STRING operation
     */

    // private static IOperation addOperation; // this and the other operations could be initialized in constructor and passed as parameter
    public Result()
    {
        var res = Calculate(5, 2, "add");
    }

    public static int Calculate(int a, int b, string operation)
    {
        // a better solution is define operations as private parameters, initialize in the constructor or Init method and pass by parameters the interface instead of create a new each time the operation is called. 
        // But I can't modify the Main(string[] args)
        int result = 0;
        if (operation.Equals("multiply"))
        {
            result = Calculate(a, b, new Multiply());
        }
        else if (operation.Equals("divide"))
        {
            result = Calculate(a, b, new Divide());
        }
        else if (operation.Equals("substract"))
        {
            result = Calculate(a, b, new Substract());
        }
        return result;
    }

    public static int Calculate(int a, int b, IOperation operation)
    {
        return operation.Calculate(a, b);
    }
}

public interface IOperation
{
    int Calculate(int a, int b);
}
public interface IAdd : IOperation { }
public interface IMultiply : IOperation { }
public interface IDivide : IOperation { }
public interface ISubstract : IOperation { }

public class Add : IAdd
{
    public int Calculate(int a, int b)
    {
        return a + b;
    }
}

public class Multiply : IMultiply
{
    public int Calculate(int a, int b)
    {
        return a * b;
    }
}

public class Divide : IDivide
{
    public int Calculate(int a, int b)
    {
        return a / b;
    }
}

public class Substract : ISubstract
{
    public int Calculate(int a, int b)
    {
        return a - b;
    }
}