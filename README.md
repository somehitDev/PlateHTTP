<h1 align="center">
    PlateHTTP
</h1>
<p align="center">
    Simple and Light WebServer for .NET
</p>
<br/><br/>

※ README is in progress...

## 🌐 Install
- git
  - clone repository
    ```zsh
    git clone https://github.com/somehitDev/PlateHTTP.git
    ```

  - add projectreference to .csproj of your project
    ```xml
    <ItemGroup>
        <ProjectReference Include="{path of PlateHTTP}/Core/PlateHTTP.csproj" />
        <!-- if need extensions -->
        <ProjectReference Include="{path of PlateHTTP}/Extensions/{name of extensions}.csproj" />
    </ItemGroup>
    ```
- nuget
  - in progress ...

<br>

## ⚒️ Example
```cs
using PlateHTTP.Core;
using PlateHTTP.Extensions.Templating;

// create application instance
var app = new WebApplication();
// enable logging(optional)
app.EnableLogging("debug");

// register route points
app.Get("/", async ( request, response ) => {
    await response.SendText("Hello, PlateHTTP!");
});

// start server
app.Start();
```

- output
```zsh
PlateHTTP::info - Server listening on http://0.0.0.0:8000/
PlateHTTP::info - Press `Ctrl+C` to shutdown server...

PlateHTTP::debug - Endpoint [GET] `/`
```

<br><br>

## 📆 CHANGELOG
- see [CHANGELOG](https://github.com/somehitDev/PlateHTTP/tree/master/CHANGELOG.md)
