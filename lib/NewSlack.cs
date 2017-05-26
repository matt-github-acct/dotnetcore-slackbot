using System.Threading.Tasks;
using Slackbot;
using System.Linq;

public class NewSlack
{
    IHttp http;

    public NewSlack(IHttp http)
    {
        this.http = http;
    }
    public async Task<string> GetUsername(string token, string userId)
    {
        var uri = $"https://slack.com/api/users.list?token={token}";

        try
        {
            var result = await http.Get(uri);

            return JSON.Deserialize<SlackUserList>(result.Body).Members.First(member => member.Id == userId).Name;
        }
        catch (System.Exception)
        {
            return string.Empty;
        }
    }
}