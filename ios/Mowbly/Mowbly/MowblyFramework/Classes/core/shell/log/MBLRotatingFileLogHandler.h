//
//  MBLRotatingFileLogHandler.h
//  Mowbly
//
//  Created by Sathish on 05/09/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLFileLogHandler.h"

@interface MBLRotatingFileLogHandler : MBLFileLogHandler {

    int _maxBackupFiles;       // Max number of backup files
    long _maxFileSize;         // Max file size
}

@property (nonatomic, assign)int maxBackupFiles;
@property (nonatomic, assign)long maxFileSize;

@end