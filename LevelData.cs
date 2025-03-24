using OpenTK.Mathematics;

namespace RhythmGame
{
    public class LevelData
    {
        public string Name { get; set; }
        public string AudioPath { get; set; }
        public string BackgroundPath { get; set; }
        public float BackgroundOpacity { get; set; } = 0.7f; // Add opacity control
        public List<float> BeatTimings { get; set; }

        public LevelData(string name)
        {
            Name = name;
            AudioPath = $"Levels/{name}/{name}.wav";
            BackgroundPath = $"Levels/{name}/{name}.webm"; // Changed from webp to webm
            BeatTimings = new List<float>();
        }

        private string LevelFolder => $"Levels/{Name}";
        
        public string GetBeatFilePath()
        {
            return Path.Combine(LevelFolder, $"{Name}.txt");
        }

        public void GenerateLevelFile()
        {
            string filePath = GetBeatFilePath();
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (float beatTime in BeatTimings)
                {
                    writer.WriteLine(beatTime);
                }
            }
        }
    }
}
