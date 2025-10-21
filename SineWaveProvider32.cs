using System;
using NAudio.Wave;

namespace SimpleToneGenerator
{
    /// <summary>
    /// Einfacher Sine Wave Provider (32-bit float) mit dynamischer Frequenz/Amplitude.
    /// Thread-safe für Laufzeitänderungen.
    /// </summary>
    public class SineWaveProvider32 : ISampleProvider
    {
        private readonly WaveFormat _waveFormat;
        private double _phase;
        private readonly object _lock = new();

        private float _frequency = 440f;
        private float _amplitude = 0.25f;

        public SineWaveProvider32()
        {
            _waveFormat = WaveFormat.CreateIeeeFloatWaveFormat(44100, 1);
        }

        public WaveFormat WaveFormat => _waveFormat;

        public float Frequency
        {
            get { lock (_lock) { return _frequency; } }
            set
            {
                lock (_lock)
                {
                    if (value < 0.0f) value = 0.0f;
                    _frequency = value;
                }
            }
        }

        public float Amplitude
        {
            get { lock (_lock) { return _amplitude; } }
            set
            {
                lock (_lock)
                {
                    _amplitude = Math.Clamp(value, 0f, 1f);
                }
            }
        }

        public void SetWaveFormat(int sampleRate, int channels)
        {
            // NAudio-WaveFormat ist in diesem Beispiel bereits gesetzt im Konstruktor.
            // Falls erforderlich, könnte man hier WaveFormat neu erstellen.
        }

        public int Read(float[] buffer, int offset, int count)
        {
            int sampleRate = _waveFormat.SampleRate;
            for (int n = 0; n < count; n++)
            {
                float freq;
                float amp;
                lock (_lock)
                {
                    freq = _frequency;
                    amp = _amplitude;
                }

                double increment = 2 * Math.PI * freq / sampleRate;
                float sampleValue = (float)(amp * Math.Sin(_phase));
                buffer[offset + n] = sampleValue;
                _phase += increment;
                if (_phase > 2 * Math.PI) _phase -= 2 * Math.PI;
            }
            return count;
        }
    }
}
