package com.cloudpact.mowbly.android.service;

import java.text.ParseException;
import java.text.SimpleDateFormat;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Date;
import java.util.HashMap;
import java.util.Iterator;
import java.util.Map;
import java.util.Map.Entry;
import java.util.TimeZone;

import org.apache.commons.codec.binary.Base64;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.ContentProviderOperation;
import android.content.ContentProviderResult;
import android.content.OperationApplicationException;
import android.database.Cursor;
import android.net.Uri;
import android.os.RemoteException;
import android.provider.ContactsContract;
import android.provider.ContactsContract.CommonDataKinds;
import android.provider.ContactsContract.Contacts;

import com.google.gson.JsonArray;
import com.google.gson.JsonObject;
/**
 * Contacts Service
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
@SuppressLint({ "UseSparseArrays", "SimpleDateFormat" })
public class ContactsService {

    // Contact Fields
    private final String ID = "id";
    private final String TYPE = "type";
    private final String VALUE = "value";

    private final String ADDRESSES = "addresses";
    private final String ADDRESS_TYPE = "type";
    private final String STREET = "street";
    private final String CITY = "city";
    private final String REGION = "region";
    private final String POSTALCODE = "postalCode";
    private final String COUNTRY = "country";
    private final String EXTRA_INFO = "additionalInfo";

    private final String BIRTHDAY = "birthday";

    private final String EMAILS = "emails";
    private final String EMAIL_TYPE = "type";
    private final String EMAIL_VALUE = "value";

    private final String IMPPS = "impps";
    private final String IM_TYPE = "type";
    private final String IM_VALUE = "value";
    private final String IM_SERVICE = "service";

    private final String NAME = "name";
    private final String FIRSTNAME = "firstName";
    private final String MIDDLENAME = "middleName";
    private final String LASTNAME = "lastName";
    private final String PREFIX = "prefix";
    private final String SUFFIX = "suffix";

    private final String NICKNAME = "nickName";

    private final String NOTE = "note";

    private final String ORGANIZATION = "organization";
    private final String ORGANIZATION_NAME = "name";
    private final String DEPARTMENT = "department";
    private final String JOBTITLE = "jobTitle";

    private final String PHONES = "phones";
    private final String PHONE_TYPE = "type";
    private final String PHONE_VALUE = "value";

    private final String PHOTOS = "photos";
    private final String PHOTO_VALUE = "value";

    private final String URLS = "urls";
    private final String URL_TYPE = "type";
    private final String URL_VALUE = "value";

    // Contact property types
    private final String FAX = "fax";
    private final String HOME = "home";
    private final String MOBILE = "mobile";
    private final String OTHER = "other";
    private final String PAGER = "pager";
    private final String WORK = "work";

    // Helper constants
    private final String PROPERTY_DATA_TYPE = "dataType";
    private final int PROPERTY_TYPE_STRING = 0;
    private final int PROPERTY_TYPE_INT = 1;
    private final int PROPERTY_TYPE_BLOB = 2;
    private final int PROPERTY_TYPE_DATE = 3;
    private final int PROPERTY_TYPE_MULTIPLE = 4;
    private final int PROPERTY_TYPE_AGGREGATE = 5;

    private final Map<String, Object> androidContactParts;
    private final Map<String, String[]> w3ContactParts;
    private final Map<String, String> mapW3C_Android;
    private final Map<String, Object> mapAndroid_W3C;
    private final Map<String, String> mapW3C_AndroidMimeTypes;
    private final Map<String, String> mapAndroid_W3CMimeTypes;
    private final Map<String, HashMap<Integer, String>> mapAndroid_W3CPropertyTypes;
    private final Map<String, HashMap<String, Integer>> mapW3C_AndroidPropertyTypes;
    private final Map<String, Object> mapSearchProperties;
    private final Map<String, HashMap<String, Integer>> propertyTypeDef;

    public ContactsService() {
    	mapW3C_Android = Collections.unmodifiableMap(new HashMap<String, String>() {

            private static final long serialVersionUID = -8257770415564439503L;
            {
                put(ID, Contacts._ID);
                put(ADDRESS_TYPE, CommonDataKinds.StructuredPostal.TYPE);
                put(STREET, CommonDataKinds.StructuredPostal.STREET);
                put(CITY, CommonDataKinds.StructuredPostal.CITY);
                put(REGION, CommonDataKinds.StructuredPostal.REGION);
                put(POSTALCODE, CommonDataKinds.StructuredPostal.POSTCODE);
                put(COUNTRY, CommonDataKinds.StructuredPostal.COUNTRY);
                put(BIRTHDAY, CommonDataKinds.Event.START_DATE);
                put(EMAIL_TYPE, CommonDataKinds.Email.DATA2);
                put(EMAIL_VALUE, CommonDataKinds.Email.DATA1);
                put(IM_TYPE, CommonDataKinds.Im.DATA2);
                put(IM_VALUE, CommonDataKinds.Im.DATA1);
                put(IM_SERVICE, CommonDataKinds.Im.DATA5);
                put(FIRSTNAME, CommonDataKinds.StructuredName.GIVEN_NAME);
                put(MIDDLENAME, CommonDataKinds.StructuredName.MIDDLE_NAME);
                put(LASTNAME, CommonDataKinds.StructuredName.FAMILY_NAME);
                put(PREFIX, CommonDataKinds.StructuredName.PREFIX);
                put(SUFFIX, CommonDataKinds.StructuredName.SUFFIX);
                put(NICKNAME, CommonDataKinds.Nickname.NAME);
                put(NOTE, CommonDataKinds.Note.NOTE);
                put(ORGANIZATION_NAME, CommonDataKinds.Organization.COMPANY);
                put(DEPARTMENT, CommonDataKinds.Organization.DEPARTMENT);
                put(JOBTITLE, CommonDataKinds.Organization.TITLE);
                put(PHONE_TYPE, CommonDataKinds.Phone.DATA2);
                put(PHONE_VALUE, CommonDataKinds.Phone.DATA1);
                put(PHOTO_VALUE, CommonDataKinds.Photo.PHOTO);
                put(URL_TYPE, CommonDataKinds.Website.DATA2);
                put(URL_VALUE, CommonDataKinds.Website.DATA1);
            }
        });

        mapAndroid_W3C = Collections.unmodifiableMap(new HashMap<String, Object>() {
			private static final long serialVersionUID = 8826513119319639855L;
			{
                put(Contacts._ID, ID);
                put(ADDRESSES, new HashMap<String, String>() {
                	private static final long serialVersionUID = -8761216730458786479L;

					{
                        put(CommonDataKinds.StructuredPostal.TYPE, ADDRESS_TYPE);
                        put(CommonDataKinds.StructuredPostal.STREET, STREET);
                        put(CommonDataKinds.StructuredPostal.CITY, CITY);
                        put(CommonDataKinds.StructuredPostal.REGION, REGION);
                        put(CommonDataKinds.StructuredPostal.POSTCODE, POSTALCODE);
                        put(CommonDataKinds.StructuredPostal.COUNTRY,COUNTRY);
                    }
                });
                put(CommonDataKinds.Event.START_DATE, BIRTHDAY);
                put(EMAILS, new HashMap<String, String>() {
                    private static final long serialVersionUID = 8649740743625561970L;

					{
                        put(CommonDataKinds.Email.DATA2, EMAIL_TYPE);
                        put(CommonDataKinds.Email.DATA1, EMAIL_VALUE);
                    }
                });
                put(IMPPS, new HashMap<String, String>() {
                    private static final long serialVersionUID = 1580573690267621565L;

					{
                        put(CommonDataKinds.Im.DATA2, IM_TYPE);
                        put(CommonDataKinds.Im.DATA1, IM_VALUE);
                        put(CommonDataKinds.Im.DATA5, IM_SERVICE);
                    }
                });
                put(NAME, new HashMap<String, String>() {
                    private static final long serialVersionUID = 1440788442702020707L;

					{
                        put(CommonDataKinds.StructuredName.GIVEN_NAME, FIRSTNAME);
                        put(CommonDataKinds.StructuredName.MIDDLE_NAME, MIDDLENAME);
                        put(CommonDataKinds.StructuredName.FAMILY_NAME, LASTNAME);
                        put(CommonDataKinds.StructuredName.PREFIX, PREFIX);
                        put(CommonDataKinds.StructuredName.SUFFIX, SUFFIX);
                    }
                });
                put(CommonDataKinds.Nickname.NAME, NICKNAME);
                put(CommonDataKinds.Note.NOTE, NOTE);
                put(ORGANIZATION, new HashMap<String, String>() {
                    private static final long serialVersionUID = -7175865262513294376L;

					{
                        put(CommonDataKinds.Organization.COMPANY, ORGANIZATION_NAME);
                        put(CommonDataKinds.Organization.DEPARTMENT, DEPARTMENT);
                        put(CommonDataKinds.Organization.TITLE, JOBTITLE);
                    }
                });
                put(PHONES, new HashMap<String, String>() {
                    private static final long serialVersionUID = -5899743740994098846L;

					{
                        put(CommonDataKinds.Phone.DATA2, PHONE_TYPE);
                        put(CommonDataKinds.Phone.DATA1, PHONE_VALUE);
                    }
                });
                put(PHOTOS, new HashMap<String, String>() {
                    private static final long serialVersionUID = 5011044673048688714L;

					{
                        put(CommonDataKinds.Photo.PHOTO, PHOTO_VALUE);
                    }
                });
                put(URLS, new HashMap<String, String>() {
                    private static final long serialVersionUID = 4926040151614204820L;

					{
                        put(CommonDataKinds.Website.DATA2, URL_TYPE);
                        put(CommonDataKinds.Website.DATA1, URL_VALUE);
                    }
                });
            }
        });

        mapW3C_AndroidMimeTypes = Collections .unmodifiableMap(new HashMap<String, String>() {
            private static final long serialVersionUID = -4679678574678933109L;

			{
                put(ADDRESSES, CommonDataKinds.StructuredPostal.CONTENT_ITEM_TYPE);
                put(BIRTHDAY, CommonDataKinds.Event.CONTENT_ITEM_TYPE);
                put(EMAILS, CommonDataKinds.Email.CONTENT_ITEM_TYPE);
                put(IMPPS, CommonDataKinds.Im.CONTENT_ITEM_TYPE);
                put(NAME, CommonDataKinds.StructuredName.CONTENT_ITEM_TYPE);
                put(NICKNAME, CommonDataKinds.Nickname.CONTENT_ITEM_TYPE);
                put(NOTE, CommonDataKinds.Note.CONTENT_ITEM_TYPE);
                put(ORGANIZATION, CommonDataKinds.Organization.CONTENT_ITEM_TYPE);
                put(PHONES, CommonDataKinds.Phone.CONTENT_ITEM_TYPE);
                put(PHOTOS, CommonDataKinds.Photo.CONTENT_ITEM_TYPE);
                put(URLS, CommonDataKinds.Website.CONTENT_ITEM_TYPE);
            }
        });

        mapAndroid_W3CMimeTypes = Collections .unmodifiableMap(new HashMap<String, String>() {
            private static final long serialVersionUID = 4402458090158965577L;

			{
                put(CommonDataKinds.StructuredPostal.CONTENT_ITEM_TYPE, ADDRESSES);
                put(CommonDataKinds.Event.CONTENT_ITEM_TYPE, BIRTHDAY);
                put(CommonDataKinds.Email.CONTENT_ITEM_TYPE, EMAILS);
                put(CommonDataKinds.Im.CONTENT_ITEM_TYPE, IMPPS);
                put(CommonDataKinds.StructuredName.CONTENT_ITEM_TYPE, NAME);
                put(CommonDataKinds.Nickname.CONTENT_ITEM_TYPE, NICKNAME);
                put(CommonDataKinds.Note.CONTENT_ITEM_TYPE, NOTE);
                put(CommonDataKinds.Organization.CONTENT_ITEM_TYPE, ORGANIZATION);
                put(CommonDataKinds.Phone.CONTENT_ITEM_TYPE, PHONES);
                put(CommonDataKinds.Photo.CONTENT_ITEM_TYPE, PHOTOS);
                put(CommonDataKinds.Website.CONTENT_ITEM_TYPE, URLS);
            }
        });

        mapAndroid_W3CPropertyTypes = Collections .unmodifiableMap(new HashMap<String, HashMap<Integer, String>>() {
            private static final long serialVersionUID = 6223766487242205853L;

			{
                put(ADDRESSES, new HashMap<Integer, String>() {
					private static final long serialVersionUID = -5613737359344109791L;
					
					{
                        put(CommonDataKinds.StructuredPostal.TYPE_HOME, HOME);
                        put(CommonDataKinds.StructuredPostal.TYPE_WORK, WORK);
                        put(CommonDataKinds.StructuredPostal.TYPE_OTHER, OTHER);
                    }
                });
                put(EMAILS, new HashMap<Integer, String>() {
					private static final long serialVersionUID = -3924175550731721157L;
					
                    {
                        put(CommonDataKinds.Email.TYPE_HOME, HOME);
                        put(CommonDataKinds.Email.TYPE_WORK, WORK);
                        put(CommonDataKinds.Email.TYPE_OTHER, OTHER);
                    }
                });
                put(IMPPS, new HashMap<Integer, String>() {
                	private static final long serialVersionUID = -657131378777468622L;

					{
                        put(CommonDataKinds.Im.TYPE_HOME, HOME);
                        put(CommonDataKinds.Im.TYPE_WORK, WORK);
                        put(CommonDataKinds.Im.TYPE_OTHER, OTHER);
                    }
                });
                put(PHONES, new HashMap<Integer, String>() {
                    private static final long serialVersionUID = 5006279093044688435L;

					{
                        put(CommonDataKinds.Phone.TYPE_FAX_WORK, FAX);
                        put(CommonDataKinds.Phone.TYPE_HOME, HOME);
                        put(CommonDataKinds.Phone.TYPE_OTHER, OTHER);
                        put(CommonDataKinds.Phone.TYPE_MOBILE, MOBILE);
                        put(CommonDataKinds.Phone.TYPE_PAGER, PAGER);
                        put(CommonDataKinds.Phone.TYPE_WORK, WORK);
                    }
                });
                put(URLS, new HashMap<Integer, String>() {
                    private static final long serialVersionUID = 1192844519787504689L;

					{
                        put(CommonDataKinds.Website.TYPE_HOME, HOME);
                        put(CommonDataKinds.Website.TYPE_WORK, WORK);
                        put(CommonDataKinds.Website.TYPE_OTHER, OTHER);
                    }
                });
            }
        });

        mapW3C_AndroidPropertyTypes = Collections .unmodifiableMap(new HashMap<String, HashMap<String, Integer>>() {
            private static final long serialVersionUID = 8667656272061444221L;

			{
                put(ADDRESSES, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 1240293838328823243L;

					{
                        put(HOME, CommonDataKinds.StructuredPostal.TYPE_HOME);
                        put(WORK, CommonDataKinds.StructuredPostal.TYPE_WORK);
                        put(OTHER, CommonDataKinds.StructuredPostal.TYPE_OTHER);
                    }
                });
                put(EMAILS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -4317899839458154162L;

					{
                        put(HOME, CommonDataKinds.Email.TYPE_HOME);
                        put(WORK, CommonDataKinds.Email.TYPE_WORK);
                        put(OTHER, CommonDataKinds.Email.TYPE_OTHER);
                    }
                });
                put(IMPPS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -7357957682148240936L;

					{
                        put(HOME, CommonDataKinds.Im.TYPE_HOME);
                        put(WORK, CommonDataKinds.Im.TYPE_WORK);
                        put(OTHER, CommonDataKinds.Im.TYPE_OTHER);
                    }
                });
                put(PHONES, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -870565825035075481L;

					{
                        put(FAX, CommonDataKinds.Phone.TYPE_FAX_WORK);
                        put(HOME, CommonDataKinds.Phone.TYPE_HOME);
                        put(OTHER, CommonDataKinds.Phone.TYPE_OTHER);
                        put(MOBILE, CommonDataKinds.Phone.TYPE_MOBILE);
                        put(PAGER, CommonDataKinds.Phone.TYPE_PAGER);
                        put(WORK, CommonDataKinds.Phone.TYPE_WORK);
                    }
                });
                put(URLS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 5169012183644696049L;

					{
                        put(HOME, CommonDataKinds.Website.TYPE_HOME);
                        put(WORK, CommonDataKinds.Website.TYPE_WORK);
                        put(OTHER, CommonDataKinds.Website.TYPE_OTHER);
                    }
                });
            }
        });

        androidContactParts = Collections.unmodifiableMap(new HashMap<String, Object>() {
            private static final long serialVersionUID = 4844319670790410867L;

			{
                put(ADDRESSES, new String[] {
                        CommonDataKinds.StructuredPostal.TYPE,
                        CommonDataKinds.StructuredPostal.STREET,
                        CommonDataKinds.StructuredPostal.CITY,
                        CommonDataKinds.StructuredPostal.REGION,
                        CommonDataKinds.StructuredPostal.POSTCODE,
                        CommonDataKinds.StructuredPostal.COUNTRY });
                put(BIRTHDAY, CommonDataKinds.Event.START_DATE);
                put(EMAILS, new String[] {
                        CommonDataKinds.Email.TYPE,
                        CommonDataKinds.Email.DATA });
                put(IMPPS, new String[] {
                        CommonDataKinds.Im.TYPE,
                        CommonDataKinds.Im.DATA });
                put(NAME, new String[] {
                        CommonDataKinds.StructuredName.GIVEN_NAME,
                        CommonDataKinds.StructuredName.MIDDLE_NAME,
                        CommonDataKinds.StructuredName.FAMILY_NAME,
                        CommonDataKinds.StructuredName.PREFIX,
                        CommonDataKinds.StructuredName.SUFFIX });
                put(NICKNAME, CommonDataKinds.Nickname.NAME);
                put(NOTE, CommonDataKinds.Note.NOTE);
                put(ORGANIZATION, new String[] {
                        CommonDataKinds.Organization.COMPANY,
                        CommonDataKinds.Organization.DEPARTMENT,
                        CommonDataKinds.Organization.TITLE });
                put(PHONES, new String[] {
                        CommonDataKinds.Phone.TYPE,
                        CommonDataKinds.Phone.NUMBER });
                put(PHOTOS, new String[] { CommonDataKinds.Photo.PHOTO });
                put(URLS, new String[] {
                        CommonDataKinds.Website.TYPE,
                        CommonDataKinds.Website.URL });
            }
        });

        w3ContactParts = Collections .unmodifiableMap(new HashMap<String, String[]>() {
            private static final long serialVersionUID = 305034133441696107L;

			{
                put(ADDRESSES, new String[] { ADDRESS_TYPE, STREET, CITY, REGION, POSTALCODE, COUNTRY, EXTRA_INFO });
                put(BIRTHDAY, null);
                put(EMAILS, new String[] { EMAIL_TYPE, EMAIL_VALUE });
                put(IMPPS, new String[] { IM_TYPE, IM_VALUE });
                put(NAME, new String[] { FIRSTNAME, MIDDLENAME, LASTNAME, PREFIX, SUFFIX });
                put(NICKNAME, null);
                put(NOTE, null);
                put(ORGANIZATION, new String[] { ORGANIZATION_NAME, DEPARTMENT, JOBTITLE });
                put(PHONES, new String[] { PHONE_TYPE, PHONE_VALUE });
                put(PHOTOS, new String[] { PHOTO_VALUE });
                put(URLS, new String[] { TYPE, VALUE });
            }
        });

        propertyTypeDef = Collections .unmodifiableMap(new HashMap<String, HashMap<String, Integer>>() {
            private static final long serialVersionUID = -3833465996059156L;

			{
                put(ADDRESSES, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 7083735218061121294L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(ADDRESS_TYPE, PROPERTY_TYPE_INT);
                        put(STREET, PROPERTY_TYPE_STRING);
                        put(CITY, PROPERTY_TYPE_STRING);
                        put(REGION, PROPERTY_TYPE_STRING);
                        put(POSTALCODE, PROPERTY_TYPE_STRING);
                        put(COUNTRY, PROPERTY_TYPE_STRING);
                        put(EXTRA_INFO, PROPERTY_TYPE_STRING);
                    }
                });
                put(BIRTHDAY, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -768457073995001530L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_DATE);
                    }
                });
                put(EMAILS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 1195470901177566997L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(EMAIL_TYPE, PROPERTY_TYPE_INT);
                        put(EMAIL_VALUE, PROPERTY_TYPE_STRING);
                    }
                });
                put(IMPPS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 3844501528587281569L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(IM_TYPE, PROPERTY_TYPE_INT);
                        put(IM_VALUE, PROPERTY_TYPE_STRING);
                    }
                });
                put(NAME, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -2078714903176594156L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_AGGREGATE);
                        put(FIRSTNAME, PROPERTY_TYPE_STRING);
                        put(MIDDLENAME, PROPERTY_TYPE_STRING);
                        put(LASTNAME, PROPERTY_TYPE_STRING);
                        put(PREFIX, PROPERTY_TYPE_STRING);
                        put(SUFFIX, PROPERTY_TYPE_STRING);
                    }
                });
                put(NICKNAME, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -1578971308021118722L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_STRING);
                    }
                });
                put(NOTE, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -665936744634360400L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_STRING);
                    }
                });
                put(ORGANIZATION, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -1494496679764104249L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_AGGREGATE);
                        put(ORGANIZATION_NAME, PROPERTY_TYPE_STRING);
                        put(DEPARTMENT, PROPERTY_TYPE_STRING);
                        put(JOBTITLE, PROPERTY_TYPE_STRING);
                    }
                });
                put(PHONES, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -3072683122014138650L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(PHONE_TYPE, PROPERTY_TYPE_INT);
                        put(PHONE_VALUE, PROPERTY_TYPE_STRING);
                    }
                });
                put(PHOTOS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = -7680349173048758595L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(PHONE_VALUE, PROPERTY_TYPE_BLOB);
                    }
                });
                put(URLS, new HashMap<String, Integer>() {
                    private static final long serialVersionUID = 4137361670867883616L;

					{
                        put(PROPERTY_DATA_TYPE, PROPERTY_TYPE_MULTIPLE);
                        put(URL_TYPE, PROPERTY_TYPE_INT);
                        put(URL_VALUE, PROPERTY_TYPE_STRING);
                    }
                });
            }
        });

        mapSearchProperties = Collections.unmodifiableMap(new HashMap<String, Object>() {
            private static final long serialVersionUID = -9045241328756702361L;

			{
                put(ADDRESSES, new String[] {
                        CommonDataKinds.StructuredPostal.STREET,
                        CommonDataKinds.StructuredPostal.CITY,
                        CommonDataKinds.StructuredPostal.REGION,
                        CommonDataKinds.StructuredPostal.POSTCODE,
                        CommonDataKinds.StructuredPostal.COUNTRY });
                put(EMAILS, CommonDataKinds.Email.DATA1);
                put(IMPPS, CommonDataKinds.Im.DATA1);
                put(NAME, new String[] {
                        CommonDataKinds.StructuredName.GIVEN_NAME,
                        CommonDataKinds.StructuredName.MIDDLE_NAME,
                        CommonDataKinds.StructuredName.FAMILY_NAME,
                        CommonDataKinds.StructuredName.PREFIX,
                        CommonDataKinds.StructuredName.SUFFIX });
                put(NICKNAME, CommonDataKinds.Nickname.NAME);
                put(NOTE, CommonDataKinds.Note.NOTE);
                put(ORGANIZATION, new String[] {
                        CommonDataKinds.Organization.COMPANY,
                        CommonDataKinds.Organization.DEPARTMENT,
                        CommonDataKinds.Organization.TITLE });
                put(PHONES, CommonDataKinds.Phone.DATA1);
                put(URLS, CommonDataKinds.Website.DATA1);
            }
        });
	}

    public JsonArray findContact(String filter, JsonArray returnFields, int numContacts, Activity activity) {
        JsonArray contacts = new JsonArray();

        // Create the selection and selectionArgs for the query
        ArrayList<String> where = new ArrayList<String>();
        ArrayList<String> whereArgs = new ArrayList<String>();

        if ("".equals(filter)) {

            // User needs all contacts to return
            where.add(ContactsContract.Contacts.DISPLAY_NAME + " LIKE ?");
            whereArgs.add("%" + filter + "%");
        } else {

            // Display name search by default
            where.add(ContactsContract.Contacts.DISPLAY_NAME + " LIKE ?");
            whereArgs.add("%" + filter + "%");

            // Build the query. Iterate through all the android mime types
            Iterator<Entry<String, String>> mimeIterator = mapW3C_AndroidMimeTypes
                    .entrySet().iterator();

            while (mimeIterator.hasNext()) {

                Entry<String, String> e = mimeIterator.next();
                String w3CType = e.getKey();
                String mimeType = e.getValue();

                // Get the fields of the w3CType
                if (!mapSearchProperties.containsKey(w3CType)) {
                    continue;
                }
                Object fields = mapSearchProperties.get(w3CType);
                if (fields.getClass().equals(String.class)) {

                    where.add("(" + (String) fields + " LIKE ? AND " + ContactsContract.Data.MIMETYPE + " = ? )");
                    whereArgs.add("%" + filter + "%");
                    whereArgs.add(mimeType);
                } else if (fields.getClass().equals(String[].class)) {

                    // Add every field as an entry to the selection
                    int count = ((String[]) fields).length;
                    String field;
                    for (int i = 0; i < count; i++) {
                        field = ((String[]) fields)[i];
                        where.add("(" + field + " LIKE ? AND " + ContactsContract.Data.MIMETYPE + " = ? )");
                        whereArgs.add("%" + filter + "%");
                        whereArgs.add(mimeType);
                    }
                }
            }
        }

        // Create the selection attribute
        StringBuffer selection = new StringBuffer();
        int count = where.size();
        for (int i = 0; i < count; i++) {
            selection.append(where.get(i));
            if (i != (count - 1)) {
                selection.append(" OR ");
            }
        }

        // Create the selectionArgs attribute
        String[] selectionArgs = new String[whereArgs.size()];
        count = whereArgs.size();
        for (int i = 0; i < count; i++) {
            selectionArgs[i] = whereArgs.get(i);
        }

        String selectionString = selection.toString();
        //selectionString = null;
        //selectionArgs = null;
        // Get all the id's where the search term matches the fields passed in.
        Cursor idCursor = activity.getContentResolver().query(
                ContactsContract.Data.CONTENT_URI,
                new String[] { ContactsContract.Data.CONTACT_ID,
                        ContactsContract.Contacts._ID,
                        ContactsContract.Contacts.LOOKUP_KEY },
                selectionString, selectionArgs,
                ContactsContract.Data.CONTACT_ID + " ASC");

        // Create a set of unique ids
        HashMap<String, JsonObject> mapContacts = new HashMap<String, JsonObject>();
        JsonObject contact;
        count = (numContacts == 0) ? -100 : 0;
        while (idCursor.moveToNext() && count < numContacts) {
            count++;
            String contactId = idCursor.getString(idCursor
                    .getColumnIndex(ContactsContract.Data.CONTACT_ID));
            if (!mapContacts.containsKey(contactId)) {
                long _id = idCursor.getLong(idCursor.getColumnIndex(ContactsContract.Contacts._ID));
                String lookUp = idCursor.getString(idCursor.getColumnIndex(ContactsContract.Contacts.LOOKUP_KEY));

                try {
                    contact = getContactInfo(
                            ContactsContract.Contacts.getLookupUri(_id, lookUp),
                            returnFields, activity);
                    mapContacts.put(contactId, contact);
                } catch (Exception e) {
                }
            }
        }
        idCursor.close();

        // Add the contacts to the result JsonArray
        for (Entry<String, JsonObject> entry : mapContacts.entrySet()) {
            contacts.add(entry.getValue());
        }

        return contacts;
    }

    @SuppressWarnings("deprecation")
	public JsonObject getContactInfo(Uri uri, JsonArray returnFields,
            Activity activity) {

        JsonObject contactInfo = new JsonObject();

        Cursor c = activity.managedQuery(uri, null, null, null, null);
        if (c.moveToNext()) {
            String contactId = c.getString(c.getColumnIndex(Contacts._ID));

            // Get the property fields that are to be returned
            boolean bFilter = false;
            ArrayList<String> filter = new ArrayList<String>();
            int count = 0;
            if (returnFields != null && (count = returnFields.size()) > 0) {
                bFilter = true;
                for (int i = 0; i < count; i++) {
                    String f = (returnFields.get(i) != null) ? returnFields.get(i).getAsString() : "";
                    filter.add(f);
                }
            }

            // Set the ID in contact info, by default
            contactInfo.addProperty(ID, contactId);

            c = activity.managedQuery(
                    ContactsContract.Data.CONTENT_URI, null,
                    ContactsContract.Data.CONTACT_ID + "=?",
                    new String[] { contactId }, null);
            while (c.moveToNext()) {

                String androidType = c.getString(c
                        .getColumnIndex(ContactsContract.Data.MIMETYPE));
                String w3CType = mapAndroid_W3CMimeTypes.get(androidType);
                if (w3CType == null || (bFilter && !filter.contains(w3CType))) {
                    continue;
                }
                int propertyType = propertyTypeDef.get(w3CType).get(
                        PROPERTY_DATA_TYPE);
                JsonObject propertyValue;
                if (propertyType == PROPERTY_TYPE_MULTIPLE) {
                    propertyValue = getMultiplePropertyValue(c, w3CType);

                    if (propertyValue != null) {
                        // Create the JSONArray for this property, if not
                        // available
                        JsonArray multiValue = (contactInfo.get(w3CType) != null) ? contactInfo.get(w3CType).getAsJsonArray() : null;
                        if (multiValue == null) {
                            multiValue = new JsonArray();
                            contactInfo.add(w3CType, multiValue);
                        }

                        // Set the value
                        multiValue.add(propertyValue);
                    }
                } else if (propertyType == PROPERTY_TYPE_AGGREGATE) {
                    propertyValue = getMultiplePropertyValue(c, w3CType);
                    if (propertyValue != null) {
                        contactInfo.add(w3CType, propertyValue);
                    }
                } else {
                    String value = getSinglePropertyValue(c, w3CType);
                    if (value != null) {
                        contactInfo.addProperty(w3CType, value);
                    }
                }
            }
        }

        return contactInfo;
    }

    private JsonObject getMultiplePropertyValue(Cursor cursor, String w3CProperty) {
        JsonObject value = null;
        String[] parts = (String[]) androidContactParts.get(w3CProperty);
        if (parts != null) {
            value = new JsonObject();
            int count = parts.length;
            String w3CName;
            for (int i = 0; i < count; i++) {
                @SuppressWarnings("unchecked")
                HashMap<String, String> androidProperty = (HashMap<String, String>) mapAndroid_W3C
                        .get(w3CProperty);
                w3CName = androidProperty.get(parts[i]);
                Integer dataType = propertyTypeDef.get(w3CProperty)
                        .get(w3CName);
                if (dataType == null) {
                    dataType = PROPERTY_TYPE_STRING;
                }
                String propertyValue = "";
                if (dataType == PROPERTY_TYPE_BLOB) {
                    byte[] data = cursor.getBlob(cursor
                            .getColumnIndex(parts[i]));
                    if (data != null) {
                        propertyValue = new String(Base64.encodeBase64(data));
                    }
                } else {
                    propertyValue = cursor.getString(cursor
                            .getColumnIndex(parts[i]));
                }
                if (w3CName.equals(TYPE)) {
                    try {
                        String temp = mapAndroid_W3CPropertyTypes.get(w3CProperty).get(Integer.parseInt(propertyValue));
                        if (temp != null) {
                            propertyValue = temp;
                        }
                    } catch (NumberFormatException e) {
                    }
                }
                value.addProperty(w3CName, propertyValue);
            }
        }
        return value;
    }

    private String getSinglePropertyValue(Cursor cursor, String w3CProperty) {
        String value = null;
        String androidName = (String) androidContactParts.get(w3CProperty);
        if (androidName != null) {
            Integer dataType = propertyTypeDef.get(w3CProperty).get(PROPERTY_DATA_TYPE);
            if (dataType == PROPERTY_TYPE_DATE) {
                String dateStr = cursor.getString(cursor.getColumnIndex(androidName));
                String dateTemplate = "yyyy-MM-dd HH:mm:ss.SSS";
                SimpleDateFormat sdf = new SimpleDateFormat(dateTemplate);
                sdf.setTimeZone(TimeZone.getTimeZone("UTC"));
                try {
                    Date date = sdf.parse(dateStr);
                    value = String.valueOf(date.getTime());
                } catch (ParseException pe) {
                }
            } else {
                value = cursor.getString(cursor.getColumnIndex(androidName));
            }
        }
        return value;
    }

    private void insertMultiValueProperty(ArrayList<ContentProviderOperation> ops, JsonArray propertyValues, String property) {
        for (int i = 0; i < propertyValues.size(); i++) {
            JsonObject propertyObj = propertyValues.get(i).getAsJsonObject();
            insertPropertyValues(ops, propertyObj, property);
        }
    }

    private void insertPropertyValues(ArrayList<ContentProviderOperation> ops, JsonObject propertyObj, String property) {

        ContentProviderOperation.Builder builder = ContentProviderOperation
                .newInsert(ContactsContract.Data.CONTENT_URI)
                .withValueBackReference(ContactsContract.Data.RAW_CONTACT_ID, 0);

        // Set Mimetype
        builder = builder.withValue(ContactsContract.Data.MIMETYPE,
                mapW3C_AndroidMimeTypes.get(property));

        String[] fields = w3ContactParts.get(property);
        if (fields != null) {
            int count = fields.length;
            for (int i = 0; i < count; i++) {
                String w3CFieldName = fields[i];
                String androidFieldName = mapW3C_Android.get(w3CFieldName);

                if (androidFieldName != null) {
                    Object propertyValue = null;
                    int dataType = propertyTypeDef.get(property).get(
                            w3CFieldName);
                    if (dataType == PROPERTY_TYPE_STRING) {
                        propertyValue = (propertyObj.get(w3CFieldName) != null) ? propertyObj.get(w3CFieldName).getAsString() : "";
                    } else if (dataType == PROPERTY_TYPE_INT) {
                        if (w3CFieldName.equals(TYPE)) {
                            String type = (propertyObj.get(w3CFieldName) != null) ? propertyObj.get(w3CFieldName).getAsString() : "";
                            propertyValue = mapW3C_AndroidPropertyTypes.get(property).get(type);
                        } else {
                            propertyValue = (propertyObj.get(w3CFieldName) != null) ? propertyObj.get(w3CFieldName).getAsInt() : "";
                        }
                    } else if (dataType == PROPERTY_TYPE_BLOB) {
                        String encStr = (propertyObj.get(w3CFieldName) != null) ? propertyObj.get(w3CFieldName).getAsString() : "";
                        propertyValue = Base64.decodeBase64(encStr.getBytes());
                    } else if (dataType == PROPERTY_TYPE_DATE) {
                        // TODO: Birthday
                    }

                    if (propertyValue != null) {
                        builder = builder.withValue(androidFieldName, propertyValue);
                    }
                }
            }
        }

        ops.add(builder.build());
    }

    private void insertSinglePropertyValue(ArrayList<ContentProviderOperation> ops, String propertyValue, String property) {
        ContentProviderOperation.Builder builder = ContentProviderOperation
                .newInsert(ContactsContract.Data.CONTENT_URI)
                .withValueBackReference(ContactsContract.Data.RAW_CONTACT_ID, 0);
        // Set Mimetype
        builder = builder.withValue(ContactsContract.Data.MIMETYPE, mapW3C_AndroidMimeTypes.get(property));

        // Set property value
        String androidProperty = mapW3C_Android.get(property);
        builder.withValue(androidProperty, propertyValue);

        ops.add(builder.build());
    }

    @SuppressWarnings("deprecation")
	public String saveContact(JsonObject contact, Activity activity) throws RemoteException, OperationApplicationException {

        // Array of ContentProviderOperations for contact creation
        ArrayList<ContentProviderOperation> ops = new ArrayList<ContentProviderOperation>();

        // Add contact type
        ops.add(ContentProviderOperation
                .newInsert(ContactsContract.RawContacts.CONTENT_URI)
                .withValue(ContactsContract.RawContacts.ACCOUNT_TYPE, null)
                .withValue(ContactsContract.RawContacts.ACCOUNT_NAME, null)
                .build());

        JsonObject propertyObj = null;
        String propertyValue = null;
        JsonArray propertyValues = null;

        // Add address
        propertyValues = (contact.get(ADDRESSES) != null) ? contact.get(ADDRESSES).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, ADDRESSES);
        }

        // Add Birthday
        propertyValue = (contact.get(BIRTHDAY) != null) ? contact.get(BIRTHDAY).getAsString() : null;
        if (propertyValue != null) {
            // TODO: Current date is saved instead of the provided one. Check it.
            insertSinglePropertyValue(ops, propertyValue, BIRTHDAY);
        }

        // Add Emails
        propertyValues = (contact.get(EMAILS) != null) ? contact.get(EMAILS).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, EMAILS);
        }

        // Add Impps
        propertyValues = (contact.get(IMPPS) != null) ? contact.get(IMPPS).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, IMPPS);
        }

        // Add the name
        propertyObj = (contact.get(NAME) != null) ? contact.get(NAME).getAsJsonObject() : null;
        if (propertyObj != null) {
            insertPropertyValues(ops, propertyObj, NAME);
        }

        // Add Nickname
        propertyValue = (contact.get(NICKNAME) != null) ? contact.get(NICKNAME).getAsString() : null;
        if (propertyValue != null) {
            insertSinglePropertyValue(ops, propertyValue, NICKNAME);
        }

        // Add Note
        propertyValue = (contact.get(NOTE) != null) ? contact.get(NOTE).getAsString() : null;
        if (propertyValue != null) {
            insertSinglePropertyValue(ops, propertyValue, NOTE);
        }

        // Add Organization
        propertyObj = (contact.get(ORGANIZATION) != null) ? contact.get(ORGANIZATION).getAsJsonObject() : null;
        if (propertyObj != null) {
            insertPropertyValues(ops, propertyObj, ORGANIZATION);
        }

        // Add Phones
        propertyValues = (contact.get(PHONES) != null) ? contact.get(PHONES).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, PHONES);
        }

        // Add Photos
        propertyValues = (contact.get(PHOTOS) != null) ? contact.get(PHOTOS).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, PHOTOS);
        }

        // Add Urls
        propertyValues = (contact.get(URLS) != null) ? contact.get(URLS).getAsJsonArray() : null;
        if (propertyValues != null) {
            insertMultiValueProperty(ops, propertyValues, URLS);
        }

        // Save contact
        String contactId = null;
        ContentProviderResult[] result = activity.getContentResolver().applyBatch(ContactsContract.AUTHORITY, ops);

        if (result != null && result.length > 0) {
            Cursor c = activity.managedQuery(result[0].uri, new String[] { ContactsContract.RawContacts.CONTACT_ID }, null, null, null);
            if (c.moveToFirst()) {
                contactId = c.getString(c.getColumnIndex(ContactsContract.RawContacts.CONTACT_ID));
            }
        }
        return contactId;
    }
}
