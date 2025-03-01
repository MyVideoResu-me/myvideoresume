﻿using MyVideoResume.Abstractions.Resume;
using MyVideoResume.Abstractions.Resume.Formats.JSONResumeFormat;

namespace MyVideoResume.Abstractions.Match;

public class JobResumeBestResumeResponse
{
    public JSONResume Resume { get; set; }
    public string SummaryRecommendations { get; set; }
    public float OldScore { get; set; }
    public float NewScore { get; set; }
}
