using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyVideoResume.Application.Importers;

public class Importer<T> where T : class, new()
{
    public Importer() { }

    public virtual T Import(string jsonString)
    {
        var result = JsonSerializer.Deserialize<T>(jsonString, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        if (result == null)
        {
            result = new T();
        }
        return result;
    }
}