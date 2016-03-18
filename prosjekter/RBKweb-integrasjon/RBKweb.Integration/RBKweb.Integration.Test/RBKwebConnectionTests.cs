using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RBKweb.Integration.Test
{
    public class RBKwebConnectionTests
    {
        [Fact]
        public async void Login_WhenGivenCorrectUsernameAndPassword_ShouldWork()
        {
            // Arrange
            var sut = new RBKwebConnection("OrionPax", "cgFMGUlt1rjdjz8h1WWg");

            // Act
            var res = await sut.Login();
            var msgRes = await sut.SendMessage("OrionPax", "Test av melding via RBKweb", "Hei!\r\n\r\nHar du det bra?\r\n\r\nHilsen din vennlige Setetellerautomat");

            // Assert
        }

    }
}
