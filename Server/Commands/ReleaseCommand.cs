﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class ReleaseCommand : Command
    {
        public override string Execute(Player source, string arguments)
        {
            return source.Controlling.Detach();
        }
    }
}
