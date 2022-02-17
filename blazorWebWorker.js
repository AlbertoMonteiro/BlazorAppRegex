const src = "blazor.webassembly.js";

window = self;
window.Module = {};
document = {
    currentScript: {
        getAttribute: name => name == "autostart" ? true : name
    },
    getElementsByTagName: () => [],
    addEventListener: () => ({}),
    createElement: () => ({}),
    createElementNS: () => ({}),
    hasChildNodes: () => false,
    baseURI: self.location.origin,
    location: self.location,
    body: {
        appendChild: (ele) => {
            if (ele.text)
                eval(ele.text);
            else if (ele.src)
                importScripts(ele.src);
        }
    }
};
let firstTime = true;
importScripts(src);

const appName = "BlazorApp1";

self.addEventListener('message', (e) => {
    const strJson = JSON.parse(e.data);
    if (strJson.method === "globalMatches")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexMatches`, strJson.regex, strJson.textValue);
    else if (strJson.method === "oneMatch")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexMatch`, strJson.regex, strJson.textValue);
    else if (strJson.method === "substitution")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexReplace`, strJson.regex, strJson.textValue, strJson.substitution);
});

self.regexCallback = window.regexCallback = function regexCallback(value, time) {
    if (firstTime) {
        firstTime = false
        return;
    }
    const strJson = BINDING.conv_string(value);
    console.log(`C# time ${time}`);
    self.postMessage(strJson);
}