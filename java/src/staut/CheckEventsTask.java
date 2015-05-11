package staut;

import java.io.BufferedReader;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.net.URLConnection;
import java.time.Duration;
import java.util.Set;
import java.util.TreeSet;
import java.util.concurrent.ScheduledFuture;
import java.util.concurrent.TimeUnit;

/**
 * Thread that periodically checks for new events to start downloading their
 * availability periodically and stops the automatic download of expired events.
 */
public class CheckEventsTask implements Runnable {
    
    private static final String BASE_BILLETTSERVICE_URL = "http://www.billettservice.no";
    private static final String BASE_EVENT_PAGE_URL = BASE_BILLETTSERVICE_URL + "/event/";

    @Override
    public void run() {
        try {
            updateEventCollects();
        } catch (Exception ex) {
            ex.printStackTrace();
        }
    }
    
    private void updateEventCollects() throws Exception {
        URL url = new URL(Collector.LERKENDAL_URL);
        URLConnection uc = url.openConnection();
        InputStreamReader input = new InputStreamReader(uc.getInputStream());
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        Set<String> ids = new TreeSet<>();
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("button--buy roundedButton") && !inputLine.contains("sesongkort")) {
                String[] quoteSplit = inputLine.split("\"");
                String[] slashSplit = quoteSplit[5].split("/");
                String eventId = slashSplit[slashSplit.length-1];
                ids.add(eventId);
            }
        }
        // Remove expired events
        Collector.activeCollects.entrySet().stream().filter((collect) -> (!ids.contains(collect.getKey()))).forEach((collect) -> {
            // event is over
            if(collect.getValue().cancel(false)) {
                Collector.activeCollects.remove(collect.getKey());
                System.out.println("Cancelled collection for id " + collect.getKey());
            } else {
                System.out.println("Failed to cancel collection for id " + collect.getKey());
            }
        });
        // Add new events
        for(String eventId : ids) {
            if(!Collector.activeCollects.containsKey(eventId)) {
                URL availabilityURL = extractAvailabilityURL(eventId);
                if(availabilityURL != null) {
                    startAvailabilityCollection(eventId, availabilityURL);
                } else {
                    System.out.println("Failed to find availabilityURL for event ID " + eventId);
                    System.out.println("Most likely internet sale has not yet started.");
                }
            }
        }
    }
    
    private URL extractAvailabilityURL(String eventId) throws IOException {
        URL eventPage = new URL(BASE_EVENT_PAGE_URL + eventId);
        URLConnection uc = eventPage.openConnection();
        InputStreamReader input = new InputStreamReader(uc.getInputStream());
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("availabilityURL")) {
                String[] colonSplit = inputLine.split(":");
                int counter=0;
                for(String prop : colonSplit) {
                    counter++;
                    if(prop.contains("availabilityURL")) {
                        return new URL(BASE_BILLETTSERVICE_URL + colonSplit[counter].split("\"")[1].replace("\\", ""));
                    }
                }
            }
        }
        return null;
    }
    
    private void startAvailabilityCollection(String name, URL availabilityURL) throws Exception {
        Duration rate = Configuration.getCollectAvailabilityPeriod();
        System.out.println("Starting collection of URL: " + availabilityURL + " every " + rate.toMinutes() + " minutes.");
        CollectAvailabilityTask cat = new CollectAvailabilityTask(name, availabilityURL);
        ScheduledFuture future = Collector.executor.scheduleAtFixedRate(cat, 0, rate.toMinutes(), TimeUnit.MINUTES);
        Collector.activeCollects.put(name, future);
    }
    
}
