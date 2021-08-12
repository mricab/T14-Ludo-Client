using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public class GLoginFailureEvent
    {
        public GLoginFailureData data;

        public GLoginFailureEvent(object source, GLoginFailureData data)
        {
            this.data = data;
        }
    }
}
