# Wey Editor Theme

面向 Unity 编辑器（2022.3+）的青色 **Light** 主题，并提供多种配色变体。

仓库：[weyson/unity_theme](https://github.com/weyson/unity_theme)

[English](README.md)

## 安装

在 `Packages/manifest.json` 中添加：

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git"
```

可选：固定版本：

```json
"com.wey.editor-theme": "https://github.com/weyson/unity_theme.git#v1.1.1"
```

本地开发：

```json
"com.wey.editor-theme": "file:../unity_theme"
```

## 使用

1. 将 **Edit > Preferences > General > Editor Theme** 设为 **Light**。
2. 打开 **Edit > Preferences > Wey > Editor Theme**，选择 **Color Scheme（配色方案）**：
   - **Cyan**（青色，默认）
   - **Warm**（暖色）
   - **Lavender**（淡紫）
   - **Sand**（沙色）
   - **Mint**（薄荷绿）
   - **Rose**（玫瑰粉）
3. 启用 **Enable Toolbar Theme Injection**（默认开启）。
4. 切换配色后，若部分界面未刷新，请执行 Domain Reload 或重启 Unity。

## 工作原理

- `Editor/StyleSheets/Extensions/light.uss` — 当前激活的完整主题（CSS 变量 + 字面量颜色的 IMGUI 选择器规则），根据所选配色方案同步。
- `Editor/StyleSheets/Palettes/*.uss` — 各配色方案的完整主题定义。
- `WeyEditorThemeInjector` — 将 `light.uss` 注入主工具栏及自带 `ToolbarLight` 样式的按钮（Play/Pause/Step、Layout、Layers 等）。
- `WeyEditorThemePaletteSync` — 将所选 palette 复制到 `light.uss`，使 Unity 全局合并正确的主题。

## 开关行为

- **开启**：工具栏 UITK 按钮使用所选配色。
- **关闭**：停止注入；Extensions USS 带来的全局 IMGUI 着色可能仍会保留，直至移除本包。

## 自定义颜色

编辑 `Editor/StyleSheets/Palettes/` 下的 palette 文件，然后在 Preferences 中切换配色方案（或执行 domain reload）以重新同步 `light.uss`。

## 许可证

Apache-2.0（见 LICENSE）。
