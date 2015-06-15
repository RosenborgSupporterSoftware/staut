using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Ionic.Zlib;

namespace Teller.Core.Infrastructure
{
    /// <summary>
    /// A class that pretends to be ActionScripts' ByteArray
    /// </summary>
    /// <remarks>Ugly hack</remarks>
    public class ByteArray
    {
        #region Fields

        // TODO: Det er vel egentlig mer korrekt å bruke en ArrayList<byte>
        private readonly List<byte> _bytes = new List<byte>();
        private int _position;

        #endregion

        #region Properties

        public int Position
        {
            get { return _position; }
            set
            {
                _position = value;
            }
        }

        public int Length
        {
            get { return _bytes.Count(); }
        }

        public long BytesAvailable
        {
            get
            {
                if (Length == 0)
                    return 0;
                return Length - Position;
            }
        }

        public byte this[int index]
        {
            get { return _bytes[index]; }
            set { _bytes[index] = value; }
        }

        public byte this[uint index]
        {
            get { return _bytes[(int) index]; }
            set { _bytes[(int)index] = value; }
        }

        #endregion

        #region Constructors

        public ByteArray()
        {
            
        }

        public ByteArray(byte[] bytes)
        {
            foreach (var b in bytes)
            {
                _bytes.Add(b);
            }
            Position = 0;
        }

        #endregion

        public uint ReadUnsignedByte()
        {
            if(BytesAvailable == 0)
                throw new ArgumentOutOfRangeException();

            return Convert.ToUInt32(_bytes[Position++]);
        }

        public void ReadBytes(ByteArray destination)
        {
            for (int i = Position; i < Length; i++)
            {
                destination.WriteByte(_bytes[i]);
            }
        }

        public void WriteByte(int dataByte)
        {
            var actualByte = Convert.ToByte(dataByte);

            if(Position < Length)
                _bytes[Position] = actualByte;
            else if(Position == Length)
                _bytes.Add(actualByte);
            // TODO: Hva om position er lenger ut? 
            Position++;
        }

        public void WriteByte(uint dataByte)
        {
            var actualByte = Convert.ToByte(dataByte);

            if (Position < Length)
                _bytes[Position] = actualByte;
            else if (Position == Length)
                _bytes.Add(actualByte);
            // TODO: Hva om position er lenger ut? 
            Position++;
        }

        public uint Push(int dataByte)
        {
            var actualByte = Convert.ToByte(dataByte);

            _bytes.Add(actualByte);

            return (uint) _bytes.Count;
        }

        public void ReadBytesFrom(Stream stream)
        {
            _position = 0;
            for (var i = stream.Position; i < stream.Length; i++)
            {
                var intval = stream.ReadByte();
                
                _bytes.Add((byte)intval);
            }
        }

        public void Uncompress()
        {
            var huh = ZlibStream.UncompressBuffer(_bytes.ToArray());
            _bytes.Clear();
            foreach (var b in huh)
            {
                _bytes.Add(b);
            }
            Position = 0;
        }

        public override string ToString()
        {
            var arr = _bytes.ToArray();
            var res = Encoding.Default.GetString(arr);

            return res;
        }
    }
}
