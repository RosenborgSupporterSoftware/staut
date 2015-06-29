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
import java.nio.charset.Charset;
import java.text.SimpleDateFormat;
import java.time.Duration;
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
    
    private static final SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH-mm");
    private static final String BASE_BILLETTSERVICE_URL = "http://www.billettservice.no";
    private static final String BASE_EVENT_PAGE_URL = BASE_BILLETTSERVICE_URL + "/event/";
    private static final String LERKENDAL_URL = BASE_BILLETTSERVICE_URL + "/venue/lerkendal-stadion-trondheim-billetter/TLD/25?attractions=100563";
    
    static Collector collector = new Collector();
    static CheckEventsTask checkEventsTask = new CheckEventsTask();
    static ScheduledThreadPoolExecutor executor = new ScheduledThreadPoolExecutor(5);
    static Map<EventInfo,ScheduledFuture> activeCollects = new TreeMap<>();
    
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
    
    public static List<EventInfo> findActiveEvents() throws IOException {
        URL url = new URL(LERKENDAL_URL);
        URLConnection uc = url.openConnection();
        InputStreamReader input = new InputStreamReader(uc.getInputStream());
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        List<EventInfo> ids = new ArrayList<>();
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("button--buy roundedButton") && !inputLine.contains("sesongkort")) {
                String[] hrefSplit = inputLine.split("href=");
                String[] quoteSplit = hrefSplit[1].split("\"");
                String[] slashSplit = quoteSplit[1].split("/");
                String eventId = slashSplit[slashSplit.length-1];
                ids.add(extractEventInfo(new Integer(eventId)));
            }
        }
        return ids;
    }
    
    public static EventInfo extractEventInfo(Integer eventId) throws IOException {
        EventInfo info = new EventInfo(eventId);
        URL eventPage = new URL(BASE_EVENT_PAGE_URL + eventId);
        URLConnection uc = eventPage.openConnection();
        InputStreamReader input = new InputStreamReader(uc.getInputStream(), Charset.forName("UTF-8"));
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("availabilityURL")) {
                String[] colonSplit = inputLine.split(":");
                int counter=0;
                for(String prop : colonSplit) {
                    counter++;
                    if(prop.contains("eventName")) {
                        String name = replaceUnicodeEscapes(colonSplit[counter].split("\"")[1]);
                        info.setEventName(name);
                        info.setOpponent(name.split("-")[1].trim().split("\\s+")[0]);
                    } else if(prop.contains("availabilityURL")) {
                        info.setAvailabilityURL(new URL(BASE_BILLETTSERVICE_URL + colonSplit[counter].split("\"")[1].replace("\\", "")));
                        String eventCode = info.getAvailabilityURL().getPath().split("/NO/")[1].split(",")[0];
                        info.setEventCode(eventCode);
                        if(eventCode != null && eventCode.length() >=3) {
                            info.setLocation(eventCode.substring(0, 3));
                        }
                        if(eventCode != null && eventCode.length() >=5) {
                            String part2 = eventCode.substring(3, 5);
                            if(part2.matches("\\d\\d")) { // Two digits
                                // This is a league game.
                                info.setCompetition("LEAGUE");
                                info.setRound(Integer.parseInt(part2));
                            } else {
                                info.setCompetition(eventCode.substring(3, 5));
                                if(eventCode.length() >=7) {
                                    info.setRound(Integer.parseInt(eventCode.substring(5,7)));
                                }
                            }
                        }
                    } else if(prop.contains("mapsellURL")) {
                        info.setGeometryURL(new URL(BASE_BILLETTSERVICE_URL + colonSplit[counter].split("\"")[1].replace("\\", "")));
                    } else if (prop.contains("eventDateTime")) {
                        String[] starttime = colonSplit[counter].split("\"")[1].split(",")[1].trim().split(" ");
                        info.setEventDate(starttime[0].trim());
                        info.setEventTime(starttime[starttime.length-1].trim() + ":" + colonSplit[counter+1].split("\"")[0]);
                    }
                }
            }
        }
        return info;
    }
    
    private static String replaceUnicodeEscapes(String orig) {
//        æ = \u00E6
//        Æ = \u00C6
//        ø = \u00F8
//        Ø = \u00D8
//        å = \u00E5
//        Å = \u00D5
        String ret = orig.replace("\\u00e6", "æ");
        ret = ret.replace("\\u00c6", "Æ");
        ret = ret.replace("\\u00f8", "ø");
        ret = ret.replace("\\u00d8", "Ø");
        ret = ret.replace("\\u00e5", "å");
        ret = ret.replace("\\u00d5", "Å");
        return ret;
    }
    
    public static void download(URL url, File file) {
        STAut.info("Downloading into: " + file);
        int total = 0;
        try (InputStream in = url.openConnection().getInputStream(); OutputStream outStream = new FileOutputStream(file)) {
            int len;
            do {
                byte[] buffer = new byte[1024];
                len = in.read(buffer);
                if(len>0) {
                    outStream.write(buffer,0,len);
                    total += len;
                }
            } while(len != -1);
            outStream.flush();
        } catch (IOException ioe) {
            STAut.error("Problem downloading URL '" + url + "' into file '" + file + "'");
            STAut.error(ioe);
        }
        STAut.info("Finished downloading: " + file + " (" + total + " bytes)");
    }
    
    public static File generateFileName(File dir, EventInfo info) {
        Date now = new Date();
        return new File(dir, info.getEventId() + "_" + info.getOpponent() + "_" + dateFormat.format(now) + ".xml");
    }
    
}