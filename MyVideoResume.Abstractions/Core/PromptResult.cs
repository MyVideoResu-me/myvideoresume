using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;

public class ResponseResult<T>
{
    public ErrorCodes? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    public T Result { get; set; }
}

public class ResponseResult: ResponseResult<string>
{
}
