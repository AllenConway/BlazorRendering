# BlazorRendering

A .NET 10 Blazor Web App demo that makes **render modes** and the **component lifecycle** visible and easy to reason about.

The `Counter` page reports which renderer is active, whether the component is interactive yet, and whether the code is executing on the server or in the browser (WebAssembly) — while live-counting each lifecycle method invocation.

## Why this exists

The Blazor Web App model in .NET 8+ introduced per-component render modes (`InteractiveServer`, `InteractiveWebAssembly`, `InteractiveAuto`, and static SSR). Understanding *where* your code runs and *how many times* lifecycle methods fire is the most common source of confusion. This project is a hands-on sandbox for exactly that.

## What it demonstrates

- `RendererInfo.Name` and `RendererInfo.IsInteractive` to inspect the active render mode at runtime
- `OperatingSystem.IsBrowser()` to distinguish server-side prerendering from client-side WASM execution
- Live call counts and timestamps for `OnInitialized`, `OnParametersSet`, and `OnAfterRender`
- The double-execution behavior of `InteractiveAuto` / `InteractiveWebAssembly` (prerender on server, then again on the client)
- A commented-out `[StreamRendering]` attribute you can toggle to observe streaming SSR

## Project structure

```
BlazorRendering.sln
└── BlazorRenderingDemo/
	├── BlazorRenderingDemo/          # Server project (host)
	│   ├── Components/
	│   │   ├── App.razor
	│   │   ├── Routes.razor
	│   │   ├── Layout/
	│   │   └── Pages/                # Home, Weather, Error
	│   └── Program.cs                # Registers Server + WebAssembly render modes
	└── BlazorRenderingDemo.Client/   # WebAssembly project
		└── Pages/
			└── Counter.razor         # The render mode / lifecycle demo
```

## Requirements

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Visual Studio 2022/2026, VS Code, or Rider (optional)

## Getting started

```bash
git clone https://github.com/AllenConway/BlazorRendering.git
cd BlazorRendering
dotnet run --project BlazorRenderingDemo/BlazorRenderingDemo
```

Then browse to the URL shown in the console and navigate to **/counter**.

## Try this

Open `BlazorRenderingDemo.Client/Pages/Counter.razor` and change the render mode directive:

| Directive | What you'll observe |
| --- | --- |
| `@rendermode InteractiveServer` | Runs over SignalR; renderer reports `Server`, executes only on the server |
| `@rendermode InteractiveWebAssembly` | Prerenders on the server, then runs in the browser; lifecycle methods fire twice |
| `@rendermode InteractiveAuto` | Server on first visit, WASM once the runtime is cached |
| *(remove the directive)* | Static SSR — the page renders but the button does nothing |

You can also uncomment `@attribute [StreamRendering]` to see streaming server-side rendering in action.

## License

MIT
