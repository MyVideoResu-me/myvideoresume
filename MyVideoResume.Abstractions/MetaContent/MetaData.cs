﻿using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.MetaContent;

public class MetaDataDTO : MetaData
{
    public string Id { get; set; }
}

public class MetaData : CommonBase
{
    public string UserId { get; set; }
    public string ReferenceId { get; set; }
    public string Name { get; set; }
    public MetaType MetaType { get; set; } = MetaType.HtmlContent;
    public string Url { get; set; }
    public string Description { get; set; }
    public DisplayPrivacy? Privacy { get; set; } = DisplayPrivacy.ToPublic;
}
