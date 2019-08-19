using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net.Config;

namespace FGA.Automate.Core
{
    public class CoreServiceEngine : ComponentLifeCycle
    {

        ///** one-shot barrier used for 
        // * <li>waiting the end of consumers starting </li>
        // * <li>& waiting the end of producers stopping</li>
        // */ 
        //private CountDown latch;

        ///**
        // *  one-shot barrier used for waiting the end of the program
        // */
        //private CountDown finalLatch;

        //private IntegrationProperties _properties;

        ///**
        // * Declaration of the threads pool using a number max of threads (configured) 
        // */    
        //private PooledExecutor _threadPool;

        ///**
        // * Array of the producers used by the engine.
        // */
        //private Producer[] producers;
        ///**
        // * Array of the consumers used by the engine.
        // */		
        //private Consumer[] consumers;

        ///**
        // * data structure shared by consumers and producers
        // */
        //private MessageExchange messageExchange;



        /**
         * Unique constructor controlled by the class itself. 
         */
        private CoreServiceEngine()
        {
        }

        /**
         * Main for starting the daemon
         * @param args
         */
        public static void main(String[] args)
        {
            //getLogger().info("Entering in the CoreServiceEngine main");
            //try {
            //    final CoreServiceEngine engine = new CoreServiceEngine();
            //    // Init: exceptions are fatal.
            //    engine.init();

            //    // start the threads
            //    engine.start();

            //    // init a latch for the stop
            //    engine.setFinalLatch(new CountDown(engine.consumers.length+engine.producers.length));

            //    // Main Thread will Wait for the stop of all producers and consumers
            //    engine.getFinalLatch().acquire();

            //    // clean stop of the pool
            //    engine.stopThreadPool();

            //} catch (Exception e) {
            //    String msg = "Main class for the integration has been interrupted. The program will end now.";
            //    getLogger().severe(msg,e);
            //    e.printStackTrace(System.err);
            //    System.err.print(msg);			
            //}

            //getLogger().info("End of the CoreServiceEngine main => QUIT");
        }

        /**
         * Method for preparing the CoreServiceEngine. It must :
         * <li>Check if the properties file exists</lI>
         * <li>read and control the properties</li>
         * <li>instanciate the thread pool , the producers and the consumers</li>
         */
        public void Init()
        {
            //_properties = new IntegrationProperties();		
            //// read the properties files and build our producers & consumers arrays
            //_properties.loadProperty(this);
            //this.initThreadPool();		
            //// init the consumers and producers
            //for( int i=0; i<this.consumers.length; i++){
            //    this.consumers[i].init();
            //}

            //for( int i=0; i<this.producers.length; i++){
            //    this.producers[i].init();
            //}

            //// init the exchange data structure
            //this.initMessageExchange(this.consumers);

            //// put a shutdown hook
            //Runtime.getRuntime().addShutdownHook(new Thread("SHUTDOWN_HOOK") {
            //    public void run() { 
            //        try {
            //            CoreServiceEngine.this.stop();
            //        } catch (Exception e) {
            //            getLogger().severe("Main class for the integration has been interrupted. The program will end now.",e);
            //        }
            //    }
            //});		
        }

        public void Start(){
            // init the barrier for waiting the end of consumers starting
        //    this.setLatch( new CountDown(this.consumers.length) );

        //    // Start the consumers
        //    for( int i=0; i<this.consumers.length; i++){
        //        this.consumers[i].start(CoreServiceEngine.this);
        //    }								
        //    // wait for the end of start of consumers
        //    this.getLatch().acquire();

        //    // Start the producers
        //    for( int i=0; i<this.producers.length; i++){
        //        this.producers[i].start(CoreServiceEngine.this);
        //    }
        }



        /**
         * build the thread Pool, using the EDU.oswego.cs library , included in JRE 1.5 
         * The used used channel (BoundedBuffer) is implemented as blocked with a max capacity of 16.
         */
        //    private void initThreadPool(){ 		
        //        _threadPool = new PooledExecutor(new BoundedBuffer(16),_properties.getNbMaxThreads());
        //        _threadPool.setMinimumPoolSize(_properties.getNbMaxThreads());
        //        _threadPool.setKeepAliveTime(-1); // live forever
        //        _threadPool.createThreads(_properties.getNbMaxThreads());		
        //    }

        //    private void stopThreadPool(){
        //        _threadPool.drain();
        //        // interrompre le preload de cache
        //        _threadPool.shutdownAfterProcessingCurrentlyQueuedTasks();
        //        try {
        //            _threadPool.awaitTerminationAfterShutdown(10000L);
        //        } catch (InterruptedException e) {
        //            getLogger().severe("Stop ThreadPool has been interrupted.",e);
        //        }
        //    }	

        //    /**
        //     * Instanciation of the data structure, shared between consumers & producers
        //     */
        //    private void initMessageExchange(Consumer[] registeredConsumers){		
        //        messageExchange = new MessageExchange(registeredConsumers); 
        //    }
            /**
             * Method to stop softly and with respect of the synchronization between Consumers and Producers<br/>
             * BLOCKING method
             */
            public void Stop() {
        //        getLogger().info("Stop Engine --->");

        //        // init the barrier for waiting the end of producers stopping
        //        this.setLatch( new CountDown(this.producers.length) );

        //        // Stop the consumers and producers
        //        for( int i=0; i<this.producers.length; i++){
        //            this.producers[i].stop();
        //        }
        //        // wait for the end of stopping of producers
        //        this.getLatch().acquire();

        //        for( int i=0; i<this.consumers.length; i++){
        //            this.consumers[i].stop();
        //        }		
        //        // notify all the blocked consumers
        //        this.getMessageExchange().send(null);

        //        getLogger().info("Stop Engine ---> Finished");
            }

            /**
             * Method to shutdown immediatly.RIGHT NOW!<br/>
             * NOT BLOCK method
             */
            public void Shutdown() {
        //        getLogger().info("Shutdown Engine --->");

        //        for( int i=0; i<this.consumers.length; i++){
        //            this.consumers[i].shutdown();
        //        }
        //        for( int i=0; i<this.producers.length; i++){
        //            this.producers[i].shutdown();
        //        }
        ////		shutdown the consumers and producers
        //        this._threadPool.shutdownNow();

        //        // release the final latch
        //        while (	this.getFinalLatch().currentCount() >0 ){
        //            this.getFinalLatch().release();
        //        }

        //        getLogger().info("Shutdown Engine ---> Finished");
            }


        //    public void createExecutor(Runnable component) throws InterruptedException{
        //        _threadPool.execute(component);
        //    }



        //    //-----------------------------------------------------------------
        //    // GETTERS & SETTERS ZONE
        //    //-----------------------------------------------------------------

        //    public static Logger getLogger(){
        //        return logger;
        //    }

        //    protected void setConsumers(Consumer[] consumers) {
        //        this.consumers = consumers;
        //    }

        //    protected void setProducers(Producer[] producers) {
        //        this.producers = producers;
        //    }

        //    public MessageExchange getMessageExchange() {
        //        return messageExchange;
        //    }

        //    public CountDown getLatch() {
        //        return latch;
        //    }

        //    private void setLatch(CountDown latch) {
        //        this.latch = latch;
        //    }

        //    public CountDown getFinalLatch() {
        //        return finalLatch;
        //    }

        //    private void setFinalLatch(CountDown finalLatch) {
        //        this.finalLatch = finalLatch;
        //    }

    }
}