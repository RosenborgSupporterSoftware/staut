package staut;

import java.time.Duration;
import java.util.List;
import java.util.Map.Entry;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

/**
 * Thread that periodically checks for new events to start downloading their
 * availability periodically and stops the automatic download of expired events.
 */
public class CheckEventsTask implements Runnable {

    @Override
    public void run() {
        try {
            updateEventCollects();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    
    private void updateEventCollects() throws Exception {
        List<EventInfo> infos = Collector.findActiveEvents();
        // Remove expired events
        STAut.info("Updating event collections.");
        for(Entry<EventInfo,ScheduledFuture> event : Collector.activeCollects.entrySet()) {
            EventInfo info = event.getKey();
            if(!infos.contains(info)) {
            // event is over
                if(event.getValue().cancel(false)) {
                    Collector.activeCollects.remove(info);
                    STAut.info("Cancelled collection for event " + info);
                } else {
                    STAut.error("Failed to cancel collection for event " + info);
                }
            }
        }
        // Add new events
        for(EventInfo info : infos) {
            if(!Collector.activeCollects.containsKey(info)) {
                if(info.getAvailabilityURL() != null) {
                    STAut.info("New event:\n" + info.detailedString());
                    startAvailabilityCollection(info);
                } else {
                    STAut.info("Failed to find availabilityURL for event " + info);
                    STAut.info("Most likely internet sale has not yet started.");
                    STAut.info("All details we could collect: " + info.detailedString());
                }
            }
        }
    }
    
    private void startAvailabilityCollection(EventInfo info) throws Exception {
        Duration rate = Configuration.getCollectAvailabilityPeriod();
        STAut.info("Starting collection of URL: " + info.getAvailabilityURL() + " every " + rate.toMinutes() + " minutes.");
        CollectAvailabilityTask cat = new CollectAvailabilityTask(info);
        cat.writeEventInfo();
        ScheduledFuture future = Collector.executor.scheduleAtFixedRate(cat, 0, rate.toMinutes(), TimeUnit.MINUTES);
        Collector.activeCollects.put(info, future);
    }
    
}
