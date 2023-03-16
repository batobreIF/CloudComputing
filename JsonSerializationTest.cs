using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text.Json;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace WorkingWithJSONData
{
    [TestClass]
    public class JsonSerializationTest
    {
        [TestMethod]
        public void Parse_ValidJsonDocument_ReturnsParsedDocument()
        {
            string content = File.ReadAllText("person.json").Trim();
            JsonDocument document = JsonDocument.Parse(content);
            Assert.AreEqual(JsonValueKind.Object, document.RootElement.ValueKind);
            Assert.AreEqual(content,document.RootElement.ToString());
            JsonElement nameProperty = document.RootElement.GetProperty("name");
            Assert.AreEqual(JsonValueKind.String, nameProperty.ValueKind);
            Assert.AreEqual("Johannes",nameProperty.GetString());
            JsonElement preference = document.RootElement.GetProperty("preferences");
            JsonElement languages = preference.GetProperty("language");

            Assert.AreEqual(JsonValueKind.Array, languages.ValueKind);
            Assert.AreEqual("en", languages[0].GetString());
            Assert.AreEqual("de", languages[1].GetString());
        }
        //[TestMethod]
        //public void Parse_InvalidJsonDocument_ReturnsParsedDocument()
        //{
        //    string content = File.ReadAllText("person.json").Trim();
        //    JsonDocument document = JsonDocument.Parse(content);
        //    Assert.AreEqual(JsonValueKind.Object, document.RootElement.ValueKind);
        //    Assert.AreEqual(content, document.RootElement.ToString());
        //    JsonElement nameProperty = document.RootElement.GetProperty("name");
        //    Assert.AreEqual(JsonValueKind.String, nameProperty.ValueKind);
        //    Assert.AreEqual("Johannes", nameProperty.GetString());
        //    JsonElement preference = document.RootElement.GetProperty("preferences");
        //    JsonElement languages = preference.GetProperty("language");

        //    Assert.AreEqual(JsonValueKind.Array, languages.ValueKind);
        //    Assert.AreEqual("en", languages[0].GetString());
        //    Assert.AreEqual("de", languages[1].GetString());
        //}
        [TestMethod]
        public void Deserialize_ValidJsonDocument_CanDeserializeProperties()
        {
            string content = File.ReadAllText("person.json");
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive = true };
            Person? person = JsonSerializer.Deserialize<Person>(content,options);
            Assert.IsNotNull(person);
            Assert.AreEqual("Johannes", person.Name);
            Assert.AreEqual(34.9, person.Age);
            CollectionAssert.AreEqual(new[] {"Waltraud", "Rudolf"}, person.Parents.ToList());
            Assert.AreEqual("dark", person.Preferences.Theme);
            CollectionAssert.AreEqual(new[] { "en", "de" }, person.Preferences.Language.ToList());
        }

        [TestMethod]
        public async void Deserialize_ValidJsonDocumentFromWebservice_CanDeserializeProperties()
        {
            using HttpClient httpClient = new HttpClient();
            string userInfo = await httpClient.GetStringAsync("https://api.github.com/users/johannesegger");
            JsonSerializerOptions options = new() { PropertyNameCaseInsensitive= true };
            GitHubUser user = JsonSerializer.Deserialize<GitHubUser>(userInfo, options);
            Assert.AreEqual("Johannes Egger", user.Name);
        }
    }

    class GitHubUser
    {
        public string Name { get; set; }
    }
}
class Person
{
    public Person(string name, double age, IEnumerable<string> parents, PersonPreferences preferences)
    {
        Name = name;
        Age = age;
        Parents = parents;
        Preferences = preferences;
    }

    public string Name { get; set; }
    public double Age { get; set; }
    public IEnumerable<string> Parents { get; set; }
    public PersonPreferences Preferences { get; }
}

public class PersonPreferences
{
    public PersonPreferences(string theme, string[] language)
    {
        Theme = theme;
        Language = language;
    }

    public string Theme { get; }
    public string[] Language { get; }
}