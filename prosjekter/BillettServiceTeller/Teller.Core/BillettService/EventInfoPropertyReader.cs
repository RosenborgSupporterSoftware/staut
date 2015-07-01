using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Runtime.Remoting.Messaging;
using Teller.Core.Entities;

namespace Teller.Core.BillettService
{
    /// <summary>
    /// En klasse som leser eventinfo.properties-filer og lager BillettServiceEvent-objekter av dem
    /// </summary>
    public class EventInfoPropertyReader
    {
        public BillettServiceEvent ReadProperties(string file)
        {
            var props = Readfile(file);

            var bsEvent = new BillettServiceEvent();

            foreach (var prop in props)
            {
                switch (prop.Key)
                {
                    case "eventname":
                        bsEvent.DisplayName = prop.Value;
                        break;
                    case "eventid":
                        bsEvent.EventNumber = Convert.ToInt32(prop.Value);
                        break;
                    case "eventcode":
                        bsEvent.EventCode = prop.Value;
                        break;
                    case "location":
                        bsEvent.Location = prop.Value;
                        break;
                    case "competition":
                        bsEvent.Tournament = prop.Value;
                        break;
                    case "round":
                        bsEvent.Round = Convert.ToInt32(prop.Value);
                        break;
                    case "opponent":
                        bsEvent.Opponent = prop.Value;
                        break;
                    case "eventdate":
                        bsEvent.Start = DateTime.ParseExact(prop.Value, @"dd\.MM\.yyyy", CultureInfo.InvariantCulture);
                        break;
                    case "eventtime":
                        var span = TimeSpan.ParseExact(prop.Value, @"hh\:mm", CultureInfo.InvariantCulture);
                        bsEvent.Start = bsEvent.Start.Add(span);
                        break;
                    case "availabilityurl":
                        bsEvent.AvailibilityUrl = prop.Value;
                        break;
                    case "geometryurl":
                        bsEvent.GeometryUrl = prop.Value;
                        break;
                }
            }

            return bsEvent;
        }

        private IDictionary<string, string> Readfile(string file)
        {
            if (String.IsNullOrWhiteSpace(file) || !File.Exists(file))
                throw new ArgumentException("Ikke særlig bra filnavn", "file");

            var lines = File.ReadAllLines(file);
            var result = new Dictionary<string, string>(lines.Length);

            foreach (var line in lines)
            {
                var split = line.Split("=".ToCharArray());
                result.Add(split[0], split[1]);
            }

            return result;
        }
    }
}
