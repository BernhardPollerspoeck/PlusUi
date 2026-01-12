TODO

## Progress Overview

| Section | Description | Done | Total | Progress | Remaining |
|---------|-------------|------|-------|----------|-----------|
| 0 | Random stuff | 3 | 10 | 🟩🟩🟩⬜⬜⬜⬜⬜⬜⬜ 30% | ~3 weeks |
| 1 | DebugServer: Tree | 6 | 9 | 🟩🟩🟩🟩🟩🟩⬜⬜⬜ 67% | ~1.5 days |
| 2 | DebugServer: Property Grid | 1 | 5 | 🟩⬜⬜⬜⬜ 20% | ~5 hrs |
| 3 | DebugServer: Change Tracking | 0 | 6 | ⬜⬜⬜⬜⬜⬜ 0% | ~2 days |
| 4 | DebugServer: Logging | 3 | 4 | 🟩🟩🟩⬜ 75% | ~4 hrs |
| 5 | DebugServer: Performance | 0 | 6 | ⬜⬜⬜⬜⬜⬜ 0% | ~2.5 days |
| 6 | DebugServer: Screenshot | 0 | 3 | ⬜⬜⬜ 0% | ~5 hrs |
| 7 | DebugServer: Logging Cleanup | 3 | 3 | 🟩🟩🟩 100% | ✅ Done |
| 8 | DebugServer: Layout Vis | 1 | 2 | 🟩⬜ 50% | ~2 hrs |
| 9 | DebugServer: Multi-App | 5 | 9 | 🟩🟩🟩🟩🟩⬜⬜⬜⬜ 56% | ~4 days |
| **Total** | | **22** | **57** | **39%** | **~5 weeks** |

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
- [ ] [ ] 999.1 5️⃣ centralized default values for properties (e.g., default font size, color) to avoid magic numbers scattered in code
- [x] [x] 999.2 3️⃣ complete code comments and XML documentation for all public methods and classes
- [x] [x] 999.3 2️⃣ ensure docs for github pages are up to date with latest features and usage instructions
- [ ] [ ] 999.4 9️⃣ rework text rendering to maybe use native text rendering lib for better performance and quality
- [ ] [ ] 999.5 7️⃣ analyze what takes all the memory since sandbox requires about 350mb on startup
- [ ] [ ] 999.6 🔟 think about a fully custom rendering engine that does not need skia at all.
- [ ] [ ] 999.7 4️⃣ ensure all code is up to dotnet 10 standards and best practices
- [ ] [ ] 999.8 3️⃣ clear out all warnings and messages.
- [x] [x] 999.9 4️⃣ ensure all not public api surfaces are internal only
- [ ] [ ] 999.10 5️⃣ return ServiceProviderService.ServiceProvider?.GetService<IPaintRegistryService>() wirft Exception in UiElment beim stoppen



### 1. DebugServer Tree Improvements
- [ ] [ ] 1.1 6️⃣ Update tree incrementally instead of full reload (including better navigation handling when pages change)
- [x] [x] 1.2 Add page prefix to element IDs for uniqueness (e.g., `MainPage.Button_123`)
- [ ] [ ] 1.3 4️⃣ Maintain expansion state during updates
- [x] [x] 1.4 Auto-expand first 2-3 levels on tree load
- [x] [x] 1.5 Fix TreeView scrolling - can scroll too far down until all content is off-screen
- [ ] [ ] 1.6 4️⃣ Fix TreeView rendering bug - text disappears when scrolling in adjacent TreeView (clipping issue)
- [x] [x] 1.7 Add selection highlight to TreeView - show colored background for selected item
- [x] [x] 1.8 Add scrollbar visualization to TreeView - currently no visual scrollbar shown
- [x] [x] 1.9 Fix TreeView expand/collapse click area - click position is registered too high up

### 2. DebugServer Property Grid Improvements
- [x] [ ] 2.1 Pin properties per element type (implemented with BindText/BindTextColor)
- [ ] [ ] 2.2 3️⃣ Pinned properties appear at top of property list (BUG: shows pinned properties at top for ALL elements, not filtered by element type)
- [ ] [ ] 2.3 3️⃣ Fix property display bugs - colors not displayed correctly
- [ ] [ ] 2.4 2️⃣ Fix property offset display - selection feels incorrect
- [ ] [ ] 2.5 4️⃣ Fix color property update - color values cannot be updated

### 3. DebugServer Change Tracking & Reset
- [ ] [ ] 3.1 5️⃣ Track all property changes made in debug session
- [ ] [ ] 3.2 4️⃣ Persist changes across navigation (re-apply when navigating back to page)
- [ ] [ ] 3.3 3️⃣ Generate change list: which element, which values changed
- [ ] [ ] 3.4 2️⃣ "Reload Page" button to discard all debug changes
- [ ] [ ] 3.5 2️⃣ Show modified indicator on changed properties
- [ ] [ ] 3.6 6️⃣ Style overrides (per page and global)

### 4. DebugServer Logging
- [x] [x] 4.1 Log output panel in debug server UI
- [x] [x] 4.2 Log level filtering (Debug, Info, Warning, Error)
- [x] [x] 4.3 Cleanup: Remove console.log, use ILogger everywhere
- [ ] [ ] 4.4 5️⃣ Event tracing (button clicks, navigation, etc.)

### 5. DebugServer Performance Monitor
- [ ] [ ] 5.1 4️⃣ FPS counter
- [ ] [ ] 5.2 4️⃣ Memory usage
- [ ] [ ] 5.3 4️⃣ Render time per frame
- [ ] [ ] 5.4 5️⃣ Frame time graph
- [ ] [ ] 5.5 6️⃣ Render event graph (when renders actually happen - for on-demand rendering)
- [ ] [ ] 5.6 2️⃣ Render utilization % (how much of 60fps is actually used)

## Medium Priority

### 6. DebugServer Screenshot
- [ ] [ ] 6.1 4️⃣ Capture current UI state as image
- [ ] [ ] 6.2 3️⃣ Save to file or clipboard
- [ ] [ ] 6.3 4️⃣ Capture specific element or full page

### 7. DebugServer Logging Cleanup
- [x] [x] 7.1 Audit codebase for Console.WriteLine/Debug.WriteLine
- [x] [x] 7.2 Replace with ILogger calls
- [x] [x] 7.3 Ensure proper log levels

## Low Priority / Future

### 8. DebugServer Layout Visualization
- [ ] [ ] 8.1 4️⃣ Wireframe overlay showing element bounds (server-side feature to toggle debug visualization remotely)
- [x] [x] 8.2 Margin/padding visualization (PlusUi.core - implemented with IsDebug property)

### 9. DebugServer UI Layout & Multi-App Support
**Target Layout:**
```
┌──────────────────────────────────────────────────────────┐
│ [● App 1 ×] [○ App 2 ×] [+]                              │  ← App Tabs
├──────────────────────────────────────────────────────────┤
│ FPS: 60 (100%) │ Memory: 45MB │ Render: 16ms │ Modified │  ← Status Bar
├──────────────────────────────────────────────────────────┤
│ [Inspector] [Logs] [Performance] [Changes] [Screenshot]  │  ← Feature Tabs
│ ┌────────────────────────┬─────────────────────────────┐ │
│ │   Tree (left)          │   Property Grid (right)     │ │  ← Tab Content
│ └────────────────────────┴─────────────────────────────┘ │
└──────────────────────────────────────────────────────────┘
```

**Tasks:**
- [x] [x] 9.1 Multi-App Support in ViewModel
  - Dictionary<string, AppState> to manage multiple connected clients
  - Each AppState contains: clientId, connected status, tree data, selected node, properties, changes list
  - Switch between apps updates all UI accordingly

- [x] [x] 9.2 App Tabs Component
  - Use PlusUi.core TabControl with BindTabs() instead of custom control
  - Each connected app = one TabItem
  - Tab header shows app name with LED indicator: ● (green) for connected
  - Selected tab determines active app in ViewModel
  - TabControl automatically handles dynamic tab addition/removal via bindings

- [~] 9.3 Global Status Bar Component (Placeholder layout done, real data pending)
  - [x] [x] Placeholder layout with static text
  - [ ] [ ] 4️⃣ FPS counter with utilization % (received from app via WebSocket)
  - [ ] [ ] 4️⃣ Memory usage (received from app)
  - [ ] [ ] 3️⃣ Last render time in ms (received from app)
  - [ ] [ ] 2️⃣ Modified indicator (shows if current app has unsaved changes)
  - [x] [x] Always visible regardless of selected feature tab

- [~] 9.4 Feature TabView Component (Placeholder tabs done, content pending)
  - [x] [x] TabView with 5 tabs: Inspector, Logs, Performance, Changes, Screenshot
  - [x] [x] Each tab shows content for currently selected app (from App Tabs)
  - [x] [x] Tab content updates when switching apps
  - [ ] [ ] 5️⃣ Actual content for each tab (currently placeholders)

- [x] [x] 9.5 Inspector Tab (refactor existing)
  - Left: TreeView showing element hierarchy (already exists)
  - Right: Property Grid with pinning support (already exists)
  - Split view within tab content area

- [x] [ ] 9.6 Logs Tab
  - Log output panel receiving logs from app via WebSocket
  - Level filtering buttons: [TRC] [DBG] [INF] [WRN] [ERR] [CRT]
  - Clear button
  - Auto-scroll and search/filter not yet implemented

- [ ] [ ] 9.7 7️⃣ Performance Tab
  - Live FPS graph (received from app)
  - Render event graph showing when frames are rendered
  - Memory usage over time
  - Frame time breakdown
  - Render utilization chart

- [ ] [ ] 9.8 5️⃣ Changes Tab
  - List of all property changes made during debug session
  - Format: "ElementId.PropertyPath: oldValue → newValue"
  - Per-app change tracking
  - [Reload Page] button to discard all changes for current app
  - [Export] button to copy changes as code/JSON

- [ ] [ ] 9.9 5️⃣ Screenshot Tab
  - Preview area showing captured screenshot
  - [Capture Full Page] button
  - [Capture Selected Element] button (uses tree selection)
  - [Save to File] and [Copy to Clipboard] buttons
  - Screenshot received from app via WebSocket as base64 image
