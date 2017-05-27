using Xunit;
using Slackbot;
using Moq;
using System;
using System.Threading.Tasks;

public class NewSlackTests
{
    public class GetWebsocketUrlTests
    {
        [Fact]
        public async void when_requesting_a_websocket_url()
        {
            var url = "best-url-eva";
            var helloRTMSession = new HelloRTMSession { url = url };
            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<HttpGetResult>(new HttpGetResult { Body = JSON.Serialize(helloRTMSession) }));

            var sut = new NewSlack(mockHttp.Object);

            var actual = await sut.GetWebsocketUrl("");
            Assert.Equal(url, actual);
        }

        [Fact]
        public async void when_http_client_throws()
        {
            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Throws(new Exception());

            var sut = new NewSlack(mockHttp.Object);


        }

        [Fact]
        public async void when_http_client_returns_non_json()
        {
            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<HttpGetResult>(new HttpGetResult { Body = "not-json" }));

            var sut = new NewSlack(mockHttp.Object);

            await Assert.ThrowsAsync<Exception>(async () => await sut.GetWebsocketUrl(""));
        }
    }

    public class GetUsernameTests
    {
        [Fact]
        public async void when_requesting_a_username_by_id()
        {
            var userId = "user-id";
            var userName = "user-name";
            var userList = new SlackUserList { Members = new[] { new SlackUser { Id = userId, Name = userName } } };

            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<HttpGetResult>(new HttpGetResult { Body = JSON.Serialize(userList) }));

            var sut = new NewSlack(mockHttp.Object);

            var actual = await sut.GetUsername("", userId);
            Assert.Equal(userName, actual);
        }

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

        [Fact]
        public async void when_http_client_returns_non_json()
        {
            var mockHttp = new Mock<IHttp>();

            mockHttp
                .Setup(http => http.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<HttpGetResult>(new HttpGetResult { Body = "not-json" }));

            var sut = new NewSlack(mockHttp.Object);

            var actual = await sut.GetUsername("", "");
            Assert.Equal("", actual);
        }
    }
}