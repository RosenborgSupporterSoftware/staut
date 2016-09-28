using System;

namespace Teller.Core.Classification.Operators
{
    [ClassificationOperator("GreaterThan")]
    public class GreaterThanOperator : IClassificationOperator
    {
        private int _parameter;

        public bool OperatorIsMatch(int data)
        {
            return data > _parameter;
        }

        public void SetParameter(string param)
        {
            if (!int.TryParse(param, out _parameter))
                _parameter = 0;
        }
    }
}
