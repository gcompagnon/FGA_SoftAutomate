
using System;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Text;

namespace CommandLine.Utility
{
    /// <summary>
    /// Utilitaire pour gerer les arguments en ligne de commande
    /// --argument=valeur
    /// -argument valeur
    /// 
    /// </summary>
    public class Arguments
    {
        // Variables
        private Dictionary<string, string> Parameters;

        // Constructor
        public Arguments(string[] Args)
        {
            Parameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            Regex Spliter = new Regex(@"^-{1,2}|^/|=|:", RegexOptions.Compiled);
            // expr reg pour enlever les " (on ne retire pas les simples cote ', car utile pour les parametres de tpe -@date='JJ/MM/AAAA'
            Regex Remover = new Regex(@"^[""]?(.*?)[""]?$", RegexOptions.Compiled);
            string Parameter = null;
            string[] Parts;

            // Valid parameters forms:
            // {-,/,--}param{ ,=,:}((")value("))
            // Examples: -param1 value1 --param2 /param3:"Test-:-work" /param4=happy -param5 '--=nice=--'
            foreach (string Txt in Args)
            {
                // Look for new parameters (-,/ or --) and a possible enclosed value (=,:)
                Parts = Spliter.Split(Txt, 3);
                switch (Parts.Length)
                {
                    // Found a value (for the last parameter found (space separator))
                    case 1:
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter))
                            {
                                Parts[0] = Remover.Replace(Parts[0], "$1");
                                Parameters.Add(Parameter, Parts[0]);
                            }
                            Parameter = null;
                        }
                        // else Error: no parameter waiting for a value (skipped)
                        break;
                    // Found just a parameter
                    case 2:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter)) Parameters.Add(Parameter, "true");
                        }
                        if (Parts[0].Length > 0)
                        {
                            Parameters.Add(Parts[0], Parts[1]);
                            Parameter = null;
                        }
                        else
                        {
                            Parameter = Parts[1];
                        }

                        break;
                    // Parameter with enclosed value
                    case 3:
                        // The last parameter is still waiting. With no value, set it to true.
                        if (Parameter != null)
                        {
                            if (!Parameters.ContainsKey(Parameter)) Parameters.Add(Parameter, "true");
                        }
                        Parameter = Parts[1];
                        // Remove possible enclosing characters (")
                        if (!Parameters.ContainsKey(Parameter))
                        {
                            Parts[2] = Remover.Replace(Parts[2], "$1");
                            Parameters.Add(Parameter, Parts[2]);
                        }
                        Parameter = null;
                        break;
                }
            }
            // In case a parameter is still waiting
            if (Parameter != null)
            {
                if (!Parameters.ContainsKey(Parameter)) Parameters.Add(Parameter, "true");
            }
        }

        /// <summary>
        /// retourne la valeur du parametre en respectant la casse Majuscule/minuscule
        /// </summary>
        /// <param name="Param"></param>
        /// <returns>null si inexistant</returns>
        public string this[string Param]
        {
            get
            {
                string value;
                Parameters.TryGetValue(Param, out value);
                return value;
            }
        }

        /// <summary>
        /// retourne l ensemble des parametres qui ne sont pas dans la liste donnée en parametres
        /// </summary>
        /// <param name="givenParameters"></param>
        /// <returns></returns>
        public string[] Intercept(string[] givenParameters)
        {
            bool found;
            System.Collections.ArrayList result = new System.Collections.ArrayList();
            foreach (string param in Parameters.Keys)
            {
                found = false;
                foreach (string givenParam in givenParameters)
                {
                    if (param.Equals(givenParam, StringComparison.CurrentCultureIgnoreCase))
                    {
                        found = true;
                    }
                }

                if (!found)
                {
                    result.Add(param);
                }

            }
            return (string[])result.ToArray(typeof(string));

        }
        /// <summary>
        /// retourne l ensemble des parametres qui debutent avec la chaine de caracteres passes en parametre
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns>nb de clé</returns>
        public int GetStartsWith(string pattern, out string[] keys, out string[] values)
        {
            System.Collections.ArrayList resultKeys = new System.Collections.ArrayList();
            System.Collections.ArrayList resultValues = new System.Collections.ArrayList();
            foreach (string param in Parameters.Keys)
            {
                if (param.StartsWith(pattern))
                {
                    resultKeys.Add(param);
                    resultValues.Add(Parameters[param]);
                }
            }
            keys = (string[])resultKeys.ToArray(typeof(string));
            values = (string[])resultValues.ToArray(typeof(string));
            return resultKeys.Count;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string param in Parameters.Keys)
            {
                sb.AppendLine("Param: >" + param + "< = >" + Parameters[param]+ "<");
            }
            return sb.ToString();
        }
    }

}