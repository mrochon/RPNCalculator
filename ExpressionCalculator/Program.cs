// See https://aka.ms/new-console-template for more information
using ExpressionCalculator;

Console.WriteLine("Simple expression calculator");

var rpn = new RPN();
rpn.Variables.Add("MFAChoice", "email");
//rpn.Variables.Add("v2", null);
//rpn.Variables.Add("var_3", "ABC");
//rpn.Parse("{v1}.ABC.eq {v1}{v2}eq or not");
//rpn.Parse("{v1}.ABC.eq {v1}{v2}eq or not");
//rpn.Parse("{v1}.ABCD.gt");
rpn.Parse("{MFAChoice}.email.eq {strongAuthenticationPhoneNumber}..ne or");
//rpn.Parse("{strongAuthenticationPhoneNumber}.. eq");
Console.WriteLine($"Result: {rpn.Execute()}");