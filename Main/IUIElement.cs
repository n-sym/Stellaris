using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris
{
    public interface IUIElement
    {
        public void Update();
        public void Draw(IDrawAPI drawAPI);
    }
}
