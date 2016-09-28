using System.Collections.Generic;
using System.Linq;
using Teller.Core.Entities;

namespace Teller.Core.Classification
{
    /// <summary>
    /// A class used to classify EttCodes 
    /// </summary>
    public class SeatClassificationEngine
    {
        #region Private fields

        private readonly SeatClassificationFile _systemRuleFile, _experimentalRuleFile;

        private List<SeatClassificationRule> _currentActiveRules;
        private readonly Dictionary<string, SeatClassificationRule> _cache = new Dictionary<string, SeatClassificationRule>();

        private bool _useExperimental;

        #endregion

        #region Constructor

        public SeatClassificationEngine(SeatClassificationFile systemRuleFile = null, SeatClassificationFile experimentalRuleFile = null)
        {
            _systemRuleFile = systemRuleFile ?? new SeatClassificationFile();
            _experimentalRuleFile = experimentalRuleFile ?? new SeatClassificationFile();

            SetupCurrentRules();
        }

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets a value indicating whether to use the experimental set of rules for matching or not
        /// </summary>
        public bool UseExperimentalRules
        {
            get
            {
                return _useExperimental;
            }
            set
            {
                if(_useExperimental==value)
                    return;
                _useExperimental = value;
                SetupCurrentRules();
            }
        }

        /// <summary>
        /// Gets a sequence of all rules in the engine
        /// </summary>
        public IEnumerable<SeatClassificationRule> AllRules => _currentActiveRules;

        #endregion
        
        /// <summary>
        /// Classifies a single EttCode using the defined ruleset
        /// </summary>
        /// <param name="code">The EttCode to classify</param>
        /// <returns>A SeatStatus for the match, or SeatStatus.Unknown in the case of no rules matching</returns>
        public SeatStatus Classify(EttCode code)
        {
            var match = GetMatchingRule(code);

            return match?.Status ?? SeatStatus.Unknown;
        }

        public SeatClassificationRule GetMatchingRule(EttCode code)
        {
            if (_cache.ContainsKey(code.Code))
                return _cache[code.Code];

            var matchingRule = AllRules.FirstOrDefault(r => r.IsMatch(code));
            if(matchingRule!=null)
                _cache.Add(code.Code, matchingRule);

            return matchingRule;
        }

        /// <summary>
        /// Adds a SeatClassificationRule to the rule set
        /// </summary>
        /// <param name="rule">The rule to add</param>
        /// <param name="experimental">Indicates if this rule should be considered experimental or not</param>
        public void AddRule(SeatClassificationRule rule, bool experimental = true)
        {
            var list = experimental ? _experimentalRuleFile : _systemRuleFile;

            list.Add(rule);
            SetupCurrentRules();
        }

        /// <summary>
        /// Removes the given SeatClassificationRule object from the rule set
        /// </summary>
        /// <param name="rule">The rule object to remove</param>
        public void RemoveRule(SeatClassificationRule rule)
        {
            var change = false;

            if (_systemRuleFile.Contains(rule))
            {
                _systemRuleFile.Remove(rule);
                change = true;
            }
            if (_experimentalRuleFile.Contains(rule))
            {
                _experimentalRuleFile.Remove(rule);
                change = true;
            }

            if (change)
            {
                SetupCurrentRules();
            }
        }

        public void ToggleExperimental(SeatClassificationRule rule)
        {
            if (_systemRuleFile.Contains(rule))
            {
                _systemRuleFile.Remove(rule);
                _experimentalRuleFile.Add(rule);
            }
            else if (_experimentalRuleFile.Contains(rule))
            {
                _experimentalRuleFile.Remove(rule);
                _systemRuleFile.Add(rule);
            }
            SetupCurrentRules();
        }

        private void SetupCurrentRules()
        {
            _currentActiveRules =
                (_useExperimental ? _systemRuleFile.Rules.Concat(_experimentalRuleFile.Rules) : _systemRuleFile.Rules)
                    .OrderBy(r => r.Order)
                    .ToList();
            _cache.Clear();
        }
    }
}
