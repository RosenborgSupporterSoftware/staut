package staut;

import java.util.HashSet;
import java.util.Map;
import java.util.Set;
import java.util.TreeMap;

/**
 * Class that contains information about a Section inside a Stadium.
 */
public class Section {
    
    private String id = null;
    private String name = null;
    private Set<Seat> seats = new HashSet<>();
    
    public Section(String id) {
        this.id = id;
    }
    
    public void setName(String name) {
        this.name = name;
    }
    
    public Set<Seat> getSeats() {
        return seats;
    }
    
    public Map<String,Integer> getQualifierCount() {
        Map<String,Integer> qualifierCount = new TreeMap<>();
        for(Seat seat: getSeats()) {
            String qualifier = seat.getQualifierBits();
            if(qualifierCount.containsKey(qualifier)) {
                qualifierCount.put(qualifier, qualifierCount.get(qualifier)+1);
            } else {
                qualifierCount.put(qualifier, 1);
            }
        }
        return qualifierCount;
    }
    
    public void addSeat(Seat s) throws Exception {
        if(seats.contains(s)) {
            System.out.println("Seat exists: " + s);
            throw new Exception("Seat already exists in this section");
        } else {
            s.setSection(this);
            seats.add(s);
        }
    }
    
    public Seat getSeat(int row, int place) throws Exception {
        for(Seat s: seats) {
            if (s.getRow() == row && s.getPlace() == place) {
                return s;
            }
        }
        Seat seat = new Seat(row, place);
        addSeat(seat);
        return seat;
    }
    
    public String getId() {
        return id;
    }
    
    public int countSeats() {
        return seats.size();
    }
    
    public int countSold() {
        int count = 0;
        count = seats.stream().filter((seat) -> (seat.isSold())).map((_item) -> 1).reduce(count, Integer::sum);
        return count;
    }
    
    public int countHold() {
        int count = 0;
        count = seats.stream().filter((seat) -> (seat.isHold())).map((_item) -> 1).reduce(count, Integer::sum);
        return count;
    }
    
    public int countOpen() {
        int count = 0;
        count = seats.stream().filter((seat) -> (seat.isOpen())).map((_item) -> 1).reduce(count, Integer::sum);
        return count;
    }
    
    @Override
    public String toString() {
        if (name != null) {
            return name;
        } else {
            return id;
        }
    }
    
    
}
