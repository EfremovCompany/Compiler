using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lexer
{
    class Reader
    {
        private string error = "";

        Dictionary<string, string> var;
        Dictionary<string, string> operators;
        Dictionary<string, string> mathOperators;
        Dictionary<string, string> separators;

        public Dictionary<string, string> GetTokensListOfVar()
        {
            Dictionary<string, string> token = new Dictionary<string, string>();
            token.Add("int", "INT");
            token.Add("const", "CONST");
            token.Add("array", "ARRAY");
            return token;
        }


        public Dictionary<string, string> GetTokensListOfOperators()
        {
            Dictionary<string, string> token = new Dictionary<string, string>();
            token.Add("globals", "GLOBALS");
            token.Add("output", "OUTPUT");
            token.Add("input", "INPUT");
            token.Add("main", "MAIN");
            token.Add("const", "CONST");
            token.Add("to", "TO");
            token.Add("for", "FOR");
            token.Add("while", "WHILE");
            token.Add("downto", "DOWNTO");
            token.Add("return", "RETURN");
            token.Add("if", "IF");
            token.Add("true", "THEN");
            token.Add("false", "ELSE");
            return token;
        }

        public Dictionary<string, string> GetTokensListOfMathOperators()
        {
            Dictionary<string, string> token = new Dictionary<string, string>();
            token.Add("<=", "LE_OP");
            token.Add(">=", "GE_OP");
            token.Add("==", "EQ_OP");
            token.Add("!=", "NE_OP");
            token.Add("=", "ASSIGN_OP");
            token.Add("-", "MINUS");
            token.Add("+", "PLUS");
            token.Add("*", "STAR");
            token.Add("div", "DIV");
            token.Add("mod", "MOD");
            token.Add("and", "AND_OP");
            token.Add("or", "OR_OP");
            token.Add("!", "NOT_OP");
            token.Add("les", "CHEVRON_L");
            token.Add("mor", "CHEVRON_R");
            return token;
        }

        public Dictionary<string, string> GetTokensListOfSeparators()
        {
            Dictionary<string, string> token = new Dictionary<string, string>();
            token.Add(";", "SEMICOLON");
            token.Add(",", "COMMA");
            token.Add(":", "COLON");
            token.Add("<", "TAG");
            token.Add("/>", "END_TAG");
            token.Add("(", "PARENTHESIS_L");
            token.Add(")", "PARENTHESIS_R");
            token.Add("[", "BRACKET_L");
            token.Add("]", "BRACKET_R");
            token.Add("{", "FIGURE_L");
            token.Add("}", "FIGURE_R");
            token.Add(".", "DOT");
            token.Add("\"", "QUOTE_STR");
            token.Add("\'", "QUOTE_CHAR");
            return token;
        }

        public Reader()
        {
            var = GetTokensListOfVar();
            operators = GetTokensListOfOperators();
            mathOperators = GetTokensListOfMathOperators();
            separators = GetTokensListOfSeparators();
            using (StreamWriter file = new StreamWriter(@"./lexer.txt", false))
            {
                file.Write("");
            }
        }

        public string GetNameOfError()
        {
            return error;
        }

        private void WriteToFile(string line)
        {
            using (StreamWriter file = new StreamWriter(@"./lexer.txt", true))
            {
                file.WriteLine(line);
            }
        }

        private bool IsIdCorrect(string id)
        {
            if (!Char.IsLetter(id[0]))
            {
                return false;
            }
            return true;
        }

        private void SetSeparator(int number, char token, string data)
        {
            WriteToFile(number.ToString() + " " + token + " " + data);
        }

        private bool IsStringDigit(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                if (!Char.IsDigit(input[i]))
                    return false;
            }
            return true;
        }

        private bool WorkWithToken(string token, int number)
        {
            //TODO: Check int in bracets
            token = token.Replace(" ", "");
            if (token == "")
            {
                return true;
            }
            if (separators.ContainsKey(token[0].ToString()) && token.Length > 1)
            {
                WriteToFile(number.ToString() + " "  + token[0] + " " + separators[token[0].ToString()]);
                if (operators.ContainsKey(token.Replace(token[0].ToString(), "")))
                    WriteToFile(number.ToString() + " " + token.Replace(token[0].ToString(), "") + " " + operators[token.Replace(token[0].ToString(), "")]);
                else if (IsStringDigit(token.Replace(token[0].ToString(), "")))
                    WriteToFile(number.ToString() + " " + token.Replace(token[0].ToString(), "") + " CONST_INT");
                else
                    WriteToFile(number.ToString() + " " + token.Replace(token[0].ToString(), "") + " IDENTIFICATOR");
                return true;
            }
            if (separators.ContainsKey(token))
            {
                WriteToFile(number.ToString() + " " + token + " " + separators[token]);
                return true;
            }
            if (operators.ContainsKey(token))
            {
                WriteToFile(number.ToString() + " " + token + " " + operators[token]);
                return true;
            }
            if (mathOperators.ContainsKey(token))
            {
                WriteToFile(number.ToString() + " " + token + " " + mathOperators[token]);
                return true;
            }
            if (var.ContainsKey(token))
            {
                WriteToFile(number.ToString() + " " + token + " " + var[token]);
                return true;
            }

            if (!IsStringDigit(token))
            {
                if (IsIdCorrect(token))
                {
                    WriteToFile(number.ToString() + " " + token + " IDENTIFICATOR");
                    return true;
                }
                else
                {
                    error = number.ToString() + " " + token + " ERROR";
                    WriteToFile(number.ToString() + " " + token + " ERROR");
                    return false;
                }
            }
            else
            {
                WriteToFile(number.ToString() + " " + token + " CONST_INT");
                return true;
            }
            //return false;
        }

        public bool CheckString(string line, int lineNumber)
        {
            string token = "";
            line += " ";
            bool isEndToken = false;
            for (int i = 0; i < line.Length - 1; i++)
            {
                if (!isEndToken)
                {
                    token += line[i];
                }
                else
                {
                    if (!WorkWithToken(token, lineNumber))
                    {
                        error = lineNumber + " " + token + " ERROR";
                        WriteToFile(error);
                        return false;
                    }
                    token = "";
                    if (line[i] != ' ')
                        token += line[i];
                    isEndToken = false;
                }
                //string s = line[i].ToString();
                if (line[i + 1] == ' ' || 
                    separators.ContainsKey(line[i + 1].ToString()) || 
                    mathOperators.ContainsKey(line[i + 1].ToString()) || (line[i + 1] == '\"' ||
                    i == line.Length - 1) || (i < line.Length - 1 && line[i] == '\\' && line[i + 1] == '\"'))
                {
                    isEndToken = true;
                }
            }
            if (token != "")
            {
                if (!WorkWithToken(token, lineNumber))
                {
                    error = lineNumber + " " + token + " ERROR";
                    WriteToFile(error);
                    return false;
                }
            }
            return true;
        }
    }
}
