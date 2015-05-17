/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package staut;

import java.net.URL;

/**
 * This class contains event information exctracted from ticketing website: 
 * Opponent, event date and time, event ID, URLs for ticketing data, etc.
 */
public class EventInfo implements Comparable {
    private final int eventId;
    private String eventName;
    private String eventDate;
    private String eventTime;
    private String opponent;
    private URL availabilityURL;
    private URL geometryURL;
    
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
        return opponent;
    }

    /**
     * @param opponent the opponent to set
     */
    public void setOpponent(String opponent) {
        this.opponent = opponent;
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
    
}
