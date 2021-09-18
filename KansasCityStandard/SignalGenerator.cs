
using System;

namespace KansasCityStandard
{
    public class SignalGenerator
    {
        private readonly double _frequency;
        private readonly UInt32 _sampleRate;
        private readonly UInt16 _secondsInLength;
        private short[] _dataBuffer;
        private SignalType _signalType = SignalType.Sine;

        public enum SignalType
        {
            Sine,
            Square,
            Triangle,
            Sawtooth
        }

        #region Constructor

        public SignalGenerator(SignalType type ,double frequency, UInt32 sampleRate, UInt16 secondsInLength)
        {
            _signalType = type;
            _frequency = frequency;
            _sampleRate = sampleRate;
            _secondsInLength = secondsInLength;
            GenerateData();
        }

        #endregion
        #region Properties

        /// <summary>
        /// Signal Type.
        /// </summary>
        public SignalType Type
        {
            get
            {
                return _signalType;
            }
            set
            {
                _signalType = value;
            }
        }

        /// <summary>
        /// Data
        /// </summary>
        public short[] Data
        {
            get
            { 
                return _dataBuffer;
            } 
        }

        #endregion
        #region Methods

        private void GenerateData()
        {
            uint bufferSize = _sampleRate * _secondsInLength;
            _dataBuffer = new short[bufferSize];

            int amplitude = 32760;

            double timePeriod = (Math.PI * 2 * _frequency) / (_sampleRate);

            if (_signalType == SignalType.Sine)
            {
                for (uint index = 0; index < bufferSize - 1; index++)
                {
                    _dataBuffer[index] = Convert.ToInt16(amplitude * Math.Sin(timePeriod * index));
                }
            }
            else if (_signalType == SignalType.Square)
            {
                for (uint index = 0; index < bufferSize - 1; index++)
                {
                    _dataBuffer[index] = Convert.ToInt16(amplitude * Math.Sign(Math.Sin(timePeriod * index)));
                }
            }
        }

        #endregion
    }
}