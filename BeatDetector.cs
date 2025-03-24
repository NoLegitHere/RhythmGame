using NAudio.Wave;

namespace RhythmGame
{
    public class BeatDetector
    {
        public static List<float> DetectBeats(string audioFile, float threshold = 0.3f)
        {
            var beats = new List<float>();
            using (var audioReader = new AudioFileReader(audioFile))
            {
                int sampleRate = audioReader.WaveFormat.SampleRate;
                int bytesPerSample = audioReader.WaveFormat.BitsPerSample / 8;
                int samples = (int)(audioReader.Length / bytesPerSample);
                
                float[] buffer = new float[1024];
                float previousEnergy = 0;
                float currentTime = 0;
                
                while (audioReader.Read(buffer, 0, buffer.Length) > 0)
                {
                    float energy = 0;
                    for (int i = 0; i < buffer.Length; i++)
                    {
                        energy += buffer[i] * buffer[i];
                    }
                    energy /= buffer.Length;

                    if (energy > threshold && energy > previousEnergy * 1.3f)
                    {
                        beats.Add(currentTime);
                    }

                    previousEnergy = energy;
                    currentTime += (float)buffer.Length / sampleRate;
                }
            }
            return beats;
        }
    }
}
