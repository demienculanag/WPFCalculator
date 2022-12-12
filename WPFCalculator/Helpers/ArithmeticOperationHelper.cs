namespace WPFCalculator.Helpers
{
    public static class ArithmeticOperationHelper
    {
        public static double Compute(string arithmeticOperation, double value1, double value2)
        {
            switch (arithmeticOperation)
            {
                case "Add": return value1 + value2;

                case "Subtract": return value1 - value2;

                case "Multiply": return value1 * value2;

                case "Divide": return value1 / value2;

                case "Modulo": return value1 % value2;

                default: return value1;


            }
        }
    }
}
