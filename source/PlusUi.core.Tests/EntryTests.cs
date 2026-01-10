using Microsoft.VisualStudio.TestTools.UnitTesting;
using PlusUi.core;

namespace PlusUi.core.Tests;

[TestClass]
public class EntryTests
{
    [TestMethod]
    public void Entry_SetIsPassword_ShouldSetProperty()
    {
        // Arrange
        var entry = new Entry();

        // Act
        entry.SetIsPassword(true);

        // Assert
        Assert.IsTrue(entry.IsPassword);
    }

    [TestMethod]
    public void Entry_BindIsPassword_ShouldBindProperty()
    {
        // Arrange
        var entry = new Entry();
        var isPassword = false;

        // Act
        entry.BindIsPassword(() => isPassword);
        entry.UpdateBindings();

        // Assert
        Assert.IsFalse(entry.IsPassword);

        // Change value and update
        isPassword = true;
        entry.UpdateBindings();

        // Assert
        Assert.IsTrue(entry.IsPassword);
    }

    [TestMethod]
    public void Entry_SetPasswordChar_ShouldSetProperty()
    {
        // Arrange
        var entry = new Entry();

        // Act
        entry.SetPasswordChar('*');

        // Assert
        Assert.AreEqual('*', entry.PasswordChar);
    }

    [TestMethod]
    public void Entry_PasswordChar_DefaultShouldBeBullet()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.AreEqual('•', entry.PasswordChar);
    }

    [TestMethod]
    public void Entry_BindPasswordChar_ShouldBindProperty()
    {
        // Arrange
        var entry = new Entry();
        var passwordChar = '*';

        // Act
        entry.BindPasswordChar(() => passwordChar);
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual('*', entry.PasswordChar);

        // Change value and update
        passwordChar = '#';
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual('#', entry.PasswordChar);
    }

    [TestMethod]
    public void Entry_SetKeyboard_ShouldSetProperty()
    {
        // Arrange
        var entry = new Entry();

        // Act
        entry.SetKeyboard(KeyboardType.Email);

        // Assert
        Assert.AreEqual(KeyboardType.Email, entry.Keyboard);
    }

    [TestMethod]
    public void Entry_Keyboard_DefaultShouldBeDefault()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.AreEqual(KeyboardType.Default, entry.Keyboard);
    }

    [TestMethod]
    public void Entry_BindKeyboard_ShouldBindProperty()
    {
        // Arrange
        var entry = new Entry();
        var keyboard = KeyboardType.Numeric;

        // Act
        entry.BindKeyboard(() => keyboard);
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual(KeyboardType.Numeric, entry.Keyboard);

        // Change value and update
        keyboard = KeyboardType.Email;
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual(KeyboardType.Email, entry.Keyboard);
    }

    [TestMethod]
    public void Entry_SetReturnKey_ShouldSetProperty()
    {
        // Arrange
        var entry = new Entry();

        // Act
        entry.SetReturnKey(ReturnKeyType.Send);

        // Assert
        Assert.AreEqual(ReturnKeyType.Send, entry.ReturnKey);
    }

    [TestMethod]
    public void Entry_ReturnKey_DefaultShouldBeDefault()
    {
        // Arrange & Act
        var entry = new Entry();

        // Assert
        Assert.AreEqual(ReturnKeyType.Default, entry.ReturnKey);
    }

    [TestMethod]
    public void Entry_BindReturnKey_ShouldBindProperty()
    {
        // Arrange
        var entry = new Entry();
        var returnKey = ReturnKeyType.Go;

        // Act
        entry.BindReturnKey(() => returnKey);
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual(ReturnKeyType.Go, entry.ReturnKey);

        // Change value and update
        returnKey = ReturnKeyType.Done;
        entry.UpdateBindings();

        // Assert
        Assert.AreEqual(ReturnKeyType.Done, entry.ReturnKey);
    }

    [TestMethod]
    public void Entry_MethodChaining_ShouldWork()
    {
        // Arrange & Act
        var entry = new Entry()
            .SetIsPassword(true)
            .SetPasswordChar('*')
            .SetKeyboard(KeyboardType.Email)
            .SetReturnKey(ReturnKeyType.Send);

        // Assert
        Assert.IsTrue(entry.IsPassword);
        Assert.AreEqual('*', entry.PasswordChar);
        Assert.AreEqual(KeyboardType.Email, entry.Keyboard);
        Assert.AreEqual(ReturnKeyType.Send, entry.ReturnKey);
    }
}
