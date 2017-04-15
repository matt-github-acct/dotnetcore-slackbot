using System.Threading.Tasks;
using Slackbot;

public class NewSlack
{
    IHttp http;
    
    public NewSlack(IHttp http)
    {
        this.http = http;
    }
    public async Task<string> GetUsername(string token, string userId)
    {
        return string.Empty;
    }
}