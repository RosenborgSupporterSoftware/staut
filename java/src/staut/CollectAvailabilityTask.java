package staut;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimerTask;

/**
 * Thread that periodically downloads availability data for a specific event.
 */
public class CollectAvailabilityTask extends TimerTask {

    private static final SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm");
    private final URL availabilityURL;
    private final Integer eventId;
    
    public CollectAvailabilityTask(Integer eventId, URL availabilityURL) {
        this.availabilityURL = availabilityURL;
        this.eventId = eventId;
    }
    
    @Override
    public void run() {
        try {
            downloadAvailability(availabilityURL);
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    private void downloadAvailability(URL url) throws IOException {
        Date now = new Date();
        File dir = new File(Configuration.getArchiveDirectory(), eventId.toString());
        dir.mkdirs();
        File xmlFile = new File(dir, eventId + "_" + dateFormat.format(now) + ".xml");
        Collector.download(url, xmlFile);
    }
    
}
