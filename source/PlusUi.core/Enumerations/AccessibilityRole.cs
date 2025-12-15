namespace PlusUi.core;

/// <summary>
/// Defines semantic roles for UI elements used by assistive technologies.
/// </summary>
public enum AccessibilityRole
{
    /// <summary>
    /// No specific accessibility role.
    /// </summary>
    None,

    /// <summary>
    /// A clickable button control.
    /// </summary>
    Button,

    /// <summary>
    /// A checkbox control that can be checked or unchecked.
    /// </summary>
    Checkbox,

    /// <summary>
    /// A radio button control for single selection from a group.
    /// </summary>
    RadioButton,

    /// <summary>
    /// A text input control.
    /// </summary>
    TextInput,

    /// <summary>
    /// A static text label.
    /// </summary>
    Label,

    /// <summary>
    /// A heading that defines a section.
    /// </summary>
    Heading,

    /// <summary>
    /// A hyperlink to another resource.
    /// </summary>
    Link,

    /// <summary>
    /// An image or graphical element.
    /// </summary>
    Image,

    /// <summary>
    /// A slider control for selecting a value within a range.
    /// </summary>
    Slider,

    /// <summary>
    /// A toggle switch control.
    /// </summary>
    Toggle,

    /// <summary>
    /// A list container.
    /// </summary>
    List,

    /// <summary>
    /// An item within a list.
    /// </summary>
    ListItem,

    /// <summary>
    /// A combo box or dropdown control.
    /// </summary>
    ComboBox,

    /// <summary>
    /// A progress indicator.
    /// </summary>
    ProgressBar,

    /// <summary>
    /// A scrollable view container.
    /// </summary>
    ScrollView,

    /// <summary>
    /// A generic container element.
    /// </summary>
    Container,

    /// <summary>
    /// A modal dialog.
    /// </summary>
    Dialog,

    /// <summary>
    /// An alert or notification.
    /// </summary>
    Alert,

    /// <summary>
    /// A toolbar container.
    /// </summary>
    Toolbar,

    /// <summary>
    /// A menu container.
    /// </summary>
    Menu,

    /// <summary>
    /// A menu item.
    /// </summary>
    MenuItem,

    /// <summary>
    /// A tab control.
    /// </summary>
    Tab,

    /// <summary>
    /// A tab panel content area.
    /// </summary>
    TabPanel,

    /// <summary>
    /// A date picker control.
    /// </summary>
    DatePicker,

    /// <summary>
    /// A time picker control.
    /// </summary>
    TimePicker,

    /// <summary>
    /// A spinner or loading indicator.
    /// </summary>
    Spinner,

    /// <summary>
    /// A tooltip element.
    /// </summary>
    Tooltip,

    /// <summary>
    /// A page or screen.
    /// </summary>
    Page,

    /// <summary>
    /// A navigation container.
    /// </summary>
    Navigation
}
