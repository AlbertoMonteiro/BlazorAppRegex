# BlazorAppRegex

This is a simple blazor app that I made to help on that issue [Add C# flavor at Regex101](https://github.com/firasdib/Regex101/issues/156).

This app is using WebAssembly Blazor and is static hosted in github pages, you can check in this link: https://albertomonteiro.github.io/BlazorAppRegex/

# Setup

You will need the stable dotnet 6 sdk installed in your machine.
<br>
Go to downloads page (https://dotnet.microsoft.com/en-us/download), download and install it.
<br>
After install, run 
```bash
dotnet --version
```
You should get something like that
```bash
6.0.101
```

# Running the project

First clone it
<br>
Then go to the repo folder: `REPO_ROOT/BlazorApp1`
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
      Content root path: C:\Dev\BlazorApp1\BlazorApp1
```
Then just open in your browser: `http://localhost:5133`

# Publishing

In the repo folder: `REPO_ROOT/BlazorApp1`
<br>
To run it, just execute the following command
```bash
dotnet publish -c Release -o out
```
That command will produce in the `out` directory the file to run the app.

If you look at the `out` directory, you should see something like that: 
```
│   web.config
│
└───wwwroot
    │   favicon.ico
    │   icon-192.png
    │   index.html
    │
    └───_framework
            blazor.boot.json
            blazor.boot.json.br
            blazor.boot.json.gz
            blazor.webassembly.js
            blazor.webassembly.js.br
            blazor.webassembly.js.gz
            BlazorApp1.dll
            BlazorApp1.dll.br
            BlazorApp1.dll.gz
            BlazorApp1.pdb.gz
            dotnet.6.0.1.5780qzte03.js
            dotnet.6.0.1.5780qzte03.js.br
            dotnet.6.0.1.5780qzte03.js.gz
            more files....
```
The web.config file is needed if you are going to host the app in IIS, so if you dont skip it and go to wwwroot folder.

In the index.html it loads the `_framework/blazor.webassembly.js` and this one will load other files in from the `_framework` folder that is required to run.

That app is hosted in GitHub Pages, you can check the gh-pages branch and go to the app site https://albertomonteiro.github.io/BlazorAppRegex/.

For that app those files were loaded

- _framework/blazor.boot.json
- _framework/blazor.webassembly.js
- _framework/dotnet.6.0.1.5780qzte03.js

# GH-Pages

To be able to publish that Blazor app in GitHub Pages I had to add the `.nojekyll` file, because without that, the files inside the `_framework`
folder wasnt being able to be download.

Other issue that I faced was related to `Error: No .NET call dispatcher has been set.`, this happened to me because when I commited the `dotnet.6.0.1.5780qzte03.js` file 
to the gh-pages branch, my git converted the line breaks(`\n\r`) to (`\n`) and that changed the file, when the `blazor.webassembly.js` file trigger the `dotnet.6.0.1.5780qzte03.js` 
it requires the file to have and specific hash and that hash isnt the same anymore, since the content was changed because the change from `\n\r` to `\n`, so
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
