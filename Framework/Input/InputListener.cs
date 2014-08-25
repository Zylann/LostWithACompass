using SFML.Window;

namespace Framework
{
    /// <summary>
    /// All methods must return true if the event has been consumed
    /// </summary>
	public interface InputHandler
	{
		bool OnKeyPressed(object sender, KeyEventArgs e);

        bool OnKeyReleased(object sender, KeyEventArgs e);

        bool OnTextEntered(object sender, TextEventArgs e);

        bool OnMouseMoved(object sender, MouseMoveEventArgs e);

        bool OnMouseButtonPressed(object sender, MouseButtonEventArgs e);

        bool OnMouseButtonReleased(object sender, MouseButtonEventArgs e);

        bool OnMouseWheelMoved(object sender, MouseWheelEventArgs e);
	}

    public class InputDispatcher
    {

    }

    public enum InputType
    {
        KeyPressed,
        KeyReleased,

        MouseButtonPressed,
        MouseButtonReleased,
        MouseWheelMoved,
        MouseMoved,

        ScreenResized,
        ScreenClosed
    }

    public class InputStates
    {
        public InputType type;
        public MouseState mouse = new MouseState();
        public KeyboardState keyboard = new KeyboardState();
    }

    public class KeyboardState
    {
        /// <summary>
        /// Last key pressed or released
        /// </summary>
        public Keyboard.Key key;
    }

    public class MouseState
    {
        /// <summary>
        /// Last known position of the mouse cursor
        /// </summary>
        public Vector2i position;

        /// <summary>
        /// Previous position of the mouse cursor
        /// </summary>
        public Vector2i lastPosition;

        /// <summary>
        /// Last pressed or released button
        /// </summary>
        public int button;

        /// <summary>
        /// Last move value of the wheel
        /// </summary>
        public int wheelDelta;
    }

}



