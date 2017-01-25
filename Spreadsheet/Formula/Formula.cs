// Skeleton written by Joe Zachary for CS 3500, January 2017
// Start PS2 by Fang He, January 24 2017

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Formulas
{
    /// <summary>
    /// Represents formulas written in standard infix notation using standard precedence
    /// rules.  Provides a means to evaluate Formulas.  Formulas can be composed of
    /// non-negative floating-point numbers, variables, left and right parentheses, and
    /// the four binary operator symbols +, -, *, and /.  (The unary operators + and -
    /// are not allowed.)
    /// </summary>
    public class Formula
    {
        /// <summary>
        /// Creates a Formula from a string that consists of a standard infix expression composed
        /// from non-negative floating-point numbers (using C#-like syntax for double/int literals), 
        /// variable symbols (a letter followed by zero or more letters and/or digits), left and right
        /// parentheses, and the four binary operator symbols +, -, *, and /.  White space is
        /// permitted between tokens, but is not required.
        /// 
        /// Examples of a valid parameter to this constructor are:
        ///     "2.5e9 + x5 / 17"
        ///     "(5 * 2) + 8"
        ///     "x*y-2+35/9"
        ///     
        /// Examples of invalid parameters are:
        ///     "_"
        ///     "-5.3"
        ///     "2 5 + 3"
        /// 
        /// If the formula is syntacticaly invalid, throws a FormulaFormatException with an 
        /// explanatory Message.
        /// </summary>
        
        /// There can be no invalid tokens.
        
        /// There must be at least one token.

        /// When reading tokens from left to right, at no point should the number of closing parentheses seen so far be greater than the number of opening parentheses seen so far.

        /// The total number of opening parentheses must equal the total number of closing parentheses.

        /// The first token of a formula must be a number, a variable, or an opening parenthesis.

        /// The last token of a formula must be a number, a variable, or a closing parenthesis.

        /// Any token that immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis.

        /// Any token that immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.

        public Formula(String formula)
        {
            IEnumerable<string> tokens = GetTokens(formula);
            List<String> listTokens = new List<string>();
            foreach (String s in tokens)
            {
                listTokens.Add(s);
            }
            if (listTokens.Count == 0)
            {
                throw new FormulaFormatException("Formula is empty");
            }
            int pareNum = 0;
            foreach (String s in tokens)
            {
                Console.Write(s+" ");
                if (s.Equals("("))
                {
                    pareNum++;
                }
                if( s.Equals(")")) 
                {
                    if(pareNum <= 0) throw new FormulaFormatException("Formular has ilegal parenthesis");
                    pareNum--;
                }
            }
            Console.WriteLine("");
            if(pareNum != 0)
            {
                throw new FormulaFormatException("Formular has ilegal parenthesis not equal openning and closing");
            }
            if (!Regex.IsMatch(listTokens[0], @"^[a-zA-Z0-9(]+$"))
            {
                throw new FormulaFormatException("Formular first token is not number or openning parenthesis");
            }
            if (!Regex.IsMatch(listTokens[listTokens.Count-1], @"^[a-zA-Z0-9)]+$"))
            {
                throw new FormulaFormatException("Formular last token is not number or closing parenthesis");
            }
            for(int i=0; i< listTokens.Count - 1; i++)
            {
                if (Regex.IsMatch(listTokens[i], @"^[(\-\+*/(]+$")&& !Regex.IsMatch(listTokens[i+1], @"^[a-zA-Z0-9(]+$"))
                {
                    throw new FormulaFormatException("Formular token immediately follows an opening parenthesis or an operator must be either a number, a variable, or an opening parenthesis");
                }
                if(Regex.IsMatch(listTokens[i], @"^[a-zA-Z0-9)]+$") && !Regex.IsMatch(listTokens[i+1], @"^[\+\-*/)]+$"))
                {
                    throw new FormulaFormatException("Formular token immediately follows a number, a variable, or a closing parenthesis must be either an operator or a closing parenthesis.");
                }
            }

        }
        /// <summary>
        /// Evaluates this Formula, using the Lookup delegate to determine the values of variables.  (The
        /// delegate takes a variable name as a parameter and returns its value (if it has one) or throws
        /// an UndefinedVariableException (otherwise).  Uses the standard precedence rules when doing the evaluation.
        /// 
        /// If no undefined variables or divisions by zero are encountered when evaluating 
        /// this Formula, its value is returned.  Otherwise, throws a FormulaEvaluationException  
        /// with an explanatory Message.
        /// </summary>
        public double Evaluate(Lookup lookup)
        {
            return 0;
        }

        /// <summary>
        /// Given a formula, enumerates the tokens that compose it.  Tokens are left paren,
        /// right paren, one of the four operator symbols, a string consisting of a letter followed by
        /// zero or more digits and/or letters, a double literal, and anything that doesn't
        /// match one of those patterns.  There are no empty tokens, and no token contains white space.
        /// </summary>
        private static IEnumerable<string> GetTokens(String formula)
        {
            // Patterns for individual tokens
            String lpPattern = @"\(";
            String rpPattern = @"\)";
            String opPattern = @"[\+\-*/]";
            String varPattern = @"[a-zA-Z][0-9a-zA-Z]*";
            // PLEASE NOTE:  I have added white space to this regex to make it more readable.
            // When the regex is used, it is necessary to include a parameter that says
            // embedded white space should be ignored.  See below for an example of this.
            String doublePattern = @"(?: \d+\.\d* | \d*\.\d+ | \d+ ) (?: e[\+-]?\d+)?";
            String spacePattern = @"\s+";

            // Overall pattern.  It contains embedded white space that must be ignored when
            // it is used.  See below for an example of this.
            String pattern = String.Format("({0}) | ({1}) | ({2}) | ({3}) | ({4}) | ({5})",
                                            lpPattern, rpPattern, opPattern, varPattern, doublePattern, spacePattern);

            // Enumerate matching tokens that don't consist solely of white space.
            // PLEASE NOTE:  Notice the second parameter to Split, which says to ignore embedded white space
            /// in the pattern.
            foreach (String s in Regex.Split(formula, pattern, RegexOptions.IgnorePatternWhitespace))
            {
                if (!Regex.IsMatch(s, @"^\s*$", RegexOptions.Singleline))
                {
                    yield return s;
                }
            }
        }
    }

    /// <summary>
    /// A Lookup method is one that maps some strings to double values.  Given a string,
    /// such a function can either return a double (meaning that the string maps to the
    /// double) or throw an UndefinedVariableException (meaning that the string is unmapped 
    /// to a value. Exactly how a Lookup method decides which strings map to doubles and which
    /// don't is up to the implementation of the method.
    /// </summary>
    public delegate double Lookup(string var);

    /// <summary>
    /// Used to report that a Lookup delegate is unable to determine the value
    /// of a variable.
    /// </summary>
    [Serializable]
    public class UndefinedVariableException : Exception
    {
        /// <summary>
        /// Constructs an UndefinedVariableException containing whose message is the
        /// undefined variable.
        /// </summary>
        /// <param name="variable"></param>
        public UndefinedVariableException(String variable)
            : base(variable)
        {
        }
    }

    /// <summary>
    /// Used to report syntactic errors in the parameter to the Formula constructor.
    /// </summary>
    [Serializable]
    public class FormulaFormatException : Exception
    {
        /// <summary>
        /// Constructs a FormulaFormatException containing the explanatory message.
        /// </summary>
        public FormulaFormatException(String message) : base(message)
        {
        }
    }

    /// <summary>
    /// Used to report errors that occur when evaluating a Formula.
    /// </summary>
    [Serializable]
    public class FormulaEvaluationException : Exception
    {
        /// <summary>
        /// Constructs a FormulaEvaluationException containing the explanatory message.
        /// </summary>
        public FormulaEvaluationException(String message) : base(message)
        {
        }
    }
}
