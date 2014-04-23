package com.cloudpact.mowbly.android.feature;

import java.io.IOException;

import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.http.Request;
import com.cloudpact.mowbly.android.service.NetworkService;

/**
 * Javascript interface for the Framework feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class NetworkFeature extends BaseFeature {

    /** Exposed name of the FrameworkFeature */
    public static final String NAME = "network";

    public static final int NETWORK_NONE = 0;
    public static final int NETWORK_WIFI = 1;
    public static final int NETWORK_CELLULAR = 2;

    public NetworkFeature() {
        super(NAME);
    }

    /**
     * Checks if network connectivity is enabled
     * @param priority
     * @param message
     * @return Response
     */
    @Method(async = false, args = {})
    public Response isConnected(int priority, String message) {
        Response response = new Response();
        response.setResult(Mowbly.getNetworkService().isConnected());
        return response;
    }

    /**
     * Gets the active network mode - NONE, WIFI, 2G, 3G
     * @return Response
     */
    @Method(async = true, args = {})
    public Response getActiveNetwork() {
        int activeNetwork = Mowbly.getNetworkService().getActiveNetwork();
        int code = NETWORK_NONE;
        String result = null;
        switch (activeNetwork) {
            case NetworkService.NETWORK_NONE:
                code = NETWORK_NONE;
                result = "NONE";
                break;
            case NetworkService.NETWORK_WIFI:
                code = NETWORK_WIFI;
                result = "WIFI";
                break;
            case NetworkService.NETWORK_2G:
                code = NETWORK_CELLULAR;
                result = "2G";
                break;
            case NetworkService.NETWORK_3G:
                code = NETWORK_CELLULAR;
                result = "3G";
                break;
            default:
                code = NETWORK_NONE;
                break;
        }

        Response response = new Response();
        response.setCode(code);
        response.setResult(result);
        return response;
    }

    /**
     * Checks if the provided host is reachable
     * @param hostname
     * @param timeout
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "hostname", type = String.class),
        @Argument(name = "timeout", type = int.class)
    })
    public Response isHostReachable(String hostname, int timeout) {
        NetworkService networkService = Mowbly.getNetworkService();
        Response response = new Response();
        if (!networkService.isConnected()) {
            response.setResult(false);
            response.setError("No network");
        } else {
            try {
                if (!hostname.startsWith("http://") && !hostname.startsWith("https://")) {
                    hostname = "http://" + hostname;
                }
                com.cloudpact.mowbly.android.http.Response resp = Request.fromUrl(hostname).setTimeout(timeout).execute();
                response.setCode(resp.getStatusCode());
                response.setResult(true);
            } catch (IOException e) {
                response.setCode(0);
                response.setResult(false);
                response.setError("Could not reach " + hostname, e.getMessage());
            }
        }
        return response;
    }
}
