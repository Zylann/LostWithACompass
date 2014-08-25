using SFML.Window;

namespace Framework
{
    public class GuiComponent : Component, InputHandler
    {
        private bool _enabled = true;
        private bool _focused;
        private bool _hovered;
        private bool _pressed;

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public bool OnKeyPressed(object sender, KeyEventArgs e)
        {
            return false;
        }

        public bool OnKeyReleased(object sender, KeyEventArgs e)
        {
            return false;
        }

        public bool OnTextEntered(object sender, TextEventArgs e)
        {
            return false;
        }

        public bool OnMouseMoved(object sender, MouseMoveEventArgs e)
        {


            foreach (Component cmp in entity.components)
            {
                if (cmp is InputHandler)
                {
                    ((InputHandler)cmp).OnMouseMoved(sender, e);
                }
            }

            return false;
        }

        public bool OnMouseButtonPressed(object sender, MouseButtonEventArgs e)
        {
            return false;
        }

        public bool OnMouseButtonReleased(object sender, MouseButtonEventArgs e)
        {
            return false;
        }

        public bool OnMouseWheelMoved(object sender, MouseWheelEventArgs e)
        {
            return false;
        }
    }
}


