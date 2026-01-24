# 4. Type-Safe Data Binding

**Narration:**

> "Data binding. The thing that connects your UI to your data.
>
> In most frameworks, you write a string. You hope it matches a property name. You run the app. It crashes. Or worse - it silently fails and you spend an hour debugging.
>
> In PlusUi, binding is type-safe. You write an expression. The compiler knows what you mean. Your IDE knows what you mean. Autocomplete works. Refactoring works.
>
> Every control that displays data has a `Bind` method. `BindText`, `BindValue`, `BindCommand`, `BindVisibility` - you bind directly to your ViewModel properties using lambda expressions.
>
> Change a property in your ViewModel? The UI updates automatically. User types in a text field? Your ViewModel gets the new value. Two-way binding, built in.
>
> Need to format a value? In other frameworks, you'd write a converter class. Implement an interface. Register it somewhere. In PlusUi - just pass a lambda. Bind an integer, format it as currency, done. One line.
>
> And because it's all C# expressions, your IDE is your safety net. Rename a property? Find all references includes your bindings. Delete something you shouldn't have? Compiler error, not a runtime crash.
>
> Works seamlessly with CommunityToolkit.Mvvm - ObservableObject, RelayCommand, all of it.
>
> Type-safe binding with inline formatting. The way it should be."

**Visual:**

> TODO - Show binding code example, then demonstrate live update in UI
