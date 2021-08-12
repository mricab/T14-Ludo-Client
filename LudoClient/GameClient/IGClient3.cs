using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameClient
{
    public interface IGClient3
    {
        void OnGBoard(GBoardEvent e);

        void OnGUpdate(GUpdateEvent e);
    }
}
