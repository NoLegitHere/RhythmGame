using NAudio.Wave;
using FFmpeg.AutoGen;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace RhythmGame
{
    public class Level
    {
        private LevelData _data;
        private WaveOutEvent? _waveOut;
        private AudioFileReader? _audioFile;
        private int _backgroundTexture;
        private float _startTime;
        private int _currentBeatIndex;
        private string _levelName;
        private VideoPlayer? _videoPlayer;
        private int _vao;
        private int _vbo;
        private int _shader;

        public Level(string levelName)
        {
            _data = new LevelData(levelName);
            this._levelName = levelName;
            LoadContent();
            SetupRendering();
        }

        private void LoadContent()
        {
            string beatFilePath = _data.GetBeatFilePath();

            if (!File.Exists(_data.AudioPath))
                throw new FileNotFoundException($"Audio file not found: {_data.AudioPath}");
            if (!File.Exists(_data.BackgroundPath))
                throw new FileNotFoundException($"Background file not found: {_data.BackgroundPath}");

            try
            {
                // Load audio
                _waveOut = new WaveOutEvent() ?? throw new InvalidOperationException("Failed to initialize audio output");
                _audioFile = new AudioFileReader(_data.AudioPath) ?? throw new InvalidOperationException("Failed to load audio file");
                _waveOut.Init(_audioFile);

                // Initialize video player with muted audio
                _videoPlayer = new VideoPlayer(_data.BackgroundPath, true);
                _backgroundTexture = _videoPlayer.TextureId;

                // Load or generate beat timings
                if (File.Exists(beatFilePath))
                {
                    _data.BeatTimings = File.ReadAllLines(beatFilePath)
                        .Select(line => float.Parse(line))
                        .ToList();
                }
                else
                {
                    _data.BeatTimings = BeatDetector.DetectBeats(_data.AudioPath);
                    _data.GenerateLevelFile();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error loading level content: {e.Message}");
                throw;
            }
        }

        private void SetupRendering()
        {
            float[] vertices = {
                // Position (x,y,z), TexCoords (u,v)
                -1.0f, -1.0f, 0.0f, 0.0f, 0.0f,
                 1.0f, -1.0f, 0.0f, 1.0f, 0.0f,
                 1.0f,  1.0f, 0.0f, 1.0f, 1.0f,
                -1.0f,  1.0f, 0.0f, 0.0f, 1.0f
            };

            _vao = GL.GenVertexArray();
            _vbo = GL.GenBuffer();
            
            GL.BindVertexArray(_vao);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);
            
            GL.EnableVertexAttribArray(0);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);
            GL.EnableVertexAttribArray(1);
            GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));

            _shader = ShaderLoader.Load("Shaders/background.vert", "Shaders/background.frag");
        }

        public void Start()
        {
            if (_waveOut == null || _audioFile == null)
            {
                throw new InvalidOperationException("Audio system not properly initialized");
            }
            
            _startTime = (float)DateTime.Now.TimeOfDay.TotalSeconds;
            _waveOut.Play();
        }

        public void Update()
        {
            float currentTime = (float)DateTime.Now.TimeOfDay.TotalSeconds - _startTime;
            
            // Update video frame
            _videoPlayer?.UpdateFrame(currentTime);
            
            // Check for beats
            while (_currentBeatIndex < _data.BeatTimings.Count && 
                   _data.BeatTimings[_currentBeatIndex] <= currentTime)
            {
                // Handle beat hit window
                _currentBeatIndex++;
            }
        }

        public void Draw()
        {
            // Clear with black background
            GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Enable blending for transparency
            GL.Enable(EnableCap.Blend);
            GL.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
            
            // Ensure texture unit 0 is active
            GL.ActiveTexture(TextureUnit.Texture0);
            GL.BindTexture(TextureTarget.Texture2D, _backgroundTexture);
            
            GL.UseProgram(_shader);
            
            // Set texture uniform
            GL.Uniform1(GL.GetUniformLocation(_shader, "uTexture"), 0);
            GL.Uniform1(GL.GetUniformLocation(_shader, "uOpacity"), _data.BackgroundOpacity);
            
            GL.BindVertexArray(_vao);
            GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
            
            DrawBeatMarkers();
        }

        private void DrawBeatMarkers()
        {
            float currentTime = (float)DateTime.Now.TimeOfDay.TotalSeconds - _startTime;
            foreach (var beatTime in _data.BeatTimings)
            {
                if (beatTime > currentTime && beatTime < currentTime + 2.0f)
                {
                    float alpha = 1.0f - (beatTime - currentTime) / 2.0f;
                    DrawBeatMarker(beatTime - currentTime, alpha);
                }
            }
        }

        private void DrawBeatMarker(float timeOffset, float alpha)
        {
            // Implementation of drawing beat markers
            // This would draw circles or other shapes to indicate where to hit
            // Position based on timeOffset, transparency based on alpha
        }

        public void Dispose()
        {
            _waveOut?.Dispose();
            _audioFile?.Dispose();
            _videoPlayer?.Dispose();
            GL.DeleteTexture(_backgroundTexture);
            GL.DeleteBuffer(_vbo);
            GL.DeleteVertexArray(_vao);
            GL.DeleteProgram(_shader);
        }
    }
}
