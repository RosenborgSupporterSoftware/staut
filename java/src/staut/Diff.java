package staut;

import java.util.Objects;

/**
 * This class contains information about availability difference between to 
 * ticket counts for a specific seat.
 */
public class Diff implements Comparable {
    private final Availability oldTicket;
    private final Availability newTicket;
    
    public Diff(Availability oldTicket, Availability newTicket) {
        this.oldTicket = oldTicket;
        this.newTicket = newTicket;
    }
    
    @Override
    public String toString() {
        if(getOldTicket().equals(getNewTicket())) {
            return "unchanged";
        } else {
            return getOldTicket() + " -> " + getNewTicket();
        }
    }

    /**
     * @return the oldTicket
     */
    public Availability getOldTicket() {
        return oldTicket;
    }
    
    /**
     * @return the newTicket
     */
    public Availability getNewTicket() {
        return newTicket;
    }

    @Override
    public int compareTo(Object o) {
        if(!this.equals(o)) {
            return -1;
        } else {
            return 0;
        }
    }
        
    @Override
    public boolean equals(Object o) {
        if(o instanceof  Diff) {
            return ((Diff)o).oldTicket.equals(oldTicket) && ((Diff)o).newTicket.equals(this.newTicket);
        } else {
            return false;
        }
    }

    @Override
    public int hashCode() {
        int hash = 5;
        hash = 19 * hash + Objects.hashCode(this.oldTicket);
        hash = 19 * hash + Objects.hashCode(this.newTicket);
        return hash;
    }
    
}
