# Duplicated Control Properties Report

Properties listed by the number of controls they are **independently defined in** (not inherited from a base class).

**Legend:**
- ⚠️ = Inconsistent implementations across controls - manual review required
- ~~strikethrough~~ ✅ = Resolved via UiPropGen source generator

**Note:** DataGrid column types (DataGridComboBoxColumn, etc.) are configuration objects, not UiElements. They don't use UiPropGen - they simply pass values through to the actual controls they create.

---

## High Duplication (6+ controls)

| Property | Count | Controls |
|----------|-------|----------|
| Header | 15 | DataGridButtonColumn, DataGridCheckboxColumn, DataGridComboBoxColumn, DataGridDatePickerColumn, DataGridEditorColumn, DataGridImageColumn, DataGridLinkColumn, DataGridProgressColumn, DataGridSliderColumn, DataGridTemplateColumn, DataGridTextColumn, DataGridTimePickerColumn, DataGrid, TabControl, TabItem |
| ⚠️ Width | 13 | DataGridButtonColumn, DataGridCheckboxColumn, DataGridComboBoxColumn, DataGridDatePickerColumn, DataGridEditorColumn, DataGridImageColumn, DataGridLinkColumn, DataGridProgressColumn, DataGridSliderColumn, DataGridTemplateColumn, DataGridTextColumn, DataGridTimePickerColumn, Scrollbar |
| Command | 12 | Button, DataGridButtonColumn, DataGridLinkColumn, DoubleTapGestureDetector, LongPressGestureDetector, PinchGestureDetector, SwipeGestureDetector, TapGestureDetector, ContextMenu, Menu, MenuItem, Toolbar |
| Binding | 10 | DataGridCheckboxColumn, DataGridComboBoxColumn, DataGridDatePickerColumn, DataGridEditorColumn, DataGridImageColumn, DataGridLinkColumn, DataGridProgressColumn, DataGridSliderColumn, DataGridTextColumn, DataGridTimePickerColumn |
| ⚠️ TextColor | 9 | ComboBox, DataGrid, DataGridLinkColumn, ContextMenu, Menu, DatePicker, TimePicker, RadioButton, Label, Link, Toolbar (auch in UiTextElement base) |
| ⚠️ TextSize | 7 | ComboBox, DataGrid, Menu, DatePicker, TimePicker, RadioButton, Toolbar (auch in UiTextElement base) |
| ⚠️ HoverBackground | 7 | Button, ComboBox, ContextMenu, Menu, DatePicker, TimePicker, Toolbar |
| ~~Placeholder~~ | 4 | ~~ComboBox, DatePicker, TimePicker, Entry~~ ✅ via `[UiPropGenPlaceholder]` |
| CommandParameter | 6 | Button, DataGridButtonColumn, DataGridLinkColumn, TapGestureDetector, MenuItem, Toolbar |
| ~~Padding~~ | 6 | ~~Button, ComboBox, DatePicker, TimePicker, Entry, Toolbar~~ ✅ via `[UiPropGenPadding]` |

---

## Medium Duplication (4-5 controls)

| Property | Count | Controls |
|----------|-------|----------|
| ⚠️ Icon | 5 | Button, MenuItem, TabItem, Toolbar, ToolbarIconGroup |
| ⚠️ Content | 5 | TabControl, TabItem, Toolbar, TooltipAttachment, TooltipExtensions |
| ⚠️ ItemsSource | 5 | ComboBox, DataGrid, DataGridComboBoxColumn, ItemsList, TreeView |
| ⚠️ SelectedItem | 4 | ComboBox, DataGrid, DataGridComboBoxColumn, TreeView |
| DisplayFormat | 4 | DataGridDatePickerColumn, DataGridTimePickerColumn, DatePicker, TimePicker |
| ThumbColor | 4 | DataGridSliderColumn, Scrollbar, Slider, Toggle |
| ~~PlaceholderColor~~ | 4 | ~~ComboBox, DatePicker, TimePicker, Entry~~ ✅ via `[UiPropGenPlaceholderColor]` |
| FontFamily | 4 | ComboBox, DatePicker, TimePicker (auch in UiTextElement base) |
| Orientation | 4 | DataGrid, ItemsList, Scrollbar, Separator |

---

## Low Duplication (2-3 controls)

| Property | Count | Controls |
|----------|-------|----------|
| ⚠️ IsOpen | 3 | ComboBox, DatePicker, TimePicker |
| IsChecked | 3 | Checkbox, DataGridCheckboxColumn, MenuItem |
| Spacing | 3 | HStack, VStack, ToolbarIconGroup |
| TrackColor | 3 | DataGridProgressColumn, ProgressBar, Scrollbar |
| ⚠️ CornerRadius | 3 | Button, ComboBox, Entry (auch in UiElement base) |
| MinDate | 2 | DataGridDatePickerColumn, DatePicker |
| MaxDate | 2 | DataGridDatePickerColumn, DatePicker |
| MinTime | 2 | DataGridTimePickerColumn, TimePicker |
| MaxTime | 2 | DataGridTimePickerColumn, TimePicker |
| MinValue | 2 | DataGridSliderColumn, LineGraph |
| MaxValue | 2 | DataGridSliderColumn, LineGraph |
| MinimumTrackColor | 2 | DataGridSliderColumn, Slider |
| MaximumTrackColor | 2 | DataGridSliderColumn, Slider |
| ProgressColor | 2 | DataGridProgressColumn, ProgressBar |
| SelectedBackground | 2 | DatePicker, TimePicker |
| ScrollOffset | 2 | DataGrid, ItemsList |
| IsEnabled | 2 | MenuItem, TabItem |
| TintColor | 2 | DataGridImageColumn, Image |

---

## Observations

| Bereich | Beschreibung |
|---------|--------------|
| DataGrid Columns | Alle 12 DataGrid-Spaltentypen duplizieren Header, Width und Binding. Konsolidierung in DataGridColumn-Basisklasse möglich. |
| Text Properties | TextColor, TextSize, FontFamily sind in UiTextElement definiert, aber auch in ComboBox, DatePicker, TimePicker, RadioButton. Diese Controls erben nicht von UiTextElement. |
| ~~Input Controls~~ | ~~Placeholder, PlaceholderColor und Padding sind in ComboBox, DatePicker, TimePicker und Entry dupliziert.~~ ✅ Alle drei via UiPropGen gelöst. |
| Command Pattern | Command und CommandParameter sind in Gesture Detectors, Buttons und Menu Items dupliziert. ICommandable Interface wäre sinnvoll. |
| DatePicker/TimePicker | Teilen viele Properties (DisplayFormat, Placeholder, PlaceholderColor, IsOpen, SelectedBackground). Gemeinsame PickerBase-Klasse wäre sinnvoll. |

---

## Empfohlene Refactorings

| Priorität | Refactoring | Betroffene Properties | Betroffene Controls |
|-----------|-------------|----------------------|---------------------|
| Hoch | DataGridColumn Basisklasse erweitern | Header, Width, Binding | Alle 12 DataGridColumn-Typen |
| Hoch | ICommandable Interface | Command, CommandParameter | Button, GestureDetectors, MenuItem, Toolbar |
| Mittel | PickerBase Basisklasse | DisplayFormat, Placeholder, PlaceholderColor, IsOpen, SelectedBackground, FontFamily | DatePicker, TimePicker |
| ~~Mittel~~ | ~~ITextInputControl Interface~~ | ~~Placeholder, PlaceholderColor, Padding~~ | ~~ComboBox, DatePicker, TimePicker, Entry~~ ✅ via UiPropGen |
| Niedrig | ISliderControl Interface | MinValue, MaxValue, ThumbColor, MinimumTrackColor, MaximumTrackColor | Slider, DataGridSliderColumn |
| Niedrig | IProgressControl Interface | ProgressColor, TrackColor | ProgressBar, DataGridProgressColumn |

---

## ⚠️ Inkonsistente Implementierungen - Details

Diese Properties haben unterschiedliche Implementierungen und erfordern manuelle Review:

| Property | Problem |
|----------|---------|
| TextColor | Typ variiert: `Color` vs `SKColor`. Unterschiedliche Update-Methoden (`UpdatePaints()`, `InvalidatePaints()`, etc.) |
| TextSize | Unterschiedliche Update-Methoden nach Änderung (`UpdateFont()`, `UpdateTextPaint()`, `InvalidateMeasure()`) |
| ~~PlaceholderColor~~ | ~~Typ variiert: `Color` vs `SKColor`~~ ✅ Vereinheitlicht via `[UiPropGenPlaceholderColor]` |
| HoverBackground | Typ variiert: `IBackground` vs `SKColor`. Unterschiedliches Verhalten |
| Width | Typ variiert: `DataGridColumnWidth` in DataGrid-Columns vs `float` in Scrollbar |
| Icon | Button hat komplexe Image-Loading-Logik. Andere verwenden einfache Referenz |
| Content | Typ variiert stark: `object?`, `Func<object?, UiElement?>`, `UiElement?` |
| ItemsSource | Generic-Typ variiert: `T`, `object?`. Unterschiedliche Collection-Subscriptions |
| SelectedItem | Generic-Typ variiert. Unterschiedliche Selection-Logik |
| IsOpen | Overlay-Management unterschiedlich zwischen ComboBox, DatePicker, TimePicker |
| CornerRadius | Redundant - bereits in UiElement base class definiert |
