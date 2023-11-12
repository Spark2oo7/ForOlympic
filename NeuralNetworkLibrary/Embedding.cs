using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.ML;
using Microsoft.ML.Transforms.Text;

namespace NeuralNetworkLibrary
{
    internal class Embedding
    {
        private PredictionEngine<TextInput, TextFeatures> predictionEngine;

        public Embedding()
        {
            var context = new MLContext();

            var emptyData = context.Data.LoadFromEnumerable(new List<TextInput>());

            Console.WriteLine("Start");

            var pipeline = context.Transforms.Text.NormalizeText("Text", null,
            keepDiacritics: false, keepNumbers: false, keepPunctuations: false)
            .Append(context.Transforms.Text.TokenizeIntoWords("Tokens", "Text"))
            .Append(context.Transforms.Text.RemoveDefaultStopWords("WordsWithoutStopWords", "Tokens", Microsoft.ML.Transforms.Text.StopWordsRemovingEstimator.Language.English))
            .Append(context.Transforms.Text.ApplyWordEmbedding("Features", "WordsWithoutStopWords",
                Microsoft.ML.Transforms.Text.WordEmbeddingEstimator.PretrainedModelKind.GloVeTwitter25D));

            var embeddingTransformer = pipeline.Fit(emptyData);

            predictionEngine = context.Model.CreatePredictionEngine<TextInput, TextFeatures>(embeddingTransformer);

            Console.WriteLine("End");
        }

        public float[] Predict(string data)
        {
            var input = new TextInput { Text = data };
            return predictionEngine.Predict(input).Features;
        }
    }
}
