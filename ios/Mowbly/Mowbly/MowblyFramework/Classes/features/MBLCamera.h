//
//  MBLCamera.h
//  Mowbly
//
//  Created by Sathish on 09/09/11.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
//  Class that bridges the Camera feature of the device to the page.

#import "MBLFeature.h"

typedef enum {

    JPEG = 0,
    PNG = 1
} IMAGE_TYPE;

@interface MBLCamera : MBLFeature <UIImagePickerControllerDelegate, UIPopoverControllerDelegate, UINavigationControllerDelegate> {

	// Options
	bool bReadData;                         // Tells to return the base64 data of the picture
	NSDictionary *options;                  // Options for takePicture and choosePicture

	NSMutableDictionary *photos;            // Array holding the photos taken from Camera
    
    UIPopoverController *popoverUI;         // Holds the popoverController reference
    
    UIImagePickerController *cameraUI;
    
    NSString *callbackId;                   // JS callback id for camera
}

@end