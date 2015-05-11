package staut;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
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
    private final static String PROPERTY_FILE_NAME = "configuration.properties";
    
    private static final Properties properties = new Properties();
    private static final File propertyFile = new File(PROPERTY_FILE_NAME);
    static {
        load();
    }
    
    private static void load() {
        System.out.println("Loading configuration from " + propertyFile.getAbsolutePath());
        try (InputStream in = new FileInputStream(propertyFile)) {
            properties.load(in);
        } catch(IOException ioe) {
            System.err.println("Error loading configuration from " + propertyFile);
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
        try (OutputStream out = new FileOutputStream(propertyFile)) {
            properties.store(out, "Configuration of SeteTellerAutomat");
        }
    }
    
    public static void dump() throws IOException {
        properties.list(System.out);
    }
    
    public static String getArchiveDirectory() {
        return getProperty(ARCHIVE_DIRECTORY);
    }
    
    public static Duration getCheckEventsPeriod() throws Exception {
        return getDuration(CHECK_EVENTS_PERIOD);
    }
    
    public static Duration getCollectAvailabilityPeriod() throws Exception {
        return getDuration(COLLECT_AVAILABILITY_PERIOD);
    }
    
    public static Duration getDuration(String property) throws Exception {
        String dur = properties.getProperty(property);
        if(dur != null) {
            Duration duration = Duration.parse(dur);
            return duration;
        } else {
            throw new Exception("Invalid setting for " + property + ":" + dur);
        }
        
    }
    
}
