using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using MGUtilties;

namespace MGInput
{
	public class InputState
	{
		/** keyboard */
		private KeyboardState currentKeyboardState;
		public KeyboardState KeyboardState
		{
			get { return currentKeyboardState; }
			set { currentKeyboardState = value; }
		}
		private Keys[] _rawPressedKeys = new Keys[8];
	
		private float TimeToTriggerRepeatingInput { get; set; } = 0.4f;
		private float TimeToCooldownRepeatingInput { get; set; } = 0.04f;
		private Dictionary<Keys, float> keyDownTimes = new Dictionary<Keys, float>();
		/* Mouse */
		private MouseState currentMouseState;
		public MouseState MouseState
		{
			get { return currentMouseState; }
			set { currentMouseState = value; }
		}
		private Vector2 mousePositionLastUpdate;
		private Vector2? mouseLockPosition;
		private Vector2 currentMouseDeltaPosition;
		public Vector2 MouseDeltaPosition
		{
			get { return currentMouseDeltaPosition; }
			set { currentMouseDeltaPosition = value; }
		}
		/* Gamepad */
		// TO DO : Need multiple states for multiple players
		private GamePadState currentGamePadState;
		public GamePadState GamePadState
		{
			get { return currentGamePadState; }
			set { currentGamePadState = value; }
		}
		private PlayerIndex playerIndex;
		public PlayerIndex PlayerIndex
		{
			get { return playerIndex; }
			set { playerIndex = value; }
		}
		public bool IsKeyDown(Keys key) => KeyboardState.IsKeyDown(key);
		public bool IsKeyPressed(Keys key) => PressedKeys.Contains(key);
		public bool IsKeyReleased(Keys key) => ReleasedKeys.Contains(key);
		public bool IsKeyToggled(Keys key)
		{
			switch (key)
			{
				case Keys.CapsLock:
					return Native.IsKeyToggled(Keys.CapsLock);
				case Keys.NumLock:
					return Native.IsKeyToggled(Keys.NumLock);
				default:
					return false;
			}
		}
		public void Clear()
		{
			DownKeys.Clear();
			PressedKeys.Clear();
			ReleasedKeys.Clear();
			KeyboardState = default(KeyboardState);
		}
		public List<Keys> DownKeys { get; } = new List<Keys>();
		public List<Keys> PressedKeys { get; } = new List<Keys>();
		public List<Keys> ReleasedKeys { get; } = new List<Keys>();
		public void CopyTo(InputState other)
		{
			other.KeyboardState = KeyboardState;
			other.currentMouseState = MouseState;
			other.mousePositionLastUpdate = mousePositionLastUpdate;
			other.mouseLockPosition = mouseLockPosition;
			other.MouseDeltaPosition = MouseDeltaPosition;
			other.PlayerIndex = PlayerIndex;
			other.GamePadState = GamePadState;
			other.DownKeys.Clear();
			other.PressedKeys.Clear();
			other.DownKeys.AddRange(DownKeys);
			other.PressedKeys.AddRange(PressedKeys);
			other.ReleasedKeys.AddRange(ReleasedKeys);
		}
		public void Update(GameTime gameTime)
		{
			KeyboardState = Keyboard.GetState();
			MouseState = Mouse.GetState();
			GamePadState = GamePad.GetState(playerIndex);

			DownKeys.Clear();
			PressedKeys.Clear();
			ReleasedKeys.Clear();
			KeyboardState.GetPressedKeys(_rawPressedKeys);

			for(int i = 0; i < KeyboardState.GetPressedKeyCount(); i++)
			{
				Keys key = _rawPressedKeys[i];
				if (!keyDownTimes.ContainsKey(key))
				{
					PressedKeys.Add(key);
					keyDownTimes.Add(key, 0.0f);
					// System.Diagnostics.Debug.WriteLine("Pressing {0}", key.ToString());
				}
			}
		
			foreach (var key in keyDownTimes.Keys)
			{
				//System.Diagnostics.Debug.WriteLine("Ckecking {0}", key.ToString());

				if (Array.IndexOf(_rawPressedKeys, key, 0, KeyboardState.GetPressedKeyCount()) != -1)
				{
					// System.Diagnostics.Debug.WriteLine("Down {0}", key.ToString());
					DownKeys.Add(key);
				}
				else
				{
					//System.Diagnostics.Debug.WriteLine("Releasing {0}", key.ToString());
					ReleasedKeys.Add(key);
				}
			}

			foreach (var key in ReleasedKeys)
			{
				// System.Diagnostics.Debug.WriteLine("Removing {0}", key.ToString());
				keyDownTimes.Remove(key);
			}
			foreach (var key in DownKeys)
			{
				float downTime = keyDownTimes[key] + gameTime.ElapsedGameTime.Ticks * (1.0f / (float) TimeSpan.TicksPerSecond);
				//System.Diagnostics.Debug.WriteLine("Updating {0}{1}", key.ToString(), downTime.ToString("F"));
				if (downTime > TimeToTriggerRepeatingInput)
				{
					//System.Diagnostics.Debug.WriteLine("Repeating {0}{1}", key.ToString(), downTime.ToString("F"));
					PressedKeys.Add(key);
					downTime -= TimeToCooldownRepeatingInput;
				}
				keyDownTimes[key] = downTime;
				
			}

			// Calculate mouse delta position
			if (mousePositionLastUpdate != null)
			{
				currentMouseDeltaPosition = currentMouseState.Position.ToVector2() - mousePositionLastUpdate;
			}

			if (mouseLockPosition.HasValue)
			{
				Mouse.SetPosition((int)mouseLockPosition.Value.X, (int)mouseLockPosition.Value.Y);
				mousePositionLastUpdate = mouseLockPosition.Value;
			}
			else
			{
				mousePositionLastUpdate = currentMouseState.Position.ToVector2();
			}
		}

		public void LockMouse(Vector2 mousePosition)
		{
			mouseLockPosition = mousePosition;
		}
		public void UnlockMouse()
		{
			mouseLockPosition = null;
		}

	}
}
