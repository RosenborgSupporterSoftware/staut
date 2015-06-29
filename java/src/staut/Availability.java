package staut;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;
import java.util.Map;
import java.util.Objects;
import java.util.TreeMap;

/**
 * This class contains information about a specific Extended Ticket Type that
 * defines the availability of tickets.
 */
public class Availability implements Comparable {
    
    // Final so equals() method has immutable behavior
    private final String code;
    
    private List<Seat> seats = new ArrayList<>();
    
    public Availability(String code) {
        this.code = code;
    }
    
    public void addSeat(Seat s) {
        seats.add(s);
    }
    
    public List<Seat> getSeats() {
        return seats;
    }
    
    public String getQualifierBits() {
        if(code.length() > 4) {
            return code.substring(0, code.length()-4);
        } else {
            return "00";
        }
    }
    
    public int getQualifierBitsInt() {
        return Integer.parseInt(getQualifierBits(), 16);
    }
    
    public String getSeatFlag() {
        if(code.length() >= 4) {
            return code.substring(code.length()-4, code.length()-2);
        } else {
            return "00";
        }
    }
    
    public int getSeatFlagInt() {
        return Integer.parseInt(getSeatFlag(), 16);
    }
        
    public String getBaseType() {
        if(code.length() > 2) {
            return code.substring(code.length()-2, code.length());
        } else {
            return code;
        }
    }
    
    public int getBaseTypeInt() {
        return Integer.parseInt(getBaseType(), 16);
    }
    
    public boolean isSoldOrHold() {
        return isSold() || isHold();
    }
    
    public boolean isSold() {
        return getBaseTypeInt() == 1 && 
                getQualifierBitsInt() >= 0x400 &&
                getQualifierBitsInt() <=  0x600;
    }
    
    public boolean isHold() {
        return getBaseTypeInt() == 1 && 
                getQualifierBitsInt() >= 0x00 &&
                getQualifierBitsInt() <=  0x30;
    }
    
    public boolean isOpen() {
        return getBaseTypeInt() == 0 || getBaseTypeInt() == 31;
    }
    
    public boolean isKnown() {
        return isOpen() || isSoldOrHold();
    }
    
    public boolean isUnknown() {
        return !isKnown();
    }
    
    /**
     * Attempt to gauge number of season tickets.
     * @return 
     */
    public boolean isSeasonTicket() {
        return getSeasonTicketCodes().contains(getQualifierBitsInt());
    }
    
    public static List<Integer> getSeasonTicketCodes() {
        List<Integer> ticketList = Arrays.asList(new Integer[]{
            // Liste hentet ut ved å finne koder 0x400 - 0x600 fra to siste kamper 2015 før de er lagt ut for salg
            0x4C7,  // Sesongkort gull voksen (jubajuba m.fl.)
            0x596,
            0x4CF,  // Sesongkort - Kjernen?
            0x4C8,  // Sesongkort voksen "svart" (FA) kontant (RoarO)
            0x5C2,
            0x4CD,  // Sesongkort familierabatt (hoboj0e)
            0x4CB,  // Sesongkort studentrabatt normal betaling (Kjello)
            0x4CE,
            0x4C9,  // Sesongkort hvit voksen
            0x590,
            0x4CC,  // Sesongkort hvit honnør/barn/student
            0x5C8,
            0x5C3,
            0x4CA,  // Sesongkort gull honnør/barn/student
            0x501,
            0x592,
            0x591,  // Sponsor-sesongkort o.l.
            0x4FA,
            0x4FB,  // Sesongkort "svart" (Brego)
            0x500,
            0x4FF,
            0x4D0,
            0x52C,
            0x595,
            0x4FD,  // Sesongkort studentrabatt avtalegiro (Kjello)
            0x4FC,  // Sesongkort (vegardj)
            0x502,
            0x4FE,
            0x593,
            0x5C4,
            0x5CC,
            0x4F9
        });
        ticketList.sort(null);
        return ticketList;
    }
    
    @Override
    public boolean equals(Object other) {
        if (this == other) {
            return true;
        }
        if (!(other instanceof Availability)) {
            return false;
        }
        return (((Availability)other).getBaseTypeInt() == this.getBaseTypeInt() && 
                ((Availability)other).getSeatFlagInt() == this.getSeatFlagInt() &&
                ((Availability)other).getQualifierBitsInt() == this.getQualifierBitsInt());
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 41 * hash + Objects.hashCode(this.code);
        return hash;
    }
    
    @Override
    public String toString() {
        return getQualifierBits() + getSeatFlag() + getBaseType();
    }

    @Override
    public int compareTo(Object o) {
        if(o instanceof Availability) {
            return Integer.parseInt(this.toString(),16) - Integer.parseInt(((Availability)o).toString(),16);
        } else {
            return 1;
        }
    }
    
    public Map<String,Integer> getSectionCount() {
        Map<String,Integer> sectionCount = new TreeMap<>();
        for(Seat seat: getSeats()) {
            String section = seat.getSection().getId();
            if(sectionCount.containsKey(section)) {
                sectionCount.put(section, sectionCount.get(section)+1);
            } else {
                sectionCount.put(section, 1);
            }
        }
        return sectionCount;
    }
}
