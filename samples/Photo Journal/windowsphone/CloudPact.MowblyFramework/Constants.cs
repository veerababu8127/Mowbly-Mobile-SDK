//-----------------------------------------------------------------------------------------
// <copyright file="Constants.cs" company="CloudPact Software Technologies (P) Ltd.">
//     Copyright (c) 2011-2014 CloudPact. All rights reserved.
// </copyright>
// <author>Sathish(sathish@cloudpact.com)</author>
// <website>http://www.mowbly.com</website>
//-----------------------------------------------------------------------------------------

namespace CloudPact.MowblyFramework
{
    static class Constants
    {
        

        

        //UserName and Password Placeholder

        internal const string USERNAME_PLACEHOLDER = "$m__username__$m";
        internal const string PASSWORD_PLACEHOLDER = "$m__password__$m";

        // App
        internal const string DIR_RESOURCES = "Resources";
        internal const string PROPERTY_APP_TYPE = "APP_TYPE";

        // Background transfer
        internal const string STRING_BACKGROUND_TRANSFER_WAITING = "BACKGROUND_TRANSFER_WAITING";

        // Camera feature
        internal const string KEY_CAMERA_CAPTURE_TASK = "Camera capture task";
        internal const string KEY_PHOTO_CHOOSER_TASK = "Photo chooser task";

        internal const string STRING_CAMERA_CAPTURE_ERROR = "CAMERA_CAPTURE_ERROR";
        internal const string STRING_CAMERA_FORMAT_ERROR = "CAMERA_FORMAT_ERROR";
        internal const string STRING_CAMERA_INITIALIZATION_ERROR = "CAMERA_INITIALIZATION_ERROR";
        internal const string STRING_CAMERA_PROCESS_PICTURE_ERROR = "CAMERA_PROCESS_PICTURE_ERROR";
        internal const string STRING_CAMERA_WRITE_PICTURE_ERROR = "CAMERA_WRITE_PICTURE_ERROR";

        // Contacts feature
        internal const string STRING_CONTACT_CHOOSE_FAILED = "CONTACT_CHOOSE_FAILED";
        internal const string STRING_CONTACT_DELETE_ERROR = "CONTACT_DELETE_ERROR";
        internal const string STRING_CONTACT_NOT_FOUND = "CONTACT_NOT_FOUND";
        internal const string STRING_CONTACTS_LOADING = "CONTACTS_LOADING";
        internal const string STRING_CONTACT_SAVE_FAILED = "CONTACT_SAVE_FAILED";
        internal const string STRING_CONTACT_SEARCH_PARAM_MISSING = "CONTACT_SEARCH_PARAM_MISSING";
        internal const string STRING_CONTACT_VIEW_FAILED = "CONTACT_VIEW_FAILED";

        // Database feature
        internal const string DIR_DB = "__d_b__";
        internal const string STRING_DATABASE_NO_CONNECTION_OPEN_ERROR = "DATABASE_NO_CONNECTION_OPEN_ERROR";
        internal const string STRING_DATABASE_NO_TRANSACTION_ACTIVE_ERROR = "DATABASE_NO_TRANSACTION_ACTIVE_ERROR";
        internal const string STRING_DATABASE_OPEN_ERROR = "DATABASE_OPEN_ERROR";
        internal const string STRING_DATABASE_OPERATION_ERROR = "DATABASE_OPERATION_ERROR";
        internal const string STRING_DATABASE_QUERY_ERROR = "DATABASE_QUERY_ERROR";
        internal const string STRING_DATABASE_TRANSACTION_ERROR = "DATABASE_TRANSACTION_ERROR";

        // Device feature
        internal const string STRING_UNAVAILABLE = "Unavailable";

        // Features
        internal const string FEATURE_CAMERA = "Camera feature";
        internal const string FEATURE_DATABASE = "Database feature";
        internal const string FEATURE_DEVICE = "Device feature";
        internal const string FEATURE_FILE = "File feature";
        internal const string FEATURE_FRAMEWORK = "Framework feature";
        internal const string FEATURE_GEOLOCATION = "geolocation feature";
        internal const string FEATURE_HTTP = "Http feature";
        internal const string FEATURE_LOG = "Log feature";
        internal const string FEATURE_MESSAGE = "Message feature";
        internal const string FEATURE_NETWORK = "Network feature";
        internal const string FEATURE_PREFERENCES = "Preferences feature";
        internal const string FEATURE_UI = "Ui feature";


        internal const string KEY_GLOBAL = "__global";

        // File system
        internal const string KEY_RESOURCES = "resources";

        internal const string PROPERTY_DIR_APP = "DIR_APP";
        internal const string PROPERTY_DIR_BAK = "DIR_BAK";
        internal const string PROPERTY_DIR_CACHE = "DIR_CACHE";
        internal const string PROPERTY_DIR_DOCS = "DIR_DOCS";
        internal const string PROPERTY_DIR_LOGS = "DIR_LOGS";
        internal const string PROPERTY_DIR_SHARED_TRANSFER = "DIR_SHARED_TRANSFER";
        internal const string PROPERTY_DIR_TMP = "DIR_TMP";

        internal const string STRING_EXTERNAL_STORAGE_ERROR = "EXTERNAL_STORAGE_ERROR";
		internal const string STRING_FILE_NOT_FOUND = "FILE_NOT_FOUND";

        //Log 
        internal const string PROPERTY_HTTP_REQUEST_TIMEOUT = "HTTP_REQUEST_TIMEOUT";

        // General
        internal const string PROPERTY_LOG_DEFAULT_THRESHOLD = "LOG_DEFAULT_THRESHOLD";

        internal const string STRING_EMPTY = "";
        internal const string STRING_ACTIVITY_CANCELLED = "ACTIVITY_CANCELLED";
        internal const string STRING_NO_CONNECTIVITY = "NO_CONNECTIVITY";
        internal const string STRING_UNKNOWN_ERROR = "UNKNOWN_ERROR";
        internal const string STRING_HOST_NOT_REACHABLE = "HOST_NOT_REACHABLE";

        // Logs
        internal const string PROPERTY_LOGS_DB = "LOGS_DB";
        internal const string PROPERTY_LOGS_DB_VERSION = "LOGS_DB_VERSION";
        internal const string PROPERTY_LOGS_FILE = "LOGS_FILE";

        
        internal const string STRING_EMPTY_JSON_ARRAY = "[]";
        internal const string STRING_EMPTY_JSON_DICTIONARY = "{}";        
        internal const string PROPERTY_APP_DESCRIPTION = "APP_DESCRIPTION";

        //Version Info

        internal const string PROPERTY_APP_NAME = "APP_NAME";
        internal const string PROPERTY_APP_VERSION = "APP_VERSION";
        internal const string PROPERTY_FRAMEWORK = "FRAMEWORK";
        internal const string PROPERTY_FRAMEWORK_VERSION = "FRAMEWORK_VERSION";

        // Page management
        internal const string KEY_IS_DEVICE_BACKPRESS = "is_device_backpress";

        internal const string PROPERTY_FEATURES = "FEATURES";
        internal const string PROPERTY_TITLE_HOME_PAGE = "TITLE_HOME_PAGE";
        internal const string PROPERTY_URL_HOME_PAGE = "URL_HOME_PAGE";
        internal const string PROPERTY_URL_PAGE_NOT_FOUND = "URL_PAGE_NOT_FOUND";
        //Log 
        internal const string KEY_LOG_TYPE = "type";
        internal const string KEY_LOG_LEVEL = "level";
        internal const string KEY_LOG_TAG = "tag";
        internal const string KEY_LOG_MESSAGE = "message";
        internal const string KEY_LOG_TIMESTAMP = "timestamp";
    }
}