using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Strata {
    public delegate dynamic F(dynamic x);
    public delegate K Adapter<T, K>(T x);
}
