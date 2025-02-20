﻿using System;

namespace Task47.Models
{
    public class FilterRequest
    {
        public DateTime CreationDate { get; set; }
        public DateTime ModificationDate { get; set; }
        public string Owner { get; set; }
        public FilterType FilterType { get; set; }
    }
}
