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

            var opNames = Enum.GetNames<Operators>();
            for (int i = 0; i < opNames.Length; i++)
            {
                compiledexpression = compiledexpression.Replace($"{opNames[i]}", $"*{i}");
            }

            var expr = compiledexpression.ToString();
            ix = 0;
            while(ix < expr.Length)
            {
                if (expr[ix] == '#')
                {
                    var id = int.Parse(expr.Substring(ix + 1, 1));
                    _stack.Push(new Element { Variable = vars[id] });
                    ++ix;
                }
                else if (expr[ix] == '*')
                {
                    var id = int.Parse(expr.Substring(ix + 1, 1));
                    _stack.Push(new Element { Operator = Enum.GetValues<Operators>()[id] });
                    ++ix;
                }
                else if (expr[ix] != ' ')
                {
                    throw new Exception("Invalid characters in expression");
                }
                ++ix;
            }
        }

        public object Execute()
        {
            var el = _stack.Pop();

            if (el.ElType == Element.ElTypes.Operator)
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
            else if (el.ElType == Element.ElTypes.Variable)
                return Variables[el.Variable] ?? String.Empty;
            else if (el.ElType == Element.ElTypes.Value)
                return el.Value;
            throw new Exception("Invalid stack");
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
            public enum ElTypes { Value, Operator, Variable }
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
