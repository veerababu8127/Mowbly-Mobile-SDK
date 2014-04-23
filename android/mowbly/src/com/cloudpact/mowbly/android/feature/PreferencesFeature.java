package com.cloudpact.mowbly.android.feature;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.service.PreferenceService;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;

/**
 * Javascript interface for the Preferences feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class PreferencesFeature extends BaseFeature {

    /** Exposed name of the PreferencesFeature */
    final static public String NAME = "preferences";

    public PreferencesFeature() {
        super(NAME);
    }
    
    /**
     * Commit the preferences to the shared preferences of the app
     * @param preferences
     * @return Response
     */
    @Method(async = false, args = {
		@Argument(name = "preferences", type = String.class)
    })
    public Response commit(String preferences) {
        PreferenceService preferenceService = Mowbly.getPreferenceService();
        preferenceService.setMowblyPreferences(preferences);
        return new Response();
    }
}
