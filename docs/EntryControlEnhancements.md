# Entry Control Enhancements

This document describes the new features added to the Entry control for password mode, password character customization, and mobile keyboard options.

## New Properties

### IsPassword
Enables password mode, which masks the entered text with a password character.

**Methods:**
- `SetIsPassword(bool isPassword)` - Sets whether the entry should be displayed as a password field
- `BindIsPassword(string propertyName, Func<bool> propertyGetter)` - Binds the property to a view model

**Example:**
```csharp
new Entry()
    .SetIsPassword(true)
    .SetText("secret");
```

### PasswordChar
Defines the character used to mask text in password mode. Default is '•'.

**Methods:**
- `SetPasswordChar(char passwordChar)` - Sets the password masking character
- `BindPasswordChar(string propertyName, Func<char> propertyGetter)` - Binds the property to a view model

**Example:**
```csharp
new Entry()
    .SetIsPassword(true)
    .SetPasswordChar('*');
```

### Keyboard
Specifies the keyboard type for mobile devices, optimizing input for specific data types.

**Available Types:**
- `KeyboardType.Default` - Default keyboard
- `KeyboardType.Text` - Text keyboard
- `KeyboardType.Numeric` - Numeric keyboard
- `KeyboardType.Email` - Email keyboard with @ and . shortcuts
- `KeyboardType.Telephone` - Phone number keyboard
- `KeyboardType.Url` - URL keyboard with / and .com shortcuts

**Methods:**
- `SetKeyboard(KeyboardType keyboard)` - Sets the keyboard type
- `BindKeyboard(string propertyName, Func<KeyboardType> propertyGetter)` - Binds the property to a view model

**Example:**
```csharp
new Entry()
    .SetKeyboard(KeyboardType.Email)
    .SetText("user@example.com");
```

### ReturnKey
Specifies the return key type for mobile devices, providing context-appropriate actions.

**Available Types:**
- `ReturnKeyType.Default` - Default return key
- `ReturnKeyType.Go` - "Go" action
- `ReturnKeyType.Send` - "Send" action
- `ReturnKeyType.Search` - "Search" action
- `ReturnKeyType.Next` - "Next" action (move to next field)
- `ReturnKeyType.Done` - "Done" action (dismiss keyboard)

**Methods:**
- `SetReturnKey(ReturnKeyType returnKey)` - Sets the return key type
- `BindReturnKey(string propertyName, Func<ReturnKeyType> propertyGetter)` - Binds the property to a view model

**Example:**
```csharp
new Entry()
    .SetKeyboard(KeyboardType.Email)
    .SetReturnKey(ReturnKeyType.Send);
```

## Complete Example

Here's a complete example showing how to use all the new features together:

```csharp
// Email field with email keyboard and send button
new Entry()
    .BindText(nameof(vm.Email), () => vm.Email, (v) => vm.Email = v)
    .SetKeyboard(KeyboardType.Email)
    .SetReturnKey(ReturnKeyType.Send)
    .SetPadding(new Margin(10, 8))
    .SetCornerRadius(10);

// Phone field with telephone keyboard
new Entry()
    .BindText(nameof(vm.Phone), () => vm.Phone, (v) => vm.Phone = v)
    .SetKeyboard(KeyboardType.Telephone)
    .SetReturnKey(ReturnKeyType.Done)
    .SetPadding(new Margin(10, 8))
    .SetCornerRadius(10);

// Password field with custom masking character
new Entry()
    .BindText(nameof(vm.Password), () => vm.Password, (v) => vm.Password = v)
    .SetIsPassword(true)
    .SetPasswordChar('*')
    .SetReturnKey(ReturnKeyType.Done)
    .SetPadding(new Margin(10, 8))
    .SetCornerRadius(10);
```

## Platform Support

- **Android**: Full support for all keyboard types and return key types
- **Desktop**: Keyboard and return key types are ignored (not applicable)
- **iOS**: Interface updated but implementation pending

## Method Chaining

All methods return the Entry instance, allowing for fluent method chaining:

```csharp
var entry = new Entry()
    .SetIsPassword(true)
    .SetPasswordChar('•')
    .SetKeyboard(KeyboardType.Default)
    .SetReturnKey(ReturnKeyType.Done)
    .SetPadding(new Margin(10, 8))
    .SetCornerRadius(10);
```
