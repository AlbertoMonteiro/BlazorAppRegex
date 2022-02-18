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
importScripts(src);

const appName = "BlazorApp1";

self.addEventListener('message', (e) => {
    const strJson = JSON.parse(e.data);
    const flags = 0; //None
    if (strJson.method === "globalMatches")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexMatches`, strJson.regex, strJson.textValue, flags);
    else if (strJson.method === "oneMatch")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexMatch`, strJson.regex, strJson.textValue, flags);
    else if (strJson.method === "substitution")
        self.DotNet.invokeMethod(appName, `${appName}.Helpers:RegexReplace`, strJson.regex, strJson.textValue, strJson.substitution, flags);
});

self.engineInit = window.engineInit = function engineInit(value) {
    console.log("ready to run");
}

self.regexCallback = window.regexCallback = function regexCallback(value) {
    const strJson = BINDING.conv_string(value);
    self.postMessage(strJson);
}