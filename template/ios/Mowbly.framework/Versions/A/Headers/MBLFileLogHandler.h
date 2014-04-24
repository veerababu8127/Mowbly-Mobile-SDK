//
//  MBLFileHandler.h
//  Mowbly
//
//  Created by Sathish on 28/08/12.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import "MBLAbstractLogHandler.h"

@interface MBLFileLogHandler : MBLAbstractLogHandler {

    NSFileHandle *fileHandle;
    NSString *filePath;
}

- (id)initWithFile:(NSString *)theFilePath;

- (void)createLogFile;

- (void)deleteLogFile;

- (void)setFile:(NSString *)theFilePath;

@end