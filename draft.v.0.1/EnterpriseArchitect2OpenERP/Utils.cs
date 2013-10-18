using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Text.RegularExpressions;

namespace EnterpriseArchitect2OpenERP
{
    public enum FunctionUsage
    {
        Field,
        Init,
        Classic
    }

    public class FunctionModel
    {
        public string Content { get; set; }

        public List<string> ListOfVariables { get; set; }

        public bool Default { get; set; }

        public FunctionUsage Usage { get; set; }

        public FunctionModel(FunctionUsage functionUsage)
        {
            Content = "";
            ListOfVariables = new List<string>();
            Default = false;
            Usage = functionUsage;
        }

        public void ScanAllVariables()
        {
            MatchCollection matches = Regex.Matches(Content, @"\$\{[^\}]*\}");
            foreach (Match match in matches)
            {
                if (!ListOfVariables.Contains(match.Value))
                {
                    ListOfVariables.Add(match.Value);
                }
            }
        }

        public void ReplaceValues(Dictionary<string, string> values, bool entoure = true)
        {
            foreach (KeyValuePair<string, string> keyvalue in values)
            {
                if (entoure)
                {
                    Content = Content.Replace("${" + keyvalue.Key + "}", keyvalue.Value) + "\r\n\t";
                }
                else
                {
                    Content = Content.Replace(keyvalue.Key, keyvalue.Value) + "\r\n\t";
                }
            }
        }

        public void ReplaceValues(List<string> values)
        {
            Dictionary<string, string> vals = new Dictionary<string, string>();
            for (int i = 0; i < values.Count; i++)
            {
                vals.Add(ListOfVariables[i], values[i]);
            }

            ReplaceValues(vals, false);
        }

        public void ReplaceValues(params string[] values)
        {
            List<string> vals = new List<string>();
            for (int i = 0; i < values.Length; i++)
            {
                vals.Add(values[i]);
            }

            ReplaceValues(vals);
        }

        public override string ToString()
        {
            return Content;
        }
    }

    public class Utils
    {
        public static string replaceValues(string value, Dictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> keyvalue in values)
            {
                value = value.Replace("${" + keyvalue.Key + "}", keyvalue.Value);
            }

            return value;
        }

        public static string replaceSimpleValues(string value, Dictionary<string, string> values)
        {
            foreach (KeyValuePair<string, string> keyvalue in values)
            {
                value = value.Replace(keyvalue.Key, keyvalue.Value);
            }

            return value;
        }

        public static string firstCharUpper(string value)
        {
            return value.Substring(0, 1).ToUpper() + value.Substring(1);
        }

        public static void MakeFile(string file)
        {
            MakeFile(file, null);
        }

        public static void CopyIcon(string directory)
        {
            File.Copy(PathData.Models + "/icon-hover.png", directory + "/icon.png");
        }

        public static void CopyIcons(string directory)
        {
            File.Copy(PathData.Models + "/icon.png", directory + "/icon.png");
            File.Copy(PathData.Models + "/icon-hover.png", directory + "/icon-hover.png");
        }

        public static FileInfo MakeFileFromModel(string model, string file, Dictionary<string, string> values)
        {
            StreamReader __model__ = File.OpenText(PathData.Models + "/" + model);

            string content = __model__.ReadToEnd();
            __model__.Close();

            if (values != null)
            {
                content = Utils.replaceValues(content, values);

                string[] lines = content.Split(new char[] { '\n' });

                List<string> lignes = new List<string>();
                for (int i = 0; i < lines.Length; i++)
                {
                    if (!lines[i].Contains("# delete this line"))
                    {
                        lignes.Add(lines[i].TrimEnd(new char[] { '\r' }));
                    }
                }

                content = string.Join(Environment.NewLine, lignes.ToArray());
            }

            return MakeFile(file, content);
        }

        public static FileInfo MakeFileFromModel(string model, string file)
        {
            return MakeFileFromModel(model, file, null);
        }

        public static FileInfo MakeFile(string file, string content)
        {
            if (File.Exists(file)) File.Delete(file);

            if (content == null)
            {
                File.Create(file);
            }
            else
            {
                StreamWriter __file__ = File.CreateText(file);
                __file__.Write(content);
                __file__.Flush();
                __file__.Close();
            }

            return new FileInfo(file);
        }

        public static string CorrectType(string type)
        {
            string Typage = type.ToLower();

            if (Program.ProjectData != null)
            {
                Typage = Program.ProjectData.CheckSelection(Typage);
            }

            Typage = (Typage.Equals("bool")) ? "boolean" : Typage;
            Typage = (Typage.Equals("byte") || Typage.Equals("int") || Typage.Equals("long")) ? "integer" : Typage;
            Typage = (Typage.Equals("double")) ? "float" : Typage;

            return Typage;
        }

        public static string TrueString(string oprator)
        {
            string result = oprator;

            result = result.Replace("&gt;", ">");
            result = result.Replace("&lt;", "<");

            return result;
        }

        public static string CorrectReturn(string type)
        {
            string result = "''";

            result = (type.Equals("boolean")) ? "True" : result;
            result = (type.Equals("integer")) ? "0" : result;
            result = (type.Equals("void")) ? "" : result;

            return result;
        }

        public static bool IsNumeric(string value)
        {
            int result = 0;
            return int.TryParse(value, out result);
        }

        public static object GetMin(string value)
        {
            value = value.Replace("..", ".");
            string[] multi = value.Split(new char[] { '.' });

            if (multi.Length > 0)
            {
                return multi[0];
            }

            return string.Empty;
        }

        public static object GetMax(string value)
        {
            value = value.Replace("..", ".");
            string[] multi = value.Split(new char[] { '.' });

            if (multi.Length == 1)
            {
                return ((multi[0].Equals("1")) || (multi[0].Equals("0")))? multi[0] : "*";
            }
            else if (multi.Length == 2)
            {
                return multi[1]; // ((multi[1].Equals("1")) || (multi[1] == "1")) ? multi[1] : "*";
            }

            return string.Empty;
        }

        public static string ReplaceSpecialChar(string text)
        {
            Dictionary<string, string> dico = new Dictionary<string, string>();
            
            dico.Add("ê", "e");
            dico.Add("Ê", "E");
            dico.Add("é", "e");
            dico.Add("É", "E");
            dico.Add("è", "e");
            dico.Add("È", "E");
            dico.Add("ë", "e");
            dico.Add("Ë", "E");

            dico.Add("à", "a");
            dico.Add("À", "A");
            dico.Add("â", "a");
            dico.Add("Â", "A");
            dico.Add("ä", "a");
            dico.Add("Ä", "A");

            dico.Add("ô", "o");
            dico.Add("Ô", "O");
            dico.Add("ö", "o");
            dico.Add("Ö", "O");
            dico.Add("ò", "o");
            dico.Add("Ò", "O");

            dico.Add("ì", "i");
            dico.Add("Ì", "I");
            dico.Add("î", "i");
            dico.Add("Î", "I");
            dico.Add("ï", "i");
            dico.Add("Ï", "I");

            dico.Add("ç", "c");

            dico.Add("ù", "u");
            dico.Add("Ù", "U");
            dico.Add("û", "u");
            dico.Add("Û", "U");
            dico.Add("ü", "u");
            dico.Add("Ü", "U");

            string value = replaceSimpleValues(text, dico);

            return value;
        }

        public static string ClassicName(string name)
        {
            string value = name.ToLower();

            Dictionary<string, string> dico = new Dictionary<string, string>();
            dico.Add(" ", "_");
            dico.Add("\\", "_");
            dico.Add("/", "_");

            value = replaceSimpleValues(value, dico);

            value = ReplaceSpecialChar(value);

            return value;
        }

        public static string GetFunctionName(string function)
        {
            string[] split = function.Split(new char[] { '(' });

            return split[0].ToLower().Trim();
        }

        public static List<string> GetFunctionArguments(string function)
        {
            string[] split = function.Split(new char[] { '(' });
            string args = split[1].Substring(0, split[1].Length - 1);

            string[] arguments = args.Split(new char[] { ',' });

            List<string> result = new List<string>();
            for(int i = 0; i < arguments.Length; i++)
            {
                result.Add(arguments[i].Trim());
            }

            return result;
        }

        private static Dictionary<string, string> functionsModels = null;
        public static FunctionModel GetFunctionModel(string function, FunctionUsage functionUsage)
        {
            if (functionsModels == null)
            {
                functionsModels = new Dictionary<string, string>();

                StreamReader __model__ = File.OpenText(PathData.Models + "/openerp_scripts.py.model");

                string content = __model__.ReadToEnd();
                __model__.Close();

                string[] functs = content.Trim().Split(new string[] { "-- new script --" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (string funct in functs)
                {
                    string[] lines = funct.Trim().Split(new char[] { '\n' });

                    string name = (lines[0].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))[1].Trim();
                    string usage = (lines[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries))[1].Trim();

                    string cont = "";
                    for (int i = 2; i < lines.Length; i++)
                    {
                        cont += lines[i];
                    }

                    functionsModels[name + ":" + usage] = cont.Trim();
                }
            }

            FunctionModel functionModel = new FunctionModel(functionUsage);

            string usefor = functionUsage.ToString().ToLower();

            if (functionsModels.ContainsKey(function + ":" + usefor))
            {
                functionModel.Content = functionsModels[function + ":" + usefor];
                functionModel.ScanAllVariables();

                return functionModel;
            }
            else
            {
                functionModel.Content = functionsModels["empty_function:" + usefor];
                functionModel.ScanAllVariables();
                functionModel.Default = true;

                return functionModel;
            }
        }

        public static List<string> getExtra(XmlNode xmlNode, string name, string defaultValue)
        {
            List<string> returnList = new List<string>();

            bool exists = false;
            foreach (XmlNode node in xmlNode.ChildNodes)
            {
                if (node.Name.Equals("tags"))
                {
                    exists = true;
                }
            }

            if (exists)
            {
                foreach (XmlNode tagsNode in xmlNode["tags"].ChildNodes)
                {
                    if (tagsNode.Attributes["name"].Value.Equals(name))
                    {
                        returnList.Add(tagsNode.Attributes["value"].Value);
                    }
                }
            }

            if (returnList.Count == 0) returnList.Add(defaultValue);

            return returnList;
        }

        public static Dictionary<string, string> FunctionArgs(List<string> list)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (string arg in list)
            {
                string[] ops = arg.Trim().Split(new char[] { '=' });
                if (ops.Length == 2)
                {
                    result.Add(ops[0], ops[1]);
                }
                else
                {
                    result.Add(ops[0], ops[0]);
                }
            }

            return result;
        }

        public static string GetArg(Dictionary<string, string> list, string arg, string def)
        {
            if (list.ContainsKey(arg))
            {
                return list[arg];
            }

            return def;
        }
    }
}
