using MyVideoResume.Abstractions.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVideoResume.Abstractions.Business;

public class TodoDTO : Todo {

    public string Id { get; set; }

    public ContactPersonDTO CreatedByUser { get; set; }
    public ContactPersonDTO AssignedToUser { get; set; }

}

public enum TodoType { 
    General = 0,
    Project = 1,
    Recruiter = 10, 
    JobSeeker = 20,
    Company = 30, //Update Company Profile; Add Contact Details; Add Job;
    Profile = 40, //Update Profile; Add Contact Details
    Job = 50,
    Resume = 60,
}

public class Todo: CommonBase, IActionItem
{
    //User who owns the Task
    public string CreatedByUserId { get; set; }

    public string AssignedToUserId { get; set; }

    public string Text { get; set; }


    public DateTime Start { get; set; }
    public DateTime End { get; set; }

    public ActionItemStatus Status { get; set; }
}