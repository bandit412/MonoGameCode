using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#region Additional Namespaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
#endregion

namespace Transform3D
{
    static class InputKeyManager
    {
        [FlagsAttribute]
        public enum Triggers : ulong
        {
            LeftArrow  = 0x0000000000000001,
            RightArrow = 0x0000000000000002,
            UpArrow    = 0x0000000000000004,
            DownArrow  = 0x0000000000000008,
            Plus       = 0x0000000000000010,
            Minus      = 0x0000000000000020,
            Pause      = 0x0000000000000040,
            Reset      = 0x0000000000000080,
            Fire       = 0x0000000000000100,
            Toggle     = 0x0000000000001000,
            ExitLevel  = 0x0000000000010000,
            Quit       = 0x0000000000100000,
            CamLeft    = 0x0000000001000000,
            CamRight   = 0x0000000010000000,
            CamUp      = 0x0000000100000000,
            CamDown    = 0x0000001000000000

            // Add new triggers below here...
        }//end enum

        private static GamePadState previousGamePadState = new GamePadState();
        private static Triggers lastKeyValuesRead;

        static public Triggers Read()
        {
            Triggers currentKeyState = 0;

            GamePadState gamepadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState keyboardState = Keyboard.GetState();

            if (keyboardState.IsKeyDown(Keys.Escape) || ((previousGamePadState.Buttons.Back != ButtonState.Pressed) && (gamepadState.Buttons.Back == ButtonState.Pressed)))
                currentKeyState |= Triggers.ExitLevel;

            // the DownArrow bit is turned on
            if (keyboardState.IsKeyDown(Keys.Down))
                currentKeyState |= Triggers.DownArrow;

            if (keyboardState.IsKeyDown(Keys.Up))
                currentKeyState |= Triggers.UpArrow;

            if (keyboardState.IsKeyDown(Keys.Right))
                currentKeyState |= Triggers.RightArrow;

            if (keyboardState.IsKeyDown(Keys.Left))
                currentKeyState |= Triggers.LeftArrow;

            if (keyboardState.IsKeyDown(Keys.Space))
                currentKeyState |= Triggers.Fire;

            if (keyboardState.IsKeyDown(Keys.P))
                currentKeyState |= Triggers.Pause;

            if (keyboardState.IsKeyDown(Keys.Escape))
                currentKeyState |= Triggers.ExitLevel;

            if (keyboardState.IsKeyDown(Keys.R))
                currentKeyState |= Triggers.Reset;

            if (keyboardState.IsKeyDown(Keys.Q))
                currentKeyState |= Triggers.Quit;

            if (keyboardState.IsKeyDown(Keys.T))
                currentKeyState |= Triggers.Toggle;

            if (keyboardState.IsKeyDown(Keys.OemPlus))
                currentKeyState |= Triggers.Plus;

            if (keyboardState.IsKeyDown(Keys.OemMinus))
                currentKeyState |= Triggers.Minus;

            if (keyboardState.IsKeyDown(Keys.A))
                currentKeyState |= Triggers.CamLeft;

            if (keyboardState.IsKeyDown(Keys.S))
                currentKeyState |= Triggers.CamRight;

            if (keyboardState.IsKeyDown(Keys.W))
                currentKeyState |= Triggers.CamUp;

            if (keyboardState.IsKeyDown(Keys.X))
                currentKeyState |= Triggers.CamDown;

            if (gamepadState.DPad.Down == ButtonState.Pressed)
                currentKeyState |= Triggers.DownArrow;

            if (gamepadState.DPad.Up == ButtonState.Pressed)
                currentKeyState |= Triggers.UpArrow;

            if (gamepadState.DPad.Right == ButtonState.Pressed)
                currentKeyState |= Triggers.RightArrow;

            if (gamepadState.DPad.Left == ButtonState.Pressed)
                currentKeyState |= Triggers.LeftArrow;

            if (gamepadState.Buttons.A == ButtonState.Pressed)
                // This is the big Zunepad button
                currentKeyState |= Triggers.Fire;

            if ((previousGamePadState.Buttons.B != ButtonState.Pressed) && (gamepadState.Buttons.B == ButtonState.Pressed))
                // This is the forward key next to the Zunepad button
                currentKeyState |= Triggers.Pause;

            previousGamePadState = gamepadState;

            lastKeyValuesRead = currentKeyState;

            return currentKeyState;
        }//eom

        static public Triggers Clear()
        {
            Triggers currentKeyState = 0;
            return currentKeyState;
        }//eom
    }//eoc
}//eon

