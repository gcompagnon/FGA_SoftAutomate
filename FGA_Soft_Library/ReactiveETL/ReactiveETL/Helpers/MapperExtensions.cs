using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ReactiveETL.Operations.Database;
using ReactiveETL.Operations.Mapper;

namespace ReactiveETL
{
       /// <summary>
    /// Extension methods for AutoMapper extension
    /// </summary>
    public static class MapperExtensions
    {
        /// <summary>
        /// Mapper entre 2 objets
        /// </summary>
        /// <param name="observed"></param>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public static MapperOperation Mapper(this IObservableOperation observed, Type sourceType, Type destinationType)
        {
            MapperOperation cmd = new MapperOperation(sourceType,destinationType);
            observed.Subscribe(cmd);
            return cmd;
        }
    }
}
