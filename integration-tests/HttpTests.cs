using Slackbot;
using Xunit;

public class HttpTests
{
    [Theory]
    [InlineData("http://httpstat.us/404", 404, "404 Not Found")]
    [InlineData("http://httpstat.us/200", 200, "200 OK")]
    public async void when_executing_get_request(string uri, int expectedStatusCode, string expectBody)
    {
        var response = await new Http().Get(uri);
        Assert.Equal(expectedStatusCode, response.StatusCode);
        Assert.Equal(expectBody, response.Body);
    }
}