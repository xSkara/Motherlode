using System;
using System.Collections.Generic;
using System.Linq;
using Motherlode.Common.Mono.Options;

namespace Motherlode.Common.Console
{
    /// <summary>
    ///     TODO: Finish the idea! May be change the syntax to something attibute based
    ///     (stealed from http://commandline.codeplex.com)
    ///     And of course write some tests for it.
    /// </summary>
    internal class FluentOptions
    {
        #region Constants and Fields

        private readonly OptionSet _optionSet = new OptionSet();
        private readonly List<string> _requiredOptions = new List<string>();

        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private string _helpOptionName;

        #endregion

        #region Constructors and Destructors

        public FluentOptions()
        {
            this.ApplicationName = "<application>";
            this.MaxCountOfExtraOptions = 0;
        }

        #endregion

        #region Public Properties

        public string ApplicationName { get; set; }

        public string Description { get; set; }

        public string[] Extra { get; private set; }

        public int MaxCountOfExtraOptions { get; set; }

        #endregion

        #region Public Methods and Operators

        public void AddBooleanOption(char shortName, string name, string description, bool defaultValue)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            this._values[name] = defaultValue;
            this._optionSet.Add(
                string.Format("{0}|{1}", shortName, name),
                description,
                delegate(string s) { this._values[name] = s != null; });
        }

        public void AddHelpOption(char shortName, string name, string description)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            this._helpOptionName = name;
            this.AddBooleanOption(shortName, name, description, false);
        }

        public void AddHelpOption()
        {
            this.AddHelpOption('h', "help", "Show this message and exit.");
        }

        public void AddStringOption(char shortName, string name, string description, string defaultValue)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            if (defaultValue == null)
            {
                throw new ArgumentNullException("defaultValue");
            }

            this._values[name] = defaultValue;
            this._optionSet.Add(
                string.Format("{0}|{1}=", shortName, name),
                description,
                delegate(string s) { this._values[name] = s != null; });
        }

        public void AddStringOption(char shortName, string name, string description, bool required)
        {
            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (description == null)
            {
                throw new ArgumentNullException("description");
            }

            if (required)
            {
                this._requiredOptions.Add(name);
            }

            this._optionSet.Add(
                string.Format("{0}|{1}=", shortName, name),
                description,
                delegate(string s) { this._values[name] = s != null; });
        }

        public bool? GetBoolean(string optionName)
        {
            if (optionName == null)
            {
                throw new ArgumentNullException("optionName");
            }

            object value;
            if (this._values.TryGetValue(optionName, out value))
            {
                return value as bool?;
            }

            return null;
        }

        public string GetString(string optionName)
        {
            return this.GetValue<string>(optionName);
        }

        public T GetValue<T>(string optionName) where T : class
        {
            if (optionName == null)
            {
                throw new ArgumentNullException("optionName");
            }

            object value;
            if (this._values.TryGetValue(optionName, out value))
            {
                return value as T;
            }

            return null;
        }

        /// <summary>
        ///     Parses the specified args.
        /// </summary>
        /// <param name="args">
        ///     The args.
        /// </param>
        /// <returns>
        ///     True if the parsing has finished succeed.
        /// </returns>
        public bool Parse(string[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            try
            {
                this.Extra = this._optionSet.Parse(args).ToArray();
                if (this.Extra.Length > this.MaxCountOfExtraOptions)
                {
                    this.showHelp(this._optionSet);
                    return false;
                }
            }
            catch (OptionException e)
            {
                this.syntaxError(e.Message);
                return false;
            }

            if (this._helpOptionName != null &&
                this.GetBoolean(this._helpOptionName) == true)
            {
                this.showHelp(this._optionSet);
                return false;
            }

            if (!this._requiredOptions.All(n => this._values.ContainsKey(n)))
            {
                this.syntaxError("Some required options are missing.");
                return false;
            }

            return true;
        }

        #endregion

        #region Methods

        private void showHelp(OptionSet p)
        {
            System.Console.WriteLine("Usage: {0} OPTIONS", this.ApplicationName);

            if (!string.IsNullOrEmpty(this.Description))
            {
                System.Console.WriteLine(this.Description);
            }

            System.Console.WriteLine();
            System.Console.WriteLine("Options:");
            p.WriteOptionDescriptions(System.Console.Out);
        }

        private void syntaxError(string message)
        {
            System.Console.WriteLine("Syntax error: ");
            System.Console.WriteLine(message);
            if (this._helpOptionName != null)
            {
                System.Console.WriteLine(
                    string.Format("Try '{0} --{1}' for more information.", this.ApplicationName, this._helpOptionName));
            }
        }

        #endregion

        /*var p = new OptionSet()
                        {
                            // Definition of global properties
                            {
                                "D:", "Predefine a global property with an value.",
                                (m, v) =>
                                {
                                    if (m == null)
                                        throw new OptionException("Missing macro name for option -D.", "-D");
                                    globalProperties.Add(new Property(m, v));
                                }
                                },
                            {
                                "d={-->}{=>}", "Alternate global property syntax.",
                                (m, v) => globalProperties.Add(new Property(m, v))
                                },
                        };

        }*/
    }
}
