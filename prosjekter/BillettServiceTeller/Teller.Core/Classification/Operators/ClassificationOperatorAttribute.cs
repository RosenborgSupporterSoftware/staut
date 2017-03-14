using System;

namespace Teller.Core.Classification.Operators
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ClassificationOperatorAttribute : Attribute
    {
        public ClassificationOperatorAttribute(string operatorName)
        {
            OperatorName = operatorName;
        }

        public string OperatorName { get; }
    }
}
