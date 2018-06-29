package staut;

import java.net.URL;

/**
 * This class contains event information exctracted from ticketing website: 
 * Opponent, event date and time, event ID, URLs for ticketing data, etc.
 */
public class EventInfo implements Comparable {
    
    public final static String LEAGUE_COMPETITION = "LEAGUE";
    public final static String CUP_COMPETITION = "NM";
    public final static String EC_COMPETITION = "EC";
    public final static String EL_COMPETITION = "EL";
    public final static String CL_COMPETITION = "CL";
    public final static String UEFA_QUALIFICATION = "Q";
    
    private final int eventId;
    private String eventName;
    private String eventDate;
    private String eventTime;
    private String location;
    private String competition;
    private int round;
    private String eventCode;
    private URL availabilityURL;
    private URL geometryURL;
    private URL eventURL;
    
    public EventInfo(int id) {
        this.eventId = id;
    }
    
    /**
     * @return the eventId
     */
    public Integer getEventId() {
        return eventId;
    }

    public String getSeason() {
        if(eventDate != null) {
            return eventDate.split("\\.")[2];
        } else {
            return null;
        }
    }
    
    /**
     * @return the eventDate
     */
    public String getEventDate() {
        return eventDate;
    }

    /**
     * @param eventDate the eventDate to set
     */
    public void setEventDate(String eventDate) {
        this.eventDate = eventDate;
    }
    
    /**
     * @return the opponent
     */
    public String getOpponent() {
        if(isLeagueGame()) {
            // Return everything from eventname after the -
            return eventName.split("-")[1].trim();
        } else {
            if(eventName.contains("(")) { 
                // We assume some extra information is added to event name, remove this
                return eventName.split("-")[1].trim().split("\\(")[0].trim();
            } else {
                // Return the first word from eventname after the -
                return eventName.split("-")[1].trim().split("\\s+")[0];
            }
        }
    }
    
    /**
     * @return the opponent, but replacing / with empty string.
     * Used for generating file names that will work on Windows.
     */
    public String getOpponentWithoutSlash() {
        String opponent = getOpponent();
        return opponent.replaceAll("/", "");
    }

    /**
     * @return the availabilityUrl
     */
    public URL getAvailabilityURL() {
        return availabilityURL;
    }

    /**
     * @param availabilityURL the availabilityURL to set
     */
    public void setAvailabilityURL(URL availabilityURL) {
        this.availabilityURL = availabilityURL;
    }

    /**
     * @return the geometryURL
     */
    public URL getGeometryURL() {
        return geometryURL;
    }

    /**
     * @param geometryURL the geometryURL to set
     */
    public void setGeometryURL(URL geometryURL) {
        this.geometryURL = geometryURL;
    }

    /**
     * @return the eventName
     */
    public String getEventName() {
        return eventName;
    }

    /**
     * @param eventName the eventName to set
     */
    public void setEventName(String eventName) {
        this.eventName = eventName;
    }

    /**
     * @return the eventTime
     */
    public String getEventTime() {
        return eventTime;
    }

    /**
     * @param eventTime the eventTime to set
     */
    public void setEventTime(String eventTime) {
        this.eventTime = eventTime;
    }

    @Override
    public int compareTo(Object o) {
        if(o instanceof EventInfo) {
            return ((EventInfo)o).getEventId() - this.getEventId();
        } else {
            return -1;
        }
    }

    public String detailedString() {
        StringBuilder sb = new StringBuilder();
        sb.append("ID: ").append(eventId).append("\n");
        sb.append("Name: ").append(eventName).append("\n");
        sb.append("Date: ").append(eventDate).append("\n");
        sb.append("Time: ").append(eventTime).append("\n");
        sb.append("Location: ").append(location).append("\n");
        sb.append("Competition: ").append(competition).append("\n");
        sb.append("Round: ").append(round).append("\n");
        sb.append("Code: ").append(eventCode).append("\n");
        sb.append("AvailabilityURL: ").append(availabilityURL).append("\n");
        sb.append("GeometryURL: ").append(geometryURL).append("\n");
        sb.append("EventURL: ").append(eventURL);
        return sb.toString();
    }
    
    @Override
    public String toString() {
        if(eventName != null && eventDate != null) {
            return eventName + " " + eventDate;
        } else {
            return "eventId: " + eventId;
        }
    }

    @Override
    public int hashCode() {
        int hash = 3;
        hash = 59 * hash + this.eventId;
        return hash;
    }
    
    @Override
    public boolean equals(Object o) {
        if(o instanceof EventInfo) {
            return ((EventInfo)o).eventId == this.eventId;
        } else {
            return false;
        }
    }

    /**
     * @return the eventCode
     */
    public String getEventCode() {
        return eventCode;
    }

    /**
     * @param eventCode the eventCode to set
     */
    public void setEventCode(String eventCode) {
        this.eventCode = eventCode;
    }

    /**
     * @return the location
     */
    public String getLocation() {
        return location;
    }

    /**
     * @param location the location to set
     */
    public void setLocation(String location) {
        this.location = location;
    }

    /**
     * @return the competition
     */
    public String getCompetition() {
        return competition;
    }

    /**
     * @param competition the competition to set
     */
    public void setCompetition(String competition) {
        this.competition = competition;
    }

    /**
     * @return the round
     */
    public int getRound() {
        return round;
    }

    /**
     * @param round the round to set
     */
    public void setRound(int round) {
        this.round = round;
    }
    
    public boolean isLeagueGame() {
        return competition.equals(LEAGUE_COMPETITION);
    }

    /**
     * @return the eventURL
     */
    public URL getEventURL() {
        return eventURL;
    }

    /**
     * @param eventURL the eventURL to set
     */
    public void setEventURL(URL eventURL) {
        this.eventURL = eventURL;
    }
    
}
