using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xNet;

namespace Semantic
{
    class Program
    {

        struct id
        {
            public string type;
            public string id_;
            public string param;
            public string eq;
        }
        static List<id> WriteId(List<string> parseData)
        {
            List<id> idData = new List<id>();
            bool isConst = false;
            if (parseData[0] == "<const")
                isConst = true;
            for (int i = 1; i < parseData.Count; i++)
            {
                if (parseData[i].Contains("output"))
                {
                    id m_id = new id();
                    m_id.type = parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 1].Replace(".", "")).Substring(".", " "));
                    m_id.eq = parseData[i + 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " "));
                    //Console.WriteLine(parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 1].Replace(".", "")).Substring(".", " ")) + " = " + parseData[i + 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")));
                    idData.Add(m_id);
                }
                if (parseData[i].Contains("<main"))
                    isConst = false;
                if (parseData[i - 1].Split('.').Length == parseData[i].Split('.').Length)
                {
                    if (isConst)
                    {
                        id id1 = new id();
                        id1.type = "const";
                        id1.id_ = parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " "));
                        id1.param = "=";
                        id1.eq = parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " "));
                        //Console.WriteLine("const <" + parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " ")) + "> = " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")));
                        idData.Add(id1);
                    }
                    else
                    {
                        if (parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) != "=" && parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) != "array" && parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) != "int")
                        {
                            id id1 = new id();
                            id1.type = "if";
                            id1.id_ = parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " "));
                            id1.param = parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " "));
                            id1.eq = parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " "));
                            //Console.WriteLine("if <" + parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " ")) + " " + parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) + "> " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")));
                            idData.Add(id1);
                        }
                        else
                        {
                            if (parseData[i - 3].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 4].Replace(".", "")).Substring(".", " ")) == "<for")
                            {
                                id id1 = new id();
                                id1.type = "for";
                                id1.id_ = parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " "));
                                id1.param = "= " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")) + " " + parseData[i + 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i + 1].Replace(".", "")).Substring(".", " "));
                                id1.eq = parseData[i + 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i + 2].Replace(".", "")).Substring(".", " "));
                                 idData.Add(id1);
                                //Console.WriteLine("for <" + parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " ")) + "> = " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")) + " " + parseData[i + 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i + 1].Replace(".", "")).Substring(".", " ")) + " " + parseData[i + 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i + 2].Replace(".", "")).Substring(".", " ")));
                            }
                            else
                            {
                                id id1 = new id();
                                id1.type = parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " "));
                                id1.id_ = parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " "));
                                id1.param = "= " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")) + " " + parseData[i + 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i + 1].Replace(".", "")).Substring(".", " "));
                                id1.eq = parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " "));
                                idData.Add(id1);
                                //Console.WriteLine(parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) + " <" + parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " ")) + "> = " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")));
                                idData.Add(id1);
                                //idData.Add(parseData[i - 2].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 3].Replace(".", "")).Substring(".", " ")) + " <" + parseData[i - 1].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i - 2].Replace(".", "")).Substring(".", " ")) + "> = " + parseData[i].Replace(".", "").Replace("IDENTIFICATOR", ("." + parseData[i].Replace(".", "")).Substring(".", " ")));
                            }
                        }

                    }
                }
            }
            return idData;
        }

        static int Main(string[] args)
        {
            try { 
            List<string> parseData = new List<string>();
            using (StreamReader file = new StreamReader("sint.txt"))
            {
                while(!file.EndOfStream)
                {
                    parseData.Add(file.ReadLine());
                }
            }
            List<id> idData = WriteId(parseData);
                for (int i = 0; i < idData.Count; i++)
                {
                    //1ое правило
                    if (idData[i].type == "array")
                    {
                        //Console.WriteLine("1");
                        if (Int32.Parse(idData[i].id_.Substring("[", "]")) < 1)
                        {
                            throw new Exception(idData[i].id_ + " range must be > 1");
                        }
                    }
                    //2ое правило
                    if (idData[i].type == "array")
                    {
                        int range = Int32.Parse(idData[i].id_.Substring("[", "]"));
                        string[] data = idData[i].eq.Split(',');
                        if (range != data.Length)
                            throw new Exception(idData[i].id_ + ". Array size is not equal to the number of its elements");
                    }
                    //3ее правило
                    if (idData[i].type == "const")
                    {
                        for (int x = 0; x < idData.Count; x++)
                        {
                            if (idData[x].type == "=" && idData[i].id_ == idData[x].id_)
                                throw new Exception(idData[i].id_ + ". Constants can not be changed");
                        }
                    }
                    //4ое правило
                    if (idData[i].type == "int")
                    {
                        try
                        {
                            Int32.Parse(idData[i].eq);
                        }
                        catch
                        {
                            bool isOk = true;
                            if (!idData[i].eq.Contains("["))
                                for (int x = 0; x < i; x++)
                                {
                                    if (idData[i].eq == idData[x].id_.Replace("[" + idData[x].id_.Substring("[", "]") + "]", ""))
                                    {
                                        if (idData[x].id_ != "int")
                                            isOk = false;
                                    }
                                }
                            if (!isOk)
                                throw new Exception(idData[i].id_ + " can't equal array");
                        }
                    }
                    //5ое правило -?
                    //6ое правило
                    if (idData[i].type == "for")
                    {
                        if (idData[i].param.Contains("down"))
                        {
                            string data = idData[i].param + "#";
                            int min = Int32.Parse(data.Substring("= ", " down"));
                            int max = Int32.Parse(idData[i].eq);
                            if (min < max)
                                throw new Exception(idData[i].type + " downto " + min.ToString() + " > " + max.ToString());
                        }
                        else
                        {
                            string data = idData[i].param;
                            int min = Int32.Parse(data.Substring("= ", " to"));
                            int max = Int32.Parse(idData[i].eq);
                            if (min > max)
                                throw new Exception(idData[i].type + " to " + min.ToString() + " < " + max.ToString());
                        }
                    }
                    //7ое правило
                    if (idData[i].type == "=" && !idData[i].id_.Contains("["))
                    {
                        bool isOk = false;
                        for (int x = 0; x < i; x++)
                        {
                            if (idData[i].id_ == idData[x].id_)
                                isOk = true;
                        }
                        if (!isOk)
                            throw new Exception(idData[i].id_ + " not declared");
                    }
                }
                Console.WriteLine("Success");
                return 0;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }
    }
}
