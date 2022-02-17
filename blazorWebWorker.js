const src = "blazor.webassembly.js";

window = self;
window.Module = {};
document = {
    currentScript: {
        getAttribute: name => {
            return name == "autostart" ? true : name;
        }
    },
    getElementsByTagName: () => [],
    addEventListener: (a, b, c, d, e) => {
        return {
            hasChildNodes: () => false
        };
    },
    createElement: (a, b, c, d, e) => {
        return {
            hasChildNodes: () => false
        };
    },
    createElementNS: (a, b, c, d, e) => {
        return {
            hasChildNodes: () => false
        };
    },
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

self.addEventListener('message', (e) => {
    const strJson = JSON.parse(e.data);
    const appName = "BlazorApp1";
    self.DotNet.invokeMethod(appName, "RegexMatches", strJson.regex, strJson.textValue);
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