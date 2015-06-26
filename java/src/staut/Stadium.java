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
    private final Map<String,Section> sections = new TreeMap<>();
    private final Set<Availability> availabilities = new TreeSet<>();
    
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
        STAut.report("Reporting summary data:");
        int seats = 0;
        int sold = 0;
        int hold = 0;
        int open = 0;
        int seasonTickets = 0;
        int unknown = 0;
        for(Section section : sections.values()) {
            sold+=section.countSold();
            hold+=section.countHold();
            open+=section.countOpen();
            seats+=section.countSeats();
            seasonTickets+=section.countSeasonTickets();
            unknown+=section.countUnknown();
        }
        STAut.report("Total seats: " + seats);
        STAut.report("Total sold: " + sold);
        STAut.report("  Total regular tickets: " + (sold-seasonTickets));
        STAut.report("  Total season tickets: " + seasonTickets);
        STAut.report("Total hold: " + hold);
        STAut.report("Total open: " + open);
        STAut.report("Total unknown: " + unknown);
    }
    
    public void reportSectionSummary() {
        STAut.report("Reporting summary for all sections.");
        for(Section section : sections.values()) {
            reportSectionSummary(section);
        }
    }
    
    public void reportSectionSummary(String sectionName) {
        for(Section section : sections.values()) {
            if(section.getId().equals(sectionName)) {
                reportSectionSummary(section);
                return;
            }
        }
        STAut.error("Unknown section: " + sectionName);
    }
    
    public void reportSectionSummary(Section section) {
        STAut.report("Summary for section " + section + ":");
        for(Entry<String,Integer> entry : section.getQualifierCount().entrySet()) {
            STAut.report("Seats with ticket type " + entry.getKey() + ": " + entry.getValue());
        }
    }
    
    public void reportTicketSummary() {
        STAut.report("Reporting summary for all ticket types.");
        for(Availability availability : availabilities) {
            reportTicketSummary(availability);
        }
    }
    
    public void reportTicketSummary(Availability availability) {
        STAut.report("Summary for ticket type " + availability + ":");
        for(Entry<String,Integer> entry : availability.getSectionCount().entrySet()) {
            STAut.report("Seats in section " + entry.getKey() + ": " + entry.getValue());
        }
    }
    
    public void reportTicketSummary(String ticketType) {
        Availability compare = new Availability(ticketType);
        for(Availability availability : availabilities) {
            if(availability.equals(compare)) {
                reportTicketSummary(availability);
                return;
            }
        }
        STAut.error("Unused ticket type: " + ticketType);
    }
    
    public void reportOpen() {
        int totalOpen = 0;
        STAut.report("Reporting data on seats with status 'open'.");
        for(Section section: sections.values()) {
            int open = section.countOpen();
            if(open >0) {
                totalOpen += open;
                System.out.println(section.toString() + ": " + open);
            }
        }
        STAut.report("Total: " + totalOpen);
    }
    
    public void reportUnknown() {
        int totalUnknown = 0;
        STAut.report("Reporting data on seats with status 'unknown'.");
        for(Section section: sections.values()) {
            int unknown = section.countUnknown();
            if(unknown >0) {
                totalUnknown += unknown;
                System.out.println(section.toString() + ": " + unknown);
            }
        }
        STAut.report("Total: " + totalUnknown);
    }
    
    public void reportSold() {
        int totalSold = 0;
        STAut.report("Reporting data on seats with status 'sold'.");
        for(Section section: sections.values()) {
            int sold = section.countSold();
            if(sold >0) {
                totalSold += sold;
                System.out.println(section.toString() + ": " + sold);
            }
        }
        STAut.report("Total: " + totalSold);
    }
        
    public void reportHold() {
        int totalHold = 0;
        STAut.report("Reporting data on seats with status 'hold'.");
        for(Section section: sections.values()) {
            int hold = section.countHold();
            if(hold >0) {
                totalHold += hold;
                System.out.println(section.toString() + ": " + hold);
            }
        }
        STAut.report("Total: " + totalHold);
    }
    
    public void reportSeasonTickets() {
        int total = 0;
        for(Availability availability : availabilities) {
            if(availability.isSeasonTicket()) {
                STAut.report("Availability: " + availability);
                int availabilitySum = 0;
                for(Entry<String,Integer> entry : availability.getSectionCount().entrySet()) {
                    availabilitySum+=entry.getValue();
                    STAut.report("Section: " + entry.getKey() + " count: " + entry.getValue());
                }
                STAut.report("Total: " + availabilitySum);
                total+=availabilitySum;
            }
        }
        STAut.report("Grand total: " + total);
    }
    
    public void dump() {
        for (Section section : sections.values()) {
            System.out.println(section + ": " + section.countSeats());
        }
        STAut.report("Total: " + totalCount());
    }
    
    public int totalCount() {
        int total = 0;
        for (Section section: sections.values()) {
            total += section.countSeats();
        }
        return total;
    }
    
}
