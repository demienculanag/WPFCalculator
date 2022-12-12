using System.ComponentModel.DataAnnotations;

namespace WPFCalculator.Models
{
    public enum MathOperator
    {
        [Display(Name = "--")]
        Default = 0,
        [Display(Name = "+")]
        Add = 1,
        [Display(Name = "-")]
        Subtract = 2,
        [Display(Name = "x")]
        Multiply = 3,
        [Display(Name = "/")]
        Divide = 4,
        [Display(Name = "%")]
        Modulo = 5
    }
}
