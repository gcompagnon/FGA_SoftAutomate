#region "  � Copyright 2005-07 to Marcos Meli - http://www.devoo.net"

// Errors, suggestions, contributions, send a mail to: marcos@filehelpers.com.

#endregion

#undef GENERICS
//#define GENERICS
//#if NET_2_0

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Diagnostics;
#if NET_2_0
using System.Collections.Generic;
#endif

namespace FileHelpers.MasterDetail
{

    /// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngine/*'/>
    /// <include file='Examples.xml' path='doc/examples/MasterDetailEngine/*'/>
#if ! GENERICS
	public sealed class MasterDetailEngine : EngineBase
	{
		
    #region "  Constructor  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr1/*'/>
        public MasterDetailEngine(Type masterType, Type detailType)
            : this(masterType, detailType, null)
        {
        }

		/// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr1/*'/>
		public MasterDetailEngine(Type masterType, Type detailType, MasterDetailSelector recordSelector) : base(detailType)
		{
			mMasterType = masterType;
			mMasterInfo = new RecordInfo(masterType);
			RecordSelector = recordSelector;
		}
 
		/// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr2/*'/>
		public MasterDetailEngine(Type masterType, Type detailType, CommonSelector action, string selector)
			: base(detailType)
		{
			mMasterInfo = new RecordInfo(masterType);

			CommonSelectorInternal sel = new CommonSelectorInternal(action, selector, mMasterInfo.mIgnoreEmptyLines || mRecordInfo.mIgnoreEmptyLines);
			mRecordSelector = new MasterDetailSelector(sel.CommonSelectorMethod);
		}

        #endregion
	
#else
    /// <typeparam name="M">The Master Record Type</typeparam>
    /// <typeparam name="D">The Detail Record Type</typeparam>
    public sealed class MasterDetailEngine<M, D> : EngineBase
        where M : class
        where D : class
    {

        #region "  Constructor  "

                /// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr1/*'/>
        public MasterDetailEngine()
            : this(null)
        {
        }


        /// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr1/*'/>
        public MasterDetailEngine(MasterDetailSelector recordSelector)
            : base(typeof(D))
        {
            mMasterType = typeof(M);
            mMasterInfo = new RecordInfo(mMasterType);
            mRecordSelector = recordSelector;
        }

        /// <include file='MasterDetailEngine.docs.xml' path='doc/MasterDetailEngineCtr2/*'/>
        public MasterDetailEngine(CommonSelector action, string selector)
            : base(typeof(D))
        {
            mMasterType = typeof(M);
            mMasterInfo = new RecordInfo(mMasterType);

            MasterDetailEngine.CommonSelectorInternal sel = new MasterDetailEngine.CommonSelectorInternal(action, selector, mMasterInfo.mIgnoreEmptyLines || mRecordInfo.mIgnoreEmptyLines);
            mRecordSelector = new MasterDetailSelector(sel.CommonSelectorMethod);
        }

        #endregion

#endif

        #region CommonSelectorInternal

#if ! GENERICS
        internal class CommonSelectorInternal
		{
            readonly CommonSelector mAction;
            readonly string mSelector;
            readonly bool mIgnoreEmpty = false;


			internal CommonSelectorInternal(CommonSelector action, string selector, bool ignoreEmpty)
			{
				mAction = action;
				mSelector = selector;
				mIgnoreEmpty = ignoreEmpty;
			}

			internal RecordAction CommonSelectorMethod(string recordString)
			{
				if (mIgnoreEmpty && recordString == string.Empty)
					return RecordAction.Skip;

				switch(mAction)
				{
					case CommonSelector.DetailIfContains:
						if (recordString.IndexOf(mSelector) >= 0)
							return RecordAction.Detail;
						else
							return RecordAction.Master;

					case CommonSelector.MasterIfContains:
						if (recordString.IndexOf(mSelector) >= 0)
							return RecordAction.Master;
						else
							return RecordAction.Detail;

					case CommonSelector.DetailIfBegins:
						if (recordString.StartsWith(mSelector))
							return RecordAction.Detail;
						else
							return RecordAction.Master;
					
					case CommonSelector.MasterIfBegins:
						if (recordString.StartsWith(mSelector))
							return RecordAction.Master;
						else
							return RecordAction.Detail;

					case CommonSelector.DetailIfEnds:
						if (recordString.EndsWith(mSelector))
							return RecordAction.Detail;
						else
							return RecordAction.Master;

					case CommonSelector.MasterIfEnds:
						if (recordString.EndsWith(mSelector))
							return RecordAction.Master;
						else
							return RecordAction.Detail;

					case CommonSelector.DetailIfEnclosed:
						if (recordString.StartsWith(mSelector) && recordString.EndsWith(mSelector))
							return RecordAction.Detail;
						else
							return RecordAction.Master;

					case CommonSelector.MasterIfEnclosed:
						if (recordString.StartsWith(mSelector) && recordString.EndsWith(mSelector))
							return RecordAction.Master;
						else
							return RecordAction.Detail;

				}

				return RecordAction.Skip;
			}
		}
#endif
        #endregion

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private readonly RecordInfo mMasterInfo;
#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private MasterDetailSelector mRecordSelector;

#if NET_2_0
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
#endif
        private readonly Type mMasterType;

        /// <summary>Returns the type of the master records handled by this engine.</summary>
        public Type MasterType
        {
            get { return mMasterType; }
        }

        /// <summary>
        /// The <see cref="MasterDetailSelector" /> to get the <see cref="RecordAction" /> (only for read operations)
        /// </summary>
	    public MasterDetailSelector RecordSelector
	    {
	        get { return mRecordSelector; }
	        set { mRecordSelector = value; }
	    }

	    #region "  ReadFile  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/ReadFile/*'/>
#if ! GENERICS
		public MasterDetails[] ReadFile(string fileName)
		{
			using (StreamReader fs = new StreamReader(fileName, mEncoding, true))
			{
				MasterDetails[] tempRes;
				tempRes = ReadStream(fs);
				fs.Close();

				return tempRes;
			}

		}
#else
        public MasterDetails<M, D>[] ReadFile(string fileName)
        {
            using (StreamReader fs = new StreamReader(fileName, mEncoding, true))
            {
                MasterDetails<M, D>[] tempRes;
                tempRes = ReadStream(fs);
                fs.Close();

                return tempRes;
            }

        }
#endif

        #endregion

        #region "  ReadStream  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/ReadStream/*'/>
#if ! GENERICS
		public MasterDetails[] ReadStream(TextReader reader)
#else
        public MasterDetails<M, D>[] ReadStream(TextReader reader)
#endif
        {
            if (reader == null)
                throw new ArgumentNullException("reader", "The reader of the Stream can't be null");

            if (RecordSelector == null)
                throw new BadUsageException("The RecordSelector can't be null on read operations.");

            NewLineDelimitedRecordReader recordReader = new NewLineDelimitedRecordReader(reader);

            ResetFields();
            mHeaderText = String.Empty;
            mFooterText = String.Empty;

            ArrayList resArray = new ArrayList();

            using (ForwardReader freader = new ForwardReader(recordReader, mMasterInfo.mIgnoreLast))
            {
                freader.DiscardForward = true;

                string currentLine, completeLine;

                mLineNumber = 1;

                completeLine = freader.ReadNextLine();
                currentLine = completeLine;

#if !MINI
                ProgressHelper.Notify(mNotifyHandler, mProgressMode, 0, -1);
#endif
                int currentRecord = 0;

                if (mMasterInfo.mIgnoreFirst > 0)
                {
                    for (int i = 0; i < mMasterInfo.mIgnoreFirst && currentLine != null; i++)
                    {
                        mHeaderText += currentLine + StringHelper.NewLine;
                        currentLine = freader.ReadNextLine();
                        mLineNumber++;
                    }
                }


                bool byPass = false;

#if ! GENERICS
                MasterDetails record = null;
#else
            MasterDetails<M, D> record = null;
#endif
                ArrayList tmpDetails = new ArrayList();

                LineInfo line = new LineInfo(currentLine);
                line.mReader = freader;


                object[] valuesMaster = new object[mMasterInfo.mFieldCount];
                object[] valuesDetail = new object[mRecordInfo.mFieldCount];

                while (currentLine != null)
                {
                    try
                    {
                        currentRecord++;

                        line.ReLoad(currentLine);

#if !MINI
                        ProgressHelper.Notify(mNotifyHandler, mProgressMode, currentRecord, -1);
#endif

                        RecordAction action = RecordSelector(currentLine);

                        switch (action)
                        {
                            case RecordAction.Master:
                                if (record != null)
                                {
#if ! GENERICS
                                    record.mDetails = tmpDetails.ToArray();
#else
                                record.mDetails = (D[])tmpDetails.ToArray();
#endif
                                    resArray.Add(record);
                                }

                                mTotalRecords++;
#if ! GENERICS
                                record = new MasterDetails();
#else
                            record = new MasterDetails<M, D>();
#endif
                                tmpDetails.Clear();
#if ! GENERICS
                                object lastMaster = mMasterInfo.StringToRecord(line, valuesMaster);
#else
                            M lastMaster = (M)mMasterInfo.StringToRecord(line, valuesMaster);
#endif

                                if (lastMaster != null)
                                    record.mMaster = lastMaster;

                                break;

                            case RecordAction.Detail:
#if ! GENERICS
                                object lastChild = mRecordInfo.StringToRecord(line, valuesDetail);
#else
                            D lastChild = (D)mRecordInfo.StringToRecord(line, valuesDetail);
#endif

                                if (lastChild != null)
                                    tmpDetails.Add(lastChild);
                                break;

                            default:
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        switch (mErrorManager.ErrorMode)
                        {
                            case ErrorMode.ThrowException:
                                byPass = true;
                                throw;
                            case ErrorMode.IgnoreAndContinue:
                                break;
                            case ErrorMode.SaveAndContinue:
                                ErrorInfo err = new ErrorInfo();
                                err.mLineNumber = mLineNumber;
                                err.mExceptionInfo = ex;
                                //							err.mColumnNumber = mColumnNum;
                                err.mRecordString = completeLine;

                                mErrorManager.AddError(err);
                                break;
                        }
                    }
                    finally
                    {
                        if (byPass == false)
                        {
                            currentLine = freader.ReadNextLine();
                            completeLine = currentLine;
                            mLineNumber = freader.LineNumber;
                        }
                    }
                }

                if (record != null)
                {
#if ! GENERICS
                    record.mDetails = tmpDetails.ToArray();
#else
                record.mDetails = (D[])tmpDetails.ToArray();
#endif
                    resArray.Add(record);
                }

                if (mMasterInfo.mIgnoreLast > 0)
                {
                    mFooterText = freader.RemainingText;
                }
            }
#if ! GENERICS
			return (MasterDetails[]) resArray.ToArray(typeof (MasterDetails));
#else
            return (MasterDetails<M, D>[])resArray.ToArray(typeof(MasterDetails<M, D>));
#endif
        }

        #endregion

        #region "  ReadString  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/ReadString/*'/>
#if ! GENERICS
		public MasterDetails[] ReadString(string source)
		{
			StringReader reader = new StringReader(source);
			MasterDetails[] res = ReadStream(reader);
			reader.Close();
			return res;
		}
#else
        public MasterDetails<M, D>[] ReadString(string source)
        {
            StringReader reader = new StringReader(source);
            MasterDetails<M, D>[] res = ReadStream(reader);
            reader.Close();
            return res;
        }
#endif


        #endregion

        #region "  WriteFile  "

#if ! GENERICS
				/// <include file='MasterDetailEngine.docs.xml' path='doc/WriteFile/*'/>
				public void WriteFile(string fileName, MasterDetails[] records)
				{
					WriteFile(fileName, records, -1);
				}

				/// <include file='MasterDetailEngine.docs.xml' path='doc/WriteFile2/*'/>
				public void WriteFile(string fileName, MasterDetails[] records, int maxRecords)
				{
					using (StreamWriter fs = new StreamWriter(fileName, false, mEncoding))
					{
						WriteStream(fs, records, maxRecords);
						fs.Close();
					}

				}
#else
        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteFile/*'/>
        public void WriteFile(string fileName, IEnumerable<MasterDetails<M, D>> records)
        {
            WriteFile(fileName, records, -1);
        }

        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteFile2/*'/>
        public void WriteFile(string fileName, IEnumerable<MasterDetails<M, D>> records, int maxRecords)
        {
            using (StreamWriter fs = new StreamWriter(fileName, false, mEncoding))
            {
                WriteStream(fs, records, maxRecords);
                fs.Close();
            }

        }
#endif


        #endregion

        #region "  WriteStream  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteStream/*'/>
#if ! GENERICS
		public void WriteStream(TextWriter writer, MasterDetails[] records)
#else
        public void WriteStream(TextWriter writer, IEnumerable<MasterDetails<M, D>> records)
#endif
        {
            WriteStream(writer, records, -1);
        }

        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteStream2/*'/>
#if ! GENERICS
		public void WriteStream(TextWriter writer, MasterDetails[] records, int maxRecords)
#else
        public void WriteStream(TextWriter writer, IEnumerable<MasterDetails<M, D>> records, int maxRecords)
#endif
        {
            if (writer == null)
                throw new ArgumentNullException("writer", "The writer of the Stream can be null");

            if (records == null)
                throw new ArgumentNullException("records", "The records can be null. Try with an empty array.");

            ResetFields();

            if (mHeaderText != null && mHeaderText.Length != 0)
                if (mHeaderText.EndsWith(StringHelper.NewLine))
                    writer.Write(mHeaderText);
                else
                    writer.WriteLine(mHeaderText);


            string currentLine = null;

            int max = maxRecords;
            if (records is IList)
                max = Math.Min(max < 0 ? int.MaxValue : max, ((IList)records).Count);

#if !MINI
            ProgressHelper.Notify(mNotifyHandler, mProgressMode, 0, max);
#endif

            int recIndex = 0;

#if ! GENERICS
			foreach(MasterDetails rec in records)
#else
            foreach (MasterDetails<M, D> rec in records)
#endif
            {
                if (recIndex == maxRecords)
                    break;

                try
                {
                    if (rec == null)
                        throw new BadUsageException("The record at index " + recIndex.ToString() + " is null.");

#if !MINI
                    ProgressHelper.Notify(mNotifyHandler, mProgressMode, recIndex + 1, max);
#endif

                    currentLine = mMasterInfo.RecordToString(rec.mMaster);
                    writer.WriteLine(currentLine);

                    if (rec.mDetails != null)
                        for (int d = 0; d < rec.mDetails.Length; d++)
                        {
                            currentLine = mRecordInfo.RecordToString(rec.mDetails[d]);
                            writer.WriteLine(currentLine);
                        }
                }
                catch (Exception ex)
                {
                    switch (mErrorManager.ErrorMode)
                    {
                        case ErrorMode.ThrowException:
                            throw;
                        case ErrorMode.IgnoreAndContinue:
                            break;
                        case ErrorMode.SaveAndContinue:
                            ErrorInfo err = new ErrorInfo();
                            err.mLineNumber = mLineNumber;
                            err.mExceptionInfo = ex;
                            //							err.mColumnNumber = mColumnNum;
                            err.mRecordString = currentLine;
                            mErrorManager.AddError(err);
                            break;
                    }
                }

            }

            mTotalRecords = recIndex;

            if (mFooterText != null && mFooterText != string.Empty)
                if (mFooterText.EndsWith(StringHelper.NewLine))
                    writer.Write(mFooterText);
                else
                    writer.WriteLine(mFooterText);

        }

        #endregion

        #region "  WriteString  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteString/*'/>
#if ! GENERICS
		public string WriteString(MasterDetails[] records)
#else
        public string WriteString(IEnumerable<MasterDetails<M, D>> records)
#endif
        {
            return WriteString(records, -1);
        }

        /// <include file='MasterDetailEngine.docs.xml' path='doc/WriteString2/*'/>
#if ! GENERICS
		public string WriteString(MasterDetails[] records, int maxRecords)
#else
        public string WriteString(IEnumerable<MasterDetails<M, D>> records, int maxRecords)
#endif
        {
            StringBuilder sb = new StringBuilder();
            StringWriter writer = new StringWriter(sb);
            WriteStream(writer, records, maxRecords);
            string res = writer.ToString();
            writer.Close();
            return res;
        }

        #endregion

        #region "  AppendToFile  "

        /// <include file='MasterDetailEngine.docs.xml' path='doc/AppendToFile1/*'/>
#if ! GENERICS
		public void AppendToFile(string fileName, MasterDetails record)
		{
			AppendToFile(fileName, new MasterDetails[] {record});
		}
#else
        public void AppendToFile(string fileName, MasterDetails<M, D> record)
        {
            AppendToFile(fileName, new MasterDetails<M, D>[] { record });
        }
#endif

        /// <include file='MasterDetailEngine.docs.xml' path='doc/AppendToFile2/*'/>
#if ! GENERICS
		public void AppendToFile(string fileName, MasterDetails[] records)
#else
        public void AppendToFile(string fileName, IEnumerable<MasterDetails<M, D>> records)
#endif
        {
            using (TextWriter writer = StreamHelper.CreateFileAppender(fileName, mEncoding, true, false))
            {
                mHeaderText = String.Empty;
                mFooterText = String.Empty;

                WriteStream(writer, records);
                writer.Close();
            }

        }

        #endregion
    }
}

//#endif
