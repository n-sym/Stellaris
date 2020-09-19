using System;
using System.Collections.Generic;
using System.Text;

namespace Stellaris
{
    public interface IDrawAPI
    {
        public void Draw(IDrawInfo drawInfo);
    }
    public interface IDrawInfo
    {

    }
}
