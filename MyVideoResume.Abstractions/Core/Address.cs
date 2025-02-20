﻿using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;

public class Address : Location
{    
    //User who owns the Task
    public string UserId { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public string Line1 { get; set; }

    public string? Line2 { get; set; }

    public string PostalZipCode { get; set; }

    public string StateProvince { get; set; }

    public bool IsDefault { get; set; }

    public CommunicationType ContactType { get; set; }
}

public class Location : GISData
{
    public string? Name { get; set; }
}

public class GISData : CommonBase
{
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

}