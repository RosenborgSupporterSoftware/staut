namespace Teller.Core.Classification
{
    /// <summary>
    /// Enumerates the fields/parts of the Ett code that a rule can check against
    /// </summary>
    public enum ClassificationRuleField
    {
        QualifierBits,
        SeatFlags,
        BaseType,
        Code,
        JubaFlags
    }
}
