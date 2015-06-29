package staut;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.SortedMap;
import java.util.TreeMap;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import org.xml.sax.InputSource;

/**
 * STAut (SeteTellerAutomat) is the main class in the staut package. It is the
 * driver for decoding, inflating, parsing and reporting of geometry and 
 * availability data from events.
 * @author vemund
 */
public class STAut {
    private static File inputFile = null;
    private static File outputDir = null;
    private static String downloadMode = "next";
    private static String sectionName = null;
    private static String ticketType = null;
    
    private static boolean verbose = true;
    private static boolean printConfig = false;
    private static boolean dumpData = false;
    private static boolean collector = false;
    private static boolean download = false;
    private static boolean reportDiff = false;
    private static boolean reportIntersection = false;
    private static boolean reportSectionData = false;
    private static boolean reportSummary = false;
    private static boolean reportTicketData = false;
    private static boolean reportSeasonTickets = false;
    private static boolean reportOpen = false;
    private static boolean reportSold = false;
    private static boolean reportHold = false;
    private static boolean reportUnknown = false;
    
    private static final List<Stadium> stadiums = new ArrayList<>();
    
    /**
     * Handles decoding, inflating, parsing and reporting of geometry and
     * availability data from events, based on commandline arguments.
     * @param args the command line arguments
     * @throws Exception If there are any issues completing the requested 
     * operations.
     */
    public static void main(String[] args) throws Exception {
        parseCommandline(args);
        if(!validateArguments()) {
            usage(1);
        }
        if(printConfig) {
            printConfiguration();
            System.exit(0);
        }
        if(collector) {
            Collector.startCollector();
            System.exit(0);
        }
        
        if(inputFile != null) {
            Stadium lerkendal = new Stadium("Lerkendal");
            if(isCompressedXML(inputFile)) {
                info("Treating input file as base64 encoded and compressed data");
                String decoded = AvailabilityDecoder.decode(inputFile);
                if(outputDir != null) {
                    writeDecodedOutput(decoded, new File(outputDir, inputFile.getName()));
                }
                AvailabilityParser.parse(decoded, lerkendal);
            } else if (isDecodedXML(inputFile)) {
                info("Treating input file as decoded and uncompressed data");
                AvailabilityParser.parse(inputFile, lerkendal);
            } else {
                error("Cannot recognize data format of input file: " + inputFile);
                System.exit(1);
            }
            stadiums.add(lerkendal);
        }
        if(download) {
            List<EventInfo> infos = Collector.findActiveEvents();
            infos.sort(null);
            for(EventInfo info : infos) {
                Stadium lerkendal = new Stadium("Lerkendal");
                URL availabilityURL = info.getAvailabilityURL();
                File tmpFile = File.createTempFile(info.getEventId().toString(), null);
                Collector.download(availabilityURL, tmpFile);
                String decoded = AvailabilityDecoder.decode(tmpFile);
                if(outputDir != null) {
                    File file = Collector.generateFileName(outputDir, info);
                    writeDecodedOutput(decoded, file);
                }
                AvailabilityParser.parse(decoded, lerkendal);
                stadiums.add(lerkendal);
                if(downloadMode.equals("next")) {
                    break;
                }
            }
        }
        
        for(Stadium stadium : stadiums) {
            if(dumpData) {
                stadium.dump();
            }
            if(reportSectionData) {
                if(sectionName != null) {
                    stadium.reportSectionSummary(sectionName);
                } else {
                    stadium.reportSectionSummary();
                }
            }
            if(reportTicketData) {
                if(ticketType != null) {
                    stadium.reportTicketSummary(ticketType);
                } else {
                    stadium.reportTicketSummary();
                }
            }
            if(reportSeasonTickets) {
                STAut.info("Season ticket codes: " + Availability.getSeasonTicketCodes());
                stadium.reportSeasonTickets();
            }
            if(reportSummary) {
                stadium.reportSummary();
            }
            if(reportSold) {
                stadium.reportSold();
            }
            if(reportHold) {
                stadium.reportHold();
            }
            if(reportOpen) {
                stadium.reportOpen();
            }
            if(reportUnknown) {
                stadium.reportUnknown();
            }
        }
        
        if(reportDiff) {
            reportDiff();
        }
        
        if(reportIntersection) {
            reportIntersection();
        }
    }
    
    private static void writeDecodedOutput(String decoded, File file) throws IOException {
        info("Writing decoded data to file: " + file.getAbsolutePath());
        Files.write(file.toPath(), decoded.getBytes());
    }
    
    private static void parseCommandline(String[] args) {
        if(args.length == 0) {
            // Default behavior
            usage(0);
            System.exit(0);
        } else {
            for(String arg : args) {
                if (arg.equals("--collector")) {
                    collector=true;
                } else if (arg.equals("--config")) {
                    printConfig=true;
                } else if (arg.equals("--silent")) {
                    verbose=false;
                } else if (arg.startsWith("--download")) {
                        download=true;
                    if(arg.equals("--download-next" )) {
                        downloadMode = "next";
                    } else if(arg.equals("--download-all")) {
                        downloadMode = "all";
                    } else {
                        error("Download option should be either'--download-next' or '--download-all'");
                        usage(1);
                    }
                } else if (arg.equals("--dump")) {
                    dumpData=true;
                } else if (arg.startsWith("--input=")) {
                    String input=arg.split("=")[1];
                    inputFile = new File(input);
                } else if (arg.startsWith("--output=")) {
                    String output=arg.split("=")[1];
                    outputDir = new File(output);
                }  else if (arg.equals("--report-diff")) {
                    reportDiff=true;
                }  else if (arg.equals("--report-intersection")) {
                    reportIntersection=true;
                } else if (arg.equals("--report-section")) {
                    reportSectionData=true;
                } else if (arg.startsWith("--report-section=")) {
                    reportSectionData=true;
                    sectionName=arg.split("=")[1];
                }  else if (arg.equals("--report-summary")) {
                    reportSummary=true;
                }  else if (arg.equals("--report-season-tickets")) {
                    reportSeasonTickets=true;
                }  else if (arg.equals("--report-ticket")) {
                    reportTicketData=true;
                }  else if (arg.startsWith("--report-ticket=")) {
                    reportTicketData=true;
                    ticketType=arg.split("=")[1];
                }  else if (arg.equals("--report-sold")) {
                    reportSold=true;
                }  else if (arg.equals("--report-hold")) {
                    reportHold=true;
                }  else if (arg.equals("--report-open")) {
                    reportOpen=true;
                }  else if (arg.equals("--report-unknown")) {
                    reportUnknown=true;
                }  else {
                    System.err.println("Unknown argument: '" + arg + "'");
                    usage(1);
                }
                
            }
        }
    }
    
    private static void usage(int code) {
        System.out.println("--collector                 Start the automated data collector");
        System.out.println("--config                    List application configuration data");
        System.out.println("--download-next             download availability data for next active event");
        System.out.println("--download-all              download availability data for all active events");
        System.out.println("--dump                      Dump raw data on availability");
        System.out.println("--input=<file>              Load availability data from input file");
        System.out.println("--output=<directory>        Store decoded availability data in output directory");
        System.out.println("--report-diff               Print difference in ticktets for two event files");
        System.out.println("--report-intersection       Print intersection in tickets for two event files");
        System.out.println("--report-hold               Print data on seats with status 'hold'");
        System.out.println("--report-open               Print data on seats with status 'open'");
        System.out.println("--report-season-tickets     Print data on season tickets");
        System.out.println("--report-section[=<sec>]    Print ticket data sorted by stadium section [for section sec]");
        System.out.println("--report-sold               Print data on seats with status 'sold'");
        System.out.println("--report-summary            Print summary report");
        System.out.println("--report-ticket[=<ETT>]     Print section data sorted by ticket type [for specific ticket type ETT]");
        System.out.println("--report-unkown             Print data on seats with status 'unknown'");
        System.out.println("--silent                    No information printed, only report data");
        System.exit(code);
    }
    
    private static boolean validateArguments() {
        if(dumpData && !(inputFile != null || download)) {
            error("--dump specified without --input or --download");
            return false;
        }
        return true;
    }
    
    private static void printConfiguration() throws IOException {
        Configuration.dump();
    }
    
    private static boolean isCompressedXML(File xmlfile) throws Exception {
        return XMLContainsElement(xmlfile, "ev_comp");
    }
    
    private static boolean isDecodedXML(File xmlfile) throws Exception {
        return XMLContainsElement(xmlfile, "Grand_total_summary");
    }
    
    private static boolean XMLContainsElement(File xmlfile, String element) throws Exception {
        DocumentBuilder builder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        InputSource source = new InputSource(new InputStreamReader(new FileInputStream(xmlfile)));
        Document document = builder.parse(source);
        return (document.getElementsByTagName(element).getLength() >= 1);
        
    }
    
    protected static void report(String text) {
        System.out.println("REPORT: " + text);
    }
    
    protected static void info(String text) {
        if(verbose) {
            System.out.println("INFO: " + text);
        }
    }
    
    protected static void error(String text) {
        System.err.println("ERR: " + text);
    }    
    
    protected static void error(Throwable e) {
        e.printStackTrace(System.err);
    }
    
    private static void reportDiff() throws Exception {
        if(!(stadiums.size() > 1)) {
            error("Not enough datasets for comparison: " + stadiums.size());
            usage(1);
        }
        Stadium oldest = stadiums.get(0);
        Stadium next = stadiums.get(1);
        info("Comparing oldest dataset to later for " + stadiums.size() + " datasets");
        Map<Diff, Integer> total = new TreeMap<>();
        for(Section section: oldest.getSections()) {
            info("Section " + section.toString());
            Map<Diff, Integer> diffs = section.diff(next.getSection(section.getId()));
            for(Entry<Diff,Integer> diff : diffs.entrySet()) {
                report(diff.getKey() + ": " + diff.getValue());
                if(!total.containsKey(diff.getKey())) {
                    total.put(diff.getKey(), diff.getValue());
                } else {
                    total.put(diff.getKey(), total.get(diff.getKey()) + diff.getValue());
                }
            }
        }
        report("Total changed seats:");
        for(Entry<Diff,Integer> diff : total.entrySet()) {
            report(diff.getKey() + ": " + diff.getValue());
        }
    }
    
    private static void reportIntersection() throws Exception {
        if(!(stadiums.size() > 1)) {
            error("Not enough datasets for comparison: " + stadiums.size());
            usage(1);
        }
        Stadium oldest = stadiums.get(0);
        Stadium next = stadiums.get(1);
        info("Comparing oldest dataset to later for " + stadiums.size() + " datasets");
        SortedMap<Availability, Integer> total = new TreeMap<>();
        for(Section section: oldest.getSections()) {
            info("Section " + section.toString());
            SortedMap<Availability, Integer> tickets = section.intersection(next.getSection(section.getId()));
            for(Entry<Availability,Integer> ticket : tickets.entrySet()) {
                report(ticket.getKey() + ": " + ticket.getValue());
                if(!total.containsKey(ticket.getKey())) {
                    total.put(ticket.getKey(), ticket.getValue());
                } else {
                    total.put(ticket.getKey(), total.get(ticket.getKey()) + ticket.getValue());
                }
            }
        }
        
        report("Total unchanged seats:");
        for(Entry<Availability,Integer> ticket : total.entrySet()) {
            report(ticket.getKey() + ": " + ticket.getValue());
        }
    }
    
}
