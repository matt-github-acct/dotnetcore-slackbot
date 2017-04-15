using Xunit;
using Slackbot;
using Moq;
using System;

public class NewSlackTests
{
    public class GetUsernameTests
    {
        [Fact]
        public async void when_http_client_throws()
        {
            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Throws(new Exception());

            var sut = new NewSlack(mockHttp.Object);

            var actual = await sut.GetUsername("", "");
            Assert.Equal("", actual);
        }
    }
}