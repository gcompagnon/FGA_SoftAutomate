using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FGA.Automate.Core
{
    /**
    * Interface for grouping a life cycle methods
    *
    */
    public interface ComponentLifeCycle
    {
        /** use for the initialization and instanciation (factory, pool ...) */
        void Init();
        /** use for starting (threads start) */
        void Start();
        /** stop softly with waiting the end of message exchange */
        void Stop();
        /** immediat shutdown */
        void Shutdown();
    }
}
