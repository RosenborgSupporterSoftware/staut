package staut;

import java.io.File;
import java.io.IOException;
import java.net.URL;
import java.util.TimerTask;

/**
 * Thread that periodically downloads availability data for a specific event.
 */
public class CollectAvailabilityTask extends TimerTask {
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
        File dir = new File(Configuration.getArchiveDirectory(), eventId.toString());
        dir.mkdirs();
        File xmlFile = Collector.generateFileName(dir, eventId);
        Collector.download(url, xmlFile);
    }
    
}
