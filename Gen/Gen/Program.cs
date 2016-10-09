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
                }
                ++i;
            }
            List<ParameterExpression> list_of_vars = new List<ParameterExpression>();
            BlockExpression blockExpression = Block(vars, GetExpressions(vars, File.ReadAllLines("./sint.txt")));
            Lambda<Action>(blockExpression).CompileToMethod(methodBuilder);
            assemblyBuilder.SetEntryPoint(methodBuilder);
            typeBuilder.CreateType();
            assemblyBuilder.Save(assemblyName.Name + ".exe");
        }

        private static List<Expression> GetExpressions(List<ParameterExpression> vars, string[] code)
        {
            List<Expression> expressions = new List<Expression>
            {
            };

            for (int i = 0; i < vars.Count; i++)
            {
                expressions.Add(vars[i]);
            }

            Dictionary<string, int> dict = new Dictionary<string, int>();
            for (int i = 0; i < code.Length; i++)
            {
                
                string item = code[i];
                item = item.Replace(".", "");
                switch (item)
                {
                    case "array":
                        code[i + 1] = code[i + 1].Replace(".", "");
                        code[i + 2] = code[i + 2].Replace(".", "");
                        dict.Add(code[i + 1], Int32.Parse(code[i + 2]));
                        
                        expressions.Add(Expression.ArrayAccess(
                            Expression.Parameter(typeof(int[]), "Array"), Expression.Parameter(typeof(int), "Index")
                        ));
                        break;
                    case "=":
                        code[i + 1] = code[i + 1].Replace(".", "");
                        code[i + 2] = code[i + 2].Replace(".", "");

                        for (int j = 0; j < vars.Count; j++)
                        {
                            if (code[i + 1] == vars[j].Name)
                            {
                                if (code[i + 2] == "input")
                                {
                                    Expression constant = Expression.Call(typeof(Console).GetMethod("ReadLine"));
                                    constant = Expression.Call(typeof(Int32).GetMethod("Parse", new Type[] { typeof(string) }), constant);
                                    expressions.Add(Expression.Assign(vars[j], constant));
                                }
                                else
                                    expressions.Add(Expression.Assign(vars[j], Expression.Constant(Int32.Parse(code[i + 2]))));
                                break;
                            }
                        }
                        break;
                    case "output":
                        ++i;
                        code[i] = code[i].Replace(".", "");
                        int Num;
                        bool isNum = Int32.TryParse(code[i], out Num);
                        if (isNum)
                        {
                            expressions.Add(Expression.Call(
                            null,
                            typeof(Console).GetMethod("Console.WriteLine", new Type[] { typeof(String) }),
                            Expression.Constant(Num.ToString())
                            ));
                        }
                        else
                        {
                            code[i] = code[i].Replace(".", "");
                            for (int j = 0; j < vars.Count; j++)
                            {
                                if (code[i] == vars[j].Name)
                                {
                                    expressions.Add(Expression.Call(null,typeof(Console).GetMethod("WriteLine", new Type[] { typeof(int) }),vars[j]));
                                    break;
                                }
                            }
                        }
                        break;
                    default:
                        break;
                }
            }

            expressions.Add(Expression.Call(typeof(Console).GetMethod("Read")));
            return expressions;
        }
    }
}