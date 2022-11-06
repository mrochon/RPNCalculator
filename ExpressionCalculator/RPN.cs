using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ExpressionCalculator
{
    internal class RPN
    {
        Stack<Element> _stack = new Stack<Element>();


        public enum Operators { not, eq, ne, lt, gt, and, or };

        public RPN()
        {
            Variables = new Dictionary<string, object>();
        }
        public Dictionary<string, object> Variables { get; }

        public void Parse(string expression)
        {
            _stack.Clear();

            /* Tokenize variable names */
            var pattern = "{[a-zA-Z0-9_]+}";
            var compiledexpression = new StringBuilder(expression);
            var rg = new Regex(pattern);
            var ix = 0;
            var vars = new List<string>();
            foreach(var name in rg.Matches(expression))
            {
                var varName = ((Match)name).Value;
                vars.Add(varName.Substring(1, varName.Length - 2));
                compiledexpression.Replace(((Match)name).Value, $"#{ix++}");
            }

            // Tokenize literals
            pattern = "\\.[a-zA-Z0-9_-]*\\.";
            rg = new Regex(pattern);
            ix = 0;
            var consts = new List<string>();
            foreach (var name in rg.Matches(expression))
            {
                var constValue = ((Match)name).Value;
                consts.Add(constValue.Substring(1, constValue.Length - 2));
                compiledexpression.Replace(((Match)name).Value, $".{ix++}");
            }

            // Tokenize operators
            var opNames = Enum.GetNames<Operators>();
            for (int i = 0; i < opNames.Length; i++)
            {
                compiledexpression = compiledexpression.Replace($"{opNames[i]}", $"*{i}");
            }

            // Convert to stack
            var expr = compiledexpression.ToString();
            ix = 0;
            while(ix < expr.Length)
            {
                switch (expr[ix])
                {
                    case '#':
                        var id = int.Parse(expr.Substring(ix + 1, 1));
                        _stack.Push(new Element { Variable = vars[id] });
                        ++ix;
                        break;
                    case '.':
                        id = int.Parse(expr.Substring(ix + 1, 1));
                        _stack.Push(new Element { Literal = consts[id] });
                        ++ix;
                        break;
                    case '*':
                        id = int.Parse(expr.Substring(ix + 1, 1));
                        _stack.Push(new Element { Operator = Enum.GetValues<Operators>()[id] });
                        ++ix;
                        break;
                    case ' ':
                        break;
                    default:
                            throw new Exception("Invalid characters in expression");
                }
                ++ix;
            }
        }

        public object Execute()
        {
            var el = _stack.Pop();

            switch (el.ElType)
            {
                case Element.ElTypes.Operator:
                    {
                        switch (el.Operator)
                        {
                            case Operators.not:
                                return !(bool)Execute();
                            case Operators.eq:
                                return Compare() == 0;
                            case Operators.ne:
                                return Compare() != 0;
                            case Operators.lt:
                                return Compare() < 0;
                            case Operators.gt:
                                return Compare() > 0;
                            case Operators.and:
                                return (bool)Execute() && (bool)Execute();
                            case Operators.or:
                                return (bool)Execute() || (bool)Execute();
                            default:
                                throw new Exception("Invalid element");
                        }
                    }
                case Element.ElTypes.Variable:
                    return Variables.FirstOrDefault(e => e.Key == el.Variable).Value?? String.Empty;
                case Element.ElTypes.Literal:
                    return el.Literal;
                case Element.ElTypes.Value:
                    return el.Value;
                default:
                    throw new Exception("Invalid stack");
            }
        }
        private int Compare()
        {
            var op1 = Execute();
            var op2 = Execute();
            if (op1.GetType() != op2.GetType())
                throw new Exception("Incompatible operands");
            var t = op1.GetType().Name;
            switch (t)
            {
                case "String":
                    return String.Compare((string)op1, (string)op2);
                default:
                    throw new Exception("Unsupported operand types");
            }
        }
        public class Element
        {
            public enum ElTypes { Value, Operator, Variable, Literal }
            private ElTypes _elType;
            public ElTypes ElType { get => _elType; }
            private bool _value;
            public bool Value
            {
                get => _value;
                set
                {
                    _value = value;
                    _elType = ElTypes.Value;
                }
            }
            private string _variable;
            public string Variable
            {
                get => _variable;
                set
                {
                    _variable = value;
                    _elType = ElTypes.Variable;
                }
            }
            private string _literal;
            public string Literal
            {
                get => _literal;
                set
                {
                    _literal = value;
                    _elType = ElTypes.Literal;
                }
            }
            private Operators _operator;
            public Operators Operator
            {
                get => _operator;
                set
                {
                    _operator = value;
                    _elType = ElTypes.Operator;
                }
            }
        }
    }
}
