package staut;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.OutputStream;
import java.net.HttpURLConnection;
import java.net.URL;
import java.nio.charset.Charset;
import java.nio.file.Files;
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
    private static final String BASE_BILLETTSERVICE_URL = "http://www.ticketmaster.no";
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
        HttpURLConnection uc = getHttpURLConnection(new URL(LERKENDAL_URL));
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
    
    private static HttpURLConnection getHttpURLConnection(URL url) throws IOException { 
        HttpURLConnection uc = (HttpURLConnection) url.openConnection();
        uc.setInstanceFollowRedirects(true);
	int status = uc.getResponseCode();
	if (status != HttpURLConnection.HTTP_OK) {
            STAut.info("Could not open URL " + url + ", status " + status);
		if (status == HttpURLConnection.HTTP_MOVED_TEMP
			|| status == HttpURLConnection.HTTP_MOVED_PERM
				|| status == HttpURLConnection.HTTP_SEE_OTHER) {
		// Redirect
		String redirect = uc.getHeaderField("Location");
                uc = (HttpURLConnection) new URL(redirect).openConnection();
                STAut.info("Redirecting to " + redirect);
            }
	}
        return uc;
    }
    
    public static EventInfo extractEventInfo(Integer eventId) throws IOException {
        EventInfo info = new EventInfo(eventId);
        URL eventPage = new URL(BASE_EVENT_PAGE_URL + eventId);
        HttpURLConnection uc = getHttpURLConnection(eventPage);
        info.setEventURL(uc.getURL());
        STAut.info("Extracting event information from URL: " + uc.getURL());
        InputStreamReader input = new InputStreamReader(uc.getInputStream(), Charset.forName("UTF-8"));
        BufferedReader in = new BufferedReader(input);
        String inputLine;
        boolean foundAvailabilityData = false;
        while ((inputLine = in.readLine()) != null) {
            if(inputLine.contains("ism_config")) {
                String[] quoteSplit = inputLine.split("ism_config")[1].split("\"");
                int counter = 0;
                for(String prop : quoteSplit) {
                    if(prop.equals("availability")) {
                        info.setAvailabilityURL(new URL(BASE_BILLETTSERVICE_URL + quoteSplit[counter+4].replace("\\", "")));
                        foundAvailabilityData = true;
                    }
                    if(prop.equals("geometry")) {
                        info.setGeometryURL(new URL(BASE_BILLETTSERVICE_URL + quoteSplit[counter+4].replace("\\", "")));
                    }
                    counter++;
                }
            }            
            if(inputLine.contains("event_info")) {
                String[] quoteSplit = inputLine.split("\"");
                int counter = 0;
                String year = null;
                String month = null;
                String day = null;
                String time = null;
                for(String prop : quoteSplit) {
                    if(prop.equals("event_info")) {
                        info.setEventName(quoteSplit[counter+4]);
                    } 
                    if(prop.equals("year")) {
                        year = quoteSplit[counter+2];
                    }
                    if(prop.equals("month")) {
                        month = quoteSplit[counter+2];
                    }
                    if(prop.equals("day")) {
                        day = quoteSplit[counter+2];
                    }
                    if(prop.equals("time")) {
                        info.setEventTime(quoteSplit[counter+2]);
                        break; // Avoid reading later times in same string
                    }     
                    counter++;
                }
                if(year != null & month != null & day != null) {
                    info.setEventDate(day + "." + month + "." + year);
                }
            }           
            if(inputLine.contains("id=\"data-ads")) {
                String[] quoteSplit = inputLine.split("\"");
                int counter = 0;
                for(String prop : quoteSplit) {
                    if(prop.equals("eventid")) {
                        info.setEventCode(quoteSplit[counter+2]);
                        String subCode = info.getEventCode();
                        if(subCode != null && subCode.length() >=3) {
                            info.setLocation(subCode.substring(0, 3));
                        }
                        if(subCode != null && subCode.length() >=5) {
                            String part2 = subCode.substring(3, 5);
                            String part3 =subCode.substring(5);
                            if(subCode.length() >= 8 && subCode.substring(7,8).equals("N")) {
                                // This is Norwegian cup.
                                info.setCompetition(EventInfo.CUP_COMPETITION);
                                info.setRound(0); // unknown
                            } else if(part3.matches("\\d\\d")) { // Two digits
                                // This is a league game.
                                info.setCompetition(EventInfo.LEAGUE_COMPETITION);
                                info.setRound(Integer.parseInt(part3));
                            } else if(part2.equals(EventInfo.EC_COMPETITION)){
                                // This is European cup.
                                info.setCompetition(EventInfo.EC_COMPETITION);
                                info.setRound(Integer.parseInt(part3));    
                            } else if(part2.equals(EventInfo.EL_COMPETITION)){
                                // This is Europa League
                                info.setCompetition(EventInfo.EL_COMPETITION);
                                info.setRound(Integer.parseInt(part3));
                            } else if(part2.equals(EventInfo.CL_COMPETITION)){
                                // This is Champions League
                                info.setCompetition(EventInfo.CL_COMPETITION);
                                info.setRound(Integer.parseInt(part3));
                            } else {
                                info.setCompetition("unknown");
                                info.setRound(0);
                            }
                        }
                        break;
                    }
                    counter++;
                }
            }
        }
//        while ((inputLine = in.readLine()) != null && !foundAvailabilityData) {
//            if(inputLine.contains("availabilityURL")) {
//                String[] colonSplit = inputLine.split(":");
//                int counter=0;
//                for(String prop : colonSplit) {
//                    counter++;
//                    if(prop.contains("eventName")) {
//                        String name = replaceUnicodeEscapes(colonSplit[counter].split("\"")[1]);
//                        info.setEventName(name);
//                    } else if(prop.contains("availabilityURL")) {
//                        foundAvailabilityData = true;
//                        info.setAvailabilityURL(new URL(BASE_BILLETTSERVICE_URL + colonSplit[counter].split("\"")[1].replace("\\", "")));
//                    } else if(prop.contains("prices")) {
//                        String eventCode = colonSplit[counter+1].split("\"")[1];
//                        info.setEventCode(eventCode);
//                        String subCode = eventCode;
//                        if(subCode != null && subCode.length() >=3) {
//                            info.setLocation(eventCode.substring(0, 3));
//                        }
//                        if(subCode != null && subCode.length() >=5) {
//                            String part2 = subCode.substring(3, 5);
//                            String part3 =subCode.substring(5);
//                            if(subCode.length() >= 8 && subCode.substring(7,8).equals("N")) {
//                                // This is Norwegian cup.
//                                info.setCompetition(EventInfo.CUP_COMPETITION);
//                                info.setRound(0); // unknown
//                            } else if(part2.matches("\\d\\d")) { // Two digits
//                                // This is a league game.
//                                info.setCompetition(EventInfo.LEAGUE_COMPETITION);
//                                info.setRound(Integer.parseInt(part2));
//                            } else if(part2.equals(EventInfo.EC_COMPETITION)){
//                                // This is European cup.
//                                info.setCompetition(EventInfo.EC_COMPETITION);
//                                info.setRound(Integer.parseInt(part3));    
//                            } else if(part2.equals(EventInfo.EL_COMPETITION)){
//                                // This is Europa League
//                                info.setCompetition(EventInfo.EL_COMPETITION);
//                                info.setRound(Integer.parseInt(part3));
//                            } else if(part2.equals(EventInfo.CL_COMPETITION)){
//                                // This is Champions League
//                                info.setCompetition(EventInfo.CL_COMPETITION);
//                                info.setRound(Integer.parseInt(part3));
//                            } else {
//                                info.setCompetition("unknown");
//                                info.setRound(0);
//                            }
//                        }
//                    } else if(prop.contains("mapsellURL")) {
//                        info.setGeometryURL(new URL(BASE_BILLETTSERVICE_URL + colonSplit[counter].split("\"")[1].replace("\\", "")));
//                    } else if (prop.contains("eventDateTime")) {
//                        String[] starttime = colonSplit[counter].split("\"")[1].split(",")[1].trim().split(" ");
//                        info.setEventDate(starttime[0].trim());
//                        info.setEventTime(starttime[starttime.length-1].trim() + ":" + colonSplit[counter+1].split("\"")[0]);
//                    }
//                }
//            }
//        }
        if(!foundAvailabilityData) {
            STAut.error("Could not find relevant event details for event. " + info.detailedString());
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
        return orig.replace("\\u00e6", "æ").
                replace("\\u00c6", "Æ").
                replace("\\u00f8", "ø").
                replace("\\u00d8", "Ø").
                replace("\\u00e5", "å").
                replace("\\u00d5", "Å").
                replace("\\", "");
    }
    
    public static int download(URL url, File file) throws IOException {
        STAut.info("Downloading into: " + file);
        int total = 0;
        Files.createFile(file.toPath());
        try (InputStream in = getHttpURLConnection(url).getInputStream(); OutputStream outStream = new FileOutputStream(file)) {
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
        return total;
    }
    
    public static File generateFileName(File dir, EventInfo info) {
        Date now = new Date();
        return new File(dir, info.getEventId() + "_" + info.getOpponentWithoutSlash() + "_" + dateFormat.format(now) + ".xml");
    }
    
}