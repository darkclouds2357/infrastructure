using System;

namespace Alidu.Core.Common
{
    public class Pagging
    {
        public Pagging(int? limit, int? offset)
        {
            HasPagging = limit.HasValue && offset.HasValue;
            if (HasPagging)
            {
                Limit = limit.Value;
                Offset = offset.Value;
            }
        }

        public bool HasPagging { get; }

        public int Limit { get; private set; }
        public int Offset { get; private set; }

        public int Page => Offset + 1;
        public int SkipItems => Page <= 1 ? 0 : (Page - 1) * Limit;
    }
}
