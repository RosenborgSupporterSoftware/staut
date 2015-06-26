package staut;

/**
 * Class that contains information about a Seat in a Section in a Stadium.
 */
public class Seat implements Comparable {
    
    private final int row;
    private final int place;
    private Availability availability;
    private Section section;
    
    public Seat(int row, int place) {
        this.row = row;
        this.place = place;
    }
    
    public String getQualifierBits() {
        return availability.getQualifierBits();
    }
    
    public String getSeatFlag() {
        return availability.getSeatFlag();
    }
        
    public String getBaseType() {
        return availability.getBaseType();
    }
    
    public boolean isSoldOrHold() {
        return availability.isSoldOrHold();
    }
    
    public boolean isSold() {
        return availability.isSold();
    }
    
    public boolean isSeasonTicket() {
        return availability.isSeasonTicket();
    }
    
    public boolean isHold() {
        return availability.isHold();
    }
    
    public boolean isOpen() {
        return availability.isOpen();
    }
    
    public boolean isUnknown() {
        return availability.isUnknown();
    }
    
    @Override
    public boolean equals(Object other) {
        if (this == other) {
            return true;
        }
        if (!(other instanceof Seat)) {
            return false;
        }
        return (((Seat)other).row == this.row) && (((Seat)other).place == this.place);
    }

    @Override
    public int hashCode() {
        int hash = 7;
        hash = 37 * hash + this.row;
        hash = 37 * hash + this.place;
        return hash;
    }
    
    public int getRow() {
        return row;
    }
    
    public int getPlace() {
        return place;
    }
    
    @Override
    public int compareTo(Object o) {
        if (!(o instanceof Seat)) {
            return 1;
        } else {
            if(((Seat)o).row == this.row) {
                return ((Seat)o).place - this.place;
            } else {
                return ((Seat)o).row - this.row;
            }
        }
    }
    
    @Override
    public String toString() {
        return "(Row=" + row + ",place=" + place + ")";
    }

    /**
     * @return the availability
     */
    public Availability getAvailability() {
        return availability;
    }

    /**
     * @param availability the availability to set
     */
    public void setAvailability(Availability availability) {
        this.availability = availability;
        this.availability.addSeat(this);
    }

    /**
     * @return the section
     */
    public Section getSection() {
        return section;
    }

    /**
     * @param section the section to set
     */
    public void setSection(Section section) {
        this.section = section;
    }
    
}
