using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SimpleSlackbot
{
    public static class SlackMessage
    {
        public static string[] FindMentionedUsers(string message)
        {
            var mentionedUsers = new List<string>();
            var matches = Regex.Matches(message, "<@(.*?)>");

            foreach (Match match in matches)
            {
                mentionedUsers.Add(match.Groups[1].ToString());
            }

            return mentionedUsers.ToArray();
        }
    }
}