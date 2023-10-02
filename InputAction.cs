using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace MGInput
{
    public class InputAction
    {
        public enum PressResult
        {
            eNone,
            eKey,
            eCombo,
            eButton
        }
        public struct KeyCombo
        {
            public Keys modifier;
            public Keys second_modifier;
            public Keys key;

            public int KeyCount { get { return second_modifier == Keys.None ? ((modifier == Keys.None) ? 0 : 1) : 2; } }
        }
        private HashSet<Keys> boundModifiers;
        private List<Keys> boundKeys;
        private List<KeyCombo> boundCombos;
        private List<Buttons> boundButtons;
        private bool IsModifierDown(KeyboardState keyboardState)
        {
            foreach (var key in boundModifiers)
            {
                if (keyboardState.IsKeyDown(key))
                    return true;
            }
            return false;
        }
        public InputAction()
        {
            boundKeys = new List<Keys>();
            boundButtons = new List<Buttons>();
			boundCombos =new List<KeyCombo>();
			boundModifiers = new HashSet<Keys>();
        }
        public void Bind(Keys key)
        {
            if (!boundKeys.Contains(key))
            {
                boundKeys.Add(key);
            }
        }
        public void Bind(KeyCombo combo)
        {
            if (!boundCombos.Contains(combo))
            {
                boundCombos.Add(combo);
                if (combo.modifier != Keys.None)
                {
                    boundModifiers.Add(combo.modifier);
                }
                if (combo.second_modifier != Keys.None)
                {
                    boundModifiers.Add(combo.second_modifier);
                }
            }
        }
        public void Bind(Buttons button)
        {
            if (!boundButtons.Contains(button))
            {
                boundButtons.Add(button);
            }
        }
        public PressResult IsPressed(InputState inputState)
        {
            foreach (var combo in boundCombos)
            {
                if ((combo.modifier != Keys.None) && (inputState.IsKeyDown(combo.modifier)) &&
                    (combo.second_modifier != Keys.None) && (inputState.IsKeyDown(combo.second_modifier)) &&
                    (inputState.IsKeyPressed(combo.key)))
                {
                    return PressResult.eCombo;
                }
            }
            foreach (var key in boundKeys)
            {
                if (inputState.IsKeyPressed(key))
                {
                    if (!IsModifierDown(inputState.KeyboardState))
                        return PressResult.eKey;
                }
            }
            return PressResult.eNone;
        }

		public PressResult IsDown(InputState inputState)
		{

            foreach (var combo in boundCombos)
            {
                if ((combo.modifier != Keys.None) && (inputState.IsKeyDown(combo.modifier)) &&
                    (combo.second_modifier != Keys.None) && (inputState.IsKeyDown(combo.second_modifier)) &&
                    (inputState.IsKeyDown(combo.key)))
                {
                    return PressResult.eCombo;
                }
            }

            foreach (var key in boundKeys)
            {
                if (inputState.IsKeyDown(key))
                {
                    if (!IsModifierDown(inputState.KeyboardState))
                        return PressResult.eKey;
                }
            }

            foreach (var button in boundButtons)
            {
                if (inputState.GamePadState.IsButtonDown(button))
                {
                    return PressResult.eButton;
                }
            }

            return PressResult.eNone;
		}
        public bool AreModifiersAppliedForAction(InputState input)
        {
            bool modifiersAccepted = false;
            foreach (KeyCombo kc in boundCombos)
            {
                var modifier1 = kc.modifier;
                var modifier2 = kc.second_modifier;

                modifiersAccepted = modifiersAccepted ||
                    (modifier1 == Keys.None || input.IsKeyDown(modifier1)) &&
                    (modifier2 == Keys.None || input.IsKeyDown(modifier2));
            }
            return modifiersAccepted;
        }

    }
}
