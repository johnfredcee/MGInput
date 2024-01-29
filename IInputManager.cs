using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace MGInput
{
	public interface IInputManager
	{
		public void Update(GameTime gameTime);
		public InputAction GetAction(string actionName);
		public float GetAxis(string axisName);
		public void Bind(string actionName, Keys key);
		public void Bind(string actionName, Keys modifier, Keys key);
		public void Bind(string actionName, Keys modifier, Keys second_modifier, Keys key);
		public void Bind(string actionName, Buttons button);
		public void BindAxis(string axisName, AxisType axisType);
		void PopInputContext();
		void PushInputContext(string context);
		public InputState InputState
		{
			get;
		}
		public Dictionary<Keys, InputSymbol> SymbolMappings
		{
			get;
		}
		public HashSet<string> ActionsPressedThisUpdate
		{
			get;
		}
		public HashSet<string> ActionsDownThisUpdate
		{
			get;
		}
		public HashSet<InputSymbol> SymbolsPressedThisUpdate
		{
			get;
		}

	}
}
