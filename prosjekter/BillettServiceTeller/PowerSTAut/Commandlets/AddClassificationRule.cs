using System;
using System.Management.Automation;
using Teller.Core;
using Teller.Core.Classification;

namespace PowerSTAut.Commandlets
{
    [Cmdlet(VerbsCommon.Add, "ClassificationRule")]
    public class AddClassificationRule : PSCmdlet
    {
        #region Parameters

        [Parameter(Mandatory = true)]
        public string RuleName { get; set; }

        [Parameter]
        public string Notes { get; set; }

        [Parameter]
        public string Index { get; set; }

        [Parameter(Mandatory = true)]
        public ClassificationRuleField Field { get; set; }

        [Parameter(Mandatory = true)]
        public string Operator { get; set; }

        [Parameter(Mandatory = true)]
        public string Value { get; set; }

        [Parameter(Mandatory = true)]
        public SeatStatus Status { get; set; }

        [Parameter]
        public SwitchParameter Production { get { return _production; } set { _production = value; } }
        private bool _production;

        protected override void ProcessRecord()
        {
            base.ProcessRecord();

            long index;
            if (!long.TryParse(Index, out index))
                index = -1;

            // TODO: Validate Operator

            var newRule = new SeatClassificationRule
            {
                RuleName = RuleName,
                Notes = Notes,
                Order = index,
                Field = Field,
                Operator = Operator,
                Value = Value,
                Status = Status
            };

            var systemFile = SeatClassificationFile.Load(SeatClassificationFile.SystemRuleFile);
            var experimentalFile = SeatClassificationFile.Load(SeatClassificationFile.ExperimentalFile);
            var engine = new SeatClassificationEngine(systemFile, experimentalFile);
            engine.AddRule(newRule, !_production);
            
            systemFile.Save(SeatClassificationFile.SystemRuleFile);
            experimentalFile.Save(SeatClassificationFile.ExperimentalFile);
        }

        #endregion
    }
}
