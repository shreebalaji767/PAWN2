# PAWN² — Full Blazor WebAssembly Game

This is the full static PAWN² implementation, not a demo. It runs entirely in the browser.

## Stack
- C# / .NET 10
- Blazor WebAssembly
- HTML5
- CSS3
- Small JavaScript helper
- No server-side database
- No login
- No API
- No localStorage/sessionStorage/cookies for game state

## Online chess assets
The chess pieces are loaded directly at runtime from Sashité's public-domain CC0 SVG assets. Nothing is downloaded into this repository.

Asset root:
https://sashite.dev/assets/chess/sides/

The project requests paths such as:
https://sashite.dev/assets/chess/sides/first/representations/western/king.svg
https://sashite.dev/assets/chess/sides/second/representations/western/king.svg

## Run
```bash
dotnet restore
dotnet run
```

## Publish static files
```bash
dotnet publish -c Release -o publish
```

Deploy `publish/wwwroot` as the static site directory.

## Important
Replace `https://example.com/` in `wwwroot/index.html` and `wwwroot/sitemap.xml` with the real public domain before deployment.


## 2026 upgrade
- Responsive board with automatic Black-side orientation and a manual **Flip Board** control.
- Improved keyboard focus visibility and accessible board controls.
- Installable PWA manifest and service-worker shell caching.
- Improved page metadata for search and social sharing.
- GitHub Pages-ready canonical URL, robots.txt, and sitemap.

### Deployment
The repository is configured for the GitHub Pages URL: `https://shreebalaji767.github.io/PAWN2/`.

If you deploy the same build at another public domain, update the canonical URL and sitemap URL in `wwwroot/index.html`, `wwwroot/robots.txt`, and `wwwroot/sitemap.xml`.
