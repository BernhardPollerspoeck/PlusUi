PlusUi is a UiFramework with Fluint style ui building.

### rules
- All UiElements need to inherit from UiElement or UiElement<T>.
- All UiLayoutElements need to inherit from UiLayoutElement or UiLayoutElement<T>.
- All Pages need to inherit from UiPageElement.
- All UserControls need to inherit from UserControl.
- All ViewModels need to inherit from ViewModelBase.
- All Properties for Elements and Controls have a Set and Bind method.
- All the Properties never are set directly. They are set using the Set or Bind methods.
- ViewModel property implementations should use c# 13 field syntax.
- Bind methods have a property getter. controls that take input there is also a optional property setter.
- controls and basic blocks descibed following not only provide the properties (for methods) as mentiones but also provide the properties form the base classes.
- Required Namespaces are: PlusUi.core, Skiasharp, System.Windows.Input.
- command implementations are SyncCommand and AsyncCommand.
- properties below have theyre type in brackets behind. e.g. Text(string) means the Property Text is of type string and the set and bind methods take strings. e.g. ""public UiTextElement SetText(string text)" and "public UiTextElement BindText(string propertyName, Func<string?> propertyGetter)" 
- properties described for the element have to come first to configure the element and the more generic properties come after that. e.g. SetText(string text) comes before SetBackgroundColor(SKColor color).

## Basic Blocks

### Pages
- Pages need to Inherit from UiPageElement. The ViewModel needs to inherit from ViewModelBase.
- Ui is build in Build() method of the page.
- UiPageElement inherits from UiLayoutElement<UiPageElement>.
- Styling scoped to the page can be done in ConfigurePageStyles(Style pageStyle) method.
- Page provides the lifecycle methods Appearing() and Disappearing().
- Properties for the page can be set in Build() before returning the Ui.

### UserControls
- UserControls need to inherit from UserControl.
- Ui is build in Build() method of the UserControl.
- UserControl inherits from UiElement<UserControl>.

### UiElement
- UiElement is the building blocks of the Ui.
- UiElements need to inherit from UiElement or UiElement<T>.
- UiElement provides the basic properties Debug, BackgroundColor, Margin, HorizontalAlignment, VerticalAlignment, CornerRadius, DesiredSize.

### UiTextElement
- UiTextElement is the building blocks of the Ui with text.
- UiTextElements need to inherit from UiTextElement or UiTextElement<T>.
- UiTextElement inherits from UiElement
- UiTextElement provides the basic properties Text, TextSize, TextColor, HorizontalTextAlignment.

### UiLayoutElement
- UiLayoutElement is the building blocks of the Ui with children.
- UiLayoutElements need to inherit from UiLayoutElement or UiLayoutElement<T>.
- UiLayoutElement provides the basic property Children.

## Controls

### HStack & VStack
- HStack and VStack are layout elements that stack children horizontally or vertically.
- HStack and VStack inherit from UiLayoutElement<HStack> and UiLayoutElement<VStack> respectively.

### Label
- Label is a UiTextElement that displays text.
- Label inherits from UiTextElement<Label>.

### Solid
- Solid is a UiElement with a solid background color.
- Solid inherits from UiElement<Solid>.
- can be used transparent to act as a spacer.

### Image
- Image is a UiElement with an image.
- Image inherits from UiElement<Image>.
- Image provides the basic property ImageSource and Aspect.

### Entry
- Entry is a UiTextElement with a text input.
- Entry inherits from UiTextElement<Entry>.
- Entry provides the basic property Padding.

### CheckBox
- CheckBox is a UiElement with a checkbox.
- CheckBox inherits from UiElement<CheckBox>.
- CheckBox provides the basic property IsChecked.
- The public method Toggle is not intended to be used by the user.

### Button
- Button is a UiTextElement with a button.
- Button inherits from UiTextElement<Button>.
- Button provides the basic property Padding and Command.

### Grid
- Grid is a UiLayoutElement with a grid layout.
- Grid inherits from UiLayoutElement<Grid>.
- Grid provides the basic property Rows, Columns, RowSpacing, ColumnSpacing.


## Example
```csharp
public class MainPage(MainPageViewModel vm) : UiPageElement(vm)
{
    protected override UiElement Build()
    {
        SetBackgroundColor(SKColors.SlateBlue);
        return new HStack(
            new VStack(
                
                new Label()
                    .BindText(nameof(vm.Text), () => $"The entry input is: [ {vm.Text} ]"),
                new Entry()
                    .BindText(nameof(vm.Text), () => vm.Text, txt => vm.Text = txt),

                new Label()
                    .SetText("Hit the button below to Change my color")
                    .BindTextColor(nameof(vm.Color), () => vm.Color),
                new HStack(
                    new Button()
                        .SetPadding(new(10, 5))
                        .SetCommand(vm.SetColorCommand)
                        .SetText("Hello World!"),
                    new Checkbox()
                        .BindIsChecked(nameof(vm.Checked), () => vm.Checked, isChecked => vm.Checked = isChecked)
                        .SetBackgroundColor(new SKColor(255, 0, 0)),

                new Image()
                    .SetAspect(Aspect.AspectFit)
                    .SetImageSource("plusui.png")
                    .SetVerticalAlignment(VerticalAlignment.Bottom)
                    .SetDesiredWidth(100)
                    .SetDesiredHeight(100)))
                    );
    }

    protected override void ConfigurePageStyles(Style pageStyle)
    {
        pageStyle.AddStyle<UiPageElement>(element => element.SetBackgroundColor(SKColors.Black));
    }

    public override void Appearing()
    {
        base.Appearing();
        vm.SetColorCommand.Execute(null);
    }
}
```
```csharp
public class MainPageViewModel : ViewModelBase
{
    public string Text
    {
        get => field;
        set => SetProperty(ref field, value);
    }
    public SKColor Color
    {
        get => field;
        set => SetProperty(ref field, value);
    }
    public bool Checked
    {
        get => field;
        set => SetProperty(ref field, value);
    }
    public ICommand SetColorCommand { get; }
    public MainPageViewModel()
    {
        SetColorCommand = new Command(() => Color = Color == SKColors.White ? SKColors.Black : SKColors.White);
    }
}
```



