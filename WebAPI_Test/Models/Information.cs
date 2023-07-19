using System;
using System.Collections.Generic;

namespace WebAPI_Test.Models;

public partial class Information
{
    public int Id { get; set; }

    public string ColName { get; set; } = null!;

    public int ColAge { get; set; }

    public string ColAddress { get; set; } = null!;

    public string ColProfession { get; set; } = null!;

    public DateTime DateTime { get; set; }
}
