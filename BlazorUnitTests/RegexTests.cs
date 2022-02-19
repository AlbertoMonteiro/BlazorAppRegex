using BlazorApp1;
using Microsoft.JSInterop;
using NSubstitute;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace BlazorUnitTests;
public class RegexTests
{
    [Fact]
    public async Task TestRegexMatchWithWrongRegexAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexMatch("(?<some\"InvalidName).", "aa", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
        Assert.Equal("[]", receivedJs);
    }

    [Fact]
    public async Task TestRegexMatchAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexMatch(".", "aa", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
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
    public async Task TestRegexMatchesAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexMatches("(?<gn>.)", "aa", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
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
    public async Task TestRegexMatchesWithWrongRegexAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexMatches("(?<g\"n>.)", "aa", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
        Assert.Equal("[]", receivedJs);
    }

    [Fact]
    public async Task TestRegexReplaceAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexReplace(".", "aa", "b", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
        Assert.Equal("bb", receivedJs);
    }

    [Fact]
    public async Task TestRegexReplaceWithWrongRegexAsync()
    {
        //arrange
        var calledMethod = "";
        var receivedJs = "";
        var finished = false;
        Helpers.JsRuntime = Substitute.For<IJSUnmarshalledRuntime>();
        Helpers.JsRuntime.When(x => x.InvokeUnmarshalled<string, int>("regexCallback", Arg.Any<string>()))
            .Do(cb =>
            {
                finished = true;
                calledMethod = cb.ArgAt<string>(0);
                receivedJs = cb.ArgAt<string>(1);
            });
        //act
        Helpers.RegexReplace("(?<some\"InvalidName).", "aa", "b", 0);

        //assert
        await Task.Run(() => { while (!finished) { } });
        Assert.Equal("regexCallback", calledMethod);
        Assert.Null(receivedJs);
    }
}