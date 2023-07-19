using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebAPI_Test.Models;

public partial class Status
{
    [Key] //設假象 KEY賦予給 "ColAge"
    public int ColAge { get; set; }

    public string ColPosition { get; set; } = null!;

    public string? ColNote { get; set; }
}
