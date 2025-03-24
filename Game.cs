using OpenTK.Windowing.Desktop;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using System;
using System.IO;

namespace RhythmGame
{
    public class Game : GameWindow
    {
        private Level? _currentLevel;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
            : base(gameWindowSettings, nativeWindowSettings)
        {
        }

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            try
            {
                _currentLevel = new Level("level1");
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"Error loading level: {e.Message}");
                // Optionally, display an error message to the user or take other appropriate action.
                Close(); // Exit the game if the level cannot be loaded.
                return;
            }

            _currentLevel.Start();
        }

        protected override void OnUpdateFrame(OpenTK.Windowing.Common.FrameEventArgs args)
        {
            base.OnUpdateFrame(args);
            _currentLevel?.Update();
        }

        protected override void OnRenderFrame(OpenTK.Windowing.Common.FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            _currentLevel?.Draw();

            SwapBuffers();
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            _currentLevel?.Dispose();
        }
    }
}
