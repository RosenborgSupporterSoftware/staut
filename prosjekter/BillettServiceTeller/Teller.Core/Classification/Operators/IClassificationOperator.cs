namespace Teller.Core.Classification.Operators
{
    /// <summary>
    /// Et interface som definerer matche-operasjoner mot Ett-koder
    /// </summary>
    public interface IClassificationOperator
    {
        bool OperatorIsMatch(int data);

        void SetParameter(string param);
    }
}
