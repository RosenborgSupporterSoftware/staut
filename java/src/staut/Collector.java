package staut;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.URL;
import java.net.URLConnection;
import java.text.SimpleDateFormat;
import java.time.Duration;
import static java.time.OffsetTime.now;
import java.util.ArrayList;
import java.util.Date;
import java.util.List;
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
    
    private static final SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm");
    private static final String BASE_BILLETTSERVICE_URL = "http://www.billettservice.no";
    private static final String BASE_EVENT_PAGE_URL = BASE_BILLETTSERVICE_URL + "/event/";
    private static final String LERKENDAL_URL = BASE_BILLETTSERVICE_URL + "/venue/lerkendal-stadion-trondheim-billetter/TLD/25?attractions=100563";
    
    static Collector collector = new Collector();
    static CheckEventsTask checkEventsTask = new CheckEventsTask();
    static ScheduledThreadPoolExecutor executor = new ScheduledThreadPoolExecutor(5);
    static Map<Integer,ScheduledFuture> activeCollects = new TreeMap<>();
    
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
    
    public static List<Integer> findActiveEvents() throws IOException {
        URL url = new URL(LERKENDAL_URL);
        URLConnection uc = url.openConnection();
        InputStreamReader input = new InputStreamReader(uc.getInputStream());
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        List<Integer> ids = new ArrayList<>();
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("button--buy roundedButton") && !inputLine.contains("sesongkort")) {
                String[] quoteSplit = inputLine.split("\"");
                String[] slashSplit = quoteSplit[5].split("/");
                String eventId = slashSplit[slashSplit.length-1];
                ids.add(new Integer(eventId));
            }
        }
        return ids;
    }
    
    public static URL extractAvailabilityURL(Integer eventId) throws IOException {
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
    
    public static void download(URL url, File file) throws IOException {
        URLConnection uc = url.openConnection();
        System.out.println("Downloading into: " + file);
        try (InputStream in = uc.getInputStream(); OutputStream outStream = new FileOutputStream(file)) {
            int len;
            do {
                byte[] buffer = new byte[1024];
                len = in.read(buffer);
                if(len>0) {
                    outStream.write(buffer,0,len);
                }
            } while(len != -1);
            outStream.flush();
        }
        System.out.println("Finished downloading: " + file);
    }
    
    public static File generateFileName(File dir, Integer eventId) {
        Date now = new Date();
        return new File(dir, eventId + "_" + dateFormat.format(now) + ".xml");
    }
    
}