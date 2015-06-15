using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Teller.Core.BillettService
{
    public class SeatDiffGenerator
    {
        public IEnumerable<SeatChange> GenerateDiff(string filepathA, string filepathB)
        {
            if(String.IsNullOrWhiteSpace(filepathA) || !File.Exists(filepathA))
                throw new ArgumentException("Invalid filepath", "filepathA");
            if (String.IsNullOrWhiteSpace(filepathB) || !File.Exists(filepathB))
                throw new ArgumentException("Invalid filepath", "filepathB");

            var fileA = BillettServiceXmlFile.LoadFile(filepathA);
            var fileB = BillettServiceXmlFile.LoadFile(filepathB);

            var seatReader = new BillettServiceSeteLeser();

            var seatsA = seatReader.ReadSeats(fileA).ToList();
            var seatsB = seatReader.ReadSeats(fileB).ToList();

            var joinQuery = from seatA in seatsA
                            join seatB in seatsB
                                on new {seatA.SectionName, seatA.RowName, seatA.SeatName}
                                equals new {seatB.SectionName, seatB.RowName, seatB.SeatName}
                            select new SeatChange(seatA, seatB);
            var diff = joinQuery.Where(sc => sc.FromEtt != sc.ToEtt)
                                .OrderBy(s => s.ToString())
                                .ToList();

            return diff;
        }
    }
}
