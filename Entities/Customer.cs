using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WorkerService.Utils;

namespace WorkerService.Entities;

public class Customer
{
    public string Name { get; set; }
    public string Email { get; set; }
    public StatementPreference Preference { get; set; }
    public DateTime LastStatementSent { get; set; }
}
