# Debug Server TODO

## High Priority

### 1. Tree Improvements
- [ ] 1.1 Update tree incrementally instead of full reload
- [ ] 1.2 Add page prefix to element IDs for uniqueness (e.g., `MainPage.Button_123`)
- [ ] 1.3 Better navigation handling when pages change
- [ ] 1.4 Maintain expansion state during updates
- [ ] 1.5 Auto-expand first 2-3 levels on tree load

### 2. Property Grid Improvements
- [ ] 2.1 Pin properties per element type
- [ ] 2.2 Pinned properties appear at top of property list

### 3. Change Tracking & Reset
- [ ] 3.1 Track all property changes made in debug session
- [ ] 3.2 Persist changes across navigation (re-apply when navigating back to page)
- [ ] 3.3 Generate change list: which element, which values changed
- [ ] 3.4 "Reload Page" button to discard all debug changes
- [ ] 3.5 Show modified indicator on changed properties
- [ ] 3.6 Style overrides (per page and global)

### 4. Logging
- [ ] 4.1 Log output panel in debug server UI
- [ ] 4.2 Log level filtering (Debug, Info, Warning, Error)
- [ ] 4.3 Cleanup: Remove console.log, use ILogger everywhere
- [ ] 4.4 Event tracing (button clicks, navigation, etc.)

### 5. Performance Monitor
- [ ] 5.1 FPS counter
- [ ] 5.2 Memory usage
- [ ] 5.3 Render time per frame
- [ ] 5.4 Frame time graph
- [ ] 5.5 Render event graph (when renders actually happen - for on-demand rendering)
- [ ] 5.6 Render utilization % (how much of 60fps is actually used)

## Medium Priority

### 6. Screenshot
- [ ] 6.1 Capture current UI state as image
- [ ] 6.2 Save to file or clipboard
- [ ] 6.3 Capture specific element or full page

### 7. Logging Cleanup
- [x] 7.1 Audit codebase for Console.WriteLine/Debug.WriteLine
- [x] 7.2 Replace with ILogger calls
- [x] 7.3 Ensure proper log levels

## Low Priority / Future

### 8. Layout Visualization
- [ ] 8.1 Wireframe overlay showing element bounds (server-side feature to toggle debug visualization remotely)
- [x] 8.2 Margin/padding visualization (PlusUi.core - implemented with IsDebug property)

### 9. UI Layout & Multi-App Support
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
- [ ] 9.1 Multi-App Support in ViewModel
  - Dictionary<string, AppState> to manage multiple connected clients
  - Each AppState contains: clientId, connected status, tree data, selected node, properties, changes list
  - Switch between apps updates all UI accordingly

- [ ] 9.2 App Tabs Component
  - Horizontal tabs showing connected apps
  - LED indicator: ● (green) for connected, ○ (gray) for disconnected
  - Close button (×) per tab to disconnect specific app
  - [+] button to manually add/connect app
  - Selected tab determines active app in ViewModel

- [ ] 9.3 Global Status Bar Component
  - FPS counter with utilization % (received from app via WebSocket)
  - Memory usage (received from app)
  - Last render time in ms (received from app)
  - Modified indicator (shows if current app has unsaved changes)
  - Always visible regardless of selected feature tab

- [ ] 9.4 Feature TabView Component
  - TabView with 5 tabs: Inspector, Logs, Performance, Changes, Screenshot
  - Each tab shows content for currently selected app (from App Tabs)
  - Tab content updates when switching apps

- [ ] 9.5 Inspector Tab (refactor existing)
  - Left: TreeView showing element hierarchy (already exists)
  - Right: Property Grid with pinning support (already exists)
  - Split view within tab content area

- [ ] 9.6 Logs Tab
  - Log output panel receiving logs from app via WebSocket
  - Level filtering buttons: [All] [Debug] [Info] [Warning] [Error]
  - Auto-scroll toggle
  - Clear button
  - Search/filter input

- [ ] 9.7 Performance Tab
  - Live FPS graph (received from app)
  - Render event graph showing when frames are rendered
  - Memory usage over time
  - Frame time breakdown
  - Render utilization chart

- [ ] 9.8 Changes Tab
  - List of all property changes made during debug session
  - Format: "ElementId.PropertyPath: oldValue → newValue"
  - Per-app change tracking
  - [Reload Page] button to discard all changes for current app
  - [Export] button to copy changes as code/JSON

- [ ] 9.9 Screenshot Tab
  - Preview area showing captured screenshot
  - [Capture Full Page] button
  - [Capture Selected Element] button (uses tree selection)
  - [Save to File] and [Copy to Clipboard] buttons
  - Screenshot received from app via WebSocket as base64 image
