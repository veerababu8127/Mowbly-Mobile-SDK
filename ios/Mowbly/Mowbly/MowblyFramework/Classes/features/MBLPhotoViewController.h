//
//  MBLPhotoViewController.h
//  Mowbly
//
//  Created by Veerababu on 09/12/13.
//  Copyright (c) 2011-2014 CloudPact. All rights reserved.

#import <UIKit/UIKit.h>

@interface MBLPhotoViewController : UIViewController

+ (MBLPhotoViewController *)photoViewControllerForPageIndex:(NSUInteger)pageIndex withData:(NSMutableDictionary *)dict;

- (NSInteger)pageIndex;

@end
