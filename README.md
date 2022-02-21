# BlazorAppRegex

This is a simple blazor app that I made to help on that issue [Add C# flavor at Regex101](https://github.com/firasdib/Regex101/issues/156).

This app is using WebAssembly Blazor and is static hosted in github pages, you can check in this link: https://albertomonteiro.github.io/BlazorAppRegex/

# Setup

You will need the stable dotnet 7(preview 1) sdk installed in your machine.
<br>
Go to downloads page (https://dotnet.microsoft.com/en-us/download), download and install it.
<br>
After install, run 
```bash
dotnet --version
```
You should get something like that
```bash
7.0.100-preview.1.22110.4
```

# Running the project

First clone it
<br>
Then go to the repo folder: `REPO_ROOT/Regex101`
<br>
To run it, just execute the following command
```bash
dotnet run
```
After the compile phase, it will start the run phase
```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5133
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Dev\Regex101\Regex101
```
Then just open in your browser: `http://localhost:5133`

# Publishing

In the repo folder: `REPO_ROOT/Regex101`
<br>
You need to install wasm-tools for the first time, it will download an compatible version with your dotnet sdk version
```bash
dotnet workload install wasm-tools
```
<br>
Now to run it, just execute the following command
```bash
dotnet publish -c Release -o release
```
That command will produce in the `release` directory the file to run the app.

If you look at the `release` directory, you should see something like that: 
```
└───wwwroot
        blazor.boot.json
        blazor.boot.json.br
        blazor.boot.json.gz
        Regex101.dll
        Regex101.dll.br
        Regex101.dll.gz
        Regex101.pdb.gz
        blazorWebWorker.js
        more files....
```
That app is hosted in GitHub Pages, you can check the gh-pages branch and go to the app site https://albertomonteiro.github.io/BlazorAppRegex/.

# GH-Pages

To be able to publish that Blazor app in GitHub Pages I had to add the `.nojekyll` file, because without that, the files inside the `_framework`
folder wasnt being able to be download.

Other issue that I faced was related to `Error: No .NET call dispatcher has been set.`, this happened to me because when I commited the `dotnet.6.0.1.5780qzte03.js` file 
to the gh-pages branch, my git converted the line breaks(`\n\r`) to (`\n`) and that changed the file, when the `blazor.webassembly.js` file trigger the `dotnet.6.0.1.5780qzte03.js` 
it requires the file to have and specific hash(**check integrity**) and that hash isnt the same anymore, since the content was changed because the change from `\n\r` to `\n`, so
I disabled that behavior on my git and it was fixed.

Git message sample:
```
> git add .
warning: LF will be replaced by CRLF in dotnet.6.0.1.5780qzte03.js.
The file will have its original line endings in your working directory
```

Error message in browser
```
Failed to find a valid digest in the 'integrity' attribute for resource 'http://172.31.176.1:8080/_framework/dotnet.6.0.1.5780qzte03.js' with computed SHA-256 integrity 'dMQSCY8hJT8+Ju/0yKK3E4K9tXu2M6WBGbc3ZRoj02A='. The resource has been blocked.
```
