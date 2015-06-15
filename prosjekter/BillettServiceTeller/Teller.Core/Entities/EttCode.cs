using System;
using System.Globalization;

namespace Teller.Core.Entities
{
    /// <summary>
    /// En klasse som representerer en TicketMaster ETT-kode
    /// </summary>
    public class EttCode
    {
        #region Fields

        private readonly int _qualifierBits;
        private readonly int _seatFlags;
        private readonly int _baseType;

        #endregion

        /// <summary>
        /// Få Qualifier Bits som en hex-string
        /// </summary>
        public string QualifierBitsHex { get { return String.Format("{0:X}", _qualifierBits); } }

        /// <summary>
        /// Få Qualifier Bits som en int
        /// </summary>
        public int QualifierBits { get { return _qualifierBits; } }

        /// <summary>
        /// Få Seat Flags som en hex-string
        /// </summary>
        public string SeatFlagsHex { get { return String.Format("{0:X2}", _seatFlags); } }

        /// <summary>
        /// Få Seat Flags som en int
        /// </summary>
        public int SeatFlags { get { return _seatFlags; } }

        /// <summary>
        /// Få Base Type som en hex-string
        /// </summary>
        public string BaseTypeHex { get { return String.Format("{0:X2}", _baseType); } }

        /// <summary>
        /// Få Base Type som en int
        /// </summary>
        public int BaseType { get { return _baseType; } }

        /// <summary>
        /// Få hele ETT-koden som en hex-string
        /// </summary>
        public string Code { get { return String.Format("{0:X8}{1:X2}{2:X2}", _qualifierBits, _seatFlags, _baseType); } }

        public EttCode(string ettCode)
        {
            if(String.IsNullOrWhiteSpace(ettCode))
                throw new ArgumentException("ettCode in wrong format: " + ettCode);

            ettCode = FormatEttCode(ettCode);

            _qualifierBits = int.Parse(ettCode.Substring(0, 8), NumberStyles.HexNumber);
            _seatFlags = int.Parse(ettCode.Substring(8, 2), NumberStyles.HexNumber);
            _baseType = int.Parse(ettCode.Substring(10, 2), NumberStyles.HexNumber);
        }

        private static string FormatEttCode(string ettCode)
        {
            int val;
            if (!int.TryParse(ettCode, NumberStyles.HexNumber, null, out val))
                return "invalid";

            return String.Format("{0:X12}", val);
        }

        #region Overrides and implicit operators

        public override string ToString()
        {
            return Code;
        }

        public static implicit operator string(EttCode ettCode)
        {
            return ettCode.Code;
        }

        public static implicit operator EttCode(string ettCode)
        {
            return new EttCode(ettCode);
        }

        #endregion
    }
}
