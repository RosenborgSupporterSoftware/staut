using System.Collections.Generic;
using Teller.Core.Entities;

namespace Teller.Core.BillettService
{
    public static class SeatStatusClassifier
    {
        public static SeatStatus Classify(EttCode code)
        {
            if (code.BaseTypeHex == "00")
                return SeatStatus.AvailableForPurchase;

            if(code.BaseTypeHex == "1E")
                return SeatStatus.HeldByTicketMasterApplication;
            
            if(IsKnownSeasonTicketCode(code))
                return SeatStatus.SeasonTicket;

            if(IsKnownSoldCode(code))
                return SeatStatus.Sold;

            return ClassifyByRange(code);
        }

        public static SeatStatus ClassifyByRange(EttCode code)
        {
            if(code.QualifierBits > 0x0 && code.QualifierBits < 0x30)
                return SeatStatus.Reserved;
            
            if(code.QualifierBits > 0x400 && code.QualifierBits < 0x600)
                return SeatStatus.Sold;
            
            //if(code.QualifierBits >= 0x464 && code.QualifierBits < 0x600)
            //    return SeatStatus.SeasonTicket;

            if(code.QualifierBits > 0x800)
                return SeatStatus.Unknown;

            return SeatStatus.Unknown;
        }

        public static bool IsKnownSeasonTicketCode(EttCode code)
        {
            var knownSeasonCodes = new List<string>
            {
                // Liste hentet ut ved å finne koder 0x400 - 0x600 fra to siste kamper 2015 før de er lagt ut for salg
                "4C7",  // Sesongkort gull voksen (jubajuba m.fl.)
                "596",
                "4CF",  // Sesongkort - Kjernen?
                "4C8",  // Sesongkort voksen "svart" (FA) kontant (RoarO)
                "5C2",
                "4CD",  // Sesongkort familierabatt (hoboj0e)
                "4CB",  // Sesongkort studentrabatt normal betaling (Kjello)
                "4CE",
                "4C9",  // Sesongkort hvit voksen
                "590",
                "4CC",  // Sesongkort hvit honnør/barn/student
                "5C8",
                "5C3",
                "4CA",  // Sesongkort gull honnør/barn/student
                "501",
                "592",
                "591",  // Sponsor-sesongkort o.l.
                "4FA",
                "4FB",  // Sesongkort "svart" (Brego)
                "500",
                "4FF",
                "4D0",
                "52C",
                "595",
                "4FD",  // Sesongkort studentrabatt avtalegiro (Kjello)
                "4FC",  // Sesongkort (vegardj)
                "502",
                "4FE",
                "593",
                "5C4",
                "5CC",
                "4F9"
            };

            return knownSeasonCodes.Contains(code.QualifierBitsHex);

            //if (code.QualifierBitsHex == "413") // hoboj0e sesongkort familierabatt 1
            //    return true;
        }

        public static bool IsKnownSoldCode(EttCode code)
        {
            if (code.QualifierBitsHex == "414")
                return true;
            if (code.QualifierBitsHex == "5C2") // Solgt basert på antagelser fra vegardj
                return true;
            if (code.QualifierBitsHex == "409") // Solgt - Vennebillett? Ihvertfall solgt.
                return true;

            if (code.QualifierBitsHex == "406") // Solgt bortesupporter?
                return true;
            if (code.QualifierBitsHex == "41C") // Solgt bortesupporter?
                return true;

            if (code.QualifierBitsHex == "401") // Solgt (vegardj)?
                return true;
            if (code.QualifierBitsHex == "417") // Solgt (vegardj)?
                return true;

            return false;
        }
    }
}
