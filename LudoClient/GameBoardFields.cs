using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudoClient
{
    enum GameBoardFields
    {
        Brown,  //0
        BrownUp,
        BrownRight,
        BrownDown,
        BrownLeft,
        BrownDownLeft,
        BrownDownRight,
        BrownUpLeft,
        BrownUpRight,
        empty9,

        BrownBox,   //10
        RedBox,
        YellowBox,
        GreenBox,
        BlueBox,
        empty15,
        empty16,
        empty17,
        empty18,
        empty19,

        RedUp, // 20
        RedBlue,
        BlueLeft,
        BlueGreen,
        GreenDown,
        GreenYellow,
        YellowLeft,
        YellorRed,
        Center,
    }
}
