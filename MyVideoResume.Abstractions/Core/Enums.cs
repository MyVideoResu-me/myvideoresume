using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Core;

public enum DisplayPrivacy
{
    [Description("Visible to Public")]
    ToPublic = 0,
    [Description("Visible to Recruiters")]
    ToRecruiters = 1,
    [Description("Visible to Connections")]
    ToConnections = 2,
    [Description("Visible to Self (Private)")]
    ToSelf = 10
}