using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace Teller.Core.Classification
{
    /// <summary>
    /// A class used to load and save a file containing SeatClassificationRule objects
    /// </summary>
    public class SeatClassificationFile
    {
        private readonly List<SeatClassificationRule> _rules = new List<SeatClassificationRule>();
        private bool _changed;

        public IEnumerable<SeatClassificationRule> Rules => _rules;

        /// <summary>
        /// Gets a value indicating whether the SeatClassificationFile has been changed or not
        /// </summary>
        public bool Changed => _changed;

        private static string ConfigurationFolder
        {
            get
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RosenborgSupporterSoftware", "STAut");
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                return path;
            }
        }

        /// <summary>
        /// Gets the path for the system rule file
        /// </summary>
        public static string SystemRuleFile => Path.Combine(ConfigurationFolder, "SystemRules.json");

        /// <summary>
        /// Gets the path for the experimental rule file
        /// </summary>
        public static string ExperimentalFile => Path.Combine(ConfigurationFolder, "ExperimentalRules.json");

        /// <summary>
        /// Loads a file containing SeatClassificationRule objects from disk
        /// </summary>
        /// <param name="path">The file to load</param>
        /// <returns>A populated SeatClassificationFile object</returns>
        public static SeatClassificationFile Load(string path)
        {
            if (!File.Exists(path))
                return new SeatClassificationFile();

            var json = File.ReadAllText(path);

            var doc = JsonConvert.DeserializeObject<SeatClassificationFile>(json);

            return doc;
        }

        public void Save(string path)
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);

            File.WriteAllText(path, json);

            _changed = false;
        }

        /// <summary>
        /// Add a SeatClassificationRule to the file
        /// </summary>
        /// <param name="rule"></param>
        public void Add(SeatClassificationRule rule)
        {
            if (!_rules.Contains(rule))
            {
                _rules.Add(rule);
                _changed = true;
            }
        }

        /// <summary>
        /// Remove a SeatClassificationRule from the file
        /// </summary>
        /// <param name="rule"></param>
        public void Remove(SeatClassificationRule rule)
        {
            if (_rules.Remove(rule))
                _changed = true;
        }

        public bool Contains(SeatClassificationRule rule)
        {
            return _rules.Contains(rule);
        }
    }
}
