using System;
using System.Collections.Generic;
using ComponentDetective.Crawler.Models;

namespace ComponentDetective.Crawler.Comparer
{
    internal class LibraryReferenceComparer : IEqualityComparer<LibraryReference>
    {
        public bool Equals(LibraryReference x, LibraryReference y)
        {
            if (ReferenceEquals(x, y))
            {
                return true;
            }

            if(x.Name.Equals(y.Name, StringComparison.InvariantCultureIgnoreCase))
            {
                return x.HintPath.Equals(y.HintPath, StringComparison.InvariantCultureIgnoreCase);
            }

            return false;
        }

        public int GetHashCode(LibraryReference obj)
        {
            return obj.Name.GetHashCode();
        }
    }
}