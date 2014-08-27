using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.Phone.Speech.Synthesis;

using XamFormsGPSDependency.DependencySvcTts;
using XamFormsGPSDependency.WinPhone.DependencySvcTts;


[assembly: Xamarin.Forms.Dependency (typeof (TextToSpeech_WP))]

namespace XamFormsGPSDependency.WinPhone.DependencySvcTts
{
    public class TextToSpeech_WP : ITextToSpeech
    {
        public TextToSpeech_WP() { }

        public async void Speak(string text)
        {
            SpeechSynthesizer synth = new SpeechSynthesizer();
            await synth.SpeakTextAsync(text);
        }
    }
}
