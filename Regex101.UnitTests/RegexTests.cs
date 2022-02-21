using System.Text.Json;
using Xunit;

namespace Regex101.UnitTests;
public class RegexTests
{
    [Fact]
    public void TestRegexMatchAsync()
    {
        //act
        var receivedJs = Helpers.RegexMatch(".", "aa", 0);

        //assert
        var doc = JsonDocument.Parse(receivedJs);
        var array = doc.RootElement.EnumerateArray();
        array.MoveNext();
        var innerGroup = array.Current.EnumerateArray();
        innerGroup.MoveNext();
        Assert.Equal(0, innerGroup.Current.GetProperty("start").GetInt32());
        Assert.Equal(1, innerGroup.Current.GetProperty("end").GetInt32());
        Assert.True(innerGroup.Current.GetProperty("isParticipating").GetBoolean());
        Assert.Equal(0, innerGroup.Current.GetProperty("groupNum").GetInt32());
        Assert.Equal("0", innerGroup.Current.GetProperty("groupName").GetString());
        Assert.Equal("a", innerGroup.Current.GetProperty("content").GetString());
    }

    [Fact]
    public void TestRegexMatchEmptyInputAsync()
    {
        //act
        var receivedJs = Helpers.RegexMatch(".", "", 0);

        //assert
        Assert.Equal("[]", receivedJs);
    }

    [Fact]
    public void TestRegexMatchesAsync()
    {
        //act
        var receivedJs = Helpers.RegexMatches("(?<gn>.)", "aa", 0);

        //assert
        var doc = JsonDocument.Parse(receivedJs);
        var array = doc.RootElement.EnumerateArray();
        array.MoveNext();
        var innerGroup = array.Current.EnumerateArray();
        innerGroup.MoveNext();
        Assert.Equal(0, innerGroup.Current.GetProperty("start").GetInt32());
        Assert.Equal(1, innerGroup.Current.GetProperty("end").GetInt32());
        Assert.True(innerGroup.Current.GetProperty("isParticipating").GetBoolean());
        Assert.Equal(0, innerGroup.Current.GetProperty("groupNum").GetInt32());
        Assert.Equal("0", innerGroup.Current.GetProperty("groupName").GetString());
        Assert.Equal("a", innerGroup.Current.GetProperty("content").GetString());
        innerGroup.MoveNext();
        Assert.Equal(0, innerGroup.Current.GetProperty("start").GetInt32());
        Assert.Equal(1, innerGroup.Current.GetProperty("end").GetInt32());
        Assert.True(innerGroup.Current.GetProperty("isParticipating").GetBoolean());
        Assert.Equal(1, innerGroup.Current.GetProperty("groupNum").GetInt32());
        Assert.Equal("gn", innerGroup.Current.GetProperty("groupName").GetString());
        Assert.Equal("a", innerGroup.Current.GetProperty("content").GetString());
        array.MoveNext();
        innerGroup = array.Current.EnumerateArray();
        innerGroup.MoveNext();
        Assert.Equal(1, innerGroup.Current.GetProperty("start").GetInt32());
        Assert.Equal(2, innerGroup.Current.GetProperty("end").GetInt32());
        Assert.True(innerGroup.Current.GetProperty("isParticipating").GetBoolean());
        Assert.Equal(0, innerGroup.Current.GetProperty("groupNum").GetInt32());
        Assert.Equal("0", innerGroup.Current.GetProperty("groupName").GetString());
        Assert.Equal("a", innerGroup.Current.GetProperty("content").GetString());
        innerGroup.MoveNext();
        Assert.Equal(1, innerGroup.Current.GetProperty("start").GetInt32());
        Assert.Equal(2, innerGroup.Current.GetProperty("end").GetInt32());
        Assert.True(innerGroup.Current.GetProperty("isParticipating").GetBoolean());
        Assert.Equal(1, innerGroup.Current.GetProperty("groupNum").GetInt32());
        Assert.Equal("gn", innerGroup.Current.GetProperty("groupName").GetString());
        Assert.Equal("a", innerGroup.Current.GetProperty("content").GetString());
    }

    [Fact]
    public void TestRegexMatchesEmptyInputAsync()
    {
        //act
        var receivedJs = Helpers.RegexMatches("(?<gn>.)", "", 0);

        //assert
        Assert.Equal("[]", receivedJs);
    }

    [Fact]
    public void TestRegexReplaceAsync()
    {
        //act
        var receivedJs = Helpers.RegexReplace(".", "aa", "b", 0, true);

        //assert
        Assert.Equal("bb", receivedJs);
    }
}