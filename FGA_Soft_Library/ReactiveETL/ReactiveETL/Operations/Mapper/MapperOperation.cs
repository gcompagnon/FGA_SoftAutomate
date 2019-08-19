using System;
using ReactiveETL.Activators;

namespace ReactiveETL.Operations.Mapper
{
    using System.Data;

    /// <summary>
    /// Operation that uses the Automapper
    /// </summary>
         public class MapperOperation : AbstractOperation
    {
             private Type _destinationClassType;
             private Type _sourceClassType;


        /// <summary>
        /// Operation de mapping entre 2 types
        /// </summary>
        /// <param name="sourceT"></param>
        /// <param name="destinationT"></param>
             public MapperOperation(Type sourceT,Type destinationT)
        {
            _sourceClassType = sourceT;
            _destinationClassType = destinationT;
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. This method just return the value and could be overriden in subclasses
        /// </summary>
        /// <param name="value">pipelined value</param>
        /// <returns>treated row</returns>
        protected override Row TreatRow(Row value)
        {
            object obj = value.ToObject(_sourceClassType);
            object dest = AutoMapper.Mapper.Map(obj, _sourceClassType, _destinationClassType);

            value = Row.FromObject(dest);

            return base.TreatRow(value);
        }

    }
}