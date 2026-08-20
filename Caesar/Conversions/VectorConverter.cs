using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace Caesar.Conversions
{
    public class VectorConverter : ValueConverter<float[], string>
    {
        public VectorConverter()
            : base(
                  vector => JsonSerializer.Serialize(vector),
                  vStr => JsonSerializer.Deserialize<float[]>(vStr) ?? new float[Constants.DIMENSIONS])
        { }
    }
}
