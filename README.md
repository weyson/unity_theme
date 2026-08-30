# Wey Editor Theme

Cyan-tinted **Light** theme for the Unity Editor (2022.3+).

Repository: [weyson/unity_theme](https://github.com/weyson/unity_theme)

## Install

Add to `Packages/manifest.json`:

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git"
```

Optional version pin:

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git#v1.0.0"
```

Local development:

```json
"com.wey.editor-theme": "file:../unity_theme"
```

## Usage

1. Set **Edit > Preferences > General > Editor Theme** to **Light**.
2. Enable **Edit > Preferences > Wey > Editor Theme** (toolbar injection, on by default).
3. Domain reload or restart Unity if some chrome does not refresh.

## How it works

- `Editor/StyleSheets/Extensions/*.uss` — merged automatically by Unity for IMGUI editor chrome.
- `WeyEditorThemeInjector` — injects `light.uss` onto the main toolbar and buttons that ship their own `ToolbarLight` styles (Play/Pause/Step, Layout, Layers, etc.).

## Toggle behavior

- **On**: toolbar UITK buttons use the cyan palette.
- **Off**: injection stops; global IMGUI tint from Extensions USS may remain until the package is removed.

## Customize colors

Edit `Editor/StyleSheets/Extensions/light.uss`, then trigger a domain reload.

## License

Apache-2.0 (see LICENSE).
