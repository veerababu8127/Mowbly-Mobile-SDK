package com.cloudpact.mowbly.android.feature;

import java.io.IOException;
import java.util.List;

import android.content.Context;
import android.location.Address;
import android.location.Geocoder;
import android.location.Location;
import android.location.LocationListener;
import android.location.LocationManager;
import android.os.Bundle;

import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.cloudpact.mowbly.log.Logger;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the GeoLocationFeature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class GeoLocationFeature extends BaseFeature {

    public static final String NAME = "geolocation";

    protected static final Logger logger = Logger.getLogger();
    protected static final String TAG = "LocationManager";

    private static LocationManager locationManager = null;

    public GeoLocationFeature() {
        super(NAME);
    }

    private LocationManager getLocationManager() {
        if (locationManager == null) {
            final PageActivity activity = ((FeatureBinder) binder).getActivity();
            locationManager = (LocationManager) activity.getSystemService(Context.LOCATION_SERVICE);
        }
        return locationManager;
    }

    @Method(async = false, args = {
        @Argument(name = "options", type = JsonObject.class)
    }, callback = true)
    public Response getCurrentPosition(JsonObject options, String callbackId) {
        logger.info(TAG, "Requested the current position");
        Response response = null;
        LocationManager locationManager = getLocationManager();
        boolean isGpsEnabled = locationManager.isProviderEnabled(LocationManager.GPS_PROVIDER);
        boolean isNetworkEnabled = locationManager.isProviderEnabled(LocationManager.NETWORK_PROVIDER);
        if (!isGpsEnabled && !isNetworkEnabled) {
            response = new Response();
            response.setCode(0);
            response.setError("No location providers enabled");

            ((FeatureBinder) binder).onAsyncMethodResult(callbackId, response);
        } else {            
            boolean enableHighAccuracy = false;
            if (options.get("enableHighAccuracy") != null) {
                enableHighAccuracy = options.get("enableHighAccuracy").getAsBoolean();
            }
            int timeout = 60000;
            if (options.get("timeout") != null) {
                timeout = options.get("timeout").getAsInt();
            }

            String provider = null;
            if (enableHighAccuracy) {
                if (isGpsEnabled) {
                    provider = LocationManager.GPS_PROVIDER;
                } else {
                    provider = LocationManager.NETWORK_PROVIDER;
                }
            } else {
                if (isNetworkEnabled) {
                    provider = LocationManager.NETWORK_PROVIDER;
                } else {
                    provider = LocationManager.GPS_PROVIDER;
                }
            }
            GeoLocationListener listener = new GeoLocationListener(callbackId);
            locationManager.requestLocationUpdates(provider, timeout, 0, listener);
        }

        return null;
    }

    @Method(async = true, args = {
        @Argument(name = "latitude", type = double.class),
        @Argument(name = "longitude", type = double.class),
        @Argument(name = "maxResults", type = int.class)
    })
    public Response getLocationForCoordinates(double latitude, double longitude, int maxResults) {
        logger.info(TAG, "Requested location for coordinates");
        Response response = new Response();
        JsonArray addresses = new JsonArray();
        try {
            final PageActivity activity = ((FeatureBinder) binder).getActivity();
            List<Address> list = new Geocoder(activity).getFromLocation(latitude, longitude, maxResults);
            
            JsonObject address;
            JsonObject addressLines;
            if (list != null && list.size() > 0) {
                int count = 0;
                for (Address addr : list) {
                    address = new JsonObject();
                    addressLines = new JsonObject();
                    for(int i = 0, numLines = addr.getMaxAddressLineIndex(); i < numLines; i++) {
                        addressLines.addProperty(String.valueOf(i), addr.getAddressLine(i));
                    }
                    address.add("addressLines", addressLines);
                    
                    address.addProperty("feature", addr.getFeatureName());
                    address.addProperty("admin", addr.getAdminArea());
                    address.addProperty("subAdmin", addr.getSubAdminArea());
                    address.addProperty("locality", addr.getLocality());
                    address.addProperty("thoroughfare", addr.getThoroughfare());
                    address.addProperty("postalCode", addr.getPostalCode());
                    address.addProperty("countryCode", addr.getCountryCode());
                    address.addProperty("country", addr.getCountryName());
                    address.addProperty("phone", addr.getPhone());
                    address.addProperty("url", addr.getUrl());
                    
                    addresses.add(address);
                    count = count + 1;
                    if (count >= maxResults) {
                        break;
                    }
                }

                logger.info(TAG, "Fetched the location for coordinates");
                response.setCode(1);
                response.setResult(addresses);
            } else {
                logger.warn(TAG, "Could not fetch location information, Reason - No Match / Backend service");
                response.setCode(0);
                response.setError("Could not fetch location information");
            }
        } catch (IOException e) {
            logger.warn(TAG, "Could not fetch location information, Reason - " + e.getMessage());
            response.setCode(0);
            response.setError("Could not fetch location information", e.getMessage());
        }
        return response;
    }

    private class GeoLocationListener implements LocationListener {

        private String callbackId;

        public GeoLocationListener(String callbackId) {
            this.callbackId = callbackId;
        }

        private Response getLocationResponse(Location location) {
            JsonObject coords = new JsonObject();
            coords.addProperty("latitude", location.getLatitude());
            coords.addProperty("longitude", location.getLongitude());
            coords.addProperty("altitude", location.getAltitude());
            coords.addProperty("accuracy", location.getAccuracy());
            coords.addProperty("altitudeAccuracy", 0);
            coords.addProperty("speed", location.getSpeed());
            coords.addProperty("heading", location.getBearing());

            JsonObject position = new JsonObject();
            position.add("coords", coords);
            position.addProperty("timestamp", location.getTime());

            JsonObject result = new JsonObject();
            result.add("position", position);

            Response response = new Response();
            response.setCode(1);
            response.setResult(result);
            return response;
        }

        @Override
        public void onLocationChanged(Location location) {
            logger.info(TAG, "Got the current position");
            ((FeatureBinder) binder).onAsyncMethodResult(callbackId, getLocationResponse(location));
            getLocationManager().removeUpdates(this);
        }

        @Override
        public void onProviderDisabled(String provider) {
        }

        @Override
        public void onProviderEnabled(String provider) {
        }

        @Override
        public void onStatusChanged(String provider, int status, Bundle extras) {
        }
    }
}
