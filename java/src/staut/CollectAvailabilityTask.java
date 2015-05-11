package staut;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.URL;
import java.net.URLConnection;
import java.text.SimpleDateFormat;
import java.util.Date;
import java.util.TimerTask;

/**
 * Thread that periodically downloads availability data for a specific event.
 * @author vemund
 */
public class CollectAvailabilityTask extends TimerTask {

    private static final SimpleDateFormat dateFormat = new SimpleDateFormat("yyyy-MM-dd'T'HH:mm");
    private final URL availabilityURL;
    private final String name;
    
    public CollectAvailabilityTask(String name, URL availabilityURL) {
        this.availabilityURL = availabilityURL;
        this.name = name;
    }
    
    @Override
    public void run() {
        try {
            downloadAvailability();
        } catch (IOException ex) {
            ex.printStackTrace();
        }
    }
    
    private void downloadAvailability() throws IOException {
        URLConnection uc = availabilityURL.openConnection();
        Date now = new Date();
        File dir = new File(Configuration.getArchiveDirectory(), name);
        dir.mkdirs();
        File xmlFile = new File(dir, name + "_" + dateFormat.format(now) + ".xml");
        System.out.println("Downloading into: " + xmlFile);
        try (InputStream in = uc.getInputStream(); OutputStream outStream = new FileOutputStream(xmlFile)) {
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
        System.out.println("Finished downloading: " + xmlFile);
    }
    
}
