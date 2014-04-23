package com.cloudpact.mowbly.feature;

/**
 * Result
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Response {

    /** Code of the result */
    protected int code;

    /** Error of the result */
    protected Error error;

    /** Result of the result */
    protected Object result;

    public Response() {
    }

    public Response(int code) {
        setCode(code);
    }

    public int getCode() {
        return code;
    }

    public void setCode(int code) {
        this.code = code;
    }

    public Error getError() {
        return error;
    }

    public void setError(String message) {
        setError(new Error(message));
    }

    public void setError(String message, String description) {
        setError(new Error(message, description));
    }

    public void setError(Error error) {
        this.error = error;
    }

    public Object getResult() {
        return result;
    }

    public void setResult(Object result) {
        this.result = result;
    }
}
