using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UK101Library
{
    internal interface IMicro
    {
        #region Properties

        #endregion
        #region Methods

        void Init(int height);

        void Run();

        void Reset();

        #endregion

    }
}
