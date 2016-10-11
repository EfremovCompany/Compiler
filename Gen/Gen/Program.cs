using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using static System.Linq.Expressions.Expression;

namespace Project
{
    class Program
    {
        static void Main(string[] args) => Run(args);

        private static void Run(string[] args)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            AssemblyName assemblyName = new AssemblyName("t");
            AssemblyBuilder assemblyBuilder = domain.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave, "./");
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyName.Name, assemblyName.Name + ".exe");
            TypeBuilder typeBuilder = moduleBuilder.DefineType("Project.Program", TypeAttributes.Public | TypeAttributes.AutoClass | TypeAttributes.BeforeFieldInit);
            MethodBuilder methodBuilder = typeBuilder.DefineMethod("Main", MethodAttributes.Private | MethodAttributes.Static | MethodAttributes.HideBySig);
            List<ParameterExpression> vars = new List<ParameterExpression>();
            string[] code = File.ReadAllLines("./sint.txt");
            int i = 0;
            while (code[i] != "./>")
            {
                if (code[i].Replace(".", "") == "int")
                {
                    vars.Add(Variable(typeof(int), code[i + 1].Replace(".", "")));
                    code[i] = "";
                }
                ++i;
            }
            code[i] = "";
            List<ParameterExpression> list_of_vars = new List<ParameterExpression>();
            BlockExpression blockExpression = Block(vars, GetExpressions(vars, File.ReadAllLines("./sint.txt"), i + 1, true));
            Lambda<Action>(blockExpression).CompileToMethod(methodBuilder);
            assemblyBuilder.SetEntryPoint(methodBuilder);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".exe");
        }

        private static List<Expression> GetExpressions(List<ParameterExpression> vars, string[] code, int exitCondition, bool isStart)
        {
            List<Expression> expressions = new List<Expression>{};

            if (isStart)
                for (int i = 0; i < vars.Count; i++)
                {
                    expressions.Add(vars[i]);
                }
            
            code[exitCondition] = code[exitCondition].Replace(".", "");
            string item = code[exitCondition];
            for (int i = exitCondition; item != "/>"; i++)
            {
                item = code[i];
                item = item.Replace(".", "");
                switch (item)
                {
                    case "array":
                        code[i + 1] = code[i + 1].Replace(".", "");
                        code[i + 2] = code[i + 2].Replace(".", "");
                        
                        expressions.Add(Expression.ArrayAccess(
                            Expression.Parameter(typeof(int[]), "Array"), Expression.Parameter(typeof(int), "Index")
                        ));
                        break;
                    case "=":
                        for (int j = 0; j < vars.Count; j++)
                        {
                            if (code[i + 1].Replace(".", "") == vars[j].Name)
                            {
                                if (code[i + 2].Replace(".", "") == "input")
                                {
                                    //string constantStr = "Write " + code[i + 1] + ":";
                                    code[i + 1] = code[i + 1].Replace(".", "");
                                    code[i + 2] = code[i + 2].Replace(".", "");
                                    expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(String) }), Expression.Constant("Write ")));
                                    expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(String) }), Expression.Constant(vars[j].Name)));
                                    expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(String) }), Expression.Constant(": ")));
                                    Expression constant = Expression.Call(typeof(Console).GetMethod("ReadLine"));
                                    constant = Expression.Call(typeof(Int32).GetMethod("Parse", new Type[] { typeof(string) }), constant);
                                    expressions.Add(Expression.Assign(vars[j], constant));
                                }
                                else
                                {
                                    string value = "";
                                    List<string> param = new List<string>();

                                    //do
                                    //{
                                    //    code[i] = code[i].Replace(".", "");
                                    //    value += code[i];
                                    //    param.Add(code[i + 1].Replace(".", ""));
                                    //    param.Add(code[i]);
                                    //    i = i + 2;
                                    //}
                                    //while ((code[i].Length - code[i].Replace(".", "").Length) / ".".Length <= (code[i + 1].Length - code[i + 1].Replace(".", "").Length) / ".".Length);
                                    //value += code[i].Replace(".", "");
                                    //param.Add(code[i].Replace(".", ""));
                                    //Console.WriteLine(value);
                                    expressions.Add(Expression.Assign(vars[j], Expression.Constant(Int32.Parse(code[i + 2].Replace(".", "")))));
                                }
                                    
                                break;
                            }
                        }
                        break;
                    case "if":
                        string eq = code[i + 1].Replace(".", "");
                        //string left = code[i + 2].Replace(".", "");
                        //string right = code[i + 3].Replace(".", "");
                        //Expression condition = Expression.IsTrue(Constant(left + eq + right));
                        string[] then = new string[50];
                        int w = i + 5;
                        while(code[w].Replace(".", "") != "else")
                        {
                            ++w;
                        }
                        Expression constantL = vars[0];
                        Expression constantR = vars[0];
                        for (int j = 0; j < vars.Count; j++)
                        {
                            if (vars[j].Name == code[i + 2].Replace(".", ""))
                                constantL = vars[j];
                            if (vars[j].Name == code[i + 3].Replace(".", ""))
                                constantR = vars[j];
                        }
                        if (eq == "==")
                            expressions.Add(IfThenElse(Equal(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        else if (eq == "!=")
                            expressions.Add(IfThenElse(NotEqual(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        else if (eq == "<")
                            expressions.Add(IfThenElse(LessThan(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        else if (eq == "<=")
                            expressions.Add(IfThenElse(LessThanOrEqual(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        else if (eq == ">=")
                            expressions.Add(IfThenElse(GreaterThanOrEqual(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        else if (eq == ">")
                            expressions.Add(IfThenElse(GreaterThan(constantL, constantR), Block(GetExpressions(vars, code, i + 2, false)), Block(GetExpressions(vars, code, w + 1, false))));
                        i = 26;
                        break;
                    case "while":
                        
                        break;
                    case "output":
                        ++i;
                        code[i] = code[i].Replace(".", "");
                        int Num;
                        bool isNum = Int32.TryParse(code[i], out Num);
                        if (isNum)
                        {
                            expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Console.WriteLine", new Type[] { typeof(String) }),Expression.Constant(Num.ToString())));
                        }
                        else
                        {
                            code[i] = code[i].Replace(".", "");
                            for (int x = 0; x < vars.Count; x++)
                            {
                                if (code[i] == vars[x].Name)
                                {
                                    expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(String) }), Expression.Constant(vars[x].Name)));
                                    expressions.Add(Expression.Call(null, typeof(Console).GetMethod("Write", new Type[] { typeof(String) }), Expression.Constant(" = ")));
                                    expressions.Add(Expression.Call(null,typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),vars[x]));
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            if (isStart)
                expressions.Add(Expression.Call(typeof(Console).GetMethod("Read")));
            return expressions;
        }
    }
}