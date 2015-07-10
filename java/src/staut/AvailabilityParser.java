/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package staut;

import java.io.File;
import java.io.StringReader;
import java.util.ArrayList;
import java.util.List;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;

/**
 *
 * @author vemund
 */
public class AvailabilityParser {
    private static final String EVENT_DETAILS = "Event_details";
    private static final String SECTION_ID_LIST = "Section_id_list";
    
    public static void parse(String decodedText, Stadium stadium) throws Exception {
        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        DocumentBuilder builder = factory.newDocumentBuilder();
        InputSource is = new InputSource(new StringReader(decodedText));
        Document document = builder.parse(is);
        parse(document, stadium);
    }
    
    public static void parse(File decodedFile, Stadium stadium) throws Exception {
        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
        DocumentBuilder builder = factory.newDocumentBuilder();
        STAut.info("Parsing: " + decodedFile);
        Document document = builder.parse(decodedFile);
        parse(document, stadium);
    }
    
    public static void parse(Document document, Stadium stadium) throws Exception {

        Node eventDetails = document.getDocumentElement().getElementsByTagName(EVENT_DETAILS).item(0);
        Node sectionIdList = document.getDocumentElement().getElementsByTagName(SECTION_ID_LIST).item(0);
        
        NodeList elements = sectionIdList.getChildNodes();
        
        for(int i=0 ; i<elements.getLength() ; i++) {
            NodeList sectionDetails = elements.item(i).getChildNodes();
            String sectionName = "";
            int row = 0;
            List<Integer> places = new ArrayList<>();
            List<String> codes = new ArrayList<>();
            for(int j=0 ; j<sectionDetails.getLength() ; j++) {
                Node sectionDetail = sectionDetails.item(j);
                if(sectionDetail.getNodeType() != Node.ELEMENT_NODE) {
                    continue;
                }
                if(sectionDetail.getNodeName().equals("Section_name")) {
                    sectionName = sectionDetail.getTextContent();
                }
                if(sectionDetail.getNodeName().equals("Row_names")) {
                    NodeList rows = sectionDetail.getChildNodes();
                    for(int r=0 ; r<rows.getLength() ; r++) {
                        if(rows.item(r).getNodeName().equals("E")) {
                            String rowString = rows.item(r).getTextContent().trim();
                            if(!rowString.isEmpty()) {
                                row = Integer.parseInt(rowString);
                            }
                        }
                    }
                }
                if(sectionDetail.getNodeName().equals("Seat_names")) {
                    Node seat = sectionDetail.getFirstChild();
                    do {
                        if(seat.getNodeName().equals("E")) {
                            places.add(Integer.parseInt(seat.getTextContent()));
                        }
                    } while((seat = seat.getNextSibling()) != null);
                }
                if(sectionDetail.getNodeName().equals("Esd")) {
                    Node code = sectionDetail.getFirstChild();
                    do {
                        if(code.getNodeName().equals("E")) {
                            String[] codeArray = code.getTextContent().split(",");
                            for(int c=0 ; c<Integer.parseInt(codeArray[1]) ; c++) {
                                codes.add(codeArray[0]);
                            }
                        }
                    } while ((code = code.getNextSibling()) != null);
                }
            }
            if(places.size() != codes.size()) {
                throw new Exception("Mismatch in number of places and codes for row: " + row + " in section " + sectionName);
            }
            Section section = stadium.getSection(sectionName);
            int counter = 0;
            for(int place : places) {
                Seat seat = section.getSeat(row, place);
                Availability availability = stadium.getAvailability(codes.get(counter));
                seat.setAvailability(availability);
                counter++;
            }
        }
    }
    
}
