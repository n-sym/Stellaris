using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris
{
    public interface IUIElement
    {
        public int ID => 0;
        public void Update();
        public void Draw(IDrawAPI drawAPI);
    }
}
