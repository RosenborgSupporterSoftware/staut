using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Teller.Core.Classification.Operators;
using Teller.Core.Entities;

namespace Teller.Core.Classification
{
    /// <summary>
    /// En klasse som representerer en regel for klassifisering av en setekode
    /// </summary>
    public class SeatClassificationRule
    {
        #region Fields

        private IClassificationOperator _operator;
        private string _operatorName;
        private int _operatorValue;

        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the optional rule name for this rule
        /// </summary>
        public string RuleName { get; set; }

        /// <summary>
        /// Gets or sets any notes for this rule, such as justification, docs, whatever
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the order in which this rule is run - lower values means being executed earlier
        /// </summary>
        public long Order { get; set; }

        /// <summary>
        /// Gets or sets the field examined by this rule
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public ClassificationRuleField Field { get; set; }

        /// <summary>
        /// Gets or sets the name of the operator used by this rule to check the Field
        /// </summary>
        public string Operator
        {
            get { return _operatorName; }
            set
            {
                if (_operatorName == value)
                    return;
                _operatorName = value;
                SetupOperator();
            }
        }

        /// <summary>
        /// Gets or sets the value to use with the Operator to check against the provided code
        /// </summary>
        public string Value
        {
            get { return _operatorValue.ToString(); }
            set
            {
                int val;
                if (int.TryParse(value, out val))
                {
                    _operatorValue = val;
                    SetupOperator();
                }
            }

        }

        /// <summary>
        /// Gets or sets the status resulting from a match with this rule
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        public SeatStatus Status { get; set; }

        #endregion

        /// <summary>
        /// Checks to see if the provided EttCode object matches this SeatClassificationRule
        /// </summary>
        /// <param name="code">The EttCode object to test</param>
        /// <returns>True if the EttCode matches, False otherwise</returns>
        public bool IsMatch(EttCode code)
        {
            if (code == null)
                return false;

            if (_operator == null)
                SetupOperator();
            if (_operator == null)
                return false;

            var codeValue = ExtractDesiredValueFromCode(code);

            return _operator.OperatorIsMatch(codeValue);
        }

        private void SetupOperator()
        {
            _operator = new OperatorManager().GetOperator(Operator);
            _operator?.SetParameter(Value);
        }

        private int ExtractDesiredValueFromCode(EttCode code)
        {
            switch (Field)
            {
                case ClassificationRuleField.BaseType:
                    return code.BaseType;
                case ClassificationRuleField.Code:
                    return code.CodeValue;
                case ClassificationRuleField.SeatFlags:
                    return code.SeatFlags;
                case ClassificationRuleField.QualifierBits:
                    return code.QualifierBits;
                case ClassificationRuleField.JubaFlags:
                    return code.JubaFlags;
                default:
                    return code.CodeValue;
            }
        }

        protected bool Equals(SeatClassificationRule other)
        {
            return string.Equals(_operatorName, other._operatorName) && _operatorValue == other._operatorValue &&
                   string.Equals(RuleName, other.RuleName) && string.Equals(Notes, other.Notes) && Order == other.Order &&
                   Field == other.Field && Status == other.Status;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((SeatClassificationRule) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = _operatorName?.GetHashCode() ?? 0;
                hashCode = (hashCode*397) ^ _operatorValue;
                hashCode = (hashCode*397) ^ (RuleName?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ (Notes?.GetHashCode() ?? 0);
                hashCode = (hashCode*397) ^ Order.GetHashCode();
                hashCode = (hashCode*397) ^ (int) Field;
                hashCode = (hashCode*397) ^ (int) Status;
                return hashCode;
            }
        }
    }
}
