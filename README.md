# Wey Editor Theme

Cyan-tinted **Light** theme for the Unity Editor (2022.3+), with multiple color scheme variants.

Repository: [weyson/unity_theme](https://github.com/weyson/unity_theme)

[简体中文](README.zh-CN.md)

## Install

Add to `Packages/manifest.json`:

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git"
```

Optional version pin:

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git#v1.1.0"
```

Local development:

```json
"com.wey.editor-theme": "file:../unity_theme"
```

## Usage

1. Set **Edit > Preferences > General > Editor Theme** to **Light**.
2. Open **Edit > Preferences > Wey > Editor Theme** and pick a **Color Scheme**:
   - **Cyan** (default)
   - **Warm**
   - **Lavender**
   - **Sand**
   - **Mint**
   - **Rose**
3. Enable **Enable Toolbar Theme Injection** (on by default).
4. Domain reload or restart Unity if some chrome does not refresh after changing the color scheme.

## How it works

- `Editor/StyleSheets/Extensions/common.uss` — shared selector rules using CSS variables.
- `Editor/StyleSheets/Extensions/light.uss` — active palette variables (synced from your color scheme choice).
- `Editor/StyleSheets/Palettes/*.uss` — palette definitions for each color scheme.
- `WeyEditorThemeInjector` — injects `light.uss` onto the main toolbar and buttons that ship their own `ToolbarLight` styles (Play/Pause/Step, Layout, Layers, etc.).
- `WeyEditorThemePaletteSync` — copies the selected palette into `light.uss` so Unity merges the correct colors globally.

## Toggle behavior

- **On**: toolbar UITK buttons use the selected palette.
- **Off**: injection stops; global IMGUI tint from Extensions USS may remain until the package is removed.

## Customize colors

Edit a palette file under `Editor/StyleSheets/Palettes/`, then switch color schemes in Preferences (or domain reload) to re-sync `light.uss`.

## License

Apache-2.0 (see LICENSE).
