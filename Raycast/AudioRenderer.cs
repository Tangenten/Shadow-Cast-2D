using System;
using SFML.Audio;
using SFML.System;

namespace netcore {
	internal class AudioRenderer : SoundStream {
		private readonly uint channels;
		private readonly uint sampleRate;
		private readonly uint bufferSize;

		public AudioRenderer() {
			this.channels = 2;
			this.sampleRate = 44100;
			this.bufferSize = 44100;

			this.Initialize(this.channels, this.sampleRate);
		}

		public short[] GenerateSineWave(uint length) {
			short[] samples = new short[length];

			float frequency = 440;
			float volume = 0.15f;

			for (int i = 0; i < length; i++) {
				samples[i] = (short) (Math.Sin(frequency * (2 * Math.PI) * i / this.sampleRate) * volume * short.MaxValue);
			}

			return samples;
		}

		public SoundBuffer GenerateSineWave(double frequency, double volume, int seconds) {
			short[] samples = new short[seconds * this.sampleRate];

			for (int i = 0; i < samples.Length; i++) {
				samples[i] = (short) (Math.Sin(frequency * (2 * Math.PI) * i / this.sampleRate) * volume * short.MaxValue);
			}

			return new SoundBuffer(samples, 1, this.sampleRate);
		}

		protected override bool OnGetData(out short[] samples) {
			samples = this.GenerateSineWave(this.bufferSize);

			return true;
		}

		protected override void OnSeek(Time timeOffset) {
		}
	}
}