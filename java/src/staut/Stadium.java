package staut;

import java.util.Collection;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;
import java.util.TreeMap;
import java.util.TreeSet;

/**
 * Class that contains information about a Stadium for events.
 */
public class Stadium {
    
    private final String name;
    private Map<String,Section> sections = new TreeMap<>();
    private Set<Availability> availabilities = new TreeSet<>();
    
    public Stadium(String name) {
        this.name = name;
    }
    
    public String getName() {
        return name;
    }
    
    public void addSection(Section s) {
        sections.put(s.getId(),s);
    }
    
    public Collection<Section> getSections() {
        return sections.values();
    }
    
    public void addAvailability(Availability a) {
        availabilities.add(a);
    }
    
    public Set<Availability> getAvailabilities() {
        return availabilities;
    }
    
    public Section getSection(String id) {
        if(sections.containsKey(id)) {
            return sections.get(id);
        } else {
            Section sec = new Section(id);
            sections.put(id, sec);
            return sec;
        }
    }
    
    public Availability getAvailability(String code) throws Exception {
        Availability availability = new Availability(code);
        if(availabilities.contains(availability)) {
            for(Availability a : availabilities) {
                if(a.equals(availability)) {
                    return a;
                }
            }
            throw new Exception("Failed looking up availability object");
        } else {
            availabilities.add(availability);
            return availability;
        }
    }
    
    public void reportSummary() {
        int seats = 0;
        int sold = 0;
        int hold = 0;
        int open = 0;
        for(Section section : sections.values()) {
            sold+=section.countSold();
            hold+=section.countHold();
            open+=section.countOpen();
            seats+=section.countSeats();
        }
        System.out.println("Total seats: " + seats);
        System.out.println("Total sold: " + sold);
        System.out.println("Total hold: " + hold);
        System.out.println("Total open: " + open);
    }
    
    public void reportSectionSummary() {
        for(Section section : sections.values()) {
            System.out.println("Section " + section);
            for(Entry<String,Integer> entry : section.getQualifierCount().entrySet()) {
                System.out.println("Code: " + entry.getKey() + " count: " + entry.getValue());
            }
        }
    }
    
    public void reportTicketSummary() {
        for(Availability availability : availabilities) {
            System.out.println("Availability " + availability);
            for(Entry<String,Integer> entry : availability.getSectionCount().entrySet()) {
                System.out.println("Section: " + entry.getKey() + " count: " + entry.getValue());
            }
        }
    }
    
    public void reportSeasonTickets() {
        int total = 0;
        for(Availability availability : availabilities) {
            if(availability.isSeasonTicket()) {
                System.out.println("Availability " + availability);
                int availabilitySum = 0;
                for(Entry<String,Integer> entry : availability.getSectionCount().entrySet()) {
                    availabilitySum+=entry.getValue();
                    System.out.println("Section: " + entry.getKey() + " count: " + entry.getValue());
                }
                System.out.println("Sum: " + availabilitySum);
                total+=availabilitySum;
            }
        }
        System.out.println("Grand total: " + total);
    }
    
    public void dump() {
        for (Section section : sections.values()) {
            System.out.println(section + ": " + section.countSeats());
        }
        System.out.println("Total: " + totalCount());
    }
    
    public int totalCount() {
        int total = 0;
        for (Section section: sections.values()) {
            total += section.countSeats();
        }
        return total;
    }
    
}
