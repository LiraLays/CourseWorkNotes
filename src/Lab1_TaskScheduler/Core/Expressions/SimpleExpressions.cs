using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SmartTaskScheduler.Library.Core.Expressions
{
    public class SimpleExpressions
    {
        public abstract class Expression
        {
            public abstract Expression Substitute(string variable, string expression);
            public abstract string ToHumanReadable();
            public virtual List<string> GetDefinitenessConditions() => new List<string>();

            public static Expression ParseExpression(string expressionText)
            {
                expressionText = expressionText.Trim();

                if (double.TryParse(expressionText, out _))
                    return new ConstantExpression(expressionText);

                if (Regex.IsMatch(expressionText, @"^[a-zA-Z_][a-zA-Z0-9_]*$"))
                    return new VariableExpression(expressionText);

                return new ComplexExpression(expressionText);
            }
        }

        public class VariableExpression : Expression
        {
            public string Name { get; set; }

            public VariableExpression(string name) => Name = name;

            public override Expression Substitute(string variable, string expression)
            {
                return Name == variable ? ParseExpression(expression) : this;
            }

            public override string ToHumanReadable() => $"переменная {Name}";
            public override string ToString() => Name;
        }

        public class ConstantExpression : Expression
        {
            public string Value { get; set; }

            public ConstantExpression(string value) => Value = value;

            public override Expression Substitute(string variable, string expression) => this;
            public override string ToHumanReadable() => $"значение {Value}";
            public override string ToString() => Value;
        }

        public class ComplexExpression : Expression
        {
            public string ExpressionText { get; set; }

            public ComplexExpression(string expression) => ExpressionText = expression;

            public override Expression Substitute(string variable, string expression)
            {
                var pattern = $@"\b{variable}\b";
                var result = Regex.Replace(ExpressionText, pattern, $"({expression})");
                return new ComplexExpression(result);
            }

            public override string ToHumanReadable()
            {
                return ExpressionText
                    .Replace(">=", "больше или равно")
                    .Replace("<=", "меньше или равно")
                    .Replace(">", "больше")
                    .Replace("<", "меньше")
                    .Replace("&&", "и")
                    .Replace("||", "или")
                    .Trim();
            }

            public override string ToString() => ExpressionText;
        }
    }
}
