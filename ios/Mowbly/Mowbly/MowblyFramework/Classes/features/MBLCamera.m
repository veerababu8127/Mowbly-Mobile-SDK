
//
//  MBLCamera.m
//  Mowbly
//
//  Created by Sathish on 09/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//

#import <AssetsLibrary/AssetsLibrary.h>
#import <MobileCoreServices/UTCoreTypes.h>
#import "MBLApp.h"
#import "MBLAppDelegate.h"
#import "MBLCamera.h"
#import "MBLConstants.h"
#import "MBLDevice.h"
#import "MBLFeatureBinder.h"
#import "MBLFeatureResult.h"
#import "MBLFileManager.h"
#import "MBLPage.h"
#import "MBLJSMessage.h"
#import "MBLPageViewController.h"
#import "MBLUtils.h"
#import "ImageHelpers.h"
#import "QSStrings.h"

static int activityCancelled = -1;

@interface MBLCamera (utils)

- (void)dismissPickerController:(UIImagePickerController *)picker;

- (NSData *)formatImage:(UIImage *)sourceImage withType:(IMAGE_TYPE)type andQuality:(CGFloat)quality;

- (NSString *)getPathForFileObject:(NSDictionary *)file;

- (NSDictionary *)getPropertiesForCameraType:(int)type;

- (UIImage*)imageWithImage:(UIImage*)sourceImage scaledToWidth:(float)i_width;

- (UIImage*)imageWithImage:(UIImage*)sourceImage scaledToHeight:(float)i_height;

- (UIImage *)scaleImage:(UIImage *)sourceImage toWidth:(unsigned int)width andHeight:(unsigned int)height;

- (void)sendMessageToJavascriptWithResult:(NSDictionary *)result orError:(NSError *)error;

- (void)sendMessageToJavascriptWithResult:(NSDictionary *)result orError:(NSError *)error withCallback:(NSString *)callbackId;

- (NSString *)writeToTempDirImageData:(NSData *)data ofType:(IMAGE_TYPE)type error:(NSError **)error;

@end

@implementation MBLCamera

- (NSString *)getPathForFileObject:(NSDictionary *)file
{
    NSString *filePath = [file objectForKey:@"path"];
    int storageType = [[file objectForKey:@"storageType"] intValue];
    int level = [[file objectForKey:@"level"] intValue];
    NSString *rootDir = [[MBLFileManager defaultManager] absoluteDirectoryPathForLevel:level
                                                                               storage:storageType
                                                                               andFeature:self];
    filePath = ([filePath hasPrefix:@"/"]) ? [rootDir stringByAppendingString:filePath] : [rootDir stringByAppendingPathComponent:filePath];
    
    NSString *parentDir = [filePath stringByDeletingLastPathComponent];
    NSFileManager *fileManager = [[NSFileManager alloc] init];
    NSError *error = nil;
    BOOL isDir;

    
    if (! [fileManager fileExistsAtPath:parentDir isDirectory:&isDir] || !isDir)
    {
        if(! [fileManager createDirectoryAtPath:parentDir withIntermediateDirectories:YES attributes:nil error:&error]) {
            
            LogError(@"Camera: Creation of destination directory %@ failed. Reason - %@", parentDir, [error description]);
        }
    }
    
    return filePath;
} // getPathForFileObject

- (void)invoke:(MBLJSMessage *)message {
    
    callbackId = [message.callbackId copy];

	if ([message.method isEqualToString:@"getPicture"])
    {
		int iSourceType = [(NSNumber *)[message.args objectAtIndex:0] intValue];
		UIImagePickerControllerSourceType sourceType = 
		(iSourceType == SOURCE_CAMERA) ? UIImagePickerControllerSourceTypeCamera : 
		(iSourceType == SOURCE_PHOTO_LIBRARY) ? UIImagePickerControllerSourceTypePhotoLibrary : 
		UIImagePickerControllerSourceTypeSavedPhotosAlbum;
        
        if (iSourceType == SOURCE_CAMERA)
        {
            LogInfo(@"Taking picture from camera");
            
            // Retain page on low memory conditions.
            [[[self appDelegate] appViewController] setRetainControllerOnLowMemory:YES];
        }
        else
        {
            LogInfo(@"Choosing picture from gallery/album");
        }
        
        if ([UIImagePickerController isSourceTypeAvailable:sourceType])
        {
            options = (NSDictionary *)[message.args objectAtIndex:1];
            bReadData = [(NSNumber *)[options objectForKey:@"readData"] boolValue];
            bool bAllowEdit = [[options objectForKey:@"allowEdit"] boolValue];
            
            cameraUI = [[UIImagePickerController alloc] init];
            cameraUI.sourceType = sourceType;
            cameraUI.mediaTypes = [NSArray arrayWithObject:(NSString *)kUTTypeImage];
            cameraUI.allowsEditing = bAllowEdit;
            cameraUI.delegate = self;
            
            UIDevice* thisDevice = [UIDevice currentDevice];
            if(thisDevice.userInterfaceIdiom == UIUserInterfaceIdiomPad)
            {
                // iPad
                popoverUI = [[UIPopoverController alloc] initWithContentViewController:cameraUI];
                popoverUI.delegate = self;
                
                CGPoint point = [[[MBLFeatureBinder defaultBinder] view] center];
                [popoverUI presentPopoverFromRect:CGRectMake(point.x/4, 0, 10, 50)
                                           inView:[[MBLFeatureBinder defaultBinder] view]
                         permittedArrowDirections:UIPopoverArrowDirectionAny
                                         animated:YES];
            }
            else
            {
                [[[self appDelegate] appViewController] setModalPresentationStyle:UIModalPresentationFullScreen];
                [[[self appDelegate] appViewController] presentViewController:cameraUI animated:YES completion:nil];   
            }
        }
        else
        {
            NSError *error = [MBLUtils errorWithCode:[NSNumber numberWithInt:0]
                                         description:(iSourceType == SOURCE_CAMERA) ? 
                              MBLLocalizedString(@"CAMERA_NOT_AVAILABLE") : 
                              MBLLocalizedString(@"PHOTO_LIBRARY_NOT_AVAILABLE")];
            [self sendMessageToJavascriptWithResult:nil orError:error];
        }
	}
    else if ([message.method isEqualToString:@"getConfiguration"])
    {
        NSMutableArray *response = [NSMutableArray array];
        NSDictionary *oCameraBack = [self getPropertiesForCamera:CAMERA_BACK];
        if(oCameraBack) {

            [response addObject:oCameraBack];
        }
        NSDictionary *oCameraFront = [self getPropertiesForCamera:CAMERA_FRONT];
        if(oCameraFront) {

            [response addObject:oCameraFront];
        }
        [self pushResponseToJavascript:response withCode:RESPONSE_CODE_OK andCallBackId:message.callbackId];
    } else  {

		// Not a supported method
		LogError(@"Feature %@ does not support method %@.", message.feature, message.method);
	}
} // invoke

#pragma mark -
#pragma mark Utils

- (void)dismissPickerController:(UIImagePickerController *)picker {

    // // ios 5+
    if([picker respondsToSelector:@selector(presentingViewController)]) {

        [[picker presentingViewController] dismissViewControllerAnimated:YES completion:nil];
    } else {

        [[picker parentViewController] dismissModalViewControllerAnimated:YES];
    }
} // dismissPickerController

- (NSData *)formatImage:(UIImage *)image withType:(IMAGE_TYPE)type andQuality:(CGFloat)quality {

    NSData *picData = nil;
    if (type == PNG) {
        
        picData = UIImagePNGRepresentation(image);
    }
    else
    {
        picData = UIImageJPEGRepresentation(image, quality);
    }
    return picData;
} // formatImage:withType:andQuality

- (NSDictionary *)getPropertiesForCamera:(int)type {
    
    NSMutableDictionary *result = nil;
    UIImagePickerControllerCameraDevice camera = (type == CAMERA_BACK) ? UIImagePickerControllerCameraDeviceRear : UIImagePickerControllerCameraDeviceFront;
    if ([UIImagePickerController isCameraDeviceAvailable:camera]) {
        
        result = [NSMutableDictionary dictionaryWithCapacity:3];
        [result setObject:[NSNumber numberWithInt:type] forKey:@"type"];
        [result setObject:[UIImagePickerController isFlashAvailableForCameraDevice:camera] ? [NSNumber numberWithBool:YES] : [NSNumber numberWithBool:NO] forKey:@"flash"];
        [result setObject:[NSArray array] forKey:@"resolutions"];
    }
    
    return result;
} // getPropertiesForCamera:

- (UIImage*)imageWithImage:(UIImage*)sourceImage scaledToWidth:(float)i_width {

    float oldWidth = sourceImage.size.width;
    float scaleFactor = i_width / oldWidth;

    float newHeight = sourceImage.size.height * scaleFactor;
    float newWidth = oldWidth * scaleFactor;

    return [self scaleImage:sourceImage toWidth:newWidth andHeight:newHeight];
} // imageWithImage:scaledToWidth:

- (UIImage*)imageWithImage:(UIImage*)sourceImage scaledToHeight:(float)i_height {

    float oldHeight = sourceImage.size.height;
    float scaleFactor = i_height / oldHeight;

    float newWidth = sourceImage.size.width * scaleFactor;
    float newHeight = oldHeight * scaleFactor;

    return [self scaleImage:sourceImage toWidth:newWidth andHeight:newHeight];
} // imageWithImage:scaledToHeight:

- (UIImage *)scaleImage:(UIImage *)sourceImage toWidth:(unsigned int)width andHeight:(unsigned int)height {

    // Process the image for user requirements
    UIImage *targetImage = nil;
	CGSize sourceSize = sourceImage.size;
	CGSize targetSize = {width, height};
	float scale = GetScaleForProportionalResize(sourceSize, targetSize, false, false);
    CGImageRef scaledCGImage = CreateCGImageFromUIImageScaled(sourceImage, scale);
    if (scaledCGImage) {
        
        targetImage = [UIImage imageWithCGImage:scaledCGImage];
        CGImageRelease(scaledCGImage);
    }
    if (! targetImage) {
        
        LogError(@"Processing image failed. Returning original image.");
        targetImage = sourceImage;
    }
    
    return targetImage;
} // scaleImage:toWidth:andHeight

- (void)sendMessageToJavascriptWithResult:(NSDictionary *)result orError:(NSError *)error {

    [self sendMessageToJavascriptWithResult:result orError:error withCallback:callbackId];
    
} // sendMessageToJavascriptWithResult:orError

- (void)sendMessageToJavascriptWithResult:(NSDictionary *)result orError:(NSError *)error withCallback:(NSString *)_callbackId
{
    if (error) {
        
		// Raise event to javascript
        NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_ERROR
                                                        error:error] toCallbackString:_callbackId];
        
		[self pushJavascriptMessage:message];
	}
    else
    {
        // Raise event to javascript
        NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK
                                                       result:result] toCallbackString:_callbackId];
        [self pushJavascriptMessage:message];
	}
} // sendMessageToJavascriptWithResult:orError:withcallback:

- (NSString *)writeToTempDirImageData:(NSData *)data ofType:(IMAGE_TYPE)type error:(NSError **)error {

    NSString *cacheDir = [[MBLFileManager defaultManager] absoluteDirectoryPathForLevel:APP_ROOT
                                                                                storage:FILE_CACHE
                                                                                andFeature:self ];
    NSString *fileExtn = (type == PNG) ? @"png" : @"jpg";
    NSString *filePath = [[cacheDir stringByAppendingFormat:@"/pic%f",[[NSDate date] timeIntervalSince1970]] stringByAppendingPathExtension:fileExtn];

    NSFileManager *fileManager = [[NSFileManager alloc] init];
    BOOL isDir;
    if (! [fileManager fileExistsAtPath:cacheDir isDirectory:&isDir] || !isDir)
    {
        if(! [fileManager createDirectoryAtPath:cacheDir withIntermediateDirectories:YES attributes:nil error:error]) {

            LogError(@"Error creating cache directory %@; Reason - %@", cacheDir, [*error localizedDescription]);
            return filePath;
        }
    }
    if(! [data writeToFile:filePath options:NSDataWritingAtomic error:*&error]) {

        if (*error) {

            LogError(@"Error writing photo into tmp file %@; Reason - %@", filePath, [*error localizedDescription]);
        }
    }

    return filePath;
} // writeToTempDirTmageData:ofType:error

#pragma mark -
#pragma mark UIImagePickerControllerDelegate methods

- (void)imagePickerControllerDidCancel:(UIImagePickerController *)picker {

	NSError *error = nil;
	bool bSuccess = NO;
	if (picker.sourceType == UIImagePickerControllerSourceTypeCamera) {

        LogInfo(@"Taking picture activity cancelled");
		if (photos && ([photos count] > 0)) {

			bSuccess = YES;

			id result = (bReadData) ? [NSMutableDictionary dictionaryWithCapacity:[photos count]] : 
            [NSMutableArray arrayWithCapacity:[photos count]];
			// Canceled after user selected mutiple. Collect the data and raise JS Event
			[photos enumerateKeysAndObjectsUsingBlock:^(id key, id obj, BOOL *stop) {

				NSString *photoPath = (NSString *)key;
				if (bReadData) {

					NSData *picData = (NSData *)obj;
					[result setObject:picData forKey:photoPath];
				} else {

					[result addObject:photoPath];
				}
			}];

			// Raise JS event
            NSString *message = [[MBLFeatureResult resultWithCode:self.RESPONSE_CODE_OK
                                                           result:result] toCallbackString:callbackId];
			[self pushJavascriptMessage:message];
		}
        
        // Cancel retain page on low memory conditions.
        [[[self appDelegate] appViewController] setRetainControllerOnLowMemory:NO];
	} else {

        LogInfo(@"Choosing picture activity cancelled");
    }

	if (! bSuccess) {

		error = [MBLUtils errorWithCode:[NSNumber numberWithInt:activityCancelled] description:MBLLocalizedString(@"ACTIVITY_CANCELED")];
		[self sendMessageToJavascriptWithResult:nil orError:error];
	}
	// Reset variables
	bReadData = nil;
	options = nil;
	photos = nil;
	
    // Release viewcontrollers
    if (popoverUI) {

        [popoverUI dismissPopoverAnimated:YES];
    }
    
    // Dismiss the picker
    [self dismissPickerController:picker];
    
} // imagePickerControllerDidCancel

- (void)imagePickerController:(UIImagePickerController *)picker
didFinishPickingMediaWithInfo:(NSDictionary *)info {
    
    /*
     if(camera || bReaddata)
     picdata = Convert and compress as per spec
     else
     picdata = picked image
     if(camera)
     if(filepath)
     Store in file path
     else
     Store in gallery
     Copy into tmp folder and set as path
     
     Return path of original image and if readData, the compressed data str
     
     else
     copy into tmp folder and set as path
     Return path of original image and if readData, the compressed data str
     */
    
    if (popoverUI)
    {
        [popoverUI dismissPopoverAnimated:YES];
    }
    // Dismiss the picker
    [self dismissPickerController:picker];
    
    NSDictionary *pickDict = [NSDictionary dictionaryWithObjectsAndKeys:picker,@"picker",info,@"info",options,@"options",callbackId,@"callbackId", nil];
    [self performSelectorInBackground:@selector(pickImage:) withObject:pickDict];
    
} // imagePickerController:didFinishPickingMediaWithInfo

- (void) pickImage:(NSDictionary *)pickDict
{
    NSDictionary *result = nil;
    NSError *error = nil;
    
    UIImagePickerController *picker = [pickDict valueForKey:@"picker"];
    NSDictionary *info = [pickDict valueForKey:@"info"];
    NSDictionary *_options = [pickDict valueForKey:@"options"];
    NSString *_callbackId = [pickDict valueForKey:@"callbackId"];
    
    BOOL bIsSourceCamera = picker.sourceType == UIImagePickerControllerSourceTypeCamera;
    int iType = [[_options objectForKey:@"type"] intValue];
    NSData *picData = nil;
    // Convert and compress
    UIImage *originalImage, *editedImage, *sourceImage;
    editedImage = (UIImage *) [info objectForKey:UIImagePickerControllerEditedImage];
    originalImage = (UIImage *) [info objectForKey:UIImagePickerControllerOriginalImage];
    sourceImage = (editedImage) ? editedImage : originalImage;
    
    if(bIsSourceCamera || bReadData) {
        
        // Scale the image
        unsigned int width = [[_options objectForKey:@"width"] unsignedIntValue];
        unsigned int height = [[_options objectForKey:@"height"] unsignedIntValue];
        float fQuality = [[_options objectForKey:@"quality"] floatValue]/100.0;

        UIImage *scaledImg = nil;
        if(width == -1) {

            if(height == -1) {

                // Auto width and height. Do nothing.
                scaledImg = sourceImage;
            } else {

                // auto width, scale by height.
                scaledImg = [self imageWithImage:sourceImage scaledToHeight:height];
            }
        } else {

            if(height == -1) {

                // auto height, scale by width.
                scaledImg = [self imageWithImage:sourceImage scaledToWidth:width];
            } else {

                // User has mentioned required size. Scale to it.
                scaledImg = [self scaleImage:sourceImage toWidth:width andHeight:height];
            }
        }

        picData = [self formatImage:scaledImg
                           withType:iType
                         andQuality:fQuality];
    } else {

        NSString *assetPath = [[info objectForKey:UIImagePickerControllerReferenceURL] absoluteString];
        NSRange range = [assetPath rangeOfString:@"&ext=" options:NSBackwardsSearch];
        NSString *fileExtn = [assetPath substringFromIndex:(range.location + range.length)];
        iType = ([fileExtn isEqualToString:@"JPG"]) ? JPEG : PNG;
        picData = [self formatImage:sourceImage withType:iType andQuality:1.0];
    }
    
    if(picData) {
        
        NSString *base64Data = @"";
        if(bReadData && picData) {
            
            base64Data = [QSStrings encodeBase64WithData:picData];
        }
        
        if(bIsSourceCamera) {
            
            NSDictionary *file = [_options objectForKey:@"filePath"];
            if(! file) {
                
                // Store in gallery
                ALAssetsLibrary* library = [[ALAssetsLibrary alloc] init];
                [library writeImageDataToSavedPhotosAlbum:picData
                                                 metadata:[info objectForKey:UIImagePickerControllerMediaMetadata]
                                          completionBlock:^(NSURL *assetURL, NSError *error) {
                                              
                                              NSDictionary *result = nil;
                                              if(error) {
                                                  
                                                  LogError(@"Error writing picture to photo gallery; Reason - %@", [error localizedDescription]);
                                              } else {
                                                  
                                                  // Asset path cannot be set to image as src. So temporarily save the image and send the path.
                                                  NSString *filePath = [[self writeToTempDirImageData:picData ofType:iType error:&error] copy];
                                                  if (! error) {
                                                      
                                                      result = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:filePath, base64Data, nil]
                                                                                           forKeys:[NSArray arrayWithObjects:@"path", @"data", nil]];
                                                  }
                                              }
                                              NSDictionary *resultDict = [NSDictionary dictionaryWithObjectsAndKeys:result,@"result",_callbackId,@"callbackId",error,@"error", nil];
                                              [self performSelectorOnMainThread:@selector(sendJsMessage:) withObject:resultDict waitUntilDone:NO];
                                          }];
                
            } else {
                
                // Save the image inside the page dir.
                NSString *filePath = [self getPathForFileObject:file];
                [picData writeToFile:filePath options:NSDataWritingAtomic error:&error];
                if (error) {
                    
                    LogError(@"Error writing photo to file %@; Reason - %@", filePath, [error localizedDescription]);
                } else {
                    
                    result = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:filePath, base64Data, nil]
                                                         forKeys:[NSArray arrayWithObjects:@"path", @"data", nil]];
                    NSDictionary *resultDict = [NSDictionary dictionaryWithObjectsAndKeys:result,@"result",_callbackId,@"callbackId",error,@"error", nil];
                    [self performSelectorOnMainThread:@selector(sendJsMessage:) withObject:resultDict waitUntilDone:NO];
                }
            }
            
            // Cancel retain page on low memory conditions.
            [[[self appDelegate] appViewController] setRetainControllerOnLowMemory:NO];
        }
        else
        {
            LogDebug(@"Asset image selected with path %@", (NSString *)[[info objectForKey:UIImagePickerControllerReferenceURL] absoluteString]);
            NSString *filePath = [[self writeToTempDirImageData:picData ofType:iType error:&error] copy];
            if(! error) {
                
                result = [NSDictionary dictionaryWithObjects:[NSArray arrayWithObjects:filePath, base64Data, nil]
                                                     forKeys:[NSArray arrayWithObjects:@"path", @"data", nil]];
                NSDictionary *resultDict = [NSDictionary dictionaryWithObjectsAndKeys:result,@"result",_callbackId,@"callbackId",error,@"error", nil];
                [self performSelectorOnMainThread:@selector(sendJsMessage:) withObject:resultDict waitUntilDone:NO];
            }
        }
    }
    else
    {
        error = [MBLUtils errorWithCode:[NSNumber numberWithInt:0]
                            description:MBLLocalizedString(@"ERROR_READING_PICTURE_DATA")];
      
        NSDictionary *resultDict = [NSDictionary dictionaryWithObjectsAndKeys:result,@"result",_callbackId,@"callbackId",error,@"error", nil];
        [self performSelectorOnMainThread:@selector(sendJsMessage:) withObject:resultDict waitUntilDone:NO];
    }
} // pickImage:

- (void) sendJsMessage:(NSDictionary *) resultDict
{
    @synchronized (self)
    {
        NSDictionary *result = [resultDict valueForKey:@"result"];
        NSString *_callbackId = [resultDict valueForKey:@"callbackId"];
        NSError *error = [resultDict valueForKey:@"error"];
        [self sendMessageToJavascriptWithResult:result orError:error withCallback:_callbackId];
    }
} // sendJsMessage:

#pragma mark -
#pragma mark UIPopOverControllerDelegate methods
- (void)popoverControllerDidDismissPopover:(UIPopoverController *)popoverController
{
    [self imagePickerControllerDidCancel:cameraUI];
}

@end