package staut;

import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.io.Reader;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;
import org.w3c.dom.Document;
import org.w3c.dom.NamedNodeMap;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.InputSource;
import org.xml.sax.SAXException;

/**
 * Parses an xmlfile containing geometry information for an event location.
 */
public class GeometryParser {
    private static final String ENCODING = "ISO-8859-1";

    public static void parse(File geometry, Stadium stadium) throws ParserConfigurationException, SAXException, IOException, Exception {
        DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();

        DocumentBuilder builder = factory.newDocumentBuilder();
        InputStream gs = new FileInputStream(geometry);
        Reader reader = new InputStreamReader(gs, ENCODING);
        InputSource source = new InputSource(reader);

        System.out.println("Parsing: " + geometry);
        Document document = builder.parse(source);

        NodeList nodeList = document.getDocumentElement().getChildNodes();

        for (int i = 0; i < nodeList.getLength(); i++) {
            Node node = nodeList.item(i);
            if (node.getNodeType() == Node.ELEMENT_NODE && node.getNodeName().equalsIgnoreCase("geometry_data")) {
                NodeList nodeList2 = node.getChildNodes();
                for (int j = 0; j < nodeList2.getLength(); j++) {
                    Node node2 = nodeList2.item(j);
                    if (node2.getNodeType() == Node.ELEMENT_NODE && node2.getNodeName().equalsIgnoreCase("page")) {
                        NodeList nodeList3 = node2.getChildNodes();
                        for(int k = 0 ; k < nodeList3.getLength() ; k++) {
                            Node node3 = nodeList3.item(k);
                            if (node3.getNodeType() == Node.ELEMENT_NODE && node3.getNodeName().equalsIgnoreCase("composite")) {
                                NodeList nodeList4 = node3.getChildNodes();
                                for(int l = 0 ; l<nodeList4.getLength() ; l++) {
                                    Node seats = nodeList4.item(l);
                                    if (seats.getNodeType() == Node.ELEMENT_NODE && seats.getNodeName().equalsIgnoreCase("n")) {
                                        NamedNodeMap map = seats.getAttributes();
                                        String from = map.getNamedItem("from").getNodeValue();
                                        int step = Math.abs(Integer.parseInt(map.getNamedItem("seat_step").getNodeValue()));
                                        String[] fromArray = from.split(",");
                                        int row = Integer.parseInt(fromArray[2]);
                                        int start = Integer.parseInt(fromArray[3]);
                                        int end = Integer.parseInt(fromArray[4]);
                                        int max = Math.max(start, end);
                                        int min = Math.min(start, end);
                                        Section section = stadium.getSection(fromArray[0]);
                                        for(int p = min ; p<=max ; p = p + step) {
                                            section.addSeat(new Seat(row,p));
                                        }
                                    } else if (seats.getNodeType() == Node.ELEMENT_NODE && seats.getNodeName().equalsIgnoreCase("u")) {
                                        NamedNodeMap map = seats.getAttributes();
                                        String from = map.getNamedItem("from").getNodeValue();
                                        String[] fromArray = from.split(",");
                                        int row = 0;
                                        if(!fromArray[1].isEmpty()) {
                                            row = Integer.parseInt(fromArray[1]);
                                        }
                                        String[] places = fromArray[2].split(";");
                                        Section section = stadium.getSection(fromArray[0]);
                                        for(String place: places) {
                                            section.addSeat(new Seat(row,Integer.parseInt(place)));
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        System.out.println("Parsing done");
    }
    
}
