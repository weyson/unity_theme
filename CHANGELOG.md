# Changelog

## 1.1.2

- Fix color scheme switch back (e.g. Warm to Cyan): remove the previous injected StyleSheet before reloading `light.uss`, so toolbar colors update correctly.

## 1.1.1

- Fix black editor UI: restore IMGUI selector rules with literal colors in each palette and `light.uss`. Unity Extensions USS does not resolve `var()` across `common.uss` / `light.uss`.

## 1.1.0

- Add six color schemes in Preferences > Wey > Editor Theme: Cyan, Warm, Lavender, Sand, Mint, Rose.
- Split USS into shared `common.uss`, active `light.uss` palette, and `Palettes/*.uss` definitions.
- Add `WeyEditorThemePaletteSync` to sync the selected palette into `light.uss`.

## 1.0.2

- Fix compile: use `UnityEditorInternal.InternalEditorUtility.RepaintAllViews()`.

## 1.0.1

- Add Unity `.meta` files so git UPM import registers Editor scripts and USS assets.

## 1.0.0

- Initial release: cyan-tinted Light editor theme.
- USS extensions for IMGUI chrome and UI Toolkit toolbar injection.
- Preferences toggle under Wey > Editor Theme.
