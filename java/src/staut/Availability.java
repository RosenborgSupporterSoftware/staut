package staut;

import java.util.ArrayList;
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
        if(isSold() || isHold()) {
            return true;
        } else {
            return false;
        }
    }
    
    public boolean isSold() {
        if(!isOpen() && !isHold()) {
            return true;
        } else {
            return false;
        }
    }
    
    public boolean isHold() {
        if(getBaseTypeInt() == 1 && getQualifierBitsInt() == 8) {
            return true;
        } else {
            return false;
        }
    }
    
    public boolean isOpen() {
        if(getBaseTypeInt() == 0 || getBaseTypeInt() == 31) {
            return true;
        } else {
            return false;
        }
    }
    
    /**
     * Attempt to gauge number of season tickets.
     * @return 
     */
    public boolean isSeasonTicket() {
        // TODO: This was just a guess, need to find correct criteria for
        // season tickets.
        if(Integer.parseInt(getQualifierBits().substring(0, 1)) == 4) {
            return true;
        } else {
            return false;
        }
    }
    
    @Override
    public boolean equals(Object other) {
        if (this == other) {
            return true;
        }
        if (!(other instanceof Availability)) {
            return false;
        }
        return (((Availability)other).code.equals(this.code));
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
