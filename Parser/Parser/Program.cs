using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Parser
{
    class Tree
    {
        public string data { get; set; }
        public Tree Var { get; set; }
        public Tree Eq { get; set; }
        public Tree Return { get; set; }

        public List<Tree> Block;
    }


    class Program
    {
        //переменные
        static List<Tokens> tokens = OpenFile("lexer.txt");
        static Tree tree = new Tree();
        //--------

        //Вывод дерева
        static void PrintTree(Tree tree, int j)
        {
            using (StreamWriter file = new StreamWriter(@"sint.txt", true))
            {
                file.WriteLine(new String('.', j) + tree.data);
            }
            if (tree.Var != null)
            {
                PrintTree(tree.Var, j + 1);
            }
            if (tree.Eq != null)
            {
                PrintTree(tree.Eq, j + 1);
            }
            if (tree.Block != null)
            {
                for (int i = 0; i < tree.Block.Count; i++)
                {
                    PrintTree(tree.Block[i], j + 1);
                }
            }
            if (tree.Return != null)
            {
                PrintTree(tree.Return, j + 1);
            }
        }
        //

        //Структура
        struct Tokens
        {
            public string line;
            public string token;
            public string name;
        }
        //--------

        //Parse const
        static int ConstInit(int i)
        {
            if (tokens[i].token != "int")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            if (tokens[i].name != "IDENTIFICATOR")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            if (tokens[i].token != "=")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            if (tokens[i].name != "CONST_INT")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            if (tokens[i].token != ";")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            return i;
        }

        static Tree WriteConstString(int i)
        {
            Tree tree = new Tree();
            tree.data = tokens[i + 2].token;
            tree.Var = new Tree();
            tree.Var.data = tokens[i + 1].token;
            tree.Eq = new Tree();
            tree.Eq.data = tokens[i + 3].token;
            return tree;
        }

        static int ConstParse(Tree tree)
        {
            int i = 2;
            tree.Block = new List<Tree>();
            while (tokens[i].token != "/>")
            {
                int old_i = i;

                i = ConstInit(i);
                tree.Block.Add(WriteConstString(old_i));

                ++i;
            }
            return i;
        }
        //-----

        //Parse-Main
        static int SelectMainReg(int i)
        {
            if (tokens[i].name == "INT")
                return 1;
            if (tokens[i].name == "ARRAY")
                return 2;
            if (tokens[i].name == "TAG" && tokens[i + 1].name == "FOR")
                return 3;
            if (tokens[i].name == "IF")
                return 4;
            if (tokens[i].name == "TAG" && tokens[i + 1].name == "THEN")
                return 5;
            if (tokens[i].name == "IDENTIFICATOR")
                return 6;
            if (tokens[i].name == "OUTPUT")
                return 7;
            if (tokens[i].name == "TAG" && tokens[i + 1].name == "ELSE")
                return 8;
            return 0;
        }

        static KeyValuePair<int, Tree> InitInt(int i)
        {
            string var = "";
            string eq = "";
            if (tokens[i].name != "IDENTIFICATOR")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            eq += tokens[i].token;
            ++i;
            if (tokens[i].token != "=")
            {
                if (tokens[i].token != ";")
                    throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                else
                {
                    ++i;
                    Tree tree = new Tree();
                    tree.data = tokens[i - 2].token;
                    KeyValuePair<int, Tree> pair = new KeyValuePair<int, Tree>(i, tree);
                    return pair;
                }
            }
            else
            {
                ++i;
                if (tokens[i].name == "CONST_INT" || tokens[i].name == "IDENTIFICATOR" || tokens[i].name == "INPUT") //ID
                {
                    var += tokens[i].token;
                    if (tokens[i].name == "INPUT")
                    {
                        ++i;
                        if (tokens[i].token != ";")
                            throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                        Tree tree2 = new Tree();
                        tree2.Var = new Tree();
                        tree2.Eq = new Tree();
                        tree2.Var.data = tokens[i].token;
                        tree2.data = "int";
                        tree2.Eq.data = tokens[i - 1].token;
                        KeyValuePair<int, Tree> pair1 = new KeyValuePair<int, Tree>(i, tree2);
                        return pair1;
                    }
                    if (tokens[i].name == "CONST_INT")
                    {
                        var = tokens[i].token;
                    }
                    else
                    {
                        ++i;
                        if (tokens[i].token == "[")
                        {
                            var += "[";
                            ++i;
                            if (tokens[i].name != "CONST_INT" && tokens[i].name != "IDENTIFICATOR")
                                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                            else
                            {
                                if (tokens[i].name == "IDENTIFICATOR")
                                {
                                    var += tokens[i].token;
                                    ++i;
                                    if (tokens[i].token == "+" || tokens[i].token == "-")
                                    {
                                        var += tokens[i].token;
                                        ++i;
                                        if (tokens[i].name != "CONST_INT")
                                            throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                                        var += tokens[i].token;
                                    }
                                }
                            }
                            //++i;
                            if (tokens[i].token != "]")
                                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                            var += tokens[i].token;
                        }
                    }
                }
                else
                    throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                ++i;
                if (tokens[i].token != ";")
                    throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                Tree tree1 = new Tree();
                tree1.Var = new Tree();
                tree1.Eq = new Tree();
                tree1.Var.data = eq;
                tree1.data = "int";
                tree1.Eq.data = var;
                KeyValuePair<int, Tree> pair = new KeyValuePair<int, Tree>(i, tree1);
                return pair;
            }
        }

        static KeyValuePair<int, Tree> InitArray(int i)
        {
            --i;
            string eq = "";
            if (tokens[i + 2].name != "IDENTIFICATOR")
                throw new Exception("Error in line " + tokens[i + 2].line.ToString() + ". Token name " + tokens[i + 2].name);
            if (tokens[i + 3].token != "[")
                throw new Exception("Error in line " + tokens[i + 3].line.ToString() + ". Token name " + tokens[i + 3].name);
            if (tokens[i + 4].name != "CONST_INT")
                throw new Exception("Error in line " + tokens[i + 4].line.ToString() + ". Token name " + tokens[i + 4].name);
            if (tokens[i + 5].token != "]")
                throw new Exception("Error in line " + tokens[i + 5].line.ToString() + ". Token name " + tokens[i + 5].name);
            string var = tokens[i + 2].token + tokens[i + 3].token + tokens[i + 4].token + tokens[i + 5].token;
            if (tokens[i + 6].token != "=")
            {
                if (tokens[i + 6].token != ";")
                    throw new Exception("Error in line " + tokens[i + 6].line.ToString() + ". Token name " + tokens[i + 6].name);
                else
                {
                    Tree tree2 = new Tree();
                    tree2.data = tokens[i + 2].token + tokens[i + 3].token + tokens[i + 4].token + tokens[i + 5].token;
                    KeyValuePair<int, Tree> pair = new KeyValuePair<int, Tree>(i + 6, tree2);
                    return pair;
                }
                //return i + 6;
            }
            else
            {
                if (tokens[i + 7].token != "{")
                    throw new Exception("Error in line " + tokens[i + 7].line.ToString() + ". Token name " + tokens[i + 7].name);
                i = i + 8;
                if (tokens[i].name != "CONST_INT")
                    throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                else
                    eq += tokens[i].token + ", ";
                ++i;
                while (tokens[i].token != "}")
                {
                    if (tokens[i].token != ",")
                        throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                    ++i;
                    if (tokens[i].name != "CONST_INT")
                        throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                    else
                        eq += tokens[i].token + ", ";
                    ++i;
                }
                if (tokens[i].token != "}")
                    throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                if (tokens[i + 1].token != ";")
                    throw new Exception("Error in line " + tokens[i + 1].line.ToString() + ". Token name " + tokens[i + 1].name);
            }
            Tree tree1 = new Tree();
            tree1.Var = new Tree();
            tree1.Eq = new Tree();
            tree1.Var.data = var;
            tree1.data = "array";
            tree1.Eq.data = eq.Substring(0, eq.Length - 2);
            KeyValuePair<int, Tree> pair1 = new KeyValuePair<int, Tree>(i + 1, tree1);
            return pair1;
            //return i + 1;
        }

        static KeyValuePair<int, Tree> ForParse(int i)
        {
            string data = "<for";
            Tree d = new Tree();
            d.data = data;
            d.Return = new Tree();
            d.Return.data = "/>";
            ++i;
            if (tokens[i].name != "IDENTIFICATOR")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            string var = tokens[i].token;
            ++i;
            if (tokens[i].token != "=")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            d.Var = new Tree();
            d.Var.data = tokens[i].token;
            ++i;
            if (tokens[i].name != "CONST_INT")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            d.Var.Eq = new Tree();
            d.Var.Eq.data = tokens[i].token;
            ++i;
            if (tokens[i].token != "to" && tokens[i].token != "downto")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            d.Eq = new Tree();
            d.Var.Var = new Tree();
            d.Var.Var.data = var;
            d.Eq.data = tokens[i].token;
            ++i;
            if (tokens[i].name != "CONST_INT")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            d.Eq.Eq = new Tree();
            d.Eq.Eq.data = tokens[i].token;
            i = MainParse(d, i + 1);
            KeyValuePair<int, Tree> pair1 = new KeyValuePair<int, Tree>(i, d);
            return pair1;
        }

        static int ArrayParse(int i)
        {
            return i;
        }

        static int OpParse(int i)
        {

            return i;
        }

        static KeyValuePair<int, string> IdParse(int i)
        {
            string res = "";
            if (tokens[i].name == "IDENTIFICATOR")
            {
                res += tokens[i].token;
                ++i;
                if (tokens[i].token == "+" || tokens[i].token == "-" || tokens[i].token == "*" || tokens[i].token == "div")
                {
                    string data = res + tokens[i].token;
                    while(tokens[i].token != ";")
                    {
                        data += tokens[i].token;
                        ++i;
                    }
                }
                if (tokens[i].token == "[")
                {
                    res += tokens[i].token;
                    ++i;
                    if ((tokens[i].name == "CONST_INT" || tokens[i].name == "IDENTIFICATOR") && tokens[i + 1].token != "]")
                    {
                        res += tokens[i].token;
                        ++i;
                        if (tokens[i].token == "+" || tokens[i].token == "-")
                        {
                            res += tokens[i].token;
                            ++i;
                            if (tokens[i].name != "CONST_INT")
                                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                            res += tokens[i].token;
                            ++i;
                        }
                        if (tokens[i].token != "]")
                            throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                        res += tokens[i].token;
                    }
                    else
                    {
                        res += tokens[i].token;
                        i = i + 1;
                        res += tokens[i].token;
                    }
                }
            }
            else
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            KeyValuePair<int, string> pair = new KeyValuePair<int, string>(i, res);
            return pair;
        }

        static KeyValuePair<int, Tree> IfParse(int i)
        {
            KeyValuePair<int, string> res = new KeyValuePair<int, string>();
              ++i;
            if (tokens[i].token != "!" && tokens[i].name != "IDENTIFICATOR")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            res = IdParse(i);
            i = res.Key;
            string var = res.Value;
            ++i;
            if (tokens[i].token != "<=" && tokens[i].token != ">=" && tokens[i].token != "==" && tokens[i].token != "!="
                && tokens[i].token != "mor" && tokens[i].token != "les")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            string condition = tokens[i].token;
            ++i;
            res = IdParse(i);
            i = res.Key;
            string eq = res.Value;
            Tree tree1 = new Tree();
            tree1.Var = new Tree();
            tree1.data = "if";
            tree1.Var.data = condition;
            tree1.Var.Var = new Tree();
            tree1.Var.Eq = new Tree();
            tree1.Var.Var.data = var;
            tree1.Var.Eq.data = eq;
            KeyValuePair<int, Tree> pair = new KeyValuePair<int, Tree>(i, tree1);
            return pair;
        }

        static KeyValuePair<int, Tree> AssignParse(int i)
        {
            string data = "=";
            KeyValuePair<int, string> pair = new KeyValuePair<int, string>();
            pair = IdParse(i);
            string var = pair.Value;
            i = pair.Key;
            ++i;
            if (tokens[i].token != "=")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            string eq = "";
            if (tokens[i].name != "INPUT")
            {
                --i;
                pair = IdParse(i + 1);
                eq = pair.Value;
                i = pair.Key;
            }
            ++i;
            if (tokens[i].token != ";")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            Tree s = new Tree();
            s.data = data;
            s.Eq = new Tree();
            s.Eq.data = eq;
            s.Var = new Tree();
            s.Var.data = var;
            KeyValuePair<int, Tree> res = new KeyValuePair<int, Tree>(i, s);
            return res;
        }

        static KeyValuePair<int, Tree> OutputParse(int i)
        {
            if (tokens[i].token != "(")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            string id = "";
            while (tokens[i].token != ")")
            {
                if (tokens[i].token != "\"")
                {
                    KeyValuePair<int, string> pair = new KeyValuePair<int, string>();
                    pair = IdParse(i);
                    id += pair.Value;
                    i = pair.Key;
                }
                else
                {
                    ++i;
                    if (tokens[i].name != "TEXT")
                        throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                    id += tokens[i].token;
                    ++i;
                    if (tokens[i].token != "\"")
                        throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                }
                ++i;
            }
            if (tokens[i].token != ")")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            ++i;
            if (tokens[i].token != ";")
                throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
            Tree s = new Tree();
            s.data = "output";
            s.Var = new Tree();
            s.Var.data = id;
            KeyValuePair<int, Tree> pair1 = new KeyValuePair<int, Tree>(i, s);
            return pair1;
        }

        static int MainParse(Tree tree, int i)
        {
            tree.Block = new List<Tree>();
            while (tokens[i].token != "/>")
            {
                int case_ = SelectMainReg(i);
                KeyValuePair<int, Tree> pair = new KeyValuePair<int, Tree>();
                switch (case_)
                {
                    case 1:
                        pair = InitInt(i + 1);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        break;
                    case 2:
                        pair = InitArray(i);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        break;
                    case 3:
                        pair = ForParse(i + 1);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        break;
                    case 4:
                        pair = IfParse(i);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        break;
                    case 5:
                        Tree s = new Tree();
                        s.data = "<true";
                        tree.Block.Add(s);
                        i = MainParse(s, i + 2);
                        break;
                    case 6:
                        pair = AssignParse(i);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        //i = AssignParse(i);
                        break;
                    case 7:
                        pair = OutputParse(i + 1);
                        i = pair.Key;
                        tree.Block.Add(pair.Value);
                        //i = OutputParse(i + 1);
                        break;
                    case 8:
                        Tree s1 = new Tree();
                        s1.data = "<false";
                        i = MainParse(s1, i + 2);
                        break;
                    case 0:
                        throw new Exception("Error in line " + tokens[i].line.ToString() + ". Token name " + tokens[i].token);
                }
                ++i;
            }
            if (i >= tokens.Count)
                throw new Exception("Tag must be close by />");
            return i;
        }
        //-----

        //Main

        static int Main(string[] args)
        {
            try
            {
                int i = 0;
                if (tokens[0].token != "<")
                {
                    throw new Exception("First symbol must be <");
                }
                if (tokens[1].name == "MAIN")
                {
                    tree.data = "<main";
                    tree.Return = new Tree();
                    tree.Return.data = "/>";
                    i = MainParse(tree, 2);
                }
                else if (tokens[1].name == "CONST")
                {
                    tree.data = "<const";
                    tree.Return = new Tree();
                    tree.Return.data = "/>";
                    i = ConstParse(tree);
                    ++i;
                    if (tokens[i].token == "<" && tokens[i + 1].name == "MAIN")
                    {
                        tree.Return.Block = new List<Tree>();
                        Tree new_tree = new Tree();
                        new_tree.data = "<main";
                        new_tree.Return = new Tree();
                        new_tree.Return.data = "/>";
                        tree.Return.Block.Add(new_tree);
                        i = MainParse(tree.Return.Block[0], i + 2);
                    }
                    else
                        throw new Exception("Need main block");
                }
                else
                    throw new Exception("Need main or const block");

                if (i != tokens.Count - 1)
                    throw new Exception("Code have symbols after close tag");
                using (StreamWriter file = new StreamWriter(@"sint.txt", false))
                {
                    file.Write("");
                }
                PrintTree(tree, 0);
                Console.WriteLine("Success");
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
        //-----

        static List<Tokens> OpenFile(string data)
        {
            List<Tokens> tokens = new List<Tokens>();
            //Tokens tokens = new Tokens();
            using (StreamReader file = new StreamReader(@data))
            {
                while (!file.EndOfStream)
                {
                    string s = file.ReadLine();
                    try
                    {
                        string[] parse = s.Split(' ');
                        Tokens token = new Tokens();
                        token.line = parse[0];
                        token.token = parse[1];
                        token.name = parse[2];
                        tokens.Add(token);
                    }
                    catch
                    {
                        throw new Exception("File is broken");
                    }
                }
            }
            return tokens;
        }
    }
}
