//
//  MBLConstants.m
//  Mowbly
//
//  Created by Sathish on 01/11/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLConstants.h"

@implementation MBLConstants

NSString* const ACCOUNTS                        = @"accounts";
NSString* const ACTION                          = @"action";
NSString* const ACTIVE_ACCOUNT                  = @"activeAccount";
NSString* const ANALYTICS                       = @"analytics";
NSString* const APP_ERROR_DOMAIN                = @"com.cloudpact.mowbly.ErrorDomain";
NSString* const AUTH_DOMAIN                     = @"domain";
NSString* const AUTH_MODE                       = @"authMode";
NSString* const CACHE_CLEARED_AT                = @"cacheClearedAt";
NSString* const CID                             = @"cid";
NSString* const CLIENT                          = @"client";
NSString* const CLIENT_SETTINGS                 = @"_client_settings_";
NSString* const CLIENT_SETTINGS_BAK             = @"_client_settings_bak_";
NSString* const COMPANY_ID                      = @"company_id";
NSString* const COMPANY_IDENTIFIER              = @"company_identifier";
NSString* const CONSTRAINT                      = @"constraint";
NSString* const CSTR_ALLOW_EMAIL                = @"allow_email";
NSString* const CSTR_ALLOW_SMS                  = @"allow_sms";
NSString* const CSTR_BLACKLISTED_URLS           = @"blacklisted_urls";
NSString* const CSTR_CACHE_CLEAR_FREQUENCY      = @"cache-clear-frequency";
NSString* const CSTR_CAMERA_ACCESS              = @"camera_access";
NSString* const CSTR_DB_ACCESS                  = @"database_access";
NSString* const CSTR_DELETE_CONTACT             = @"delete_contact";
NSString* const CSTR_GALLERY_READ_ACCESS        = @"gallery_read_access";
NSString* const CSTR_GALLERY_WRITE_ACCESS       = @"gallery_write_access";
NSString* const CSTR_GPS_ACCESS                 = @"gps_access";
NSString* const CSTR_HTTP_ACCESS                = @"http_access";
NSString* const CSTR_MAX_ACCOUNTS               = @"max_accounts";
NSString* const CSTR_MAX_CACHE_MEM              = @"max_cache_filesystem_size";
NSString* const CSTR_MAX_DB_SIZE                = @"max_database_size";
NSString* const CSTR_MAX_INTERNAL_MEM           = @"max_internal_filesystem_size";
NSString* const CSTR_MAX_PREF_SIZE              = @"max_preferences_size";
NSString* const CSTR_MAX_SMS                    = @"max_sms";
NSString* const CSTR_MULTIPLE                   = @"multiple";
NSString* const CSTR_NETWORK_TYPE               = @"network_type";
NSString* const CSTR_PROTOCOLS                  = @"protocols";
NSString* const CSTR_READ_ACCESS                = @"read_access";
NSString* const CSTR_READ_CONTACT               = @"read_contact";
NSString* const CSTR_SDCARD_ACCESS              = @"sdcard_access";
NSString* const CSTR_SYCHRONIZATION_FREQUENCY   = @"synchronization-frequency";
NSString* const CSTR_UPDATE_FREQUENCY           = @"update-frequency";
NSString* const CSTR_WHITELISTED_URLS           = @"whitelisted_urls";
NSString* const CSTR_WRITE_ACCESS               = @"write_access";
NSString* const CSTR_WRITE_CONTACT              = @"write_contact";
NSString* const CSTR_WRITE_SDCARD               = @"write_image_to_sdcard";
NSString* const DATA                            = @"data";
NSString* const DELETED_USERS                   = @"deletedUsers";
NSString* const DEVICE_ID                       = @"deviceId";
NSString* const DOWNLOADED_AT                   = @"downloadedAt";
NSString* const EXPIRE                          = @"expire";
NSString* const FEATURES                        = @"features";
NSString* const FORCE_INSTALL                   = @"force_install";
NSString* const FORCE_UPDATE                    = @"force_update";
NSString* const GLOBAL                          = @"__global";
NSString* const GUEST                           = @"guest";
NSString* const ID                              = @"id";
NSString* const INBOX_TABLE_NAME                = @"inbox";
NSString* const INSTALL                         = @"new";
NSString* const IS_SYSTEM_INSTALLED             = @"is_system_installed";
NSString* const IS_SYSTEM_REINSTALL             = @"is_system_reinstall";
NSString* const JS_MESSAGE                      = @"js_message";
NSString* const LANGUAGE                        = @"language";
NSString* const LOCAL                           = @"__local";
NSString* const LOGS_TABLE_NAME                 = @"logs";
NSString* const LOGS_DB_VERSION                 = @"1";
NSString* const LOGS_TYPE                       = @"type";
NSString* const LOGS_LEVEL                      = @"level";
NSString* const LOGS_TAG                        = @"tag";
NSString* const LOGS_MESSAGE                    = @"message";
NSString* const LOGS_TIMESTAMP                  = @"timestamp";
NSString* const MESSAGE                         = @"message";
NSString* const METADATA                        = @"metadata";
NSString* const MOWBLY_BUILD_HEADER             = @"mowbly-build";
NSString* const MOWBLY_PREFERENCES              = @"mowblyPreferences";
NSString* const MOWBLY_UNAME_HEADER             = @"mowbly-uname";
NSString* const MOWBLY_VERSION_HEADER           = @"mowbly-version";
NSString* const NETWORK                         = @"network";
NSString* const NO_OF_LAUNCHES                  = @"no_of_launches";
NSString* const NO_OF_LOGINS                    = @"no_of_logins";
NSString* const OFFLINE_MESSAGES                = @"offlineMessages";
NSString* const OFFLINE_MESSAGES_DB_NAME        = @"offline";
NSString* const OFFLINE_MESSAGES_DB_VERSION     = @"1";
NSString* const OFFLINE_MESSAGES_TABLE_NAME     = @"offline";
NSString* const ORIENTATION                     = @"orientation";
NSString* const OUTBOX_TABLE_NAME               = @"outbox";
NSString* const PAGE_ICON                       = @"pageIcon";
NSString* const PAGE_NAME                       = @"pageName";
NSString* const PAGE_PARENT                     = @"pageParent";
NSString* const PAGE_URL                        = @"pageUrl";
NSString* const PARAMETER_IDENTIFIER            = @"identifier";
NSString* const PARAMETER_CID                   = @"cid";
NSString* const PARAMETER_SPACE                 = @"space";
NSString* const PARAMETER_UNAME                 = @"uname";
NSString* const PARAMETER_PWD                   = @"pwd";
NSString* const PARAMETER_DEVICEID              = @"deviceid";
NSString* const PARENT_PAGE_SPACE               = @"parentSpace";
NSString* const PASSWORD                        = @"password";
NSString* const PASSWORD_PLACEHOLDER            = @"$m__password__$m";
NSString* const POLICY                          = @"policy";
NSString* const POL_ACCOUNTS                    = @"Accounts";
NSString* const POL_ATTR                        = @"pol_attr";
NSString* const POL_ATTR_NO_OF_SMS              = @"pol_attr_num_sms";
NSString* const POL_ATTRS_CAMERA                = @"pol_attrs_camera";
NSString* const POL_ATTRS_CONTACT               = @"pol_attrs_contacts";
NSString* const POL_ATTRS_CLIENT_FILES          = @"pol_attrs_client_files";
NSString* const POL_ATTRS_CLIENT_LOGS           = @"pol_attrs_client_logs";
NSString* const POL_ATTRS_DATBASE               = @"pol_attrs_database";
NSString* const POL_ATTRS_FILE                  = @"pol_attrs_files";
NSString* const POL_ATTRS_HTTP                  = @"pol_attrs_http";
NSString* const POL_ATTRS_LOCATION              = @"pol_attrs_location";
NSString* const POL_ATTRS_MESSAGE               = @"pol_attrs_message";
NSString* const POL_ATTRS_NETWORK               = @"pol_attrs_network";
NSString* const POL_ATTRS_POLICYMANAGER         = @"pol_attrs_policy_manager";
NSString* const POL_ATTRS_PREFERENCE            = @"pol_attrs_preferences";
NSString* const POL_ATTRS_SYSTEM_ANALYTICS      = @"pol_attrs_system_analytics";
NSString* const POL_ATTRS_SYSTEMMANAGER         = @"pol_attrs_system_manager";
NSString* const POL_CAMERA                      = @"Camera";
NSString* const POL_CLIENT                      = @"client_policy";
NSString* const POL_CLIENT_FILES                = @"File";
NSString* const POL_CLIENT_LOGS                 = @"Logs";
NSString* const POL_CONTACTS                    = @"Contacts";
NSString* const POL_DATABASE                    = @"Database";
NSString* const POL_FILE                        = @"Files";
NSString* const POL_HTTP                        = @"Http";
NSString* const POL_LOCATION                    = @"Location";
NSString* const POL_MESSAGE                     = @"Message";
NSString* const POL_NETWORK                     = @"Network";
NSString* const POL_PASSWORD                    = @"Password";
NSString* const POL_POLICY_MGR                  = @"Policy Manager";
NSString* const POL_PREFERENCES                 = @"Preferences";
NSString* const POL_SYSTEM_ANALYTICS            = @"Analytics";
NSString* const POL_SYSTEM_MGR                  = @"System Manager";
NSString* const PKEY                            = @"pkey";
NSString* const PREF_APNS_TOKEN                 = @"apns_token";
NSString* const PREFERENCES                     = @"preferences";
NSString* const REQUESTED_AT                    = @"requestedAt";
NSString* const REVOKE                          = @"revoke";
NSString* const ROLES                           = @"roles";
NSString* const SERVICE_URL                     = @"service_url";
NSString* const SPACE                           = @"space";
NSString* const SPACE_DEV                       = @"dev";
NSString* const SPACE_PROD                      = @"prod";
NSString* const SPACE_TEST                      = @"test";
NSString* const STATUS                          = @"status";
NSString* const SYNCHRONIZED_AT                 = @"synchronizedAt";
NSString* const SYSTEM                          = @"system";
NSString* const SYSTEM_BAK                      = @"system_bak";
NSString* const SYSTEM_BUILD                    = @"system_build";
NSString* const SYSTEM_DOWNLOADED_BUILD         = @"system_downloaded_build";
NSString* const SYSTEM_DOWNLOADED_VERSION       = @"system_downloaded_version";
NSString* const SYSTEM_VERSION                  = @"system_version";
NSString* const TIMESTAMP                       = @"timestamp";
NSString* const TITLE                           = @"title";
NSString* const TRANSLATIONS                    = @"translations";
NSString* const TYPE                            = @"type";
NSString* const UPDATE                          = @"update";
NSString* const UPDATED_AT                      = @"updatedAt";
NSString* const URL                             = @"url";
NSString* const USER                            = @"user";
NSString* const USERNAME_PLACEHOLDER            = @"$m__username__$m";
NSString* const USERS                           = @"users";
NSString* const USERNAME                        = @"userName";
NSString* const USERNAME_AUTH                   = @"username";

int const ACTION_CUSTOM_MESSAGE                 = 0;
int const ACTION_REMOTE_WIPE                    = 1;
int const CAMERA_BACK                           = 0;
int const CAMERA_FRONT                          = 1;
int const SOURCE_PHOTO_LIBRARY                  = 0;
int const SOURCE_CAMERA                         = 1;
int const SOURCE_PHOTO_ALBUM                    = 2;
int const STATUS_DEVICE_ACKNOWLEDGED            = 1;
int const STATUS_RECEIVED                       = 0;
int const STATUS_SYNC_PENDING                   = 0;

@end