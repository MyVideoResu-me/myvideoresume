using System;
using Microsoft.Extensions.ML;
using Microsoft.AspNetCore.Mvc;
using MyVideoResume.ML.SentimentAnalysis.DataModels;

namespace MyVideoResume.ML.SentimentAnalysis;

/// <summary>
/// Controller for handling sentiment analysis operations.
/// </summary>
[Route("[controller]")]
[ApiController]
public class SentimentController : ControllerBase
{
    private readonly PredictionEnginePool<SampleObservation, SamplePrediction> _predictionEnginePool;

    /// <summary>
    /// Initializes a new instance of the <see cref="SentimentController"/> class.
    /// </summary>
    /// <param name="predictionEnginePool">The ML.NET prediction engine pool for sentiment analysis.</param>
    public SentimentController(PredictionEnginePool<SampleObservation, SamplePrediction> predictionEnginePool)
    {
        // Get the ML Model Engine injected, for scoring
        _predictionEnginePool = predictionEnginePool;
    }

    /// <summary>
    /// Analyzes the sentiment of the provided text.
    /// </summary>
    /// <param name="sentimentText">The text to analyze for sentiment.</param>
    /// <returns>A sentiment score between 0 (negative) and 100 (positive).</returns>
    /// <response code="200">Returns the sentiment score.</response>
    /// <response code="400">If the input text is null or empty.</response>
    [HttpPost("sentimentprediction")]
    [ProducesResponseType(typeof(float), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<float> PredictSentimentPost([FromBody] string sentimentText)
    {
        if (string.IsNullOrEmpty(sentimentText))
        {
            return BadRequest("Input text cannot be null or empty");
        }

        return ProcessSentimentPrediction(sentimentText);
    }

    /// <summary>
    /// Processes the sentiment prediction using the ML model.
    /// </summary>
    /// <param name="sentimentText">The text to analyze.</param>
    /// <returns>The sentiment score.</returns>
    [NonAction]
    private ActionResult<float> ProcessSentimentPrediction(string sentimentText)
    {
        // Predict sentiment using ML.NET model
        SampleObservation sampleData = new SampleObservation { Col0 = sentimentText };

        // Predict sentiment
        SamplePrediction prediction = _predictionEnginePool.Predict(sampleData);

        float percentage = CalculatePercentage(prediction.Score);

        return percentage;
    }

    /// <summary>
    /// Converts the ML model's raw score to a percentage between 0 and 100.
    /// </summary>
    /// <param name="value">The raw score from the ML model.</param>
    /// <returns>A percentage between 0 and 100.</returns>
    [NonAction]
    private float CalculatePercentage(double value)
    {
        return 100 * (1.0f / (1.0f + (float)Math.Exp(-value)));
    }
}
