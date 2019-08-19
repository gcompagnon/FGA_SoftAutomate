using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FGA.Automate.Core;

namespace FGA.Automate.Dataconverter
{

    public class EmptyDataConverter : DataConverter
    {
        public override void Init()
        {
        }

        protected override Object Convert(Object inObject)
        {
            return inObject;
        }
    }
    /// <summary>
    ///  Abstract class for implementing DataConverter classes.
    /// <li>Each Binding component uses an instance of a specific DataConverter </li>
    /// <li>by default, the included one makes no convertion</li>
    /// For creating a new DataConverter, a subclasse must implement :
    /// <li>init method , that is called once at the starting, it could be use for initialize factory or singleton</li>
    /// <li>convert method, that is called at each message exchange, the parameter is the input and the result should be returned.</li>
    /// In case of exception, the proposed output is null.
    /// </summary>
    public abstract class DataConverter
    {

        /**
        * Contructor for a final converter
        */
        protected DataConverter()
        {
        }

        /**
        * Init of the converter
        * @throws Exception
        */
        public abstract void Init();


        /// <summary>
        /// Utility method for converting the Object given in another type
        ///	 the input could be firstly converted by the included converter before its own convertion
        /// </summary>
        /// <param name="inObject">input as a list of objects of a given type</param>
        /// <returns>output of the convertion</returns>
        public Object _convert(Object inObject)
        {
            try
            {
                // if there is an included DC ... call its convertion first
                if (IncludedDC != null)
                {
                    inObject = IncludedDC._convert(inObject);
                }
                // call of the subclass implementation
                Object outObject = this.Convert(inObject);
                return outObject;
            }
            catch (Exception cce)
            {
                BindingComponent.ExceptionLogger.Fatal("Not convert what it was expected ", cce);
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="inObject"></param>
        /// <returns></returns>
        protected abstract Object Convert(Object inObject);

        /// <summary>
        /// Set the DataConverter that is linked with an included other dataconverter.
        /// </summary>
        /// <param name="includedDC"></param>
        public DataConverter IncludedDC
        {
            private get { return IncludedDC; }
            set { IncludedDC = value; }
        }

    }/// FIN DATACONVERTER
}
