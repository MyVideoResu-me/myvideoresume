# [MyVideoResu.ME](https://myvideoresu.me)
My Video Resume is an Open Source Platform that enhances your resume with Content (Video,Images, Links)

[![.NET](https://github.com/SeanHogg/myvideoresume/actions/workflows/dotnet.yml/badge.svg)](https://github.com/SeanHogg/myvideoresume/actions/workflows/dotnet.yml)

## AI/ML Resume Tools
- [Sentiment Analysis](https://myvideoresu.me/Tools/SentimentAnalysis). Allows you to verify that the content of your resume is positive.
- [Resume Summarization](https://myvideoresu.me/Tools/SummarizeResume). Allows you to summarize your resume.
- [Job + Resume Match](https://myvideoresu.me/Tools/JobResumeMatch). Allows you to analyze your resume with a job description. Determine if you are a good match. 

## ⚒️ Resume Builder (Coming Soon)
## 🔍 Resume Parser 
- [PDF Resume to JSON Resume Parser](https://myvideoresu.me/Tools/pdftojson). Allows you convert your resume into a standard JSON format.

## 📚 Tech Stack

| <div style="width:140px">**Category**</div> | <div style="width:100px">**Choice**</div> | **Descriptions** |
|---|---|---|
| **Language** | [TypeScript](https://github.com/microsoft/TypeScript) | TypeScript is JavaScript with static type checking and helps catch many silly bugs at code time. |
| **Language** | [C#, .NET 8](https://github.com/microsoft/dotnet) | Microsoft .NET is a mature and fast framework. |
| **AI/ML** | [ML.NET](https://dotnet.microsoft.com/en-us/apps/machinelearning-ai/ml-dotnet), [Phi-3-Mini](https://huggingface.co/microsoft/Phi-3-mini-4k-instruct-onnx/tree/main/cpu_and_mobile/cpu-int4-rtn-block-32) | An open source and cross-platform framework for AI & machine learning (ML). |
| **Web Framework** | [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) | Blazor is a modern front-end web framework based on HTML, CSS, and C# that helps you build web apps fast. |
| **Web UI Library** | [Radzen-Blazor](https://github.com/radzenhq/radzen-blazor) | A set of 90+ free and open source native Blazor UI controls. |
| **PDF Reader** | [PDF.js](https://github.com/mozilla/pdf.js) | PDF.js reads content from PDF files and is used by the resume parser at its first step to read a resume PDF’s content. |


## Data (MyVideoResume.Server)
The project uses Entity Framework (CodeFirst) Migrations. All EF Patterns are applicable. 

`dotnet tool install --global dotnet-ef`

Update Tools:
`dotnet tool update --global dotnet-ef`

The Following Contexts are available:

* DataContext > uses the "Default" Connection String - CRUD actions in the primary Database (All Data including Auth tables)


### Steps to Add Migration and to Update the Database
1. Add Entities to DataContext.
1. Change Working Directory to MyVideoResume.Server via the Commandline. (Verify you have the latest code and it compiles)
1. Create a Migration:

`dotnet ef migrations add "GiveYourMigrationANameHere" --output-dir "Migrations\MyVideoResume" --context DataContext`

4. Update the Database 
[If you have verified everything works locally. You should Push your latest changes, verify build and deploy is successful, then run]

`dotnet ef database update --context DataContext`

(if you need to remove a migration `dotnet ef migrations remove --context DataContext`) 


## ML/AI Research
- https://jamiemaguire.net/index.php/2024/09/01/semantic-kernel-implementing-100-local-rag-using-phi-3-with-local-embeddings/
- https://github.com/elbruno/phi3-labs/blob/main/src/LabsPhi301/Program.cs
- https://elbruno.com/2024/05/31/powering-up-net-apps-with-phi-3-and-semantickernel/
- https://devblogs.microsoft.com/dotnet/using-phi3-csharp-with-onnx-for-text-and-vision-samples-md/
- https://accessibleai.dev/post/introtosemantickernel/
- https://learn.microsoft.com/en-us/dotnet/ai/semantic-kernel-dotnet-overview
- https://news.microsoft.com/source/features/ai/the-phi-3-small-language-models-with-big-potential/
- https://github.com/microsoft/Phi-3CookBook
- https://learn.microsoft.com/en-us/dotnet/ai/get-started-app-chat-template?tabs=github-codespaces
- https://learn.microsoft.com/en-us/azure/search/retrieval-augmented-generation-overview
- https://github.com/SciSharp/LLamaSharp
- https://build5nines.com/build-a-generative-ai-app-in-c-with-phi-3-mini-llm-and-onnx/