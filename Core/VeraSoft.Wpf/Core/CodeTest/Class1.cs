using System;

namespace Solution
{
    class Solution
    {
        public Solution()
        {
            CalculateAmount(100, 10, EmployeeLocation.Ireland);
        }
        static void Main(string[] args)
        {
            /* Enter your code here. Read input from STDIN. Print output to STDOUT */
            decimal hourlyRate = Convert.ToDecimal(Console.ReadLine());
            double hoursWorked = Convert.ToDouble(Console.ReadLine());
            CalculateAmount(hourlyRate, hoursWorked, EmployeeLocation.Ireland);
        }

        private static void CalculateAmount(decimal hourlyRate, double hoursWorked, EmployeeLocation location)
        {
            Console.WriteLine("Employee location: " + location.ToString());
            decimal result = CalculateNet(CalculateGross(hourlyRate, hoursWorked), location);
        }


        private static decimal CalculateGross(decimal hourlyRate, double hours)
        {
            IGrossAmount grossCalculator = new GrossAmount();
            return grossCalculator.CalculateAmount(hourlyRate, hours);
        }

        private static decimal CalculateNet(decimal amount, EmployeeLocation location)// given gross amount
        {
            Console.WriteLine("Less deductions");
            decimal result = new NetAmount().CalculateAmount(amount, location);

            return result;
        }
    }

    public class GrossAmount : IGrossAmount
    {
        public decimal CalculateAmount(decimal hourlyRate, double hours)
        {
            decimal result = hourlyRate * (decimal)hours;
            Console.WriteLine("Gross Amount: €" + result.ToString("F"));
            return result;
        }
    }
    public interface IGrossAmount
    {
        decimal CalculateAmount(decimal hourlyRate, double hours);
    }
    public interface INetAmount
    {
        decimal CalculateAmount(decimal currentAmount, EmployeeLocation location);
    }

    public class NetAmount : INetAmount
    {
        public decimal CalculateAmount(decimal currentAmount, EmployeeLocation location)
        {
            decimal result = 0;
            switch (location)
            {
                case EmployeeLocation.Germany:
                    result = ApplyGermanyDeductions(currentAmount);
                    break;

                case
                    EmployeeLocation.Ireland:
                    result = ApplyIrelandDeductions(currentAmount);
                    break;

                case
                    EmployeeLocation.Italy:
                    result = ApplyItalyDeductions(currentAmount);
                    break;
            }
            return result;
        }

        private decimal ApplyGermanyDeductions(decimal currentAmount)
        {
            decimal result = 0;
            var deductions = new GermanyDeductions();
            decimal incomeDeduction = deductions.CalculateIncomeTax(currentAmount);
            decimal pensionDeduction = deductions.CalculatePension(currentAmount);
            result = currentAmount - incomeDeduction - pensionDeduction;
            Console.WriteLine("Net Amount: €" + result.ToString("F"));
            return result;
        }

        private decimal ApplyIrelandDeductions(decimal currentAmount)
        {
            decimal result = 0;
            var deductions = new IrelandDeductions();
            decimal incomeDeduction = deductions.CalculateIncomeTax(currentAmount);
            decimal pensionDeduction = deductions.CalculatePension(currentAmount);
            decimal uscDeduction = deductions.CalculatePension(currentAmount);
            result = currentAmount - incomeDeduction - pensionDeduction - uscDeduction;
            Console.WriteLine("Net Amount: €" + result.ToString("F"));
            return result;
        }

        private decimal ApplyItalyDeductions(decimal currentAmount)
        {
            decimal result = 0;
            var deductions = new GermanyDeductions();
            decimal incomeDeduction = deductions.CalculateIncomeTax(currentAmount);
            decimal pensionDeduction = deductions.CalculatePension(currentAmount);
            result = currentAmount - incomeDeduction - pensionDeduction;
            Console.WriteLine("Net Amount: €" + result.ToString("F"));
            return result;
        }
    }

    public interface IDeductionIncomeTax
    {
        decimal CalculateIncomeTax(decimal currentAmount);
    }

    public interface IDeductionUniversalSocialCharge
    {
        decimal CalculateDeductionUniversalSocial(decimal currentAmount);
    }

    public interface IDeductionPension
    {
        decimal CalculatePension(decimal currentAmount);
    }
    public interface IGermanyDeductions : IDeductionIncomeTax, IDeductionPension
    {

    }
    public interface IItalyDeductions : IDeductionIncomeTax, IDeductionPension
    {

    }
    public interface IIrelandDeductions : IDeductionIncomeTax, IDeductionPension, IDeductionUniversalSocialCharge
    {

    }

    public class IrelandDeductions : IIrelandDeductions
    {
        public decimal CalculateIncomeTax(decimal currentAmount)
        {
            decimal result = CalculateExtraTax.Calculate(currentAmount, 600, (decimal)0.25, (decimal)0.40);
            Console.WriteLine("Income tax: €" + result.ToString("F"));
            return result;
        }

        public decimal CalculateDeductionUniversalSocial(decimal currentAmount)
        {
            decimal result = CalculateExtraTax.Calculate(currentAmount, 500, (decimal)0.07, (decimal)0.08);

            Console.WriteLine("Universal Social Charge: €" + result.ToString("F"));
            return result;
        }

        public decimal CalculatePension(decimal currentAmount)
        {
            var result = currentAmount * (decimal)0.049;
            Console.WriteLine("Pension: €" + result.ToString("F"));
            return result;
        }
    }

    public static class CalculateExtraTax
    {
        public static decimal Calculate(decimal currentAmount, decimal minAmount, decimal minTax, decimal otherMax)
        {
            decimal result = 0;
            decimal extra = currentAmount - minAmount;

            if (extra > 0)
                result = minAmount * minTax;
            else
                result = currentAmount * minTax;

            if (extra > 0)
            {
                decimal extraTax = extra * otherMax;
                result = result + extraTax;
            }
            return result;
        }
    }

    public class ItalyDeductions : IItalyDeductions
    {
        public decimal CalculateIncomeTax(decimal currentAmount)
        {
            decimal result = currentAmount * (decimal)0.25;
            Console.WriteLine("Income tax: €" + result.ToString("F"));
            return result;
        }

        public decimal CalculatePension(decimal currentAmount)
        {
            var result = currentAmount * (decimal)0.0919;
            Console.WriteLine("Pension: €" + result.ToString("F"));
            return result;
        }
    }

    public class GermanyDeductions : IGermanyDeductions
    {
        public decimal CalculateIncomeTax(decimal currentAmount)
        {
            decimal result = CalculateExtraTax.Calculate(currentAmount, 400, (decimal)0.25, (decimal)0.32);
            Console.WriteLine("Income tax: €" + result.ToString("F"));
            return result;
        }

        public decimal CalculatePension(decimal currentAmount)
        {
            var result = currentAmount * (decimal)0.02;
            Console.WriteLine("Pension: €" + result.ToString("F"));
            return result;
        }
    }

    public enum EmployeeLocation
    {
        Ireland = 0,
        Italy = 1,
        Germany = 2
    }
}