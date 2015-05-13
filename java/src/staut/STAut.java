package staut;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStreamReader;
import java.net.URL;
import java.nio.file.Files;
import java.util.ArrayList;
import java.util.List;
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

    private static String geometryPath;
    
    private static File inputFile = null;
    private static File outputDir = null;
    private static String downloadMode = "next";
    
    private static boolean printConfig = false;
    private static boolean dumpData = false;
    private static boolean collector = false;
    private static boolean download = false;
    private static boolean reportSectionData = false;
    private static boolean reportSummary = false;
    private static boolean reportTicketData = false;
    
    private static List<Stadium> stadiums = new ArrayList<>();
    
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
            usage();
            System.exit(1);
        }
        if(printConfig) {
            printConfiguration();
        }
        if(collector) {
            Collector.startCollector();
            System.exit(0);
        }
        
        if(inputFile != null) {
            Stadium lerkendal = new Stadium("Lerkendal");
            if(isCompressedXML(inputFile)) {
                System.out.println("Treating input file as base64 encoded and compressed data");
                String decoded = AvailabilityDecoder.decode(inputFile);
                if(outputDir != null) {
                    writeDecodedOutput(decoded, new File(outputDir, inputFile.getName()));
                }
                AvailabilityParser.parse(decoded, lerkendal);
            } else if (isDecodedXML(inputFile)) {
                System.out.println("Treating input file as decoded and uncompressed data");
                AvailabilityParser.parse(inputFile, lerkendal);
            } else {
                System.err.println("Cannot recognize data format of input file: " + inputFile);
                System.exit(1);
            }
            stadiums.add(lerkendal);
        } else if(download) {
            List<Integer> eventIds = Collector.findActiveEvents();
            eventIds.sort(null);
            for(Integer id : eventIds) {
                Stadium lerkendal = new Stadium("Lerkendal");
                URL availabilityURL = Collector.extractAvailabilityURL(id);
                File tmpFile = File.createTempFile(id.toString(), null);
                Collector.download(availabilityURL, tmpFile);
                String decoded = AvailabilityDecoder.decode(tmpFile);
                if(outputDir != null) {
                    File file = Collector.generateFileName(outputDir, id);
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
                stadium.reportSectionSummary();
            }
            if(reportTicketData) {
                stadium.reportTicketSummary();
            }
            if(reportSummary) {
                stadium.reportSummary();
            }
        }
    }
    
    private static void writeDecodedOutput(String decoded, File file) throws IOException {
        System.out.println("Writing decoded data to file: " + file.getAbsolutePath());
        Files.write(file.toPath(), decoded.getBytes());
    }
    
    private static void parseCommandline(String[] args) {
        if(args.length == 0) {
            // Default behavior
            usage();
            System.exit(0);
        } else {
            for(String arg : args) {
                if (arg.equals("--collector")) {
                    collector=true;
                } else if (arg.equals("--config")) {
                    printConfig=true;
                } else if (arg.startsWith("--download")) {
                    if(arg.equals("--download" )) {
                        download=true;
                    } else if(arg.startsWith("--download=")) {
                        download=true;
                        String mode=arg.split("=")[1];
                        if(!(mode.equals("next") || mode.equals("all"))) {
                            System.err.println("Download mode should be either'next' or 'all'");
                            System.exit(1);
                        } else {
                            downloadMode = mode;
                        }
                    } else {
                        System.err.println("Wrong syntax for --download option");
                        usage();
                    }
                } else if (arg.equals("--dump")) {
                    dumpData=true;
                } else if (arg.startsWith("--input=")) {
                    String input=arg.split("=")[1];
                    inputFile = new File(input);
                } else if (arg.startsWith("--output=")) {
                    String output=arg.split("=")[1];
                    outputDir = new File(output);
                } else if (arg.equals("--report_section_data")) {
                    reportSectionData=true;
                }  else if (arg.equals("--report_summary")) {
                    reportSummary=true;
                }  else if (arg.equals("--report_ticket_data")) {
                    reportTicketData=true;
                }  else {
                    System.err.println("Unknown argument: '" + arg + "'");
                    usage();
                }
                
            }
        }
    }
    
    private static void usage() {
        System.out.println("--collector                 Start the automated data collector");
        System.out.println("--config                    List application configuration data");
        System.out.println("--download[=<next|all>]     download availability data for next (default) or all active events");
        System.out.println("--dump                      Dump raw data on availability");
        System.out.println("--input=<file>              Load availability data from input file");
        System.out.println("--output=<file>             Store decoded availability data in output file");
        System.out.println("--report_section_data       Print ticket data sorted by stadium section");
        System.out.println("--report_summary            Print summary report");
        System.out.println("--report_ticket_data        Print section data sorted by ticket type");
        System.exit(0);
    }
    
    private static boolean validateArguments() {
        if(download && inputFile != null) {
            System.err.println("Cannot use both --input and --download options for event data.");
            return false;
        }
        if(dumpData && !(inputFile != null || download)) {
            System.out.println("--dump specified without --input or --download");
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
    
}
