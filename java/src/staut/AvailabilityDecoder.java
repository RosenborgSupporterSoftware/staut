package staut;

import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.util.ArrayList;
import java.util.Base64;
import java.util.List;
import java.util.zip.Inflater;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import org.w3c.dom.Document;
import org.xml.sax.InputSource;

/**
 * AvailabilityDecoder
 * Decodes a downloaded xmlfile containing an ev_comp element with base64
 * encoded and zlib deflated text content and writes the result to an xmlfile
 */
public class AvailabilityDecoder {
    
    /**
     * Extracts the content of xml element ev_comp and returns it as a String.
     * @param xmlfile File to read from.
     * @return String contining content of ev_comp element.
     * @throws Exception If there is a problem extracting the String.
     */
    private static String extractFromXml(File xmlfile) throws Exception {
        DocumentBuilder builder = DocumentBuilderFactory.newInstance().newDocumentBuilder();
        InputSource source = new InputSource(new InputStreamReader(new FileInputStream(xmlfile)));
        Document document = builder.parse(source);
        return document.getElementsByTagName("ev_comp").item(0).getTextContent();
    }

    /**
     * Decodes a downloaded xmlfile containing an ev_comp element with base64
     * encoded and zlib deflated text content and writes the result to an xmlfile
     * @param xmlfile Path to the file to decode
     * @return 
     * @throws Exception If unable to complete the decoding or inflating of
     * the target file.
     */
    public static String decode(File xmlfile) throws Exception {
        String ev_comp = extractFromXml(xmlfile);
        InputStream stream = new ByteArrayInputStream(ev_comp.getBytes());
        InputStream decodedStream = Base64.getDecoder().wrap(stream);
        return "<xml>"+ ran(decodedStream) + "</xml>";
    }
    
    /**
     * Emulates the ran() method from the Globals class in ISCApp.swf
     * @param stream An InputStream that a base64 encoded byte stream can be 
     * read from.
     * @throws IOException If there are problems decoding or deflating the 
     * byte stream.
     */
    private static String ran(InputStream byteStream) throws IOException {
        int numberBytes;
        int OM;
        int STEP;
        int VSIZE;
        int sCompress;
        List<Integer> V = new ArrayList<>();
        int ssa;
        int es;
        int mes;
        int us;
        int currByte;
        
        numberBytes = byteStream.read();
        OM = byteStream.read();
        STEP = byteStream.read();
        VSIZE = byteStream.read();
        sCompress = byteStream.read();
        V.add(165);
        for(int i=0 ; i < VSIZE - 1 ; i++) {
            V.add(byteStream.read());
        }
        
        List<Integer> sd = new ArrayList<>();
        int nextByte;
        while((nextByte = byteStream.read()) != -1) {
            sd.add(nextByte);
        }
        int position = 0;
        do {
            ssa = sCompress;
            ssa = ((ssa * V.get(0)) & 4194303);
            for(int j=1 ; j < (V.size() - 1) ; j++) {
                es = V.get(j);
                ssa = (((ssa + es) * sCompress) & 4194303);
            }
            ssa = (ssa + V.get(V.size() - 1));
            mes = ((ssa >> 12) & 0xFF);
            us = (ssa % V.size());
            V.set(us,(V.get(us) ^ sCompress));
            currByte = sd.get(position);
            currByte = (currByte ^ mes);
            sd.set(position, currByte);
            sCompress = currByte;
            position = (position + STEP);
//            System.out.println("Position: " + position);
        } while (position<sd.size());
        return inflate(sd);
    }
    
    /**
     * Inflates a List of bytes (Integer) into a text string using zlib
     * algorithm.
     * @param compressed The List of bytes (Integer) to inflate.
     * @return String containing the inflated text.
     * @throws IOException if there are problems inflating the binary code.
     */
    private static String inflate(List<Integer> compressed) throws IOException {
        String outputString = "";
        try {
            int compressedLength = compressed.size();
            byte[] output = new byte[compressedLength];
            int counter=0;
            for(int comp : compressed) {
                output[counter] = (byte)comp;
                counter++;
            }

            // Decompress the bytes
            Inflater decompresser = new Inflater();
            decompresser.setInput(output, 0, compressedLength);
            do {
                byte[] result = new byte[2000000];
                int resultLength = decompresser.inflate(result);
                // Decode the bytes into a String
                outputString += new String(result, 0, resultLength, "UTF-8");
            } while(decompresser.getRemaining() > 0);
            decompresser.end();
            return outputString;
        } catch( java.io.UnsupportedEncodingException | java.util.zip.DataFormatException ex) {
            throw new IOException("Failed to inflate compressed data", ex);
        }  
    }
    
}
