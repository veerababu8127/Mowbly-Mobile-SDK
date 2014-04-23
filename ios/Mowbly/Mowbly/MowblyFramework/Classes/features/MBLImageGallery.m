//
//  MBLImageGallery.m
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.
//
#import <UIKit/UIKit.h>
#import "MBLImageGallery.h"
#import "MBLFeature.h"
#import "CJSONDeserializer.h"
#import "MBLJSMessage.h"
#import "MBLPhotoViewController.h"
#import "MBLAppDelegate.h"
#import "MBLImageGalleryViewController.h"

@implementation MBLImageGallery

- (void) invoke:(MBLJSMessage *)message {
    
    MBLImageGalleryViewController *externalView=[[MBLImageGalleryViewController alloc]initDict:[message.args objectAtIndex:0]];
    MBLAppDelegate *appDelegate = (MBLAppDelegate *)[[UIApplication sharedApplication] delegate];
    [[appDelegate navigationController] pushViewController:externalView animated:NO];
    
}//invoke
@end
