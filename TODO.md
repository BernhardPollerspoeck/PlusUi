TODO

## Progress Overview

| Section | Description | Done | Total | Progress | Remaining |
|---------|-------------|------|-------|----------|-----------|
| 0 | Random stuff | 11 | 12 | 🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩🟩⬜ 92% | ~1 week |
| 0.5 | App Default Style | 0 | 3 | ⬜⬜⬜ 0% | ~2 days |
| 3 | DebugServer: Change Tracking | 0 | 6 | ⬜⬜⬜⬜⬜⬜ 0% | ~2 days |
| **Total** | | **11** | **21** | **52%** | **~2 weeks** |

---

**Checkbox Legend:** `[Done] [Verified]`
- First checkbox: Implementation completed by Claude
- Second checkbox: Tested and verified by user

## High Priority

### 0. Random stuff
- [x] [x] 999.1 5️⃣ centralized default values for properties (e.g., default font size, color) to avoid magic numbers scattered in code
- [x] [x] 999.2 3️⃣ complete code comments and XML documentation for all public methods and classes
- [x] [x] 999.3 2️⃣ ensure docs for github pages are up to date with latest features and usage instructions
- [ ] [ ] 999.5 7️⃣ analyze what takes all the memory since sandbox requires about 350mb on startup
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
