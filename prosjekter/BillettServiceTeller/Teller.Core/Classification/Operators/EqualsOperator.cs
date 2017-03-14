namespace Teller.Core.Classification.Operators
{
    [ClassificationOperator("Equals")]
    public class EqualsOperator : IClassificationOperator
    {
        private int _parameter;

        public bool OperatorIsMatch(int data)
        {
            return data == _parameter;
        }

        public void SetParameter(string param)
        {
            if (!int.TryParse(param, out _parameter))
                _parameter = 0;
        }
    }
}
