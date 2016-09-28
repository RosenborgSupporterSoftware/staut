using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Teller.Core.Classification.Operators
{
    public class OperatorManager
    {
        private static readonly Dictionary<string, Type> OperatorTypes;

        static OperatorManager()
        {
            var operators = Assembly.GetExecutingAssembly()
                                    .GetExportedTypes()
                                    .Where(
                                        t =>
                                            !t.IsInterface && typeof(IClassificationOperator).IsAssignableFrom(t) &&
                                            t.GetCustomAttribute<ClassificationOperatorAttribute>(false) != null)
                                    .ToList();

            OperatorTypes = new Dictionary<string, Type>(operators.Count);
            foreach (var op in operators)
            {
                OperatorTypes.Add(op.GetCustomAttribute<ClassificationOperatorAttribute>(false).OperatorName, op);
            }
        }

        public IClassificationOperator GetOperator(string operatorName)
        {
            if(!OperatorTypes.ContainsKey(operatorName))
                return null;

            return Activator.CreateInstance(OperatorTypes[operatorName]) as IClassificationOperator;
        }

        public IEnumerable<string> GetOperatorNames()
        {
            return OperatorTypes.Keys;
        }
    }
}
