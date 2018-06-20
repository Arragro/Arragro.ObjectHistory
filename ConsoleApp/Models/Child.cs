using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp.Models
{
    public class Child
    {
        public int ChildId { get; set; }
        public string Test { get; set; }

        public Parent Parent { get; set; }
    }
}
