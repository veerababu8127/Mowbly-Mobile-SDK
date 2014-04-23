package com.cloudpact.mowbly.feature;

/**
 * Error
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Error {
    /** The error message */
    protected String message;

    /** The error description */
    protected String description;

    public Error(String message) {
        this(message, null);
    }

    public Error(String message, String description) {
        setMessage(message);
        setDescription(description);
    }

    public String getMessage() {
        return message;
    }

    public void setMessage(String message) {
        this.message = message;
    }

    public String getDescription() {
        return description;
    }

    public void setDescription(String description) {
        this.description = description;
    }
}
