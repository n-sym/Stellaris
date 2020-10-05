using System.Collections.Generic;

namespace Stellaris
{
    public class UIManager
    {
        public int currentState;
        public int Current => currentState;
        public IUIState[] States;
        public UIManager(IUIState[] states)
        {
            States = states;
        }

        public void Enter(int state)
        {
            currentState = state;
        }
        public void Enter(IUIState state)
        {
            for (int i = 0; i < States.Length; i++)
            {
                if (States[i] == state) currentState = i;
            }
        }
        public virtual void Update()
        {
            States[currentState].Update();
        }
        public virtual void Draw(IDrawAPI drawAPI)
        {
            States[currentState].Draw(drawAPI);
        }
    }
    public interface IUIState
    {
        public List<IUIElement> Elements { get; set; }
        public void Update();
        public void Draw(IDrawAPI drawAPI);
    }
    public interface IUIElement
    {
        public void Update();
        public void Draw(IDrawAPI drawAPI);
    }
}
