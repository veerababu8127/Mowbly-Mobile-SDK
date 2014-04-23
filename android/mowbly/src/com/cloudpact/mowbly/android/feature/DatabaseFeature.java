package com.cloudpact.mowbly.android.feature;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.Iterator;
import java.util.List;
import java.util.UUID;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.content.Context;
import android.database.Cursor;
import android.database.CursorWindow;
import android.database.sqlite.SQLiteCursor;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteException;
import android.database.sqlite.SQLiteOpenHelper;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.util.GsonUtils;
import com.cloudpact.mowbly.feature.BaseFeature;
import com.cloudpact.mowbly.feature.Response;
import com.google.gson.JsonArray;
import com.google.gson.JsonElement;
import com.google.gson.JsonObject;

/**
 * Javascript interface for the Database feature
 * 
 * @author Prashanth Jonnala <prashanth@cloudpact.com>
 */
public class DatabaseFeature extends BaseFeature {

    public static final String NAME = "database";

    private HashMap<String, DatabaseHelper> helpers = new HashMap<String, DatabaseHelper>();

    public DatabaseFeature() {
        super(NAME);
    }

    /**
     * Open database for specified arguments. If not present, creates a new database
     * @param name
     * @param storageLevel
     * @param version
     * @param password
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "name", type = String.class),
        @Argument(name = "storageLevel", type = int.class),
        @Argument(name = "version", type = int.class),
        @Argument(name = "password", type = String.class)
    })
    public Response openDatabase(String name, int storageLevel, int version, String password) {
    	String path = getPath(name, storageLevel);
    	String uuid = createDatabaseHelper(path, version);
        Response response = new Response();
        response.setCode(1);
        response.setResult(uuid);

        return response;
    }
    
    protected String createDatabaseHelper(String path, int version){
    	Activity activity = ((FeatureBinder) binder).getActivity();
        String uuid = UUID.randomUUID().toString();
        DatabaseHelper helper = new DatabaseHelper(activity, path, version);
        helpers.put(uuid, helper);
        return uuid;
    }
    
    protected String getPath(String name, int storageLevel){
    	return Mowbly.createMowblyFile().setPath(name)
        .setStorageLevel(storageLevel)
        .getAbsolutePath();
    }

    @Method(async = true, args = {
        @Argument(name = "uuid", type = String.class)
    })
    public Response beginTransaction(String uuid) {
        Response response = new Response();
        response.setCode(0);
        response.setError("Not supported");
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "uuid", type = String.class)
    })
    public Response commit(String uuid) {
        Response response = new Response();
        response.setCode(0);
        response.setError("Not supported");
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "uuid", type = String.class)
    })
    public Response rollback(String uuid) {
        Response response = new Response();
        response.setCode(0);
        response.setError("Not supported");
        return response;
    }

    @Method(async = true, args = {
        @Argument(name = "uuid", type = String.class)
    })
    public Response close(String uuid) {
        Response response = new Response();
        response.setCode(0);
        response.setError("Not supported");
        return response;
    }
    
    protected DatabaseHelper getDatabaseHelper(String uuid){
    	return helpers.get(uuid);
    }
    
    /**
     * Execute SQL query on an opened database reference
     * @param options
     * @return Response
     */
    @Method(async = true, args = {
        @Argument(name = "options", type = JsonObject.class)
    })
    public Response executeQuery(JsonObject options) {
        Response response = new Response();

        String uuid = options.get("id").getAsString();
        String sql = options.get("sql").getAsString();
        String queryId = options.get("queryId").getAsString();
        JsonArray params = options.get("params") != null ? options.get("params").getAsJsonArray() : new JsonArray();

        if (helpers.containsKey(uuid)) {
            DatabaseHelper helper = getDatabaseHelper(uuid);

            if(helper == null){
            	return null;
            }

            response.setCode(1);

            JsonObject result = new JsonObject();
            result.addProperty("queryId", queryId);

            JsonObject data = new JsonObject();

            SQLiteDatabase db = null;
            Cursor cursor = null;
            try {
                db = helper.getWritableDatabase();

                if (isDDLQuery(sql)) {
                    db.execSQL(sql, getSelectionArgsFromParams(params));
                } else {
                    cursor = db.rawQuery(sql, getSelectionArgsFromParams(params));
                    data.add("rows", getCursorAsJson(cursor));
                    if (cursor != null) {
                        cursor.close();
                    }
                }

                result.add("data", data);
                
                cursor = db.rawQuery("select last_insert_rowid()", null);
                cursor.moveToFirst();
           	 	long insertId = cursor.getLong(0);
                data.addProperty("insertId", insertId);
                if (cursor != null) {
                    cursor.close();
                }
                
                cursor = db.rawQuery("SELECT changes()", null);
                long affectedRowCount = 0;
                if(cursor != null && cursor.getCount() > 0 && cursor.moveToFirst())
                {
                    affectedRowCount = cursor.getLong(0);
                }
                data.addProperty("rowsAffected", affectedRowCount);
            } catch (SQLiteException e) {
                response.setCode(100);
                response.setError(e.getMessage());
            } finally {
                if (cursor != null) {
                    cursor.close();
                }

                if (db != null) {
                    db.close();
                }

                helper.close();
            }

            response.setResult(result);
        } else {
            response.setCode(0);
            response.setError("No database found for " + uuid);
        }

        return response;
    }

    @SuppressLint("DefaultLocale")
	private boolean isDDLQuery(String sql) {
        String lsql = sql.toLowerCase();
        if (
                lsql.startsWith("drop") ||
                lsql.startsWith("create") ||
                lsql.startsWith("alter") ||
                lsql.startsWith("truncate")) {
            return true;
        }
        return false;
    }

    private String[] getSelectionArgsFromParams(JsonArray params) {
        String[] sargs = new String[params.size()];

        Iterator<JsonElement> iterator = params.iterator();        
        List<String> args = new ArrayList<String>();
        JsonElement arg;
        while (iterator.hasNext()) {
            arg = iterator.next();
            if (arg.isJsonNull()) {
                args.add("null");
            } else if (arg.isJsonPrimitive()) {
                args.add(arg.getAsString());
            } else {
                args.add(arg.toString());
            }
        }

        return args.toArray(sargs);
    }

    private JsonArray getCursorAsJson(Cursor cursor) {
        String[] names = cursor.getColumnNames();
        GsonUtils gsonUtils = Mowbly.getGsonUtils();
        JsonArray rows = new JsonArray();
        JsonObject row;
        for (boolean hasItem = cursor.moveToFirst(); hasItem; hasItem = cursor.moveToNext()) {
            row = new JsonObject();
            for (int i = 0, j = cursor.getColumnCount(); i < j; i++) {
            	row.add(names[i],  gsonUtils.getJsonElement(getValueByType(cursor,i)));
            }
            rows.add(row);
        }

        return rows;
    }
    
    @SuppressWarnings("deprecation")
   	private Object getValueByType(Cursor cursor, int i) {
           SQLiteCursor sqLiteCursor = (SQLiteCursor) cursor;
           CursorWindow cursorWindow = sqLiteCursor.getWindow();
           int pos = cursor.getPosition();
           if (cursorWindow.isNull(pos, i)) {
               return null;
           } else if (cursorWindow.isLong(pos, i)) {
           	return cursor.getLong(i);
           } else if (cursorWindow.isFloat(pos, i)) {
           	return cursor.getDouble(i);
           } else if (cursorWindow.isString(pos, i)) {
           	return cursor.getString(i);
           } else if (cursorWindow.isBlob(pos, i)) {
               return cursor.getBlob(i);
           }
           return cursor.getString(i);
       }

    public static class DatabaseHelper extends SQLiteOpenHelper {

        private final String mPath;

        public DatabaseHelper(Context context, String name, int version) {
            super(context, name, null, version);
            mPath = name;
        }

        public String getPath() {
            return mPath;
        }

        public long getDatabaseSize() {
            return new File(mPath).length();
        }

        @Override
        public void onCreate(SQLiteDatabase db) {
        }

        @Override
        public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
        }
    }
}
