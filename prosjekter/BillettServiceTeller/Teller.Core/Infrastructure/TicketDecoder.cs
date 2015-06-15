using System;
using System.Xml.Linq;

namespace Teller.Core.Infrastructure
{
    // Klasse som har ansvar for å dekode "hemmelige" billettdata fra BillettService
    public class TicketDecoder
    {
        public XDocument ExtractXml(XDocument xdoc)
        {
            var data = GetCompressedData(xdoc);
            if(String.IsNullOrWhiteSpace(data))
                throw new ArgumentException("Ubrukelig xml");

            var decodedBytes = new ByteArray(Convert.FromBase64String(data));

            var decodedXml = Ran(decodedBytes);

            return XDocument.Parse("<xml>" + decodedXml + "</xml>");
        }

        [Obsolete("Selve fila bør fremover kun leses av BillettServiceReader-klassen - da skal denne metoden være overflødig")]
        public XDocument ExtractXml(string filename)
        {
            var dataString = GetCompressedDataFromFile(filename);

            var data = Convert.FromBase64String(dataString);

            var ba = new ByteArray(data);

            var res = Ran(ba);

            return XDocument.Parse("<xml>" + res + "</xml>");
        }

        private string GetCompressedData(XDocument xdoc)
        {
            if (xdoc.Root == null)
                return String.Empty;
            var dataElement = xdoc.Root.Element("ev_comp");
            return dataElement == null ? String.Empty : dataElement.Value;
        }

        private string GetCompressedDataFromFile(string filename)
        {
            var xml = XDocument.Load(filename);

            return GetCompressedData(xml);
        }

        private string Ran(ByteArray ba)
        {
            uint globals_ssa = 161;
            uint globals_es;
            uint globals_mes;
            uint globals_us;

            uint numberBytes = 0;
            uint OM = 0;
            uint STEP = 0;
            uint VSIZE = 0;
            uint sCompress = 0;
            var i = 0;
            uint currByte = 0;
            var j = 0;

            try {
                if (ba.Length > 0){
                    numberBytes = ba.ReadUnsignedByte();
                    OM = ba.ReadUnsignedByte();
                    if ((((ba.BytesAvailable > numberBytes)) && ((OM == 1))))
                    {
                        STEP = ba.ReadUnsignedByte();
                        VSIZE = ba.ReadUnsignedByte();
                        sCompress = ba.ReadUnsignedByte();
                        //V = [new uint(Globals.get_errorstr(Globals.MSG_COMPRESSION_WALL_VALUE))];
                        var V = new ByteArray();
                        V.Push(165);
                        i = 0;
                        while (i < (VSIZE - 1)) {
                            V.Push((int)ba.ReadUnsignedByte());
                            i = (i + 1);
                        };
                        VSIZE = (VSIZE + 1);
                        globals_ssa = (globals_ssa + (VSIZE + 1));
                        var sd = new ByteArray();
                        ba.ReadBytes(sd);
                        sd.Position = 0;
                        ba.Position = 0;
                        do  {
                            globals_ssa = sCompress;
                            globals_ssa = ((globals_ssa * V[0]) & 4194303);
                            j = 1;
                            while (j < (V.Length - 1)) {
                                globals_es = V[j];
                                globals_ssa = (((globals_ssa + globals_es) * sCompress) & 4194303);
                                j = (j + 1);
                            };
                            globals_ssa = (globals_ssa + V[(V.Length - 1)]);
                            globals_mes = ((globals_ssa >> 12) & 0xFF);
                            globals_us = (uint) (globals_ssa % V.Length);
                            V[globals_us] = Convert.ToByte((V[globals_us] ^ sCompress));
                            currByte = sd.ReadUnsignedByte();
                            sd.Position--;
                            currByte = (currByte ^ globals_mes);
                            sd.WriteByte(currByte);
                            sd.Position--;
                            sCompress = currByte;
                            sd.Position = (sd.Position + (int)STEP);
                        } while (sd.BytesAvailable > 0);
                        sd.Position = 0;
                        sd.Uncompress();
                        return sd.ToString();
                    };
                };
            } 
            catch(Exception e) 
            {
                Console.WriteLine(e.Message);
            };

            return String.Empty;
        }

    }
}
