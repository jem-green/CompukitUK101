using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KansasCityStandard
{
    public class Test
    {
         public void Generate()
        {

            uint numsamples = 44100;
            ushort numchannels = 1;
            ushort samplelength = 1; // in bytes
            uint samplerate = 22050;

            FileStream f = new FileStream("a.wav", FileMode.Create);
            BinaryWriter wr = new BinaryWriter(f);

            wr.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
            wr.Write(36 + numsamples * numchannels * samplelength);
            wr.Write(System.Text.Encoding.ASCII.GetBytes("WAVEfmt "));
            wr.Write(16);
            wr.Write((ushort)1);
            wr.Write(numchannels);
            wr.Write(samplerate);
            wr.Write(samplerate * samplelength * numchannels);
            wr.Write(samplelength * numchannels);
            wr.Write((ushort)(8 * samplelength));
            wr.Write(System.Text.Encoding.ASCII.GetBytes("data"));
            wr.Write(numsamples * samplelength);

            // for now, just a square wave
            //Waveform a = new Waveform(440, 50);

            //double t = 0.0;
            //for (int i = 0; i < numsamples; i++, t += 1.0 / samplerate)
            //{
            //    wr.Write((byte)((a.sample(t) + (samplelength == 1 ? 128 : 0)) & 0xff));
            //}
        }
    }
}
