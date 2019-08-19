using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using Microsoft.Win32;
using System.Globalization;
using log4net.Config;
using log4net;
using System.IO;
using System.Xml.Linq;
using FGA.Automate.Dataconverter;
using System.Xml;

namespace FGA.Automate.Core
{
    /// <summary>
    /// Exploitation du fichier de configuration, des variables d environnement, et de la ligne de commande
    /// et de la configuration de la plateforme: version de l'OS et du framework .NET
    /// </summary>
    class IntegrationProperties
    {

    /** name of the property giving the path to the property file */
    public const String PROPERTIES_FILE = "msd.integrator.properties.file";
    /** Name of the beginning of the prop giving Producer class name */
    public const String PRODUCER = "msd.integrator.producer.";
    public const String CONSUMER = "msd.integrator.consumer.";
    public const String DATACONVERTER = ".dataconverter.";
    public const String INSTALLED = ".installed";
    public const String YES = "yes";
    public const String NO = "no";

    public const String NB_MAX_THREADS = "msd.integrator.nbMaxThreads"; 
    public const int NB_MAX_THREADS_DEFAULT_VALUE = 2;

    private int nbMaxThreads;

    private const String PROPERTIES_FILE_MSG = "No path to the properties file. Property {0} must be set";
    private const String PROP_FILE_NOT_FOUND_MSG = "The properties file is not found in path : {0}. with the name {1}";
    private const String ON_MAX_THREADS_MSG = "Value of the Property {0} must be set with an int value and >=2. It will be set to this default value {1}";
    private const String CLASS_NOT_FOUND_MSG = "Class not found {0} in the classpath";	
    private const String CLASS_NOT_IMPLEMENT_BC_MSG = "Class '{0}' does not extend '{1}' class";
    private const String CLASS_NOT_IMPLEMENT_CONSTRUCTOR_MSG= "Class '{0}' does not implement a empty constructor";
    private const String CLASS_CONSTRUCTOR_NOT_PUBLIC_MSG= "Class '{0}' does not implement a empty constructor with public access";
    
    public static ILog ExceptionLogger
    {
        get { return LogManager.GetLogger("FGA_Soft_ERREUR"); }
    }


    /// <summary>
    /// retourne la version de la plateforme NET
    /// </summary>
    /// <param name="engine"></param>
    public static String getNETFrameworkVersion(){
        RegistryKey installed_versions = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\NET Framework Setup\NDP");
        string[] version_names = installed_versions.GetSubKeyNames();
        //version names start with 'v', eg, 'v3.5' which needs to be trimmed off before conversion
        double Framework = Convert.ToDouble(version_names[version_names.Length - 1].Remove(0, 1), CultureInfo.InvariantCulture);
        int SP = Convert.ToInt32(installed_versions.OpenSubKey(version_names[version_names.Length - 1]).GetValue("SP", 0));
        return String.Concat( Framework," ", SP);
    }

    /**
     * Configuration load.
     * @param instance of the engine that need the Producers and Consumers config
     * 
     */	
    public void loadProperty(CoreServiceEngine engine){

        IList<String> producersName = new List<String>(5);
        IList<bool> producersInstalled = new List<bool>(5);
        // List<String>    	
        IList<String> consumersName = new List<String>(5);
        // List<Boolean>    	
        IList<bool> consumersInstalled = new List<bool>(5);

        // key : name of the producer or consumer. value ArrayList<String> of the dataconverter names
        IDictionary<String,List<String>> dataconverters = new Dictionary<String,List<String>>();    	

        // looking for the properties file
        String propertiesFilePath = Environment.GetEnvironmentVariable(PROPERTIES_FILE);
        if(propertiesFilePath == null){ // fatal no property file
            ExceptionLogger.FatalFormat(PROPERTIES_FILE_MSG,new Object[]{PROPERTIES_FILE});
            throw new FileNotFoundException(PROPERTIES_FILE_MSG,PROPERTIES_FILE);
        }
        FileStream fs = null;
        XmlReader xmlStream = null;
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.ConformanceLevel = ConformanceLevel.Fragment;
        settings.IgnoreWhitespace = true;
        settings.IgnoreComments = true;
        try{
            
            
            fs = File.OpenRead(propertiesFilePath);
            xmlStream = XmlReader.Create(fs,settings);
        }catch(FileNotFoundException fnfe){
            if( fs != null){
                ExceptionLogger.FatalFormat(PROP_FILE_NOT_FOUND_MSG,new Object[]{propertiesFilePath , fs.Name});
            }
            throw fnfe;
        }

        // Exploiter le stream fs pour lire le XML en LINQ
        XElement xmlConfig = XElement.Load(xmlStream,LoadOptions.PreserveWhitespace);

        // optional property for the thread pool size
        String nbMaxThreadsString = null;
        foreach( string value in xmlConfig.Elements().Where(a => a.Name == NB_MAX_THREADS))
        {
            nbMaxThreadsString = value;
        }
        
        bool result = int.TryParse(nbMaxThreadsString, out nbMaxThreads);
        if( !result)
            nbMaxThreads = NB_MAX_THREADS_DEFAULT_VALUE;


        // Load the config concerning consumers

        foreach( XElement e in xmlConfig.Elements() )
        {
            
        }



        //// Load the config concerning consumers
        //this.loadBindingComponents(properties,CONSUMER, consumersName, consumersInstalled, dataconverters);
        //// instanciate the array for consumers
        //Consumer[] consumers = new Consumer[consumersName.size()];		
        //this.instanciateBindingComponents(consumersName, consumersInstalled, dataconverters, consumers);
        //engine.setConsumers(consumers);

        //// Load the config concerning producers
        //this.loadBindingComponents(properties,PRODUCER, producersName, producersInstalled, dataconverters);
        //// instanciate the array for producers
        //Producer[] producers = new Producer[producersName.size()];		
        //this.instanciateBindingComponents(producersName, producersInstalled, dataconverters, producers);
        //engine.setProducers(producers);
    }

//----------------------------------------------
//             PRIVATE UTIL METHODS 	
//----------------------------------------------
    
    /**
     * 
     * @param bindingComponentsName  List<String> contains the name of the BC
     * @param bindingComponentsInstalled List<Boolean> contains the boolean for the flag installed or not
     * @param dataconverters hashmap with as key : name of the producer or consumer. value List<String> of the dataconverter names
     * @param instances
     * @throws Exception
     */
    private void instanciateBindingComponents( List<String> bindingComponentsName , List<Boolean> bindingComponentsInstalled, Dictionary<String,List<String>> dataconverters, BindingComponent[] instances )
    {
        
        IEnumerable<String> itBCNames = bindingComponentsName.AsEnumerable<String>();
        IEnumerable<Boolean> itBCInstalled = bindingComponentsInstalled.AsEnumerable<Boolean>();
        //int i=0;
        //while(itBCNames.hasNext()){
        //    String BCClassName = (String)itBCNames.next();
        //    Boolean BCNameInstalledFlag = (Boolean)itBCInstalled.next();
        //    try{
        //        Class BCClass = Class.forName(BCClassName);				
        //        instances[i] = (BindingComponent)BCClass.newInstance();
        //        if( BCNameInstalledFlag.booleanValue() ){
        //            instances[i].setStatus(BindingComponent.INSTALLED);					
        //        }else{
        //            instances[i].setStatus(BindingComponent.NOT_INSTALLED);
        //        }
        //    }catch(ClassNotFoundException cnfe){
        //        String msg = CLASS_NOT_FOUND_MSG.format(new Object[]{BCClassName});
        //        getLogger().severe(msg);
        //        throw cnfe;
        //    }catch(ClassCastException cce){
        //        String msg = CLASS_NOT_IMPLEMENT_BC_MSG.format(new Object[]{BCClassName,BindingComponent.class});
        //        getLogger().severe(msg);
        //        throw cce;				
        //    } catch (InstantiationException ie) {
        //        String msg = CLASS_NOT_IMPLEMENT_CONSTRUCTOR_MSG.format(new Object[]{BCClassName});
        //        getLogger().severe(msg);
        //        throw ie;				
        //    } catch (IllegalAccessException iae) {
        //        String msg = CLASS_CONSTRUCTOR_NOT_PUBLIC_MSG.format(new Object[]{BCClassName});
        //        getLogger().severe(msg);
        //        throw iae;				
        //    }			
        //    // part of dataconverter instanciation
        //    ArrayList dataconvertersNames = (ArrayList)dataconverters.get(BCClassName);
        //    if( dataconvertersNames != null ){
        //        DataConverter dc = instanciateDataConverter(dataconvertersNames);
        //        instances[i].setIncludedDataConverter(dc);
        //    }
        //    i++;
        //}
    }
    
    /**
     * instanciate the dataconverters
     * The last is included in its predecessor (order is important)
     * @param dataconvertersNames ArrayList<String> name of the DataConverter , the first will be returned, the second is included 
     * @return
     */
    private DataConverter instanciateDataConverter(ArrayList dataconvertersNames) 
    {		
        DataConverter lastDC = null;
        //for(int iDC=dataconvertersNames.size(); iDC>0; iDC--){
        //    String DCClassName = (String)dataconvertersNames.get(iDC-1);
        //    try{
        //        Class DCClass = Class.forName(DCClassName);				
        //         DataConverter DC = (DataConverter)DCClass.newInstance();
        //         DC.setIncludedDC(lastDC);
        //         lastDC = DC;				
        //    }catch(ClassNotFoundException cnfe){
        //        String msg = CLASS_NOT_FOUND_MSG.format(new Object[]{DCClassName});
        //        getLogger().severe(msg);
        //        throw cnfe;
        //    }catch(ClassCastException cce){
        //        String msg = CLASS_NOT_IMPLEMENT_BC_MSG.format(new Object[]{DCClassName,DataConverter.class});
        //        getLogger().severe(msg);
        //        throw cce;				
        //    } catch (InstantiationException ie) {
        //        String msg = CLASS_NOT_IMPLEMENT_CONSTRUCTOR_MSG.format(new Object[]{DCClassName});
        //        getLogger().severe(msg);
        //        throw ie;				
        //    } catch (IllegalAccessException iae) {
        //        String msg = CLASS_CONSTRUCTOR_NOT_PUBLIC_MSG.format(new Object[]{DCClassName});
        //        getLogger().severe(msg);
        //        throw iae;				
        //    }			
        //}
        return lastDC;
    }
    /**
     * Load and read the properties file concerning the configuration of binding components
     * @param properties the system properties containing the config<read only>
     * @param KEY key of the property <read only>
     * @param bindingComponentsName  List<String> will contain the name of the BC
     * @param bindingComponentsInstalled List<Boolean> will contain the boolean for the flag installed or not
     * @param dataconverters hashmap with as key : name of the producer or consumer. value Array<String> of the dataconverter names
     * @throws Exception
     */
    //public void loadBindingComponents(Properties properties, final String KEY, List bindingComponentsName,List bindingComponentsInstalled, HashMap dataconverters )
    //{

    //    String BCname;
    //    boolean installed;

    //    // Reading of the producer conf
    //    boolean finish = false;
    //    boolean finishDataConverter = false;

    //    int i=1;
    //    int j=1;
    //    while(! finish){
    //        String key = KEY+i;
    //        BCname = properties.getProperty(key);

    //        if(BCname == null){
    //            finish = true;    			
    //        }else{
    //            // read the optional installed flag
    //            String installedString = properties.getProperty(key+INSTALLED);

    //            if( (installedString != null)&&(installedString.equalsIgnoreCase(NO))){
    //                installed = false;    				
    //            }else{ // default value
    //                installed = true;    				
    //            }
    //            bindingComponentsName.add(BCname);
    //            bindingComponentsInstalled.add( (installed?Boolean.TRUE:Boolean.FALSE ) );    			

    //            // read the optional dataconverters
    //            finishDataConverter = false;
    //            List dataconverterNames =null;
    //            j=1;
    //            while(!finishDataConverter){
    //                String keyDataConverter = key+DATACONVERTER+j;
    //                String dataconverter = properties.getProperty(keyDataConverter);
    //                if( dataconverter == null ){
    //                    finishDataConverter = true;    					
    //                }else{
    //                    if( j==1 ){
    //                        dataconverterNames = new ArrayList(3);
    //                    }
    //                    dataconverterNames.add(dataconverter);
    //                }
    //                j++;
    //            }// end of dataconverter reading
    //            if( dataconverterNames!= null ){ 
    //                dataconverters.put(BCname, dataconverterNames);
    //            }
    //        }
    //        i++;
    //    }// end of reading consumer/producer
    //}		



    // GETTERS 
    public int getNbMaxThreads() {
        return nbMaxThreads;
    }


    private void errorOnMaxThreads(){
        //String msg = ON_MAX_THREADS_MSG.format(new Object[]{ NB_MAX_THREADS, NB_MAX_THREADS_DEFAULT_VALUE} );
        //getLogger().severe(msg);
    } 
    }
}
