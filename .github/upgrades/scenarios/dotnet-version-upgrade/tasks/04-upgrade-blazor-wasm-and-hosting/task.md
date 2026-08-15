# 04-upgrade-blazor-wasm-and-hosting: Apply Blazor-specific compatibility changes

For Blazor WebAssembly and Blazor Server projects, update any Blazor-specific packages, update CSS/Static assets handling if changed, and verify the hosting project's TargetFramework. For WASM, ensure runtime/Mono/wasm build targets are compatible with the net11 SDK preview (update Microsoft.AspNetCore.Components.WebAssembly.* packages as needed).

Affected items: YoutubeLinks.Blazor project and any client/host projects.

Done when: Blazor apps run locally (dev serve) and build without errors; client-side artifacts are produced and served.
