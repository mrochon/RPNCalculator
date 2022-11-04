// See https://aka.ms/new-console-template for more information
using ExpressionCalculator;

Console.WriteLine("Simple expression calculator");

var rpn = new RPN();
rpn.Variables.Add("v1", "ABC");
rpn.Variables.Add("v2", null);
rpn.Variables.Add("var_3", "ABC");
rpn.Parse("{v1}{var_3}eq {v1}{v2}eq or not");
Console.WriteLine($"Result: {rpn.Execute()}");