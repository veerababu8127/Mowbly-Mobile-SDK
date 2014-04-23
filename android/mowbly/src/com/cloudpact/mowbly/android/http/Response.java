package com.cloudpact.mowbly.android.http;

import java.io.IOException;
import java.io.OutputStream;
import java.util.HashMap;

import org.apache.http.Header;
import org.apache.http.HttpEntity;
import org.apache.http.HttpResponse;
import org.apache.http.util.EntityUtils;

import com.google.gson.JsonElement;
import com.google.gson.JsonParseException;
import com.google.gson.JsonParser;

/**
 * Response class
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Response {

    /** HTTP response */
    protected final HttpResponse response;

    /** Http Status Code */
    protected final int statusCode;

    /** HttpEntity */
    protected final HttpEntity entity;

    /**
     * Create response from {@link HttpResponse}
     * @param resp
     */
    public Response(HttpResponse resp) {
        response = resp;
        entity = response.getEntity();
        statusCode = response.getStatusLine().getStatusCode();
    }

    public HttpResponse getHttpResponse() {
        return response;
    }

    public HttpEntity getEntity() {
        return entity;
    }

    public int getStatusCode() {
        return statusCode;
    }

    public HashMap<String, String> getHeaders() {
        HashMap<String, String> responseHeaders = new HashMap<String, String>();
        Header[] headers = response.getAllHeaders();
        for (Header header : headers) {
            responseHeaders.put(header.getName(), header.getValue());
        }
        return responseHeaders;
    }

    public void writeTo(OutputStream os) throws IOException {
        entity.writeTo(os);
        os.flush();
    }

    public JsonElement getResult() throws IOException {
        JsonElement result = null;
        try {
            String data = EntityUtils.toString(entity);
            result = new JsonParser().parse(data);
        } catch (JsonParseException e) {
            IOException ioe = new IOException();
            ioe.initCause(e);
            throw ioe;
        }
        return result;
    }
}
