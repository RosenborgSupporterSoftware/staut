package staut;

import java.time.Duration;
import java.util.Map;
import java.util.TreeMap;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.ScheduledThreadPoolExecutor;
import java.util.concurrent.TimeUnit;

/**
 * Class that starts background threads for automatically searching for events
 * and periodically downloading availability data for each event.
 */
public class Collector {
    public static final String LERKENDAL_URL = "http://www.billettservice.no/venue/lerkendal-stadion-trondheim-billetter/TLD/25?attractions=100563";
    
    static Collector collector = new Collector();
    static CheckEventsTask checkEventsTask = new CheckEventsTask();
    static ScheduledThreadPoolExecutor executor = new ScheduledThreadPoolExecutor(5);
    static Map<String,ScheduledFuture> activeCollects = new TreeMap<>();
    
    private Collector() {
    }
    
    public static void main(String[] args) throws Exception {
        if(args.length == 0) {
            startCollector();
        }
    }
    
    public static void startCollector() throws Exception {
        //Start in daemon mode
        Duration rate = Configuration.getCheckEventsPeriod();
        Collector.executor.scheduleAtFixedRate(checkEventsTask, 0, rate.toMinutes(), TimeUnit.MINUTES);
        while(true) {
            try {
                Thread.sleep(60000);
            } catch (InterruptedException ie) {
                // ignore
            }
        }
    }
    
}