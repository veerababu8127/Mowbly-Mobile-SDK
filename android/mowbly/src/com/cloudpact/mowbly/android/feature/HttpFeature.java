package com.cloudpact.mowbly.android.feature;

import java.io.File;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;
import java.util.Map.Entry;

import org.apache.http.entity.FileEntity;
import org.apache.http.entity.StringEntity;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.util.EntityUtils;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.http.Request;
import com.cloudpact.mowbly.android.service.FileService;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the Http feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 * @since 1.0.0
 */
public class HttpFeature extends BaseFeature {

    /** Exposed name of the HttpFeature */
    public static final String NAME = "http";
    
    public String uname = null;
	public String pwd  = null;
    
    
    public HttpFeature() {
        super(NAME);
    }

    protected String getPathForFileOptions(JsonObject options) {
        return Mowbly.getMowblyFileServiceUtil().getPathForFileOptions(options);
    }
    
    @Method(async = true, args = {
            @Argument(name = "options", type = JsonObject.class)
        })
        public Response request(JsonObject options) {
            Response response = new Response();
            if (!Mowbly.getNetworkService().isConnected()) {
                response.setCode(0);
                response.setError("No network");
            } else {
                try {
                	
                	if(options.get("username") != null){
                		uname = options.get("username").getAsString();
                	}
                	if(options.get("password") != null){
                		pwd = options.get("password").getAsString();
                	}
                	
                    String url = getUrlFromOptions(options);
                    if(url == null){
                    	return null;
                    }
                    FileService fileService = Mowbly.getFileService();
                    String method = (options.get("type") != null) ? options.get("type").getAsString() : "GET";

                    int timeout = options.get("timeout").getAsInt();

                    Request request = Request.fromUrl(url, false).setTimeout(timeout);
                    addHeadersFromOptions(request, options);
                    if (method.equals("GET")) {
                        request.setMethod(Request.GET);
                    } else if (method.equals("DELETE")) {
                        request.setMethod(Request.DELETE);
                    } else if (method.equals("POST") || method.equals("PUT")) {
                        JsonArray parts = null;
                        if (options.get("parts") != null) {
                            parts = options.get("parts").getAsJsonArray();
                        }
                        boolean isMultipart = (parts != null && parts.size() > 0);
                        if (isMultipart) {
                        	MultipartEntity entity = new MultipartEntity(HttpMultipartMode.BROWSER_COMPATIBLE);
                            JsonObject part;
                            for (int i = 0, j = parts.size(); i < j; i++) {
                                part = parts.get(i).getAsJsonObject();
                                String type = (part.get("type") != null) ? part.get("type").getAsString() : "string";
                                String name = part.get("name").getAsString();
                                String contentType = (part.get("contentType") != null) ? part.get("contentType").getAsString() : "application/octet-stream";
                                if (type.equals("string")) {
                                    String value = part.get("value").getAsString();
                                    	entity.addPart(name, getStringBody(value, contentType));
                             
                                } else {
                                    JsonObject value = part.get("value").getAsJsonObject();
                                    String path = getPathForFileOptions(value);
                                    if (path.startsWith("file:///")) {
                                        path = path.substring(7);
                                    }
                                    File file = new File(path);
                                    if (!file.exists()) {
                                        throw new FileNotFoundException("File not found");
                                    }
                                    String filename = null;
                                    if (part.get("filename") != null) {
                                        filename = part.get("filename").getAsString();
                                    }
                                    if (filename == null || filename.equals("")) {
                                        filename = file.getName();
                                    }
                                    entity.addPart(name, new FileBody(file, filename, contentType, "utf-8"));
                                }
                            }

                            request.setMethod(method.equals("POST") ? Request.POST_MULTIPART : Request.PUT_MULTIPART);
                            request.setEntity(entity);
                        } else {
                            if (options.get("data") != null) {
                                String dataType = options.get("dataType").getAsString();
                                if (dataType.equals("json")) {
                                    request.setMethod(method.equals("POST") ? Request.POST : Request.PUT);
                                    JsonObject data = options.get("data").getAsJsonObject();
                                    for (Entry<String, JsonElement> param : data.entrySet()) {
                                    	addParamToRequest(request, param);
                                    }
                                } else if (dataType.equals("file")) {
                                    // Added to handle posting file upload using post data method
                                    JsonObject data = options.get("data").getAsJsonObject();
                                    String path = getPathForFileOptions(data.get("file").getAsJsonObject());
                                    if (path.startsWith("file:///")) {
                                        path = path.substring(7);
                                    }
                                    String contentType = (data.get("contentType") != null) ? data.get("contentType").getAsString() : "application/octet-stream";
                                    File file = new File(path);
                                    if (!file.exists()) {
                                        throw new FileNotFoundException("File not found");
                                    }
                                    request.setMethod(method.equals("POST") ? Request.POST_DATA : Request.PUT_DATA);
                                    request.setEntity(new FileEntity(file, contentType));
                                } else {
                                    request.setMethod(method.equals("POST") ? Request.POST_DATA : Request.PUT_DATA);
                                    addStringEntityFromOptions(request, options);
                                 }
                            }
                        }
                    }
                    
                    if(options.get("authMode") != null){
                    	
    	                switch (options.get("authMode").getAsInt()) {
    		        		case Request.BASIC_AUTH:
    		        			request.setBasicAuth(
    		        					uname,
    		        					pwd
    		        			);
    		        			break;
    		
    		        		case Request.NTLM_AUTH:
    		        			request.setNTLMAuth(
    		        					uname,
    		        					pwd,
    		        					options.get("domain") != null ? options.get("domain").getAsString() : " "  
    		        			);
    		        			break;
    		        			
    		        		default:
    		        			//Default case authMode 0;
    		        			break;
    		        	}
                    }
                    
                    com.cloudpact.mowbly.android.http.Response resp = request.execute();

                    JsonObject heads = new JsonObject();
                    for (Entry<String, String> head : resp.getHeaders().entrySet()) {
                        heads.addProperty(head.getKey(), head.getValue());
                    }

                    JsonObject result = new JsonObject();
                    result.add("headers", heads);
                    // TODO Verify if we need to send the data to the user even if the user has specifically asked for downloading
                    if (options.get("downloadFile") == null) {
                        String responseData = "";
                        if (resp.getEntity() != null) {
                            responseData = EntityUtils.toString(resp.getEntity());
                        }
                        result.addProperty("data", responseData);
                    } else {
                        JsonObject value = options.get("downloadFile").getAsJsonObject();
                        String path = getPathForFileOptions(value);
                        FileOutputStream fos;
                        try {
                            fos = new FileOutputStream(fileService.getFile(path));
                            resp.writeTo(fos);
                        } catch (IOException e) {
                            response.setCode(0);
                            response.setError("Error occured while writing the response to file");
                        }
                    }

                    response.setCode(resp.getStatusCode());
                    response.setResult(result);
                } catch (FileNotFoundException e) {
                    response.setCode(0);
                    response.setError("File not found", e.getMessage());
                } catch (IOException e) {
                    response.setCode(0);
                    response.setError("Failed to connect", e.getMessage());
                }
            }
            return response;
        }

        protected String getUrlFromOptions(JsonObject options){
        	String url = options.get("url").getAsString();
        	return url;
        }
        
        protected void addHeadersFromOptions(Request request, JsonObject options){
        	if (options.get("headers") != null) {
                JsonObject headers = options.get("headers").getAsJsonObject();
                for (Entry<String, JsonElement> param : headers.entrySet()) {
                    request.addHeader(param.getKey(), param.getValue().getAsString());
                }
            }
        }
        
        protected StringBody getStringBody(String value, String contentType) throws UnsupportedEncodingException{
    		return new StringBody(value,
    			contentType,
    			Charset.defaultCharset()
    		);
        }
        
        private void addParamToRequest(Request request, Entry<String, JsonElement> param){
        	request.addParam(
        		param.getKey(),
        		getValueForParam(param)
        	);
        }
        
        protected String getValueForParam(Entry<String, JsonElement> param){
        	JsonElement v = param.getValue();
        	return v.isJsonNull() ? null : v.getAsString();
        }
        
        private void addStringEntityFromOptions(Request request, JsonObject options) throws UnsupportedEncodingException{
            request.setEntity(new StringEntity(
            			getDataFromOptions(options),
    	        		org.apache.http.protocol.HTTP.UTF_8
    	        	)
            );
        }
        
        protected String getDataFromOptions(JsonObject options){
        	return options.get("data").getAsString();
        }
}
