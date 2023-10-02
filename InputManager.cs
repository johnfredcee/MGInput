using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MGUtilties;

namespace MGInput
{
    public class InputManager : GameComponent, IInputManager
    {
        // Maps descriptions of actions to actions
        private Dictionary<string, InputAction> _actions;
        // Maps descriptins of axes to axes
        private Dictionary<string, InputAxis> _axes;
        // Tracks actions 
        private HashSet<string> _actionsPressedThisUpdate;
        private HashSet<string> _actionsDownThisUpdate;
        // Tracks symbols
		private HashSet<InputSymbol> _symbolsPressedThisUpdate;
        // Encapsuates system input
        private InputState _currentInputState;
        // Filter for actions 
        private Stack<string> _currentInputContext;
        public InputState InputState => _currentInputState;
        public HashSet<string> ActionsPressedThisUpdate => _actionsPressedThisUpdate;
		public HashSet<string> ActionsDownThisUpdate => _actionsDownThisUpdate;
		public HashSet<InputSymbol> SymbolsPressedThisUpdate => _symbolsPressedThisUpdate;
        public Dictionary<Keys, InputSymbol> SymbolMappings
        {
            get { return _symbolDefinitions; }
            set { _symbolDefinitions = value ?? new Dictionary<Keys, InputSymbol>(); }
        }
        public void PushInputContext(string context)
        {
            _currentInputContext.Push(context);
        }
        public void PopInputContext()
        {
            _currentInputContext.Pop();
        }
        /// <summary>
        /// Initializes the input manager for one player.
        /// </summary>
        /// <param name="playerIndex">The index of the player, defaults to One</param>
        public InputManager(Game game) : base(game)
        {
            _actionsPressedThisUpdate = new HashSet<string>();
			_actionsDownThisUpdate = new HashSet<string>();
			_symbolsPressedThisUpdate = new HashSet<InputSymbol>();
            _currentInputState = new InputState();
            _currentInputState.PlayerIndex = PlayerIndex.One;
            _actions = new Dictionary<string, InputAction>();
            _axes = new Dictionary<string, InputAxis>();
            _currentInputContext = new Stack<string>();
			BindConsoleInput();
        }
        /// <summary>
        /// Binds a keyboard key to an action.
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <param name="key">The keyboard key</param>
        public void Bind(string actionName, Keys key)
        {
            CreateOrGetAction(actionName).Bind(key);
        }

        public void Bind(string actionName, Keys modifier, Keys key)
        {
            var combo = new InputAction.KeyCombo { modifier = modifier, second_modifier = Keys.None, key = key };
            CreateOrGetAction(actionName).Bind(combo);
        }
        public void Bind(string actionName, Keys modifier, Keys second_modifier, Keys key)
        {
            var combo = new InputAction.KeyCombo { modifier = modifier, second_modifier = second_modifier, key = key };
            CreateOrGetAction(actionName).Bind(combo);
        }

        /// <summary>
        /// Binds a game pad button to an action.
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <param name="button">The game pad button</param>
        public void Bind(string actionName, Buttons button)
        {
            CreateOrGetAction(actionName).Bind(button);
        }

        public void BindAxis(string axisName, AxisType axisType)
        {
            CreateOrGetGetAxisBind(axisName).SetAxisType(axisType);
        }

        /// <summary>
        /// This gets the latest keyboard and game pad state, run this every tick before checking inputs!
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            // Clear just pressed/released lists
            _actionsPressedThisUpdate.Clear();
			_symbolsPressedThisUpdate.Clear();
			_actionsDownThisUpdate.Clear();
			
            _currentInputState.Update(gameTime);

			
            var deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            foreach (var act in _actions)
            {
                var actionName = act.Key;
                var action = act.Value;
                var actionContext = actionName.Split(".")[0];
                var currentInputContext = _currentInputContext.Count > 0 ? _currentInputContext.Peek() : "";
                if ((currentInputContext.Length == 0) || (currentInputContext == actionContext))
                {
                    if (action.IsPressed(_currentInputState) != InputAction.PressResult.eNone)
                    {
                        _actionsPressedThisUpdate.Add(actionName);
                    }
                    if (action.IsDown(_currentInputState) != InputAction.PressResult.eNone)
                    {
                        _actionsDownThisUpdate.Add(actionName);
                    }
                }
            }

            for (int i = 0; i < _currentInputState.PressedKeys.Count; i++)
            {
                Keys key = _currentInputState.PressedKeys[i];
                InputSymbol symbol;
                if (SymbolMappings.TryGetValue(key, out symbol))
                    _symbolsPressedThisUpdate.Add(symbol);
            }
        }

        public void LockMouse(Vector2 mousePosition)
        {
            _currentInputState.LockMouse(mousePosition);
        }
        public void UnlockMouse()
        {
            _currentInputState.UnlockMouse();
        }


        public float GetAxis(string axisName)
        {
            return CreateOrGetGetAxisBind(axisName).GetValue(_currentInputState.MouseDeltaPosition, _currentInputState.GamePadState);
        }

        public InputAction GetAction(string actionName)
        {
            InputAction result;
            _actions.TryGetValue(actionName, out result);
            return result;
        }

        /// <summary>
        /// This will get the action with the specified name, and if none exist, simply create a new one.
        /// </summary>
        /// <param name="actionName">The name of the action</param>
        /// <returns>The action</returns>
        private InputAction CreateOrGetAction(string actionName)
        {
            InputAction result;
            if (!_actions.TryGetValue(actionName, out result))
            {
                result = new InputAction();
                _actions.Add(actionName, result);
            }
            return result;
        }
        private InputAxis CreateOrGetGetAxisBind(string axisName)
        {
            InputAxis result;
            if (!_axes.TryGetValue(axisName, out result))
            {
                result = new InputAxis();
                _axes.Add(axisName, result);
            }
            return result;
        }

        private void BindConsoleInput()
        {
            Bind("ConsoleAction.ExecuteCommand", Keys.Enter);
            Bind("ConsoleAction.MoveLeft", Keys.Left);
            Bind("ConsoleAction.MoveRight", Keys.Right);
            Bind("ConsoleAction.MoveLeftWord", Keys.LeftControl, Keys.Left);
            Bind("ConsoleAction.MoveLeftWord", Keys.RightControl, Keys.Left);
            Bind("ConsoleAction.MoveRightWord", Keys.LeftControl, Keys.Right);
            Bind("ConsoleAction.MoveRightWord", Keys.RightControl, Keys.Right);
            Bind("ConsoleAction.MoveToBeginning", Keys.Home);
            Bind("ConsoleAction.MoveToEnd", Keys.End);
            Bind("ConsoleAction.DeletePreviousChar", Keys.Back);
            Bind("ConsoleAction.DeleteCurrentChar", Keys.Delete);
            Bind("ConsoleAction.UppercaseModifier", Keys.LeftShift, Keys.None);
            Bind("ConsoleAction.UppercaseModifier", Keys.RightShift, Keys.None);
            Bind("ConsoleAction.PreviousCommandInHistory", Keys.Up);
            Bind("ConsoleAction.NextCommandInHistory", Keys.Down);
            Bind("ConsoleAction.Cut", Keys.LeftControl, Keys.X);
            Bind("ConsoleAction.Cut", Keys.RightControl, Keys.X);
            Bind("ConsoleAction.Copy", Keys.LeftControl, Keys.C);
            Bind("ConsoleAction.Copy", Keys.RightControl, Keys.C);
            Bind("ConsoleAction.Paste", Keys.LeftControl, Keys.V);
            Bind("ConsoleAction.Paste", Keys.RightControl, Keys.V);
            Bind("ConsoleAction.NewLine", Keys.LeftShift, Keys.Enter);
            Bind("ConsoleAction.NewLine", Keys.RightShift, Keys.Enter);
            //Bind( Keys.LeftShift, Keys.Up, "ConsoleAction.MovePreviousLine" );
            //Bind( Keys.RightShift, Keys.Up, "ConsoleAction.MovePreviousLine" );
            //Bind( Keys.LeftShift, Keys.Down, "ConsoleAction.MoveNextLine" );
            //Bind( Keys.RightShift, Keys.Down, "ConsoleAction.MoveNextLine" );
            Bind("ConsoleAction.AutocompleteForward", Keys.LeftControl, Keys.Space);
            Bind("ConsoleAction.AutocompleteForward", Keys.RightControl, Keys.Space);
            Bind("ConsoleAction.AutocompleteBackward", Keys.LeftControl, Keys.LeftShift, Keys.Space);
            Bind("ConsoleAction.AutocompleteBackward", Keys.LeftControl, Keys.RightShift, Keys.Space);
            Bind("ConsoleAction.AutocompleteBackward", Keys.RightControl, Keys.LeftShift, Keys.Space);
            Bind("ConsoleAction.AutocompleteBackward", Keys.RightControl, Keys.RightShift, Keys.Space);
            Bind("ConsoleAction.Tab", Keys.Tab);
            Bind("ConsoleAction.RemoveTab", Keys.LeftShift, Keys.Tab);
            Bind("ConsoleAction.RemoveTab", Keys.RightShift, Keys.Tab);
            //Bind( Keys.NumLock, "ConsoleAction.NumLock" );
            Bind("ConsoleAction.SelectionModifier", Keys.LeftShift, Keys.None);
            Bind("ConsoleAction.SelectionModifier", Keys.RightShift, Keys.None);
			Bind("ViewAction.ViewMap", Keys.F10);
			Bind("ViewAction.ViewHeatMap0", Keys.F5);
			Bind("ViewAction.ViewHeatMap1", Keys.F6);
			Bind("ViewAction.ViewHeatMap3", Keys.F7);
			Bind("ViewAction.ViewHeatMap4", Keys.F8);
			Bind("ViewAction.ViewHeatMap5", Keys.F9);
			Bind("ViewAction.ScrollUp", Keys.W);
			Bind("ViewAction.ScrollDown", Keys.S);
			Bind("ViewAction.ScrollLeft", Keys.A);
    		Bind("ViewAction.ScrollRight", Keys.D);
        }

	    private Dictionary<Keys, InputSymbol> _symbolDefinitions = new Dictionary<Keys, InputSymbol>
        {
            // Digits.
            { Keys.D1, new InputSymbol("1", "!") },
            { Keys.D2, new InputSymbol("2", "@") },
            { Keys.D3, new InputSymbol("3", "#") },
            { Keys.D4, new InputSymbol("4", "$") },
            { Keys.D5, new InputSymbol("5", "%") },
            { Keys.D6, new InputSymbol("6", "^") },
            { Keys.D7, new InputSymbol("7", "&") },
            { Keys.D8, new InputSymbol("8", "*") },
            { Keys.D9, new InputSymbol("9", "(") },
            { Keys.D0, new InputSymbol("0", ")") },
            { Keys.NumPad1, new InputSymbol("1") },
            { Keys.NumPad2, new InputSymbol("2") },
            { Keys.NumPad3, new InputSymbol("3") },
            { Keys.NumPad4, new InputSymbol("4") },
            { Keys.NumPad5, new InputSymbol("5") },
            { Keys.NumPad6, new InputSymbol("6") },
            { Keys.NumPad7, new InputSymbol("7") },
            { Keys.NumPad8, new InputSymbol("8") },
            { Keys.NumPad9, new InputSymbol("9") },
            { Keys.NumPad0, new InputSymbol("0") },

            // Letters.
            { Keys.Q, new InputSymbol("q", "Q") },
            { Keys.W, new InputSymbol("w", "W") },
            { Keys.E, new InputSymbol("e", "E") },
            { Keys.R, new InputSymbol("r", "R") },
            { Keys.T, new InputSymbol("t", "T") },
            { Keys.Y, new InputSymbol("y", "Y") },
            { Keys.U, new InputSymbol("u", "U") },
            { Keys.I, new InputSymbol("i", "I") },
            { Keys.O, new InputSymbol("o", "O") },
            { Keys.P, new InputSymbol("p", "P") },
            { Keys.OemOpenBrackets, new InputSymbol("[", "{") },
            { Keys.OemCloseBrackets, new InputSymbol("]", "}") },

            { Keys.A, new InputSymbol("a", "A") },
            { Keys.S, new InputSymbol("s", "S") },
            { Keys.D, new InputSymbol("d", "D") },
            { Keys.F, new InputSymbol("f", "F") },
            { Keys.G, new InputSymbol("g", "G") },
            { Keys.H, new InputSymbol("h", "H") },
            { Keys.J, new InputSymbol("j", "J") },
            { Keys.K, new InputSymbol("k", "K") },
            { Keys.L, new InputSymbol("l", "L") },
            { Keys.OemSemicolon, new InputSymbol(";", ":") },
            { Keys.OemQuotes, new InputSymbol("'", "\"") },
            { Keys.OemPipe, new InputSymbol("\\", "|") },

            { Keys.OemBackslash, new InputSymbol("\\", "|") },
            { Keys.Z, new InputSymbol("z", "Z") },
            { Keys.X, new InputSymbol("x", "X") },
            { Keys.C, new InputSymbol("c", "C") },
            { Keys.V, new InputSymbol("v", "V") },
            { Keys.B, new InputSymbol("b", "B") },
            { Keys.N, new InputSymbol("n", "N") },
            { Keys.M, new InputSymbol("m", "M") },
            { Keys.OemComma, new InputSymbol(",", "<") },
            { Keys.OemPeriod, new InputSymbol(".", ">") },
            { Keys.OemQuestion, new InputSymbol("/", "?") },

            // Special.
            { Keys.Space, new InputSymbol(" ", " ") },
            { Keys.OemMinus, new InputSymbol("-", "_") },
            { Keys.OemPlus, new InputSymbol("=", "+") },
            { Keys.Decimal, new InputSymbol(".") },
            { Keys.Add, new InputSymbol("+") },
            { Keys.Subtract, new InputSymbol("-") },
            { Keys.Multiply, new InputSymbol("*") },
            { Keys.Divide, new InputSymbol("/") }
            //{ Keys.Tab, new SymbolPair("\t", "\t") } // Tab char is not supported in many fonts.
            //{ Keys.Tab, new SymbolPair("    ", "    ") } // Use 4 spaces instead.
        };
 
    }
}
