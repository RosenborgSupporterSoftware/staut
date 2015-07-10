package staut;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URISyntaxException;
import java.text.ParseException;
import java.time.Duration;
import java.util.Properties;

/**
 * This class maintains the configuration of the staut package. It reads and
 * writes the configuration data to/from a properties file.
 */
public class Configuration {
    public final static String ARCHIVE_DIRECTORY = "archive.directory";
    public final static String CHECK_EVENTS_PERIOD = "check.events.period";
    public final static String COLLECT_AVAILABILITY_PERIOD = "collect.availability.period";
    public final static String DOWNLOAD_RETRY_COUNT = "download.retry.count";
    public final static String DOWNLOAD_RETRY_WAIT = "download.retry.wait";
    private final static String PROPERTY_FILE_NAME = "configuration.properties";
    
    private static final Properties properties = new Properties();
    
    static {
        load();
    }
    
    private static File getPropertyFile() {
        try {
            return new File(new File(Configuration.class.getProtectionDomain().getCodeSource().getLocation().toURI().getPath()).getParentFile(),PROPERTY_FILE_NAME);
        } catch (URISyntaxException ex) {
            STAut.error("Could not find path to jar file");
            return null;
        }
    }
    
    private static void load() {
        File propfile = getPropertyFile();
        STAut.info("Loading configuration from " + propfile.getAbsolutePath());
        try (InputStream in = new FileInputStream(propfile)) {
            properties.load(in);
        } catch(IOException ioe) {
            STAut.error("Error loading configuration from " + propfile);
            ioe.printStackTrace();
        }
    }
    
    public static void setProperty(String key, String value) throws IOException {
        properties.setProperty(key, value);
        save();
    }
    
    public static String getProperty(String key) {
        return properties.getProperty(key);
    }
    
    private static void save() throws IOException {
        try (OutputStream out = new FileOutputStream(getPropertyFile())) {
            properties.store(out, "Configuration of SeteTellerAutomat");
        }
    }
    
    public static void dump() throws IOException {
        properties.list(System.out);
    }
    
    public static String getArchiveDirectory() {
        return getProperty(ARCHIVE_DIRECTORY);
    }
    
    public static Duration getCheckEventsPeriod() throws ParseException {
        return getDuration(CHECK_EVENTS_PERIOD);
    }
    
    public static Duration getCollectAvailabilityPeriod() throws ParseException {
        return getDuration(COLLECT_AVAILABILITY_PERIOD);
    }
    
    public static Duration getDownloadRetryWait() {
        try {
            return getDuration(DOWNLOAD_RETRY_WAIT);
        } catch (ParseException pe) {
            STAut.error("Could not parse property '" + DOWNLOAD_RETRY_WAIT + "': " + getProperty(DOWNLOAD_RETRY_WAIT));
            return Duration.ZERO;
        }
    }
    
    public static int getDownloadRetryCount() {
        return Integer.parseInt(getProperty(DOWNLOAD_RETRY_COUNT));
    }
    
    public static Duration getDuration(String property) throws ParseException {
        String dur = properties.getProperty(property);
        if(dur != null) {
            Duration duration = Duration.parse(dur);
            return duration;
        } else {
            throw new ParseException("Invalid setting for " + property + ":" + dur,0);
        }
        
    }
    
}
