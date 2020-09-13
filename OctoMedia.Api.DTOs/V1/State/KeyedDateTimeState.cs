using System;
using System.Collections.Generic;

namespace OctoMedia.Api.DTOs.V1.State
{
    public class KeyedDateTimeState : State
    {
        public Dictionary<string, DateTime> Values { get; set; } = null!;
    }
}