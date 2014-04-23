package com.cloudpact.mowbly.android.http;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.io.UnsupportedEncodingException;
import java.net.URLEncoder;
import java.security.KeyStore;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.zip.GZIPInputStream;

import org.apache.http.Header;
import org.apache.http.HeaderElement;
import org.apache.http.HttpEntity;
import org.apache.http.HttpException;
import org.apache.http.HttpResponse;
import org.apache.http.HttpResponseInterceptor;
import org.apache.http.HttpVersion;
import org.apache.http.NameValuePair;
import org.apache.http.auth.AuthScope;
import org.apache.http.auth.Credentials;
import org.apache.http.auth.NTCredentials;
import org.apache.http.auth.UsernamePasswordCredentials;
import org.apache.http.client.HttpClient;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.client.methods.HttpDelete;
import org.apache.http.client.methods.HttpEntityEnclosingRequestBase;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpPut;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.conn.ClientConnectionManager;
import org.apache.http.conn.params.ConnManagerParams;
import org.apache.http.conn.scheme.PlainSocketFactory;
import org.apache.http.conn.scheme.Scheme;
import org.apache.http.conn.scheme.SchemeRegistry;
import org.apache.http.conn.ssl.SSLSocketFactory;
import org.apache.http.entity.HttpEntityWrapper;
import org.apache.http.entity.mime.FormBodyPart;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.impl.conn.tsccm.ThreadSafeClientConnManager;
import org.apache.http.message.BasicHeader;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.BasicHttpParams;
import org.apache.http.params.CoreProtocolPNames;
import org.apache.http.params.HttpConnectionParams;
import org.apache.http.params.HttpParams;
import org.apache.http.params.HttpProtocolParams;
import org.apache.http.protocol.HttpContext;

import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.android.util.TrustAllSSLSocketFactory;

/**
 * Request
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class Request {

	/** Http Protocol */
    public static final String HTTP = "http";

    /** Https Protocol */
    public static final String HTTPS = "https";

    /** Content-Type header */
    protected static final String HEADER_CONTENT_TYPE = "Content-Type";

    /** Accept header */
    protected static final String HEADER_ACCEPT = "Accept";

    /** Authorization header */
    protected static final String HEADER_AUTHORIZATION = "Authorization";

    /** User-Agent header */
    protected static final String HEADER_USER_AGENT = "User-Agent";

    /** Default user agent request header value */
    protected static final String DEFAULT_USER_AGENT = "Mowbly Android/3.1";

    public static final int GET = 0;
    public static final int POST = 1;
    public static final int POST_DATA = 2;
    public static final int POST_MULTIPART = 3;
    public static final int PUT = 4;
    public static final int PUT_DATA = 5;
    public static final int PUT_MULTIPART = 6;
    public static final int DELETE = 7;
    
    public static final int BASIC_AUTH = 1;
    public static final int NTLM_AUTH = 2;

    protected String userAgent = DEFAULT_USER_AGENT;

    protected int bufferSize = 8192;

    protected String scheme;

    protected String host;

    protected int port;

    private String baseUri;

    private String path = "";

    private int method = GET;

    private int timeout = 30000;

    private Map<String, String> getParams = new HashMap<String, String>();

    private Map<String, String> params = new HashMap<String, String>();

    private Map<String, String> headers = new HashMap<String, String>();

    private HttpEntity entity;
    
    private Credentials credentials;
    
    public void setCredentials(Credentials credentials){
    	this.credentials = credentials; 
    }
    
    public Credentials getCredentials(){
    	return this.credentials;
    }

    private Request() {
    }

    /**
     * Create Request for host
     *
     * @param host
     */
    public Request(String host) {
        this(host, -1, HTTPS);
    }

    /**
     * Create the Request for host and scheme
     * 
     * @param host The host
     * @param scheme The scheme
     */
    public Request(String host, String scheme) {
        this(host, -1, scheme);
    }

    /**
     * Create the Request for host, port, and scheme
     *
     * @param host
     * @param port
     * @param scheme
     */
    public Request(String host, int port, String scheme) {
        this.scheme = scheme;
        this.host = host;
        this.port = port;

        StringBuffer uri = new StringBuffer(scheme);
        uri.append("://");
        uri.append(host);
        if (port > 0) {
            uri.append(":");
            uri.append(port);
        }
        setBaseUri(uri.toString());
    }
    
    public static Request fromUrl(String url){
    	return fromUrl(url, true);
    };

    public static Request fromUrl(String url, boolean doEncode) {
    	if (!url.startsWith("http://") && !url.startsWith("https://")) {
    		throw new IllegalArgumentException("Only http and https protocol supported");
    	}

    	String uri = url;
    	int s;
    	if ((s = url.indexOf("?")) != -1) {
    		uri = url.substring(0, s);
    	}

    	Request request = new Request().setMethod(GET);
    	s = url.indexOf("?");
        int e = 0;
        while (s != -1) {
        	String name = null, value = null;
            e = url.indexOf("=", s);
            if(e > 0){
            	name = url.substring(s + 1, e);
                s = e + 1;
                e = url.indexOf("&", s);
                if (e < 0) {
                    value = url.substring(s, url.length());
                } else {        
                    value = url.substring(s, e);
                }
                request.addParam(name, value, "GET", doEncode);
            }else{
            	uri = url;
            }
            s = e;
        }

    	request.setBaseUri(uri);
    	return request;
    	/*return new Request().setBaseUri(url).setMethod(GET);*/
    }

    public Request setBaseUri(String baseUri) {
    	this.baseUri = baseUri;
    	return this;
    }

    /**
     * Set the value to set as the user agent header.
     * Specifying a null or empty agent parameter will fallback to
     * the default user agent header value.
     *
     * @param agent
     * @return this client
     */
    public Request setUserAgent(String agent) {
        if (agent != null && agent.length() > 0) {
            userAgent = agent;
        } else {
            userAgent = DEFAULT_USER_AGENT;
        }
        return this;
    }

    /**
     * Set buffer size used to send the request and read the response
     *
     * @param bufferSize
     * @return this request
     */
    public Request setBufferSize(int bufferSize) {
        if (bufferSize < 1) {
            throw new IllegalArgumentException("Buffer size must be greater than zero");
        }

        this.bufferSize = bufferSize;
        return this;
    }

    /**
     * @return The path of the request
     */
    public String getPath() {
        return path;
    }

    /**
     * @param path
     * @return this request
     */
    public Request setPath(String path) {
        this.path = path;
        return this;
    }

    /**
     * @param method
     * @return this request
     */
    public Request setMethod(int method) {
        this.method = method;
        return this;
    }

    /**
     * @param timeout
     * @return this request
     */
    public Request setTimeout(int timeout) {
        this.timeout = timeout;
        return this;
    }

    /**
     * @return headers
     */
    public Map<String, String> getHeaders() {
        return headers;
    }

    /**
     * @param headers
     * @return this request
     */
    public Request setHeaders(Map<String, String> headers) {
        this.headers = headers;
        return this;
    }

    /**
     * @param name
     * @param value
     * @return Request
     */
    public Request addHeader(String name, String value) {
        headers.put(name, value);
        return this;
    }

    public Request addHeader(String name, Object value) {
        headers.put(name, String.valueOf(value));
        return this;
    }

    /**
     * @param headers
     * @return this request
     */
    public Request addHeaders(Map<String, String> headers) {
        this.headers.putAll(headers);
        return this;
    }

    /**
     * @return params
     */
    public Map<String, String> getParams() {
        return params;
    }

    public Request addParam(String name, Object value, String type, boolean doEncode) {
        return addParam(name, String.valueOf(value), type, doEncode);
    }
    
    public Request addParam(String name, Object value, String type){
    	return addParam(name, value, type, true);
    }

    public Request addParam(String name, String value, String type , boolean doEncode) {
        /*
        try {
            value = URLEncoder.encode(value, "UTF-8");
        } catch (UnsupportedEncodingException e1) {
            // Ignore. Should not come here.
        }
        */
        if (type.equals("GET")) {
        	if(doEncode){
	            try {
	                value = URLEncoder.encode(value, "UTF-8");
	            } catch (UnsupportedEncodingException e1) {
	                // Ignore. Should not come here.
	            }
        	}

            getParams.put(name, value);
        } else {
            params.put(name, value);
        }
        return this;
    }

    /**
     * @param name
     * @param value
     * @return Request
     */
    public Request addParam(String name, String value) {
        return addParam(name, value, "POST", false);
    }

    public Request addParam(String name, Object value) {
        return addParam(name, String.valueOf(value));
    }

    public Request setEntity(HttpEntity entity) {
        this.entity = entity;
        return this;
    }

    /**
     * @return The generated uri for GET methods
     */
    private String generatePath() {
        String basePath = path;
        if (basePath != null) {
            if (getParams != null && !getParams.isEmpty()) {
            	StringBuffer buffer = new StringBuffer();
                for (Entry<String, String> param : getParams.entrySet()) {
                    if (buffer.length() > 0) {
                        buffer.append('&');
                    }
                    buffer.append(param.getKey()).append('=');
                    if (param.getValue() != null) {
                        buffer.append(param.getValue());
                    }
                }
                
                if (buffer.length() > 0) {
                    basePath += (basePath.indexOf('?') > 0) ? "&": "?";
                    basePath += buffer.toString();
                }
            }
        }
        return basePath;
    }

    private String getUrl() {
    	return baseUri + generatePath();
    }

    private HttpClient getHttpClient(int timeout) {
        HttpClient client;
        try {
            KeyStore trustStore = KeyStore.getInstance(KeyStore.getDefaultType());
            trustStore.load(null, null);

            SSLSocketFactory sf = new TrustAllSSLSocketFactory(trustStore);
            sf.setHostnameVerifier(SSLSocketFactory.ALLOW_ALL_HOSTNAME_VERIFIER);

            HttpParams params = new BasicHttpParams();
            HttpProtocolParams.setVersion(params, HttpVersion.HTTP_1_1);
            HttpProtocolParams.setContentCharset(params, org.apache.http.protocol.HTTP.UTF_8);

            SchemeRegistry registry = new SchemeRegistry();
            registry.register(new Scheme("http", PlainSocketFactory.getSocketFactory(), 80));
            registry.register(new Scheme("https", sf, 443));

            ClientConnectionManager ccm = new ThreadSafeClientConnManager(params, registry);

            client = new DefaultHttpClient(ccm, params);
        } catch (Exception e) {
            client = new DefaultHttpClient();
        }
        
        if(credentials != null){
        	DefaultHttpClient httpClient = (DefaultHttpClient)client;
        	httpClient.getAuthSchemes().register("ntlm", new NTLMSchemeFactory());
        	httpClient.getCredentialsProvider().setCredentials(AuthScope.ANY, credentials);
        }
        
        ((DefaultHttpClient)client).addResponseInterceptor(new HttpResponseInterceptor() {
        	@Override
        	public void process(HttpResponse response, HttpContext context)
					throws HttpException, IOException {
                HttpEntity entity = response.getEntity();
                Header encheader = entity.getContentEncoding();
                if (encheader != null) {
                    HeaderElement[] codecs = encheader.getElements();
                    for (int i = 0; i < codecs.length; i++) {
                        if (codecs[i].getName().equalsIgnoreCase("gzip")) {
                            response.setEntity(new GzipDecompressingEntity(
                                    entity));
                            return;
                        }
                    }
                }
            }
        });
        
        final HttpParams params = client.getParams();
        HttpConnectionParams.setConnectionTimeout(params, timeout);
        HttpConnectionParams.setSoTimeout(params, timeout);
        ConnManagerParams.setTimeout(params, timeout);
        params.setParameter(CoreProtocolPNames.USER_AGENT, PageActivity.userAgent);
        return client;
    }
    
    private class GzipDecompressingEntity extends HttpEntityWrapper{
		
		private static final String GZIP_CODEC = "gzip";
    	
    	public GzipDecompressingEntity(HttpEntity e) {
			super(e);
		}
    	@Override
    	public InputStream getContent() throws IOException {
    		return new GZIPInputStream(super.getContent());
    	}
    	@Override
        public Header getContentEncoding() {
            return new BasicHeader(org.apache.http.protocol.HTTP.CONTENT_ENCODING, GZIP_CODEC);
        }

    	@Override
        public void writeTo(OutputStream outstream) throws IOException {
			if (outstream == null) {
				throw new IllegalArgumentException("Output stream may not be null");
			}
			InputStream instream = getContent();
		    try {
		       int l;
		       byte[] tmp = new byte[2048];
		       while ((l = instream.read(tmp)) != -1) {
		            outstream.write(tmp, 0, l);
		       }
		    } finally {
		        instream.close();
		    }   		
        }
    }

    private HttpUriRequest createRequest() {
        HttpUriRequest request = null;

        if (this.method == GET) {
            // GET request
            request = new HttpGet(getUrl());
        } else if (this.method == DELETE) {
            // DELETE request
            request = new HttpDelete(getUrl());
        } else {
            if (this.method == POST || this.method == POST_MULTIPART || this.method == POST_DATA) {
                // POST request
                request = new HttpPost(getUrl());
            } else {
                // PUT request
                request = new HttpPut(getUrl());
            }

            if (this.method == POST || this.method == PUT) {
                if (params != null && !params.isEmpty()) {
                    List<NameValuePair> requestParams = new ArrayList<NameValuePair>();
                    for (Entry<String, String> entry : params.entrySet()) {
                        requestParams.add(new BasicNameValuePair(entry.getKey(), entry.getValue()));
                    }
                    try {
                        entity = new UrlEncodedFormEntity(requestParams);
                    } catch (UnsupportedEncodingException e) {
                        // Ignore. Should not come here.
                    }
                }
            } else {
                if (params != null && !params.isEmpty()) {
                    for (Entry<String, String> entry : params.entrySet()) {
                        try {
                            ((MultipartEntity) entity).addPart(
                                    new FormBodyPart(entry.getKey(), new StringBody(entry.getValue())));
                        } catch (UnsupportedEncodingException e) {
                            // Ignore should not come here
                        }
                    }
                }
            }

            if (entity != null) {
                request.addHeader(entity.getContentType());
                ((HttpEntityEnclosingRequestBase) request).setEntity(entity);
            }
        }

        if (request != null) {
        	request.setHeader("Accept-Encoding", "gzip");
            for (Entry<String, String> entry : headers.entrySet()) {
                request.setHeader(entry.getKey(), entry.getValue());
            }
        }

        return request;
    }
    
    public void setNTLMAuth(final String username, final String password, final String domain){
		setCredentials(new NTCredentials(username, password, "", domain));
	}
	
	public void setBasicAuth(final String username, final String password){
		setCredentials(new UsernamePasswordCredentials(username, password));
	}

    public Response execute() throws IOException {
        HttpClient client = getHttpClient(timeout);
        HttpResponse resp = client.execute(createRequest());
        //client.getConnectionManager().shutdown();

        return new Response(resp);
    }
}
