package staut;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.util.TimerTask;

/**
 * Thread that periodically downloads availability data for a specific event.
 */
public class CollectAvailabilityTask extends TimerTask {
    private final EventInfo info;
    
    public CollectAvailabilityTask(EventInfo info) {
        this.info = info;
    }
    
    private File getArchiveDirectory() {
        return new File(Configuration.getArchiveDirectory(), info.getSeason() + "_" + info.getEventId() + "_" + info.getOpponent());
    }
    
    public void writeEventInfo() {
        getArchiveDirectory().mkdirs();
        // Store the event info into a separate file
        File eventInfo = new File(getArchiveDirectory(), "eventinfo.properties");
        if(eventInfo.exists()) {
            STAut.info("File with event info already exists: " + eventInfo);
            return;
        }
        try (BufferedWriter writer = new BufferedWriter(new FileWriter(eventInfo))) {
            writer.write("eventname="+ info.getEventName());
            writer.newLine();
            writer.write("eventid=" + info.getEventId());
            writer.newLine();
            writer.write("opponent=" + info.getOpponent());
            writer.newLine();
            writer.write("eventdate=" + info.getEventDate());
            writer.newLine();
            writer.write("eventtime=" + info.getEventTime());
            writer.newLine();
            writer.write("availabilityurl=" + info.getAvailabilityURL());
            writer.newLine();
            writer.write("geometryurl=" + info.getGeometryURL());
            writer.newLine();
        } catch (IOException ioe) {
            STAut.error("Problem writing event info to file: " + eventInfo);
            ioe.printStackTrace();
        }
    }
    
    @Override
    public void run() {
        try {
            downloadAvailability(info);
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    private void downloadAvailability(EventInfo info) throws IOException {
        File xmlFile = Collector.generateFileName(getArchiveDirectory(), info);
        Collector.download(info.getAvailabilityURL(), xmlFile);
    }
    
}
