using Bunit;
using Microsoft.AspNetCore.Components.Web;
using TypingMaster.Client.Features.TypingPractice;

namespace TypingMaster.Tests
{
    public class KeyEventGeneratorTests : IDisposable
    {
        private readonly TestContext _context;
        private IRenderedComponent<KeyEventGenerator> _component;

        public KeyEventGeneratorTests()
        {
            _context = new TestContext();
            _component = _context.RenderComponent<KeyEventGenerator>();
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        [Theory]
        [InlineData("a", "KeyA", false, 'a', 'a')]
        [InlineData("a", "KeyA", true, 'a', 'A')]
        //[InlineData("1", "Digit1", false, '1', '1')]
        //[InlineData("1", "Digit1", true, '1', '!')]
        public void HandleKeyDown_ShouldCreateCorrectKeyEvent(string key, string code, bool shiftKey, char expectedKey, char expectedTypedKey)
        {
            // Arrange
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Key = key,
                Code = code,
                ShiftKey = shiftKey
            };

            // Act
            _component = _context.RenderComponent<KeyEventGenerator>(para=>para.Add(p=>p.CharExpected, expectedKey));
            _component.Instance.HandleKeyDown(keyboardEventArgs);

            // Assert
            var keyEvent = _component.Instance.KeyEvent;
            Assert.NotNull(keyEvent);
            Assert.Equal(expectedKey, keyEvent.Key);
            Assert.Equal(expectedTypedKey, keyEvent.TypedKey);
            Assert.NotEqual(default, keyEvent.KeyDownTime);
            Assert.Equal(default, keyEvent.KeyUpTime);
        }

        [Fact]
        public void HandleKeyUp_ShouldCompleteKeyEvent()
        {
            // Arrange
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = false
            };

            // Act
            _component.Instance.HandleKeyDown(keyboardEventArgs);
            _component.Instance.HandleKeyUp(keyboardEventArgs);

            // Assert
            var keyEvent = _component.Instance.GetKeyEvents().FirstOrDefault();
            Assert.NotNull(keyEvent);
            Assert.NotEqual(default, keyEvent.KeyUpTime);
            Assert.True(keyEvent.KeyUpTime > keyEvent.KeyDownTime);
        }

        [Fact]
        public void Reset_ShouldClearAllEvents()
        {
            // Arrange
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = false
            };

            _component.Instance.HandleKeyDown(keyboardEventArgs);
            _component.Instance.HandleKeyUp(keyboardEventArgs);
            Assert.Single(_component.Instance.GetKeyEvents());

            // Act
            _component.Instance.Reset();

            // Assert
            Assert.Empty(_component.Instance.GetKeyEvents());
        }

        [Fact]
        public void HandleKeyDown_DuplicateEvent_ShouldNotCreateDuplicate()
        {
            // Arrange
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = false
            };

            // Act
            _component.Instance.HandleKeyDown(keyboardEventArgs);

            // Try to add the same event again
            _component.Instance.HandleKeyDown(keyboardEventArgs);

            // Assert
            Assert.Single(_component.Instance.GetKeyEvents());
        }

        [Fact]
        public void HandleKeyDown_WithoutKeyUp_ShouldNotCreateNewEvent()
        {
            // Arrange
            var keyboardEventArgs = new KeyboardEventArgs
            {
                Key = "a",
                Code = "KeyA",
                ShiftKey = false
            };

            // Act
            _component.Instance.HandleKeyDown(keyboardEventArgs);
            var secondKeyboardEventArgs = new KeyboardEventArgs
            {
                Key = "b",
                Code = "KeyB",
                ShiftKey = false
            };
            _component.Instance.HandleKeyDown(secondKeyboardEventArgs);

            // Assert
            Assert.Single(_component.Instance.GetKeyEvents());
            var keyEvent = _component.Instance.GetKeyEvents().First();
            Assert.Equal("a", keyEvent.Key.ToString());
        }
    }
}