using Xunit;

namespace Slackbot
{
  public class SlackMessageTests
  {
    public class FindMentionedusersTests
    {
      [Fact]
      public void when_message_is_empty()
      {
        var expected = new string[0];
        var actual = SlackMessage.FindMentionedUsers(string.Empty);

        Assert.Equal(expected, actual);
      }

      [Fact]
      public void when_no_user_is_mentioned()
      {
        var expected = new string[0];
        var actual = SlackMessage.FindMentionedUsers(string.Empty);

        Assert.Equal(expected, actual);
      }

      [Fact]
      public void when_one_user_is_mentioned()
      {
        var expected = new[] { "mentioned-user" };
        var actual = SlackMessage.FindMentionedUsers("some message <@mentioned-user>");

        Assert.Equal(expected, actual);
      }

      [Fact]
      public void when_multiple_users_are_mentioned()
      {
        var expected = new[] { "mentioned-user", "another-mentioned-user" };
        var actual = SlackMessage.FindMentionedUsers("some message <@mentioned-user> and some more message <@another-mentioned-user>");

        Assert.Equal(expected, actual);
      }
    }
  }
}
