TODO

## Progress Overview

| Section | Description | Done | Total | Progress | Remaining |
|---------|-------------|------|-------|----------|-----------|
| 0 | Random stuff | 11 | 14 | 🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩⬜⬜⬜ 79% | ~3 weeks |
| 0.5 | App Default Style | 0 | 3 | ⬜⬜⬜ 0% | ~2 days |
| 1 | DebugServer: Tree | 9 | 9 | 🟩🟩🟩🟩🟩🟩🟩🟩🟩 100% | ✅ Done |
| 2 | DebugServer: Property Grid | 5 | 5 | 🟩🟩🟩🟩🟩 100% | ✅ Done |
| 3 | DebugServer: Change Tracking | 0 | 6 | ⬜⬜⬜⬜⬜⬜ 0% | ~2 days |
| 4 | DebugServer: Logging | 4 | 4 | 🟩🟩🟩🟩 100% | ✅ Done |
| 5 | DebugServer: Performance | 6 | 6 | 🟩🟩🟩🟩🟩🟩 100% | ✅ Done |
| 6 | DebugServer: Screenshot | 6 | 6 | 🟩🟩🟩🟩🟩🟩 100% | ✅ Done |
| 7 | DebugServer: Logging Cleanup | 3 | 3 | 🟩🟩🟩 100% | ✅ Done |
| 8 | DebugServer: Layout Vis | 2 | 2 | 🟩🟩 100% | ✅ Done |
| **Total** | | **46** | **58** | **79%** | **~4 weeks** |

---

**Checkbox Legend:** `[Done] [Verified]`
- First checkbox: Implementation completed by Claude
- Second checkbox: Tested and verified by user

**Complexity Legend:** (Claude Code CLI + User Testing)

| Emoji | Effort | Description |
|-------|--------|-------------|
| 1️⃣ | ~15 min | Trivial, single change |
| 2️⃣ | ~30 min | Very simple, few files |
| 3️⃣ | ~1 hr | Simple, manageable |
| 4️⃣ | ~2 hrs | Moderate, multiple components |
| 5️⃣ | ~4 hrs | Medium effort, half day |
| 6️⃣ | ~1 day | Larger change |
| 7️⃣ | ~2 days | Complex, lots of testing |
| 8️⃣ | ~3-4 days | Very complex |
| 9️⃣ | ~1 week | Architecture change |
| 🔟 | ~2+ weeks | Major project, rewrite |

## High Priority

### 0. Random stuff
- [x] [x] 999.1 5️⃣ centralized default values for properties (e.g., default font size, color) to avoid magic numbers scattered in code
- [x] [x] 999.2 3️⃣ complete code comments and XML documentation for all public methods and classes
- [x] [x] 999.3 2️⃣ ensure docs for github pages are up to date with latest features and usage instructions
- [ ] [ ] 999.4 9️⃣ rework text rendering to maybe use native text rendering lib for better performance and quality
- [ ] [ ] 999.5 7️⃣ analyze what takes all the memory since sandbox requires about 350mb on startup
- [ ] [ ] 999.6 🔟 think about a fully custom rendering engine that does not need skia at all.
- [x] [x] 999.7 4️⃣ ensure all code is up to dotnet 10 standards and best practices
- [x] [x] 999.8 3️⃣ clear out all warnings and messages.
- [x] [x] 999.9 4️⃣ ensure all not public api surfaces are internal only
- [x] [x] 999.10 5️⃣ return ServiceProviderService.ServiceProvider?.GetService<IPaintRegistryService>() wirft Exception in UiElment beim stoppen
- [x] [x] 999.11 6️⃣ Remove remaining on-demand service resolves (ServiceProvider?.GetService<T>()) - cache in constructor instead
- [x] [x] 999.12 4️⃣ Refactor ComboBox to move more T-independent code to non-generic base class
- [x] [x] 999.13 5️⃣ Fix failing layout tests (Button height, Grid width, ItemsList)
- [ ] [ ] 999.14 6️⃣ Define more properties via UiPropGen source generator (see duplicated-properties-report.md)
- [ ] [ ] 999.15 7️⃣ Build-time SVG to ICO conversion for .exe icons (MSBuild task or dotnet tool)
- [x] [x] 999.16 3️⃣ Logo rework: Plus sign looks bad at small sizes (taskbar/titlebar icon)

### 0.5 App Default Style
- [ ] [ ] 0.5.1 5️⃣ Evaluate default values from DebugServer (analyze current styles and derive sensible defaults)
- [ ] [ ] 0.5.2 4️⃣ Testing of default styles across all controls
- [ ] [ ] 0.5.3 7️⃣ Rework Sandbox to full control library showcase (TabView with tabs on the left, all controls displayed)

**Controls:**
- [ ] [ ] Page
- [ ] [ ] ActivityIndicator
- [ ] [ ] Border
- [ ] [ ] Button
- [ ] [ ] Checkbox
- [ ] [ ] ComboBox
- [ ] [ ] ContextMenu
- [ ] [ ] DatePicker
- [ ] [ ] Entry
- [ ] [ ] Grid
- [ ] [ ] HStack
- [ ] [ ] Image
- [ ] [ ] ItemsList
- [ ] [ ] Label
- [ ] [ ] LineGraph
- [ ] [ ] Link
- [ ] [ ] Menu
- [ ] [ ] ProgressBar
- [ ] [ ] RadioButton
- [ ] [ ] Scrollbar
- [ ] [ ] ScrollView
- [ ] [ ] Separator
- [ ] [ ] Slider
- [ ] [ ] Solid
- [ ] [ ] TabControl
- [ ] [ ] TimePicker
- [ ] [ ] Toggle
- [ ] [ ] Toolbar
- [ ] [ ] ToolbarIconGroup
- [ ] [ ] TreeView
- [ ] [ ] UniformGrid
- [ ] [ ] VStack

**User Controls & Popups:**
- [ ] [ ] UserControl (base class)
- [ ] [ ] RawUserControl (base class)
- [ ] [ ] UiPopupElement (base class)

### ✅ 1. DebugServer Tree Improvements

### ✅ 2. DebugServer Property Grid Improvements

### 3. DebugServer Change Tracking & Reset
- [ ] [ ] 3.1 5️⃣ Track all property changes made in debug session
- [ ] [ ] 3.2 4️⃣ Persist changes across navigation (re-apply when navigating back to page)
- [ ] [ ] 3.3 3️⃣ Generate change list: which element, which values changed
- [ ] [ ] 3.4 2️⃣ "Reload Page" button to discard all debug changes
- [ ] [ ] 3.5 2️⃣ Show modified indicator on changed properties
- [ ] [ ] 3.6 6️⃣ Style overrides (per page and global)

### ✅ 4. DebugServer Logging

### ✅ 5. DebugServer Performance Monitor

### ✅ 6. DebugServer Screenshot

### ✅ 7. DebugServer Logging Cleanup

### ✅ 8. DebugServer Layout Visualization
