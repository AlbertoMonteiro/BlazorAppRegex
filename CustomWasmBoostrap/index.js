/* global importScripts, Module, BINDING, MONO */
// import DotNet from '@microsoft/dotnet-js-interop';
self.window = self;

// The dotnet js interop _requires_ window, so we just map it to self within the worker
require('@microsoft/dotnet-js-interop');
import { BrotliDecode } from './decode';
const appBinDirName = 'appBinDir';

async function fetchAsBrotli(uri) {
    const data = await fetch(`${uri}.br`);
    const originalResponseBuffer = await data.arrayBuffer();
    const originalResponseArray = new Int8Array(originalResponseBuffer);
    const dec = BrotliDecode(originalResponseArray);
    return new Response(dec, { headers: { 'content-type': data.headers['content-type'] ?? "application/json" } });
}

const init = async () => {
    const data = await fetchAsBrotli("/blazor.boot.json");
    const manifestJson = await data.json();
    const { assembly, runtime } = manifestJson.resources;

    console.log(assembly, runtime);
    const dotnetJsResourceName = Object.keys(runtime).filter(
        n => n.startsWith('dotnet.') && n.endsWith('.js')
    )[0];

    // Load primary runtime
    importScripts(`${dotnetJsResourceName}`);

    // Fetch emscripten runtime
    await self.createDotnetRuntime(async api => {
        const { MONO: mono, BINDING: binding, Module: module } = api;
        self.Module = module;
        self.BINDING = binding;
        self.MONO = mono;

        return {
            disableDotnet6Compatibility: false,
        };
    });

    // Load all dlls
    await Promise.all(
        Object.keys(assembly).map(r =>
            addResourceAsAssembly(r, changeExtension(r, '.dll'))
        )
    );

    MONO.mono_wasm_load_runtime(appBinDirName, 0);
    MONO.mono_wasm_runtime_ready();

    // Bare minimum glue to make invoking from js work
    self.Blazor = {
        _internal: {
            invokeJSFromDotNet,
            endInvokeDotNetFromJS,
        },
    };

    // Call init function inside Helpers.cs
    await BINDING.call_assembly_entry_point('BlazorApp1', [[]], 'm');
};

async function addResourceAsAssembly(dependency, loadAsName) {
    const runDependencyId = `blazor:${dependency.name}`;
    Module.addRunDependency(runDependencyId);

    try {
        // Wait for the data to be loaded and verified
        const dllFile = await fetchAsBrotli(`${dependency}`);
        const dataBuffer = await dllFile.arrayBuffer();

        // Load it into the Mono runtime
        const data = new Uint8Array(dataBuffer);
        const heapAddress = Module._malloc(data.length);
        const heapMemory = new Uint8Array(
            Module.HEAPU8.buffer,
            heapAddress,
            data.length
        );
        heapMemory.set(data);
        MONO.mono_wasm_add_assembly(loadAsName, heapAddress, data.length);

        MONO.loaded_files.push(dependency.url);
    } catch (errorInfo) {
        console.error(errorInfo);
        return;
    }

    Module.removeRunDependency(runDependencyId);
}

function changeExtension(filename, newExtensionWithLeadingDot) {
    const lastDotIndex = filename.lastIndexOf('.');
    if (lastDotIndex < 0) {
        throw new Error(`No extension to replace in '${filename}'`);
    }

    return filename.substr(0, lastDotIndex) + newExtensionWithLeadingDot;
}

///////////////////////////////////////////////////////
// Required glue, copied from .net repo              //
///////////////////////////////////////////////////////

function invokeJSFromDotNet(callInfo, arg0, arg1, arg2) {
    const functionIdentifier = monoPlatform.readStringField(callInfo, 0);
    const resultType = monoPlatform.readInt32Field(callInfo, 4);
    const marshalledCallArgsJson = monoPlatform.readStringField(callInfo, 8);
    const targetInstanceId = monoPlatform.readUint64Field(callInfo, 20);

    if (marshalledCallArgsJson !== null) {
        const marshalledCallAsyncHandle = monoPlatform.readUint64Field(
            callInfo,
            12
        );

        if (marshalledCallAsyncHandle !== 0) {
            DotNet.jsCallDispatcher.beginInvokeJSFromDotNet(
                marshalledCallAsyncHandle,
                functionIdentifier,
                marshalledCallArgsJson,
                resultType,
                targetInstanceId
            );
            return 0;
        } else {
            const resultJson = DotNet.jsCallDispatcher.invokeJSFromDotNet(
                functionIdentifier,
                marshalledCallArgsJson,
                resultType,
                targetInstanceId
            );
            return resultJson === null
                ? 0
                : BINDING.js_string_to_mono_string(resultJson);
        }
    } else {
        const func = DotNet.jsCallDispatcher.findJSFunction(
            functionIdentifier,
            targetInstanceId
        );
        const result = func.call(null, arg0, arg1, arg2);

        switch (resultType) {
            case DotNet.JSCallResultType.Default:
                return result;
            case DotNet.JSCallResultType.JSObjectReference:
                return DotNet.createJSObjectReference(result).__jsObjectId;
            case DotNet.JSCallResultType.JSStreamReference: {
                const streamReference = DotNet.createJSStreamReference(result);
                const resultJson = JSON.stringify(streamReference);
                return BINDING.js_string_to_mono_string(resultJson);
            }
            case DotNet.JSCallResultType.JSVoidResult:
                return null;
            default:
                throw new Error(`Invalid JS call result type '${resultType}'.`);
        }
    }
}

function endInvokeDotNetFromJS(callId, success, resultJsonOrErrorMessage) {
    const callIdString = BINDING.conv_string(callId);
    const successBool = success !== 0;
    const resultJsonOrErrorMessageString = BINDING.conv_string(
        resultJsonOrErrorMessage
    );
    DotNet.jsCallDispatcher.endInvokeDotNetFromJS(
        callIdString,
        successBool,
        resultJsonOrErrorMessageString
    );
}

const uint64HighOrderShift = Math.pow(2, 32);
const maxSafeNumberHighPart = Math.pow(2, 21) - 1; // The high-order int32 from Number.MAX_SAFE_INTEGER

let currentHeapLock = null;

function getValueI16(ptr) {
    return MONO.getI16(ptr);
}
function getValueI32(ptr) {
    return MONO.getI32(ptr);
}
function getValueFloat(ptr) {
    return MONO.getF32(ptr);
}
function getValueU64(ptr) {
    // There is no Module.HEAPU64, and Module.getValue(..., 'i64') doesn't work because the implementation
    // treats 'i64' as being the same as 'i32'. Also we must take care to read both halves as unsigned.
    const heapU32Index = ptr >> 2;
    const highPart = Module.HEAPU32[heapU32Index + 1];
    if (highPart > maxSafeNumberHighPart) {
        throw new Error(
            `Cannot read uint64 with high order part ${highPart}, because the result would exceed Number.MAX_SAFE_INTEGER.`
        );
    }

    return highPart * uint64HighOrderShift + Module.HEAPU32[heapU32Index];
}

function getArrayDataPointer(array) {
    return array + 12; // First byte from here is length, then following bytes are entries
}

function assertHeapIsNotLocked() {
    if (currentHeapLock) {
        throw new Error('Assertion failed - heap is currently locked');
    }
}

const monoPlatform = {
    toUint8Array: function toUint8Array(array) {
        const dataPtr = getArrayDataPointer(array);
        const length = getValueI32(dataPtr);
        const uint8Array = new Uint8Array(length);
        uint8Array.set(Module.HEAPU8.subarray(dataPtr + 4, dataPtr + 4 + length));
        return uint8Array;
    },

    getArrayLength: function getArrayLength(array) {
        return getValueI32(getArrayDataPointer(array));
    },

    getArrayEntryPtr: function getArrayEntryPtr(array, index, itemSize) {
        // First byte is array length, followed by entries
        const address = getArrayDataPointer(array) + 4 + index * itemSize;
        return address;
    },

    getObjectFieldsBaseAddress: function getObjectFieldsBaseAddress(
        referenceTypedObject
    ) {
        // The first two int32 values are internal Mono data
        return referenceTypedObject + 8;
    },

    readInt16Field: function readHeapInt16(baseAddress, fieldOffset) {
        return getValueI16(baseAddress + (fieldOffset || 0));
    },

    readInt32Field: function readHeapInt32(baseAddress, fieldOffset) {
        return getValueI32(baseAddress + (fieldOffset || 0));
    },

    readUint64Field: function readHeapUint64(baseAddress, fieldOffset) {
        return getValueU64(baseAddress + (fieldOffset || 0));
    },

    readFloatField: function readHeapFloat(baseAddress, fieldOffset) {
        return getValueFloat(baseAddress + (fieldOffset || 0));
    },

    readObjectField: function readHeapObject(baseAddress, fieldOffset) {
        return getValueI32(baseAddress + (fieldOffset || 0));
    },

    readStringField: function readHeapObject(
        baseAddress,
        fieldOffset,
        readBoolValueAsString
    ) {
        const fieldValue = getValueI32(baseAddress + (fieldOffset || 0));
        if (fieldValue === 0) {
            return null;
        }

        if (readBoolValueAsString) {
            // Some fields are stored as a union of bool | string | null values, but need to read as a string.
            // If the stored value is a bool, the behavior we want is empty string ('') for true, or null for false.
            const unboxedValue = BINDING.unbox_mono_obj(fieldValue);
            if (typeof unboxedValue === 'boolean') {
                return unboxedValue ? '' : null;
            }
            return unboxedValue;
        }

        let decodedString;
        if (currentHeapLock) {
            decodedString = currentHeapLock.stringCache.get(fieldValue);
            if (decodedString === undefined) {
                decodedString = BINDING.conv_string(fieldValue);
                currentHeapLock.stringCache.set(fieldValue, decodedString);
            }
        } else {
            decodedString = BINDING.conv_string(fieldValue);
        }

        return decodedString;
    },

    readStructField: function readStructField(baseAddress, fieldOffset) {
        return baseAddress + (fieldOffset || 0);
    },

    beginHeapLock: function () {
        assertHeapIsNotLocked();
        currentHeapLock = new MonoHeapLock();
        return currentHeapLock;
    },

    invokeWhenHeapUnlocked: function (callback) {
        // This is somewhat like a sync context. If we're not locked, just pass through the call directly.
        if (!currentHeapLock) {
            callback();
        } else {
            currentHeapLock.enqueuePostReleaseAction(callback);
        }
    },
};

class MonoHeapLock {
    // Within a given heap lock, it's safe to cache decoded strings since the memory can't change
    stringCache = new Map();
    postReleaseActions;

    enqueuePostReleaseAction(callback) {
        if (!this.postReleaseActions) {
            this.postReleaseActions = [];
        }

        this.postReleaseActions.push(callback);
    }

    release() {
        if (currentHeapLock !== this) {
            throw new Error("Trying to release a lock which isn't current");
        }

        currentHeapLock = null;

        while (this.postReleaseActions?.length) {
            const nextQueuedAction = this.postReleaseActions.shift();

            // It's possible that the action we invoke here might itself take a succession of heap locks,
            // but since heap locks must be released synchronously, by the time we get back to this stack
            // frame, we know the heap should no longer be locked.
            nextQueuedAction();
            assertHeapIsNotLocked();
        }
    }
}

// Run init to load wasm
init();

const appName = "BlazorApp1";

function bindStaticMethod(assembly, typeName, method) {
    // Fully qualified name looks like this: "[debugger-test] Math:IntAdd"
    const fqn = `[${assembly}] ${typeName}:${method}`;
    return BINDING.bind_static_method(fqn);
}

self.addEventListener('message', (e) => {
    const strJson = JSON.parse(e.data);
    const flags = 0; //None
    if (strJson.method === "globalMatches")
        regexMatches(strJson.regex, strJson.textValue, flags);
    else if (strJson.method === "oneMatch")
        regexMatch(strJson.regex, strJson.textValue, flags);
    else if (strJson.method === "substitution")
        regexReplace(strJson.regex, strJson.textValue, strJson.substitution, flags, false);
    else if (strJson.method === "listSubstitution")
        regexListReplace(strJson.regex, strJson.textValue, strJson.substitution, flags, false);
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