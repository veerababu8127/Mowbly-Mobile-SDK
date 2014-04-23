package com.cloudpact.mowbly.android.feature;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.provider.ContactsContract;
import android.provider.ContactsContract.RawContacts;

import com.cloudpact.mowbly.R;
import com.cloudpact.mowbly.android.Intents;
import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.service.ContactsService;
import com.cloudpact.mowbly.android.ui.PageActivity;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonObject;

/**
 * Javascsript interface for Contacts feature
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class ContactsFeature extends BaseFeature {

    public static final String NAME = "contacts";

    private String callbackId = null;
    private JsonArray returnFields;
    
    protected ContactsService contactsService = null;

    public ContactsFeature() {
        super(NAME);
    }
    
    protected ContactsService getContactsService(){
    	if(contactsService == null){
    		contactsService = Mowbly.getContactsService();
    	}
    	return contactsService;
    }

    @Method(async = true, args = {
        @Argument(name = "filter", type = String.class),
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response findContact(String filter, JsonObject options) {
        Response response = new Response();
        int limit = (options.get("limit") != null) ? options.get("limit").getAsInt() : 0;
        JsonArray rfields = (options.get("properties") != null) ? options.get("properties").getAsJsonArray() : new JsonArray();
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        JsonArray contacts = getContactsService().findContact(filter, rfields, limit, activity);
        response.setCode(1);
        response.setResult(contacts);
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "info", type = JsonObject.class)
    })
    public Response saveContact(JsonObject info) {
        Response response = new Response();
        try {
            final PageActivity activity = ((FeatureBinder) binder).getActivity();
            String contactId = getContactsService().saveContact(info, activity);
            response.setCode(1);
            response.setResult(contactId);
        } catch (Exception e) {
            response.setCode(0);
            response.setError("Error occured while saving contact", e.getMessage());
        }
        return response;
    }

    @Method(async=true, args = {
        @Argument(name = "phoneNumber", type = String.class)
    })
    public Response callContact(String phoneNumber) {
        Intent callIntent = new Intent(Intent.ACTION_CALL);
        callIntent.setData(Uri.parse("tel:" + Uri.encode(phoneNumber)));

        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        activity.startActivity(callIntent);
        return new Response();
    }

    @Method(async=true, args = {
        @Argument(name = "id", type = String.class)
    })
    public Response deleteContact(String id) {
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        int result = activity.getContentResolver().delete(
        		RawContacts.CONTENT_URI.buildUpon().appendQueryParameter(ContactsContract.CALLER_IS_SYNCADAPTER, "true").build(),
        		ContactsContract.Data.CONTACT_ID + " = ?", 
        		new String[]{ id });

        Response response = new Response();
        if (result > 0) {
            response.setCode(1);
            response.setResult(true);
        } else {
            response.setCode(0);
            response.setError("Could not delete contact");
        }
        return response;
    }

    @Method(async=true, args = {
        @Argument(name = "filter", type = JsonArray.class),
        @Argument(name = "multiple", type = boolean.class),
        @Argument(name = "chooseProperty", type = boolean.class),
        @Argument(name = "performDefaultAction", type = boolean.class)
    })
    public Response pickContact(JsonArray filter, boolean multiple, boolean chooseProperty, boolean performDefaultAction, String callbackId) {
        this.callbackId = callbackId;

        Intent contactIntent = new Intent(Intent.ACTION_PICK, ContactsContract.Contacts.CONTENT_URI);
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        activity.startActivityForResult(contactIntent, Intents.getRequestCode(activity, R.string.action_CONTACTS_PICK_CONTACT));
        return null;
    }

    @Method(async=true, args = {
        @Argument(name = "id", type = String.class)
    })
    public Response viewContact(String id) {
        Response response = new Response();
        if (id == null || id.equals("")) {
            response.setCode(0);
            response.setError("Contact id cannot be empty");
        } else {
            Intent viewContactIntent = new Intent(Intent.ACTION_VIEW,
                    Uri.withAppendedPath(ContactsContract.Contacts.CONTENT_URI, id));
    
            final PageActivity activity = ((FeatureBinder) binder).getActivity();
            activity.startActivity(viewContactIntent);
        }
        return response;
    }

    /**
     * Activity result handler for pick contact sub-activity.
     * 
     * @param requestCode
     *            - The request code our class passed to the sub activity
     * @param resultCode
     *            - The result code from the sub activity
     * @param intent
     *            - The intent containing result data
     */
    public void onActivityResult(int requestCode, int resultCode, Intent intent) {
        Response response = new Response();
        JsonObject contactInfo = null;
        final PageActivity activity = ((FeatureBinder) binder).getActivity();
        if (requestCode == Intents.getRequestCode(activity, R.string.action_CONTACTS_PICK_CONTACT)) {
            if (resultCode == Activity.RESULT_CANCELED) {
                response.setCode(-1);
                //response.setError("Activity cancelled");
            } else {
                contactInfo = getContactsService().getContactInfo(intent.getData(), 
                        returnFields, activity);
                response.setCode(1);
                response.setResult(contactInfo);
            }
        }
        returnFields = null;

        ((FeatureBinder) binder).onAsyncMethodResult(callbackId, response);
        callbackId = null;
    }
}
