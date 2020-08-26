using System.Collections.Generic;
using System.Linq;

namespace Alidu.Common
{
    public class Sort
    {
        private readonly List<string> _asc;
        private List<string> _desc;

        public Sort(params string[] sorts)
        {
            _asc = new List<string>();
            _desc = new List<string>();
            if (sorts == null || !sorts.Any())
                HasSort = false;
            else
            {
                HasSort = true;
                foreach (var item in sorts)
                {
                    if (item.StartsWith('+'))
                        _asc.Add(item.Replace("+", ""));
                    if (item.StartsWith('-'))
                        _desc.Add(item.Replace("-", ""));
                }
            }
        }

        public bool HasSort { get; }
        public IReadOnlyCollection<string> Asc => _asc;
        public IReadOnlyCollection<string> Desc => _desc;
    }
}
