using System;
using log4net;
using FGA.Automate.Dataconverter;

namespace FGA.Automate.Core
{

    /// <summary>
    /// Common methods of Producers and Consumers : management of their LifeCycle
    /// <br/>
    /// Contains the constants for the status
    /// </summary>
    public abstract class BindingComponent : ComponentLifeCycle
    {
        public const byte NOT_INSTALLED = 0;
        public const byte INSTALLED = 1;
        public const byte START_ASKED = 2;
        public const byte STARTED = 3;
        public const byte STOP_ASKED = 4;
        public const byte STOPPED = 5;

        public byte status = NOT_INSTALLED;

        public static ILog ExceptionLogger
        {
            get { return LogManager.GetLogger("FGA_Soft_ERREUR"); }
        }

        private readonly DataConverter defaultDataConverter = new EmptyDataConverter();

        public byte Status
        {
            get { return Status; }
            set { status = value; }
        }

        public void Start(CoreServiceEngine engine)
        {
            if (this.Status != BindingComponent.NOT_INSTALLED)
            {
                this.Status = BindingComponent.START_ASKED;
                this.CoreServiceEngine = engine;
                this.Start();
                this.Status = BindingComponent.STARTED;
            }
        }


        public void Stop()
        {
            this.Status = BindingComponent.STOP_ASKED;
        }

        public DataConverter IncludedDataConverter
        {
            get { return IncludedDataConverter; }
            set {IncludedDataConverter=value; }
        }

        protected CoreServiceEngine CoreServiceEngine
        {
            get { return CoreServiceEngine; }
            set { CoreServiceEngine = value; }            
        }


        public void Init()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
