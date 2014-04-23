package com.cloudpact.mowbly.android.log.handler;

import java.io.File;

import android.content.ContentValues;
import android.content.Context;
import android.database.sqlite.SQLiteDatabase;
import android.database.sqlite.SQLiteDiskIOException;
import android.database.sqlite.SQLiteException;
import android.database.sqlite.SQLiteOpenHelper;

import com.cloudpact.mowbly.android.Mowbly;
import com.cloudpact.mowbly.android.service.FileService;
import com.cloudpact.mowbly.log.LogEvent;
import com.cloudpact.mowbly.log.Logger;
import com.cloudpact.mowbly.log.handler.AbstractHandler;
/**
 * Database Handler for Logs
 * 
 * @author Aravind Baskaran <aravind@cloudpact.com>
 */
public class DatabaseHandler extends AbstractHandler {

    private static final String LOGS_DB_NAME = "logs";
    private static final int LOGS_DB_VERSION = 1;
    private static final String LOGS_TABLE_NAME = "logs";

    protected LogDatabaseHelper dbHelper;
    
    private static final Logger logger = Logger.getLogger();

    public DatabaseHandler(Context context) {
        FileService fs = Mowbly.getFileService();
        File path = new File(fs.getDocumentsDir(), LOGS_DB_NAME);
        setDatabaseHelper(context, path.getAbsolutePath());
    }

    protected void setDatabaseHelper(Context context, String path){
    	dbHelper = new LogDatabaseHelper(context, path);
    }
    
    @Override
    public synchronized void handle(LogEvent event) {
        SQLiteDatabase db = null;
        try {
            db = dbHelper.getWritableDatabase();
            db.insert(LOGS_TABLE_NAME, null, getContentValues(event));
        } catch (SQLiteException e) {
        } finally {
            if (db != null) {
                db.close();
            }
        }
    }

    protected ContentValues getContentValues(LogEvent event) {
        ContentValues values = new ContentValues();
        values.put("type", event.getType());
        values.put("level", event.getLevel().toString());
        values.put("tag", event.getTag());
        values.put("message", event.getMessage());
        values.put("timestamp", event.getTimestamp());
        return values;
    }

    public synchronized void clearLogs() {
        if (dbHelper != null) {
            SQLiteDatabase db = dbHelper.getWritableDatabase();
            dbHelper.onCreate(db);
        }
    }

    protected static class LogDatabaseHelper extends SQLiteOpenHelper {
    	
        public LogDatabaseHelper(Context context, String dbName) {
            super(context, dbName, null, LOGS_DB_VERSION);
        }
        
        protected String getCreateStatement(String tableName){
        	return "CREATE TABLE " + tableName + "(_id INTEGER PRIMARY KEY AUTOINCREMENT, type VARCHAR(255), level VARCHAR(50), tag TEXT, message LONGTEXT, timestamp LONGTEXT)";
        }
        
        protected String getDropStatement(String tableName){
        	return "DROP TABLE IF EXISTS " + tableName;
        }

        @Override
        public void onCreate(SQLiteDatabase db) {
        	try{
	            db.execSQL(getDropStatement(LOGS_TABLE_NAME));
	            db.execSQL(getCreateStatement(LOGS_TABLE_NAME));
        	}catch(SQLiteDiskIOException e){
        		logger.error("DatabaseLogHandler", "failed for table" + LOGS_TABLE_NAME  + ";Reason - " + e.getMessage());
        	}
        }

        @Override
        public void onUpgrade(SQLiteDatabase db, int oldVersion, int newVersion) {
            onCreate(db);
        }
    }
}
