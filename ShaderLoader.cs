using OpenTK.Graphics.OpenGL4;

namespace RhythmGame
{
    public static class ShaderLoader
    {
        public static int Load(string vertPath, string fragPath)
        {
            string vertexShaderSource = File.ReadAllText(vertPath);
            string fragmentShaderSource = File.ReadAllText(fragPath);

            int vertexShader = GL.CreateShader(ShaderType.VertexShader);
            GL.ShaderSource(vertexShader, vertexShaderSource);
            GL.CompileShader(vertexShader);

            int fragmentShader = GL.CreateShader(ShaderType.FragmentShader);
            GL.ShaderSource(fragmentShader, fragmentShaderSource);
            GL.CompileShader(fragmentShader);

            int program = GL.CreateProgram();
            GL.AttachShader(program, vertexShader);
            GL.AttachShader(program, fragmentShader);
            GL.LinkProgram(program);

            GL.DeleteShader(vertexShader);
            GL.DeleteShader(fragmentShader);

            return program;
        }
    }
}
