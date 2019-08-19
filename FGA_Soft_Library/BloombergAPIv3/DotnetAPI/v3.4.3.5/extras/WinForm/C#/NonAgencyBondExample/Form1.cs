/// ==========================================================
/// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT 
///  WARRANTY OF ANY KIND, EITHER EXPRESSED OR IMPLIED,    
/// INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES   
/// OF MERCHANTABILITY AND/OR FITNESS FOR A  PARTICULAR   
/// PURPOSE.								
/// ==========================================================
/// Purpose of this example:
/// - Make asynchronous refdata request
///   request using //blp/refdata service.
/// ==========================================================
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using Event = Bloomberglp.Blpapi.Event;
using Element = Bloomberglp.Blpapi.Element;
using InvalidRequestException = Bloomberglp.Blpapi.InvalidRequestException;
using Message = Bloomberglp.Blpapi.Message;
using Name = Bloomberglp.Blpapi.Name;
using Request = Bloomberglp.Blpapi.Request;
using Service = Bloomberglp.Blpapi.Service;
using Session = Bloomberglp.Blpapi.Session;
using SessionOptions = Bloomberglp.Blpapi.SessionOptions;
using EventQueue = Bloomberglp.Blpapi.EventQueue;
using CorrelationID = Bloomberglp.Blpapi.CorrelationID;
using EventHandler = Bloomberglp.Blpapi.EventHandler;

namespace NonAgencyBondExample
{

    public partial class Form1 : Form
    {
        private static readonly Name SECURITY_DATA = new Name("securityData");
        private static readonly Name SECURITY = new Name("security");
        private static readonly Name FIELD_DATA = new Name("fieldData");
        private static readonly Name RESPONSE_ERROR = new Name("responseError");
        private static readonly Name SECURITY_ERROR = new Name("securityError");
        private static readonly Name FIELD_EXCEPTIONS = new Name("fieldExceptions");
        private static readonly Name FIELD_ID = new Name("fieldId");
        private static readonly Name ERROR_INFO = new Name("errorInfo");
        private static readonly Name CATEGORY = new Name("category");
        private static readonly Name MESSAGE = new Name("message");

        const int NUMBER_OF_SCENARIO = 3;
        private string d_host = string.Empty;
        private int d_port = 0;
        private Session d_session = null;
        private Service d_service = null;
        private Dictionary<string, int> d_scenarioIndexLookup = null;

        public Form1()
        {
            InitializeComponent();
            // init control values
            initialize();
            // start API session
            if (startAPISession())
            {
                // started sucessfully, enable security controls
                labelSecurity.Enabled = true;
                textBoxSecurity.Enabled = true;
                buttonGetData.Enabled = true;
                buttonGetAllData.Enabled = true;
                buttonClearData.Enabled = true;
                buttonClearAllData.Enabled = true;
            }
            else
            {
                // failed
                d_session = null;
            }
        }

        #region "Private functions"
        /// <summary>
        ///  init form controls and variables
        /// </summary>
        private void initialize()
        {
            // variables
            d_host = "localhost";
            d_port = 8194;
            d_scenarioIndexLookup = new Dictionary<string, int>();
            d_scenarioIndexLookup.Add("Scenario 1", 1);
            d_scenarioIndexLookup.Add("Scenario 2", 2);
            d_scenarioIndexLookup.Add("Scenario 3", 3);

            String[] inputFields = new string[] { "MTG_PREPAY_SPEED", "MTG_PREPAY_TYP", 
                "PREPAY_SPEED_VECTOR", "ALLOW_DYNAMIC_CASHFLOW_CALCS", "DEFAULT_PERCENT",
                "DEFAULT_TYPE", "DEFAULT_SPEED_VECTOR", "LOSS_SEVERITY", 
                "RECOVERY_LAG"};

            dataGridViewAnalyticInput.Rows.Add(new object[] { "Prepay Speed", 100, 100, 100 });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Prepay Type", "CPR", "CPR", "CPR" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Prepay Vector", "15", "2 12 R 10", "2" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Dynamics", "Y", "Y", "Y" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Default Speed", 200, 100, 100 });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Default Type", "CDR", "CDR", "CDR" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Default Vector", "2 10 R 8", "2 4 6 8", "6 12 R 2" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Severity Curve", "60 24 R 85", "60 12 R 85", "85 12 R 60" });
            dataGridViewAnalyticInput.Rows.Add(new object[] { "Recovery Lag", 0, 0, 0 });
            // set tag property
            for (int index = 0; index < inputFields.Length; index++)
            {
                dataGridViewAnalyticInput.Rows[index].Tag = inputFields[index];
            }


            // set vary price to skip 5
            comboBoxVaryPrice.SelectedIndex = 2;
            // create sub-items for listview
            foreach (ListViewItem item in listViewScenarios.Items)
            {
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
                item.SubItems.Add(string.Empty);
            }

            try
            {
                // load Vector tab information 
                //[note: Make sure this rtf file is in the same directory as the executable.]
                richTextBoxVectors.LoadFile("vectors.rtf");
            }
            catch 
            {
                richTextBoxVectors.Text = "Missing vectors.rtf file in the executable directory.";
            }
        }

        /// <summary>
        /// Create and start session. 
        /// Open refdata service for reference data requests.
        /// </summary>
        /// <returns></returns>
        private bool startAPISession()
        {
            bool status = false;

            // setup sessionOption
            SessionOptions sessionOptions = new SessionOptions();
            sessionOptions.ServerHost = d_host;
            sessionOptions.ServerPort = d_port;

            // create new session
            d_session = new Session(sessionOptions, new EventHandler(processEvent));
            // start session
            if (!d_session.Start())
            {
                MessageBox.Show("Failed to start session.", "Session");
            }
            else
            {
                // open refdata service
                if (!d_session.OpenService("//blp/refdata"))
                {
                    MessageBox.Show("Failed to open //blp/refdata", "Service");
                }
                else
                {
                    // sucess
                    status = true;
                }
            }
            return status;
        }

        /// <summary>
        /// Clear tab page textbox
        /// </summary>
        /// <param name="tab"></param>
        private void clearTextBox(TabPage tab)
        {
            // loop through Data tab for field textbox to update data
            foreach (Control control in tab.Controls)
            {
                // found GroupBox
                if (control.GetType() == typeof(GroupBox))
                {
                    // Loop through GroupBox control collection
                    foreach (Control child in control.Controls)
                    {
                        // textbox Tag property contain field name
                        if (child.GetType() == typeof(TextBox))
                        {
                            child.Text = "-";
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Clear scenarios data
        /// </summary>
        private void clearScenarios()
        {
            foreach (ListViewItem item in listViewScenarios.Items)
            {
                for (int index = 1; index < listViewScenarios.Columns.Count; index++)
                {
                    item.SubItems[index].Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Populate tab textbox data
        /// </summary>
        /// <param name="tab"></param>
        /// <param name="field"></param>
        private void populateTextBox(TabPage tab, string field, string value)
        {
            bool foundTextbox = false;
            // loop through Data tab for field textbox to update data
            foreach (Control control in tab.Controls)
            {
                // found GroupBox
                if (control.GetType() == typeof(GroupBox))
                {
                    // Loop through GroupBox control collection
                    foreach (Control child in control.Controls)
                    {
                        // textbox Tag property contain field name
                        if (child.Tag != null && child.Tag.ToString().Trim().Length > 0)
                        {
                            // check if textbox control contain correct field
                            if (child.Tag.ToString() == field)
                            {
                                // Populate data
                                child.Text = value;
                                foundTextbox = true;
                                break;
                            }
                        }
                    }
                }
                if (foundTextbox)
                {
                    // no need to loop through controls
                    break;
                }
            }
        }

        /// <summary>
        /// Populate listview control with scenario data
        /// </summary>
        /// <param name="scenario"></param>
        /// <param name="priceIndex"></param>
        /// <param name="returnFields"></param>
        private void populateScenario(string scenario, int priceIndex, Element returnFields)
        {
            int currentPriceIndex = 0;
            if (returnFields.NumElements > 0)
            {
                // get number of fields returned
                int numElements = returnFields.NumElements;
                for (int j = 0; j < numElements; ++j)
                {
                    // reset index
                    currentPriceIndex = -1;
                    // get field
                    Element field = returnFields.GetElement(j);
                    foreach (ListViewItem item in listViewScenarios.Items)
                    {
                        // look for correct price 
                        if (item.Text.Contains("Price"))
                        {
                            // move to next price position
                            currentPriceIndex++;
                        }
                        
                        // check if in correct price slot
                        if (currentPriceIndex < priceIndex)
                        {
                            // continue to search for correct price section
                            continue;
                        }

                        // check if row updateable
                        if (item.Tag != null && item.Tag.ToString().Trim().Length > 0)
                        {
                            // check if correct row
                            if (item.Tag.ToString() == field.Name.ToString())
                            {
                                // populate data
                                int index = d_scenarioIndexLookup[scenario];
                                item.SubItems[index].Text = field.GetValueAsString();
                                break;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculate scenarios average for each price
        /// </summary>
        private void calculateScenariosAverage()
        {
            foreach (ListViewItem item in listViewScenarios.Items)
            {
                // check if field can be average
                if (item.Checked)
                {
                    double value = 0;
                    double sum = 0;
                    // calculate average for this item
                    foreach (KeyValuePair<string, int> scenario in d_scenarioIndexLookup)
                    {
                        double.TryParse(item.SubItems[scenario.Value].Text, out value);
                        sum += value;
                    }
                    // get average
                    sum /= d_scenarioIndexLookup.Count;
                    item.SubItems[4].Text = sum.ToString();
                }
            }
        }

        /// <summary>
        /// Get security data
        /// </summary>
        private void getData()
        {
            // get reference data service
            if (d_service == null)
            {
                d_service = d_session.GetService("//blp/refdata");
            }
            // create reference data request
            Request request = d_service.CreateRequest("ReferenceDataRequest");
            // set security
            request.GetElement("securities").AppendValue(textBoxSecurity.Text.ToUpper().Trim() + " Mtge");
            // set fields
            Element fields = request.GetElement("fields");
            fields.AppendValue("MTG_PL_CPR_1M");
            fields.AppendValue("MTG_CDR_1M");
            fields.AppendValue("MTG_VPR_1M");
            fields.AppendValue("MTG_SEV_1M");
            fields.AppendValue("CREDIT_MODELED_INDICATOR");
            fields.AppendValue("MTG_AMT_OUT_FACE");
            fields.AppendValue("MTG_FACTOR");
            fields.AppendValue("CPN");
            fields.AppendValue("MTG_TRANCHE_TYP_LONG");
            fields.AppendValue("RTG_SP_INITIAL");
            fields.AppendValue("RTG_SP");
            fields.AppendValue("RTG_MDY_INITIAL");
            fields.AppendValue("RTG_MOODY");
            fields.AppendValue("ORIG_CREDIT_SUPPORT");
            fields.AppendValue("CURR_CREDIT_SUPPORT");
            fields.AppendValue("CREDIT_SUPPORT_COVERAGE");
            fields.AppendValue("COLLAT_TYP");
            fields.AppendValue("MTG_POOL_FACTOR");
            fields.AppendValue("MTG_NUM_POOLS");
            fields.AppendValue("MTG_WACPN");
            fields.AppendValue("MTG_WHLN_WALA");
            fields.AppendValue("MTG_WHLN_30DLQ");
            fields.AppendValue("MTG_WHLN_60DLQ");
            fields.AppendValue("MTG_WHLN_90DLQ");
            fields.AppendValue("BANKRUPT_PCT");
            fields.AppendValue("MTG_WHLN_FCLS");
            fields.AppendValue("MTG_WHLN_REO");
            fields.AppendValue("MTG_DELQ_60PLUS_CUR");
            fields.AppendValue("MTG_DELQ_90PLUS_CUR");
            fields.AppendValue("CURR_CUM_LOSS_AMT");
            
            // create event queue for synchronus request
            EventQueue eventQueue = new EventQueue();
            // send request
            d_session.SendRequest(request, eventQueue, null);
            // wait for data to come back
            while (true)
            {
                Event eventObj = eventQueue.NextEvent();
                // process data
                foreach (Message msg in eventObj)
                {
                    // check for request error
                    if (msg.HasElement(RESPONSE_ERROR))
                    {
                        MessageBox.Show("REQUEST FAILED: " + 
                            msg.GetElement(RESPONSE_ERROR).ToString(), "Request Error");
                        continue;
                    }

                    // get securities
                    Element securities = msg.GetElement(SECURITY_DATA);
                    int numSecurities = securities.NumValues;
                    for (int i = 0; i < numSecurities; ++i)
                    {
                        // get security
                        Element security = securities.GetValueAsElement(i);
                        string ticker = security.GetElementAsString(SECURITY);
                        // check for security error
                        if (security.HasElement("securityError"))
                        {
                            MessageBox.Show("SECURITY FAILED: " +
                                security.GetElement(SECURITY_ERROR).ToString(), "Security Error");
                            continue;
                        }
                        // get fields
                        Element returnFields = security.GetElement(FIELD_DATA);
                        if (returnFields.NumElements > 0)
                        {
                            // get number of fields returned
                            int numElements = returnFields.NumElements;
                            for (int j = 0; j < numElements; ++j)
                            {
                                // get field
                                Element field = returnFields.GetElement(j);
                                // populate textbox on Data tab
                                populateTextBox(tabPageData, field.Name.ToString(), field.GetValueAsString());
                            }
                        }
                    }
                }
                
                if (eventObj.Type == Event.EventType.RESPONSE)
                {
                    // all the data came back for this request
                    break;
                }
            }
        }

        /// <summary>
        /// Get analytic data for prices and scenarios
        /// </summary>
        private void getAnalyticData()
        {
            double price1 = 0;
            double price2 = 0;
            double price3 = 0;
            double[] prices;
            string overridePriceYieldField = string.Empty;
            string returnPriceYieldField = string.Empty;

            // get reference data service
            if (d_service == null)
            {
                d_service = d_session.GetService("//blp/refdata");
            }

            // get prices
            double.TryParse(textBoxPrice1.Text, out price1);
            double.TryParse(textBoxPrice2.Text, out price2);
            double.TryParse(textBoxPrice3.Text, out price3);
            prices = new double[] { price1, price2, price3 };

            // set to either price or yield override
            if (radioButtonPrice.Checked)
            {
                // price
                overridePriceYieldField = radioButtonPrice.Tag.ToString();
                returnPriceYieldField = radioButtonYield.Tag.ToString();
            }
            else
            {
                // yield
                overridePriceYieldField = radioButtonYield.Tag.ToString();
                returnPriceYieldField = radioButtonPrice.Tag.ToString(); 
            }

            // run scenarios for each price
            int priceIndex = 0;
            long correlationId = 0;
            foreach (double price in prices)
            {
                // max 3 scenario for each price
                for (int scenario = 1; scenario <= NUMBER_OF_SCENARIO; scenario++)
                {
                    // create reference data request
                    Request request = d_service.CreateRequest("ReferenceDataRequest");
                    // set security
                    request.GetElement("securities").AppendValue(textBoxSecurity.Text.ToUpper().Trim() + " Mtge");
                    // set fields
                    Element fields = request.GetElement("fields");
                    fields.AppendValue(returnPriceYieldField);
                    fields.AppendValue("MTG_WAL");
                    fields.AppendValue("MTG_STATIC_MOD_DUR");
                    fields.AppendValue("MTG_PRINC_WIN");
                    fields.AppendValue("I_SPRD_ASK");
                    fields.AppendValue("Z_SPRD_ASK");
                    fields.AppendValue("E_SPRD_ASK");
                    fields.AppendValue("FIRST_LOSS_DATE");
                    fields.AppendValue("PROJ_BOND_CUM_LOSS_AMT");
                    fields.AppendValue("PROJ_COLL_CUM_LOSS_AMT");
                    fields.AppendValue("PROJ_BOND_CUM_LOSS_PCT");
                    fields.AppendValue("PROJ_COLL_CUM_LOSS_PCT");
                    fields.AppendValue("PROJ_BOND_WRITEDWN_PCT_CURR_FACE");
                    fields.AppendValue("PROJ_COLL_WRITEDWN_PCT_CURR_FACE");

                    // set overrides
                    Element overrides = request["overrides"];
                    Element overrideField = null;
                    // override field
                    overrideField = overrides.AppendElement();
                    // set fieldId 
                    overrideField.SetElement("fieldId", overridePriceYieldField);
                    // set override value
                    overrideField.SetElement("value", price.ToString());

                    // get scenario info
                    foreach (DataGridViewRow row in dataGridViewAnalyticInput.Rows)
                    {
                        // field name is in row tag property
                        if (row.Tag != null && row.Tag.ToString().Trim().Length > 0)
                        {
                            string overrideValue = string.Empty;
                            // textbox by default
                            overrideValue = row.Cells[scenario].Value.ToString();

                            // override field
                            overrideField = overrides.AppendElement();
                            // set fieldId 
                            overrideField.SetElement("fieldId", row.Tag.ToString());
                            // set override value
                            overrideField.SetElement("value", overrideValue);
                        }
                    }

                    // make asynchronous request, data will come back in processEvent()
                    // correlationId is use to map data to listview
                    d_session.SendRequest(request,  new CorrelationID(correlationId));
                    correlationId++;
                }
                // point to next price
                priceIndex++;
            }
        }

        /// <summary>
        /// Process data and populate to listview
        /// </summary>
        /// <param name="eventObj"></param>
        private void processData(Event eventObj)
        {
            // process data
            foreach (Message msg in eventObj)
            {
                // get correlation id
                int scenario = (int)msg.CorrelationID.Value % NUMBER_OF_SCENARIO + 1;
                int priceIndex = (int)msg.CorrelationID.Value / NUMBER_OF_SCENARIO;

                // check for request error
                if (msg.HasElement(RESPONSE_ERROR))
                {
                    MessageBox.Show("REQUEST FAILED: " +
                        msg.GetElement(RESPONSE_ERROR).ToString(), "Request Error");
                    continue;
                }

                // get securities
                Element securities = msg.GetElement(SECURITY_DATA);
                int numSecurities = securities.NumValues;
                for (int i = 0; i < numSecurities; ++i)
                {
                    // get security
                    Element security = securities.GetValueAsElement(i);
                    string ticker = security.GetElementAsString(SECURITY);
                    // check for security error
                    if (security.HasElement("securityError"))
                    {
                        MessageBox.Show("SECURITY FAILED: " +
                            security.GetElement(SECURITY_ERROR).ToString(), "Security Error");
                        continue;
                    }
                    // get fields
                    Element returnFields = security.GetElement(FIELD_DATA);

                    // populate scenario fields on Analytic tab
                    populateScenario(dataGridViewAnalyticInput.Columns[scenario].HeaderText, priceIndex, returnFields);
                }
            }
        }
        #endregion

        #region "Events"
        /// <summary>
        /// Asynchronous request data event
        /// </summary>
        /// <param name="eventObj"></param>
        /// <param name="session"></param>
        private void processEvent(Event eventObj, Session session)
        {
            if (InvokeRequired)
            {
                // make sure data get process in winform thread
                Invoke(new EventHandler(processEvent), new object[] { eventObj, session });
            }
            else
            {
                try
                {
                    switch (eventObj.Type)
                    {
                        case Event.EventType.PARTIAL_RESPONSE:
                            // process partial data
                            processData(eventObj);
                            break;
                        case Event.EventType.RESPONSE:
                            // preocess return data
                            processData(eventObj);
                            // calculate average
                            calculateScenariosAverage();
                            break;
                    }
                }
                catch (System.Exception e)
                {
                    MessageBox.Show("Exception: " + e.Message, "Populate Data Exception");
                }
            }
        }

        /// <summary>
        /// Vary price for scenarios
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void comboBoxVaryPrice_SelectedIndexChanged(object sender, EventArgs e)
        {
            int varyPrice = 0;
            double price1 = 0;

            // get vary price
            int.TryParse(comboBoxVaryPrice.SelectedItem.ToString(), out varyPrice);
            // get scenario 1 price
            double.TryParse(textBoxPrice1.Text, out price1);
            if (varyPrice > 0)
            {
                // vary price for scenario 2 and 3
                price1 += varyPrice;
                textBoxPrice2.Text = price1.ToString();
                price1 += varyPrice;
                textBoxPrice3.Text = price1.ToString();
            }
        }

        /// <summary>
        /// Only allow floating numeric value
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxPrice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsNumber(e.KeyChar) & (Keys)e.KeyChar != Keys.Back)
            {
                if (e.KeyChar == '.')
                {
                    TextBox price = (TextBox)sender;
                    if (price.Text.Contains("."))
                    {
                        e.Handled = true;
                    }
                }
                else
                {
                    e.Handled = true;
                }
            } 
        }

        /// <summary>
        /// Stop session on form closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (d_session != null)
            {
                d_session.Stop();
                d_session = null;
            }
        }

        /// <summary>
        /// Get data for selected tab
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetData_Click(object sender, EventArgs e)
        {
            if (textBoxSecurity.Text.Trim().Length == 0)
            {
                // missing security
                MessageBox.Show("Please enter a security", "Security Input");
                return;
            }
            // change cursor to hourglass
            Cursor.Current = Cursors.WaitCursor;
            switch (tabControlNonAgency.SelectedTab.Text)
            {
                case "Data":
                    // clear data
                    clearTextBox(tabPageData);
                    // request for data
                    getData();
                    break;
                default: // Analytic
                    // clear scenarios
                    clearScenarios();
                    // show data tab
                    tabControlNonAgency.SelectedTab = tabPageAnalyic;
                    // request for analytic data
                    getAnalyticData();
                    break;
            }
            // change cursor to normal
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// get both Data and Analytic data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGetAllData_Click(object sender, EventArgs e)
        {
            if (textBoxSecurity.Text.Trim().Length == 0)
            {
                // missing security
                MessageBox.Show("Please enter a security", "Security Input");
                return;
            }
            // change cursor to hourglass
            Cursor.Current = Cursors.WaitCursor;
            // clear data
            clearTextBox(tabPageData);
            // request for data
            getData();
            // clear scenarios
            clearScenarios();
            // request for analytic data
            getAnalyticData();
            // calculate average
            calculateScenariosAverage();
            // change cursor to normal
            Cursor.Current = Cursors.Default;
        }

        /// <summary>
        /// Clear tab data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearData_Click(object sender, EventArgs e)
        {
            switch (tabControlNonAgency.SelectedTab.Text)
            {
                case "Data":
                    // clear data
                    clearTextBox(tabPageData);
                    break;
                default: // Analytic
                    // clear scenarios
                    clearScenarios();
                    break;
            }

        }

        /// <summary>
        /// Clear all data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonClearAllData_Click(object sender, EventArgs e)
        {
            // clear data
            clearTextBox(tabPageData);
            // clear scenarios
            clearScenarios();
        }


        /// <summary>
        /// Drag and drop content to textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_DragDrop(object sender, DragEventArgs e)
        {
            // Get the entire text object that has been dropped on us.
            string tmp = e.Data.GetData(DataFormats.Text).ToString();
            // cast to textbox control
            TextBox control = (TextBox)sender;
            // set text property
            control.Text = tmp.Trim();

        }

        /// <summary>
        /// Drag content over textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.Text))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// Change override between price and yield
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            string displayName = string.Empty;
            string fieldName = string.Empty;
            string oldDisplayName = string.Empty;
            string oldFieldName = string.Empty;
            if (radioButtonPrice.Checked)
            {
                // change display text to price
                displayName = radioButtonPrice.Text;
                fieldName = radioButtonPrice.Tag.ToString();
                oldDisplayName = radioButtonYield.Text;
                oldFieldName = radioButtonYield.Tag.ToString();
            }
            else
            {
                // change display text to yield
                displayName = radioButtonYield.Text; 
                fieldName = radioButtonYield.Tag.ToString();
                oldDisplayName = radioButtonPrice.Text;
                oldFieldName = radioButtonPrice.Tag.ToString();
            }

            // change lable name
            labelVaryPrice.Text = labelVaryPrice.Text.Replace(oldDisplayName, displayName);
            labelPrice1.Text = labelPrice1.Text.Replace(oldDisplayName, displayName);
            labelPrice2.Text = labelPrice2.Text.Replace(oldDisplayName, displayName);
            labelPrice3.Text = labelPrice3.Text.Replace(oldDisplayName, displayName);
            // update listview text and fields
            foreach (ListViewItem item in listViewScenarios.Items)
            {
                if (item.Text == displayName) 
                {
                    item.Text = oldDisplayName;
                    item.Tag = oldFieldName;
                }
                else
                {
                    if (item.BackColor == Color.LightBlue)
                    {
                        item.Text = item.Text.Replace(oldDisplayName, displayName);
                    }
                }
            }
            // clear scenarios
            clearScenarios();
        }

        /// <summary>
        /// Convert alpha to upper case
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridViewAnalyticInput_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // make sure all alpha are in upper case
            dataGridViewAnalyticInput.Rows[e.RowIndex].Cells[e.ColumnIndex].Value =
                dataGridViewAnalyticInput.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString().ToUpper();
        }
        #endregion
    }
}