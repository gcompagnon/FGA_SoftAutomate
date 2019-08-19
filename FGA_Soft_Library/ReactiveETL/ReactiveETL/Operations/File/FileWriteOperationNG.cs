using ReactiveETL.Activators;
using System;

namespace ReactiveETL.Operations.File
{
    /// <summary>
    /// Operation to write to a file
    /// </summary>
    public class FileWriteOperation : AbstractOperation
    {
        private FileWriteActivator _activator;
        private Type _classType;

        /// <summary>
        /// File Write constructor
        /// </summary>
        /// <param name="activator">file write parameters</param>
        /// <param name="T">class of the object</param>
        public FileWriteOperation(FileWriteActivator activator, Type T)
        {
            _activator = activator;
            _classType = T;
        }

        /// <summary>
        /// Method called by OnNext > Dispatch to process the notified value. This method just return the value and could be overriden in subclasses
        /// </summary>
        /// <param name="value">pipelined value</param>
        /// <returns>treated row</returns>
        protected override Row TreatRow(Row value)
        {
            _activator.InitializeEngine();
            _activator.Engine.Write(value.ToObject(_classType));

            return base.TreatRow(value);
        }

        /// <summary>
        /// Notifies the observer of the end of the sequence.
        /// </summary>
        public override void OnCompleted()
        {
            _activator.Release();
            base.OnCompleted();
        }
    }
}