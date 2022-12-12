using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFCalculator.Models
{
    public class CalculatorData
    {
        public string CurrentEntryText { get; set; } = "0";
        public double? LeftValue { get; set; }
        public double? RightValue { get; set; }
        public double HoldValue { get; set; }
        public MathOperator LastOperator { get; set; }
        public MathOperator NewOperator { get; set; }

    

    }
}
