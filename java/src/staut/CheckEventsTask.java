package staut;

import java.net.URL;
import java.time.Duration;
import java.util.List;
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
        List<Integer> ids = Collector.findActiveEvents();
        // Remove expired events
        Collector.activeCollects.entrySet().stream().filter((collect) -> (!ids.contains(collect.getKey()))).forEach((collect) -> {
            // event is over
            if(collect.getValue().cancel(false)) {
                Collector.activeCollects.remove(collect.getKey());
                STAut.info("Cancelled collection for id " + collect.getKey());
            } else {
                STAut.error("Failed to cancel collection for id " + collect.getKey());
            }
        });
        // Add new events
        for(Integer eventId : ids) {
            if(!Collector.activeCollects.containsKey(eventId)) {
                URL availabilityURL = Collector.extractAvailabilityURL(eventId);
                if(availabilityURL != null) {
                    startAvailabilityCollection(eventId, availabilityURL);
                } else {
                    STAut.info("Failed to find availabilityURL for event ID " + eventId);
                    STAut.info("Most likely internet sale has not yet started.");
                }
            }
        }
    }
    
    private void startAvailabilityCollection(Integer eventId, URL availabilityURL) throws Exception {
        Duration rate = Configuration.getCollectAvailabilityPeriod();
        STAut.info("Starting collection of URL: " + availabilityURL + " every " + rate.toMinutes() + " minutes.");
        CollectAvailabilityTask cat = new CollectAvailabilityTask(eventId, availabilityURL);
        ScheduledFuture future = Collector.executor.scheduleAtFixedRate(cat, 0, rate.toMinutes(), TimeUnit.MINUTES);
        Collector.activeCollects.put(eventId, future);
    }
    
}
