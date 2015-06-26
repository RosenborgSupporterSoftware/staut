using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Teller.Core.Export;
using Xunit;
using Teller.Core.Infrastructure;

namespace Teller.Tests.Infrastructure
{
    public class TicketDecoderTests
    {
        // 

        [Fact]
        public void ServiceDecoder_ServiceDecodes_Service()
        {
            // Arrange
            var reader = new BillettServiceReader();
            
            // Act
            //var info = reader.ReadData(@"D:\temp\Billett-telling\TLD0415-2015,NO-438515-050515-0654.xml");

            // Assert
        }

    }
}
