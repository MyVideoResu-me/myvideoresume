
using Docnet.Core;
using Docnet.Core.Models;
using Microsoft.AspNetCore.Http;
using Spire.Doc;

namespace MyVideoResume.Documents;

public interface IDocumentProcessor
{
    string ConvertToString(IFormFile file);
    string WordToString(Stream stream);
    string PdfToString(System.IO.Stream stream);
    string JSONToString(System.IO.Stream stream);
}

public class DocumentProcessor : IDocumentProcessor
{

    public string ConvertToString(IFormFile file)
    {
        var result = string.Empty;

        switch (file.ContentType)
        {
            case "application/json":

                result = JSONToString(file.OpenReadStream());
                break;
            case "application/pdf":
                result = PdfToString(file.OpenReadStream());
                break;
            case "application/msword":
            case "application/vnd.openxmlformats-officedocument.wordprocessingml.document":
                result = WordToString(file.OpenReadStream());
                break;
            default:
                break;
        }
        return result;
    }

    public string WordToString(Stream stream)
    {
        var result = string.Empty;

        using Document doc = new Document(stream);
        result = doc.GetText();
        doc.Close();

        return result;
    }

    public string JSONToString(Stream stream)
    {
        var result = string.Empty;
        using (StreamReader reader = new StreamReader(stream))
        {
            result = reader.ReadToEnd();
        }
        return result;
    }

    public string PdfToString(Stream stream)
    {
        var result = string.Empty;
        using (IDocLib lib = DocLib.Instance)
        {
            byte[] bytes;
            List<byte> totalStream = new();
            byte[] buffer = new byte[32];
            int read;
            while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                totalStream.AddRange(buffer.Take(read));
            }
            bytes = totalStream.ToArray();
            using var docReader = lib.GetDocReader(bytes, new PageDimensions());
            for (var i = 0; i < docReader.GetPageCount(); i++)
            {
                using (var pageReader = docReader.GetPageReader(i))
                {
                    result += pageReader.GetText();
                }
            }
        }

        return result;
    }
}
