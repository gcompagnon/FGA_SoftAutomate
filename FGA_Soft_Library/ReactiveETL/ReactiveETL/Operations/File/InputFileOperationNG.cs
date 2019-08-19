using System;
using System.IO;
using System.Collections;
using ReactiveETL.Files;

namespace ReactiveETL.Operations.File
{
    /// <summary>
    /// Operation for reading a file
    /// </summary>
    public class InputFileOperation : AbstractObservableOperation
    {
        private string _filename;
        private Stream _strm;
        private StreamReader _strmReader;
        private Type _type;

        /// <summary>
        /// File read constructor
        /// </summary>
        /// <param name="filename">full path to the file</param>
        /// <param name="type">type of the objects in the file</param>
        public InputFileOperation(string filename,Type type)
        {
            _filename = filename;
            _type = type;
        }

        /// <summary>
        /// File read constructor
        /// </summary>
        /// <param name="strm">Stream to the file</param>
        /// <param name="type">type of the objects in the file</param>
        public InputFileOperation(Stream strm,Type type)
        {
            _strm = strm;
            _type = type;
        }

        /// <summary>
        /// File read constructor
        /// </summary>
        /// <param name="strmReader">Stream to the file</param>
        /// <param name="type">type of the objects in the file</param>
        public InputFileOperation(StreamReader strmReader,Type type)
        {
            _strmReader = strmReader;
            _type = type;
        }

        /// <summary>
        /// Notifies the observer of a new value in the sequence. It's best to override Dispatch or TreatRow than this method because this method contains pipelining logic.
        /// </summary>
        public override void Trigger()
        {
            CountTreated++;

            try
            {
                IEnumerator fList = null;

                if (_strm != null)
                {
                    using (StreamReader reader = new StreamReader(_strm))
                    {
                        fList = FluentFile.For(_type).From(reader).GetEnumerator();                        
                    }
                }
                else if (_strmReader != null)
                {
                    fList = FluentFile.For(_type).From(_strmReader).GetEnumerator();                    
                }
                else if (_filename != null)
                {
                    fList = FluentFile.For(_type).From(_filename).GetEnumerator();                    
                }
                IterateElements(fList);                
            }
            catch (Exception ex)
            {
                log4net.LogManager.GetLogger(this.GetType()).Error("Operation error", ex);
                Observers.PropagateOnError(ex);
            }

            Completed = true;
            Observers.PropagateOnCompleted();
        }

        private void IterateElements(IEnumerator fList)
        {
            while (fList.MoveNext())
            {
                Observers.PropagateOnNext(Row.FromObject(fList.Current));
            }
        }
    }
}