using System;
using System.Linq;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.AudioFormat;
using System.IO;

namespace KinectMenu
{

    /// <summary>
    /// The class that handles audio from the kinect.
    /// </summary>
    class SpeechEngine
    {

        /// <summary>
        /// Handles the Singleton
        /// </summary>
        #region Singleton

        private static SpeechEngine _instance;

        public static SpeechEngine getInstance()
        {
            if (_instance == null)
            {
                _instance = new SpeechEngine();
            }
            return _instance;
        }

        private SpeechEngine()
        {
            InitializeSpeech();
        }

        #endregion Singleton

        #region Event Handlers

        /// <summary>
        /// A handle to the Windows Speech Recognition Engine
        /// </summary>
        private SpeechRecognitionEngine speechEngine;

        /// <summary>
        /// A handle to the Kinect audio source
        /// </summary>
        private KinectAudioSource source;

        /// <summary>
        /// Registers a speech recognized event with the speech engine.
        /// </summary>
        /// <param name="handler">The speech recognized handler to register</param>
        public void registerSpeechRecognized(EventHandler<SpeechRecognizedEventArgs> handler)
        {
            speechEngine.SpeechRecognized += handler;
        }

        /// <summary>
        /// Removes the speech recognized event handler from the speech engine.
        /// </summary>
        /// <param name="handler"></param>
        public void removeSpeechRecognized(EventHandler<SpeechRecognizedEventArgs> handler)
        {
            speechEngine.SpeechRecognized -= handler;
        }

        /// <summary>
        /// Registers a speech rejected event with the speech engine.
        /// </summary>
        /// <param name="handler">The speech rejected handler to register</param>
        public void registerSpeechRejected(EventHandler<SpeechRecognitionRejectedEventArgs> handler)
        {
            speechEngine.SpeechRecognitionRejected += handler;
        }

        /// <summary>
        /// Removes the speech rejected event handler from the speech engine.
        /// </summary>
        /// <param name="handler"></param>
        public void removeSpeechRejected(EventHandler<SpeechRecognitionRejectedEventArgs> handler)
        {
            speechEngine.SpeechRecognitionRejected -= handler;
        }

        #endregion Event Handlers

        #region Initialization

        /// <summary>
        /// Called by the constructor. Sets up the kinect audio source, the grammar and the speech engine.
        /// </summary>
        private void InitializeSpeech()
        {
            RecognizerInfo ri = InitializeKinectAudio();

            // Create a set of menu options choices, create a SemanticResultKey and create a grammarBuilder
            Choices menuOptions = new Choices(new String[] { 
                "new game", "load game", "options", "quit", "game 1", "game 2", "game 42", "game 666", "campaign", "zen mode", "score mode", "multiplayer", "volume", "difficulty", "screen brightness", "cancel"
            });
            SemanticResultKey menuOptionsKey = new SemanticResultKey("menuOptions", menuOptions);
            GrammarBuilder menuGrammar = new GrammarBuilder(menuOptionsKey);

            // Create a set of command choices, create a SemanticResultKey and create a grammarBuilder
            Choices command = new Choices(new String[] { "kinect" });
            SemanticResultKey commandKey = new SemanticResultKey("command", command);
            GrammarBuilder commandGrammar = new GrammarBuilder(commandKey);

            //By combining the grammar builders, the user always has to say the command before the menu options
            GrammarBuilder combinedGrammarBuilders = commandGrammar + menuGrammar;
            Grammar combinedGrammar = new Grammar(combinedGrammarBuilders);

            speechEngine = new SpeechRecognitionEngine();
            speechEngine.LoadGrammar(combinedGrammar);

            Stream s = source.Start();
            speechEngine.SetInputToAudioStream(s,
                new SpeechAudioFormatInfo(
                EncodingFormat.Pcm, 16000, 16, 1,
                32000, 2, null));

            speechEngine.RecognizeAsync(RecognizeMode.Multiple);
            Console.WriteLine("Speech engine running");
        }

        /// <summary>
        /// Finds and sets up the Kinect audio source.
        /// </summary>
        /// <returns></returns>
        RecognizerInfo InitializeKinectAudio()
        {
            Console.Write("Getting audio source from Kinect...");
            source = new KinectAudioSource();

            source.FeatureMode = true;
            source.AutomaticGainControl = false; //Important to turn this off for speech recognition
            source.SystemMode = SystemMode.OptibeamArrayOnly; //No AEC for this sample

            RecognizerInfo ri = GetKinectRecognizer();

            if (ri == null)
            {
                Console.WriteLine("Could not find Kinect speech recognizer. Please refer to the sample requirements.");
                return null;
            }

            Console.WriteLine("Audio source created");
            return ri;
        }

        private static RecognizerInfo GetKinectRecognizer()
        {
            Func<RecognizerInfo, bool> matchingFunc = r =>
            {
                string value;
                r.AdditionalInfo.TryGetValue("Kinect", out value);
                return "True".Equals(value, StringComparison.InvariantCultureIgnoreCase) && "en-US".Equals(r.Culture.Name, StringComparison.InvariantCultureIgnoreCase);
            };
            return SpeechRecognitionEngine.InstalledRecognizers().Where(matchingFunc).FirstOrDefault();
        }

        #endregion Initialization

    }
}
