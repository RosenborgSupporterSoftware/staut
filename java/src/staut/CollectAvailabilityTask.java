package staut;

import java.io.BufferedWriter;
import java.io.File;
import java.io.FileWriter;
import java.io.IOException;
import java.nio.file.Files;
import java.util.TimerTask;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 * Thread that periodically downloads availability data for a specific event.
 */
public class CollectAvailabilityTask extends TimerTask {
    private final EventInfo info;
    
    public CollectAvailabilityTask(EventInfo info) {
        this.info = info;
    }
    
    private File getArchiveDirectory() {
        return new File(Configuration.getArchiveDirectory(), 
                info.getSeason() + "_" + 
                info.getCompetition() + "_" +
                info.getRound() + "_" +
                info.getOpponent() + "_" + 
                info.getEventId()
        );
    }
    
    private File getFailedDownloadsDirectory() {
        return new File(Configuration.getArchiveDirectory(),"Failed_Downloads");
    }
    
    public void writeEventInfo() throws IOException {
        Files.createDirectories(getArchiveDirectory().toPath());
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
            writer.write("eventcode=" + info.getEventCode());
            writer.newLine();
            writer.write("location=" + info.getLocation());
            writer.newLine();
            writer.write("competition=" + info.getCompetition());
            writer.newLine();
            writer.write("round=" + info.getRound());
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
        int retryCount = 0;
        int size;
        do {
            if(retryCount>Configuration.getDownloadRetryCount()) {
                // We give up
                STAut.error("Retried download to " + xmlFile + " " + Configuration.getDownloadRetryCount() 
                        + " times without success, giving up for now.");
                getFailedDownloadsDirectory().mkdirs();
                Files.move(xmlFile.toPath(), new File(getFailedDownloadsDirectory(),xmlFile.getName()).toPath());
                return;
            } else {
                if(retryCount > 0) {
                    try {
                        Thread.sleep(Configuration.getDownloadRetryWait().toMillis());
                    } catch (InterruptedException ex) {
                        // Interrupted, we don't care.
                    }
                    xmlFile.delete();
                }
                retryCount++;
            }
            size = Collector.download(info.getAvailabilityURL(), xmlFile);
        } while (!validateDownload(xmlFile, size));
        STAut.info("Validated downloaded file: " + xmlFile);
    }
    
    private boolean validateDownload(File xmlFile, int size) {
        if(size == 0) { // Empty file download
            STAut.error("Downloaded file " + xmlFile + " is empty.");
            return false;
        }
        try {
            if(STAut.isErrorXML(xmlFile)) {
                String error = extractErrorFromXml(xmlFile);
                STAut.error("Got error when collecting availability XML " + xmlFile + ": " + error);
                return false;
            } 
        } catch (Exception ex) {
            STAut.error("Error parsing error messages in downloaded file: " + ex.getMessage());
            return false;
        }
        try {
            String decoded = AvailabilityDecoder.decode(xmlFile);
            AvailabilityParser.parse(decoded, new Stadium("dummy"));
        } catch (Exception e) {
            STAut.error("Downloaded file " + xmlFile + " could not be decoded and parsed: " + e.getMessage());
            // Decoding and parsing failed, likely downloaded file is broken in some way
            return false;
        }
        return true;
    }
 
    private static String extractErrorFromXml(File xmlFile) {
        return AvailabilityDecoder.extractFromXml(xmlFile, "error");
    }
    
}
