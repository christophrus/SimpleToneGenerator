using System;
using System.Globalization;
using System.Windows;
using System.Windows.Input;
using NAudio.Wave;

namespace SimpleToneGenerator
{
    public partial class MainWindow : Window
    {
        private WaveOutEvent? _waveOut;
        private SineWaveProvider32? _sineProvider;
        private bool _playing = false;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (s, e) => UpdateFrequencyDisplay(FreqSlider.Value);
        }

        private void BtnStartStop_Click(object sender, RoutedEventArgs e)
        {
            if (!_playing)
            {
                StartTone();
                BtnStartStop.Content = "Stop";
            }
            else
            {
                StopTone();
                BtnStartStop.Content = "Start";
            }
            _playing = !_playing;
        }

        private void StartTone()
        {
            _sineProvider = new SineWaveProvider32();
            _sineProvider.SetWaveFormat(44100, 1);
            _sineProvider.Frequency = (float)FreqSlider.Value;
            _sineProvider.Amplitude = (float)AmpSlider.Value;

            _waveOut = new WaveOutEvent();
            _waveOut.Init(_sineProvider);
            _waveOut.Play();
        }

        private void StopTone()
        {
            _waveOut?.Stop();
            _waveOut?.Dispose();
            _waveOut = null;
            _sineProvider = null;
        }

        // Slider bewegt → Textbox & Ton aktualisieren
        private void FreqSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double freq = e.NewValue;
            UpdateFrequencyDisplay(freq);

            if (_sineProvider != null)
                _sineProvider.Frequency = (float)freq;
        }

        private void AmpSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (_sineProvider != null)
            {
                _sineProvider.Amplitude = (float)e.NewValue;
            }
        }

        // Textbox verlassen → Slider aktualisieren
        private void FreqTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            ApplyTextboxFrequency();
        }

        // ENTER in Textbox gedrückt → Slider aktualisieren
        private void FreqTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ApplyTextboxFrequency();
                Keyboard.ClearFocus(); // Textbox verlässt Fokus
            }
        }

        // Hilfsmethode: Textbox-Text → Frequenz übernehmen
        private void ApplyTextboxFrequency()
        {
            if (double.TryParse(FreqTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out double enteredFreq))
            {
                enteredFreq = Math.Clamp(enteredFreq, FreqSlider.Minimum, FreqSlider.Maximum);
                FreqSlider.Value = enteredFreq;
            }
            else
            {
                // Ungültige Eingabe → zurücksetzen
                FreqTextBox.Text = ((int)FreqSlider.Value).ToString();
            }
        }

        private void UpdateFrequencyDisplay(double freq)
        {
            if (FreqLabel != null)
                FreqLabel.Text = $"{(int)freq} Hz";
            if (FreqTextBox != null)
                FreqTextBox.Text = ((int)freq).ToString();
        }
    }
}