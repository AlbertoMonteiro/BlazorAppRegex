import createDotnetRuntime from './dotnet.js'

const { MONO, BINDING, Module, RuntimeBuildInfo } = await createDotnetRuntime();
const appName = "Regex101";

function bindStaticMethod(assembly, typeName, method) {
    // Fully qualified name looks like this: "[debugger-test] Math:IntAdd"
    const fqn = `[${assembly}] ${typeName}:${method}`;
    return BINDING.bind_static_method(fqn);
}

self.addEventListener('message', (e) => {
    const strJson = JSON.parse(e.data);
    const flags = 0; //None
    if (strJson.method === "globalMatches") {
        const result = regexMatches(strJson.regex, strJson.textValue, flags);
        self.postMessage(result);
    }
    else if (strJson.method === "oneMatch") {
        const result = regexMatch(strJson.regex, strJson.textValue, flags);
        self.postMessage(result);
    }
    else if (strJson.method === "substitution") {
        const result = regexReplace(strJson.regex, strJson.textValue, strJson.substitution, flags, false);
        self.postMessage(result);
    }
    else if (strJson.method === "listSubstitution") {
        const result = regexListReplace(strJson.regex, strJson.textValue, strJson.substitution, flags, false);
        self.postMessage(result);
    }
});


self.engineInit = function engineInit() {
    console.log("ready to run");
    self.regexMatches = bindStaticMethod(appName, `${appName}.Helpers`, "RegexMatches");
    self.regexMatch = bindStaticMethod(appName, `${appName}.Helpers`, "RegexMatch");
    self.regexReplace = bindStaticMethod(appName, `${appName}.Helpers`, "RegexReplace");
    self.regexListReplace = bindStaticMethod(appName, `${appName}.Helpers`, "RegexListReplace");
}

self.regexCallback = function regexCallback(value) {
    const strJson = BINDING.conv_string(value);
    self.postMessage(strJson);
}

self.engineInit();