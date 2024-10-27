using System.Security.Authentication.ExtendedProtection;

namespace Substrate
{
    public class WindowBase
    {
        protected bool ShouldClose = true;

        internal virtual void SystemDraw(float dt)
        {
            Draw(dt);

            if (!ShouldClose)
                Close();
        }

        public virtual void Draw(float dt)
        {

        }

        public virtual void Close()
        {
            Substrate.App.CloseWindow(this);
        }
    }
}
